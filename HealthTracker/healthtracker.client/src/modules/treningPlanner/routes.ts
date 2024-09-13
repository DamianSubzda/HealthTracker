import { type RouteRecordRaw } from "vue-router";

import TreningsPlanner from "@/modules/treningPlanner/pages/TreningPlannerPage.vue";

export const treningPlannerRoutes: Array<RouteRecordRaw> = [
  {
    path: "/planner",
    name: "Planner",
    component: TreningsPlanner,
    meta: { requiresAuth: true }
  },
];
