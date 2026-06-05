//M003-#ticket510 (Leo): Refactor naming conventions - files to PascalCase, folders to kebab-case
//M003-#ticket510 (Leo): Use ActionResponse class for standardized OData action responses
const cds = require("@sap/cds");
const ActionResponse = require('../../../core/ActionResponse');

module.exports = {
    execute: async (req) => {
        // console.log("adjustDefaultVariantSetting payload: ", req.data);
        let {
            clearAllDefault,
            uuid,
            email
        } = req.data;

        let { user } = req;
        const id = user.id;
        // console.log("adjustDefaultVariantSetting id: ", id);

        if (!email) email = id;
        // console.log("adjustDefaultVariantSetting email: ", email);

        //1. clear all default variant of this user first
        try {
            await cds.run(UPDATE('CNMA_ADMINCONFIGURATION_VARIANTSETTINGS')
                .set({ isDefaultVariant: false })
                .where({ createdBy: email })
            );
        } catch (error) {
            return ActionResponse.error('Failure when clear all default variant for user');
        };

        //2.
        if (clearAllDefault) {
            return ActionResponse.ok('Adjust Variant Setting success');
        }

        //3.
        if (!uuid) {
            return ActionResponse.error('Require provide uuid of variant setting to set it as default');
        }

        //4. 
        let query = SELECT.from('CNMA_ADMINCONFIGURATION_VARIANTSETTINGS')
            .columns('variantKey', 'variantName', 'workListId', 'isDefaultVariant', 'isGlobalVariant', 'filterBarVariant', 'filterDataVariant', 'tableColumnVariant', 'tableSortVariant', 'tableGroupVariant')
            .where({ ID: uuid });
        let results;
        try {
            results = await cds.run(query);
            // console.log("Result", JSON.stringify(results));
        } catch (error) {
            return ActionResponse.error('Variant is not found');
        }

        //5.
        try {
            await cds.run(UPDATE('CNMA_ADMINCONFIGURATION_VARIANTSETTINGS')
                .set({ isDefaultVariant: true })
                .where({ ID: uuid })
            );
            return ActionResponse.ok('Adjust Variant Setting success');
        } catch (error) {
            return ActionResponse.error('Failure when set variant as default');
        };
    }
}
