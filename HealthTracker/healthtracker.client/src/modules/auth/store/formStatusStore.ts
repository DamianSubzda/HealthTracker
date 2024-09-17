import { defineStore } from "pinia";

interface IErrorModel {
  code: string;
  description: string;
}

interface IFormStatusModel {
  success: string | null;
  errors: IErrorModel[];
}

export const useFormStatusStore = defineStore("formStatus", {
  state: (): IFormStatusModel => ({
    success: null,
    errors: [],
  }),
  actions: {
    setFormStatus(formData: IFormStatusModel) {
      this.errors = formData.errors;
      this.success = formData.success;
    },
    setErrors(errors: IErrorModel[]) {
      this.errors = errors;
    },
    setSuccess(sucess: string) {
      this.success = sucess;
    },
    clearFormStatus() {
      this.errors = [];
      this.success = null;
    },
  },
});
