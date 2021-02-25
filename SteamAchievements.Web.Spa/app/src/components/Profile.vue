<template>
    <b-row v-if="profile != null">
        <b-col>
            <img id="profileImage" :src="profile.avatarUrl" class="float-left" />
            <h1 id="steamUserIdHeading">{{ profile.steamUserId }}</h1>
            <span>{{ profile.headline }}</span>
        </b-col>
    </b-row>
</template>
<script lang="ts">
import { Options, Vue } from "vue-class-component";
import { Inject } from "vue-property-decorator";
import { MutationPayload } from "vuex";
import { AppState } from "../store";
import ISteamProfile from "../models/ISteamProfile";
import RestClient from "../helpers/RestClient";

@Options({ name: "Profile" })
export default class Profile extends Vue {
    @Inject()
    restClient!: RestClient;

    profile: ISteamProfile | null = null;

    mounted() {
        this.$store.subscribe((mutation: MutationPayload, state: AppState) => {
            if (mutation.type === "setUser" && state.user && state.user.steamUserId) {
                this.getProfile();
            }
        });

        if (this.$store.state.user?.steamUserId) {
            this.getProfile();
        }
    }

    async getProfile(): Promise<void> {
        if (this.$store.state.profile) {
            this.profile = this.$store.state.profile;
            return;
        }

        if (this.$store.state.user?.steamUserId) {
            this.profile = await this.restClient.getJson(`/api/Profile/${this.$store.state.user.steamUserId}`);

            this.$store.commit("setProfile", this.profile);
        }
    }
}
</script>
