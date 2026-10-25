export interface CategoryQueryParams{
  pageIndex: number;
  pageSize: number;
  search?: string;
  sort?: 'name' | 'id';
  direction?: 'asc' | 'desc';
}
