import { reactive, computed } from "vue";
import { useUserStore } from "@/modules/auth/store/auth";

const createLink = (name: string, description: string, icon: string, isAuthRequired: boolean) => {
  return computed(() => {
    const userStore = useUserStore();
    return {
      name: name,
      description: description,
      icon: icon,
      isHidden: isAuthRequired ? !userStore.token : userStore.token && !(name === "Home" || name === "About")
    };
  });
};

const getLinks = () => {
  return reactive({
    home: createLink("Home", "Home", "home", false),
    meals: createLink("Diary", "Meals", "restaurant", true),
    trainingPlaner: createLink("Planner", "Trainings Planner", "fitness_center", true),
    health: createLink("Health", "Health Check", "health_and_safety", true),
    goals: createLink("Goals", "Goals and Progress", "emoji_events", true),
    profile: createLink("Profile", "Profile", "person", true),
    community: createLink("Community", "Community", "groups", true),
    about: createLink("About", "About", "info", false)
  });
};

const getAuthLinks = () => {
  return reactive({
    register: createLink("Register", "Register", "person_add", false),
    login: createLink("Login", "Login", "login", false),
    logout: createLink("Logout", "Logout", "logout", true)
  });
};

export { getLinks, getAuthLinks };
