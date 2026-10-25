//import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, Inject, OnInit, inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AsyncValidatorFn, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, map } from 'rxjs';
//import { environment } from '../../environments/environment';
import { Category } from '../../models/category';
import { BaseFormComponent } from '../../../shared/components/base-form.component';
import { CategoryService } from '../../services/category.service';
import { CategoryGraphQlService } from '../../services/categories-graphql.service'
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar'
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component'

@Component({
  selector: 'app-category-edit',
  templateUrl: './category-edit.component.html',
  styleUrl: './category-edit.component.scss'
})
export class CategoryEditComponent extends BaseFormComponent implements OnInit{
  // the view title
  title?: string;
  // the category object to edit or create
  category?: Category;
  // the category object id, as fetched from the active route:
  // It's NULL when we're adding a new category, and not NULL when we're editing an existing one.
  id?: number;
  // the categories array for the select
  categories?: Category[];

  dialogRef = inject(MatDialogRef<CategoryEditComponent>);

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private categoryService: CategoryService,
    private categoryGraphQlService: CategoryGraphQlService,
    @Inject(MAT_DIALOG_DATA) public data: { id: number },
    private snackBar: MatSnackBar
  ) {
    super();
  }
  ngOnInit() {
    this.id = this.data.id;
    this.form = this.fb.group({
      name: ['',
        Validators.required,
        this.isDupeField("name")
      ]
    });
    this.loadData();
  }
  loadData() {
    if (this.id) {
      // EDIT MODE
      // fetch the category from the server
      this.categoryService.get(this.id).subscribe({
        next: (result) => {
          this.category = result;
          this.title = "Edit - " + this.category.name;
          // update the form with the category value
          this.form.patchValue(this.category);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Category";
    }
  }
  onSubmit() {
    var category = (this.id) ? this.category : <Category>{};
    if (category) {
      category.name = this.form.controls['name'].value;
      if (this.id) {
        // EDIT mode
        this.categoryService
          .put(category)
          .subscribe({
            next: (result) => {
              this.snackBar.open("Category " + category!.id + " has been updated.", undefined, { duration: 2000 });
              console.log("fire");
              this.dialogRef.close(true);
            },
            error: (error) => console.error(error)
          });
      }
      else {
        // ADD NEW mode
        this.categoryService
          .post(category)
          .subscribe({
            next: (result) => {
              // go back to categories view
              this.dialogRef.close(true);
            },
            error: (error) => console.error(error)
          });
      }
    }
  }  

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{
      [key: string]: any
    } | null> => {
      return this.categoryService.isDupeField(
        this.id ?? 0,
        fieldName,
        control.value)
        .pipe(map(result => {
          return (result ? { isDupeField: true } : null);
        }));
    }
  }

  onDelete(): void {
    if (!this.id) return;

    this.categoryService
      .confirmAndDelete(this.id)
      .subscribe(result => {
        console.log(result);
        if (result == null) {
          this.dialogRef.close(true);
        }
      });
  }

  onToggleArchive(): void {
    if (!this.id) return;

    if(this.category?.isActive){
      this.categoryService
      .confirmAndArchive(this.id)
      .subscribe(result => {
        if (result == null) {
          this.dialogRef.close(true);
        }
      });
    }
    else{
      this.categoryService
      .confirmAndUnarchive(this.id)
      .subscribe(result => {
        if (result == null) {
          this.dialogRef.close(true);
        }
      });
    }

  }
}
