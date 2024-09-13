import { useUserStore } from '../modules/auth/store/auth';
import {createRouter, createWebHistory} from 'vue-router'

import Home from '../modules/home/pages/HomePage.vue'
import About from '../modules/about/pages/AboutPage.vue'
import Diary from '../modules/diary/pages/DiaryPage.vue'
import TreningsPlanner from '../modules/treningPlanner/pages/TreningPlannerPage.vue'
import Health from '../modules/health/pages/HealthPage.vue'
import Goals from '../modules/goals/pages/GoalsPage.vue'
import Community from '../modules/community/pages/CommunityPage.vue'

import Register from '../modules/auth/pages/RegisterPage.vue'
import Login from '../modules/auth/pages/LoginPage.vue'
import LoginSuccess from '../modules/auth/pages/LoginSuccessPage.vue'
import PasswordNew from '../modules/auth/pages/PasswordNewPage.vue'
import PasswordReset from '../modules/auth/pages/PasswordResetPage.vue'

import Profile from '../modules/profile/pages/ProfilePage.vue'
import CreatePost from '../modules/community/pages/CreatePostPage.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'Home',
      component: Home
    },
    {
      path: '/about',
      name: 'About',
      component: About
    },
    {
      path: '/diary',
      name: 'Diary',
      component: Diary
    },
    {
      path: '/planner',
      name: 'Planner',
      component: TreningsPlanner
    },
    {
      path: '/health',
      name: 'Health',
      component: Health
    },
    {
      path: '/goals',
      name: 'Goals',
      component: Goals
    },
    {
      path: '/community',
      name: 'Community',
      component: Community
    },
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
    {
      path: '/profile',
      name: 'Profile',
      component: Profile
    },
    {
      path: '/profile/:id',
      name: 'UsersProfile',
      component: Profile,
      beforeEnter: (to, from, next) => {
        const userStore = useUserStore();
        if (userStore.userId?.toString() === to.params.id) {
          next('/profile');
        } else {
          next();
        }
      }
    },
    {
      path: '/post/create',
      name: 'CreatePost',
      component: CreatePost
    }
  ]
})
export default router