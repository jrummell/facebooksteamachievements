<template>
    <font-awesome-icon v-if="loading" icon="spinner" spin size="2x"></font-awesome-icon>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { Prop } from "vue-property-decorator";

@Component
export default class LoadingIndicator extends Vue {
    @Prop({ required: true })
    loading: boolean;
}
</script>
