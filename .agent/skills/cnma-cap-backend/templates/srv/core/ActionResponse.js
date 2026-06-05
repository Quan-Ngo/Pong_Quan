module.exports = class ActionResponse {
    static ok(message, data) {
        return {
            success: true,
            message: message,
            data: data
        };
    }

    static error(message) {
        return {
            success: false,
            message: message
        };
    }
};
