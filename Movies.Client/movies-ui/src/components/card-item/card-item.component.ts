import { Component, Input } from '@angular/core';

@Component({
  selector: 'movies-card-item',
  standalone: true,
  imports: [],
  templateUrl: './card-item.component.html',
  styleUrl: './card-item.component.scss',
})
export class CardItemComponent {
    @Input() movie!: Movie;

    getReleaseYear(): number | NotAssigned {
      return this.movie.releaseDate?.getFullYear() ?? "N/A";
    }

    getGenresString(): string {
      return this.movie.genres.map(g => g.name).join(', ');
    }
}
