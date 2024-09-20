import { defineStore } from "pinia";
import { useUserStore } from "@/shared/store/userStore";
import type { ILink } from "../types/Link.ts";

export const useNavigationStore = defineStore("navigation", {
  state: () => ({
    links: [] as ILink[],
    authLinks: [] as ILink[],
  }),

  actions: {
    createLink(
      name: string,
      description: string,
      icon: string,
      isAuthRequired: boolean,
      isAdminAccessRequired: boolean
    ) {
      const userStore = useUserStore();

      const link: ILink = {
        name,
        description,
        icon,
        isHidden:
          (isAuthRequired
            ? !userStore.token
            : userStore.token && !(name === "Home" || name === "About")) ||
          (isAdminAccessRequired ? !userStore.checkIfAdmin() : false),
      };

      return link;
    },
    initializeLinks() {
      this.links = [
        this.createLink("Home", "Home", "home", false, false),
        this.createLink("Diary", "Meals", "restaurant", true, false),
        this.createLink(
          "Planner",
          "Trainings Planner",
          "fitness_center",
          true,
          false
        ),
        this.createLink(
          "Health",
          "Health Check",
          "health_and_safety",
          true,
          false
        ),
        this.createLink(
          "Goals",
          "Goals and Progress",
          "emoji_events",
          true,
          false
        ),
        this.createLink("Profile", "Profile", "person", true, false),
        this.createLink("Community", "Community", "groups", true, false),
        this.createLink("About", "About", "info", false, false),
        this.createLink("Dashboard", "Dashboard", "dashboard", true, true),
      ];

      this.authLinks = [
        this.createLink("Register", "Register", "person_add", false, false),
        this.createLink("Login", "Login", "login", false, false),
        this.createLink("Logout", "Logout", "logout", true, false),
      ];
    },

    updateLinkVisibility() {
      const userStore = useUserStore();

      this.links.forEach((link) => {
        link.isHidden =
          (link.name === "Dashboard" && !userStore.checkIfAdmin()) ||
          (!userStore.token && link.name !== "Home" && link.name !== "About");
      });

      this.authLinks.forEach((authLink) => {
        authLink.isHidden = !userStore.token && authLink.name === "Logout";
      });
    },

    addLink(link: ILink) {
      this.links.push(link);
    },

    addAuthLink(authLink: ILink) {
      this.authLinks.push(authLink);
    },
  },
});
