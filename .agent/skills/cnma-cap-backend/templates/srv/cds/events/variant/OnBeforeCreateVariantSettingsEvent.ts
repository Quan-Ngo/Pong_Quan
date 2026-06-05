//M003-#ticket510 (Leo): Refactor naming conventions - files to PascalCase, folders to kebab-case
import * as cds from '@sap/cds';

export class OnBeforeCreateVariantSettingsEvent {
    static async execute(req: cds.Request): Promise<void> {
        try {
            const user = req.user as any;
            const id = user.id;
            // console.log("id: ", id);

            const { variantKey, variantName, workListId, isDefaultVariant } = req.data as any;
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

            const query = SELECT
                .from('CNMA_ADMINCONFIGURATION_VARIANTSETTINGS')
                .columns('ID', 'variantKey', 'variantName', 'workListId', 'createdBy')
                .where(conflictCondition);

            const results = await cds.run(query);
            if (results && (results as any).length) {
                req.error(409, "Variant is conflicted");
                return;
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
