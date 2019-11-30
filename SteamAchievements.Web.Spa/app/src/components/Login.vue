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

@Component({})
export default class Login extends Vue {
    @Inject()
    facebookConfig: FacebookConfig;

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
        let response = await fetch(`api/User/${this.userId}`);
        if (response.status === 404) {
            // create a user if they don't exist
            const model = { facebookUserId: this.userId };

            response = await fetch("/api/User", {
                method: "POST",
                body: JSON.stringify(model),
                headers: {
                    "Content-Type": "application/json"
                }
            });
        }

        const user: IUser = await response.json();

        this.$store.commit("setUser", user);
    }
}
</script>
