
import cds from '@sap/cds';
import { ApiResponse } from '../../model/core/ApiResponse';

module.exports = async (srv: any) => {
    const { MyEntity } = srv.entities;

    // READ Handler
    srv.on('READ', MyEntity, async (req: any) => {
        try {
            // Custom Logic
            const result = await cds.run(SELECT.from(MyEntity));
            return result;
        } catch (error) {
            req.error(500, 'Error reading entity');
        }
    });

    // Custom Action Handler
    srv.on('myCustomAction', async (req: any) => {
        try {
            const { param } = req.data;
            // Business Logic
            return new ApiResponse(true, `Action executed with ${param}`);
        } catch (error) {
            return new ApiResponse(false, error.message);
        }
    });
};
