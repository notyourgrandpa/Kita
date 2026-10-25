import { Component, OnInit, OnDestroy, inject, Inject } from '@angular/core';
//import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

//import { environment } from '../../environments/environment';
import { Product } from '../../models/product';
import { Unit } from '../../models/unit';
import { Category } from '../../../categories/models/category';
import { BaseFormComponent } from '../../../shared/components/base-form.component'
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

import { ProductService } from '../../services/product.service';
import { ProductGraphQlService } from '../../services/product-graphql.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryService } from '../../../categories/services/category.service';


@Component({
  selector: 'app-product-edit-dialog',
  templateUrl: './product-edit-dialog.component.html',
  styleUrl: './product-edit-dialog.component.scss'
})
export class ProductEditDialogComponent extends BaseFormComponent implements OnInit, OnDestroy {
  // the view title
  title?: string;

  // the product object to edit or create
  product?: Product;

  // the product object id, as fetched from the active route: It's NULL when we're adding a new product,
  // and not NULL when we're editing an existing one.
  id?: number;

  // the countries observable for the select (using async pipe)
  categories?: Observable<Category[]>;
  units?: Observable<Unit[]>;

  // Activity Log (for debugging purposes)
  //activityLog: string = '';

  private destroySubject = new Subject();

  dialogRef = inject(MatDialogRef<ProductEditDialogComponent>);

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private productGraphQlService: ProductGraphQlService,
    private categoryService: CategoryService,
    @Inject(MAT_DIALOG_DATA) public data: { productId: number },
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    super();
    this.id = data.productId ?? 0;
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      categoryId: new FormControl('', Validators.required),
      costPrice: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      sellingPrice: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      isActive: new FormControl('', Validators.required),
      unit: new FormControl('', Validators.required),
      minimumStock: new FormControl('', [Validators.required, Validators.pattern(/^[0-9]+$/)]),
    }, null, this.isDupeProduct());

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

    // load categories
    this.loadCategories();
    this.loadUnits();

    if (this.id) {
      // EDIT MODE
      // fetch the product from the server
      this.productService.get(this.id).subscribe({
        next: (result) => {
          this.product = result;
          this.title = "Edit - " + this.product.name;
          // update the form with the product value
          this.form.patchValue(this.product);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Product";
    }
  }

  loadCategories() {
    // fetch all the countries from the server
    this.categories = this.categoryService
      .getData(0, 9999, "name", "asc", null, null)
      .pipe(map(x => x.data));
  }

  loadUnits(){
    this.units = this.productService.getUnits();
    console.log(this.units);
  }

  onSubmit() {
    var product = (this.id) ? this.product : <Product>{};
    if (product) {
      product.name = this.form.controls['name'].value;
      product.categoryId = +this.form.controls['categoryId'].value;
      product.costPrice = +this.form.controls['costPrice'].value;
      product.sellingPrice = +this.form.controls['sellingPrice'].value;
      product.isActive = this.form.controls['isActive'].value;
      product.unit = this.form.controls['unit'].value;
      product.minimumStock = +this.form.controls['minimumStock'].value;

      if (this.id) {
        // EDIT mode
        this.productService
          .put(product)
          .subscribe({
            next: (result) => {
              this.snackBar.open("Product updated successfully.", "Close", { duration: 3000 });
              this.dialogRef.close(true);
            },
            error: (error) => console.error(error)
          });
      }
      else {
        // ADD NEW mode
        this.productService
          .post(product)
          .subscribe({
            next: (result) => {
              this.snackBar.open("Product created successfully.", "Close", { duration: 3000 });
              // go back to products view
              this.dialogRef.close(true);
            },
            error: (error) => console.error(error)
          });
      }
    }
  }

  onDelete(): void {
    if (!this.id) return;

    this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Product',
        message: 'Are you sure you want to delete this product?'
      }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.productService.delete(this.id!).subscribe({
          next: () => {
            this.snackBar.open("Product deleted successfully.", "Close", { duration: 3000 });
            this.dialogRef.close(true);
            this.loadData();
          },
          error: (error) => console.error(error)
        });
      }
    });
  }

  isDupeProduct(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var product = <Product>{};
      product.id = (this.id) ? this.id : 0;
      product.name = this.form.controls['name'].value;
      product.categoryId = +this.form.controls['categoryId'].value;
      product.costPrice = +this.form.controls['costPrice'].value;
      product.sellingPrice = +this.form.controls['sellingPrice'].value;
      return this.productService.isDupeProduct(product).pipe(map(result => {
        return (result ? { isDupeProduct: true } : null);
      }));
    }
  }
}
