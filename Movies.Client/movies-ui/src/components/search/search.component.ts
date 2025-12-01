import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { CardItemComponent } from '../card-item/card-item.component';
import { Movie } from '../../models/movie';
import { SearchType } from '../../types/search-type';
import { debounceTime, distinctUntilChanged, finalize, Observable, Subject, switchMap } from 'rxjs';
import { ApiService } from '../../services/api.service';
import { Suggestion } from '../../models/suggestion';
import { PaginatedResult } from '../../models/paginated-result';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'movies-search',
  standalone: true,
  imports: [CommonModule, CardItemComponent, FormsModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent implements OnInit {
    loading = signal(false);
    movies = signal<Movie[]>([]);
    totalPages = signal(1);
    currentPage = signal(1);

    searchQuery = '';
    selectedSearchType = SearchType.Fuzzy;
    suggestions: Suggestion[] = [];

    private searchInput$ = new Subject<string>();
    
    // Available Search Types
    searchTypes = [
      { name: 'Fuzzy Search', value: SearchType.Fuzzy },
      { name: 'Lexical Search', value: SearchType.Lexical },
      { name: 'Semantic Search', value: SearchType.Semantic },
    ];

    constructor(private apiService: ApiService) {}

    ngOnInit(): void {
      this.loadMovies(this.currentPage());
      
      this.searchInput$.pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(query => {
          if (this.selectedSearchType === SearchType.Fuzzy && query.length > 2) {
            return this.apiService.getSuggestions(query);
          }
          return [];
        })
      ).subscribe(suggestions => {
        this.suggestions = suggestions;
      });
    }

    onSearchInputChange(query: string): void {
      if (!query) {
        this.suggestions = [];
      }
      this.searchInput$.next(query);
    }

    loadMovies(page: number): void {
      let request$: Observable<PaginatedResult<Movie>>;

      this.loading.set(true);
      
      request$ = this.apiService.browseMovies(
        this.selectedSearchType,
        this.searchQuery,
        page
      );

      request$.pipe(
        finalize(() => {
          this.loading.set(false);
        })
      ).subscribe({
        next: (data) => {
          this.movies.set(data.results || []);
          this.currentPage.set(data.pageNumber);
          this.totalPages.set(data.totalPages);
          this.suggestions = [];
        },
        error: (err) => console.error('Failed to fetch movies:', err)
      });
    }
    
    onSearchSubmit(): void {
      this.loadMovies(1);
    }

    onPageChange(page: number): void {
      if (page >= 1 && page <= this.totalPages()) {
        this.loadMovies(page);
      }
    }
    
    selectSuggestion(suggestion: Suggestion): void {
      this.searchQuery = suggestion.title;
      this.suggestions = [];
      this.onSearchSubmit();
    }

    handlePageButtonClick(page: number | string): void {
      if (typeof page === 'number') {
        this.onPageChange(page);
      }
    }

    getPagesArray(): (number | string)[] {
      const current = this.currentPage();
      const total = this.totalPages();
      const maxVisiblePages = 5;
      const pages: (number | string)[] = [];

      if (total <= maxVisiblePages) {
        for (let i = 1; i <= total; i++) {
          pages.push(i);
        }
      } else {
        const middleRangeSize = maxVisiblePages - 4;
        const buffer = Math.floor(middleRangeSize / 2);

        let start = Math.max(2, current - buffer);
        let end = Math.min(total - 1, current + buffer);
        
        if (current < 1 + buffer + 2) {
            end = 1 + middleRangeSize + 1;
            start = 2;
        } else if (current > total - buffer - 2) {
            start = total - middleRangeSize - 1;
            end = total - 1;
        }

        pages.push(1);

        if (start > 2) {
          pages.push('...');
        }

        for (let i = start; i <= end; i++) {
          pages.push(i);
        }

        if (end < total - 1) {
          pages.push('...');
        }

        if (total > 1 && pages[pages.length - 1] !== total) {
            pages.push(total);
        }
      }
      
      return pages.filter((value, index, self) => 
          self.findIndex(t => t === value) === index
      );
    }
}
