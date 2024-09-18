<template>
    <main>
        <div className="create-post-panel">
            <div v-if="isPostCreated" className="panel-create-success">
                <p>Post created successfully!</p>
            </div>
            <div className="panel-editing">
                <textarea :value="textInput" @input="update" placeholder="Write markdown content..."></textarea>
                <div className="panel-editing-buttons">
                    <button @click="openFileDialog">
                        <img src="@/assets/icons/add.svg" alt="Insert image" />
                    </button>
                    <button @click="removeImage">
                        <img src="@/assets/icons/close-o.svg" alt="Remove image" />
                    </button>
                    <button @click="createPostClick">
                        <img src="@/assets/icons/arrow-long-right.svg" alt="Create Post" />
                    </button>
                    <input ref="fileInput" type="file" @change="handleFileSelect" accept="image/*" style="display: none;" />
                </div>
            </div>

            <div className="panel-preview">
                <p>Post preview:</p>

                <div class="content">
                    <div class="header">
                        <p>{{ userStore.firstName }} {{ userStore.lastName }}</p>
                    </div>
                    <div class="main">
                        <div v-html="compiledMarkdown" className="text-preview"></div>
                        <div class="attachment" v-if="imageUrl">
                            <img :src="imageUrl" alt="Attached image" style="border-radius: 0.5rem;" />
                        </div>
                    </div>
                    <div class="footer">
                        <button class="like">
                            <i class="bi bi-hand-thumbs-up-fill"></i>&nbsp;{{ 0 }}
                        </button>
                        <button class="comment">
                            <i class='bi bi-chat-dots-fill'></i>&nbsp;{{ 0 }}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </main>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue';
import { marked } from 'marked';
import _ from 'lodash';
import { useUserStore } from '../../auth/store/userStore';
import { apiPostPost } from './../../../api/community/postController'

const userStore = useUserStore();
const textInput = ref<string>("");
const isPostCreated = ref(false);
const fileInput = ref<HTMLInputElement | null>(null);
const imageUrl = ref<string | null>(null);

const compiledMarkdown = computed(() => {
    return marked(textInput.value);
});

const update = _.debounce((e: Event) => {
    textInput.value = (e.target as HTMLTextAreaElement).value;
    isPostCreated.value = false;
}, 300);

async function createPostClick() {
    const result = await apiPostPost(userStore.userId, textInput.value, fileInput.value);
    if (result) {
        textInput.value = "";
        removeImage();
        isPostCreated.value = true;
    }
}

function openFileDialog() {
    fileInput.value?.click();
}

function removeImage() {
    imageUrl.value = null;
    if (fileInput.value) {
        fileInput.value.value = '';
    }
}

function handleFileSelect(event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files ? target.files[0] : null;
    
    if (file) {
        const reader = new FileReader();
        reader.onload = function(e) {
            imageUrl.value = e.target?.result as string;
        };
        reader.readAsDataURL(file);
    }
}
</script>

<style lang="scss" scoped>
.create-post-panel {
    display: flex;
    flex-direction: column;
    width: 100%;
    justify-content: center;
    align-items: center;

    .panel-create-success{
        color:chartreuse;
        margin-top: 1rem;
    }

    .panel-editing {

        display: flex;
        flex-direction: column;
        width: 70%;
        margin: 2rem;

        textarea {
            min-height: 20vh;
            border: none;
            border: 1px solid #ccc;
            resize: none;
            outline: none;
            background-color: #7e7676;
            font-size: 14px;
            font-family: "Monaco", courier, monospace;
            padding: 20px;
            width: 100%;
            border-top-left-radius: 1rem;
            border-top-right-radius: 1rem;
        }
        .panel-editing-buttons{
            display: grid;
            grid-template-columns: 3rem 3rem 1fr;
            justify-content: center;
            background-color: #7e7676;
            padding: 0.5rem;
            border-bottom-left-radius: 1rem;
            border-bottom-right-radius: 1rem;

            button {
                height: 3rem;
                background-color: rgb(112, 112, 112);
                border-radius: 50rem;
                border: 0;
                cursor: pointer;
                
                img {
                    transition: transform 0.3s ease;
                }

                &:hover{
                    background-color: rgb(95, 95, 95);
                    z-index: 9999;
                    img{
                        transform: scale(1.2, 1.2);
                        transition: transform 0.3s ease;
                    }
                    
                }
            }
        }
    }

    .panel-preview {
        width: 70%;

        .content {
            margin-top: 0.5rem;
            margin-bottom: 0.5rem;
            width: 95%;
            border: 1px solid black;
            border-radius: 1rem;
            background-color: rgba(62, 50, 50, 1);
            width: 100%;
            display: flex;
            flex-direction: column;

            .header {
                border-radius: 1rem 1rem 0 0;
                padding: 0.5rem;
            }

            .main {
                border-top: 2px solid rgb(73, 61, 61);
                padding: 1rem;

                .text-preview {
                    word-wrap: break-word;
                    white-space: normal;
                }
                
                .attachment{
                    padding-top: 0.5rem;

                    img {
                        width: 100%;
                    }
                }
            }

            .footer {
                border-radius: 0 0 1rem 1rem;
                text-align: center;
                display: flex;
                height: 3rem;
                padding: 0.4rem;
                gap: 0.2rem;

                button {
                    background-color: rgb(73, 61, 61);
                    color: white;
                    width: 100%;
                    border: none;
                    border-radius: 1rem;

                    &:hover {
                        background-color: rgb(112, 112, 112);
                    }
                }
            }

            .footer button i {
                display: inline-block;
                transition: transform 0.3s ease;
            }

            .footer button:active i {
                transform: scale(1.1);
            }

        }
    }
}
</style>
