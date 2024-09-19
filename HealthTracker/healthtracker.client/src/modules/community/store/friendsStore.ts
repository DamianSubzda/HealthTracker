import { defineStore } from "pinia";
import type { IFriend, IFriendRequest } from "../types/Friend";

export const useFriendsStore = defineStore("friendData", {
  state: () => ({
    friends: [] as IFriend[],
    friendRequests: [] as IFriendRequest[],
  }),
  actions: {
    setFriends(friendsData: IFriend[]) {
      this.friends = friendsData;
    },
    addFriend(friendData: IFriend | IFriendRequest) {
      if ("newMessagesCount" in friendData) {
        this.friends.push(friendData);
      } else {
        const newFriend: IFriend = {
          ...friendData,
          newMessagesCount: 0,
        };
        this.friends.push(newFriend);
      }
    },
    setNewMessagesCount(userId: number, count: number) {
      const friend = this.friends.find((f) => f.userId === userId);
      if (friend) {
        friend.newMessagesCount = count;
      }
    },
    resetNewMessagesCount(userId: number) {
      const friend = this.friends.find((f) => f.userId === userId);
      if (friend) {
        friend.newMessagesCount = 0;
      }
    },
    incrementNewMessagesCount(userId: number) {
      const friend = this.friends.find((f) => f.userId === userId);
      if (friend) {
        friend.newMessagesCount++;
      }
    },
    //Requests
    setFriendRequests(requestsData: IFriendRequest[]) {
      this.friendRequests = requestsData;
    },
    addFriendRequest(requestData: IFriendRequest) {
      this.friendRequests.push(requestData);
    },
    removeFriendRequest(userId: number) {
      this.friendRequests = this.friendRequests.filter(
        (r) => r.userId !== userId
      );
    },
  },
});
