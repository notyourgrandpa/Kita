import { Injectable } from "@angular/core";
import { ApiResult, BaseService } from "../../shared/services/base.service";
import { InventoryLog } from "../models/inventory-logs";
import { Observable, catchError, filter, of, switchMap, tap, throwError } from "rxjs";
import { HttpClient, HttpErrorResponse, HttpParams } from "@angular/common/http";
import { Product } from "../../products/models/product";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ConfirmDialogComponent } from "../../shared/confirm-dialog/confirm-dialog.component";
import { Router } from "@angular/router";


@Injectable({
  providedIn: 'root',
})

export class InventoryLogService extends BaseService<InventoryLog>{
  constructor(http: HttpClient, private dialog: MatDialog, private snackBar: MatSnackBar, private router: Router) {
    super(http);
  }

  override getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null): Observable<ApiResult<InventoryLog>> {
    var url = this.getUrl("api/InventoryLogs");
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
    return this.http.get<ApiResult<InventoryLog>>(url, { params });
  }

  override get(id: number): Observable<InventoryLog> {
    var url = this.getUrl("api/inventoryLogs/" + id);
    return this.http.get<InventoryLog>(url);
  }
  override put(item: InventoryLog): Observable<InventoryLog> {
    var url = this.getUrl("api/inventoryLogs");
    return this.http.put<InventoryLog>(url, item);
  }
  override post(item: InventoryLog): Observable<InventoryLog> {
    var url = this.getUrl("api/inventoryLogs");
    return this.http.post<InventoryLog>(url, item).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = "An unknown error occurred.";
        if (error.error?.message) {
          errorMessage = error.error.message;
        } else if (error.status === 0) {
          errorMessage = "Backend is not reachable.";
        }
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  override delete(id: number): Observable<void> {
    var url = this.getUrl("api/inventoryLogs/" + id);
    return this.http.delete<void>(url);
  }

  voidInventoryLog(id: number):Observable<InventoryLog> {
    var url = this.getUrl("api/inventoryLogs/" + id);
    return this.http.patch<InventoryLog>(url, {});
  }

  confirmAndDelete(id: number, redirectTo?: string, reloadCallback?: () => void): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Inventory Log',
        message: 'Are you sure you want to delete this inventory log?'
      }
    }).afterClosed().pipe(
      filter(result => result === true),
      switchMap(() => this.delete(id)),
      tap(() => {
        this.snackBar.open('Inventory Log deleted successfully.', 'Close', { duration: 3000 });

        if (redirectTo) {
          this.router.navigate([redirectTo]);
        }

        if (reloadCallback) {
          reloadCallback();
        }
      }),
      catchError(err => {
        this.snackBar.open('Failed to delete the inventory.', 'Close', { duration: 3000 });
        console.error('Delete failed', err);
        return of(null);
      })
    ).subscribe();
  }
}
