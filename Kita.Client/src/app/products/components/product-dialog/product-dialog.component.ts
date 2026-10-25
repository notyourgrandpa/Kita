import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service'

@Component({
  selector: 'app-product-dialog',
  templateUrl: './product-dialog.component.html',
  styleUrl: './product-dialog.component.scss'
})
export class ProductDialogComponent implements OnInit {
  product!: Product;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { productId: number },
    private dialogRef: MatDialogRef<ProductDialogComponent>,
    private productService: ProductService
  ) { }

  ngOnInit() {
    this.productService.get(this.data.productId).subscribe(p => this.product = p);
  }
}
