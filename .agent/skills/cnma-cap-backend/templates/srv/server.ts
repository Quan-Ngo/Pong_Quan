import express from 'express';
import cors from 'cors';
import cds from '@sap/cds';
import path from 'path';
import dotenv from 'dotenv';

/**
 * Custom CAP Bootstrap Server
 *
 * This file hooks into CDS `cds.on('bootstrap')` to:
 * 1. Add custom Express middleware (CORS, auth, logging)
 * 2. Register non-CDS REST routes (health checks, public APIs)
 * 3. Setup scheduled jobs (cron)
 * 4. Configure authentication (XSUAA / Passport)
 *
 * IMPORTANT: This file is referenced from package.json `"main": "srv/server.js"`
 * After TypeScript build, it becomes `gen/srv/server.js`.
 */

// Load environment variables for local development
dotenv.config({ path: path.resolve(__dirname, '../.env') });

cds.on('bootstrap', async (app: express.Application) => {
    // ===== MIDDLEWARE =====
    app.use(cors());
    app.use(express.json({ limit: '50mb' }));
    app.use(express.urlencoded({ extended: true }));

    // ===== AUTHENTICATION (BTP XSUAA) =====
    // Uncomment for production:
    // import passport from "passport";
    // const { JWTStrategy } = require("@sap/xssec").v3;
    // const xsenv = require("@sap/xsenv");
    // const services = xsenv.getServices({ uaa: { tag: "xsuaa" } });
    // passport.use("JWT", new JWTStrategy(services.uaa));
    // app.use(passport.initialize());
    // app.use(passport.authenticate("JWT", { session: false }));

    // ===== CUSTOM ROUTES =====
    // Health check endpoint
    app.get('/health', (req, res) => {
        res.status(200).json({ status: 'ok', timestamp: new Date().toISOString() });
    });

    // Custom API routes (non-OData)
    // const ManageRouter = require('./src/ManageRouter');
    // app.use('/api/custom', ManageRouter);

    // ===== LOGGING =====
    console.log(`[Bootstrap] Custom server initialized`);
});

// CDS server started hook
cds.on('served', async () => {
    console.log(`[Server] All CDS services served successfully`);

    // ===== CRON JOBS =====
    // Schedule background tasks here
    // const CronService = require('./src/services/CronServiceImpl');
    // CronService.start();
});

module.exports = cds.server;
