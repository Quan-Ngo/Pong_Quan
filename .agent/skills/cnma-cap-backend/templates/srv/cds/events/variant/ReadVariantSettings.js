//M003-#ticket510 (Leo): Refactor naming conventions - files to PascalCase, folders to kebab-case
const cds = require("@sap/cds");

module.exports = {
    execute: async (req) => {
        try {
            let { user, query } = req;
            const createById = user.id;

            console.log("Read user id: ", createById);

            // Extract the query conditions from the incoming request
            let reqConditions = query.SELECT.where || [];

            // Predefined conditions
            const predefinedConditions = [
                // { ref: ['isGlobalVariant'] }, '=', { val: true },
                // 'and',
                { ref: ['createdBy'] }, '=', { val: createById }
            ];

            // Combine the request conditions with the predefined condition
            let combinedCondition;
            if (reqConditions.length > 0) {
                combinedCondition = ['(', ...reqConditions, ')', 'and', ...predefinedConditions];
            } else {
                combinedCondition = predefinedConditions;
            }
            console.log(`Condition: ${JSON.stringify(combinedCondition)}`);

            // Create the final query with combined conditions
            // Note: Table name matches standard CAP persistence naming: NAMESPACE_ENTITY
            // Adjust 'CNMA_ADMINCONFIGURATION_VARIANTSETTINGS' if your namespace differs
            // Ideally should be dynamic based on the entity definition but hardcoded for SQL performance in template
            // Assuming namespace 'cnma.adminconfiguration' -> 'CNMA_ADMINCONFIGURATION_VARIANTSETTINGS'
            // IF using generic 'db.schema' namespace -> 'DB_SCHEMA_VARIANTSETTINGS'

            // For now, let's use dynamic reflection if possible or just standard CAP SELECT
            // With CAP, we can query by Entity Name directly if registered
            // But here we use raw table name or Entity definition if available

            // Standard Approach: use req.target
            // But the original code used hardcoded table name. Let's try to stick to CDS API

            // Using raw CDS query to persist original logic
            let selectQuery = SELECT
                .from('CNMA_ADMINCONFIGURATION_VARIANTSETTINGS')
                .columns('ID', 'variantKey', 'variantName', 'workListId', 'createdBy', 'isDefaultVariant', 'isGlobalVariant', 'filterBarVariant', 'filterDataVariant')
                .where(combinedCondition);

            let results = await cds.run(selectQuery);
            // console.log("READ results: ", JSON.stringify(results));
            return results;
        } catch (error) {
            console.error('Error in READ handler:', error);
            req.error(500, 'An error occurred while fetching data');
        }
    }
}
