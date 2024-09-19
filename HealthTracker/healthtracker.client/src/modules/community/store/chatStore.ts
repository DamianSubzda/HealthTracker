import { useFriendsStore } from './../store/friendsStore';
import { defineStore } from "pinia";
import { useUserStore } from "@/shared/store/userStore";
import type { IChat, IMessage } from '../types/Chat';
import type { IFriend } from '../types/Friend';


export const useChatStore = defineStore("chatData", {
  state: (): IChat => ({
    messages: [],
    friendToChat: null,
    isChatExpanded: false,
    isLoadingOlderMessages: false,
    pageNumber: 1,
    pageSize: 10,
  }),
  actions: {
    setChatData(chatData: IChat) {
      this.messages = chatData.messages;
      this.friendToChat = chatData.friendToChat;
      this.isChatExpanded = chatData.isChatExpanded,
      this.pageNumber = chatData.pageNumber;
      this.pageSize = chatData.pageSize;
    },
    setMessagesFromAPI(
      messageData: { id: number; text: string; userIdFrom: number; isReaded: boolean }[]
    ) {
      this.isLoadingOlderMessages = false;
      const userStore = useUserStore();
      this.messages = messageData.map(
        (message: { id: number; text: string; userIdFrom: number; isReaded: boolean }) => ({
          id: message.id,
          text: message.text,
          isYours: message.userIdFrom === userStore.userId,
          isReaded: message.isReaded
        })
      );
    },
    setMessages(messageData: IMessage[]) {
      this.messages = messageData;
    },
    addMessagesFromAPI(
      messageData: { id: number; text: string; userIdFrom: number; isReaded: boolean }[],
      userId: number | null
    ) {
      this.isLoadingOlderMessages = true;
      const newMessages = messageData.map(
        (message: { id: number; text: string; userIdFrom: number; isReaded: boolean }) => ({
          id: message.id,
          text: message.text,
          isYours: message.userIdFrom === userId,
          isReaded: message.isReaded
        })
      ).reverse();
      this.messages = [...newMessages, ...this.messages];
    },
    addMessageFromChatHub(
      id: number,
      message: string,
      userFrom: number,
      userTo: number,
      userId: number | null
    ) {
      this.isLoadingOlderMessages = false;
      const friendsStore = useFriendsStore();
      const isYours = userFrom === userId;
      this.messages.push({
        id: id,
        text: message,
        isYours: isYours,
        isReaded: isYours
      });
      if (!isYours) {
        friendsStore.incrementNewMessagesCount(userFrom);
      }
    },
    clearUserData() {
      this.messages = [];
      this.friendToChat = null;
      this.isChatExpanded = false;
      this.pageNumber = 1;
      this.pageSize = 10;
    },
    setFriendToChat(friend: IFriend){
      this.friendToChat = friend;
    }
  },
});