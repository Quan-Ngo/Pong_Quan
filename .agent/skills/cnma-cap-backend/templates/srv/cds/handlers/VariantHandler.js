//M003-#ticket510 (Leo): Refactor naming conventions - files to PascalCase, folders to kebab-case
const readVariantSettings = require('../events/variant/ReadVariantSettings');
const createVariantSettings = require('../events/variant/CreateVariantSettings');
const adjustDefaultVariantSetting = require('../events/variant/AdjustDefaultVariantSetting');

module.exports = (srv) => {
    srv.on("READ", 'VariantSettings', async (req) => {
        return await readVariantSettings.execute(req);
    });

    srv.before("CREATE", 'VariantSettings', async (req) => {
        return await createVariantSettings.execute(req);
    });

    srv.on('adjustDefaultVariantSetting', async (req) => {
        return await adjustDefaultVariantSetting.execute(req);
    });
};
