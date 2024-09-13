<template>
  <div>
    <p class="form-label">{{ formTitle }}</p>
    <p v-for="error in formStatus.errors" class="error" v-bind:key="error">
      {{ error }}
    </p>
    <p class="success">
      {{ formStatus.success }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { formStatus, clearFormStatus } from '@/api/account/sendDataService'
clearFormStatus()
defineProps<{
  formTitle: string
}>()
</script>

<style lang="scss" scoped>
@use '@/assets/styles/form.scss';
</style>