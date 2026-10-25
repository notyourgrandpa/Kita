import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../../shared/services/base.service';
import { Observable, catchError, filter, of, switchMap, tap } from 'rxjs';
import { Product } from '../models/product';
import { Unit } from '../models/unit';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ProductQueryParams } from '../models/product-query-params';

@Injectable({
  providedIn: 'root',
})

export class ProductService
  extends BaseService<Product> {
  constructor(
    http: HttpClient,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    super(http);
  }
  getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null
  ): Observable<ApiResult<Product>> {
    var url = this.getUrl("api/products");
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);
    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }
    return this.http.get<ApiResult<Product>>(url, { params });
  }

  getUnits(): Observable<Unit[]>{
    const url = this.getUrl("api/units");

    return this.http.get<Unit[]>(url);
  }

  getProducts(params: ProductQueryParams): Observable<ApiResult<Product>> {
    const url = this.getUrl('api/products');

    let httpParams = new HttpParams()
      .set('pageIndex', params.pageIndex)
      .set('pageSize', params.pageSize);

    if (params.filterQuery) {
      httpParams = httpParams.set('filterQuery', params.filterQuery);
    }

    if (params.filter?.categoryId != null) {
      httpParams = httpParams.set('categoryId', params.filter.categoryId);
    }

    if (params.filter?.unitId != null) {
      httpParams = httpParams.set('unitId', params.filter.unitId);
    }

    if (params.filter?.isActive != null) {
      httpParams = httpParams.set('isActive', params.filter.isActive);
    }

    if (params.sort) {
      httpParams = httpParams
        .set('sortColumn', params.sort)
        .set('sortOrder', params.direction ?? 'asc');
    }

    return this.http.get<ApiResult<Product>>(url, { params: httpParams });
  }

  get(id: number): Observable<Product> {
    var url = this.getUrl("api/products/" + id);
    return this.http.get<Product>(url);
  }
  put(item: Product): Observable<Product> {
    var url = this.getUrl("api/products/" + item.id);
    return this.http.put<Product>(url, item);
  }
  post(item: Product): Observable<Product> {
    var url = this.getUrl("api/products");
    return this.http.post<Product>(url, item);
  }

  delete(id: number): Observable<void> {
    var url = this.getUrl("api/products/" + id);
    return this.http.delete<void>(url);
  }

  isDupeProduct(item: Product): Observable<boolean> {
    var url = this.getUrl("api/Products/IsDupeProduct");
    return this.http.post<boolean>(url, item);
  }

  confirmAndDelete(id: number, redirectTo?: string, reloadCallback?: () => void): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Product',
        message: 'Are you sure you want to delete this product?'
      }
    }).afterClosed().pipe(
      filter(result => result === true),
      switchMap(() => this.delete(id)),
      tap(() => {
        this.snackBar.open('Product deleted successfully.', 'Close', { duration: 3000 });

        if (redirectTo) {
          this.router.navigate([redirectTo]);
        }

        if (reloadCallback) {
          reloadCallback();
        }
      }),
      catchError(err => {
        this.snackBar.open('Failed to delete the category.', 'Close', { duration: 3000 });
        console.error('Delete failed', err);
        return of(null);
      })
    ).subscribe();
  }

  search(term: string): Observable<Product[]> {
    return this.http.get<Product[]>(`/api/products/search`, {
      params: { term }
    });
  }
}
