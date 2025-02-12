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
  selector: 'app-sales-data',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sales-data.component.html',
  styleUrl: './sales-data.component.css'
})
export class SalesDataComponent {

  user: NewCustomer | null = null;
  addresses: NewAddress[] = [];
  paymentMethod: string = 'creditCard';
  selectedAddress: NewAddress | null = null;
  products: Product[] = [];
  salesOrderHeader: SalesOrderHeader[] = [];
  salesOrderDetails: SalesOrderDetail[] = [];
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
    //Ottengo l'e-mail dell'utente dal token
    var email = this.authentication.getEmailFromToken();

    if (email) {
      //Ottengo l'utente in base alla sua e-mail
      this.customerHttpService.getUserByEmail(email).subscribe(
        (response) => {
          this.user = response;
          if (this.user && email) {
            //Ottengo gli indirizzi dell'utente in base alla sua e-mail
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
    if (email) {
      //Ottengo l'utente in base alla sua e-mail
      this.customerHttpService.getUserByEmail(email).subscribe(
        (response) => {
          this.user = response;
          if (this.user) {
            //Ottengo l'ordine attivo dell'utente in base al suo Id
            this.salesOrderHttpService.getSalesOrderHeaderByCustomerId(this.user.customerOldId).subscribe(
              (response) => {
                if (response != null) {
                  this.salesOrderHeader = response;
                  this.salesOrderDetails = this.salesOrderHeader[0].salesOrderDetails;
                  if (this.salesOrderDetails) {
                    this.salesOrderDetails.forEach((detail) => {
                      if (detail.product != null && detail.product != undefined) {
                        this.products.push(detail.product);
                      }
                    });
                  }
                }
                else {
                  console.log("Nessun ordine trovato");
                }
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

  //Aggiunge un nuovo indirizzo per l'utente
  addNewAddress(): void {
    if (this.user) {
      //Aggiunge il nuovo indirizzo
      this.customerHttpService.postNewAddress(this.user.customerOldId, this.newAddress).subscribe(
        (response) => {
          var email = this.authentication.getEmailFromToken();
          if (this.user && email) {
            //Ricarico gli indirizzi in base all'e-mail dell'utente
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

  // Funzione per selezionare un indirizzo
  selectAddress(address: NewAddress): void {
    this.selectedAddress = address;
    this.errorMessage = '';
  }

  // Funzione per eliminare un indirizzo
  deleteNewAddress(addressOldId: number): void {
    if (this.user) {
      //Elimino un indirizzo in base al suo OldId
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

  // Funzione per confermare l'ordine
  confirmOrder() {
    if (this.selectedAddress != null) {
      this.salesOrderHeader[0].shipDate = new Date().toISOString().slice(0, 19).replace("T", " ") + ".000";
      this.salesOrderHeader[0].status = 5;
      this.salesOrderHeader[0].onlineOrderFlag = false;
      this.salesOrderHeader[0].shipToAddressId = this.selectedAddress?.addressOldId;
      this.salesOrderHeader[0].billToAddressId = this.selectedAddress?.addressOldId;
      //Aggiorno il SalesOrderHeader perché l'ordine è stato completato
      this.salesOrderHttpService.putSalesOrderHeader(this.salesOrderHeader[0].salesOrderId, this.salesOrderHeader[0]).subscribe(
        (response) => {
          this.router.navigate(['/salescompleted']);
        },
        (error) => {
          this.logHttpService.postLog(error.message).subscribe();
        }
      );
    }
    else {
      this.errorMessage = 'Please select a shipping address';
    }
  }
}
