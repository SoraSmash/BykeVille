import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../authentication/auth.service';

@Injectable({
  providedIn: 'root'
})
export class ProductshttpService {

  constructor(private http: HttpClient, private auth: AuthService) { }

  private apiUrl = 'https://localhost:7117/Products'

  getProduct(): Observable<any> {
    return this.http.get(`${this.apiUrl}`);
  }

  getProductByName(name: string): Observable<any> {
   return this.http.get( `${this.apiUrl}/ByName?name=${name}`);
  }
  
  getProductByCategory(category: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/ByCategories?categoryName=${category}`);
  }

  getNewArrivals(): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:7117/Products/NewArrival`);
  }

  getMostPurchased(): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:7117/Products/MostPurchased`);
  }

  getProductById(id: number): Observable<any> {
    return this.http.get(`https://localhost:7117/Products/${id}`);
  }

  getProductByModel(modelId: number): Observable<any> {
    return this.http.get(`https://localhost:7117/Products/Models?productModelId=${modelId}`);
  }

  getProductDescriptionByModel(modelId: number): Observable<string> {
    return this.http.get<string>(`${this.apiUrl}/DescriptionByProductModelID?productModelId=${modelId}`, { responseType: 'text' as 'json' });
  }

  
}
