import Vue from "vue";
import Vuex from "vuex";
import IUser from "@/models/IUser";
import IResources from "@/models/IResources";
import ISteamProfile from "@/models/ISteamProfile";

Vue.use(Vuex);

interface IApiSettings {
    clientId: string;
    clientSecret: string;
}

export class AppState {
    user?: IUser;
    profile?: ISteamProfile;
    resources: IResources;
    apiSettings: IApiSettings = {
        // https://cli.vuejs.org/guide/mode-and-env.html#environment-variables
        clientId: process.env.VUE_APP_CLIENT_ID,
        clientSecret: process.env.VUE_APP_CLIENT_SECRET
    };
    facebookAppId: number = process.env.VUE_APP_FACEBOOK_APP_ID;
    helpUrl: string = process.env.VUE_APP_HELP_URL;
}

export default new Vuex.Store({
    state: new AppState(),
    mutations: {
        setUser(state, payload: IUser) {
            state.user = { ...state.user, ...payload };
        },
        setProfile(state, payload: ISteamProfile) {
            state.profile = { ...state.profile, ...payload };
        },
        setResources(state, payload: IResources) {
            state.resources = { ...state.resources, ...payload };
        }
    },
    actions: {},
    modules: {}
});
