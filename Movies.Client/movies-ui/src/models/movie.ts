import { Genre } from "./genre";

export class Movie extends Entity {
    title!: string;
    voteAverage?: number;
    releaseDate?: string;
    posterPath?: string;
    imdbId?: string;
    genres: Genre[] = []
}