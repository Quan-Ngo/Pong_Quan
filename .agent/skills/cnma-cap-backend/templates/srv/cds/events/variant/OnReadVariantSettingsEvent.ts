//M003-#ticket510 (Leo): Refactor naming conventions - files to PascalCase, folders to kebab-case
import * as cds from '@sap/cds';

export class OnReadVariantSettingsEvent {
    static async execute(req: cds.Request): Promise<any> {
        try {
            const user = req.user as any;
            const queryRaw = req.query as any;
            const createById = user.id;

            console.log("Read user id: ", createById);

            // Extract the query conditions from the incoming request
            let reqConditions = queryRaw.SELECT.where || [];

            // Predefined conditions
            const predefinedConditions = [
                // { ref: ['isGlobalVariant'] }, '=', { val: true },
                // 'and',
                { ref: ['createdBy'] }, '=', { val: createById }
            ];

            // Combine the request conditions with the predefined condition
            let combinedCondition: any[];
            if (reqConditions.length > 0) {
                combinedCondition = ['(', ...reqConditions, ')', 'and', ...predefinedConditions];
            } else {
                combinedCondition = predefinedConditions;
            }
            console.log(`Condition: ${JSON.stringify(combinedCondition)}`);

            // Using raw CDS query to persist original logic
            const selectQuery = SELECT
                .from('CNMA_ADMINCONFIGURATION_VARIANTSETTINGS')
                .columns('ID', 'variantKey', 'variantName', 'workListId', 'createdBy', 'isDefaultVariant', 'isGlobalVariant', 'filterBarVariant', 'filterDataVariant')
                .where(combinedCondition);

            const results = await cds.run(selectQuery);
            // console.log("READ results: ", JSON.stringify(results));
            return results;
        } catch (error) {
            console.error('Error in READ handler:', error);
            req.error(500, 'An error occurred while fetching data');
        }
    }
}
