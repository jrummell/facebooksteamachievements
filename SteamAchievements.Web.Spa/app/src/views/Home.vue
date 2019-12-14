<template>
    <b-card>
        <div v-if="!loading && showSettings">
            <div class="alert alert-info">
                Configure your steam
                <router-link to="/settings">{{ resources.menuSettings }}</router-link>
            </div>
        </div>
        <div v-if="!loading">
            <div class="alert alert-info">
                <p>{{resources.publishInstructions}}</p>
            </div>
            <div>
                <b-button variant="primary" class="mr-2" @click="publish">
                    <font-awesome-icon icon="check"></font-awesome-icon>
                    {{resources.buttonPublish || "Publish"}}
                </b-button>
                <b-button variant="danger" @click="hide">
                    <font-awesome-icon icon="times"></font-awesome-icon>
                    {{resources.buttonHide || "Hide"}}
                </b-button>
            </div>
            <div v-if="achievements.length == 0">You have no unpublished achievements.</div>

            <b-row v-for="item in achievements" :key="item.game.id">
                <b-col md="12">
                    <h4 class="pt-3">{{item.game.name}}</h4>
                    <b-row>
                        <b-col
                            v-for="achievement in item.achievements"
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
import {
    IAchievement,
    IResources,
    IGameAchievements,
    IUser,
    ISteamProfile
} from "../models";

@Component
export default class Home extends Vue {
    @Inject()
    restClient: RestClient;

    resources: IResources = this.$store.state.resources;

    user: IUser = this.$store.state.user;
    loading: boolean = true;
    showSettings: boolean = false;
    achievements: IGameAchievements[] = this.$store.state.achievements || [];

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
        if (
            this.achievements.length === 0 &&
            this.user &&
            this.user.id &&
            this.user.steamUserId
        ) {
            this.loading = true;

            await this.restClient.postJson<{}, number>(
                `/api/Achievement/Update/${this.user.id}`,
                {}
            );

            await this.getAchievements();

            this.loading = false;
            this.showSettings = false;
        } else {
            this.loading = false;
        }

        this.showSettings = this.user && !this.user.steamUserId;
    }

    async getAchievements(): Promise<void> {
        const achievements = await this.restClient.getJson<IGameAchievements[]>(
            `/api/Achievement/${this.user.id}`
        );

        if (achievements) {
            this.achievements = achievements;
        }
    }

    select(achievement: IAchievement) {
        achievement.selected = !(achievement.selected || false);
    }

    async publish(): Promise<void> {
        const user: IUser = this.$store.state.user;

        let descriptions: string[] = [];

        let selectedAchivementIds: number[] = [];
        this.achievements.forEach((value, index) => {
            const selected = value.achievements.filter(
                a => a.selected === true
            );

            selectedAchivementIds = selectedAchivementIds.concat(
                selected.map(a => a.id)
            );

            let description = `${value.game.name}: `;

            description += selected
                .map(a => {
                    const achievementDescription = user.publishDescription
                        ? ` (${a.description})`
                        : "";

                    return a.name + achievementDescription;
                })
                .join(", ");

            descriptions.push(description);
        });

        const achievementCount = selectedAchivementIds.length;
        if (achievementCount === 0) {
            return;
        }

        const message = `${
            user.steamUserId
        } unlocked ${achievementCount} achievement${
            achievementCount > 1 ? "s" : ""
        }! \r\n\r\n${descriptions.join(". ")}`;

        // https://developers.facebook.com/docs/sharing/reference/share-dialog#jssdk
        FB.ui(
            {
                method: "share",
                href: window.location.href,
                quote: message
            },
            async (): Promise<void> => {
                this.loading = true;

                await this.markPublished(user.id, selectedAchivementIds);

                await this.getAchievements();

                this.loading = false;
            }
        );
    }

    async markPublished(
        userId: string,
        achievementIds: number[]
    ): Promise<void> {
        await this.restClient.postJson(
            `/api/Achievement/${userId}`,
            achievementIds
        );
    }

    async hide(): Promise<void> {
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

        await this.restClient.deleteJson(
            `/api/Achievement/${user.id}`,
            selectedAchivementIds
        );

        await this.getAchievements();

        this.loading = false;
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
