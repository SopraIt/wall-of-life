const webpack = require('webpack');
const merge = require('webpack-merge');
const commonConfig = require('./webpack-config.common');
const autoprefixer = require('autoprefixer');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = merge(commonConfig, {
    output: {
        path: `${__dirname}/../electron_application_builder/public/builds`,
        filename: '[name].min.js',
    },
    module: {
        rules: [
            {
                test: /\.(less|css)$/,
                loader: ExtractTextPlugin.extract([
                    {
                        loader: 'css-loader',
                        options: {
                            minimize: true,
                        },
                    },
                    {
                        loader: 'postcss-loader',
                        options: {
                            plugins: () => [autoprefixer({
                                browsers: [
                                    '> 1%',
                                    'last 2 versions',
                                ],
                            })],
                        },
                    }, 'less-loader']),
            },
        ],
    },
    plugins: [
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: JSON.stringify('production'),
            },
        }),
        new webpack.optimize.UglifyJsPlugin({
            include: /\.min\.js$/,
            minimize: true,
            compress: {
                warnings: false,
            },
        }),
    ],
});
