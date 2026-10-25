import { Component } from '@angular/core';
import { AdministrationService } from '../../services/administration.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-data-maintenance',
  templateUrl: './data-maintenance.component.html',
  styleUrl: './data-maintenance.component.scss'
})
export class DataMaintenanceComponent {
  selectedFile: File | null = null;
  isImporting: boolean = false;

  constructor(
    private adminService: AdministrationService,
    private snackBar: MatSnackBar
  ){

  }

  onImport(){
    if(!this.selectedFile){
      this.snackBar.open("File is missing.", "Close", { duration: 3000 });
      return;
    }

    this.isImporting = true;

    this.adminService
    .importData(this.selectedFile!)
      .pipe(finalize(() => {
        this.isImporting = false;
      }))
      .subscribe({
        next: (result) => {
          console.log(result);
          this.snackBar.open(`Imported ${result.categories} categories and ${result.products} products.`, "Ok", { duration: 3000 });
        },
        error: (error) =>{
          this.snackBar.open(error?.error?.message ?? "Import failed.", "Ok", { duration: 5000 });
        }
      })
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files?.length) {
      this.selectedFile = input.files[0];
    }
  }
}
