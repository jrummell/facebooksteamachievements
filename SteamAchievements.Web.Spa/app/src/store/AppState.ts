import IUser from "@/models/IUser";
import IResources from "@/models/IResources";
import ISteamProfile from "@/models/ISteamProfile";
import IGame from "@/models/IGame";
import IApiSettings from "./IApiSettings";

export default class AppState {
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
    version: string = process.env.VUE_APP_VERSION || "9.0.0";
    games: IGame[] = [];
}
