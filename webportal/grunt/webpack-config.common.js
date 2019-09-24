const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = {
    entry: {
        app: [`${__dirname}/../src/app/index`],
        main: [`${__dirname}/../src/app/styles/main.less`],
    },
    resolve: {
        // Important! Do not remove ''. If you do, imports without
        // an extension won't work anymore!
        // -> Changed to '* because should be better
        extensions: ['*', '.js', '.jsx'],
    },
    module: {
        rules: [
            {
                test: /\.(ttf|eot|woff|woff2|svg)/,
                loader: 'file-loader',
            },
            {
                test: /\.jsx?$/,
                exclude: /node_modules/,
                use: [
                    {
                        loader: 'babel-loader',
                        options: {
                            presets: ['react']
                        }
                    }
                ],
            }
        ],
    },
    plugins: [
        new ExtractTextPlugin('[name].css'),
    ],
};
