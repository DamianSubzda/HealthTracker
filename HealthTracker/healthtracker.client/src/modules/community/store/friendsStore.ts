import { defineStore } from "pinia";

interface IFriendRequestModel {
  userId: number;
  firstName: string;
  lastName: string;
}

interface IFriendModel extends IFriendRequestModel {
  newMessagesCount: number;
}

export const useFriendsStore = defineStore("friendData", {
  state: () => ({
    friends: [] as IFriendModel[],
    friendRequests: [] as IFriendRequestModel[],
  }),
  actions: {
    setFriends(friendsData: IFriendModel[]) {
      this.friends = friendsData;
    },
    addFriend(friendData: IFriendModel | IFriendRequestModel) {
      if ("newMessagesCount" in friendData) {
        this.friends.push(friendData);
      } else {
        const newFriend: IFriendModel = {
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
    setFriendRequests(requestsData: IFriendRequestModel[]) {
      this.friendRequests = requestsData;
    },
    addFriendRequest(requestData: IFriendRequestModel) {
      this.friendRequests.push(requestData);
    },
    removeFriendRequest(userId: number) {
      this.friendRequests = this.friendRequests.filter(
        (r) => r.userId !== userId
      );
    },
  },
});

export type { IFriendModel, IFriendRequestModel };
