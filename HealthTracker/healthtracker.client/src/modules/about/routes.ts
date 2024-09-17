import { type RouteRecordRaw } from "vue-router";
import About from '@/modules/about/pages/AboutPage.vue'

export const aboutRoutes: Array<RouteRecordRaw> = [
    {
        path: '/about',
        name: 'About',
        component: About
    },
];