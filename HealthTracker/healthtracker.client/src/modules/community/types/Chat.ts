import type { IFriend } from "./Friend";

interface IMessage {
    id: number;
    text: string;
    isYours: boolean;
    isReaded: boolean;
  }
  
  interface IChat {
    messages: IMessage[];
    friendToChat: IFriend | null;
    isChatExpanded: boolean;
    isLoadingOlderMessages: boolean;
    pageNumber: number;
    pageSize: number;
  }

export type { IMessage, IChat };