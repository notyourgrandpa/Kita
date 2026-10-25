import { Component, OnInit, ViewChild } from '@angular/core';
import { ExpensesModel } from '../models/expenses.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ExpensesService } from '../services/expenses.service';

@Component({
  selector: 'app-expenses',
  templateUrl: './expenses.component.html',
  styleUrl: './expenses.component.scss'
})
export class ExpensesComponent implements OnInit {
  constructor(private expensesService: ExpensesService) { }

  ngOnInit() {
    this.loadData();
  }

  public displayedColumns = [
    'category',
    'description',
    'amount',
    'date'
  ];
  public expenses!: MatTableDataSource<ExpensesModel>;
  isLoading = true;

  defaultPageIndex = 0;
  defaultPageSize = 10;
  public defaultSortColumn = "date";
  public defaultSortOrder: "asc" | "desc" = "asc";

  pageIndex = 0;
  pageSize = 10;
  totalCount = 0;

  @ViewChild(MatSort) sort!: MatSort;

  filterTextChanged: Subject<string> = new Subject<string>();
  filterQuery?: string;

  onFilterTextChanged(filterText: string) {
    if (!this.filterTextChanged.observed) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        })
    }
  }

  loadData(query?: string) {
    this.isLoading = true;
    var pageEvent = new PageEvent;
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
      ? this.defaultSortColumn
      : null;
    var filterQuery = (this.filterQuery)
      ? this.filterQuery
      : null;

    this.expensesService.getData(
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
          this.expenses = new MatTableDataSource<ExpensesModel>(result.data);
        },
        error: (error) => {
          this.isLoading = false;
          console.log(error);
        }
      })
  }
}
