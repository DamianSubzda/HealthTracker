<template>
  <div class="sidebar_item" v-if="!item.isHidden">
    <p>
      <router-link class="item" :to="{name: item.name}">
        <span class="material-icons">{{ item.icon }}</span>
        <span class="text">{{ item.name }}</span>
      </router-link>
    </p>
  </div>
</template>

<script lang="ts" setup>
defineProps<{
  item: {
    name: string;
    description: string;
    icon: string;
    isHidden: boolean;
  }
}>()
</script>

<style lang="scss" scoped>
</style>