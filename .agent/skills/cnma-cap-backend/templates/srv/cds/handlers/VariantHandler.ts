//M003-#ticket510 (Leo): Refactor naming conventions - files to PascalCase, folders to kebab-case
//M008-#ticket510 (AGSAP): Rename events to OnBefore/OnAfter/OnAction convention
import * as cds from '@sap/cds';
import { OnReadVariantSettingsEvent } from '../events/variant/OnReadVariantSettingsEvent';
import { OnBeforeCreateVariantSettingsEvent } from '../events/variant/OnBeforeCreateVariantSettingsEvent';
import { OnAdjustDefaultVariantSettingAction } from '../actions/OnAdjustDefaultVariantSettingAction';

export = (srv: cds.ApplicationService) => {
    srv.on("READ", 'VariantSettings', async (req) => {
        return await OnReadVariantSettingsEvent.execute(req);
    });

    srv.before("CREATE", 'VariantSettings', async (req) => {
        return await OnBeforeCreateVariantSettingsEvent.execute(req);
    });

    srv.on('adjustDefaultVariantSetting', async (req) => {
        return await OnAdjustDefaultVariantSettingAction.execute(req);
    });
};
