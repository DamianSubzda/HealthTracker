<template>
  <div>Login successful</div>
</template>

<script>
import router from "@/router";
import { useUserStore } from "@/shared/store/userStore";
import { useNavigationStore } from '@/shared/store/navigationStore'
export default {
  mounted() {
    const urlParams = new URLSearchParams(window.location.search);
    const userDTO = urlParams.get('user');
    const userStore = useUserStore();
    const navigationStore = useNavigationStore();
    if (userDTO) {
      const userData = JSON.parse(decodeURIComponent(userDTO));
      localStorage.setItem('user', JSON.stringify(userData));
      router.push("/").then(() => {
        userStore.updateUserData();
        navigationStore.updateLinkVisibility();
      });
    }
  }
}
</script>
