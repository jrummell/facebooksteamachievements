module.exports = {
    outputDir: "build",
    productionSourceMap: true,
    chainWebpack: config => {
        config.externals({
            vue: "Vue",
            bootstrap: "Bootstrap",
            "bootstrap-vue": "BootstrapVue",
            "vue-class-component": "VueClassComponent",
            //TODO: "vue-property-decorator": "window['vue-property-decorator']",
            vuex: "Vuex"
        });
    }
};
