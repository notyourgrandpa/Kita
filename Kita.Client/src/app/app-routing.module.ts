import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { HealthCheckComponent } from './health-check/health-check.component';
import { ProductListComponent } from './products/components/product-list/product-list.component';
import { CategoriesComponent } from './categories/components/categories-list/categories.component';
import { CategoryEditComponent } from './categories/components/category-edit/category-edit.component';
import { LoginComponent } from './auth/login.component';

import { AuthGuard } from './auth/auth.guard';
import { InventoryLogsComponent } from './inventory-logs/components/inventory-logs-list/inventory-logs.component';
import { InventoryLogEditComponent } from './inventory-logs/components/inventory-log-edit/inventory-log-edit.component';
import { SalesTransactionsComponent } from './sales-transactions/components/sales-transactions-list/sales-transactions.component';
import { SalesTransactionEditComponent } from './sales-transactions/components/sales-transaction-edit/sales-transaction-edit.component';
import { ExpensesComponent } from './expenses/expenses-list/expenses.component';
import { ExpenseEditComponent } from './expenses/expense-edit/expense-edit.component';
import { ProductEditDialogComponent } from './products/components/product-edit-dialog/product-edit-dialog.component';
import { DataMaintenanceComponent } from './administration/components/data-maintenance/data-maintenance.component';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'health-check', component: HealthCheckComponent },
  { path: 'products', component: ProductListComponent },
  { path: 'product/:id', component: ProductEditDialogComponent, canActivate: [AuthGuard] },
  { path: 'product', component: ProductEditDialogComponent, canActivate: [AuthGuard] },
  { path: 'categories', component: CategoriesComponent },
  { path: 'category/:id', component: CategoryEditComponent, canActivate: [AuthGuard] },
  { path: 'category', component: CategoryEditComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'inventory-logs', component: InventoryLogsComponent },
  { path: 'inventory-log/:id', component: InventoryLogEditComponent, canActivate: [AuthGuard] },
  { path: 'inventory-log', component: InventoryLogEditComponent, canActivate: [AuthGuard] },
  { path: 'sales-transactions', component: SalesTransactionsComponent },
  { path: 'sales-transaction/:id', component: SalesTransactionEditComponent, canActivate: [AuthGuard] },
  { path: 'sales-transaction', component: SalesTransactionEditComponent, canActivate: [AuthGuard] },
  { path: 'expenses', component: ExpensesComponent },
  { path: 'expense', component: ExpenseEditComponent, canActivate: [AuthGuard] },
  { path: 'expense/:id', component: ExpenseEditComponent, canActivate: [AuthGuard] },
  { path: 'data-maintainance', component: DataMaintenanceComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
