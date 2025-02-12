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
import { CartService } from '../../shared/httpservices/cart.service';
import { LogshttpService } from '../../shared/httpservices/logshttp.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-single',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-single.component.html',
  styleUrls: ['./product-single.component.css']
})
export class ProductSingleComponent implements OnInit {
  productId: number = -1;
  product: Product | null = null;
  products: Product[] = [];
  selectedColor: string = '';
  selectedSize: string = '';
  uniqueColors: string[] = [];
  uniqueSizes: string[] = [];
  description: string = '';

  constructor(private route: ActivatedRoute, private productsHttpService: ProductshttpService, private authentication: AuthService, private customerHttpService: CustomershttpService, private salesOrderHttpService: SalesorderhttpService, private cartService: CartService, private logHttpService: LogshttpService, private router: Router) { }

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));

    // Ottiene il prodotto in base al suo id
    this.productsHttpService.getProductById(this.productId).subscribe(
      (response) => {
        this.product = response;

        // Carica i prodotti correlati in base al modello del prodotto
        if (this.product?.productModelId) {
          this.productsHttpService.getProductByModel(this.product?.productModelId).subscribe(
            (response) => {
              this.products = response;
              this.selectProduct(this.products[0]);
              this.extractUniqueColorsAndSizes();
            },
            (error) => {
              this.logHttpService.postLog(error.message).subscribe();
            }
          );

          this.productsHttpService.getProductDescriptionByModel(this.product.productModelId).subscribe(
            (response) => {
              this.description = response;
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

    this.authentication.getEmailFromToken();

  }

  // Estrai colori e dimensioni uniche
  extractUniqueColorsAndSizes(): void {
    const colorsSet = new Set<string>();
    const sizesSet = new Set<string>();

    try {
      this.products.forEach(product => {
        if (product.color) colorsSet.add(product.color);
        if (product.size) sizesSet.add(product.size);
      });

      //Salvo i colori senza ripetizioni nell'array uniqueColors
      this.uniqueColors = Array.from(colorsSet);

      // Ordine predefinito delle taglie alfanumeriche
      const sizeOrder = ['XS', 'S', 'M', 'L', 'XL', 'XXL'];

      // Creo due array per le taglie numeriche e alfanumeriche
      const numericSizes: string[] = [];
      const nonNumericSizes: string[] = [];

      sizesSet.forEach(size => {
        if (!isNaN(Number(size))) {
          numericSizes.push(size); // Taglie numeriche
        } else {
          nonNumericSizes.push(size); // Taglie alfanumeriche
        }
      });

      // Ordiniamo le taglie numeriche in ordine crescente
      numericSizes.sort((a, b) => Number(a) - Number(b));

      // Ordinamento delle taglie alfanumeriche secondo l'ordine definito
      nonNumericSizes.sort((a, b) => {
        const indexA = sizeOrder.indexOf(a);
        const indexB = sizeOrder.indexOf(b);

        // Se entrambi gli elementi sono validi nell'array sizeOrder
        if (indexA !== -1 && indexB !== -1) {
          return indexA - indexB;
        }

        // Se uno dei due non è trovato nell'array sizeOrder, lo mettiamo dopo
        return indexA === -1 ? 1 : -1;
      });

      // Uniamo i due array (numeriche e alfanumeriche)
      this.uniqueSizes = numericSizes.concat(nonNumericSizes);
    }
    catch (error: unknown) {
      if (error instanceof Error) {
        this.logHttpService.postLog(error.message).subscribe();
      } else {
        this.logHttpService.postLog('An unknown error occurred').subscribe();
      }
    }
  }



  // Funzione per selezionare un prodotto in base al colore e alla dimensione
  selectProduct(product: Product): void {
    this.product = product;
    this.selectedColor = product.color || '';
    this.selectedSize = product.size || '';
  }



  // Funzione per gestire il cambiamento di colore
  onColorChange(color: string): void {
    const selectedProduct = this.products.find(p => p.color === color && p.size === this.selectedSize);
    if (selectedProduct) {
      this.selectProduct(selectedProduct);
    }
  }



  // Funzione per gestire il cambiamento di dimensione
  onSizeChange(size: string): void {
    const selectedProduct = this.products.find(p => p.size === size && p.color === this.selectedColor);
    if (selectedProduct) {
      this.selectProduct(selectedProduct);
    }
  }

  //Aggiunge un prodotto nel carrello
  addToCart(): void {
    var salesOrderHeader: SalesOrderHeader[] = [];
    var salesOrderDetail: SalesOrderDetail;
    var indexFound: number = -1;

    //Ottiene la e-mail dal token
    var email = this.authentication.getEmailFromToken();

    if (!email) {
      // Se l'utente non è loggato, reindirizza alla pagina di login
      this.router.navigate(['/login']);
      return; // Esci dalla funzione
    }

    if (email) {
      //Ottiene l'utente in base all'e-mail
      this.customerHttpService.getUserByEmail(email).subscribe(
        (response) => {
          const user: NewCustomer = response;
          if (user) {
            //Ottiene l'ordine attivo dell'utente in base al suo Id
            this.salesOrderHttpService.getSalesOrderHeaderByCustomerId(user.customerOldId).subscribe(
              (response) => {
                if (response != null && this.product != null) {
                  salesOrderHeader = response;
                  //Se esiste un ordine attivo per l'utente
                  if (salesOrderHeader) {
                    const salesOrderDetails: SalesOrderDetail[] = salesOrderHeader[0].salesOrderDetails;
                    salesOrderDetails.forEach(detail => {
                      if (detail.productId == this.product?.productId)
                        indexFound = salesOrderDetails.indexOf(detail);
                    });

                    //se il prodotto non è nel carrello viene aggiunto
                    if (indexFound == -1) {
                      salesOrderDetail = {
                        salesOrderId: salesOrderHeader[0].salesOrderId,
                        salesOrderDetailId: 0,
                        orderQty: 1,
                        productId: this.product.productId,
                        unitPrice: this.product.listPrice,
                        unitPriceDiscount: 0.00,
                        lineTotal: this.product.listPrice * 1,
                        rowguid: "63AC9402-5414-495A-8EC9-DE0FC315070A",
                        modifiedDate: new Date().toISOString(),
                        product: null,
                        salesOrder: null
                      }
                      this.salesOrderHttpService.postSalesOrderDetail(salesOrderDetail).subscribe(
                        (response) => {
                          // Aggiorna i dati del carrello
                          this.cartService.loadCartData();
                        },
                        (error) => {
                          this.logHttpService.postLog(error.message).subscribe();
                        }
                      );
                    } else {
                      //se è già nel carrello aggiorna quantità
                      salesOrderDetails[indexFound].orderQty += 1;
                      this.salesOrderHttpService.putSalesOrderDetail(salesOrderDetails[indexFound].salesOrderDetailId, salesOrderDetails[indexFound]).subscribe(
                        (response) => {
                          // Aggiorna i dati del carrello
                          this.cartService.loadCartData();
                        },
                        (error) => {
                          this.logHttpService.postLog(error.message).subscribe();
                        }
                      );
                    }
                  }
                }
                else {
                  //se non esiste un ordine attivo ne crea uno
                  if (this.product) {
                    var newSalesOrderHeader: SalesOrderHeader = {
                      salesOrderId: 0,
                      revisionNumber: 1,
                      orderDate: new Date().toISOString(),
                      dueDate: new Date(new Date().setDate(new Date().getDate() + 7)).toISOString(),
                      status: 1,
                      onlineOrderFlag: true,
                      salesOrderNumber: 'SO',
                      customerId: user.customerOldId,
                      shipMethod: 'CARGO TRANSPORT 5',
                      subTotal: this.product?.listPrice,
                      taxAmt: 10,
                      freight: 5,
                      totalDue: 115,
                      rowguid: "63AC9402-5414-495A-8EC9-DE0FC315070A",
                      modifiedDate: new Date().toISOString(),
                      customer: null,
                      salesOrderDetails: [],
                      billToAddress: null,
                      shipToAddress: null
                    };
                    this.salesOrderHttpService.postSalesOrderHeader(newSalesOrderHeader).subscribe(
                      (response) => {
                        if (response != null && this.product != null) {
                          newSalesOrderHeader = response.body;
                          if (newSalesOrderHeader) {
                            salesOrderDetail = {
                              salesOrderId: newSalesOrderHeader.salesOrderId,
                              salesOrderDetailId: 0,
                              orderQty: 1,
                              productId: this.product.productId,
                              unitPrice: this.product.listPrice,
                              unitPriceDiscount: 0.00,
                              lineTotal: this.product.listPrice * 1,
                              rowguid: "63AC9402-5414-495A-8EC9-DE0FC315070A",
                              modifiedDate: new Date().toISOString(),
                              product: null,
                              salesOrder: null
                            }
                            this.salesOrderHttpService.postSalesOrderDetail(salesOrderDetail).subscribe(
                              (response) => {
                                // Aggiorna i dati del carrello
                                this.cartService.loadCartData();
                              },
                              (error) => {
                                this.logHttpService.postLog(error.message).subscribe();
                              }
                            );
                          }
                        }
                      },
                      (error) => {
                        this.logHttpService.postLog(error.message).subscribe();
                      }
                    );
                  }
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

}
