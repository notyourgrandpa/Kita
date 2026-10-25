import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-product-list-dialog',
  templateUrl: './product-list-dialog.component.html',
  styleUrl: './product-list-dialog.component.scss'
})
export class ProductListDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data:
      {
        categoryId: number
      })
  {
  }
}
