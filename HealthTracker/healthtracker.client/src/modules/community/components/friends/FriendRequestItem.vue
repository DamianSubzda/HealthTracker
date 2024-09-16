<template>
  <main>
    <div class="friend-item" @click="handleClick">
      <div></div>
      <div class="avatar">
        <i class="bi bi-person-circle"></i>
      </div>
      <div class="person-info">
        <p>{{ friend.firstName }} {{ friend.lastName }}</p>
      </div>
      <div class="button-group">
        <button @click.stop="acceptRequest">
          <img src="@/assets/icons/check.svg" alt="Accept" style="height: 26px; width: 26px;"/>
        </button>
        <button @click.stop="declineRequest">
          <img src="@/assets/icons/close-o.svg" alt="Decline" />
        </button>
      </div>
    </div>
  </main>
</template>

<script lang="ts" setup>
import { apiPutFriendshipAccept, apiPutFriendshipDecline } from "@/api/community/friendshipController";
import { useFriendsStore, type IFriendRequestModel } from "@/modules/community/store/friendsStore"

const friendsStore = useFriendsStore();

const props = defineProps<{
  friend: IFriendRequestModel,
  onClick: (friend : IFriendRequestModel) => void,
}>()

function handleClick() {
  props.onClick(props.friend);
}

async function acceptRequest() {
  await apiPutFriendshipAccept(props.friend.userId);
  friendsStore.removeFriendRequest(props.friend.userId);
  friendsStore.addFriend(props.friend)
}

async function declineRequest() {
  await apiPutFriendshipDecline(props.friend.userId);
  friendsStore.removeFriendRequest(props.friend.userId);
}

</script>

<style lang="scss" scoped>
.friend-item {
  display: grid;
  grid-template-columns: 1fr 1fr 14fr 6fr;
  grid-column-gap: 0.5rem;
  justify-content: center;
  align-content: center;
  border-radius: 10px 0 0 10px;
  padding: 0.5rem;
  font-size: 16px;
  color: white;
  padding-left: 1rem;

  .notification {
    font-size: 30%;
    align-content: center;

    i {
      color: yellow;
      border-radius: 100%;
      box-shadow: 0px 0px 12px 2px rgba(255, 255, 0, 1);
    }
  }

  .avatar {
    height: inherit;
    display: flex;
    align-items: center;
    justify-content: end;
  }

  .person-info {
    height: inherit;
    display: inline-flex;
    text-align: center;
    align-items: center;
  }

  &:hover {
    cursor: pointer;
    background-color: rgba(100, 100, 100, 0.2);
  }

  .button-group {
    display: flex;
    flex-direction: row;
    justify-content: end;
    gap: 5px;

    @media (max-width:360px) {
      flex-direction: column;
    }

    button {
      display: flex;
      justify-content: center;
      align-items: center;
      background-color: rgb(153, 144, 144);
      border-radius: 50%;
      border: 0;
      height: 24px;
      width: 24px;

      img {
        height: 90%;
        width: 90%;
        transition: transform 0.3s ease;
        cursor: pointer;
        z-index: 9999;
      }

      &:hover {
        background-color: rgb(95, 95, 95);
        z-index: 9999;

        img {
          transform: scale(1.2, 1.2);
          transition: transform 0.3s ease;
        }
      }


    }

  }
}
</style>