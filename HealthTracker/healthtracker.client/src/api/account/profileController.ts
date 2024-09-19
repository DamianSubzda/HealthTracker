import { useUserStore } from "@/shared/store/userStore";
import apiClient from "../apiClient";

interface IProfile {
  id: number;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  about: string;
  phoneNumber: string;
  dateOfBirth: string;
  dateOfCreate: string;
  profilePicture: string;
}

async function apiGetProfileById(userId: number) {
  const userStore = useUserStore();
  try {
    const response = await apiClient.get(`api/users/${userId}`, {
      headers: {
        Authorization: `Bearer ${userStore.token}`,
      },
    });
    const profile: IProfile = response.data;
    return profile;
  } catch (error) {
    console.error("Error fetching user:", error);
    return null;
  }
}

async function apiPostUserPhoto(userId: number, image: HTMLInputElement | null) {
  if (!userId || !image || !image.files) {
    return null;
  }

  const formData = new FormData();
  formData.append('photo', image.files[0]);

  try {
    const userStore = useUserStore();
    const response = await apiClient.post(
      `/api/users/${userId}/photo`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',       
          Authorization: `Bearer ${userStore.token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.error("Failed to upload photo:", error);
    return null;
  }
}

export type { IProfile };
export { apiGetProfileById, apiPostUserPhoto };
