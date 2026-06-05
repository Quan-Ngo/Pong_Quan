/**
 * Sample CDS Action Handler Template
 * Convention: On{ActionName}Action.ts
 * Location: srv/cds/actions/
 */
import * as cds from '@sap/cds';
import ActionResponse from '../../core/ActionResponse';
// import { JSONUtils } from '../../utils/JSONUtils';
// import { Logger } from '../../utils/logger';

export class OnSampleAction {
    /**
     * Executes the custom action logic.
     * @param {cds.Request} req - The CDS request object
     * @returns {Promise<any>} ActionResponse envelope
     */
    static async execute(req: cds.Request): Promise<any> {
        try {
            // Extract parameters from req.data
            const { param1, param2 } = req.data as any;

            // TODO: Implement your business logic here
            // const result = await someService.doSomething(param1);

            // Return success response
            return ActionResponse.ok('Action executed successfully', {
                detail: `Processed ${param1}`
            });

        } catch (err: any) {
            // Logger.error(`[OnSampleAction] Error: ${err.message}`);
            return ActionResponse.error(`Failed to execute action: ${err.message}`);
        }
    }
}
