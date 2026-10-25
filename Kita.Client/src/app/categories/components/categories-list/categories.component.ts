import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Category } from '../../models/category';
import { CategoryService } from '../../services/category.service';
import { CategoryGraphQlService } from '../../services/categories-graphql.service'
import { MatDialog } from '@angular/material/dialog';
import { ProductListDialogComponent } from '../../../products/components/product-list-dialog/product-list-dialog.component';
import { CategoryEditComponent } from '../category-edit/category-edit.component';
import { AuthService } from '../../../auth/auth.service';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  public displayedColumns: string[] = [
    'name',
    'isActive',
    'totalProducts',
    'action'
  ];
  public categories: MatTableDataSource<Category> = new MatTableDataSource<Category>([]);
  isLoading = true;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";

  defaultFilterColumn: string = "name";
  filterQuery?: string;

  public pageIndex = 0;
  public pageSize = 10;
  public totalCount = 0;

  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(
    private categoryService: CategoryService,
    private categoryGraphQlService: CategoryGraphQlService,
    private dialog: MatDialog,
    private authService: AuthService,
    private router: Router,

  ) {
  }

  ngOnInit() {
    this.loadData();
  }

  // debounce filter text changes
  onFilterTextChanged(filterText: string) {
    if (!this.filterTextChanged.observed) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        });
    }
    this.filterTextChanged.next(filterText);
  }

  loadData(query?: string) {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  getData(event: PageEvent) {
    this.isLoading = true;
    console.log('PAGINATOR EVENT:', event);

    const sortColumn = this.sort
      ? this.sort.active
      : this.defaultSortColumn;

    const sortOrder = this.sort
      ? this.sort.direction
      : this.defaultSortOrder;
    var filterColumn = (this.filterQuery)
      ? this.defaultFilterColumn
      : null;
    var filterQuery = (this.filterQuery)
      ? this.filterQuery
      : null;

    this.categoryService.getData(
      event.pageIndex,
      event.pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery)
      .subscribe({
        next: (result) => {
          this.isLoading = false;
          this.categories.data = result.data
          this.totalCount = result.totalCount;
          this.pageIndex = result.pageIndex;
          this.pageSize = result.pageSize;
        },
        error: (error) => {
          this.isLoading = false;
          console.error(error);
        }
      });
  }

  onDelete(id: number): void {
    if (!id) return;
    this.categoryService
      .confirmAndDelete(id)
      .subscribe({
        next: (result) =>{
          this.loadData();
        }
      })

  }

  viewCategoryProducts(categoryId: number) {
    this.dialog.open(ProductListDialogComponent,
      {
        width: '800px',
        data: { categoryId }
      })
  }

  openCategoryEditDialog(categoryId: number = 0) {
    if (!this.authService.redirectToLoginIfNotAuthenticated(this.router.url)) {
      return;
    }
    const dialogRef = this.dialog.open(CategoryEditComponent,
      {
        width: '800px',
        data: { id: categoryId }
      });
    dialogRef.afterClosed().subscribe(result =>
    {
      console.log(result);
      if (result)
        this.loadData();
    })
  }
}
