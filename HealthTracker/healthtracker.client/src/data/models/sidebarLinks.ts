import { reactive, computed } from "vue";
import { useUserStore } from "@/modules/auth/store/userStore";

const createLink = (name: string, description: string, icon: string, isAuthRequired: boolean, isAdminAccessRequired: boolean) => {
  return computed(() => {
    const userStore = useUserStore();
    return {
      name: name,
      description: description,
      icon: icon,
      isHidden: (isAuthRequired ? !userStore.token : userStore.token && !(name === "Home" || name === "About")) ||
       (isAdminAccessRequired ? !userStore.checkIfAdmin() : false)
    };
  });
};

const getLinks = () => {
  return reactive({
    home: createLink("Home", "Home", "home", false, false),
    meals: createLink("Diary", "Meals", "restaurant", true, false),
    trainingPlaner: createLink("Planner", "Trainings Planner", "fitness_center", true, false),
    health: createLink("Health", "Health Check", "health_and_safety", true, false),
    goals: createLink("Goals", "Goals and Progress", "emoji_events", true, false),
    profile: createLink("Profile", "Profile", "person", true, false),
    community: createLink("Community", "Community", "groups", true, false),
    about: createLink("About", "About", "info", false, false),
    dashboard: createLink("Dashboard", "Dashboard", "dashboard", true, true)
  });
};

const getAuthLinks = () => {
  return reactive({
    register: createLink("Register", "Register", "person_add", false, false),
    login: createLink("Login", "Login", "login", false, false),
    logout: createLink("Logout", "Logout", "logout", true, false)
  });
};

export { getLinks, getAuthLinks };
