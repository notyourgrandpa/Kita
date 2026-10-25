import { AbstractControl, FormArray, ValidationErrors, ValidatorFn } from "@angular/forms";

export const paidEnoughValidator: ValidatorFn = (form: AbstractControl): ValidationErrors | null =>{
  const items = form.get('items') as FormArray;
  const paidAmount = form.get('paidAmount')?.value ?? 0;

  if (!items || !items.length) return null; // No items yet — don’t validate
  
  // Compute total based on items
  const total = items.controls.reduce((sum, item) => {
    const qty = item.get('qty')?.value || 0;
    const price = item.get('price')?.value || 0;
    return sum + (qty * price);
  }, 0);

  return paidAmount >= total ? null : { insufficientPayment: true}
}
