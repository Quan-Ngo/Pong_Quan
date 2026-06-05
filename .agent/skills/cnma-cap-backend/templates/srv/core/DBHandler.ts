import { CommonServiceImpl } from "../core/CommonService";
import { ServiceResponse } from "../model/core/ServiceResponse";
import cds from '@sap/cds';
const { SELECT, INSERT, UPDATE } = cds.ql;
const { HTTP_STATUS } = require('../enum/HttpStatusCodeEnum');

/**
 * DBHandler - Generic CRUD handler for HANA entities.
 * Extends CommonServiceImpl to access i18n and response builders.
 *
 * IMPORTANT PATTERNS:
 * - Always use `cds.tx(cds.context)` for transactions (not `cds.tx(req)`)
 * - Always `tx.commit()` after successful operations to release connection pool
 * - Always `tx.rollback()` in catch blocks
 * - Return ServiceResponse with appropriate HTTP status codes
 */
export class DBHandlerService extends CommonServiceImpl {

    async getData(queryCondition: any, entity: any, options?: { limit?: number, skip?: number }): Promise<ServiceResponse<any>> {
        const tx = cds.tx(cds.context);
        try {
            const selectQuery = SELECT.from(entity);
            if (queryCondition !== "*") {
                selectQuery.where(queryCondition);
            }
            if (options?.limit || options?.skip) {
                selectQuery.limit(options.limit ?? Number.MAX_SAFE_INTEGER, options.skip ?? 0);
            }

            const results = await tx.run(selectQuery);
            await tx.commit();

            if (results && results.length > 0) {
                return new ServiceResponse(HTTP_STATUS.OK, this.getText('SERVICE_RETRIEVED_DATA'), results);
            }
            return new ServiceResponse(HTTP_STATUS.NOT_FOUND, this.getText('SERVICE_NO_DATA_FOUND'), []);
        } catch (error) {
            await tx.rollback().catch((err) => console.error('Rollback error:', err));
            return new ServiceResponse(HTTP_STATUS.INTERNAL_SERVER_ERROR, this.getText('SERVICE_GET_DATA_FAILED_ENTITY', [entity.name]), error);
        }
    }

    async batchCreateEntries(entries: any[], entity: any): Promise<ServiceResponse<any>> {
        const tx = cds.tx(cds.context);
        try {
            const result = await tx.run(INSERT.into(entity).entries(entries));
            await tx.commit();
            return result
                ? new ServiceResponse(HTTP_STATUS.OK, 'Batch create entries successfully')
                : new ServiceResponse(HTTP_STATUS.UNPROCESSABLE_ENTITY, 'Batch create entries failed');
        } catch (error) {
            await tx.rollback().catch((err) => console.error('Rollback failed:', err));
            return new ServiceResponse(HTTP_STATUS.INTERNAL_SERVER_ERROR, 'Batch create entries failed', error.message);
        }
    }
}
