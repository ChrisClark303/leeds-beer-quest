import { EstablishmentRatings } from "./establishment-ratings";

export class BeerEstablishment {
    name: string;
    category: string;
    url: URL;
    excerpt: string;
    thumbnail: URL;
    location: Location;
    address : string;
    phone: string;
    twitter: string;
    ratings: EstablishmentRatings;
    tags: string;
}
