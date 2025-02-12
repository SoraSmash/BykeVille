import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductshttpService } from '../../shared/httpservices/productshttp.service';
import { Product } from '../../shared/models/product';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../shared/authentication/auth.service';
import { CustomershttpService } from '../../shared/httpservices/customershttp.service';
import { NewCustomer } from '../../shared/models/newcustomer';
import { SalesOrderHeader } from '../../shared/models/salesorderheader';
import { SalesorderhttpService } from '../../shared/httpservices/salesorderhttp.service';
import { SalesOrderDetail } from '../../shared/models/salesorderdetail';
import { RouterModule } from '@angular/router';
import { CartService } from '../../shared/httpservices/cart.service';
import { LogshttpService } from '../../shared/httpservices/logshttp.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit {
  products: Product[] = [];
  salesOrderHeader: SalesOrderHeader[] = [];
  salesOrderDetails: SalesOrderDetail[] = [];
  user: NewCustomer | null = null;
  totalProducts: number = 0;

  constructor(
    private authentication: AuthService,
    private salesOrderHttpService: SalesorderhttpService,
    private customerHttpService: CustomershttpService,
    private productHttpService: ProductshttpService,
    private cartService: CartService,
    private logHttpService: LogshttpService
  ) { }

  ngOnInit(): void {
    //Carica i prodotti del carrello
    this.loadCartData();
    //Aggiorna il counter del carrello sulla navbar
    this.cartService.loadCartData();
  }

  //Carica i dati per il carrello
  loadCartData(): void {
    const email = this.authentication.getEmailFromToken();

    if (email && this.authentication.GetLoginInfo()) {
      this.customerHttpService.getUserByEmail(email).subscribe(
        (response) => {
          this.user = response;
          if (this.user) {
            this.salesOrderHttpService.getSalesOrderHeaderByCustomerId(this.user.customerOldId).subscribe(
              (response) => {
                if (response && response.length > 0) {
                  this.salesOrderHeader = response;
                  this.salesOrderDetails = this.salesOrderHeader[0].salesOrderDetails;
                  this.updateCartDetails(); // Chiamata per aggiornare il carrello
                } else {
                  this.resetCart();
                }
              },
              (error) => {
                this.logHttpService.postLog(error.message).subscribe();
                this.resetCart();
              }
            );
          }
        },
        (error) => {
          this.logHttpService.postLog(error.message).subscribe();
          this.resetCart();
        }
      );
    } else {
      this.resetCart();
    }
  }

  // Funzione per ricalcolare i dettagli del carrello
  updateCartDetails(): void {
    this.products = []; // Azzeriamo la lista dei prodotti per evitare duplicati
    this.totalProducts = 0; // Resettiamo il totale

    try {
      this.salesOrderDetails.forEach((detail) => {
        if (detail.product) {
          this.products.push(detail.product);
          this.totalProducts += detail.orderQty;
        }
      });
      this.cartService.updateTotalProducts(this.totalProducts); // Aggiorniamo il carrello nel servizio
    }
    catch (error: unknown) {
      if (error instanceof Error) {
        this.logHttpService.postLog(error.message).subscribe();
      } else {
        this.logHttpService.postLog('An unknown error occurred').subscribe();
      }
    }
  }

  // Funzione per azzerare il carrello quando non ci sono ordini
  resetCart(): void {
    this.products = [];
    this.salesOrderDetails = [];
    this.totalProducts = 0;
    this.cartService.updateTotalProducts(this.totalProducts); // Azzeriamo il totale anche nel servizio
  }

  // Funzione per rimuovere un prodotto dal carrello
  removeProduct(productToRemove: Product): void {
    const indexOfProductToRemove = this.products.indexOf(productToRemove);

    if (indexOfProductToRemove > -1) {
      this.salesOrderHttpService.deleteSalesOrderDetail(this.salesOrderDetails[indexOfProductToRemove].salesOrderDetailId).subscribe(
        (response) => {
          if (response) {
            this.loadCartData(); // Ricarichiamo i dati del carrello dopo la rimozione
          }
        },
        (error) => {
          this.logHttpService.postLog(error.message).subscribe();
        }
      );
    }
  }

  // Funzione per aggiornare la quantità di un prodotto nel carrello
  updateQuantity(index: number, addremoveQty: number): void {
    this.salesOrderDetails[index].orderQty += addremoveQty;

    this.salesOrderHttpService.putSalesOrderDetail(this.salesOrderDetails[index].salesOrderDetailId, this.salesOrderDetails[index]).subscribe(
      (response) => {
        if (response) {
          this.loadCartData(); // Ricarichiamo i dati del carrello dopo l'aggiornamento
        }
      },
      (error) => {
        this.logHttpService.postLog(error.message).subscribe();
      }
    );
  }
}
