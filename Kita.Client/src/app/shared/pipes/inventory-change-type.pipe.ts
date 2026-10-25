import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'inventoryChangeType'
})
export class InventoryChangeTypePipe implements PipeTransform {
  transform(value?: number): string {
    switch (value) {
      case 1: return 'Stock In';
      case 2: return 'Stock Out';
      case 3: return 'Adjustment';
      default: return 'Unknown';
    }
  }
}
