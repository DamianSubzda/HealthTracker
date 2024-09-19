import type { NavigationGuardNext, RouteLocationNormalized } from 'vue-router';
import { useUserStore } from '@/shared/store/userStore';

export function authGuard(to: RouteLocationNormalized, from: RouteLocationNormalized, next: NavigationGuardNext) {
  const userStore = useUserStore();
  
  if (to.meta.requiresAdmin) {
    if (userStore.roles && userStore.roles.includes('Admin')) {
      next();
    } else {
      next('/unauthorized');
    }
    return;
  }

  if (to.meta.requiresAuth) {
    if (userStore.token) {
      next();
    } else {
      next('/login');
    }
  } else {
    next();
  }
}
