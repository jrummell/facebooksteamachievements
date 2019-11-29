import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import FacebookConfig from "./config/FacebookConfig";

Vue.config.productionTip = false;

import facebookLogin from "facebook-login-vuejs";

import Login from "./components/Login.vue";

Vue.component(Login.name, Login);
Vue.component(facebookLogin.name, facebookLogin);

const facebookConfig = new FacebookConfig();

new Vue({
    router,
    store,
    provide: { facebookConfig },
    render: h => h(App)
}).$mount("#app");
