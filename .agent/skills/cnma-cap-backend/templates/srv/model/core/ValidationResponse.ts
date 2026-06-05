import { IApiResponse, IValidationResponseData } from "../../interfaces/ICommon";
import { ApiResponse } from "./ApiResponse";

export class ValidationResponse<T> implements IValidationResponseData<T> {
    valid: boolean;
    message: string;
    data: T | null;

    constructor(valid: boolean, message: string, data: T | null = null) {
        this.valid = valid;
        this.message = message;
        this.data = data;
    }

    toApiResponse(): IApiResponse<T> {
        return new ApiResponse(this.valid, this.message, this.data);
    }
}
