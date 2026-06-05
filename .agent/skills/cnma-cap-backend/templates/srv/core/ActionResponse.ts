interface SuccessResponse<T> {
    success: true;
    message: string;
    data?: T;
}

interface ErrorResponse {
    success: false;
    message: string;
}

export default class ActionResponse {
    static ok<T>(message: string, data?: T): SuccessResponse<T> {
        return {
            success: true,
            message: message,
            data: data
        };
    }

    static error(message: string): ErrorResponse {
        return {
            success: false,
            message: message
        };
    }
}
