import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../../shared/services/base.service';
import { Observable, map } from 'rxjs';
import { Category } from '../models/category';
import { Apollo, gql } from 'apollo-angular';


@Injectable({
  providedIn: 'root',
})
export class CategoryGraphQlService
  extends BaseService<Category> {
  constructor(
    http: HttpClient, private apollo: Apollo) {
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
    return this.apollo
      .query({
        query: gql`
          query GetCategoriesApiResult(
              $pageIndex: Int!,
              $pageSize: Int!,
              $sortColumn: String,
              $sortOrder: String,
              $filterColumn: String,
              $filterQuery: String) {
            categoriesApiResult(
              pageIndex: $pageIndex
              pageSize: $pageSize
              sortColumn: $sortColumn
              sortOrder: $sortOrder
              filterColumn: $filterColumn
              filterQuery: $filterQuery
            ) { 
               data { 
                 id
                 name
                 totalProducts
               },
                pageIndex
                pageSize
                totalCount
                totalPages
                sortColumn
                sortOrder
                filterColumn
                filterQuery
              }
          }
        `,
        variables: {
          pageIndex,
          pageSize,
          sortColumn,
          sortOrder,
          filterColumn,
          filterQuery
        }
      })
      .pipe(map((result: any) =>
        result.data.categoriesApiResult));
  }

  get(id: number): Observable<Category> {
    return this.apollo
      .query({
        query: gql`
          query GetCategoryById($id: Int!){
            categories(where: {$id: {eq: $id } }){
              nodes {
                id
                name
              }
            }
          }
        `,
        variables: {
          id
        }
      })
      .pipe(map((result: any) =>
        result.data.categories.nodes[0]));
  }

  put(input: Category): Observable<Category> {
    const dto = {
      id: input.id,
      name: input.name
    }
    return this.apollo
      .mutate({
        mutation: gql`
          mutation UpdateCategory($category: CategoryUpdateInputDTOInput!){
            updateCategory(categoryDTO: $category){
              id
              name
            }
          }
        `,
        variables: {
          category: dto
        }
      }).pipe(map((result: any) =>
        result.data.updateCategory));
  }

  post(item: Category): Observable<Category> {
    const dto = {
      name: item.name
    };
    return this.apollo
    .mutate({
      mutation: gql
        `mutation AddCategory($category: CategoryCreateInputDTOInput!){
          addCategory(categoryDTO: $category){
            id
            name
          }
        }
      `,
      variables: {
        category: dto
      }
    }).pipe(map((result: any) =>
      result.data.addCategory));
  }

  override delete(id: number): Observable<void> {
    throw new Error('Method not implemented.');
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
}
