import Vue from "vue";
import Vuex from "vuex";
import ProfileModel from "@/models/ProfileModel";
import IUser from "@/models/IUser";

Vue.use(Vuex);

export class AppState {
    user?: IUser;
    profile?: ProfileModel;
}

export default new Vuex.Store({
    state: new AppState(),
    mutations: {
        setUser(state, payload: IUser) {
            state.user = { ...state.user, ...payload };
        },
        setProfile(state, payload: ProfileModel) {
            state.profile = { ...state.profile, ...payload };
        }
    },
    actions: {},
    modules: {}
});
