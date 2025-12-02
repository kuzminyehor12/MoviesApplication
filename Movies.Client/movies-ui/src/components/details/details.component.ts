import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { forkJoin, tap } from 'rxjs';
import { Movie } from '../../models/movie';
import { ApiService } from '../../services/api.service';
import { MovieExtended } from '../../models/movie-extended';
import { HttpClient } from '@angular/common/http';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { environment } from '../../environment';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'movies-details',
  standalone: true,
  imports: [CommonModule, DatePipe, DecimalPipe],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss',
})
export class DetailsComponent implements OnInit {
  private apiService = inject(ApiService);
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);
  
  movieDetail = signal<MovieExtended | null>(null);
  recommendations = signal<any[]>([]); 
  loading = signal(true);
  posterUrl = signal<string | null>(null);

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
        const idString = params.get('id');
        const movieId = idString ? +idString : null; 

        if (movieId) {
            this.fetchData(movieId);
        } else {
            console.error("Movie ID is missing from the route.");
            this.loading.set(false);
        }
    });
  }

  fetchData(id: number): void {
    this.loading.set(true);
    this.movieDetail.set(null);
    this.posterUrl.set(null);
    this.recommendations.set([]);

    forkJoin({
      details: this.apiService.getMovieExtended(id),
      recs: this.apiService.getRecommendations(id)
    }).pipe(
      tap(() => this.loading.set(false))
    ).subscribe({
      next: ({ details, recs }) => {
        this.movieDetail.set(details);
        const mappedRecs = (recs as Movie[] || []).map(rec => ({
            ...rec,
            posterFromOmdb: signal<string | null>(null)
        }));

        this.recommendations.set(mappedRecs);
        this.loadRecommendationPosters(mappedRecs);
        this.loadPoster(details.imdbId, details.posterPath);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load movie data:', err);
        this.cdr.detectChanges();
        this.loading.set(false);
      }
    });
  }

  private loadPoster(imdbId?: string | null, posterPath?: string | null): void {
    if (imdbId) {
        this.attemptOmdbFallback(imdbId);
    } else if (posterPath) {
        this.posterUrl.set(this.buildTmdbUrl('w780', posterPath));
    } else {
        this.posterUrl.set(null);
    }

    this.cdr.detectChanges();
  }

  private buildTmdbUrl(size: string, path: string): string {
    const tmdbBaseUrl = 'https://image.tmdb.org/t/p/';
    return `${tmdbBaseUrl}${size}${path}`;
  }

  handleImageError(): void {
    const detail = this.movieDetail();
    if (detail && detail.imdbId && this.posterUrl() !== detail.imdbId) {
      this.attemptOmdbFallback(detail.imdbId);
    } else {
      this.posterUrl.set(null);
      this.cdr.detectChanges();
    }
  }

  private attemptOmdbFallback(imdbId: string): void {
    const omdbUrl = `https://www.omdbapi.com/?i=${imdbId}&apikey=${environment.omdbApiKey}`;
    
    this.http.get<{ Poster: string }>(omdbUrl).subscribe({
      next: (response) => {
        if (response.Poster && response.Poster !== 'N/A') {
          this.posterUrl.set(response.Poster); 
        } else {
          this.posterUrl.set(null);
          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error('OMDb request failed:', err);
        this.posterUrl.set(null);
        this.cdr.detectChanges();
      }
    });
  }

  getReleaseYear(): number | NotAssigned {
    const dateStr = this.movieDetail()?.releaseDate;
    if (dateStr) {
      const date = new Date(dateStr);
      if (!isNaN(date.getFullYear())) {
        return date.getFullYear();
      }
    }
    return 'N/A';
  }

  getRuntime(): NotAssigned {
    return 'N/A'; 
  }

  loadRecommendationPosters(recs: any[]): void {
      recs.forEach(rec => {
          if (rec.imdbId) {
              const omdbUrl = `https://www.omdbapi.com/?i=${rec.imdbId}&apikey=${environment.omdbApiKey}`;
              
              this.http.get<{ Poster: string }>(omdbUrl).subscribe({
                  next: (response) => {
                      if (response.Poster && response.Poster !== 'N/A') {
                          rec.posterFromOmdb.set(response.Poster); 
                      }
                      this.cdr.detectChanges(); 
                  },
                  error: (err) => {
                      console.error(`OMDb request failed for ID ${rec.imdbId}:`, err);
                      this.cdr.detectChanges();
                  }
              });
          }
      });
  }


  getRecPosterUrl(rec: any): string {
    if (rec.posterFromOmdb()) {
        return rec.posterFromOmdb();
    }

    if (rec.posterPath) {
      return this.buildTmdbUrl('w185', rec.posterPath);
    } 

    return 'https://placehold.co/150x225/A0A0A0/FFFFFF?text=No+Poster';
  }

  navigateToDetail(movieId: number): void {
    this.router.navigate(['/movie', movieId]).then(() => {
        this.fetchData(movieId); 
    }); 
  }
}