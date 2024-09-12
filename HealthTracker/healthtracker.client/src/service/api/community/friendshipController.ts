import { useUserStore } from "@/store/account/auth";
import apiClient from "../axios";

const getFriendList = async () => {
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

async function postFriendshipRequest(id: number) {
  const userStore = useUserStore();

  await apiClient
    .post(
      `/api/users/friends`,
      {
        user1Id: userStore.userId,
        user2Id: id,
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

async function getFriendship(id: number) {
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

export { getFriendList, postFriendshipRequest, getFriendship };