import { type RouteRecordRaw } from "vue-router";

import Goals from "@/modules/goals/pages/GoalsPage.vue";

export const goalRoutes: Array<RouteRecordRaw> = [
  {
    path: "/goals",
    name: "Goals",
    component: Goals,
    meta: { requiresAuth: true },
  },
];
