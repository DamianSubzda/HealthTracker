import { useUserStore } from './../store/account/auth';
import {createRouter, createWebHistory} from 'vue-router'

import Home from '../components/views/home/HomePage.vue'
import About from '../components/views/about/AboutPage.vue'
import Diary from '../components/views/diary/DiaryPage.vue'
import TreningsPlanner from '../components/views/treningsPlanner/TreningsPlannerPage.vue'
import Health from '../components/views/health/HealthPage.vue'
import Goals from '../components/views/goals/GoalsPage.vue'
import Community from '../components/views/community/CommunityPage.vue'
import Register from '../components/views/account/register/RegisterPage.vue'
import Login from '../components/views/account/login/LoginPage.vue'
import NewPass from '../components/views/account/new_pass/NewPassPage.vue'
import PassReset from '../components/views/account/pass_reset/PassResetPage.vue'
import LoginSuccess from '../components/views/account/login/SuccessLoginPage.vue'
import UserProfile from '../components/views/account/profile/ProfilePage.vue'

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
      component: PassReset
    },
    {
      path: '/login/new-pass',
      name: 'New Password',
      component: NewPass
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
      path: '/profile/:id',
      name: 'UserProfile',
      component: UserProfile
    }
  ]
})
export default router