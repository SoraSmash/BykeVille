import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from '../authentication/auth.service';
import { CustomershttpService } from './customershttp.service';
import { SalesorderhttpService } from './salesorderhttp.service';
import { SalesOrderDetail } from '../models/salesorderdetail';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  
  private totalProductsSubject = new BehaviorSubject<number>(0);
  
  totalProducts$ = this.totalProductsSubject.asObservable();

  constructor(
    private authentication: AuthService,
    private customerHttpService: CustomershttpService,
    private salesOrderHttpService: SalesorderhttpService
  ) {}


  // Metodo per ottenere il numero di prodotti nel carrello (observable)
  getTotalProducts() {
    return this.totalProductsSubject.asObservable();
  }

  // Metodo per aggiornare il numero di prodotti nel carrello
  updateTotalProducts(total: number) {
    this.totalProductsSubject.next(total);
  }

  // Metodo per azzerare il carrello (ad esempio dopo il logout)
  resetCart() {
    this.totalProductsSubject.next(0);
  }

  //Metodo per caricare i contenuti del carrello 
  loadCartData(): void {
    const email = this.authentication.getEmailFromToken();

    if (email && this.authentication.GetLoginInfo()) {
      this.customerHttpService.getUserByEmail(email).subscribe(
        (response) => {
          const user = response;
          if (user) {
            this.salesOrderHttpService.getSalesOrderHeaderByCustomerId(user.customerOldId).subscribe(
              (response) => {
                if (response && response.length > 0) {
                  const salesOrderHeader = response;
                  const salesOrderDetails = salesOrderHeader[0].salesOrderDetails;


                  // Calcola il numero totale di prodotti nel carrello
                  const totalProducts = salesOrderDetails.reduce(
                    (total: number, item: SalesOrderDetail) => total + item.orderQty,
                    0
                  );
  
                  // Aggiorna il numero totale di prodotti
                  this.updateTotalProducts(totalProducts);


                }
              },
              (error) => console.error('Errore nella ricerca degli ordini:', error)
            );
          }
        },
        (error) => console.error('Errore nel recupero dell\'utente:', error)
      );
    }
  }

}
