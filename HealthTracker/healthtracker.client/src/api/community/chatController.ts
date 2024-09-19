import { useUserStore } from "@/shared/store/userStore";
import apiClient from "../apiClient";

const apiGetMessagesWithFriend = async (
    friendId: number | null,
    pageNumber: number,
    pageSize: number
) => {
  const userStore = useUserStore();
  const response = await apiClient.get(`/api/users/messages/${userStore.userId}/${friendId}/`, {
    headers: {
      'Authorization': `Bearer ${userStore.token}`
    }, 
    params: {
      pageNumber: pageNumber,
      pageSize: pageSize
    }
  })
    .catch((error) => {
      console.log(error);
      return null;
    });
  return response?.data;
};

const apiGetNumberOfNewMessagesForFriend = async (
  friendId: number
) => {
const userStore = useUserStore();
const response = await apiClient.get(`/api/users/messages/${friendId}/${userStore.userId}/new`, {
  headers: {
    'Authorization': `Bearer ${userStore.token}`
  }
})
  .catch((error) => {
    console.log(error);
    return { data: 0 };
  });
return response?.data;
};

const apiPutMessagesToRead = async(userFrom: number) => {
  const userStore = useUserStore();
  await apiClient.put(`/api/users/messages/${userStore.userId}/${userFrom}`, {}, {
    headers: {
      'Authorization': `Bearer ${userStore.token}`,
    }
  })
    .catch((error) => {
      console.log(error);
    });
}

export { apiGetMessagesWithFriend, apiPutMessagesToRead, apiGetNumberOfNewMessagesForFriend };
