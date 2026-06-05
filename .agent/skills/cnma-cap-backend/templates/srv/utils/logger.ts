import winston from 'winston';

const { combine, timestamp, json, printf, colorize } = winston.format;

// Custom format for console output in development
const consoleFormat = printf(({ level, message, timestamp, ...metadata }) => {
    let msg = `${timestamp} [${level}] ${message}`;

    if (Object.keys(metadata).length > 0) {
        msg += ` ${JSON.stringify(metadata)}`;
    }

    return msg;
});

// Determine log level from environment
const logLevel = process.env.LOG_LEVEL || 'info';

// Create Winston logger instance
export const logger = winston.createLogger({
    level: logLevel,
    format: combine(
        timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
        json()
    ),
    transports: [
        // Console transport with colorized output for development
        new winston.transports.Console({
            format: combine(
                colorize(),
                timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
                consoleFormat
            ),
        }),
    ],
});

// Helper methods for structured logging
export const log = {
    info: (message: string, meta?: Record<string, any>) => logger.info(message, meta),
    warn: (message: string, meta?: Record<string, any>) => logger.warn(message, meta),
    error: (message: string, meta?: Record<string, any>) => logger.error(message, meta),
    debug: (message: string, meta?: Record<string, any>) => logger.debug(message, meta),
};

export default logger;
