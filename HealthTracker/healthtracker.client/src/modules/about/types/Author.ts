interface IUrl {
  name: string;
  url: string;
}

interface IAuthor {
  firstName: string;
  lastName: string;
  role: string;
  image: string;
  urls: IUrl[];
}

export type { IAuthor, IUrl };
