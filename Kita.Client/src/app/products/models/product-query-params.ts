import { SortDirection } from '@angular/material/sort';

export interface ProductQueryParams {
  pageIndex: number;
  pageSize: number;
  filterColumn?: string;
  filterQuery?: string;
  filter?: ProductFilter;
  sort?: string;
  direction?: SortDirection;
}

export interface ProductFilter{
  categoryId?: number;
  isActive?: boolean;
  unitId?: number;
}
