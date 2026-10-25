import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { BaseFormComponent } from '../../../shared/components/base-form.component';
import { InventoryLog } from '../../models/inventory-logs';
import { Observable, Subject, combineLatest } from 'rxjs';
import { map, startWith, takeUntil } from 'rxjs/operators';
import { Product } from '../../../products/models/product';
import { ActivatedRoute, Router } from '@angular/router';
import { InventoryLogService } from '../../services/inventory-logs.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';
import { ProductService } from '../../../products/services/product.service';

@Component({
  selector: 'app-inventory-log-edit',
  templateUrl: './inventory-log-edit.component.html',
  styleUrl: './inventory-log-edit.component.scss'
})
export class InventoryLogEditComponent extends BaseFormComponent implements OnInit, OnDestroy{
  title?: string;
  inventoryLog?: InventoryLog;
  id?: number;
  products!: Observable<Product[]>;
  filteredProducts?: Observable<Product[]>;

  selectedProductStock: number | null = null;

  private destroySubject = new Subject();

  searchCtrl = new FormControl();

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private inventoryLogService: InventoryLogService,
    private productService: ProductService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public inventoryLogId: number,
    private dialogRef: MatDialogRef<InventoryLogEditComponent>
  ) {
    super();
    this.id = inventoryLogId ?? 0
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      productId: new FormControl('', Validators.required),
      quantity: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      changeType: new FormControl('', [Validators.required]),
      remarks: new FormControl('', [Validators.required])
    }, null);

    // react to form changes
    this.form.valueChanges
      .pipe(takeUntil(this.destroySubject))
      .subscribe(() => {
        if (!this.form.dirty) {
          this.log("Form Model has been loaded.");
        }
        else {
          this.log("Form was updated by the user.");
        }
      });

    // react to changes in the form.name control
    this.form.get("name")!.valueChanges
      .pipe(takeUntil(this.destroySubject))
      .subscribe(() => {
        if (!this.form.dirty) {
          this.log("Name has been loaded with initial values.");
        }
        else {
          this.log("Name was updated by the user.");
        }
      });

    this.loadProducts();

    this.filteredProducts = combineLatest([
      this.products,
      this.searchCtrl.valueChanges.pipe(startWith(''))
    ]).pipe(
      map(([products, search]) =>
        products.filter(p =>
          p.name.toLowerCase().includes((search ?? '').toLowerCase())
        )
      )
    );

    this.loadData();
  }

  ngOnDestroy() {
    // emit a value with the takeUntil notifier
    this.destroySubject.next(true);
    // complete the subject
    this.destroySubject.complete();
  }

  log(str: string) {
    console.log("["
      +
      new Date().toLocaleString()
      + "] " + str);
  }

  loadData() {
    // retrieve the ID from the 'id' parameter
    //var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    //this.id = idParam ? +idParam : 0;

    if (this.id) {
      // EDIT MODE
      // fetch the inventoryLog from the server
      this.inventoryLogService.get(this.id).subscribe({
        next: (result) => {
          this.inventoryLog = result;
          this.title = "Edit - Inventory Log";
          // update the form with the inventoryLog value
          this.form.patchValue(this.inventoryLog);

          this.onProductSelected(this.inventoryLog.productId);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Inventory Log";
    }
  }

  loadProducts() {
    // fetch all the products from the server
    this.products = this.productService
      .getData(0, 9999, "name", "asc", null, null)
      .pipe(map(x => x.data));
  }

  onProductSelected(productId: number) {
    // products$ is async, so subscribe here or in template
    this.products?.subscribe((products) => {
      const product = products.find(p => p.id === productId);
      this.selectedProductStock = product ? product.stock : null;
    });
  }

  onSubmit() {
    var inventoryLog = (this.id) ? this.inventoryLog : <InventoryLog>{};
    if (inventoryLog) {
      inventoryLog.productId = this.form.controls['productId'].value;
      inventoryLog.quantity = +this.form.controls['quantity'].value;
      inventoryLog.changeType = +this.form.controls['changeType'].value;
      inventoryLog.remarks = this.form.controls['remarks'].value;

      if (this.id) {
        // EDIT mode
        this.inventoryLogService
          .voidInventoryLog(inventoryLog.id)
          .subscribe({
            next: (result) => {
              this.snackBar.open("Inventory log has been voided successfully.", "Close", { duration: 3000 })
              this.dialogRef.close(true);
            },
            error: (error) => this.snackBar.open(error.error?.message, "Close")
          });
      }
      else {
        // ADD NEW mode
        this.inventoryLogService
          .post(inventoryLog)
          .subscribe({
            next: (result) => {
              console.log();
              this.snackBar.open("Inventory Log " + result.id + " has been created.", "Close", {duration: 3000})
              this.dialogRef.close(true);
            },
            error: (err) => {
              console.error("Error:", err.message);
              this.snackBar.open(err.message, "Close", { duration: 3000 });
            }
          });
      }
    }
  }

  onDelete(): void {
    if (!this.id) return;

    this.inventoryLogService.confirmAndDelete(this.id, '/products');
  }
}
