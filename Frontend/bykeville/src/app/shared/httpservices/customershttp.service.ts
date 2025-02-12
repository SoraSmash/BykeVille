import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../authentication/auth.service';
import { NewAddress } from '../models/newaddress';

@Injectable({
  providedIn: 'root'
})
export class CustomershttpService {

  constructor(private http: HttpClient, private auth: AuthService) { }

  getUserByEmail(email: string): Observable<any> {
    return this.http.get(`https://localhost:7117/NewCustomers/UserByEmail?emailAddress=${email}`,
      {headers: this.auth.authenticationJwtHeader});
  }

  getAddressesByEmail(email: string): Observable<any> {
    return this.http.get(`https://localhost:7117/NewCustomers/AddressesByEmail?emailAddress=${email}`,
      {headers: this.auth.authenticationJwtHeader});
  }

  postNewAddress(customerId: number, newAddress: NewAddress): Observable<any> {
    return this.http.post(`https://localhost:7117/NewCustomers/AddAddress/${customerId}`, newAddress, {
      headers: this.auth.authenticationJwtHeader,
      observe: 'response'
    });
  }

  deleteNewAddress(addressOldId: number): Observable<any> {
    return this.http.delete(`https://localhost:7117/NewCustomers/DeleteAddress/${addressOldId}`, {
      headers: this.auth.authenticationJwtHeader,
      observe: 'response'
    });
  }
}
