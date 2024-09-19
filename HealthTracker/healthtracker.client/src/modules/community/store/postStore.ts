import { defineStore } from "pinia";
import type { IPost } from "../types/Post";

export const usePostStore = defineStore('postData', {
    state: () => ({
        posts: [] as IPost[],
    }),
    actions: {
        setPosts(poststoSet: IPost[]){
            this.posts = poststoSet;
        },
        addPosts(postsToAdd: IPost[]){
            this.posts = [...this.posts, ...postsToAdd];
        },
        removePosts(){
            this.posts = [];
        }
    }
});
