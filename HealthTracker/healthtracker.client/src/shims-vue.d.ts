/* eslint-disable */
declare module '*.vue' {
    import type { DefineComponent } from 'vue'
    const component: DefineComponent<{}, {}, any>
    export default component
}

declare module 'vue-jwt-decode' {
    const VueJwtDecode: {
      decode: (token: string) => any;
    };
    export default VueJwtDecode;
  }
  