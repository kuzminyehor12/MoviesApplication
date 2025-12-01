import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../models/paginated-result';
import { Movie } from '../models/movie';
import { Suggestion } from '../models/suggestion';
import { environment } from '../environment';

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
      .set('page', pageNumber.toString());

    return this.http.get<PaginatedResult<Movie>>(`${this.baseUrl}search/${searchType}`, { params });
  }

  getSuggestions(input: string): Observable<Suggestion[]> {
    return this.http.get<Suggestion[]>(`${this.baseUrl}suggestions`, {
      params: new HttpParams().set('query', input)
    });
  }
}
