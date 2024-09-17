import { type RouteRecordRaw } from "vue-router";

import Health from "@/modules/health/pages/HealthPage.vue";

export const healthRoutes: Array<RouteRecordRaw> = [
  {
    path: "/health",
    name: "Health",
    component: Health,
    meta: { requiresAuth: true },
  },
];
