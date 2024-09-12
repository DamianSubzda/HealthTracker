<template>
  <div class="profile-picture">
    <img v-if="picture != null" :src="localPicture" alt="profile picture">
    <img v-else src="@/assets/community/pictures/defaultProfilePicture.png" alt="default picture">
    <div className="change-picture">
      <button v-if="userStore.userId == profileId" v-on:click="openFileDialog">
        <img src="@/assets/icons/change-photo.svg" alt="Insert image" />
        <input ref="fileInput" type="file" @change="handleFileSelect" accept="image/*" style="display: none;" >
      </button>

    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue"
import config from "@/config.json"
import { useUserStore } from "@/store/account/auth";
import { setUserPhoto } from "@/service/api/account/profileController";

const props = defineProps<{
  picture: string,
  profileId: number
}>();

const localPicture = ref(`${config.serverURL}${props.picture}?v=${new Date().getTime()}`);
const userStore = useUserStore();
const fileInput = ref<HTMLInputElement | null>(null);

async function handleFileSelect() {
    if (fileInput.value) {
        try {
            const response = await setUserPhoto(props.profileId, fileInput.value);
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
    margin-top: 5px;
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
  }

  img {
    height: 10rem;
    width: 10rem;
    object-fit: cover;
    object-position: center;
  }
}
</style>