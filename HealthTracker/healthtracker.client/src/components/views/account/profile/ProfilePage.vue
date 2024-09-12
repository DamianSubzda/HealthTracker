<template>
  <div v-if="isLoading" style="justify-content: center; display: flex; margin-top: 1rem;">
    <LoadingScreen :cubSize="25"/>
  </div>
  <div v-else-if="profile != null">
    <div v-if="!isFriendshipRequestSended" class="header">
      <button @click="sendFriendshipRequest">Send friend request</button>
    </div>
    <div class="personal-info">
      <ProfileInfo :profile="profile" />
      <ProfileContent :tabs="['Posts', 'Goals', 'Training plans', 'Friends']" :profile="profile" />
    </div>

  </div>
  <div v-else>
    <ErrorScreen :code="404" :message="`User not found!`"/>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import ErrorScreen from "./../../../shared/ErrorScreen.vue"
import { useRoute } from 'vue-router';
import { type IProfile, getProfileById } from '@/service/api/account/profileController';
import LoadingScreen from '../../../shared/LoadingScreen.vue'
import { getFriendship, postFriendshipRequest } from '@/service/api/community/friendshipController';
import ProfileContent from './components/ProfileContent.vue';
import ProfileInfo from './components/ProfileInfo.vue';
import { useUserStore } from "@/store/account/auth";

const profile = ref<IProfile | null>(null);
const isLoading = ref(false);
const route = useRoute();
const isFriendshipRequestSended = ref(true);
const userStore = useUserStore();
const isOwnProfile = ref(false);

onMounted(async () => {
  isLoading.value = true;

  let userId = route.params.id ? Number(route.params.id) : userStore.userId;
  isOwnProfile.value = !route.params.id;

  const fetchedProfile = userId ? await getProfileById(userId) : null;
  
  if (fetchedProfile != null) {
    profile.value = fetchedProfile;
    await getFriendshipStatus();
  }
  isLoading.value = false;
});

async function getFriendshipStatus() {
  if (isOwnProfile.value){
    isFriendshipRequestSended.value = true;
    return;
  }

  if (profile.value) {
    const friendshipData = await getFriendship(profile.value.id);
    if (friendshipData != null) {
      isFriendshipRequestSended.value = true;
    } else {
      isFriendshipRequestSended.value = false;
    }
  }
}

async function sendFriendshipRequest() {
  if (profile.value) {
    isFriendshipRequestSended.value = await postFriendshipRequest(profile.value.id);
  }
}


</script>
<style scoped lang="scss">
.header {
  padding: 10px;

  button {
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 1rem;
    height: inherit;
    width: 100%;
    padding: 1rem;
    cursor: pointer;
    background-color: rgb(153, 144, 144);
    border: 0;
    font-size: x-large;

    &:hover {
      background-color: rgb(95, 95, 95);
    }
  }
}

.personal-info {
  display: grid;
  grid-template-columns: 1fr 3fr;
  padding: 1rem;
  width: inherit;
  align-items: start;
  gap: 1rem;

  @media (max-width: 768px) {
    grid-template-columns: 100%;
    grid-template-rows: 1fr auto;
  }
}
</style>