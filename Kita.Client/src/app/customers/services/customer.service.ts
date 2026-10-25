import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Customer } from '../models/customer';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private http: HttpClient) { }

  search(term: string): Observable<Customer[]> {
    if (!term || term.trim() === '') {
      return of([]); 
    }
    return this.http.get<Customer[]>(`/api/customers/search`, {
      params: { term }
    });
  }
}
