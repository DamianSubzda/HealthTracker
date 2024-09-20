<template>
    <div class="main">
        <Sidebar />
        <Header title="HealthTracker" />
        <div class="content">
            <RouterView v-slot="{ Component }">
                <Transition name="fade" mode="out-in">
                    <component :is="Component" class="view" />
                </Transition>
            </RouterView>
            <Footer class="footer" />
        </div>
    </div>
</template>

<script setup lang="ts">
import Footer from '@/shared/components/TheFooter.vue'
import Sidebar from '@/shared/components/TheSidebar.vue'
import Header from '@/shared/components/TheHeader.vue'
import { onMounted } from 'vue';
import { useUserStore } from '@/shared/store/userStore'
import { useNavigationStore } from '@/shared/store/navigationStore'

const userStore = useUserStore();
const navigationStore = useNavigationStore();

onMounted(async () => {
    await initialApplication()
});

async function initialApplication() {
    console.log('Strona została odświeżona lub załadowana');

    userStore.updateUserData();
    navigationStore.updateLinkVisibility();
}
</script>

<style lang="scss" scoped>
.main {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
    overflow-x: hidden;
    background: rgb(37, 32, 32);
    background: linear-gradient(135deg, rgba(37, 32, 32, 1) 0%, rgba(62, 50, 50, 1) 50%, rgba(126, 99, 99, 1) 100%);
    background-repeat: no-repeat;
    background-attachment: fixed;

    .content {
        display: flex;
        flex-direction: column;
        flex: 1;
        padding-top: 4rem;
        padding-left: 4rem;
        overflow-y: auto;
    }

    .footer {
        margin-top: auto;
    }
}

.fade-enter-from,
.fade-leave-to {
    opacity: 0;
}

.fade-enter-active,
.fade-leave-active {
    transition: opacity 0.3s ease-out;
}
</style>
