import { Component, OnInit } from '@angular/core';
import { ProductshttpService } from '../../shared/httpservices/productshttp.service';
import { CommonModule } from '@angular/common';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';
import { Product } from '../../shared/models/product';
import { CartService } from '../../shared/httpservices/cart.service';

@Component({
  selector: 'app-product-news',
  standalone: true,
  imports: [CommonModule, ProductCardComponent],
  templateUrl: './product-news.component.html',
  styleUrl: '../../shared/product-card/product-card.component.css'
})
export class ProductNewsComponent implements OnInit{
  title = 'bykeville';

  newArrivals: Product[] = [];
  mostPurchased: Product[] = [];
  

  constructor(private bikeService: ProductshttpService, private cartService: CartService) {
    this.cartService.loadCartData();
  }


  ngOnInit(): void {
    this.loadNewArrivals();
    this.loadMostPurchased();
  }

  //Carico gli ultim arrivi
  private loadNewArrivals() {
    this.bikeService.getNewArrivals().subscribe(data => {
      this.newArrivals = data;
    });
  }

  //Carico i prodotti più comprati
  private loadMostPurchased() {
    this.bikeService.getMostPurchased().subscribe(data => {
      this.mostPurchased = data;
    });
  }
}