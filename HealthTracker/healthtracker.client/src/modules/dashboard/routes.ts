import { type RouteRecordRaw } from "vue-router";
import DashboardPage from '@/modules/dashboard/pages/DashboardPage.vue'

export const dashboardRoutes: Array<RouteRecordRaw> = [
    {
        path: '/dashboard',
        name: 'Dashboard',
        component: DashboardPage,
        meta: { requiresAuth: true, requiresAdmin: true },
    },
];