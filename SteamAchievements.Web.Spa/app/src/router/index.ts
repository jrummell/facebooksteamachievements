import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";

const routes: RouteRecordRaw[] = [
    {
        path: "/",
        name: "home",
        component: () => import(/* webpackChunkName: "home" */ "../views/Home.vue")
    },
    {
        path: "/games",
        name: "games",
        component: () => import(/* webpackChunkName: "games" */ "../views/Games.vue")
    },
    {
        path: "/settings",
        name: "settings",
        component: () => import(/* webpackChunkName: "settings" */ "../views/Settings.vue")
    }
];

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
});

export default router;
