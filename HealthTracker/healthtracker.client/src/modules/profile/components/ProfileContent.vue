<template>
    <div class="content-right">
        <div class="content-header">
            <button v-for="tab in tabs" :key="tab" @click="setActiveTab(tab)" :class="{ active: activeTab === tab }">
                {{ tab }}
            </button>
        </div>
        <!-- Zawartość zakładek -->
        <div v-if="activeTab === 'Posts'" className="post-panel panel">
            <router-link :to="{ name: 'CreatePost' }" v-if="userStore.userId == profile.id">
                <img src="@/assets/icons/add.svg" alt=" " style="" />
            </router-link>

            <div v-if="arePostsLoading" style="justify-content: center; display: flex; margin-top: 1rem;">
                <LoadingScreen :cubSize="25" />
            </div>
            <div v-else v-for="post in posts" :key="post.id" className="post-div">
                <Post :post="post" />
            </div>
        </div>
        <div v-if="activeTab === 'Goals'" className="goal-panel panel">
            <p>Here are the goals...</p>
        </div>
        <div v-if="activeTab === 'Training plans'" className="traning-panel panel">
            <p>Here are the training plans...</p>
        </div>
        <div v-if="activeTab === 'Friends'" className="friend-panel panel">
            <div v-if="areFriendsLoading" style="justify-content: center; display: flex; margin-top: 1rem;">
                <LoadingScreen :cubSize="25" />
            </div>
            <div v-else>
                <div v-if="friendsStore.friendRequests.length != 0" class="friends-requests">
                    <p>Friend requests...</p>
                    <div v-for="friend in friendsStore.friendRequests" :key="friend.userId">
                        <FriendRequestItem :friend="friend" :onClick="() => redirectToProfile(friend)" />
                    </div>
                </div>
                <div class="friends">
                    <p>Friends...</p>
                    <div v-for="friend in friendsStore.friends" :key="friend.userId">
                        <FriendItem :friend="friend" :onClick="() => redirectToProfile(friend)"/>
                    </div>
                </div>

            </div>

        </div>
    </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Post from '@/modules/community/components/post/PostSection.vue'
import LoadingScreen from '@/shared/components/LoadingWidget.vue';
import { getUserPosts } from '@/api/community/postController'
import { type IPost } from '@/data/models/postModels';
import { type IProfile } from '@/api/account/profileController';
import { useUserStore } from '@/modules/auth/store/userStore';
import { apiGetFriendList, apiGetFriendshipRequestsForUser } from '@/api/community/friendshipController';
import FriendItem from '@/modules/community/components/friends/FriendItem.vue';
import FriendRequestItem from '@/modules/community/components/friends/FriendRequestItem.vue';
import { useFriendsStore, type IFriendModel, type IFriendRequestModel } from '@/modules/community/store/friendsStore';
import router from '@/router';

const posts = ref<IPost[] | null>(null);
const arePostsLoading = ref(true);
const areFriendsLoading = ref(true);
const postPageNumber = ref(1);
const postPageSize = 10;
const userStore = useUserStore();
const friendsStore = useFriendsStore();

const props = defineProps<{
    tabs: string[],
    profile: IProfile,
}>();

onMounted(async () => {
    if (props.profile) {
        await getPosts();
        await getFriends();
    }
});

async function getPosts() {
    const result = await getUserPosts(props.profile.id, postPageNumber.value, postPageSize);
    if (result) {
        posts.value = result
        arePostsLoading.value = false;
    } else {
        arePostsLoading.value = false;
    }
}

async function getFriends() {
    
    await getUsersFriends();
    await getFriendshipRequests();
    areFriendsLoading.value = false;
}

async function getUsersFriends() {
    const friends = await apiGetFriendList();
    friendsStore.setFriends(friends);
}

async function getFriendshipRequests() {
    const friendRequests = await apiGetFriendshipRequestsForUser();
    friendsStore.setFriendRequests(friendRequests);
}

const activeTab = ref(props.tabs[0]);

function setActiveTab(tabName: string) {
    activeTab.value = tabName;
}

function redirectToProfile(friend: IFriendRequestModel | IFriendModel) {
  router.push({ name: 'UsersProfile', params: { id: friend.userId } });
}

</script>
<style scoped lang="scss">
.content-right {
    display: flex;
    flex-direction: column;
    border: 1px solid #ccc;
    border-top: 0;
    border-radius: 10px;

    .content-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        border-bottom: 1px solid grey;
        margin-bottom: 0.5rem;

        @media (max-width: 388px) {
            flex-direction: column;
        }

        button {
            flex-grow: 1;
            word-wrap: break-word;
            word-spacing: normal;
            color: white;
            font-size: large;
            border-top: 1px solid #ccc;
            border-left: 1px solid #ccc;
            border-right: 0px solid #ccc;
            border-bottom: 0;

            border-top-left-radius: 10px;
            border-top-right-radius: 10px;
            width: 100%;
            padding: 10px;
            cursor: pointer;
            background-color: rgb(95, 95, 95);

            &:hover {
                background-color: #a3a3a3;
            }
        }

        .active {
            background-color: rgb(63, 63, 63);
        }
    }

    .panel {
        padding: 0.5rem;
    }

    .post-panel {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;

        a {
            height: 3rem;
            width: 3rem;
            display: flex;
            justify-content: center;
            align-items: center;
            background-color: rgb(153, 144, 144);
            border-radius: 50%;
            border: 0;
            cursor: pointer;

            img {
                height: 2rem;
                transition: transform 0.3s ease;
            }

            &:hover {
                background-color: rgb(95, 95, 95);
                z-index: 9999;

                img {
                    transform: scale(1.2, 1.2);
                    transition: transform 0.3s ease;
                }

            }
        }

        .post-div {
            display: flex;
            width: 100%;
            justify-content: center;
        }
    }
    .friend-panel{
        .friends-requests {
            a{
                padding: 0;
            }
        }
    }

}
</style>