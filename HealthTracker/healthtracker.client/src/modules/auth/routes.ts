import { type RouteRecordRaw } from 'vue-router';
import { useUserStore } from '@/shared/store/userStore';

import Register from '@/modules/auth/pages/RegisterPage.vue'
import Login from '@/modules/auth/pages/LoginPage.vue'
import LoginSuccess from '@/modules/auth/pages/LoginSuccessPage.vue'
import PasswordNew from '@/modules/auth/pages/PasswordNewPage.vue'
import PasswordReset from '@/modules/auth/pages/PasswordResetPage.vue'

export const authRoutes: Array<RouteRecordRaw> = [
    {
        path: '/register',
        name: 'Register',
        component: Register
      },
      {
        path: '/login',
        name: 'Login',
        component: Login
      },
      {
        path: '/login-success',
        name: 'LoginSuccess',
        component: LoginSuccess
      },
      {
        path: '/login/pass-reset',
        name: 'Reset Password',
        component: PasswordReset
      },
      {
        path: '/login/new-pass',
        name: 'New Password',
        component: PasswordNew
      },
      {
        path: '/logout',
        name: 'Logout',
        beforeEnter: (_to, _from, next) => {
          localStorage.removeItem("user");
          const userStore = useUserStore();
          userStore.updateUserData();
          next('/login')
        },
        redirect: ''
      },
];
