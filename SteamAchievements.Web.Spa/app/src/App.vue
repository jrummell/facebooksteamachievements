<template>
    <div id="app">
        <div class="header mb-2">
            <router-link to="/">
                <img src="/images/banner-light.png" class="img-fluid" alt="Steam Achievements" />
            </router-link>
        </div>
        <div v-if="loaded">
            <div v-if="loggedIn">
                <b-nav tabs class="mb-2">
                    <b-nav-item :active="$route.name == 'home'">
                        <router-link to="/">{{ resources.menuHome }}</router-link>
                    </b-nav-item>
                    <b-nav-item :active="$route.name == 'games'">
                        <router-link to="/games">{{ resources.menuGames }}</router-link>
                    </b-nav-item>
                    <b-nav-item :active="$route.name == 'settings'">
                        <router-link to="/settings">{{ resources.menuSettings }}</router-link>
                    </b-nav-item>
                    <b-nav-item :href="helpUrl" target="_blank">{{ resources.menuHelp }}</b-nav-item>
                </b-nav>
                <div class="mt-2 mb-2">
                    <profile></profile>
                </div>
            </div>
            <div v-else>
                <login class="mt-4"></login>
            </div>
        </div>
        <loading-indicator :loading="!loaded"></loading-indicator>

        <router-view v-if="loaded && loggedIn" />

        <div class="footer">
            <p>
                <a href="http://www.facebook.com/SteamAchievements" target="_blank">Steam Achievements</a>
                is not developed by Facebook or Valve. Steam Achievements is an
                <a href="https://github.com/jrummell/facebooksteamachievements" target="_blank">open source project</a>
                created by a dude who likes to frag on the PC. Logo design by
                <a href="http://www.facebook.com/vince.costello" target="_blank">Vince Costello</a>. If you have a
                suggestion or have found a bug, please report it on the project's
                <a href="https://github.com/jrummell/facebooksteamachievements/issues" target="_blank">issue tracker</a
                >. Please include your Steam Community Profile URL when reporting any issues.
            </p>
            <p>Valve, Steam and the Steam logo are registered trademarks of Valve Corporation.</p>

            <p>Version {{ version }}.</p>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { MutationPayload } from "vuex";
import { AppState, MutationTypes } from "./store";
import { IResources } from "./models";
import { Inject } from "vue-property-decorator";
import RestClient from "./helpers/RestClient";

@Component
export default class App extends Vue {
    @Inject()
    restClient: RestClient;

    loggedIn: boolean = false;
    loaded: boolean = false;
    resources: IResources | null = null;
    helpUrl: string = this.$store.state.helpUrl;
    version: string = this.$store.state.version;

    mounted() {
        this.loggedIn = this.$store.state.user != undefined;

        this.$store.subscribe((mutation: MutationPayload, state: AppState) => {
            if (mutation.type == MutationTypes.setUser) {
                this.loggedIn = state.user != undefined;
            }
        });

        this.getResources();
    }

    async getResources(): Promise<void> {
        if (!(await this.authorize())) {
            return;
        }

        this.resources = await this.restClient.getJson("/api/Resource");

        this.$store.commit(MutationTypes.setResources, this.resources);

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

.footer {
    color: #999;
    margin-top: 1em;
}
</style>
