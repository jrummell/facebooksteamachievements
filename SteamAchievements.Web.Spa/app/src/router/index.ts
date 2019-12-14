import Vue from "vue";
import VueRouter, { RouteConfig } from "vue-router";
import Home from "../views/Home.vue";
import Settings from "../views/Settings.vue";
import Games from "../views/Games.vue";

Vue.use(VueRouter);

const routes: RouteConfig[] = [
    {
        path: "/",
        name: "home",
        component: Home
    },
    {
        path: "/games",
        name: "games",
        component: Games
    },
    {
        path: "/settings",
        name: "settings",
        component: Settings
    }
];

const router = new VueRouter({
    base: process.env.BASE_URL,
    routes
});

export default router;
