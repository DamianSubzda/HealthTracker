import { useFormStatusStore } from '@/modules/auth/store/formStatusStore';
import apiClient from "./../apiClient";
import type { ILoginModel, IRegisterModel } from "@/data/models/formDataModels";

// const tasksForEndpoint = (endpoint: string, responseContent: any) => {
//   if (endpoint == "/login") {
//     localStorage.setItem("user", JSON.stringify(responseContent));
//     formStatus.value.success = "User is logged";
//     const userStore = useUserStore();
//     router.push("/").then(() => {
//       userStore.updateUserData();
//     });
//   } else {
//     formStatus.value.success = responseContent;
//     document.getElementById("reset_button")!.click();
//   }
// };

// const sendData = async (endpoint: string, postData: string) => {
//   const result: IResponseModel = {
//     status: false,
//     content: "",
//   };
//   try {
//     const { data } = await apiClient.post(`/api${endpoint}`, postData);
//     result.status = true;
//     if (endpoint == "/login") {
//       result.content = data;
//     } else {
//       result.content = data.message;
//     }
//   } catch (error: any) {
//     result.status = false;
//     result.content = error.response.data;
//   }
//   return result;
// };

// const preventSubmit = async (endpoint: string, data: string) => {
//   clearFormStatus();
//   const response: IResponseModel = await sendData(endpoint, data);
//   if (response.status) {
//     tasksForEndpoint(endpoint, response.content);
//   } else {
//     response.content.forEach((element: { description: string }) => {
//       formStatus.value.errors.push(element.description);
//     });
//   }
// };

async function apiPostLogin(loginData: ILoginModel) {
  const formStatusStore = useFormStatusStore();
  formStatusStore.clearFormStatus();
  const result = await apiClient
    .post(`/login`, loginData)
    .catch((error) => {
      formStatusStore.setErrors(error.response.data);
      return null;
    });
  if (result === null) {
    return null;
  }
  formStatusStore.setSuccess("User is logged");
  return result;
}

async function apiPostRegister(registerData: IRegisterModel) {
  console.log(registerData);
  const formStatusStore = useFormStatusStore();
  formStatusStore.clearFormStatus();
  const result = await apiClient
    .post(`/register`, registerData)
    .catch((error) => {
      formStatusStore.setErrors(error.response.data);
      return null;
    });
  if (result === null) {
    return null;
  }
  console.log(result);
  formStatusStore.setSuccess(result.data.message);
  return result;
}

async function apiGetLoginWithGoogle() {
  const formStatusStore = useFormStatusStore();
  const result = await apiClient
  .get(`/login-google`)
  .catch((error) => {
    formStatusStore.setErrors(error.response.data);
    return null;
  });
  if (result === null) {
    return null;
  }
  formStatusStore.setSuccess("User is logged");
  return result;
}

export { apiPostLogin, apiPostRegister, apiGetLoginWithGoogle };
