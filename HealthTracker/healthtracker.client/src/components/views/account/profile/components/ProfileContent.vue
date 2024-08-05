<template>
    <div class="content-right">
        <div class="content-header">
            <button v-for="tab in tabs" :key="tab" @click="setActiveTab(tab)" :class="{ active: activeTab === tab }">
                {{ tab }}
            </button>
        </div>
        <!-- Zawartość zakładek -->
        <div v-if="activeTab === 'Posts'">
            <p>Here are the posts...</p>
        </div>
        <div v-if="activeTab === 'Goals'">
            <p>Here are the goals...</p>
        </div>
        <div v-if="activeTab === 'Training plans'">
            <p>Here are the training plans...</p>
        </div>
        <div v-if="activeTab === 'Friends'">
            <p>Friends confirmations here...</p>
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue'

const props = defineProps({
    tabs: {
        type: Array as () => string[],
        default: () => [],
    }
});

const activeTab = ref(props.tabs[0]);

function setActiveTab(tabName: string) {
    activeTab.value = tabName;
}
</script>
<style scoped lang="scss">
.content-right {
    display: flex;
    flex-direction: column;
    padding: 1rem;
    border: 1px solid #ccc;
    border-radius: 10px;

    .content-header {
        display: flex;
        flex-direction: row;

        button {
            width: 100%;
            padding: 10px;
            margin-bottom: 5px;

            cursor: pointer;
            border: 1px solid #ccc;
            background-color: #f8f8f8;

            &:hover {
                background-color: #e8e8e8;
            }
        }
    }

}
</style>