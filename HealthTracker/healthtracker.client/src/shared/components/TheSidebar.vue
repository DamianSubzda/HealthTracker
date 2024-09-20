<template>
    <aside :class="`${is_expanded && 'is-expanded'}`">
        <div class="control">
            <div class="logo">
                <img src="@/assets/pictures/LogoHT.png" class="logo-img" alt="Logo" />
            </div>
            <div class="menu-toggle-wrap">
                <button class="menu-toggle" @click="ToggleMenu">
                    <span class="material-icons">
                        keyboard_double_arrow_right
                    </span>
                </button>
            </div>
        </div>
        <div class="menu">
            <SidebarItem v-for="link in navigationStore.links" :item="link" :key="link.name" />
        </div>
        <div class="flex"></div>
        <div class="menu">
            <SidebarItem v-for="link in navigationStore.authLinks" :item="link" :key="link.name" />
        </div>
    </aside>
</template>

<script lang="ts" setup>
import { ref, onBeforeMount } from "vue";
import SidebarItem from "./SidebarItem.vue";
import { useNavigationStore } from "./../store/navigationStore"
const is_expanded = ref(false)
const navigationStore = useNavigationStore();

onBeforeMount(() => {
    navigationStore.initializeLinks();
});

const ToggleMenu = () => {
    is_expanded.value = !is_expanded.value
}
</script>

<style lang="scss">
@use "@/assets/styles/sidebar";
</style>
