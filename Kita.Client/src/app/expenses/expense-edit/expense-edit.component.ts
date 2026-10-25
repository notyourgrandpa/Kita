import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExpensesService } from '../services/expenses.service';
import { ExpensesModel } from '../models/expenses.model';
import { BaseFormComponent } from '../../shared/components/base-form.component';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-expense-edit',
  templateUrl: './expense-edit.component.html',
  styleUrl: './expense-edit.component.scss'
})
export class ExpenseEditComponent extends BaseFormComponent implements OnInit {
  title?: string;
  id?: number;
  expense?: ExpensesModel;

  constructor(
    private activatedRoute: ActivatedRoute,
    private expensesService: ExpensesService,
    private snackBar: MatSnackBar,
    private router: Router
  )
  {
    super()
  }

  ngOnInit() {
    this.form = new FormGroup({
      category: new FormControl('', Validators.required),
      description: new FormControl('', Validators.required),
      amount: new FormControl('', [Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,2})?$/)]),
      date: new FormControl('', Validators.required)
    }, null)
    this.loadData();
  }

  onSubmit() {
    let expense = (this.id) ? this.expense : <ExpensesModel>{};

    if (expense) {
      expense.description = this.form.controls['description'].value;
      expense.category = this.form.controls['category'].value;
      expense.amount = this.form.controls['amount'].value;
      expense.date = this.form.controls['date'].value;

      if (this.id) {
        //edit mode
        this.expensesService.put(expense).subscribe({
          next: (result) => {
            this.snackBar.open("Expense " + this.id + "has been updated.");

            this.router.navigate(['/expenses']);
          },
          error: (error) => {
            this.snackBar.open(error);
          }
        })
      }
      else {
        //add mode
        this.expensesService.post(expense).subscribe({
          next: (result) =>{
            this.snackBar.open("Expense created succesfullly.");
            this.router.navigate(['/expenses']);
          },
          error: (error) => {
            this.snackBar.open(error);
          }
        })
      }
    }
  }

  onDelete(){

  }

  loadData() {
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      this.expensesService.get(this.id).subscribe({
        next: (result) => {
          this.expense = result;
        }
      });
    }
    else {
      this.title = "Create a new Expense";
    }
  }
}
