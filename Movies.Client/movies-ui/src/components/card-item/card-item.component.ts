import { Component, Input, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Movie } from '../../models/movie';
import { environment } from '../../environment';
import { Router } from '@angular/router';

@Component({
  selector: 'movies-card-item', 
  standalone: true,
  imports: [CommonModule, DecimalPipe],
  templateUrl: './card-item.component.html',
  styleUrl: './card-item.component.scss'
})
export class CardItemComponent implements OnInit {
  @Input({ required: true }) movie!: Movie;

  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router); 
  
  private readonly tmdbBaseUrl = 'https://image.tmdb.org/t/p/'; 
  
  private readonly posterSizes = ['w500', 'w342', 'w185']; 
  
  moviePosterUrl: string = '';
  private currentSizeIndex: number = 0;
  private omdbAttempted: boolean = false;

  ngOnInit(): void {
    this.attemptOmdbFallback();
  }
  
  private buildImageUrl(size: string, path: string): string {
    return `${this.tmdbBaseUrl}${size}${path}`;
  }

  navigateToDetail(): void {
    this.router.navigate(['/movie', this.movie.id]); 
  }

  handleImageError(): void {
    if (!this.omdbAttempted && this.movie.imdbId) {
      this.attemptOmdbFallback();
      return;
    }

    if (this.currentSizeIndex < this.posterSizes.length - 1) {
      this.currentSizeIndex++;
      const nextSize = this.posterSizes[this.currentSizeIndex];
      this.moviePosterUrl = this.buildImageUrl(nextSize, this.movie.posterPath!);
      console.warn(`[${this.movie.title}] Poster failed. Retrying with TMDB size: ${nextSize}`);
      return;
    }
    
    this.moviePosterUrl = ''; 
    console.error(`[${this.movie.title}] Poster failed after exhausting all options.`);
    this.cdr.detectChanges();
  }
  
  private attemptOmdbFallback(): void {
    this.omdbAttempted = true;
    
    if (!this.movie.imdbId) {
      this.moviePosterUrl = ''; 
      this.cdr.detectChanges();
      return;
    }

    const omdbUrl = `https://www.omdbapi.com/?i=${this.movie.imdbId}&apikey=${environment.omdbApiKey}`;

    this.http.get<{ Poster: string }>(omdbUrl).subscribe({
      next: (response) => {
        if (response.Poster && response.Poster !== 'N/A') {
          this.moviePosterUrl = response.Poster; 
          console.log(`[${this.movie.title}] Success using OMDb fallback.`);
        } else {
          this.moviePosterUrl = ''; 
        }
        
        this.cdr.detectChanges(); 
      },
      error: (err) => {
        console.error(`[${this.movie.title}] OMDb request failed.`, err);
        this.moviePosterUrl = '';
        this.cdr.detectChanges(); 
      }
    });
  }

  getReleaseYear(): number | NotAssigned {
    if (this.movie.releaseDate) {
      const date = new Date(this.movie.releaseDate);
      if (!isNaN(date.getFullYear())) {
        return date.getFullYear();
      }
    }
    return 'N/A';
  }

  getGenresString(): string {
    if (this.movie.genres && this.movie.genres.length > 0) {
        return this.movie.genres.map((g: any) => g.name).join(', ');
    }
    return 'Unknown Genre';
  }
}
