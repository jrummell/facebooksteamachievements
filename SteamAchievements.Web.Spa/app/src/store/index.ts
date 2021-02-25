import { createStore } from "vuex";
import { IResources, ISteamProfile, IGame, IUser } from "@/models";
import AppState from "./AppState";
import IApiSettings from "./IApiSettings";
import MutationTypes from "./MutationTypes";

export default createStore<AppState>({
    state: new AppState(),
    mutations: {
        setUser(state: AppState, payload: IUser) {
            state.user = { ...state.user, ...payload };
        },
        setProfile(state: AppState, payload: ISteamProfile) {
            state.profile = { ...state.profile, ...payload };
        },
        setResources(state: AppState, payload: IResources) {
            state.resources = { ...state.resources, ...payload };
        },
        setGames(state: AppState, payload: IGame[]) {
            state.games = [...payload];
        }
    },
    actions: {},
    modules: {}
});

export { AppState, IApiSettings, MutationTypes };
