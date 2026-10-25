import { AuthService } from './../../../auth/auth.service';
import { InventoryLogEditComponent } from './../inventory-log-edit/inventory-log-edit.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { InventoryLog } from '../../models/inventory-logs';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { InventoryLogService } from '../../services/inventory-logs.service';
import { ProductDialogService } from '../../../products/services/product-dialog.service'
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { InventoryLogDetailsComponent } from '../inventory-log-details/inventory-log-details.component';

@Component({
  selector: 'app-inventory-logs',
  templateUrl: './inventory-logs.component.html',
  styleUrl: './inventory-logs.component.scss'
})
export class InventoryLogsComponent implements OnInit {
  public displayedColumns: string[] = [
    'date',
    'productName',
    'quantity',
    'changeType',
    'remarks',
    'actions'
  ];

  public inventoryLogs!: MatTableDataSource<InventoryLog>;
  isLoading = true;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "date";
  public defaultSortOrder: "asc" | "desc" = "asc";

  defaultFilterColumn: string = "date";
  filterQuery?: string;

  public pageIndex = 0;
  public pageSize = 10;
  public totalCount = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(
    private inventoryLogService: InventoryLogService,
    private productDialogService: ProductDialogService,
    private dialog: MatDialog,
    private authService: AuthService,
    private snackBar: MatSnackBar) {
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
    var sortColumn = (this.sort)
      ? this.sort.active
      : this.defaultSortColumn;
    var sortOrder = (this.sort)
      ? this.sort.direction
      : this.defaultSortOrder;
    var filterColumn = (this.filterQuery)
      ? this.defaultFilterColumn
      : null;
    var filterQuery = (this.filterQuery)
      ? this.filterQuery
      : null;

    this.inventoryLogService.getData(
      event.pageIndex,
      event.pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery)
      .subscribe({
        next: (result) => {
          this.isLoading = false;
          this.totalCount = result.totalCount;
          this.pageIndex = result.pageIndex;
          this.pageSize = result.pageSize;
          this.inventoryLogs = new MatTableDataSource<InventoryLog>(result.data);
        },
        error: (error) => {
          this.isLoading = false;
          console.error(error)
        }
      });
  }

  onDelete(id: number): void {
    if (!id) return;
    this.inventoryLogService.confirmAndDelete(id, undefined, () => this.loadData());
  }

  openProductDialog(productId: number) {
    this.productDialogService.open(productId);
  }

  openInventoryLogEditDialog(inventoryLogId?: number) {
    const isEdit = !!inventoryLogId;
    if (!this.authService.canEdit('inventory-log')) {
      this.snackBar.open(
        isEdit
          ? 'You do not have permission to edit inventory logs.'
          : 'You do not have permission to create inventory logs.', 'Close', { duration: 3000 });
      return; // Prevent dialog from opening
    }

    const dialogRef = this.dialog.open(InventoryLogEditComponent, {
      width: '600px',
      data: inventoryLogId
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadData();
    });
  }

  openInventoryLogDetailsDialog(inventoryLogId: number) {
    const dialogRef = this.dialog.open(InventoryLogDetailsComponent, { width: '800px', data: inventoryLogId});
  }
}
