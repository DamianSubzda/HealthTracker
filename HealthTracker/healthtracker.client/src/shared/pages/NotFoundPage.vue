<template>
    <main>
        <ErrorScreen :code="404" :message="`Page not found!`" />
    </main>
</template>

<script lang="ts" setup>
import ErrorScreen from './../components/ErrorWidget.vue';
</script>

<style lang="scss" scoped></style>