import { useFormStatusStore } from '@/modules/auth/store/formStatusStore';
import apiClient from "./../apiClient";
import type { ILoginModel, IRegisterModel } from "@/modules/auth/types/FormModels";

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


export { apiPostLogin, apiPostRegister };
