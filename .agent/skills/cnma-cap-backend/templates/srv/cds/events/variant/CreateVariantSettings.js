//M003-#ticket510 (Leo): Refactor naming conventions - files to PascalCase, folders to kebab-case
const cds = require("@sap/cds");

module.exports = {
    execute: async (req) => {
        try {
            let { user } = req;
            const id = user.id;
            // console.log("id: ", id);

            let { variantKey, variantName, workListId, isDefaultVariant } = req.data;
            const conflictCondition = [
                { ref: ['variantKey'] }, '=', { val: variantKey },
                'and',
                { ref: ['variantName'] }, '=', { val: variantName },
                'and',
                { ref: ['workListId'] }, '=', { val: workListId },
                'and',
                { ref: ['createdBy'] }, '=', { val: id }
            ];
            // console.log(`Conflict Condition: ${JSON.stringify(conflictCondition)}`);

            let query = SELECT
                .from('CNMA_ADMINCONFIGURATION_VARIANTSETTINGS')
                .columns('ID', 'variantKey', 'variantName', 'workListId', 'createdBy')
                .where(conflictCondition);

            const results = await cds.run(query);
            if (results && results.length) {
                return req.error(409, "Variant is conflicted");
            }


            if (isDefaultVariant) {
                await cds.run(UPDATE('CNMA_ADMINCONFIGURATION_VARIANTSETTINGS')
                    .set({ isDefaultVariant: false })
                    .where({ createdBy: id })
                );
            }
        } catch (error) {
            console.error('Error in CREATE VariantSettings handler:', error);
            req.error(500, 'An error occurred while create data');
        }
    }
}
