module.exports = {
    entry: {
        'polyfills': './src/polyfills.ts',
        /*'vendor': './src/vendor.ts',*/
        'app': './src/main.ts'
    },

    resolve: {
        extensions: ['.ts', '.js']
    },

    module: {
        rules: [
            {
                test: /\.ts$/,
                use: [
                    {
                        loader: 'awesome-typescript-loader',
                        options: {
                            configFileName: helpers.root('', 'tsconfig.json')
                        }
                    },
                    {
                        loader: 'angular2-template-loader'
                    }
                ]
            },
            {
                test: /\.html$/,
                use: [{
                    loader: 'html-loader',
                    options: {
                        minimize: false,
                        removeComments: false,
                        collapseWhitespace: false
                    }
                }]
            },
            {
                test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
                loader: 'file-loader?name=dist/assets/[name].[hash].[ext]'
            },
            {
                test: /\.css$/,
                exclude: helpers.root('app'),
                loader: ExtractTextPlugin.extract({
                    fallback: 'style-loader',
                    use: 'css-loader?sourceMap'
                })
            },
            {
                test: /\.css$/,
                include: helpers.root('app'),
                loader: 'raw-loader'
            }
        ]
    },

    plugins: [
        new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery' }), // Maps these identifiers to the jQuery package (because Bootstrap expects it to be a global variable)
        // Workaround for angular/angular#11580 for angular v4
        new webpack.ContextReplacementPlugin(
            /angular(\\|\/)core(\\|\/)@angular/,
            helpers.root('./app'), // location of your src
            {} // a map of your routes
        ),

        new webpack.optimize.CommonsChunkPlugin({
            //order is important: 
            //The CommonsChunkPlugin identifies the hierarchy among three chunks: app -> vendor -> polyfills. 
            //Where Webpack finds that app has shared dependencies with vendor, it removes them from app. 
            //It would remove polyfills from vendor if they shared dependencies, which they don't.
            name: ['app', 'vendor', 'polyfills']
        }),
    ]
};