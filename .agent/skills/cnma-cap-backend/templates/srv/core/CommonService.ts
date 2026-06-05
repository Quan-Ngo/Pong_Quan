import { ICommonService, IServiceResponse, IValidationResponseData, IApiResponse } from '../interfaces/ICommon';
const { HTTP_SUCCES_STATUSES } = require('../enum/HttpStatusCodeEnum');
const { getText } = require('./CnmaTextHandler');

/**
 * CommonServiceImpl - Base class for all services.
 * Provides: i18n text retrieval, response builders, managed field setters.
 *
 * EXTEND THIS CLASS for any new service that needs:
 * - Localized messages (getText)
 * - Standardized response formatting (buildServiceResponse, buildApiResponse)
 * - Audit fields (setCreateManaged, setUpdateManaged)
 */
export class CommonServiceImpl implements ICommonService {

    getText(textKey: string, params?: string[]): string {
        return getText(textKey, params);
    }

    buildServiceResponse<T>(statusCode: number, message: string, data?: T): IServiceResponse<T> {
        const success = HTTP_SUCCES_STATUSES.includes(statusCode);
        return {
            statusCode, success, message, data: data || null,
            toApiResponse() {
                return { success: this.success, message: this.message, data: this.data };
            }
        };
    }

    buildValidationResponse<T>(valid: boolean, message: string): IValidationResponseData<T> {
        return {
            valid, message,
            toApiResponse() {
                return { success: this.valid, message: this.message, data: null };
            }
        };
    }

    setCreateManaged(data: any, owner: string): void {
        data.createdAt = new Date().toISOString();
        data.createdBy = owner;
        data.modifiedAt = new Date().toISOString();
        data.modifiedBy = owner;
    }

    setUpdateManaged(data: any, owner: string): void {
        data.modifiedAt = new Date().toISOString();
        data.modifiedBy = owner;
    }
}
