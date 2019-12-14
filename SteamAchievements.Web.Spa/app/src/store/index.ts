import Vue from "vue";
import Vuex from "vuex";
import { IResources, ISteamProfile, IGame, IUser } from "@/models";
import AppState from "./AppState";
import IApiSettings from "./IApiSettings";
import MutationTypes from "./MutationTypes";

Vue.use(Vuex);

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
        },
        setGames(state, payload: IGame[]) {
            Vue.set(state, "games", [...payload]);
        }
    },
    actions: {},
    modules: {}
});

export { AppState, IApiSettings, MutationTypes };
