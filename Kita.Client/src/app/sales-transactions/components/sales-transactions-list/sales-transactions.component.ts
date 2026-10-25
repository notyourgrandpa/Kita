import { Component, OnInit, ViewChild } from '@angular/core';
import { SalesTransaction } from '../../models/sales-transaction';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { SalesTransactionService } from '../../services/sales-transaction.service';

@Component({
  selector: 'app-sales-transactions',
  templateUrl: './sales-transactions.component.html',
  styleUrl: './sales-transactions.component.scss'
})
export class SalesTransactionsComponent implements OnInit {
  ngOnInit() {
    this.loadData();
  }
  public displayedColumns: string[] = [
    'totalAmount',
    'paymentMethod',
    'processedByUsername',
    'date',
    'actions'
  ]

  public salesTransactions!: MatTableDataSource<SalesTransaction>;
  isLoading: boolean = true;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;

  public defaultSortColumn: string = "date";
  public defaultSortOrder: 'asc' | 'desc' = 'asc';

  defaultFilterColumn = "date";

  pageIndex: number = 0;
  pageSize: number = 10;
  totalCount: number = 0;

  filterQuery?: string;

  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(private salesTransactionService: SalesTransactionService) { }

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

    this.salesTransactionService.getData(
      event.pageIndex,
      event.pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery)
      .subscribe({
        next: (result) => {
          this.isLoading = false;
          this.totalCount  = result.totalCount;
          this.pageIndex = result.pageIndex;
          this.pageSize = result.pageSize;
          this.salesTransactions = new MatTableDataSource<SalesTransaction>(result.data);
        },
        error: (error) => {
          this.isLoading = false;
          console.error(error)
        }
      });
  }

  onDelete(id: number): void {
    this.salesTransactionService.confirmAndDelete(id, undefined, () => this.loadData());
  }
}
