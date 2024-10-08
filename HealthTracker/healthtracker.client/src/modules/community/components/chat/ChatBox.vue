<template>
  <div class="chat-messinput">
    <div class="messages" ref="messagesContainer" @scroll="handleScroll">
      <div v-for="message in chatStore.messages"
        :class="['message', message.isYours ? 'own-message' : 'received-message']" :key="message.id">
        {{ message.text }}
      </div>
    </div>
    <div class="chat-input">
      <button @click="sendMessageToHub"><i class='bi bi-send-fill'></i></button>
      <input type="text" v-model="messageToSend" placeholder="Write message..." @keyup.enter="sendMessageToHub" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick, watch } from 'vue';
import { useUserStore } from "@/shared/store/userStore";
import { useFriendsStore } from '@/modules/community/store/friendsStore';
import { useChatStore } from '@/modules/community/store/chatStore';
import { apiGetMessagesWithFriend, apiPutMessagesToRead } from '@/api/community/chatController';
import { sendMesssage } from './../../hubs/chatHub';


const userStore = useUserStore();
const chatStore = useChatStore();
const friendsStore = useFriendsStore();
const messagesContainer = ref();
const messageToSend = ref('');

onMounted(async () => {
  chatStore.pageNumber = 1;
  await loadMessages();
  await nextTick(() => {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
  });
});

watch(
  () => chatStore.friendToChat,
  async () => {
    chatStore.pageNumber = 1;
    chatStore.setMessages([]);
    await loadMessages();
    await nextTick(() => {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    });
    if (chatStore.isChatExpanded) {
      await resetNewMessagesCount();
    }
  },
);

watch(
  () => chatStore.isChatExpanded,
  async () => {
    if (chatStore.isChatExpanded == true) {
      await resetNewMessagesCount();
    }
  });

watch(
  () => chatStore.messages,
  async () => {
    if (chatStore.isChatExpanded) {
      await nextTick(() => {
        if (!chatStore.isLoadingOlderMessages) {
          messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
        }
      });
    }
  }, { deep: true });


const handleScroll = async () => {
  if (messagesContainer.value.scrollTop === 0 && chatStore.pageNumber > 1) {
    await loadMessages();
  }
};

async function resetNewMessagesCount() {
  if (chatStore.friendToChat) {
    friendsStore.resetNewMessagesCount(chatStore.friendToChat.userId);
    await apiPutMessagesToRead(chatStore.friendToChat.userId);
  }
}

async function loadMessages() {
  if (chatStore.friendToChat == null) return;
  const actualScrollHeight = messagesContainer.value.scrollHeight;
  const response = await apiGetMessagesWithFriend(chatStore.friendToChat.userId, chatStore.pageNumber, chatStore.pageSize);
  if (response.length > 0) {
    chatStore.addMessagesFromAPI(response, userStore.userId);
    chatStore.pageNumber++;
    await nextTick();
    messagesContainer.value.scrollTop += (messagesContainer.value.scrollHeight - actualScrollHeight);
  }
}

async function sendMessageToHub() {
  if (messageToSend.value.trim() !== '' && chatStore.friendToChat !== null) {
    try {
      if (userStore.userId) {
        await sendMesssage(messageToSend.value);
        messageToSend.value = '';
        nextTick().then(() => {
          messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
        });
      }
    } catch (err) {
      console.error(err);
    }
  }
}
</script>

<style lang="scss" scoped>
.chat-messinput {
  grid-column: 2/3;
  background-color: rgb(199, 167, 167);
  border-radius: 0 0 0 10px;
  height: inherit;

  .messages {
    overflow-y: auto;
    scrollbar-width: thin;
    scrollbar-color: #888 #f0f0f0;
    height: 85%;
    padding: 5px;
    background-color: rgb(199, 167, 167);
    border-bottom: 1px solid rgb(182, 152, 152);


    &::-webkit-scrollbar {
      width: 8px;
    }

    &::-webkit-scrollbar-track {
      background: #f0f0f0;
    }

    &::-webkit-scrollbar-thumb {
      background: #888;
    }

    &::-webkit-scrollbar-thumb:hover {
      background: #555;
    }

    .message {
      display: flex;
      overflow-wrap: anywhere;
      margin-bottom: 10px;
      margin-right: 1rem;
      margin-left: 1rem;
      padding: 8px;
      border-radius: 10px;
      border: 1px solid;
      max-width: 60%;
      width: fit-content;
      color: black;
    }

    .own-message {
      justify-content: end;
      margin-left: auto;
      background-color: #6fc5ff;
      border-color: #67b7ec;
    }

    .received-message {
      justify-content: start;
      background-color: #fff;
      border-color: rgb(221, 221, 221);
    }
  }

  .chat-input {
    display: flex;
    height: 15%;
    gap: 2px;
    background-color: rgb(199, 167, 167);

    button {
      cursor: pointer;
      background-color: rgb(199, 167, 167);
      border: none;
      width: 3.5rem;
      border-radius: 1.5rem;

      &:hover {
        background-color: rgb(182, 152, 152);
      }

      &:active {
        background-color: rgb(182, 152, 152);
      }
    }

    button i {
      display: inline-block;
      transition: transform 0.3s ease;
    }

    button:active i {
      transform: scale(1.5);
    }

    input {
      width: 100%;
      background-color: rgb(199, 167, 167);
      border: 0;
      outline: 0;
      padding-left: 1rem;
      font-weight: 900;

      &:hover {
        background-color: rgb(182, 152, 152);
      }

      &:active {
        background-color: rgb(182, 152, 152);
        border: 0;
      }

      &:focus {
        background-color: rgb(182, 152, 152);
        border: 0;
        transition: border-radius 0.3s ease-in;
        border-radius: 1.5rem;
      }
    }
  }
}
</style>