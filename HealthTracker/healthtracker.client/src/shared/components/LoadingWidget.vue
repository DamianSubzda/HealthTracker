<template>
    <div class="loader"
        :style="{ width: (3.6 * cubSize) + 'px', height: (2 * cubSize) + 'px', '--cub-size': cubSize + 'px' }"></div>
</template>

<script setup>
defineProps({
    cubSize: {
        type: Number,
        default: 25
    }
});

</script>

<style scoped lang="scss">
.loader {
    opacity: 0.8;
    --g1: conic-gradient(from 90deg at left 3px top 3px, #0000 90deg, #fff 0);
    --g2: conic-gradient(from -90deg at bottom 3px right 3px, #0000 90deg, #fff 0);
    background: var(--g1), var(--g1), var(--g1), var(--g2), var(--g2), var(--g2);
    background-position: left, center, right;
    background-repeat: no-repeat;
    animation: l9 1s infinite;
}

@keyframes l9 {
    0% {
        background-size: var(--cub-size) 50%, var(--cub-size) 50%, var(--cub-size) 50%
    }

    25% {
        background-size: var(--cub-size) 100%, var(--cub-size) 50%, var(--cub-size) 50%
    }

    50% {
        background-size: var(--cub-size) 50%, var(--cub-size) 100%, var(--cub-size) 50%
    }

    75% {
        background-size: var(--cub-size) 50%, var(--cub-size) 50%, var(--cub-size) 100%
    }

    100% {
        background-size: var(--cub-size) 50%, var(--cub-size) 50%, var(--cub-size) 50%
    }
}
</style>
