var path = require('path');
var webpack = require('webpack');
var CommonsChunkPlugin = require("./node_modules/webpack/lib/optimize/CommonsChunkPlugin");

module.exports = {
    entry: {
        app: [// Set up an ES6-ish environment
            'babel-polyfill',
            // Add your application's scripts below
            './Scripts/compiled/app',
            './Scripts/compiled/AchievementService'
        ],
        games: ['./Scripts/compiled/games'],
        publish: ['./Scripts/compiled/publish'],
        settings: ['./Scripts/compiled/settings']
    },
    output: {
        path: path.join(__dirname, "Scripts/dist"),
        filename: "[name].bundle.js",
        chunkFilename: "[id].chunk.js"
    },
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
        new CommonsChunkPlugin({
            filename: "shared.js",
            name: "shared"
        })
    ]
}