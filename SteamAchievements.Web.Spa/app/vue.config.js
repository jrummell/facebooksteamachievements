module.exports = {
    outputDir: "build",
    productionSourceMap: true,
    chainWebpack: config => {
        config.externals([
            {
                vue: "Vue",
                bootstrap: "Bootstrap",
                "bootstrap-vue": "BootstrapVue"
            }
        ]);
    }
};
