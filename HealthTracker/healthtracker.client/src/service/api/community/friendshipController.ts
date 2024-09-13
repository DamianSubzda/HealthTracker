import { useUserStore } from "@/store/account/auth";
import apiClient from "../axios";

async function apiGetFriendList() {
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
}

async function apiGetFriendshipRequestsForUser(){
  const userStore = useUserStore();
  const respone = await apiClient.get(`/api/users/${userStore.userId}/friends/requests`, {
    headers: {
      Authorization: `Bearer ${userStore.token}`,
    }
  }).catch((error) => {
    console.log(error);
    return null;
  })

  return respone?.data;
}

async function apiGetFriendship(friendId: number) {
  const userStore = useUserStore();
  const response = await apiClient
    .get(`/api/users/${userStore.userId}/friends/${friendId}`, {
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

async function apiPostFriendshipRequest(friendId: number) {
  const userStore = useUserStore();
  await apiClient
    .post(
      `/api/users/friends`,
      {
        userId: userStore.userId,
        friendId: friendId,
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
  apiGetFriendshipRequestsForUser,
};
