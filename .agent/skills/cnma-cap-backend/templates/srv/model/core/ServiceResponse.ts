import { IApiResponse, IServiceResponse } from "../../interfaces/ICommon";
import { ApiResponse } from "./ApiResponse";
const { HTTP_SUCCES_STATUSES } = require('../../enum/HttpStatusCodeEnum');

export class ServiceResponse<T> implements IServiceResponse<T> {
    statusCode: number;
    success: boolean;
    message: string;
    data: T | null;

    constructor(statusCode: number, message: string, data?: T | null) {
        const success = HTTP_SUCCES_STATUSES.includes(statusCode);
        this.statusCode = statusCode;
        this.success = success;
        this.message = message;
        this.data = data;
    }

    toApiResponse(): IApiResponse<T> {
        return new ApiResponse(this.success, this.message, this.data);
    }
}
