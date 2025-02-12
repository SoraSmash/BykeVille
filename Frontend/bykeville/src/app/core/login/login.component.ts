import { Component } from '@angular/core';
import { AuthService } from '../../shared/authentication/auth.service';
import { HttpStatusCode } from '@angular/common/http';
import * as jwt_decode from 'jwt-decode';
import { Router } from '@angular/router';


//username -> email

@Component({
  selector: 'app-loginjwt',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  constructor(private authentication: AuthService, private router: Router){}
  loginCredentials: Credentials = new Credentials();
  jwtToken: any;
  decodedTokenPayload: any;

  RunLogin(usr: HTMLInputElement, pwd: HTMLInputElement){
    if(usr.value && pwd.value){
      this.loginCredentials.Email = usr.value;
      this.loginCredentials.Password = pwd.value;

      this.authentication.LoginPost(this.loginCredentials).subscribe({
        next: (response : any) => {
          switch(response.status){
            case HttpStatusCode.Ok:
              this.jwtToken = response.body.token;
              this.authentication.SetLoginInfo(true, this.jwtToken);
              this.decodedTokenPayload = jwt_decode.jwtDecode(this.jwtToken);
              this.router.navigate(['/productnews']);
              break;
            case HttpStatusCode.NoContent:
              break;
          }
          
        },
        error: (err) => {
          switch(err.status){
            case HttpStatusCode.Unauthorized:
              this.authentication.SetLoginInfo(false, this.jwtToken);
              alert('Email o Password errati');
              break;
          }
        }
      });
    } else{
      alert('Email e Password sono campi obbligatori');
    }

  }

  goToSignUp() {
    this.router.navigate(['/signup']);
  }


}

export class Credentials{
  Email: string;
  Password: string;
  constructor(){
    this.Email = '';
    this.Password = '';
  }
}