import {createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { authGuard } from './guards';

import NotFoundPage from '@/shared/pages/NotFoundPage.vue'

import { aboutRoutes } from '@/modules/about/routes';
import { authRoutes } from '@/modules/auth/routes';
import { communityRoutes } from '@/modules/community/routes';
import { diaryRoutes } from '@/modules/diary/routes';
import { goalRoutes } from '@/modules/goals/routes';
import { healthRoutes } from '@/modules/health/routes';
import { homeRoutes } from '@/modules/home/routes';
import { profileRoutes } from '@/modules/profile/routes';
import { treningPlannerRoutes } from '@/modules/treningPlanner/routes';

const routes: Array<RouteRecordRaw> = [
  ...aboutRoutes,
  ...authRoutes,
  ...communityRoutes,
  ...diaryRoutes,
  ...goalRoutes,
  ...healthRoutes,
  ...homeRoutes,
  ...profileRoutes,
  ...treningPlannerRoutes,
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFoundPage,
    meta: { requiresAuth: true }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes: routes
});

router.beforeEach(authGuard)

export default router