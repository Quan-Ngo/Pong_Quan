import { Request } from '@sap/cds/apis/services';
import { eBTPDestinationServices } from '../../utils/enum/BTPDestinationServices';
import { logger } from '../../utils/logger';
import { BTPServiceLoggingMiddleware } from '../../middlewares/btp-service-logging.middleware';
import { executeHttpRequest, HttpResponse } from '@sap-cloud-sdk/http-client';
import { getDestination, buildHeadersForDestination, Destination } from '@sap-cloud-sdk/connectivity';
import axios from 'axios';
import * as qs from 'qs';

export interface IDestinationCloudService {
    getBTPDestination(destinationName: string): Promise<any | null>;
    sendGetRequest(servicePath: string, requestPath: string): Promise<any>;
    sendPostRequest(servicePath: string, requestPath: string, payload: any): Promise<any>;
    sendPutRequest(servicePath: string, requestPath: string, payload: any): Promise<any>;
    fetchJwtToken(authTokenUrl: string, clientId: string, clientSecret: string): Promise<string>;
    getAuthorizationTokenForSystemUser(destinationName: string): Promise<string>;
}

export interface ServiceResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
    statusCode?: number;
}

/**
 * Base service for SAP BTP Destination-based HTTP operations.
 * Provides common HTTP methods (GET, POST) that leverage SAP Cloud SDK
 * for destination retrieval, authentication, and CSRF token handling.
 */
export class DestinationCloudService implements IDestinationCloudService {
    private destinationName: string;

    constructor(destinationName: string = '') {
        this.destinationName = destinationName;
    }

    getDestinationName(): string {
        return this.destinationName;
    }

    async getBTPDestination(): Promise<any | null> {
        try {
            if (!this.destinationName) {
                logger.warn('Destination name not set');
                return null;
            }
            logger.info('Retrieving BTP destination', { destinationName: this.destinationName });

            const btpDestinationData = await getDestination({
                "destinationName": this.destinationName
            });

            if (!btpDestinationData) {
                logger.error('Destination not found', { destinationName: this.destinationName });
                return null;
            }

            BTPServiceLoggingMiddleware.logDestinationRetrieval(this.destinationName, true);

            // Safe navigation for config
            let btpDestinationConfig = btpDestinationData.originalProperties?.destinationConfiguration ?? null;

            // Fallback for Dev/Test environments where properties might be at root
            if (!btpDestinationConfig && btpDestinationData.originalProperties) {
                btpDestinationConfig = btpDestinationData.originalProperties;
            }

            return {
                destinationData: btpDestinationData,
                destinationConfig: btpDestinationConfig
            };
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : 'Unknown error';
            logger.error('Failed to get destination', { destinationName: this.destinationName, error: errorMessage });
            BTPServiceLoggingMiddleware.logDestinationRetrieval(this.destinationName, false, error as Error);
            return null;
        }
    }

    async configurationDestinationCloud(authToken: string): Promise<void> {
        try {
            // TODO: Implement configuration logic
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : 'Unknown error';
            logger.error('Destination configuration error', { error: errorMessage });
        }
    }

    async fetchJwtToken(authTokenUrl: string, clientId: string, clientSecret: string): Promise<string> {
        try {
            const data = qs.stringify({
                'client_id': clientId,
                'client_secret': clientSecret,
                'grant_type': 'client_credentials',
                'response_type': 'token'
            });

            const config = {
                method: 'post',
                maxBodyLength: Infinity,
                url: authTokenUrl,
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                data: data
            };

            const response = await axios.request(config);
            return response.data.access_token;

        } catch (error) {
            logger.error("JWT token fetch failed", { error: error instanceof Error ? error.message : String(error) });
            throw error;
        }
    }

    async getAuthorizationTokenForSystemUser(destinationName: string): Promise<string> {
        let authToken: string | null = null;

        try {
            const btpDestination = await getDestination({
                "destinationName": destinationName
            });

            if (!btpDestination?.originalProperties?.destinationConfiguration) {
                throw new Error(`Invalid destination configuration for: ${destinationName}`);
            }

            const { clientId, clientSecret, tokenServiceURL, tokenServiceUrl } = btpDestination.originalProperties.destinationConfiguration;
            const finalTokenUrl = tokenServiceURL || tokenServiceUrl; // Handle case variation

            const bodyParams = new URLSearchParams();
            bodyParams.append('grant_type', 'client_credentials');
            bodyParams.append('client_id', clientId);
            bodyParams.append('client_secret', clientSecret);

            if (!finalTokenUrl) throw new Error('Token Service URL missing in destination config');

            const tokenResponse = await fetch(finalTokenUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: bodyParams,
            });

            if (!tokenResponse.ok) {
                throw new Error(`Failed to retrieve auth token: ${tokenResponse.statusText}`);
            }

            const tokenJson = await tokenResponse.json() as { access_token?: string };
            authToken = tokenJson.access_token || null;

            if (!authToken) {
                throw new Error('Failed to retrieve auth token.');
            }

        } catch (error) {
            logger.error('Authorization token error', { error: error instanceof Error ? error.message : String(error) });
            throw error;
        }

        return authToken as string;
    }

    async getCsrfRequestHeaders(destination: Destination): Promise<Record<string, string>> {
        try {
            return await buildHeadersForDestination(destination);
        } catch (error: unknown) {
            const err = error as Error;
            logger.error("Failed to fetch CSRF headers", { error: err.message });
            return {};
        }
    }

    // ─── Private: Unified HTTP Executor ───────────────────────────────

    /**
     * Core HTTP executor – all public send*Request methods delegate here.
     * Handles destination lookup, logging, CSRF, and error normalisation in ONE place.
     *
     * @param {'GET'|'POST'|'PUT'|'PATCH'|'DELETE'} method   HTTP verb
     * @param {string}  servicePath    Base OData service path
     * @param {string}  requestPath    Entity / endpoint path
     * @param {any}  [data]         Request body (ignored for GET)
     * @param {object}  [sdkOptions]   Extra SAP Cloud SDK options (e.g. { fetchCsrfToken: true })
     * @returns {Promise<any>}
     * @private
     */
    protected async _executeRequest(
        method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE',
        servicePath: string,
        requestPath: string,
        data: any = undefined,
        sdkOptions: Record<string, any> = {}
    ): Promise<any> {
        const btpDestination = await this.getBTPDestination();
        if (!btpDestination) throw new Error("Destination not found");

        const { destinationData } = btpDestination;
        const requestEndpoint = `${servicePath}${requestPath}`;

        const callLog = BTPServiceLoggingMiddleware.logServiceCallStart(
            this.destinationName,
            method,
            requestEndpoint,
            data
        );

        try {
            const requestConfig: any = {
                method,
                url: requestEndpoint,
            };
            if (data !== undefined) requestConfig.data = data;

            const response = await executeHttpRequest(destinationData, requestConfig, sdkOptions);

            BTPServiceLoggingMiddleware.logServiceCallSuccess(callLog, response.status, response.data);
            return response;
        } catch (error: any) {
            BTPServiceLoggingMiddleware.logServiceCallError(callLog, error, error.response?.status);
            throw error;
        }
    }

    // ─── Public API (thin wrappers) ──────────────────────────────────

    /**
     * Execute a GET request using SAP Cloud SDK.
     * @param servicePath Base path for the service
     * @param requestPath Specific endpoint path
     */
    async sendGetRequest(servicePath: string, requestPath: string): Promise<any> {
        return this._executeRequest('GET', servicePath, requestPath);
    }

    /**
     * Execute a POST request using SAP Cloud SDK.
     * @param servicePath Base path for the service
     * @param requestPath Specific endpoint path
     * @param payload Request body data
     */
    async sendPostRequest(servicePath: string, requestPath: string, payload: any): Promise<any> {
        return this._executeRequest('POST', servicePath, requestPath, payload, { fetchCsrfToken: true });
    }

    /**
     * Execute a PUT request using SAP Cloud SDK.
     * @param servicePath Base path for the service
     * @param requestPath Specific endpoint path
     * @param payload Request body data
     */
    async sendPutRequest(servicePath: string, requestPath: string, payload: any): Promise<any> {
        return this._executeRequest('PUT', servicePath, requestPath, payload, { fetchCsrfToken: true });
    }
}
