import { useUserStore } from "@/store/account/auth";
import apiClient from "../axios";

const apiGetFriendList = async () => {
  const userStore = useUserStore();
  const response = await apiClient
    .get(`/api/users/${userStore.userId}/friends`, {
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

async function apiPostFriendshipRequest(id: number) {
  const userStore = useUserStore();

  await apiClient
    .post(
      `/api/users/friends`,
      {
        userId: userStore.userId,
        friendId: id,
      },
      {
        headers: {
          Authorization: `Bearer ${userStore.token}`,
        },
      }
    )
    .catch((error) => {
      console.log(error);
      return false;
    });
  return true;
}

async function apiGetFriendship(id: number) {
  const userStore = useUserStore();
  const response = await apiClient
    .get(`/api/users/${userStore.userId}/friends/${id}`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
    })
    .catch((error) => {
      if (error.status != 404) {
        console.log(error);
      }
      return null;
    });

  return response?.data;
}

async function apiPutFriendshipAccept(friendId: Number) {
  const userStore = useUserStore();
  await apiClient
    .put(`/api/users/${userStore.userId}/friends/${friendId}/accept`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
    })
    .catch((error) => {
      console.log(error);
    });
}

async function apiPutFriendshipDecline(friendId: Number) {
  const userStore = useUserStore();
  await apiClient
    .put(`/api/users/${userStore.userId}/friends/${friendId}/decline`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
    })
    .catch((error) => {
      console.log(error);
    });
}

async function apiDeleteFriendship(friendId: Number) {
  const userStore = useUserStore();
  await apiClient
    .delete(`/api/users/${userStore.userId}/friends/${friendId}`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
    })
    .catch((error) => {
      console.log(error);
    });
}

export {
  apiGetFriendList,
  apiPostFriendshipRequest,
  apiGetFriendship,
  apiPutFriendshipAccept,
  apiPutFriendshipDecline,
  apiDeleteFriendship,
};
