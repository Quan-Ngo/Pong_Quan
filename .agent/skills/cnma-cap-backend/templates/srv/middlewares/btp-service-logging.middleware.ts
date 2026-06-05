import { logger } from '../utils/logger';
import type { AxiosRequestConfig, AxiosResponse } from 'axios';

/**
 * BTP Service Call Logging Middleware
 * Tracks HTTP calls to SAP BTP services with structured logging
 */

export interface ServiceCallLog {
    destinationName: string;
    method: string;
    url: string;
    startTime: number;
    endTime?: number;
    duration?: number;
    statusCode?: number;
    success?: boolean;
    error?: string;
}

export class BTPServiceLoggingMiddleware {
    /**
     * Log BTP service call start
     */
    static logServiceCallStart(
        destinationName: string,
        method: string,
        url: string,
        data?: any
    ): ServiceCallLog {
        const startTime = Date.now();

        logger.info('BTP service call started', {
            destinationName,
            method,
            url,
            hasData: !!data,
            dataSize: data ? JSON.stringify(data).length : 0,
            timestamp: new Date().toISOString(),
        });

        return {
            destinationName,
            method,
            url,
            startTime,
        };
    }

    /**
     * Log successful BTP service call
     */
    static logServiceCallSuccess(
        callLog: ServiceCallLog,
        statusCode: number,
        responseData?: any
    ): void {
        const endTime = Date.now();
        const duration = endTime - callLog.startTime;

        logger.info('BTP service call completed', {
            destinationName: callLog.destinationName,
            method: callLog.method,
            url: callLog.url,
            statusCode,
            duration: `${duration}ms`,
            success: true,
            responseSize: responseData ? JSON.stringify(responseData).length : 0,
            timestamp: new Date().toISOString(),
        });
    }

    /**
     * Log failed BTP service call
     */
    static logServiceCallError(
        callLog: ServiceCallLog,
        error: Error,
        statusCode?: number
    ): void {
        const endTime = Date.now();
        const duration = endTime - callLog.startTime;

        logger.error('BTP service call failed', {
            destinationName: callLog.destinationName,
            method: callLog.method,
            url: callLog.url,
            statusCode,
            duration: `${duration}ms`,
            success: false,
            error: error.message,
            errorStack: error.stack,
            timestamp: new Date().toISOString(),
        });
    }

    /**
     * Create Axios request interceptor
     */
    static createRequestInterceptor(destinationName: string) {
        return (config: AxiosRequestConfig): AxiosRequestConfig => {
            // Store start time in config for duration calculation
            (config as any).__startTime = Date.now();
            (config as any).__destinationName = destinationName;

            logger.debug('BTP request intercepted', {
                destinationName,
                method: config.method?.toUpperCase(),
                url: config.url,
                headers: config.headers,
            });

            return config;
        };
    }

    /**
     * Create Axios response interceptor
     */
    static createResponseInterceptor(destinationName: string) {
        return {
            onFulfilled: (response: AxiosResponse): AxiosResponse => {
                const startTime = (response.config as any).__startTime || Date.now();
                const duration = Date.now() - startTime;

                logger.info('BTP response received', {
                    destinationName: (response.config as any).__destinationName || destinationName,
                    method: response.config.method?.toUpperCase(),
                    url: response.config.url,
                    statusCode: response.status,
                    duration: `${duration}ms`,
                    responseSize: JSON.stringify(response.data).length,
                });

                return response;
            },
            onRejected: (error: any): Promise<any> => {
                const startTime = (error.config as any).__startTime || Date.now();
                const duration = Date.now() - startTime;

                logger.error('BTP request failed', {
                    destinationName: (error.config as any).__destinationName || destinationName,
                    method: error.config?.method?.toUpperCase(),
                    url: error.config?.url,
                    statusCode: error.response?.status,
                    duration: `${duration}ms`,
                    error: error.message,
                    errorCode: error.code,
                });

                return Promise.reject(error);
            },
        };
    }

    /**
     * Log destination retrieval
     */
    static logDestinationRetrieval(destinationName: string, success: boolean, error?: Error): void {
        if (success) {
            logger.info('BTP destination retrieved', {
                destinationName,
                timestamp: new Date().toISOString(),
            });
        } else {
            logger.error('BTP destination retrieval failed', {
                destinationName,
                error: error?.message,
                timestamp: new Date().toISOString(),
            });
        }
    }
}

export default BTPServiceLoggingMiddleware;
