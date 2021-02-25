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
import { Vue, Options } from "vue-class-component";
import { Inject } from "vue-property-decorator";
import IUser from "../models/IUser";
import RestClient from "../helpers/RestClient";
import FB from "../helpers/FB";

@Options({ name: "Login" })
export default class Login extends Vue {
    @Inject()
    restClient!: RestClient;

    isConnected = false;
    FB?: FB;
    userId = 0;

    get appId(): string {
        return this.$store.state.facebookAppId.toString();
    }

    sdkLoaded(payload: { isConnected: boolean; FB: object }) {
        this.isConnected = payload.isConnected;
        this.FB = payload.FB as FB;
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
        if (this.FB) {
            this.FB.api("/me", "GET", { fields: "id" }, (user: { id: string }) => {
                this.userId = Number.parseInt(user.id, 10);

                this.$store.commit("setUser", { facebookUserId: this.userId });

                this.getUser();
            });
        }
    }

    async getUser() {
        let user = await this.restClient.getJson<IUser>(`api/User/${this.userId}`);
        if (user == null) {
            // create a user if they don't exist
            const model = { facebookUserId: this.userId };

            user = await this.restClient.postJson("/api/User", model);
        }

        this.$store.commit("setUser", user);
    }
}
</script>
