<template>
    <div class="form" :class="{ 'is-logging': isLogging }">
        <FormStatus formTitle="Login" />
        <Vueform class="form-content" v-model="formData" @submit="login" :float-placeholders="false" :endpoint="false"
            :display-errors="false" sync>
            <GroupElement name="email_username">
                <TextElement name="EmailUserName" label="Email or username" placeholder="user@example.com"
                    rules="required" :addons="{
                        before: `<i class='bi bi-pencil-fill'></i>`
                    }" />
            </GroupElement>
            <GroupElement name="password">
                <TextElement name="Password" label="Password" placeholder="Password" input-type="password"
                    rules="required|min:6|regex:/^(?=.*[^\w\d])(?=.*\d)(?=.*[A-Z]).+$/" :addons="{
                        before: `<i class='bi bi-lock-fill'></i>`
                    }" />
            </GroupElement>
            <GroupElement name="controll">
                <ButtonElement id="reset_button" name="reset" type="reset" :resets="true" hidden />
                <ButtonElement class="login-button" name="submit" button-label="Login" align="center" :submits="true"
                    full size="lg" :columns="{ container: 12, label: 0, wrapper: 12 }" />
                <LoginWithGoogle v-model="isLogging" />
            </GroupElement>
            <GroupElement name="control2">
                <ButtonElement name="forgot" button-type="anchor" @click="goToResetPassword"
                    button-label="Forgot password?" :columns="{ default: 6 }" full size="sm" secondary />
                <ButtonElement name="register" @click="goToRegister" button-label="Register" :columns="{ default: 6 }"
                    full size="sm" secondary button-type="anchor" />
            </GroupElement>
        </Vueform>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { ILoginModel } from '@/modules/auth/types/FormModels';
import LoginWithGoogle from './LoginWithGoogle.vue'
import { apiPostLogin } from '@/api/account/authController'
import FormStatus from "@/shared/components/FormStatus.vue"
import { useUserStore } from "@/shared/store/userStore"
import { useNavigationStore } from '@/shared/store/navigationStore'
import router from "@/router/index";

const isLogging = ref(false);

const formData = ref<ILoginModel>({
    EmailUserName: '',
    Password: '',
});

const login = async () => {
    isLogging.value = true;
    const result = await apiPostLogin(formData.value);

    if (result) {
        localStorage.setItem("user", JSON.stringify(result.data));
        const userStore = useUserStore();
        const navigationStore = useNavigationStore();
        router.push("/").then(() => {
            userStore.updateUserData();
            navigationStore.updateLinkVisibility();
        });
    }

    isLogging.value = false;
    formData.value.Password = '';
}

const goToResetPassword = () => {
    router.push({ name: 'Reset Password' });
};

const goToRegister = () => {
    router.push({ name: 'Register' });
};

</script>

<style lang="scss" scoped>
.login-button {
    margin-top: 1rem;
}

.form.is-logging {
    opacity: 0.5;
    pointer-events: none;
}
</style>