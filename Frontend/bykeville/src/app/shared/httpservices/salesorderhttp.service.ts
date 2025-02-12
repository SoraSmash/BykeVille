import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../authentication/auth.service';
import { SalesOrderHeader } from '../models/salesorderheader';
import { SalesOrderDetail } from '../models/salesorderdetail';

@Injectable({
  providedIn: 'root'
})
export class SalesorderhttpService {

  constructor(private http: HttpClient, private auth: AuthService) { }

  getSalesOrderHeaderByCustomerId(customerId: number): Observable<any> {
    return this.http.get(`https://localhost:7117/SalesOrderHeaders/ByCustomerID/${customerId}`,
    {headers: this.auth.authenticationJwtHeader});
  }

  getAllSalesOrderHeaderByCustomerId(customerId: number): Observable<any> {
    return this.http.get(`https://localhost:7117/SalesOrderHeaders/AllByCustomerID/${customerId}`,
      {headers: this.auth.authenticationJwtHeader});
  }

  postSalesOrderHeader(salesOrderHeader: SalesOrderHeader): Observable<any> {
    return this.http.post('https://localhost:7117/SalesOrderHeaders', salesOrderHeader, {
      headers: this.auth.authenticationJwtHeader,
      observe: 'response'
    });
  }

  postSalesOrderDetail(salesOrderDetail: SalesOrderDetail): Observable<any> {
    return this.http.post('https://localhost:7117/SalesOrderHeaders/SalesOrderDetail', salesOrderDetail, {
      headers: this.auth.authenticationJwtHeader,
      observe: 'response'
    });
  }

  deleteSalesOrderDetail(salesOrderDetailId: number): Observable<any> {
    return this.http.delete(`https://localhost:7117/SalesOrderHeaders/SalesOrderDetail?id=${salesOrderDetailId}`, {
      headers: this.auth.authenticationJwtHeader,
      observe: 'response'
    });
  }

  putSalesOrderDetail(salesOrderDetailId: number, salesOrderDetail: SalesOrderDetail): Observable<any> {
    return this.http.put(`https://localhost:7117/SalesOrderHeaders/SalesOrderDetail/${salesOrderDetailId}`, salesOrderDetail, {
      headers: this.auth.authenticationJwtHeader,
      observe: 'response'
    });
  }

  putSalesOrderHeader(salesOrderHeaderId: number, salesOrderHeader: SalesOrderHeader): Observable<any> {
    return this.http.put(`https://localhost:7117/SalesOrderHeaders/${salesOrderHeaderId}`, salesOrderHeader, {
      headers: this.auth.authenticationJwtHeader,
      observe: 'response'
    });
  }
}
