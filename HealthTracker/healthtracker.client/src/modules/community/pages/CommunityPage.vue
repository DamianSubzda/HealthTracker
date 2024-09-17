<template>
  <main class="community-page">

    <div class="wall" :class="`${isMobileExpanded && 'is_mobile_expanded'}`">
      <div class="wall-header">
        <SearchBar />
        <div class="friends-button">
          <button class="menu-toggle" @click="toggleMobile">
            <span class="material-icons">
              keyboard_double_arrow_down
            </span>
          </button>
        </div>
      </div>
      <div class="mobile-expander" v-if="isMobile">
        <div v-if="areFriendsLoading" style="height: calc(50% - 1rem); justify-content: center; display: flex; margin-top: 1rem;">
          <LoadingScreen :cubSize="25"/>
        </div>
        <FriendsList v-else class="list-mobile" />
        <ChatBox :is_expanded="true" class="chat-mobile" />
      </div>
      <div class="wall-body">
        <div v-if="arePostsLoading" style="justify-content: center; display: flex; margin-top: 1rem;">
          <LoadingScreen :cubSize="25"/>
        </div>
        <div v-else v-for="post in currentPosts.posts" :key="post.id" class="posts">
          <Post :post="post" />
        </div>
      </div>
    </div>

    <div class="right-content" v-if="!isMobile">
      <div v-if="areFriendsLoading" style="justify-content: center; display: flex; margin-top: 1rem;">
          <LoadingScreen :cubSize="25"/>
      </div>
      <FriendsList v-else class="list" />
      <ChatItem v-if="chatStore.friendToChat" class="chat" />
    </div>

  </main>
</template>

<script lang="ts" setup>
import FriendsList from './../components/friends/FriendsList.vue'
import ChatItem from './../components/chat/ChatItem.vue'
import ChatBox from './../components/chat/ChatBox.vue';
import Post from './../components/post/PostSection.vue'
import SearchBar from './../components/SearchBar.vue'
import LoadingScreen from '@/shared/components/LoadingWidget.vue'
import { currentPosts } from '@/data/models/postModels';
import { ref, onMounted, onUnmounted, computed } from "vue";
import { getPostOnWall } from '@/api/community/postController';
import { apiGetFriendList } from '@/api/community/friendshipController';
import { useUserStore } from '@/modules/auth/store/userStore';
import { useChatStore } from './../store/chatStore';
import { useFriendsStore } from './../store/friendsStore';
import { connectToChatHub } from './../hubs/chatHub'
import { getNumberOfNewMessagesForFriend } from '@/api/community/chatController';

const chatStore = useChatStore();
const userStore = useUserStore();
const friendsStore = useFriendsStore();
const isMobile = ref(window.innerWidth < 785 || window.innerHeight < 590);
const isButtonExpandedClicked = ref(false);
const isMobileExpanded = computed(() => isMobile.value && isButtonExpandedClicked.value);

const postPageNumber = ref(1)
const postPageSize = 10

const arePostsLoading = ref(true);
const areFriendsLoading = ref(true);

onMounted(async () => {
  window.addEventListener('resize', handleResize);
  await connectToChatHub();
  await getFriends();
  await getPosts();
});

onUnmounted(() => {
  window.removeEventListener('resize', handleResize);
});

function toggleMobile() {
  isButtonExpandedClicked.value = !isButtonExpandedClicked.value
}

function handleResize() {
  isMobile.value = window.innerWidth < 785 || window.innerHeight < 590;
}

async function getFriends() {
  if (!userStore.userId) {
    return;
  }
  const response = await apiGetFriendList();
  if (response != null) {
    friendsStore.friends = response.map((friend: any) => ({
      ...friend,
      newMessagesCount: 0
    }));

    friendsStore.friends.forEach(async (friend) => {
      const response = await getNumberOfNewMessagesForFriend(friend.userId);
      friend.newMessagesCount = response; 
    });
    areFriendsLoading.value = false;
  }
}

async function getPosts() {
  const posts = await getPostOnWall(postPageNumber.value, postPageSize);
  if (posts) {
    currentPosts.value.posts = posts
    arePostsLoading.value = false;
  } else {
    arePostsLoading.value = false;
  }
}

</script>

<style lang="scss" scoped>
.community-page {
  position: fixed;
  display: flex;
  height: calc(100% - 4rem);
  width: calc(100% - 4rem);
  justify-items: stretch;
  align-items: stretch;

  .wall {
    display: flex;
    flex-direction: column;
    grid-column: 1;
    align-items: center;
    width: 80%;
    height: 100%;

    .wall-header {
      display: flex;
      grid-row: 1;
      grid-column: 1;
      border-bottom: 1px solid #d3d3d3;
      width: 100%;
      height: 4rem;
      position: sticky;
      top: 0;
      background-color: inherit;
      z-index: 10;
      align-content: center;

      .friends-button {
        display: none;
        justify-content: center;
        align-content: center;
        flex-wrap: wrap;
        padding: 0.5rem;

        .friends-button button {
          height: fit-content;
          width: fit-content;
        }

        .menu-toggle {
          .material-icons {
            transition: 0.5s ease-out;
          }
        }
      }
    }

    .mobile-expander {
      height: 0;
      width: inherit;
      overflow: hidden;
      opacity: 0;
      transition: height 1.5s ease, opacity 1s ease;

      .list-mobile {
        height: 50%;
        margin-bottom: 0;
      }

      .chat-mobile {
        height: 50%;
      }
    }

    .wall-body {
      grid-row: 2;
      grid-column: 1;
      height: calc(100% - 4rem);
      width: 100%;
      overflow-y: scroll;
      overflow-x: hidden;
      transition: height 0.5s ease-out;
      scrollbar-width: thin;

      .posts {
        display: flex;
        width: 100%;
        justify-content: center;
      }
    }

    &.is_mobile_expanded {
      .mobile-expander {
        height: calc(100% - 4rem);
        opacity: 1;
        transition: 1s ease-in;
      }

      .wall-body {
        height: 0;
        transition: height 1s ease-in;
      }

      .wall-header .friends-button .menu-toggle .material-icons {
        transform: scaleY(-1);
        transition: 0.7s ease-out;
      }
    }
  }

  .right-content {
    width: 25%;
    height: 100%;
    display: flex;
    justify-content: space-between;
    flex-direction: column;
    transition: width 0.3s ease-out;

    .list {
      flex-grow: 1;
    }

    .chat {
      flex-shrink: 0;
      height: calc(4rem + 35vh);
    }
  }

  @media (max-height: 590px),
  (max-width: 785px) {
    .wall {
      width: 100%;

      .wall-header {
        .friends-button {
          display: flex;
        }
      }
    }

    .right-content {
      display: none;
    }
  }
}
</style>