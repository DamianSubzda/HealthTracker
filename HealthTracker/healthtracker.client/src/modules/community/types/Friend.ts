interface IFriendRequest {
  userId: number;
  firstName: string;
  lastName: string;
}

interface IFriend extends IFriendRequest {
  newMessagesCount: number;
}

export type { IFriend, IFriendRequest };
