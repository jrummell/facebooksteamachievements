import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import FacebookConfig from "./config/FacebookConfig";

Vue.config.productionTip = false;

import BootstrapVue from "bootstrap-vue";

Vue.use(BootstrapVue);

import { library } from "@fortawesome/fontawesome-svg-core";
import {
    faQuestion,
    faSave,
    faCheck,
    faTimes,
    faSpinner,
    faExclamation
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

library.add(faQuestion, faSave, faCheck, faTimes, faSpinner, faExclamation);

Vue.component("font-awesome-icon", FontAwesomeIcon);

import Vuelidate from "vuelidate";
Vue.use(Vuelidate);

import facebookLogin from "facebook-login-vuejs";

Vue.component(facebookLogin.name, facebookLogin);

import Login from "./components/Login.vue";
import Profile from "./components/Profile.vue";
import HelpButton from "./components/HelpButton.vue";

Vue.component(Login.name, Login);
Vue.component(Profile.name, Profile);
Vue.component(HelpButton.name, HelpButton);

const facebookConfig = new FacebookConfig();

new Vue({
    router,
    store,
    provide: { facebookConfig },
    render: h => h(App)
}).$mount("#app");
