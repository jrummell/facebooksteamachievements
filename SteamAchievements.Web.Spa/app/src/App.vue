<template>
    <div id="app">
        <div class="header">
            <router-link to="/">
                <img src="/images/banner-light.png" class="img-responsive" alt="Steam Achievements" />
            </router-link>
        </div>
        <div v-if="loaded">
            <div v-if="loggedIn">
                <b-nav tabs>
                    <b-nav-item>
                        <router-link to="/">{{ resources.menuHome }}</router-link>
                    </b-nav-item>
                    <b-nav-item>
                        <router-link to="/games">{{ resources.menuGames }}</router-link>
                    </b-nav-item>
                    <b-nav-item>
                        <router-link to="/settings">{{ resources.menuSettings }}</router-link>
                    </b-nav-item>
                    <b-nav-item :href="helpConfig.helpUrl" target="_blank">{{ resources.menuHelp }}</b-nav-item>
                </b-nav>
                <profile></profile>
            </div>

            <div v-else>
                <login></login>
            </div>
        </div>
        <div v-else>
            <font-awesome-icon icon="spinner" spin size="2x"></font-awesome-icon>
        </div>

        <router-view />
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { MutationPayload } from "vuex";
import { AppState } from "./store/index";
import IResources from "./models/IResources";
import HelpConfig from "./config/HelpConfig";
import { Inject } from "vue-property-decorator";
import RestClient from "./helpers/RestClient";

@Component
export default class App extends Vue {
    @Inject()
    restClient: RestClient;

    helpConfig: HelpConfig = new HelpConfig();

    loggedIn: boolean = false;
    loaded: boolean = false;
    resources: IResources | null = null;

    mounted() {
        this.loggedIn = this.$store.state.user != undefined;

        this.$store.subscribe((mutation: MutationPayload, state: AppState) => {
            if (mutation.type == "setUser") {
                this.loggedIn = state.user != undefined;

                //TODO: route to settings page if steamUserId is null
            }
        });

        this.getResources();
    }

    async getResources(): Promise<void> {
        if (!(await this.authorize())) {
            return;
        }

        this.resources = await this.restClient.getJson("/api/Resource");

        this.$store.commit("setResources", this.resources);

        this.loaded = true;
    }

    async authorize(): Promise<boolean> {
        const apiSettings = this.$store.state.apiSettings;
        const body = `grant_type=client_credentials&client_id=${apiSettings.clientId}&client_secret=${apiSettings.clientSecret}`;

        // https://gomakethings.com/using-oauth-with-fetch-in-vanilla-js/
        const json = await this.restClient.postFormUrlEncoded<{
            access_token: string;
        }>("/connect/token", body);
        const token: string = json.access_token;

        this.restClient.setToken(token);

        return true;
    }
}
</script>

<style lang="less">
/******************
    Element styles
******************/

body {
    padding: 0px;
    margin: 0px;
}

img {
    border-style: none;
}

p {
    padding-top: 5px;
    padding-bottom: 5px;
}

/******************
    general styles
******************/
.hidden {
    display: none;
}

.clear {
    clear: both;
}

.footer {
    color: #999;
    margin-top: 1em;
}

button a {
    padding-left: 0.5em;
}

.checkbox input[type="checkbox"] {
    margin-left: 0;
}

.btn .glyphicon {
    padding-right: 0.25em;
}

.header {
    padding-bottom: 1em;
}

.fbbody {
    padding-left: 28px;
}

/******************
    Index styles
******************/
#profileDiv #steamUserIdHeading {
    padding-top: 0;
    margin-top: 0;
    font-size: 200%;
}

#profileDiv #profileImage {
    width: 69px;
    height: 69px;
    padding: 0 5px 5px 0;
}

#gamesDiv .game {
    text-align: center;
    height: 115px;
}

#gamesDiv .game img {
    display: inline-block;
    width: 184px;
    height: 69px;
    margin: 0px;
    padding: 0px;
}

/******************
    Publish styles
******************/
.unpublished .game {
    height: 2em;
}

.unpublished .game h3 {
    font-size: 150%;
}

.unpublished .achievement {
    border: 1px solid #fff;
}

.unpublished .selected {
    background-color: #fff9d7;
    border: 1px solid #e2c822;
}

.unpublished .achievement input {
    display: none;
}

.unpublished .achievement img {
    vertical-align: middle;
    width: 64px;
    height: 64px;
    margin: 5px;
    display: inline-block;
}

.unpublished .achievement .text {
    height: 64px;
    margin: 5px;
    display: inline-block;
    overflow: hidden;
}

.unpublished .achievement .text,
.unpublished .achievement .text label {
    max-width: 70%;
}

.unpublished .achievement .text label {
    max-width: 100%;
}

.unpublished .achievement .name {
    display: block;
    font-weight: bold;
    margin-bottom: 0.25em;
}

.unpublished .achievement .description {
    font-weight: normal;
    display: block;
}

/* publish dialog anchor hack */
#middleAnchor {
    position: absolute;
    display: block;
    text-decoration: none;
    color: #fff;
}

.settings-page .error-settings-link {
    display: none !important;
}

/******************
    Documents
******************/
#toc .toc-h3 {
    padding-left: 1em;
}

/******************
    validation
******************/
.field-validation-error,
.validation-summary-errors {
    color: Red;
}

/******************
    bootstrap message
******************/
.message-dismiss {
    padding-left: 0.5em;
    font-weight: normal;
    color: #999;
}

.message-container {
    padding: 0.5em;
    line-height: 1em;
}

.message-container p {
    padding: 0 1em;
}

.message-container p .glyphicon {
    margin-right: 0.3em;
}

/******************
    UserVoice
******************/
#uservoice-feedback {
    display: none;
}

@media (min-width: 768px) {
    #uservoice-feedback {
        display: block;
    }
}
</style>
