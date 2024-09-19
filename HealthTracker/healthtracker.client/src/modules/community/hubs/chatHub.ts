import { useFriendsStore } from '@/modules/community/store/friendsStore';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { useUserStore } from "@/shared/store/userStore";
import { useChatStore } from "@/modules/community/store/chatStore";
import { apiPutMessagesToRead } from '@/api/community/chatController';
import config from "@/config.json"

let connection: HubConnection | null = null;

function getConnection() {
  const userStore = useUserStore();

  if (connection && (connection.state === HubConnectionState.Connected ||
                    connection.state === HubConnectionState.Connecting ||
                    connection.state === HubConnectionState.Reconnecting)) {
    return connection;
  }
  connection = new HubConnectionBuilder()
      .withUrl(`${config.serverURL}chatHub`, {
      accessTokenFactory: () => userStore.token ?? "",
    })
    .withAutomaticReconnect()
    .build();

  return connection;
}

async function connectToChatHub() {
  const chatStore = useChatStore();
  const userStore = useUserStore();

  if (!userStore.userId || !userStore.token) {
    return;
  }

  try {
    const connection = getConnection();

    if (connection.state === HubConnectionState.Disconnected) {
      await connection.start();
      console.log("Connected to Chat");
    }

    connection.off("ReceiveMessage");
    connection.on("ReceiveMessage", async (id, userFrom, userTo, message) => {
      chatStore.addMessageFromChatHub(id, message, userFrom, userTo, userStore.userId);

      if (chatStore.friendToChat && chatStore.friendToChat.userId == userFrom && chatStore.isChatExpanded == true){
        const friendsStore = useFriendsStore();
        friendsStore.resetNewMessagesCount(chatStore.friendToChat.userId);
        await apiPutMessagesToRead(chatStore.friendToChat.userId);
      }
    });
  } catch (err) {
    console.error("Error connecting to Chat:", err);
  }
}

async function disconnectWithChat(){
  if (connection && connection.state === HubConnectionState.Connected) {
    try {
      await connection.stop();
      console.log("Disconnected from Chat");
      connection = null;
    } catch (err) {
      console.error("Error disconnecting from Chat:", err);
    }
  } else {
    console.log("No active connection to disconnect");
  }
}

async function sendMesssage(messageToSend: string) {
  const userStore = useUserStore();
  const chatStore = useChatStore();
  const connection = getConnection();

  if (connection.state !== "Connected") {
    console.error(
      "Connection is not in 'Connected' state. Current state: ",
      connection.state
    );
    await connectToChatHub();
    await sendMesssage(messageToSend);
    return;
  }
  if (chatStore.friendToChat != null) {
    await connection.invoke("SendMessageToUser", userStore.userId, chatStore.friendToChat.userId, messageToSend);
  }
}

export { connectToChatHub, disconnectWithChat, sendMesssage };