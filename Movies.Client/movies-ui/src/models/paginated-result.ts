export class PaginatedResult<T> {
  results: T[] = [];
  itemsCount: number = 0;
  pageNumber: number = 0;
  pageSize: number = 0;
  totalPages: number = 0;
}