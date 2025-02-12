import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NewCustomer } from '../models/newcustomer';
import { AuthService } from '../authentication/auth.service';


// export interface Customer {
//   customerId: number;
//   nameStyle: boolean;
//   title?: string;
//   firstName: string;
//   middleName?: string;
//   lastName: string;
//   suffix?: string;
//   companyName?: string;
//   salesPerson?: string;
//   emailAddress: string;
//   phone: string;
//   passwordHash: string;
//   passwordSalt?: string;
//   rowguid?: string;
//   modifiedDate?: string;
//   role?: string;
//   customerOldId?: number;
//   newCustomerAddresses: [];
// }


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:7117/NewCustomers';

  constructor(private http: HttpClient, private auth: AuthService) {}

  getUserByEmail(email: string): Observable<NewCustomer> {
    return this.http.get<NewCustomer>(`${this.apiUrl}/UserByEmail?emailAddress=${encodeURIComponent(email)}`);
  
  }


  updateUser(user: NewCustomer): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${user.customerOldId}`, user);
  }
}
