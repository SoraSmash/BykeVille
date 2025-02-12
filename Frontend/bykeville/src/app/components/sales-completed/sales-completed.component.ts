import { Component } from '@angular/core';
import { CartService } from '../../shared/httpservices/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sales-completed',
  standalone: true,
  imports: [],
  templateUrl: './sales-completed.component.html',
  styleUrl: './sales-completed.component.css'
})
export class SalesCompletedComponent {


  constructor(private cartService: CartService, private router: Router) {
    this.cartService.resetCart();
  }

  goToHome() {
    this.router.navigate(['/productnews']);
  }

}
