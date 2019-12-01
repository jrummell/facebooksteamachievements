import Vue from "vue";
import Vuex from "vuex";
import IUser from "@/models/IUser";
import IResources from "@/models/IResources";
import ISteamProfile from "@/models/ISteamProfile";

Vue.use(Vuex);

export class AppState {
    user?: IUser;
    profile?: ISteamProfile;
    resources: IResources;
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
