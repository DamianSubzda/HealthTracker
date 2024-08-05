<template>
    <div class="profile-picture">
            <img v-if="picture != null" :src="picture" alt="profile picture">
            <img v-else src="@/assets/community/pictures/defaultProfilePicture.png" alt="default picture">
    </div>
</template>
<script setup lang="ts">

defineProps<{
    picture: string
}>();

</script>
<style scoped lang="scss">
.profile-picture {
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;

  img {
    height: 10rem;
    width: 10rem;
    object-fit: cover;
    object-position: center;
  }
}

</style>