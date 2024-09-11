import { useUserStore } from "@/store/account/auth";
import apiClient from "../axios";

const getPostOnWall = async (pageNumber: number, pageSize: number) => {
  const userStore = useUserStore();
  if (!userStore.userId) {
    console.log("No user ID provided");
    return null;
  }
  const response = await apiClient
    .get(`/api/users/${userStore.userId}/wall/posts`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
      params: {
        pageNumber: pageNumber,
        pageSize: pageSize,
      },
    })
    .catch((error) => {
      console.log(error);
      return null;
    });

  return response?.data;
};

const getUserPosts = async(userId: number | null, pageNumber: number, pageSize: number) => {
  const userStore = useUserStore();
  if (!userId || !userStore.userId) {
    console.log("No user ID provided");
    return null;
  }
  const response = await apiClient
    .get(`/api/users/${userId}/posts`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
      params: {
        pageNumber: pageNumber,
        pageSize: pageSize,
      },
    })
    .catch((error) => {
      console.log(error);
      return null;
    });

  return response?.data;
}

const getPostComments = async (
  postId: number,
  pageNumber: number,
  pageSize: number
) => {
  const userStore = useUserStore();

  if (!postId) {
    return;
  }
  const response = await apiClient
    .get(`/api/users/posts/${postId}/comments`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
      params: {
        pageNumber: pageNumber,
        pageSize: pageSize,
      },
    })
    .catch((error) => {
      console.log(error);
      return null;
    });

  return response?.data;
};

const getChildComments = async (postId: number, parentCommentId: number | null) => {
  const userStore = useUserStore();
  const response = await apiClient
    .get(`/api/users/posts/${postId}/comments/${parentCommentId}`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
    })
    .catch((error) => {
      console.log(error);
      return null;
    });

  return response?.data;
};

const likePostByPostId = async (postId: number) => {
  const userStore = useUserStore();
  const response = await apiClient
    .post(
      `/api/users/posts/likes`,
      {
        userId: userStore.userId,
        postId: postId,
      },
      {
        headers: {
          Authorization: `Bearer ${userStore.token}`,
        },
      }
    )
    .catch((error) => {
      console.log(error);
      return null;
    });

  return response?.data;
};

const deleteLikeByPostId = async (postId: number) => {
  const userStore = useUserStore();
  await apiClient
    .delete(`/api/users/${userStore.userId}/posts/${postId}/likes`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
    })
    .catch((error) => {
      console.log(error);
      return null;
    });

  return true;
};

const addCommentToPost = async (postId: number, content: string) => {
  const userStore = useUserStore();
  const response = await apiClient
    .post(
      `/api/users/posts/comments`,
      {
        postId: postId,
        userId: userStore.userId,
        content: content,
      },
      {
        headers: {
          Authorization: `Bearer ${userStore.token}`,
        },
      }
    )
    .catch((error) => {
      console.log(error);
      return null;
    });
  return response?.data;
};

const addCommentToParent = async (
  postId: number,
  commentId: number,
  content: string
) => {
  const userStore = useUserStore();
  const response = await apiClient
    .post(
      `/api/users/posts/comments/${commentId}`,
      {
        postId: postId,
        userId: userStore.userId,
        content: content,
      },
      {
        headers: {
          Authorization: `Bearer ${userStore.token}`,
        },
      }
    )
    .catch((error) => {
      console.log(error);
      return null;
    });

  return response?.data;
};

const createPost = async (userId: Number, content: string, image: HTMLInputElement | null) => {
  
  if (!userId) {
    return null;
  }
  const formData = new FormData();
  formData.append('userId', userId.toString());
  formData.append('content', content);
  if (image && image?.files) {
    formData.append('imageFile', image.files[0]);
  }
  console.log(formData);
  const userStore = useUserStore();
  const response = await apiClient
    .post(`/api/users/posts`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        Authorization: `Bearer ${userStore.token}`,
      },
    })
    .catch((error) => {
      console.log(error);
      return null;
    });

  return response?.data;
};


export {
  getPostOnWall,
  getUserPosts,
  getPostComments,
  getChildComments,
  likePostByPostId,
  deleteLikeByPostId,
  addCommentToPost,
  addCommentToParent,
  createPost,
};
