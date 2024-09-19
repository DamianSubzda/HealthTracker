<template>
    <div class="content-left">
        <div>
            <ProfilePicture :profile="profile" />
            <div class="profile-name">
                <h1>{{ profile.firstName }}</h1>
                <h1>{{ profile.lastName }}</h1>
            </div>
        </div>
        <div v-if="profile.about != null">
            <p>{{ profile.about }}</p>
        </div>
        <div class="contact">
            <div className="email">
                <p>Email:</p>
                <h2>{{ profile.email }}</h2>
            </div>
            <div className="phonenumber">
                <p>Phonenumber:</p>
                <h2>{{ profile.phoneNumber }}</h2>
            </div>
            
        </div>
        <div class="dates">
            <div className="birthday">
                <p>Birthday:</p>
                <h2>{{ formatUtcToLocal(profile.dateOfBirth) }}</h2>
            </div>
            <div className="ht-date">
                <p>With HealthTracker from:</p>
                <h2>{{ formatUtcToLocal(profile.dateOfCreate) }}</h2>
            </div>
            
        </div>
    </div>
</template>
<script setup lang="ts">
import type { IProfile } from "./../types/Profile.ts"
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
    gap: 1.5rem;
    padding: 1rem;
    background-color: rgb(62, 50, 50);
    border-radius: 8px;
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
    margin-bottom: 1rem;
    height: auto;

    .profile-name {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        gap: 5px; 

        h1 {
            margin: 0;
            font-size: x-large;
            color: white;
        }
    }

    p {
        margin: 0.5rem 0;
        color: #fcfafa;
        font-size: medium;
        word-wrap: break-word;
        white-space: normal;
    }

    h2 {
        margin: 0;
        font-size: large;
        color: #000000;
        font-weight: bold;
        word-wrap: break-word;
        white-space: normal;
    }

    .contact, .dates {
        background-color: rgb(141, 119, 119);
        padding: 1rem;
        border-radius: 8px;
        box-shadow: 0 2px 4px rgba(0,0,0,0.05);

        p, h2 {
            display: inline-block;
            margin-right: 10px;
        }
    }

    .contact, .dates {
        border-top: 2px solid rgb(192, 166, 166);
    }
}
.email, .phonenumber, .birthday, .ht-date {
    display: flex;
    flex-direction: column;
}

</style>