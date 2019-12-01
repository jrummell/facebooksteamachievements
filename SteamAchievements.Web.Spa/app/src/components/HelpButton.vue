<template>
    <b-button :href="href" target="_blank">
        <font-awesome-icon icon="question"></font-awesome-icon>
        {{ text }}
    </b-button>
</template>
<script lang="ts">
import Vue from "vue";
import { Component, Prop } from "vue-property-decorator";

@Component
export default class HelpButton extends Vue {
    @Prop({ required: false })
    link?: string;

    @Prop({ required: false })
    anchor?: string;

    get href(): string {
        const link = this.link || this.$store.state.helpUrl;
        if (this.anchor) {
            return `${link}#${this.anchor}`;
        }
        return link;
    }

    @Prop({ required: false })
    text?: string;

    get textInternal(): string {
        return this.text || this.$store.state.resources.buttonHelp;
    }
}
</script>
