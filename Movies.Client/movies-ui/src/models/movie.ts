class Movie extends Entity {
    title!: string;
    voteAverage?: number;
    releaseDate?: Date;
    posterPath?: string;
    imdbId?: string;
    genres: Genre[] = []
}