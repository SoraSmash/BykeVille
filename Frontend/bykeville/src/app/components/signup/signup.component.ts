import { Component } from '@angular/core';
import { FormBuilder, FormGroup, PatternValidator, Validators } from '@angular/forms';
import { AuthService } from '../../shared/authentication/auth.service';
import { Customer } from '../../shared/models/customer';
import { ReactiveFormsModule } from '@angular/forms';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Router } from '@angular/router';


import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-signup-component',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css'],
})
export class SignUpComponent {
  signUpForm: FormGroup;

  titles: string[] = ['Mr', 'Mrs', 'Ms'];
  suffixes: string[] = ['Jr.', 'Sr.'];

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.signUpForm = this.fb.group(
      {
        firstName: ['', Validators.required],
        MiddleName: [''],
        lastName: ['', Validators.required],
        emailAddress: ['', [Validators.required, Validators.email]],
        passwordHash: ['', Validators.required],
        CompanyName: ['', Validators.required],
        phone: ['', Validators.required],
        confirmPassword: ['', Validators.required],
        termsAccepted: [false, Validators.requiredTrue],
        title: ['', Validators.required],
        suffix: ['', Validators.required],
      },
      { validators: PasswordMatchValidator() }
    );
  }

  onSubmit() {

    // Controlla se la checkbox "termsAccepted" è selezionata
    if (!this.signUpForm.get('termsAccepted')?.value) {
      alert('⚠️ You must accept the Terms and Conditions to proceed!');
      return; // Blocca l'esecuzione del resto del metodo
    }

    if (this.signUpForm.valid) {

      const formValue = this.signUpForm.value;
      // Prendo i valori del formValue e inserisco confirmPassword e termsAccepted nelle rispettive variabili
      // mentre tutti gli altri valori verranno inseriti in filteredValues
      const { confirmPassword, termsAccepted, ...filteredValues } = formValue;

      const signUpData: Customer = {
        customerId: -1,
        ...filteredValues,
        salesPerson: "",
        passwordSalt: "wdwdwdw",
        rowguid: "B9EDE243-A6F4-4629-B1D4-FFE1AEDC6DE7",
        modifiedDate: "2024-11-29",
        salesOrderHeaders: [],
        nameStyle: true,
        customerAddresses: [],
      };

      //Registro l'utente
      this.authService.SignUpPost(signUpData).subscribe({
        next: (response) => {
          alert('Registration successful!');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          alert('Registration failed. Please try again.');
        },
      });
    } else {
      alert('Please fill all required fields correctly.');
    }
  }
}

//Confronta le due nuove password inserite
export function PasswordMatchValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const password = control.get('passwordHash');
    const confirmPassword = control.get('confirmPassword');

    return password && confirmPassword && password.value !== confirmPassword.value
      ? { passwordsMismatch: true }
      : null;
  };
}