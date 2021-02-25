<template>
    <b-card v-if="resources" class="settings-page">
        <div v-if="saveSuccess" class="text-success">{{ resources.settingsSuccess }}</div>
        <div v-if="steamProfileValid === false" class="text-danger">{{ resources.settingsInvalidCustomUrl }}</div>

        <b-form>
            <span v-if="steamUserIdValid === false" class="text-danger">{{ resources.settingsCustomUrlRequired }}</span>
            <b-form-group :label="resources.settingsCustomUrl">
                <b-row>
                    <b-col>
                        <b-input-group>
                            <b-input-group-prepend class="d-none d-sm-block">
                                <b-input-group-text>http://steamcommunity.com/id/</b-input-group-text>
                            </b-input-group-prepend>
                            <b-form-input v-model="model.steamUserId" @change="validateSteamUserId"></b-form-input>
                            <b-input-group-append>
                                <b-input-group-text>
                                    <font-awesome-icon
                                        v-if="steamUserIdValid"
                                        icon="check"
                                        full-width
                                    ></font-awesome-icon>
                                    <font-awesome-icon v-else icon="exclamation" full-width></font-awesome-icon>
                                </b-input-group-text>
                                <help-button anchor="configure-your-steam-community-profile"></help-button>
                            </b-input-group-append>
                        </b-input-group>
                    </b-col>
                </b-row>
            </b-form-group>
            <b-form-group label="Publish Options">
                <b-form-checkbox v-model="model.publishDescription">
                    {{ resources.settingsPublishDescriptionLabel }}
                </b-form-checkbox>
            </b-form-group>
            <b-button @click="save" variant="primary">
                <font-awesome-icon icon="save"></font-awesome-icon>
                {{ resources.save || "Save" }}
            </b-button>
        </b-form>
    </b-card>
</template>

<script lang="ts">
import { Options, Vue } from "vue-class-component";
import { Inject } from "vue-property-decorator";
import { IUser } from "../models";
import RestClient from "../helpers/RestClient";
import { MutationTypes } from "../store";

@Options({ name: "Settings" })
export default class Settings extends Vue {
    @Inject()
    restClient!: RestClient;

    resources = this.$store.state.resources;

    saveSuccess = false;
    steamProfileValid: boolean | null = null;
    steamUserIdValid: boolean | null = null;

    get model(): IUser | undefined {
        return this.$store.state.user;
    }

    mounted() {
        if (this.model) {
            this.validateSteamUserId(this.model.steamUserId || "");
        }
    }

    async validateSteamUserId(value: string): Promise<void> {
        if (this.model && this.model.steamUserId) {
            const response = await this.restClient.getJson(`/api/Profile/${value}`);

            this.steamProfileValid = response != null;
        }
    }

    async save(): Promise<void> {
        if (this.steamProfileValid) {
            const response = await this.restClient.putJson("/api/User", this.model);

            this.steamUserIdValid = response != null;

            if (this.steamUserIdValid) {
                this.$store.commit(MutationTypes.setUser, response);

                this.saveSuccess = true;
            }
        }
    }
}
</script>
