import { Component, Inject, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Product } from '../../models/product';

@Component({
  selector: 'app-product-details-dialog',
  templateUrl: './product-details-dialog.component.html',
  styleUrl: './product-details-dialog.component.css'
})
export class ProductDetailsDialogComponent implements OnInit {

  id?: number;

  product?: Product;
  
  constructor(private productService: ProductService,
    @Inject(MAT_DIALOG_DATA) public data: { productId: number }
  ) 
  { 
    this.id = data.productId ?? 0;
  }

  ngOnInit(): void {
    this.productService.get(<number>this.id).subscribe(product => {
      this.product = product;
    });
  }
}
