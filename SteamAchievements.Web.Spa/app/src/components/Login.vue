<template>
    <!-- <v-facebook-login :app-id="appId" version="5.0"></v-facebook-login> -->
    <facebook-login
        ref="facebookLogin"
        class="button"
        :appId="appId"
        @login="onLogin"
        @get-initial-status="onLogin"
    ></facebook-login>
</template>
<script lang="ts">
import Vue from "vue";
import { Prop, Component, Inject } from "vue-property-decorator";
import facebookLogin from "facebook-login-vuejs";
import FacebookConfig from "../config/FacebookConfig";

@Component({})
export default class Login extends Vue {
    @Inject()
    facebookConfig: FacebookConfig;

    get appId(): string {
        return this.facebookConfig.appId.toString();
    }

    mounted() {}

    onLogin() {
        this.$root.$emit("login-loggedin");
    }
}
</script>
