import { CategoryService } from '../../../categories/services/category.service';
import { AuthService } from '../../../auth/auth.service';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from '../../../shared/angular-material.module';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { ProductListComponent } from '../product-list/product-list.component';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiResult } from '../../../shared/services/base.service';

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  beforeEach(async () => {
    // Create a mock productService object with a mock 'getProducts' method
    let productService = jasmine.createSpyObj<ProductService>('ProductService',
      ['getProducts', 'getUnits']);
    // Configure the 'getProducts' spy method
    productService.getProducts.and.returnValue(
      // return an Observable with some test data
      of<ApiResult<Product>>(<ApiResult<Product>>{
        data: [
          <Product>{
            name: 'TestProduct1',
            id: 1, sellingPrice: 1, costPrice: 1, isActive: true,
            categoryId: 1, categoryName: 'TestCountry1'
          },
          <Product>{
            name: 'TestProduct2',
            id: 2, sellingPrice: 1, costPrice: 1, isActive: true,
            categoryId: 1, categoryName: 'TestCountry1'
          },
          <Product>{
            name: 'TestProduct3',
            id: 3, sellingPrice: 1, costPrice: 1, isActive: true,
            categoryId: 1, categoryName: 'TestCountry1'
          }
        ],
        totalCount: 3,
        pageIndex: 0,
        pageSize: 10
      }));
    productService.getUnits.and.returnValue(of([]));
    await TestBed.configureTestingModule({
      declarations: [ProductListComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        RouterTestingModule
      ],
      providers: [
        { provide: CategoryService, useValue: { getCategories: () => of({ data: [] }) } },
        { provide: AuthService, useValue: { redirectToLoginIfNotAuthenticated: () => false } },
        {
          provide: ProductService,
          useValue: productService
        }
      ]
    })
      .compileComponents();
  });
  beforeEach(() => {
    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should display a "Products" title', () => {
    let title = fixture.nativeElement
      .querySelector('h1');
    expect(title.textContent).toEqual('Products');
  });

  it('should contain a table with a list of one or more Products', () => {
    let table = fixture.nativeElement
      .querySelector('table.mat-mdc-table');
    let tableRows = table
      .querySelectorAll('tr.mat-mdc-row');
    expect(tableRows.length).toBeGreaterThan(0);
  });
});
