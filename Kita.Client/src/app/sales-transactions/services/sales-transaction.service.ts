import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResult, BaseService } from '../../shared/services/base.service';
import { SalesTransaction } from '../models/sales-transaction';
import { Observable, catchError, filter, of, switchMap, tap } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Product } from '../../products/models/product';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class SalesTransactionService extends BaseService<SalesTransaction> {

  constructor(http: HttpClient, private snackBar: MatSnackBar, private router: Router, private dialog: MatDialog) { super(http) }

  override getData(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string, filterColumn: string | null, filterQuery: string | null): Observable<ApiResult<SalesTransaction>> {
    var url = this.getUrl("api/salesTransactions");
    var params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);
    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery)
    }

    return this.http.get<ApiResult<SalesTransaction>>(url, { params });
  }
  override get(id: number): Observable<SalesTransaction> {
    var url = this.getUrl("api/salesTransactions/" + id);
    return this.http.get<SalesTransaction>(url);
  }
  override put(item: SalesTransaction): Observable<SalesTransaction> {
    var url = this.getUrl("api/salesTransactions/" + item.id);
    return this.http.put<SalesTransaction>(url, item);
  }
  override post(item: SalesTransaction): Observable<SalesTransaction> {
    var url = this.getUrl("api/salesTransactions");
    return this.http.post<SalesTransaction>(url, item);
  }
  override delete(id: number): Observable<void> {
    var url = this.getUrl("api/salesTransactions/" + id);
    return this.http.delete<void>(url);
  }

  confirmAndDelete(id: number, redirectTo?: string, reloadCallback?: () => void): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Sales Transaction',
        message: 'Are you sure you want to delete this sales transaction?'
      }
    }).afterClosed().pipe(
      filter(result => result === true),
      switchMap(() => this.delete(id)),
      tap(() => {
        this.snackBar.open('Sales Transaction deleted successfully.', 'Close', { duration: 3000 });

        if (redirectTo) {
          this.router.navigate([redirectTo]);
        }

        if (reloadCallback) {
          reloadCallback();
        }
      }),
      catchError(err => {
        this.snackBar.open('Failed to delete the sales transaction.', 'Close', { duration: 3000 });
        console.error('Delete failed', err);
        return of(null);
      })
    ).subscribe();
  }

  create(transaction: SalesTransaction): Observable<SalesTransaction> {
    return this.http.post<SalesTransaction>('/api/salesTransactions', transaction);
  }

  update(transaction: SalesTransaction): Observable<SalesTransaction> {
    return this.http.put<SalesTransaction>(`/api/salesTransactions/${transaction.id}`, transaction);
  }
}
