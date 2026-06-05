import { IApiResponse, IServiceResponse } from "../../interfaces/ICommon";

export class ApiResponse<T> implements IApiResponse<T> {
    success: boolean;
    message: string;
    data: T | null;

    constructor(success: boolean, message: string, data?: T | null) {
        this.success = success;
        this.message = message;
        this.data = data ?? null;
    }
}
