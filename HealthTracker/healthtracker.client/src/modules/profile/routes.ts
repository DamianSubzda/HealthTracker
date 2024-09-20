import { type RouteRecordRaw } from "vue-router";
import { useUserStore } from "@/shared/store/userStore";
import Profile from "@/modules/profile/pages/ProfilePage.vue";

export const profileRoutes: Array<RouteRecordRaw> = [
  {
    path: "/profile",
    name: "Profile",
    component: Profile,
    meta: { requiresAuth: true },
  },
  {
    path: "/profile/:id",
    name: "UsersProfile",
    component: Profile,
    beforeEnter: (to, from, next) => {
      const userStore = useUserStore();
      if (userStore.userId?.toString() === to.params.id) {
        next("/profile");
      } else {
        next();
      }
    },
    meta: { requiresAuth: true },
  },
];
