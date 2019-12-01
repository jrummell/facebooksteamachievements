<template>
    <div v-if="profile != null">
        <img id="profileImage" :src="profile.avatarUrl" class="float-left" />
        <h1 id="steamUserIdHeading">{{ profile.steamUserId }}</h1>
        <span>{{ profile.headline }}</span>
    </div>
</template>
<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { Prop, Inject } from "vue-property-decorator";
import { MutationPayload } from "vuex";
import { AppState } from "../store";
import IUser from "../models/IUser";
import ISteamProfile from "../models/ISteamProfile";

@Component
export default class Profile extends Vue {
    profile: ISteamProfile | null = null;

    mounted() {
        this.$store.subscribe((mutation: MutationPayload, state: AppState) => {
            if (
                mutation.type === "setUser" &&
                state.user &&
                state.user.steamUserId
            ) {
                this.getUser();
            }
        });

        if (this.$store.state.user.steamUserId) {
            this.getUser();
        }
    }

    async getUser(): Promise<void> {
        if (this.$store.state.user.steamUserId) {
            const response = await fetch(
                `/api/Profile/${this.$store.state.user.steamUserId}`
            );
            this.profile = await response.json();

            this.$store.commit("setProfile", this.profile);
        }
    }
}
</script>
