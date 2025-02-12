import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Credentials } from '../../core/login/login.component';
import { Observable } from 'rxjs';
import { Customer } from '../models/customer';
import * as jwt_decode from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private isLogged = false;

  authenticationBasicHeader = new HttpHeaders({
    'Content-Type': 'application/json',
    responseType: 'Text',

  })

  authenticationJwtHeader = new HttpHeaders({
    'Content-Type': 'application/json',
    responseType: 'Text',

  })

  constructor(private http: HttpClient) { this.autoLogin(); }

  LoginPost(credentials: Credentials): Observable<any> {
    return this.http.post('https://localhost:7117/Login/CheckCredentials', credentials,
      { observe: 'response' }
    );
  }

  SignUpPost(signUpData: Customer): Observable<any> {
    return this.http.post('https://localhost:7117/SignIn', signUpData, {
      observe: 'response',
      headers: { 'Content-Type': 'application/json-patch+json' },
    });
  }

  SetLoginInfo(isLogged: boolean, jwtToken: string = '') {
    if (isLogged) {
      localStorage.setItem('jwtToken', jwtToken);
      this.authenticationJwtHeader = this.authenticationJwtHeader.set(
        'Authorization',
        'Bearer ' + jwtToken
      );
    } else {
      localStorage.removeItem('jwtToken');
      this.authenticationJwtHeader = new HttpHeaders({
        'Content-Type': 'application/json',
        responseType: 'text',
      });
    }

    this.isLogged = isLogged;
  }

  GetLoginInfo(): boolean {
    return this.isLogged;
  }

  getEmailFromToken(): string | null {
    const token = localStorage.getItem('jwtToken'); // O il tuo metodo per ottenere il token

    if (!token) {
      return null;
    }

    try {
      const decodedToken: any = jwt_decode.jwtDecode(token);
      return decodedToken.unique_name; // Supponiamo che l'email sia nel claim 'email'
    } catch (error) {
      console.error('Token non valido o errore nella decodifica', error);
      return null;
    }
  }

  autoLogin(): void{
    const token = localStorage.getItem('jwtToken');
    if (token){
      try{
          const decodedToken:any =  jwt_decode.jwtDecode(token);

          const isTokenValid= decodedToken && new Date(decodedToken.exp * 1000) > new Date();

          if (isTokenValid){
            this.isLogged = true;
            this.authenticationJwtHeader = this.authenticationJwtHeader.set(
              'Authorization',
              'Bearer '+ token
            );
          } else {
            this.logout();
          }
      }
      catch (error) {
      console.error('Errore durante la verifica del token ', error);
      this.logout();
      }
    } else {
      this.isLogged=false;
    }
  }

  logout(){
    localStorage.removeItem('jwtToken');
    this.isLogged=false;
    this.authenticationJwtHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      responseType: 'Text',
    });
  }

  getPasswordFromToken(): string | null {
    const token = localStorage.getItem('jwtToken'); 
    
  
    if (!token) {
      return null;
    }
  
    try {
      const decodedToken: any = jwt_decode.jwtDecode(token);
      return decodedToken.password; 
    } catch (error) {
      console.error('Token non valido o errore nella decodifica', error);
      return null;
    }
  }

  checkPassword(credentials: Credentials): Observable<any> {
    return this.http.post('https://localhost:7117/Login/CheckPassword', credentials,
      { observe: 'response' }
    );
  }
}
