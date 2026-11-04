'use strict';
const assert = require('assert');
const fs = require('fs');
const path = require('path');
const vm = require('vm');
const code = fs.readFileSync(path.join(__dirname, '../../VisitorManagment.Web/wwwroot/js/maintenance-status.js'), 'utf8');
const flush = () => new Promise(resolve => setImmediate(resolve));
function client(fetcher, hasConfig = true) {
    const events = {};
    const redirects = [];
    const timers = [];
    const config = { dataset: { statusUrl: '/omid/MaintenanceStatus', maintenanceUrl: '/omid/UpdateSamane' } };
    const window = { addEventListener(name, fn) { events[name] = fn; }, setInterval(fn) { timers.push(fn); }, location: { replace(url) { redirects.push(url); } } };
    vm.runInNewContext(code, { window, document: { getElementById() { return hasConfig ? config : null; } }, fetch: fetcher });
    return { window, events, redirects, timers };
}
(async function () {
    let resolve;
    let calls = 0;
    const current = client((url, options) => {
        calls++;
        assert.strictEqual(url, '/omid/MaintenanceStatus');
        assert.strictEqual(options.cache, 'no-store');
        return new Promise(done => { resolve = done; });
    });
    current.events.focus(); current.events.pageshow();
    assert.strictEqual(calls, 1, 'parallel checks must not overlap');
    resolve({ ok: true, json: async () => ({ blocked: true, maintenanceUrl: 'https://untrusted.example' }) });
    await flush();
    assert.deepStrictEqual(current.redirects, ['/omid/UpdateSamane'], 'use the local configured destination and release the call page');
    current.timers[0]();
    assert.strictEqual(calls, 1, 'navigation must not cause a request storm');

    const developer = client(async () => ({ ok: true, json: async () => ({ blocked: false }) }));
    await flush();
    assert.deepStrictEqual(developer.redirects, [], 'developer and normal mode must stay on the current page');

    let offlineCalls = 0;
    const offline = client(() => { offlineCalls++; return Promise.reject(new Error('offline')); });
    await flush(); offline.timers[0](); await flush();
    assert.strictEqual(offlineCalls, 2, 'network failure must allow the next status check');
    assert.deepStrictEqual(offline.redirects, [], 'network failure is not a maintenance command');

    const failure = client(async () => ({ ok: false }));
    await flush();
    assert.deepStrictEqual(failure.redirects, [], 'HTTP errors must not be interpreted as disabled/enabled state');
    client(() => { throw new Error('should not fetch without configuration'); }, false);
    console.log('Maintenance browser-client checks passed. No real browser or camera was used.');
})().catch(error => { console.error(error); process.exitCode = 1; });
