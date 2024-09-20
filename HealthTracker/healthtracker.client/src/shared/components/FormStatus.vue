<template>
    <div>
        <p class="form-label">{{ formTitle }}</p>
        <p v-for="error in formStatusStore.errors" class="error" v-bind:key="error.code">
            {{ error.description }}
        </p>
        <p class="success">
            {{ formStatusStore.success }}
        </p>
    </div>
</template>

<script setup lang="ts">
import { useFormStatusStore } from "@/modules/auth/store/formStatusStore"
const formStatusStore = useFormStatusStore();

formStatusStore.clearFormStatus()
defineProps<{
    formTitle: string
}>()
</script>

<style lang="scss" scoped>
@use '@/assets/styles/form.scss';
</style>