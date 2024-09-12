<template>
    <div class="content-right">
        <div class="content-header">
            <button v-for="tab in tabs" :key="tab" @click="setActiveTab(tab)" :class="{ active: activeTab === tab }">
                {{ tab }}
            </button>
        </div>
        <!-- Zawartość zakładek -->
        <div v-if="activeTab === 'Posts'" className="post-panel panel">
            <router-link :to="`post/create`">
                <img src="@/assets/icons/add.svg" alt="Insert image" />
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
            <p>Friends confirmations here...</p>
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Post from './../../../community/post/PostSection.vue'
import LoadingScreen from '@/components/shared/LoadingScreen.vue';
import { getUserPosts } from './../../../../../service/api/community/postController'
import { type IPost } from '@/data/models/postModels';

const posts = ref<IPost[] | null>(null);
const arePostsLoading = ref(true);
const postPageNumber = ref(1);
const postPageSize = 10;

const props = defineProps({
    tabs: {
        type: Array as () => string[],
        default: () => [],
    },
    profile: {
        type: Object,
        default: null
    }
});

onMounted(async () => {
    await getPosts();
});

async function getPosts() {
    const result = await getUserPosts(props.profile.id, postPageNumber.value, postPageSize);
    if (result) {
        posts.value = result
        arePostsLoading.value = false;
    } else {
        console.error("Failed to load posts or no posts available");
    }
}

const activeTab = ref(props.tabs[0]);

function setActiveTab(tabName: string) {
    activeTab.value = tabName;
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
        display: grid;
        grid-template-columns: 1fr 1fr 1fr;
        border-bottom: 1px solid grey;
        margin-bottom: 0.5rem;

        @media (max-width: 328px) {
        grid-template-columns: 1fr;
        }

        button {
            word-wrap:break-word;
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
            border-radius: 50rem;
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


}
</style>