import { type RouteRecordRaw } from "vue-router";

import CreatePost from "@/modules/community/pages/CreatePostPage.vue";
import Community from "@/modules/community/pages/CommunityPage.vue";

export const communityRoutes: Array<RouteRecordRaw> = [
  {
    path: "/community",
    name: "Community",
    component: Community,
    meta: { requiresAuth: true },
  },
  {
    path: "/community/post/create",
    name: "CreatePost",
    component: CreatePost,
    meta: { requiresAuth: true },
  },
];
