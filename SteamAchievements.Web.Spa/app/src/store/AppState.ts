import IApiSettings from "./IApiSettings";
import { IGame, ISteamProfile, IResources, IUser, IGameAchievements } from "@/models";

export default class AppState {
    user?: IUser;
    profile?: ISteamProfile;
    resources?: IResources;
    apiSettings: IApiSettings = {
        // https://cli.vuejs.org/guide/mode-and-env.html#environment-variables
        clientId: process.env.VUE_APP_CLIENT_ID,
        clientSecret: process.env.VUE_APP_CLIENT_SECRET
    };
    facebookAppId: number = process.env.VUE_APP_FACEBOOK_APP_ID;
    helpUrl: string = process.env.VUE_APP_HELP_URL;
    version: string = process.env.VUE_APP_VERSION || "10.0.0";
    games: IGame[] = [];
    achievements: IGameAchievements[] = [];
}
