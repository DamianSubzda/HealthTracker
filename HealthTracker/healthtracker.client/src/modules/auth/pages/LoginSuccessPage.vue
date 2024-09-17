<template>
  <div>Login successful</div>
</template>

<script>
import router from "@/router";
import { useUserStore } from "@/modules/auth/store/userStore";
export default {
  mounted() {
    const urlParams = new URLSearchParams(window.location.search);
    const userDTO = urlParams.get('user');
    const userStore = useUserStore();
    if (userDTO) {
      const userData = JSON.parse(decodeURIComponent(userDTO));
      localStorage.setItem('user', JSON.stringify(userData));
      router.push("/").then(() => {
        userStore.updateUserData();
      });
    }
  }
}
</script>
