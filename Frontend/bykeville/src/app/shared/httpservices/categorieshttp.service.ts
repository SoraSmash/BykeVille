import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';


export interface ProductCategory {
  productCategoryId: number;
  parentProductCategoryId: number | null;
  name: string;
  inverseParentProductCategory: ProductCategory[]; 
}

@Injectable({
  providedIn: 'root',
})

export class CategorieshttpService {
  
  //url della chiamata al backend
  private apiUrl = 'https://localhost:7117/Products/Categories'

  //per la categoria selezionata
  private selectedCategory = new BehaviorSubject<ProductCategory | null>(null);
  selectedCategory$ = this.selectedCategory.asObservable();

  //per la lista delle categorie in generale
  private categories = new BehaviorSubject<ProductCategory[]>([]);
  categories$ = this.categories.asObservable();

  //per caricare le categorie all'inizializzazione
  constructor(private http: HttpClient) {
    this.loadCategories();
  }

  //imposta la categoria selezionata
  setCategory(category: ProductCategory | null): void {
    this.selectedCategory.next(category);
  }
  
  //carica le categorie dal backend 
  private loadCategories(): void {
    this.http.get<ProductCategory[]>(this.apiUrl).subscribe(categories => {

      //Popola la lista delle categorie
      this.categories.next(categories);  
    });
  }


}


