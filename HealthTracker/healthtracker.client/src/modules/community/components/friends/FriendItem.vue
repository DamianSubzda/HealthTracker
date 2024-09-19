<template>
  <main>
    <div class="friend-item" @click="handleClick">
      <div class="notification">
        <i v-if="friend.newMessagesCount > 0" class="bi bi-circle-fill"></i>
      </div>
      <div class="avatar">
        <i class="bi bi-person-circle"></i>
      </div>
      <div class="person-info">
        <p>{{ friend.firstName }} {{ friend.lastName }}</p>
      </div>
    </div>
  </main>
</template>

<script lang="ts" setup>
import type { IFriend } from "@/modules/community/types/Friend.ts"

const props = defineProps<{
  friend: IFriend,
  onClick: (friend : IFriend) => void,
}>()

function handleClick() {
  props.onClick(props.friend);
}

</script>

<style lang="scss" scoped>
.friend-item{
  display: grid;
  grid-template-columns: 1fr 1fr 20fr;
  grid-column-gap: 0.5rem;
  justify-content: center;
  align-content: center;
  border-radius: 10px 0 0 10px;
  padding: 0.5rem;
  font-size: 16px;
  color: white;
  padding-left: 1rem;

  .notification{
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
  }

  &:hover{
    cursor: pointer;
    background-color: rgba(100, 100, 100, 0.2);
  }
}
</style>