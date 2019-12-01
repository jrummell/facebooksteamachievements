<template>
    <b-card v-if="resources" class="settings-page">
        <b-card-body>
            <b-row v-if="saveSuccess">{{ resources.settingsSuccess }}</b-row>
            <b-row v-if="validateProfileError">{{ resources.settingsInvalidCustomUrl }}</b-row>

            <b-form>
                <span
                    v-if="steamUserIdValid === false"
                    class="danger"
                >{{resources.settingsCustomUrlRequired}}</span>
                <b-form-group :label="resources.settingsCustomUrl">
                    <b-row>
                        <b-col md="10">
                            <b-input-group>
                                <b-input-group-prepend>
                                    <b-input-group-text>http://steamcommunity.com/id/</b-input-group-text>
                                </b-input-group-prepend>
                                <b-form-input
                                    v-model="model.steamUserId"
                                    @change="onSteamUserIdChanged"
                                ></b-form-input>
                                <b-input-group-append>
                                    <b-input-group-text>
                                        <font-awesome-icon
                                            v-if="steamUserIdValid"
                                            icon="check"
                                            full-width
                                        ></font-awesome-icon>
                                        <font-awesome-icon v-else icon="exclamation" full-width></font-awesome-icon>
                                    </b-input-group-text>
                                </b-input-group-append>
                            </b-input-group>
                        </b-col>
                        <b-col md="2">
                            <help-button anchor="configure-your-steam-community-profile"></help-button>
                        </b-col>
                    </b-row>
                </b-form-group>
                <b-form-group label="Publish Options">
                    <b-form-checkbox
                        v-model="model.publishDescription"
                    >{{ resources.settingsPublishDescriptionLabel }}</b-form-checkbox>
                </b-form-group>
                <b-button @click="save" variant="primary">
                    <font-awesome-icon icon="save"></font-awesome-icon>
                    {{ resources.save || "Save" }}
                </b-button>
            </b-form>
        </b-card-body>
    </b-card>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { Inject } from "vue-property-decorator";
import IResources from "../models/IResources";
import IUser from "../models/IUser";

@Component
export default class Settings extends Vue {
    resources: IResources | null = null;

    saveSuccess: boolean = false;
    validateProfileError: boolean = false;
    steamUserIdValid: boolean | null = null;

    model: IUser | null = null;

    mounted() {
        this.resources = this.$store.state.resources;
        this.model = this.$store.state.user;
    }

    async onSteamUserIdChanged(value: string): Promise<void> {
        if (this.model && this.model.steamUserId) {
            const response = await fetch(`/api/Profile/${value}`);

            this.steamUserIdValid = response.ok;
        }
    }

    async save(): Promise<void> {
        const response = await fetch("/api/User", {
            method: "PUT",
            body: JSON.stringify(this.model),
            headers: {
                "Content-Type": "application/json"
            }
        });

        this.steamUserIdValid = response.ok;

        this.model = await response.json();

        this.$store.commit("setUser", this.model);
    }
}
</script>
