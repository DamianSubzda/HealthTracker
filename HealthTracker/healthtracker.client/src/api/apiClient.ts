import router from "@/router";
import axios from "axios";
import config from "@/config.json";

const apiClient = axios.create({
  baseURL: config.serverURL,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response && error.response.status === 401) {
      console.error("User unauthorized!");
      router.push("/logout");
    }
    return Promise.reject(error);
  }
);

export default apiClient;
