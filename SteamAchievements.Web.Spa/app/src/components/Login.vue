<template>
    <facebook-login
        class="button"
        :appId="appId"
        @sdk-loaded="sdkLoaded"
        @login="onLogin"
        @get-initial-status="onLogin"
    ></facebook-login>
</template>
<script lang="ts">
import Vue from "vue";
import { Prop, Component, Inject } from "vue-property-decorator";
import facebookLogin from "facebook-login-vuejs";
import FacebookConfig from "../config/FacebookConfig";
import IUser from "../models/IUser";
import RestClient from "../helpers/RestClient";

@Component({})
export default class Login extends Vue {
    @Inject()
    facebookConfig: FacebookConfig;
    @Inject()
    restClient: RestClient;

    isConnected: boolean = false;
    FB: any;
    userId: number;

    get appId(): string {
        return this.facebookConfig.appId.toString();
    }

    sdkLoaded(payload: { isConnected: boolean; FB: any }) {
        this.isConnected = payload.isConnected;
        this.FB = payload.FB;
        if (this.isConnected) {
            this.getFacebookUser();
        }
    }

    onLogin() {
        if (this.isConnected) {
            this.getFacebookUser();
        }
    }

    getFacebookUser() {
        this.FB.api("/me", "GET", { fields: "id" }, (user: { id: string }) => {
            this.userId = Number.parseInt(user.id, 10);

            this.$store.commit("setUser", { facebookUserId: this.userId });

            this.getUser();
        });
    }

    async getUser() {
        let user = await this.restClient.getJson<IUser>(
            `api/User/${this.userId}`
        );
        if (user == null) {
            // create a user if they don't exist
            const model = { facebookUserId: this.userId };

            user = await this.restClient.postJson("/api/User", model);
        }

        this.$store.commit("setUser", user);
    }
}
</script>
