import { type RouteRecordRaw } from "vue-router";

import Diary from "@/modules/diary/pages/DiaryPage.vue";

export const diaryRoutes: Array<RouteRecordRaw> = [
  {
    path: "/diary",
    name: "Diary",
    component: Diary,
    meta: { requiresAuth: true },
  },
];
