<template>
    <b-card>
        <div v-if="!loading">
            <div class="alert alert-info">
                <p>{{resources.publishInstructions}}</p>
            </div>
            <div>
                <b-button variant="primary" class="mr-2">
                    <font-awesome-icon icon="check"></font-awesome-icon>
                    {{resources.buttonPublish || "Publish"}}
                </b-button>
                <b-button variant="danger">
                    <font-awesome-icon icon="times"></font-awesome-icon>
                    {{resources.buttonHide || "Hide"}}
                </b-button>
            </div>
            <div v-if="achievements.length == 0">You have no unpublished achievements.</div>

            <b-row v-for="game in achievements" :key="game.game">
                <b-col md="12">
                    <h4>{{game.game}}</h4>
                    <b-row>
                        <b-col
                            v-for="achievement in game.achievements"
                            :key="achievement.id"
                            md="6"
                        >
                            <b-row class="achievement" :class="{selected: achievement.selected}">
                                <b-col md="3">
                                    <b-form-checkbox
                                        class="achievement-check"
                                        v-model="achievement.selected"
                                    ></b-form-checkbox>
                                    <img :src="achievement.imageUrl" :alt="achievement.name" />
                                </b-col>
                                <b-col md="9">
                                    <label>
                                        <span class="name">{{achievement.name}}</span>
                                        <span class="description">{{achievement.description}}</span>
                                    </label>
                                </b-col>
                            </b-row>
                        </b-col>
                    </b-row>
                </b-col>
            </b-row>
        </div>
        <loading-indicator :loading="loading"></loading-indicator>
    </b-card>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { Inject } from "vue-property-decorator";
import RestClient from "../helpers/RestClient";
import { MutationPayload } from "vuex";
import { AppState, MutationTypes } from "../store";
import { IAchievement, IResources, IGameAchievements } from "../models";

@Component
export default class Home extends Vue {
    @Inject()
    restClient: RestClient;

    resources: IResources = this.$store.state.resources;

    loading: boolean = true;
    achievements: IGameAchievements[] = this.$store.state.achievements || [];

    mounted() {
        this.getAchievements();

        this.$store.subscribe((mutation: MutationPayload, state: AppState) => {
            if (mutation.type == MutationTypes.setUser) {
                this.getAchievements();
            }
        });
    }

    async getAchievements(): Promise<void> {
        if (
            this.achievements.length === 0 &&
            this.$store.state.user &&
            this.$store.state.user.id
        ) {
            this.loading = true;

            const userId: number = this.$store.state.user.id;
            await this.restClient.postJson<{}, number>(
                `/api/Achievement/Update/${userId}`,
                {}
            );

            const achievements = await this.restClient.getJson<
                IGameAchievements[]
            >(`/api/Achievement/${userId}`);

            if (achievements) {
                this.achievements = achievements;
            }

            this.loading = false;
        }
    }

    select(achievement: IAchievement) {
        achievement.selected = !(achievement.selected || false);
    }
}
</script>

<style lang="less" scoped>
.achievement {
    padding: 5px;
    margin: 2px;

    .achievement-check {
        display: inline-block;
    }

    .name {
        display: block;
        font-weight: bold;
    }
}
.achievement.selected {
    background-color: #fff9d7;
    border: 1px solid #e2c822;
}
</style>
