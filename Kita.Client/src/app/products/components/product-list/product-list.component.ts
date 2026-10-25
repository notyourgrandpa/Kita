import { ProductQueryParams } from '../../models/product-query-params';
import { Component, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, SortDirection } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { Subject, debounceTime, distinctUntilChanged, map, Observable } from 'rxjs';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { Category } from '../../../categories/models/category';
import { CategoryQueryParams } from '../../../inventory-logs/models/category-query-params';
import { CategoryService } from '../../../categories/services/category.service';
import { ProductEditDialogComponent } from '../product-edit-dialog/product-edit-dialog.component';
import { AuthService } from '../../../auth/auth.service';
import { Router } from '@angular/router';
import { Unit } from '../../models/unit';

@Component({
  selector: 'app-product-list-core',
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnChanges {
  private readonly baseColumns: string[] = [
    'name',
    'categoryName',
    'costPrice',
    'sellingPrice',
    'stock',
    'isActive'
  ];

  private readonly ExtendedColumns: string[] = ['unit','dateAdded', 'lastUpdated'];
  public displayedColumns: string[] = [...this.baseColumns, 'actions'];
  public products: MatTableDataSource<Product> = new MatTableDataSource<Product>([]);
  public loading = false;
  public categories?: Observable<Category[]> ;
  public units?: Observable<Unit[]>;
  @Input() categoryId?: number;
  @Input() compact = false;
  selectedCategoryId: number | null = null;
  selectedStatus: boolean | null = null;
  selectedUnit: number | null = null;

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
    private productService: ProductService,
    private categoryService: CategoryService,
    private authService: AuthService,
    private dialog: MatDialog,
    private router: Router
  ) {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryId']) {
      this.selectedCategoryId = this.categoryId ?? null;
    }

    if (changes['compact']) {
      this.updateDisplayedColumns();
    }
  }

  ngOnInit() {
    this.updateDisplayedColumns();
    this.loadData();
    this.loadCategories();
    this.loadUnits();
  }

  private updateDisplayedColumns(): void {
    this.displayedColumns = this.compact ? [...this.baseColumns] : [...this.baseColumns, ...this.ExtendedColumns, 'actions'];
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
    this.loading = true;
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  getData(event: PageEvent) {
    const sortColumn = this.sort
      ? this.sort.active
      : this.defaultSortColumn;

    const sortOrder = this.sort
      ? this.sort.direction
      : this.defaultSortOrder;

    const filterColumn = this.filterQuery
      ? this.defaultFilterColumn
      : undefined;

    const filterQuery = this.filterQuery
      ? this.filterQuery
      : undefined;

    const categoryId = this.selectedCategoryId ?? undefined;
    const status = this.selectedStatus ?? undefined;
    const unitId = this.selectedUnit ?? undefined;
    console.log(`getData called with pageIndex: ${event.pageIndex}, pageSize: ${event.pageSize}, sortColumn: ${sortColumn}, sortOrder: ${sortOrder}, filterColumn: ${filterColumn}, filterQuery: ${filterQuery}, categoryId: ${categoryId}, status: ${status}, unitId: ${unitId}`);

    const productQueryParams: ProductQueryParams = {
      pageIndex: event.pageIndex,
      pageSize: event.pageSize,
      filterColumn,
      filterQuery,
      filter: {
        categoryId,
        isActive: status,
        unitId
      },
      sort: sortColumn,
      direction: sortOrder
    };

    this.loading = true;

    this.productService.getProducts(productQueryParams).subscribe({
      next: (result) => {
        this.products.data = result.data;

        this.pageIndex = result.pageIndex;
        this.pageSize = result.pageSize;
        this.totalCount = result.totalCount;

        this.loading = false;
      },
      error: (error) => {
        console.error(error);
        this.loading = false;
      }
    });
  }

  loadCategories(){
    const categoryQueryParams: CategoryQueryParams = {
      pageIndex: 0,
      pageSize: 999,
      sort: 'name',
      search: '',
      direction: 'asc'
    }
    this.categories = this.categoryService
      .getCategories(categoryQueryParams)
      .pipe(map(x => x.data));
  }

  loadUnits(){
    this.units = this.productService.getUnits();
  }

  onDelete(id: number): void {
    if(!this.authService.redirectToLoginIfNotAuthenticated(this.router.url)){
      return;
    }
    this.productService.confirmAndDelete(id, undefined, () => this.loadData());
  }

  onCategoryChanged(categoryId: number) {
    this.selectedCategoryId = categoryId;
    this.loadData();
  }

  onStatusChanged(status: boolean) {
    this.selectedStatus = status;
    this.loadData();
  }

  onUnitChanged(unitId: number) {
    this.selectedUnit = unitId;
    this.loadData();
  }


  openProductEditDialog(productId: number = 0){
    if(!this.authService.redirectToLoginIfNotAuthenticated(this.router.url)){
      return;
    }
  
    this.dialog.open(ProductEditDialogComponent, {
      width: '600px',
      data: { productId: productId }
    }).afterClosed().subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }
}
