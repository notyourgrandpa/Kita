import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseFormComponent } from '../../../shared/components/base-form.component';
import { Product } from '../../../products/models/product';
import { Observable, Subject, debounceTime, distinctUntilChanged, filter, map, of, switchMap, takeUntil } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../products/services/product.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { SalesTransactionService } from '../../services/sales-transaction.service';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { SalesTransaction } from '../../models/sales-transaction';
import { SalesTransactionItem } from '../../models/sales-transaction-item';
import { Customer } from '../../../customers/models/customer';
import { CustomerService } from '../../../customers/services/customer.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatTableDataSource } from '@angular/material/table';
import { paidEnoughValidator } from '../../../shared/validators/paid-enough.validator';

@Component({
  selector: 'app-sales-transaction-edit',
  templateUrl: './sales-transaction-edit.component.html',
  styleUrl: './sales-transaction-edit.component.scss'
})
export class SalesTransactionEditComponent extends BaseFormComponent implements OnInit, OnDestroy {
  // the view title
  title?: string;

  // the product object to edit or create
  salesTransaction?: SalesTransaction;

  // the product object id, as fetched from the active route: It's NULL when we're adding a new product,
  // and not NULL when we're editing an existing one.
  id?: number;

  // the countries observable for the select (using async pipe)
  products?: Observable<Product[]>;

  // Activity Log (for debugging purposes)
  //activityLog: string = '';

  private destroySubject = new Subject();

  filteredCustomers!: Observable<Customer[]>;
  filteredProducts!: Observable<Product[]>;

  productSearch = new FormControl('');
  selectedProduct?: Product;

  dataSource = new MatTableDataSource<FormGroup>();

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private salesTransactionService: SalesTransactionService,
    private customerService: CustomerService,
    private productService: ProductService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private fb: FormBuilder
  ) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      customer: [null, Validators.required],
      paymentMethod: ['Cash', Validators.required],
      items: this.fb.array<FormGroup>([], Validators.required),
      paidAmount: [0, [Validators.required, Validators.pattern(/^[0-9]+(\.[0-9]{1,2})?$/)]]
    }, { validators: paidEnoughValidator });

    this.filteredProducts = this.productSearch.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      map(value => typeof value === 'string' ? value :  ''),
      filter(term => term.length > 0),
      switchMap(term => this.productService.search(term))
    );

    this.filteredCustomers = this.form.get('customer')!.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      map(value => typeof value === 'string' ? value : value?.name ?? ''),
      filter(term => term.length > 0),
      switchMap(term => this.customerService.search(term))
    );

    this.items.valueChanges.subscribe(() => {
      this.dataSource.data = this.items.controls;
    });
    // initialise once
    this.dataSource.data = this.items.controls;

    this.loadData();
  }

  ngOnDestroy() {
    // emit a value with the takeUntil notifier
    this.destroySubject.next(true);
    // complete the subject
    this.destroySubject.complete();
  }

  loadData() {

    // load categories
    this.loadProducts();

    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      // EDIT MODE
      // fetch the product from the server
      this.salesTransactionService.get(this.id).subscribe({
        next: (result) => {
          this.salesTransaction = result;
          this.title = "Edit - " + this.salesTransaction.id;
          // update the form with the product value
          this.form.patchValue(this.salesTransaction);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Sales Transaction";

      // --- Option A: if you are sure Walk-in has Id = 1
      this.form.get('customer')!.setValue({ id: 1, name: 'Walk-in' });

      // --- Option B: fetch it dynamically
      // this.customerService.search('Walk-in')
      //   .pipe(map(list => list.find(c => c.name === 'Walk-in')))
      //   .subscribe(c => this.form.get('customer')!.setValue(c));
    }
  }

  loadProducts() {
    // fetch all the products from the server
    this.products = this.productService
      .getData(0, 9999, "name", "asc", null, null)
      .pipe(map(x => x.data));
  }

  get items(): FormArray<FormGroup> {
    return this.form.get('items') as FormArray<FormGroup>;
  }

  displayCustomer(customer: Customer): string {
    return customer ? customer.name : '';
  }

  displayProduct(product: Product): string {
    return product ? `${product.name} - ${product.sellingPrice.toLocaleString()}₱` : '';
  }

  onProductSelected(event: MatAutocompleteSelectedEvent) {
    this.selectedProduct = event.option.value as Product;

    // immediately add to cart if you like:
    // this.addProduct();
    // or wait for Add button
  }

  addProduct() {
    if (!this.selectedProduct) return;
    const product = this.selectedProduct;

    // Look for existing item in the cart
    const existing = this.items.controls.find(
      ctrl => ctrl.value.product.id === product.id
    );

    if (existing) {
      // increment quantity and update subtotal
      const qty = existing.value.qty + 1;
      existing.patchValue({
        qty,
        subtotal: qty * product.sellingPrice
      });
    } else {
      // add a new FormGroup to the FormArray
      this.items.push(
        this.fb.group({
          product: [product],
          qty: [1, [Validators.required, Validators.min(1)]],
          price: [product.sellingPrice],
          subtotal: [product.sellingPrice]
        })
      );
    }

    // clear the autocomplete input
    this.productSearch.reset('');
    this.selectedProduct = undefined;
  }


  removeItem(index: number) {
    this.items.removeAt(index);
  }

  updateSubtotal(index: number) {
    const group = this.items.at(index);
    const qty = group.get('qty')!.value;
    const price = group.get('price')!.value;
    group.get('subtotal')!.setValue(qty * price, { emitEvent: false });
  }

  get total(): number {
    return this.items.controls
      .map(ctrl => ctrl.value.subtotal)
      .reduce((a, b) => a + b, 0);
  }

  get change(): number {
    return (this.form.get('paidAmount')!.value || 0) - this.total;
  }

  onSubmit() {
    var salesTransaction = this.id ? this.salesTransaction : <SalesTransaction>{}
    // edit mode
    if (salesTransaction) {
      const itemsControl = this.form.get('items') as FormArray;

      const items: SalesTransactionItem[] = itemsControl.controls.map(control => {
        const value = control.value;
        return {
          productId: value.product.id,
          quantity: value.qty,
          unitPrice: value.price,
          subtotal: value.subtotal
        };
      });

      var customer = this.form.get('customer')!.value;

      salesTransaction.customerId = customer.id;
      salesTransaction.paymentMethod = this.form.get('paymentMethod')!.value;
      salesTransaction.items = items;

      if (this.id) {
        this.salesTransactionService.update(salesTransaction).subscribe({
          next: (result) => {
            this.router.navigate(['/sales-transactions']);
          },
          error: (error) => {
            this.snackBar.open(error);
          }
        });
      }
      // create mode
      else {
        this.salesTransactionService.create(salesTransaction).subscribe({
          next: (result) => {
            this.router.navigate(['/sales-transactions']);
          },
          error: (error) => {
            this.snackBar.open(error.error.message, "Ok", {duration: 3000});
          }
        });
      }
    }
  }

}
