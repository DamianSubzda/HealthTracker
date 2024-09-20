interface IPost {
  id: number;
  userId: number;
  userFirstName: string;
  userLastName: string;
  content: string;
  imageURL: string;
  dateOfCreate: string;
  amountOfComments: number;
  likes: ILike[];
}

interface IComment {
  id: number;
  postId: number;
  userId: number;
  userFirstName: string;
  userLastName: string;
  parentCommentId: number | null;
  amountOfChildComments: number;
  content: string;
  dateOfCreate: string;
}

interface ILike {
  userId: number;
  postId: number;
}

export type { IPost, ILike, IComment };
