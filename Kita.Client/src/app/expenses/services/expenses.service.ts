import { Observable } from "rxjs";
import { ApiResult, BaseService } from "../../shared/services/base.service";
import { ExpensesModel } from "../models/expenses.model";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root',
})

export class ExpensesService extends BaseService<ExpensesModel> {
  override get(id: number): Observable<ExpensesModel> {
    let url = this.getUrl("api/expenses/" + id);
    return this.http.get<ExpensesModel>(url);
  }
  override put(item: ExpensesModel): Observable<ExpensesModel> {
    let url = this.getUrl("api/expenses");
    return this.http.put<ExpensesModel>(url, item);
  }
  override post(item: ExpensesModel): Observable<ExpensesModel> {
    let url = this.getUrl("api/expenses");
    return this.http.post<ExpensesModel>(url, item);
  }
  override delete(id: number): Observable<void> {
    let url = this.getUrl("api/expenses/" + id);
    return this.http.delete<void>(url);
  }
  constructor(http: HttpClient)
  {
    super(http);
  }

  getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null): Observable<ApiResult<ExpensesModel>> {
    var url = this.getUrl("api/expenses");
    var params = new HttpParams()
      .set("pageIndex", pageIndex)
      .set("pageSize", pageSize)
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder)
    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery)
    }
    return this.http.get<ApiResult<ExpensesModel>>(url, {params})
  }
}
