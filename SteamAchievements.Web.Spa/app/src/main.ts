import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";

//import BootstrapVue from "bootstrap-vue";

import { library } from "@fortawesome/fontawesome-svg-core";
import { faQuestion, faSave, faCheck, faTimes, faSpinner, faExclamation } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

library.add(faQuestion, faSave, faCheck, faTimes, faSpinner, faExclamation);

import facebookLogin from "facebook-login-vuejs";

import HelpButton from "./components/HelpButton.vue";
import LoadingIndicator from "./components/LoadingIndicator.vue";

import RestClient from "./helpers/RestClient";
const restClient = new RestClient();

const app = createApp(App)
    .provide("restClient", restClient)
    .use(store)
    .use(router);

//app.use(BootstrapVue);
app.component("facebook-login", facebookLogin);
app.component("font-awesome-icon", FontAwesomeIcon);
app.component("help-button", HelpButton);
app.component("loading-indicator", LoadingIndicator);

app.mount("#app");
