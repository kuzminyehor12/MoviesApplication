import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../models/paginated-result';
import { Movie } from '../models/movie';
import { Suggestion } from '../models/suggestion';
import { environment } from '../environment';
import { MovieExtended } from '../models/movie-extended';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private baseUrl = environment.apiUrl; 

  constructor(private http: HttpClient) {}

  browseMovies(
    searchType: string,
    query: string,
    pageNumber: number = 1
  ): Observable<PaginatedResult<Movie>> {
    let params = new HttpParams()
      .set('query', query)
      .set('pageNumber', pageNumber.toString());

    return this.http.get<PaginatedResult<Movie>>(`${this.baseUrl}search/${searchType}`, { params });
  }

  getSuggestions(input: string): Observable<Suggestion[]> {
    return this.http.get<Suggestion[]>(`${this.baseUrl}suggestions`, {
      params: new HttpParams().set('query', input)
    });
  }

  getRecommendations(movieId: number): Observable<Movie[]> {
    return this.http.get<Movie[]>(`${this.baseUrl}recommendations`, {
      params: new HttpParams().set('movieId', movieId)
    });
  }

   getMovieExtended(movieId: number): Observable<MovieExtended> {
    return this.http.get<MovieExtended>(`${this.baseUrl}movies`, {
      params: new HttpParams().set('movieId', movieId)
    });
  }
}
