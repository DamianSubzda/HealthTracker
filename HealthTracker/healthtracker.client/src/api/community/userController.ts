import { useUserStore } from "@/shared/store/userStore";
import apiClient from "../apiClient";

async function apiGetSearchedUsers(query: string) {
  const userStore = useUserStore();
  try {
    const response = await apiClient.get(
      `/api/users/${userStore.userId}/search?query=${encodeURIComponent(query)}`,
      {
        headers: {
          Authorization: `Bearer ${userStore.token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.error("Błąd podczas wyszukiwania użytkowników:", error);
    return null;
  }
}

export { apiGetSearchedUsers };
