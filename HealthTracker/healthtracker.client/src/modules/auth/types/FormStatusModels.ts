interface IErrorModel {
  code: string;
  description: string;
}

interface IFormStatusModel {
  success: string | null;
  errors: IErrorModel[];
}

export type { 
  IErrorModel,
  IFormStatusModel
 }