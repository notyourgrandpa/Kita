import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../../shared/services/base.service';
import { Observable, filter, switchMap, tap, catchError, of } from 'rxjs';
import { Category } from '../models/category';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoryQueryParams } from '../../inventory-logs/models/category-query-params';

@Injectable({
  providedIn: 'root',
})
export class CategoryService
  extends BaseService<Category> {
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
  ): Observable<ApiResult<Category>> {
    var url = this.getUrl("api/Categories");
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortDirection", sortOrder);
    if (filterColumn && filterQuery) {
      params = params
        .set("filters[0].field", filterColumn)
        .set("filters[0].value", filterQuery);
    }
    return this.http.get<ApiResult<Category>>(url, { params });
  }

  getCategories(
    params: CategoryQueryParams
  ): Observable<ApiResult<Category>> {
    const url = this.getUrl('api/categories');

    let httpParams = new HttpParams()
      .set('pageIndex', params.pageIndex)
      .set('pageSize', params.pageSize);

    if (params.search) {
      httpParams = httpParams.set('search', params.search);
    }

    if (params.sort) {
      httpParams = httpParams
        .set('sort', params.sort)
        .set('direction', params.direction ?? 'asc');
    }

    return this.http.get<ApiResult<Category>>(url, { params: httpParams });
  }

  get(id: number): Observable<Category> {
    var url = this.getUrl("api/Categories/" + id);
    return this.http.get<Category>(url);
  }

  put(item: Category): Observable<Category> {
    var url = this.getUrl("api/Categories/" + item.id);
    return this.http.put<Category>(url, item);
  }

  post(item: Category): Observable<Category> {
    var url = this.getUrl("api/Categories");
    return this.http.post<Category>(url, item);
  }

  delete(id: number): Observable<void> {
    var url = this.getUrl("api/Categories/" + id);
    return this.http.delete<void>(url);
  }

  archive(id: number): Observable<void> {
    var url = this.getUrl("api/Categories/Archive/" + id);
    return this.http.post<void>(url, null);
  }

  unarchive(id: number): Observable<void> {
    const url = this.getUrl("api/Categories/Unarchive/" + id);
    return this.http.post<void>(url, null);
  }

  isDupeField(categoryId: number, fieldName: string, fieldValue: string):
    Observable<boolean> {
    var params = new HttpParams()
      .set("categoryId", categoryId)
      .set("fieldName", fieldName)
      .set("fieldValue", fieldValue);
    var url = this.getUrl("api/Categories/IsDupeField");
    return this.http.post<boolean>(url, null, { params });
  }

  confirmAndDelete(id: number): Observable<void | null> {
    return this.confirm(
      () => this.delete(id),
      'Delete Category',
      'Are you sure you want to delete this category?',
      'Category deleted successfully.',
      'Failed to delete the category.'
    );
  }

  confirmAndArchive(id: number): Observable<void | null> {
    return this.confirm(
      () => this.archive(id),
      'Archive Category',
      'Are you sure you want to archive this category?',
      'Category archived successfully.',
      'Failed to archive the category.'
    );
  }

  confirmAndUnarchive(id: number): Observable<void | null> {
    return this.confirm(
      () => this.unarchive(id),
      'Unarchive Category',
      'Are you sure you want to unarchive this category?',
      'Category Unarchived successfully.',
      'Failed to Unarchive the category.'
    );
  }

  confirm<T>(
    action: () => Observable<T>,
    title: string,
    message: string,
    successMessage: string,
    errorMessage: string
  ): Observable<T | null> {

    return this.dialog.open(ConfirmDialogComponent, {
      data: {
        title,
        message
      }
    })
    .afterClosed()
    .pipe(
      filter(result => result === true),

      switchMap(() => action()),

      tap(() => {
        this.snackBar.open(
          successMessage,
          'Close',
          { duration: 3000 }
        );
      }),

      catchError(err => {
        this.snackBar.open(
          errorMessage,
          'Close',
          { duration: 3000 }
        );

        console.error(err);

        return of(null);
      })
    );
  }

}
