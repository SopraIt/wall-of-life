const merge = require('webpack-merge');
const devConfig = require('./webpack-config.dev');
const prodConfig = require('./webpack-config.prod');

module.exports = {
    dev: merge(devConfig, {}),
    prod: merge(prodConfig, {}),
};
