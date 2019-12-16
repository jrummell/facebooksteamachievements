import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

Vue.config.productionTip = false;

import BootstrapVue from "bootstrap-vue";

Vue.use(BootstrapVue);

import { library } from "@fortawesome/fontawesome-svg-core";
import { faQuestion, faSave, faCheck, faTimes, faSpinner, faExclamation } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

library.add(faQuestion, faSave, faCheck, faTimes, faSpinner, faExclamation);

Vue.component("font-awesome-icon", FontAwesomeIcon);

import facebookLogin from "facebook-login-vuejs";

Vue.component(facebookLogin.name, facebookLogin);

import Login from "./components/Login.vue";
import Profile from "./components/Profile.vue";
import HelpButton from "./components/HelpButton.vue";
import LoadingIndicator from "./components/LoadingIndicator.vue";

// Note: the explicit html tag name must be used here or this will not work in a production build
Vue.component("login", Login);
Vue.component("profile", Profile);
Vue.component("help-button", HelpButton);
Vue.component("loading-indicator", LoadingIndicator);

import RestClient from "./helpers/RestClient";
const restClient = new RestClient();

new Vue({
    router,
    store,
    provide: { restClient },
    render: h => h(App)
}).$mount("#app");
