<template>
  <main class="post">
    <div class="content">
      <div class="header">
        <p>{{ localPost.userFirstName }} {{ localPost.userLastName }}</p>
      </div>
      <div class="main">
        <div v-html="safeHtml" className="post-text"></div>
        <div class="attachment" v-if="localPost.imageURL">
          <img :src="`${config.serverURL}${localPost.imageURL}`" alt="Attached image"
            style="width: 100%; border-radius: 0.5rem;" />
        </div>
      </div>
      <div class="footer">
        <button class="like" @click="presslikePostButton">
          <i class="bi bi-hand-thumbs-up-fill"></i>&nbsp;{{ localPost.likes.length }}
        </button>
        <button class="comment" @click="toggleComments">
          <i class='bi bi-chat-dots-fill'></i>&nbsp;{{ commentsCount }}
        </button>
      </div>
      <!--Show after click-->
      <div class="comment-section" v-if="isCommentsVisible">
        <div class="comment-input">
          <button @click="addComment"><i class='bi bi-send-fill'></i></button>
          <input type="text" v-model="commentToAdd" placeholder="Write comment...">
        </div>
        <UsersComment v-for="comment in comments" :key="comment.id" :item="comment" :depth=0 :post-id=comment.postId />
        <button v-if="isMoreComments" class="load-comments-button" @click="getComments">Load more comments...</button>
      </div>
    </div>
  </main>
</template>

<script lang="ts" setup>
import config from "@/config.json"
import UsersComment from './UsersComment.vue'
import { ref, computed, onMounted } from 'vue';
import DOMPurify from 'dompurify';
import MarkdownIt from 'markdown-it';
import { type IPost } from '@/data/models/postModels';
import type { IComment } from '@/data/models/postModels';
import { getPostComments, likePostByPostId, deleteLikeByPostId, addCommentToPost } from '@/api/community/postController';
import { useUserStore } from "@/modules/auth/store/userStore";

const props = defineProps<{
  post: IPost
}>();

const localPost = props.post;

const userStore = useUserStore();
const comments = ref<IComment[]>([]);
const commentsCount = ref(localPost.amountOfComments)
const isCommentsVisible = ref(false);
const isMoreComments = ref(false);
const commentToAdd = ref('');
const pageNr = ref(0);
const pageSize = 10;

const safeHtml = computed(() => {
  const md = new MarkdownIt();
  const rawHtml = md.render(localPost.content);
  return DOMPurify.sanitize(rawHtml);
});

onMounted(async () => {
  await getComments();
});

function toggleComments() {
  isCommentsVisible.value = !isCommentsVisible.value;
  pageNr.value = 0;
}

async function presslikePostButton() {
  const likeIndex = localPost.likes.findIndex((like) => like.userId === userStore.userId)

  if (likeIndex > -1) {
    const respone = await deleteLikeByPostId(localPost.id);
    if (respone != null) {
      localPost.likes.splice(likeIndex, 1);
    }
  } else {
    const response = await likePostByPostId(localPost.id);
    localPost.likes.push(response);
  }
}

async function getComments() {
  pageNr.value += 1;
  const recivedData = await getPostComments(localPost.id, pageNr.value, pageSize);
  if (recivedData.comments) {
    recivedData.comments.forEach((element: IComment) => {
      comments.value.push(element);
    });
    if (recivedData.totalCommentsLeft > 0) {
      isMoreComments.value = true;
    } else {
      isMoreComments.value = false;
    }
  }
}

async function addComment() {
  if (commentToAdd.value) {
    const response = await addCommentToPost(localPost.id, commentToAdd.value)
    if (response != null) {
      commentsCount.value += 1;
      comments.value.unshift(response);
      commentToAdd.value = '';
    }
  } else {
    console.log("Can't add an empty comment.");
  }
}


</script>

<style lang="scss" scoped>
.post {
  margin-top: 0.5rem;
  margin-bottom: 0.5rem;
  width: 95%;
  border: 1px solid black;
  border-radius: 1rem;
  background-color: rgba(62, 50, 50, 1);

  .content {
    width: 100%;

    .header {
      border-radius: 1rem 1rem 0 0;
      padding: 0.5rem;
    }

    .main {
      border-top: 2px solid rgb(73, 61, 61);
      padding: 1rem;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;

      .post-text {
        word-wrap: break-word;
        white-space: normal;
        width: 100%;
      }

      .attachment {
        padding-top: 0.5rem;

        img {
          width: 100%;
        }
      }
    }

    .footer {
      border-radius: 0 0 1rem 1rem;
      text-align: center;
      display: flex;
      height: 3rem;
      padding: 0.4rem;
      gap: 0.2rem;

      button {
        background-color: rgb(73, 61, 61);
        color: white;
        width: 100%;
        border: none;
        border-radius: 1rem;

        &:hover {
          background-color: rgb(112, 112, 112);
        }
      }
    }

    .footer button i {
      display: inline-block;
      transition: transform 0.3s ease;
    }

    .footer button:active i {
      transform: scale(1.1);
    }

    .comment-section {
      padding: 1rem;
      width: inherit;
      display: flex;
      flex-direction: column;

      .comment-input {
        display: flex;
        height: 15%;
        gap: 2px;
        width: inherit;

        button {
          background-color: rgb(73, 61, 61);
          cursor: pointer;
          border: none;
          width: 3.5rem;
          border-radius: 1.5rem;

          &:hover {
            background-color: rgb(112, 112, 112);
          }

          // &:active {

          // }
        }

        button i {
          display: inline-block;
          transition: transform 0.3s ease;
        }

        button:active i {
          transform: scale(1.5);
        }

        input {
          height: 2.5rem;
          background-color: rgb(73, 61, 61);
          color: white;
          width: 100%;
          border: 0;
          outline: 0;
          padding-left: 1rem;
          font-weight: 400;
          border-radius: 1.5rem;

          // &:hover {}

          &:active {
            border: 0;
          }

          &:focus {
            border: 0;
          }
        }
      }

      .load-comments-button {
        color: white;
        background-color: rgb(73, 61, 61);
        cursor: pointer;
        border: none;
        padding: 0.5rem;
        border-radius: 1.5rem;
        margin-top: 0.5rem;

        &:hover {
          background-color: rgb(112, 112, 112);
        }
      }
    }
  }

}
</style>