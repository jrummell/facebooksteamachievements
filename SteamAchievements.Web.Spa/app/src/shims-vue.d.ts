/* eslint-disable */
declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<{}, {}, any>
  export default component
}

declare module 'facebook-login-vuejs';

import { ComponentCustomProperties } from 'vue'
import { Store } from 'vuex'
import { AppState } from './store'

declare module '@vue/runtime-core' {
    // provide typings for `this.$store`
  interface ComponentCustomProperties {
    $store: Store<AppState>
  }
}
