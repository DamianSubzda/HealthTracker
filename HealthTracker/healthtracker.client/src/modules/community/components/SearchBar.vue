<template>
    <div class="searchbar">
        <div class="search">
            <input placeholder="Search..." class="search-input" v-model="searchQuery"
                :class="{ 'active-results': searchResults.length }">
            <div class="search-results" v-if="searchResults.length">
                <ul>
                    <li v-for="user in searchResults" :key="user.id">
                        <router-link :to="{ name: 'UsersProfile', params: { id: user.id } }" @click="searchQuery = ''">
                            {{ user.firstName }} {{ user.lastName }}
                        </router-link>
                    </li>
                </ul>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { debounce } from 'lodash-es';
import { ref, watch } from 'vue';
import { getSearchedUsers } from '@/api/community/userController'

const searchQuery = ref('');
const searchResults = ref<SearchedUser[]>([]);

type SearchedUser = {
    id: number,
    firstName: string,
    lastName: string,
    userName: string,
    profilePicture: string | null
}

const debouncedSearch = debounce(async (query: string) => {
    if (query.length > 1) {
        await searchUsers(query);
    } else {
        searchResults.value = [];
    }
}, 400);

watch(searchQuery, (newQuery) => {
    debouncedSearch(newQuery);
});

async function searchUsers(query: string) {
    searchResults.value = await getSearchedUsers(query);
}
</script>

<style scoped lang="scss">
.searchbar {
    width: 100%;

    .search {
        display: flex;
        flex-direction: column;
        padding: 0.5rem;
        align-items: center;
        width: 100%;

        .search-input {
            height: 100%;
            width: 95%;
            background-color: rgb(187, 147, 147);
            box-sizing: border-box;
            border: 2px solid black;
            border-top-left-radius: 8px;
            border-top-right-radius: 8px;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
            font-size: 16px;
            background-image: url('/src/assets/search.svg');
            background-position: center left 10px;
            background-repeat: no-repeat;
            text-align: center;
            padding: 12px 20px 12px 40px;

            &:focus {
                outline: none;
                border-color: black;
            }
        }

        .active-results {
            border-bottom-left-radius: 0px;
            border-bottom-right-radius: 0px;
            border-bottom: 0;
            outline-width: 0;
        }

        .search-results {
            padding-top: .2rem;
            padding-bottom: 0.5rem;
            width: 95%;
            color: black;
            background-color: rgb(187, 147, 147);
            border-left: 2px solid black;
            border-right: 2px solid black;
            border-bottom: 2px solid black;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
            outline-width: 2px;
            outline-color: white;
            display: flex;
            justify-content: center;
            font-size: large;

            ul {
                color: black;
                padding-top: .5rem;
                width: 90%;
                border-top: 1px solid rgba(0, 0, 0, 0.5);
                list-style-type: disclosure-closed;

                a {
                    color: black;
                }

                a &:focus,
                :hover {
                    background-color: transparent;
                    color: gold;
                }
            }
        }
    }
}
</style>
