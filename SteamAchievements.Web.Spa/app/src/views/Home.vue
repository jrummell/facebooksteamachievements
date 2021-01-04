<template>
    <b-card>
        <div v-if="!loading && showSettings">
            <div class="alert alert-info">
                {{ resources.homeConfigure }}
                <router-link to="/settings">{{ resources.menuSettings }}</router-link>
            </div>
        </div>
        <div v-if="!loading">
            <div v-if="achievements.length > 0">
                <div class="alert alert-info">{{ resources.publishInstructions }}</div>
                <div v-if="selectedAchievements.length > 0">
                    <b-button variant="primary" class="mr-2" @click="publish">
                        <font-awesome-icon icon="check"></font-awesome-icon>
                        {{ resources.buttonPublish || "Publish" }}
                    </b-button>
                    <b-button variant="danger" @click="hide">
                        <font-awesome-icon icon="times"></font-awesome-icon>
                        {{ resources.buttonHide || "Hide" }}
                    </b-button>
                </div>
            </div>
            <div v-if="achievements.length == 0 && !showSettings">{{ resources.homeNoUnPublishedAchievements }}</div>

            <b-row v-for="item in achievements" :key="item.game.id">
                <b-col md="12">
                    <h4 class="pt-3" @click="selectGame(item)">
                        <b-form-checkbox
                            :id="`game-${item.game.id}`"
                            :inline="true"
                            class="mr-0 pt-1"
                            @change="selectGame(item)"
                            v-model="item.selected"
                        ></b-form-checkbox>
                        <label :for="`game-${item.game.id}`">{{ item.game.name }}</label>
                    </h4>
                    <b-row>
                        <b-col v-for="achievement in item.achievements" :key="achievement.id" md="6">
                            <div
                                :class="{
                                    'bg-secondary': achievement.selected,
                                    'text-light': achievement.selected,
                                    'p-2': true,
                                    'mb-2': true
                                }"
                            >
                                <label :for="`achievement-check-${achievement.apiName}`">
                                    <b-row>
                                        <b-col cols="3">
                                            <img :src="achievement.imageUrl" :alt="achievement.name" class="mt-2" />
                                        </b-col>
                                        <b-col cols="9">
                                            <b-form-checkbox
                                                :id="`achievement-check-${achievement.apiName}`"
                                                class="d-inline-block"
                                                v-model="achievement.selected"
                                            ></b-form-checkbox>
                                            <span class="font-weight-bold">{{ achievement.name }}</span>
                                            <p>{{ achievement.description }}</p>
                                        </b-col>
                                    </b-row>
                                </label>
                            </div>
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
import { BFormCheckbox } from "bootstrap-vue";
import RestClient from "../helpers/RestClient";
import { MutationPayload } from "vuex";
import { AppState, MutationTypes } from "../store";
import { IAchievement, IResources, IGameAchievements, IUser, ISteamProfile } from "../models";

@Component
export default class Home extends Vue {
    @Inject()
    restClient: RestClient;

    resources: IResources = this.$store.state.resources;

    user: IUser = this.$store.state.user;
    loading: boolean = true;
    showSettings: boolean = false;
    achievements: IGameAchievements[] = this.$store.state.achievements || [];

    get selectedAchievements(): IAchievement[] {
        const selected: IAchievement[] = [];

        this.achievements.forEach(gameAchievement => {
            gameAchievement.achievements
                .filter(achievement => achievement.selected)
                .forEach(achievement => {
                    selected.push(achievement);
                });
        });

        return selected;
    }

    mounted() {
        this.loadAchievements();

        this.$store.subscribe((mutation: MutationPayload, state: AppState) => {
            if (mutation.type == MutationTypes.setUser) {
                if (state.user) {
                    this.user = state.user;
                }
                this.loadAchievements();
            }
        });
    }

    async loadAchievements(): Promise<void> {
        if (this.achievements.length === 0 && this.user && this.user.id && this.user.steamUserId) {
            this.loading = true;

            await this.restClient.postJson<{}, number>(`/api/Achievement/Update/${this.user.id}`, {});

            await this.getAchievements();

            this.loading = false;
            this.showSettings = false;
        } else {
            this.loading = false;
            this.showSettings = true;
        }
    }

    async getAchievements(): Promise<void> {
        const achievements = await this.restClient.getJson<IGameAchievements[]>(`/api/Achievement/${this.user.id}`);

        if (achievements) {
            this.achievements = achievements;
        }
    }

    select(achievement: IAchievement) {
        achievement.selected = !(achievement.selected || false);
    }

    selectGame(game: IGameAchievements) {
        game.achievements.forEach(achievement => {
            achievement.selected = !game.selected;
        });
    }

    async publish(): Promise<void> {
        if (this.selectedAchievements.length === 0) {
            return;
        }

        const user: IUser = this.$store.state.user;

        let descriptions: string[] = [];

        let selectedAchivementIds: number[] = [];
        this.achievements.forEach((value, index) => {
            const selected = value.achievements.filter(a => a.selected === true);

            selectedAchivementIds = selectedAchivementIds.concat(selected.map(a => a.id));

            let description = `${value.game.name}: `;

            description += selected
                .map(a => {
                    const achievementDescription = user.publishDescription ? ` (${a.description})` : "";

                    return a.name + achievementDescription;
                })
                .join(", ");

            descriptions.push(description);
        });

        const achievementCount = selectedAchivementIds.length;
        if (achievementCount === 0) {
            return;
        }

        const message = `${user.steamUserId} unlocked ${achievementCount} achievement${
            achievementCount > 1 ? "s" : ""
        }! \r\n\r\n${descriptions.join(". ")}`;

        // https://developers.facebook.com/docs/sharing/reference/share-dialog#jssdk
        FB.ui(
            {
                method: "share",
                href: window.location.href,
                quote: message
            },
            async (response: any): Promise<void> => {
                if (response.error_code) {
                    return;
                }

                this.loading = true;

                await this.markPublished(user.id, selectedAchivementIds);

                await this.getAchievements();

                this.loading = false;
            }
        );
    }

    async markPublished(userId: string, achievementIds: number[]): Promise<void> {
        await this.restClient.postJson(`/api/Achievement/${userId}`, achievementIds);
    }

    async hide(): Promise<void> {
        if (this.selectedAchievements.length === 0) {
            return;
        }

        const user: IUser = this.$store.state.user;

        const selectedAchivementIds: number[] = [];
        this.achievements.forEach(g => {
            g.achievements
                .filter(a => a.selected === true)
                .forEach(a => {
                    selectedAchivementIds.push(a.id);
                });
        });

        this.loading = true;

        await this.restClient.deleteJson(`/api/Achievement/${user.id}`, selectedAchivementIds);

        await this.getAchievements();

        this.loading = false;
    }
}
</script>
