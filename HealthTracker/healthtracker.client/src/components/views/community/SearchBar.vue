<template>
    <div class="searchbar">
        <div class="search">
            <input placeholder="Szukaj..." 
                   class="search-input" 
                   v-model="searchQuery"
                   :class="{'active-results': searchResults.length}">
            <div class="search-results" v-if="searchResults.length">
                <ul>
                    <li v-for="user in searchResults" :key="user.id">{{ user.firstName }} {{ user.lastName }}</li>
                </ul>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { debounce } from 'lodash-es';
import apiClient from '@/service/api/axios';
import { ref, watch } from 'vue';
import { useUserStore } from '@/store/account/auth';

const userStore = useUserStore();
const searchQuery = ref('');
const searchResults = ref([]);

const debouncedSearch = debounce(async (query: string) => {
    if (query.length > 1) {
        await searchUsers(query);
    } else {
        searchResults.value = [];
    }
}, 400); // Opóźnienie

watch(searchQuery, (newQuery) => {
    debouncedSearch(newQuery);
});

async function searchUsers(query: string) {
    try {
        const response = await apiClient.get(`/api/users/${userStore.userId}/search?query=${encodeURIComponent(query)}`, {
            headers: {
                'Authorization': `Bearer ${userStore.token}`
            }
        });
        searchResults.value = await response.data;
        console.log(searchResults.value);
    } catch (error) {
        console.error('Błąd podczas wyszukiwania użytkowników:', error);
        searchResults.value = [];
    }
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
        height: 100%;
        width: 100%;
        .search-input {
            height: 100%;
            width: 95%;
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
        }
        .active-results {
            border-bottom-left-radius: 0px;
            border-bottom-right-radius: 0px;
            border-bottom: 0;
            outline-width: 0;
        }
        .search-results {
            width: 95%;
            color: black;
            background-color: rgb(175, 159, 159);
            border-left: 2px solid black;
            border-right: 2px solid black;
            border-bottom: 2px solid black;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
            outline-width: 2px;
            outline-color: white;
        }
    }
}
</style>
