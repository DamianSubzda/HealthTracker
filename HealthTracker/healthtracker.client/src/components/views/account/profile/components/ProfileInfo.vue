<template>
    <div class="content-left">
        <div>
            <ProfilePicture :picture="profile.profilePicture"/>
            <div class="profile-name">
                <h1>{{ profile.firstName }}</h1>
                <h1>{{ profile.lastName }}</h1>
            </div>
        </div>
        <div v-if="profile.about != null">
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
            <p>With HealthTracker from:</p>
            <h2>{{ formatUtcToLocal(profile.dateOfCreate) }}</h2>
        </div>
    </div>
</template>
<script setup lang="ts">
import { type IProfile} from '@/service/api/account/profileController';
import ProfilePicture from './ProfilePicture.vue'

defineProps<{
    profile: IProfile
}>();

function formatUtcToLocal(inputDate: string) {
  const date = new Date(inputDate);

  const day = date.getDate().toString().padStart(2, '0');
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const year = date.getFullYear();

  return `${day}-${month}-${year}r.`;
}


</script>
<style scoped lang="scss">
.content-left {

display: flex;
flex-direction: column;
gap: 1rem;

.profile-name {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 10px;
}

.contact {
    font-weight: 300;    
}

.dates {
    font-weight: 300;
}
}
</style>