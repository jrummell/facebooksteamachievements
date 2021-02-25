<template>
    <font-awesome-icon v-if="loading" icon="spinner" spin size="2x"></font-awesome-icon>
</template>

<script lang="ts">
import { Options, Vue } from "vue-class-component";
import { Prop } from "vue-property-decorator";

@Options({ name: "LoadingIndicator" })
export default class LoadingIndicator extends Vue {
    @Prop({ required: true })
    loading!: boolean;
}
</script>
