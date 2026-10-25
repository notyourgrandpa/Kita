import { Injectable } from '@angular/core';
import { ProductDialogComponent } from '../components/product-dialog/product-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root'
})

export class ProductDialogService {
  constructor(private dialog: MatDialog) {

  }
  open(productId: number) {
    this.dialog.open(ProductDialogComponent,
      {
        width: '450px',
        data: { productId },
        panelClass: 'custom-dialog-container'
      }
    )
  }
}
