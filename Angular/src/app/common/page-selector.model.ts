export interface PageSelectorModel {
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortDirection?: string;
  searchTerm?: string;
}
