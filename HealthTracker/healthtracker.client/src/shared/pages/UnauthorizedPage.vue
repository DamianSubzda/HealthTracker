<template>
    <main>
        <ErrorScreen :code="403" :message="`Unauthorized Access!`"/>
    </main>
</template>

<script lang="ts" setup>
import ErrorScreen from './../components/ErrorWidget.vue';
</script>

<style lang="scss" scoped></style>