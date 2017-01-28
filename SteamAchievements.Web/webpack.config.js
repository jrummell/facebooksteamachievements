var path = require('path');
var webpack = require('webpack');

module.exports = {
    entry: [
        // Set up an ES6-ish environment
        'babel-polyfill',
        // Add your application's scripts below
        './Scripts/compiled/app',
        './Scripts/compiled/AchievementService',
        './Scripts/compiled/facebook',
        './Scripts/compiled/games',
        './Scripts/compiled/publish',
        './Scripts/compiled/settings'
    ],
    output: {
        publicPath: '/Scripts/dist',
        filename: './Scripts/dist/all.js'
    },
    debug: true,
    devtool: 'source-map',
    module: {
        loaders: [
            {
                test: /\.js$/,
                include: path.resolve(__dirname, 'Scripts/compiled'),
                loader: 'babel-loader',
                query: {
                    presets: ['es2015']
                }
            }
        ]
    },
    plugins: [
        //new webpack.optimize.UglifyJsPlugin()
    ]
}