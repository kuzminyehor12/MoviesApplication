import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { CardItemComponent } from '../card-item/card-item.component';

@Component({
  selector: 'movies-search',
  standalone: true,
  imports: [CommonModule, CardItemComponent],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent {
    getMovies(): Movie[] {
      return [
        {
          id: 1,
          title: "The Shawshank Redemption",
          voteAverage: 9.3,
          releaseDate: new Date(1994),
          genres: [
            {
              id: 1,
              name: "Drama"
            }
          ]
        },
        {
          id: 2,
          title: "The Dark Knight",
          voteAverage: 9.0,
          releaseDate: new Date(2008),
          genres: [
            {
              id: 2,
              name: "Action"
            },
            {
              id: 3,
              name: "Crime"
            }
          ]
        },
        {
          id: 3,
          title: "Inception",
          voteAverage: 8.8,
          releaseDate: new Date(2010),
          genres: [
            {
              id: 2,
              name: "Sci-Fi"
            },
            {
              id: 3,
              name: "Thriller"
            }
          ]
        },
        {
          id: 4,
          title: "Inception",
          voteAverage: 8.8,
          releaseDate: new Date(2010),
          genres: [
            {
              id: 2,
              name: "Sci-Fi"
            },
            {
              id: 3,
              name: "Thriller"
            }
          ]
        },
        {
          id: 5,
          title: "Inception",
          voteAverage: 8.8,
          releaseDate: new Date(2010),
          genres: [
            {
              id: 2,
              name: "Sci-Fi"
            },
            {
              id: 3,
              name: "Thriller"
            }
          ]
        },
        {
          id: 6,
          title: "Inception",
          voteAverage: 8.8,
          releaseDate: new Date(2010),
          genres: [
            {
              id: 2,
              name: "Sci-Fi"
            },
            {
              id: 3,
              name: "Thriller"
            }
          ]
        },
        {
          id: 7,
          title: "Inception",
          voteAverage: 8.8,
          releaseDate: new Date(2010),
          genres: [
            {
              id: 2,
              name: "Sci-Fi"
            },
            {
              id: 3,
              name: "Thriller"
            }
          ]
        },
        {
          id: 8,
          title: "Inception",
          voteAverage: 8.8,
          releaseDate: new Date(2010),
          genres: [
            {
              id: 2,
              name: "Sci-Fi"
            },
            {
              id: 3,
              name: "Thriller"
            }
          ]
        }
      ]
    }
}
