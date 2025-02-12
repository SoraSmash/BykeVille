import { Component } from '@angular/core';
import { CustomershttpService } from '../../shared/httpservices/customershttp.service';
import { AuthService } from '../../shared/authentication/auth.service';
import { NewCustomer } from '../../shared/models/newcustomer';
import { SalesOrderHeader } from '../../shared/models/salesorderheader';
import { SalesOrderDetail } from '../../shared/models/salesorderdetail';
import { Product } from '../../shared/models/product';
import { SalesorderhttpService } from '../../shared/httpservices/salesorderhttp.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LogshttpService } from '../../shared/httpservices/logshttp.service';

@Component({
  selector: 'app-user-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-orders.component.html',
  styleUrls: ['./user-orders.component.css']
})
export class UserOrdersComponent {
  user: NewCustomer | null = null;
  products: Product[] = [];
  salesOrderHeaders: SalesOrderHeader[] = [];
  salesOrderDetails: SalesOrderDetail[] = [];

  constructor(
    private customerHttpService: CustomershttpService,
    private authentication: AuthService,
    private salesOrderHttpService: SalesorderhttpService,
    private logHttpService: LogshttpService
  ) {}

  ngOnInit(): void {
    const email = this.authentication.getEmailFromToken();

    if (email) {
      this.customerHttpService.getUserByEmail(email).subscribe(
        (response) => {
          this.user = response;
          if (this.user) {
            this.salesOrderHttpService.getAllSalesOrderHeaderByCustomerId(this.user.customerOldId).subscribe(
              (response) => {
                if (response != null) {
                  this.salesOrderHeaders = response;
                  this.salesOrderHeaders.forEach(header => {
                    header.salesOrderDetails.forEach(detail => {
                      this.salesOrderDetails.push(detail);
                    });
                  });
                  if (this.salesOrderDetails) {
                    this.salesOrderDetails.forEach((detail) => {
                      if (detail.product != null && detail.product != undefined) {
                        this.products.push(detail.product);
                      }
                    });
                  }
                } else {
                  console.log('Nessun ordine trovato');
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

    // Metodo per ottenere i dettagli dell'ordine per ogni SalesOrderHeader
    getOrderDetailsBySalesOrderId(salesOrderId: number): SalesOrderDetail[] {
      return this.salesOrderDetails.filter(detail => detail.salesOrderId === salesOrderId);
    }
  
    // Metodo per ottenere il prodotto per ogni SalesOrderDetail
    getProductForDetail(detail: SalesOrderDetail): Product | undefined {
      const productIndex = this.salesOrderDetails.indexOf(detail);
      return this.products[productIndex];
    }
}
