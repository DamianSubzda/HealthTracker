import type { IAuthor, IUrl } from "../types/Author";

const authors: IAuthor[] = [
  {
    firstName: "Damian",
    lastName: "Subzda",
    role: "Developer",
    image: "programista1.png",
    urls: [
      {
        name: "Github",
        url: "https://github.com/DamianSubzda",
      },
      {
        name: "Example",
        url: "https://wp.pl",
      },
    ] as IUrl[],
  },
  {
    firstName: "Filip",
    lastName: "Stańczak",
    role: "Developer",
    image: "programista2.png",
    urls: [
      {
        name: "Github",
        url: "https://github.com/Hikareel",
      },
    ] as IUrl[],
  },
];

export { authors };
