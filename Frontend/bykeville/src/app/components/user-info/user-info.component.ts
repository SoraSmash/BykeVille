import { Component, OnInit } from '@angular/core';
import { UserService } from '../../shared/httpservices/user-service.service';
import { ChangeDetectorRef } from '@angular/core';
import { AuthService } from '../../shared/authentication/auth.service';
import { trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NewCustomer } from '../../shared/models/newcustomer';
import { LogshttpService } from '../../shared/httpservices/logshttp.service';
import { Credentials } from '../../core/login/login.component';




@Component({
  selector: 'app-user-info',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css'],
})
export class UserInfoComponent implements OnInit {
  users: NewCustomer[] = [];
  email: string | null = '';
  originalUsers: NewCustomer[] = [];

  constructor(
    private userService: UserService,
    private cdr: ChangeDetectorRef,
    private authService: AuthService,
    private logHttpService: LogshttpService
  ) { }


  isOpen: boolean = false;

  currentPassword: string = '';
  newPassword: string = '';
  confirmPassword: string = '';

  openPopup() {
    const password = this.authService.getPasswordFromToken();
    console.log("ciao", password)
    if (password) {
      console.log("ciao", password)
      this.currentPassword = password;
    }
    this.isOpen = true;
  }

  closePopup() {
    this.isOpen = false;
  }

  onSubmit() {

    var isChecked = false;
    this.email = this.authService.getEmailFromToken();

    if (this.email != null) {
      const credentials = new Credentials();
      credentials.Email = this.email;
      credentials.Password = this.currentPassword;

      this.authService.checkPassword(credentials).subscribe({
        next: (data) => {
          isChecked = data.body;

          if (!isChecked) {
            alert("Current password is incorrect!");
            return;
          }

          if (this.newPassword !== this.confirmPassword) {
            alert("New passwords do not match!");
            return;
          }

          this.users[0].passwordHash = this.newPassword;
          this.users[0].newCustomerAddresses = [];
          
          this.userService.updateUser(this.users[0]).subscribe({
            next: () => {
              alert('Changes saved successfully!');
            },
            error: (error) => {
              this.logHttpService.postLog(error.message).subscribe();
            },
          });
          this.closePopup();
        },
        error: (error) => {
          this.logHttpService.postLog(error.message).subscribe();
        },
      });
    }
  }




  ngOnInit(): void {
    this.email = this.authService.getEmailFromToken();
    if (this.email) {
      this.loadUserData(this.email);
    } else {
      console.error('Email not found. The user might not be logged in.');
    }
  }

  loadUserData(email: string): void {
    this.userService.getUserByEmail(email).subscribe({
      next: (data) => {
        this.users = [data];
        this.originalUsers = JSON.parse(JSON.stringify(this.users));
      },
      error: (error) => {
        this.logHttpService.postLog(error.message).subscribe();
      },
    });
  }

  saveChanges(user: NewCustomer): void {
    user.newCustomerAddresses = [];
    user.passwordHash = "";
    this.userService.updateUser(user).subscribe({
      next: () => {
        alert('Changes saved successfully!');
      },
      error: (error) => {
        this.logHttpService.postLog(error.message).subscribe();
      },
    });
  }

  cancelChanges(): void {
    this.users = JSON.parse(JSON.stringify(this.originalUsers));
    alert('Changes canceled.');
  }


}