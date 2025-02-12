import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductshttpService } from '../../shared/httpservices/productshttp.service';
import { Product } from '../../shared/models/product';
import { CommonModule } from '@angular/common';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';
import { LogshttpService } from '../../shared/httpservices/logshttp.service';

@Component({
  selector: 'app-product-searched',
  standalone: true,
  imports: [CommonModule, ProductCardComponent],
  templateUrl: './product-searched.component.html',
  styleUrl: '../../shared/product-card/product-card.component.css'
})
export class ProductSearchedComponent {

  searchQuery: string = '';

  products: Product[] = [];

  constructor(private route: ActivatedRoute, private productsHttpService: ProductshttpService, private logHttpService: LogshttpService) {
    //Ottengo la parola inserita dall'utente direttamente dall'url
    this.route.queryParams.subscribe(params => {
      this.searchQuery = params['query'];

      //Ottengo i prodotti che contengono la parola inserita dall'utente
      this.productsHttpService.getProductByName(this.searchQuery).subscribe(
        (response) => {
          this.products = response;
        },
        (error) => {
          this.logHttpService.postLog(error.message).subscribe();
        }
      );
    });
  }
}
