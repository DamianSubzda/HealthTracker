<template>
  <div class="profile-picture">
    <img v-if="profile.profilePicture != null" :src="localPicture" alt="profile picture">
    <img v-else src="@/assets/pictures/defaultProfilePicture.png" alt="default picture">
    <div v-if="userStore.userId == profile.id" className="change-picture">
      <button  v-on:click="openFileDialog">
        <img src="@/assets/icons/change-photo.svg" alt=" " />
        <input ref="fileInput" type="file" @change="handleFileSelect" accept="image/*" style="display: none;">
      </button>

    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue"
import config from "@/config.json"
import { useUserStore } from "@/shared/store/userStore";
import { apiPostUserPhoto, type IProfile } from "@/api/account/profileController";

const props = defineProps<{
    profile: IProfile
}>();

const localPicture = ref(`${config.serverURL}${props.profile.profilePicture}?v=${new Date().getTime()}`);
const userStore = useUserStore();
const fileInput = ref<HTMLInputElement | null>(null);

async function handleFileSelect() {
  if (fileInput.value) {
    try {
      const response = await apiPostUserPhoto(props.profile.id, fileInput.value);
      if (response) {
        localPicture.value = `${config.serverURL}${response}?v=${new Date().getTime()}`;
        fileInput.value.value = "";
      } else {
        console.error('Failed to upload photo.');
      }
    } catch (error) {
      console.error('Error uploading file:', error);
    }
  }
}

function openFileDialog() {
  fileInput.value?.click();
}

</script>
<style scoped lang="scss">
.profile-picture {
  flex-direction: column;
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;

  .change-picture {
    height: 2rem;
    width: 2rem;
    padding: 5px;
    margin-top: 5px;
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
  }

  button {
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 1rem;
    height: inherit;
    width: inherit;
    cursor: pointer;
    background-color: rgb(153, 144, 144);
    border: 0;

    img {
      border-radius: inherit;
      height: 100%;
    }

    &:hover {
      background-color: rgb(95, 95, 95);
      z-index: 9999;

      img {
        height: inherit;
        transition: height 0.3s ease;
      }
    }
  }

  img {
    height: 10rem;
    width: 10rem;
    object-fit: cover;
    object-position: center;
  }
}
</style>