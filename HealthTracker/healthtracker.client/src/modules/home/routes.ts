import { type RouteRecordRaw } from "vue-router";
import Home from "@/modules/home/pages/HomePage.vue";

export const homeRoutes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "Home",
    component: Home,
  },
];
