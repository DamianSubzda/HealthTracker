<template>
    <div class="error-container">
        <div class="error-icon">⚠️</div>
        <h2>Error {{ code }}</h2>
        <p>{{ message }}</p>
    </div>
</template>

<script setup>
defineProps({
    code: {
        type: Number,
        default: 500
    },
    message: {
        type: String,
        default: "An unexpected error occurred. Please try again later."
    }
});
</script>

<style scoped>
.error-container {
    font-size: large;
    text-align: center;
    padding: 2rem;
    background-color: #ffcccc;
    border: 1px solid #ff0000;
    border-radius: 10px;
    color: #ff0000;
    width: fit-content;
    margin: auto;
    margin-top: 2rem;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
}

.error-icon {
    font-size: 3rem;
    margin-bottom: 1rem;
}

h2 {
    font-size: 2rem;
}

p {
    font-size: 1.2rem;
}
</style>
