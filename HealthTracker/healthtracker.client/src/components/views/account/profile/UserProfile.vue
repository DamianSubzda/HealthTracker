<template>
  <div v-if="isLoading" style="height: 8vh;">
    <LoadingScreen />
  </div>
  <div v-else-if="profile != null">
    <div class="header">
      <button>Send friend request</button>
    </div>
    <div class="personal-info">
      <div class="content-left">
        <div>
          <div class="profile-picture">
            <img v-if="profile.profilePicture != null" :src="profile.profilePicture" alt="profile picture">
            <img v-else src="@/assets/community/pictures/defaultProfilePicture.png" alt="default picture">
          </div>
          <div class="profile-name">
            <h1>{{ profile.firstName }}</h1>
            <h1>{{ profile.lastName }}</h1>
          </div>
        </div>
        <div v-if="profile.about!=null">
          <p>{{ profile.about }}</p>
        </div>
        <div class="contact">
          <p>Email:</p>
          <h2>{{ profile.email }}</h2>
          <p>Phonenumber:</p>
          <h2>{{ profile.phoneNumber }}</h2>
        </div>
        <div class="dates">
          <p>Birthday:</p>
          <h2>{{ formatUtcToLocal(profile.dateOfBirth) }}</h2>
          <p>In the HealthTracker from:</p>
          <h2>{{ formatUtcToLocal(profile.dateOfCreate) }}</h2>
        </div>
      </div>
      <div class="content-right">
        <div class="content-header">
          <button @click="setActiveTab('Posts')">Posts</button>
          <button @click="setActiveTab('Goals')">Goals</button>
          <button @click="setActiveTab('Training plans')">Training Plans</button>
        </div>

        <!-- Zawartość zakładek -->
        <div v-if="activeTab === 'Posts'">
          <!-- Treść zakładki Posts -->
          <p>Here are the posts...</p>
        </div>
        <div v-if="activeTab === 'Goals'">
          <!-- Treść zakładki Goals -->
          <p>Here are the goals...</p>
        </div>
        <div v-if="activeTab === 'Training plans'">
          <!-- Treść zakładki Training Plans -->
          <p>Here are the training plans...</p>
        </div>
      </div>
    </div>


  </div>
  <div v-else>
    <p>404 User not found!</p>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { type IProfile, getProfileById } from '@/service/api/account/profileController';
import LoadingScreen from '../../../shared/LoadingScreen.vue'

const profile = ref<IProfile | null>(null);
const isLoading = ref(false);
const route = useRoute();
const activeTab = ref('Posts');

onMounted(async () => {
  isLoading.value = true;
  const userId = route.params.id.toString();
  const fetchedProfile = await getProfileById(userId);

  if (fetchedProfile != null) {
    profile.value = fetchedProfile;
  }
  console.log(profile.value);
  isLoading.value = false;
});

function setActiveTab(tabName: string) {
  activeTab.value = tabName;
}

function formatUtcToLocal(inputDate: string) {
  const date = new Date(inputDate);

  const day = date.getDate().toString().padStart(2, '0');
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const year = date.getFullYear();

  return `${day}-${month}-${year}r.`;
}
</script>
<style scoped lang="scss">
.header{
  padding: 10px;
  button{
    width: 100%;
    padding: 1rem;
  }
}
.personal-info {
  display: grid;
  grid-template-columns: 1fr 3fr;
  padding: 1rem;
  width: inherit;

  @media (max-width: 768px) {
    grid-template-columns: 1fr;
    grid-template-rows: 1fr auto;
  }

  .content-left {

    display: flex;
    flex-direction: column;
    gap: 1rem;

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

    .profile-name {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 10px;
    }

    .contact {

    }

    .dates {

    }
  }

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
}
</style>