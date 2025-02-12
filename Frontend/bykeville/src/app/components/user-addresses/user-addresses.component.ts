import { Component } from '@angular/core';
import { CustomershttpService } from '../../shared/httpservices/customershttp.service';
import { AuthService } from '../../shared/authentication/auth.service';
import { NewCustomer } from '../../shared/models/newcustomer';
import { NewAddress } from '../../shared/models/newaddress';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SalesOrderHeader } from '../../shared/models/salesorderheader';
import { SalesorderhttpService } from '../../shared/httpservices/salesorderhttp.service';
import { SalesOrderDetail } from '../../shared/models/salesorderdetail';
import { Product } from '../../shared/models/product';
import { Router } from '@angular/router';
import { LogshttpService } from '../../shared/httpservices/logshttp.service';

@Component({
  selector: 'app-user-addresses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-addresses.component.html',
  styleUrl: './user-addresses.component.css'
})
export class UserAddressesComponent {
  user: NewCustomer | null = null;
    addresses: NewAddress[] = [];
    selectedAddress: NewAddress | null = null;
    errorMessage: string = ''; 
  
    newAddress: NewAddress = {
      addressId: 0,
      addressLine1: '',
      addressLine2: '',
      city: '',
      stateProvince: '',
      countryRegion: '',
      postalCode: '',
      rowguid: 'AF18BC29-F378-4512-A59B-D55217F4F82B',
      modifiedDate: '2010-10-10',
      addressOldId: 0,
      newCustomerAddresses: []
    };

    constructor(private customerHttpService: CustomershttpService, private authentication: AuthService, private salesOrderHttpService: SalesorderhttpService, private router: Router, private logHttpService: LogshttpService) { }

    ngOnInit(): void {
      //Ottengo l'e-mail dal token
      var email = this.authentication.getEmailFromToken();
  
      if (email) {
        //Ottengo l'utente in base alla sua e-mail
        this.customerHttpService.getUserByEmail(email).subscribe(
          (response) => {
            this.user = response;
            if (this.user && email) {
              //Ottengo gli indirizzi in base all'e-mail dell'utente
              this.customerHttpService.getAddressesByEmail(email).subscribe(
                (response) => {
                  this.addresses = response;
                },
                (error) => {
                  this.logHttpService.postLog(error.message).subscribe();
                }
              );
            }
          },
          (error) => {
            this.logHttpService.postLog(error.message).subscribe();
          }
        );
      }
    }

    //Aggiunge un nuovo indirizzo
    addNewAddress(): void {
      if (this.user) {
        //Aggiungo il nuovo indirizzo
        this.customerHttpService.postNewAddress(this.user.customerOldId, this.newAddress).subscribe(
          (response) => {
            var email = this.authentication.getEmailFromToken();
            if (this.user && email) {
              //Ricarico gli indirizzi
              this.customerHttpService.getAddressesByEmail(email).subscribe(
                (response) => {
                  this.addresses = response;
                },
                (error) => {
                  this.logHttpService.postLog(error.message).subscribe();
                }
              );
            }
            //Resetto i campi per aggiungere un nuovo indirizzo
            this.newAddress = {
              addressId: 0,
              addressLine1: '',
              addressLine2: '',
              city: '',
              stateProvince: '',
              countryRegion: '',
              postalCode: '',
              rowguid: 'AF18BC29-F378-4512-A59B-D55217F4F82B',
              modifiedDate: '2010-10-10',
              addressOldId: 0,
              newCustomerAddresses: []
            };
          },
          (error) => {
            this.logHttpService.postLog(error.message).subscribe();
          }
        );
      }
    }

    //Elimina un indirizzo
    deleteNewAddress(addressOldId: number): void {
      if (this.user) {
        //Elimino l'indirizzo in base al suo OldId
        this.customerHttpService.deleteNewAddress(addressOldId).subscribe(
          (response) => {
            var email = this.authentication.getEmailFromToken();
            if (this.user && email) {
              //Ricarico gli indirizzi
              this.customerHttpService.getAddressesByEmail(email).subscribe(
                (response) => {
                  this.addresses = response;
                  this.selectedAddress = null;
                },
                (error) => {
                  this.logHttpService.postLog(error.message).subscribe();
                }
              );
            }
            this.newAddress = {
              addressId: 0,
              addressLine1: '',
              addressLine2: '',
              city: '',
              stateProvince: '',
              countryRegion: '',
              postalCode: '',
              rowguid: 'AF18BC29-F378-4512-A59B-D55217F4F82B',
              modifiedDate: '2010-10-10',
              addressOldId: 0,
              newCustomerAddresses: []
            };
          },
          (error) => {
            this.logHttpService.postLog(error.message).subscribe();
          }
        );
      }
    }
}
