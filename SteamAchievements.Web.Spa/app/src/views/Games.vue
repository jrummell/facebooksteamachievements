<template>
    <b-card>
        <b-row>
            <b-col>
                <b-alert show>
                    {{ resources.gamesYourGames }}
                    <help-button anchor="supported-games" :text="resources.buttonHelp"></help-button>
                </b-alert>
            </b-col>
        </b-row>
        <b-row>
            <b-col>
                <loading-indicator :loading="loading"></loading-indicator>
            </b-col>
        </b-row>
        <b-row>
            <b-col v-for="game in games" :key="game.id" md="4" lg="3">
                <div class="mb-2">
                    <a target="_blank" :href="`${game.statsUrl}?tab=achievements`">
                        <img :src="game.imageUrl" :alt="game.name" :title="game.name" />
                    </a>
                </div>
            </b-col>
        </b-row>
    </b-card>
</template>

<script lang="ts">
import Vue from "vue";
import { Inject, Component } from "vue-property-decorator";
import RestClient from "../helpers/RestClient";
import { IGame, IResources } from "../models";
import { MutationTypes } from "../store";

@Component
export default class Games extends Vue {
    @Inject()
    restClient: RestClient;

    resources: IResources = this.$store.state.resources;

    loading: boolean = true;

    games: IGame[] = this.$store.state.games;

    mounted() {
        this.getGames();
    }

    async getGames(): Promise<void> {
        if (this.games.length > 0) {
            this.loading = false;
            return;
        }

        const games = await this.restClient.getJson<IGame[]>(`/api/Game/${this.$store.state.user.steamUserId}`);
        if (games) {
            this.$store.commit(MutationTypes.setGames, games);
            this.games = games;
        }

        this.loading = false;
    }
}
</script>
