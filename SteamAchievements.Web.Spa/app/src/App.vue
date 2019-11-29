<template>
    <div id="app">
        <div id="nav" v-if="loggedIn">
            <router-link to="/">Home</router-link>|
            <router-link to="/about">About</router-link>
        </div>
        <div v-else>
            <login></login>
        </div>
        <router-view />
    </div>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";

@Component
export default class App extends Vue {
    loggedIn: boolean = false;

    mounted() {
        this.$root.$on("login-loggedin", () => {
            this.loggedIn = true;
        });
    }
}
</script>

<style>
#app {
    font-family: "Avenir", Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
}

#nav {
    padding: 30px;
}

#nav a {
    font-weight: bold;
    color: #2c3e50;
}

#nav a.router-link-exact-active {
    color: #42b983;
}
</style>
