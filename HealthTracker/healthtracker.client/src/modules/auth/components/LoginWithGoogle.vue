<template>
  <ButtonElement @click="loginWithGoogle" button-label="Log in with Google" align="center" full size="lg"
    name="LoginWithGoogle" :columns="{ container: 12, label: 0, wrapper: 12 }" />
</template>

<script setup>
import config from "@/config.json"
const isLogging = defineModel();

async function loginWithGoogle() {
  isLogging.value = true;
  window.location.href = `${config.serverURL}login-google`;
}
</script>
