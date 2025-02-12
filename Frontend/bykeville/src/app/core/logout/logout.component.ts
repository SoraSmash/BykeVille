import { Component } from '@angular/core';
import { AuthService } from '../../shared/authentication/auth.service';
import { CartService } from '../../shared/httpservices/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout',
  standalone: true,
  imports: [],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export class LogoutComponent {
  constructor(private auth: AuthService, private cartService: CartService, private router: Router) {
    this.auth.SetLoginInfo(false, "");
    this.cartService.resetCart();
  }

  RunLogout() {
    this.auth.SetLoginInfo(false, "");
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
