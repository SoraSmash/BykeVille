import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductshttpService } from '../../shared/httpservices/productshttp.service';
import { CommonModule } from '@angular/common';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';
import { LogshttpService } from '../../shared/httpservices/logshttp.service';

//[Componente creato da Matt il 10/12/2024]


@Component({
  selector: 'app-product-by-cat',
  standalone: true,
  imports: [CommonModule, ProductCardComponent],
  templateUrl: './product-by-cat.component.html',
  styleUrl: '../../shared/product-card/product-card.component.css'
})


export class ProductByCatComponent implements OnInit {

  //lista dei prodotti
  products: any[] = [];

  //nome della categoria
  categoryName: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private productsHttpService: ProductshttpService,
    private logHttpService: LogshttpService
  ) { }

  ngOnInit(): void {

    //sottoscrizione ai parametri dell'url per avere il nome della categoria
    this.route.paramMap.subscribe((params) => {
      const encodedCategoryName = params.get('categoryName'); // Ottieni il valore codificato
      this.categoryName = encodedCategoryName ? decodeURIComponent(encodedCategoryName) : null; // Decodifica il valore


      //carica i prodotti
      if (this.categoryName) {
        this.loadProducts(this.categoryName);
      }
    },
      (error) => {
        this.logHttpService.postLog(error.message).subscribe();
      }
    );
  }

  //metodo per caricare i prodotti dal backend
  loadProducts(categoryName: string): void {
    this.productsHttpService.getProductByCategory(categoryName).subscribe(
      (products) => {
        this.products = products;
      },
      (error) => {
        this.logHttpService.postLog(error.message).subscribe();
      }
    );
  }

}
