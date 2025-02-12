import { Component } from '@angular/core';
import { Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css'
})
export class ProductCardComponent {

  constructor (private router: Router){}

  @Input() name: string = '';         // Nome del prodotto
  @Input() thumbNailPhoto: string  | undefined;; // Foto del prodotto (base64)
  @Input() listPrice: number = 0;
  @Input() productId: number = -1;

  goToProductDetails(productId: number) {
    this.router.navigate(['productsingle', productId]);
  }
}
