import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../shared/authentication/auth.service';
import { CommonModule } from '@angular/common';
import { CategorieshttpService } from '../../shared/httpservices/categorieshttp.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductCategory } from '../../shared/httpservices/categorieshttp.service';
import { CartComponent } from '../../components/cart/cart.component';
import { CartService } from '../../shared/httpservices/cart.service';
import { NewCustomer } from '../../shared/models/newcustomer';
import { SalesOrderHeader } from '../../shared/models/salesorderheader';
import { SalesOrderDetail } from '../../shared/models/salesorderdetail';
import { Product } from '../../shared/models/product';
import { Subscription } from 'rxjs';




@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit, OnDestroy{

  searchText: string = '';

  //contiene tutte le categorie principali e le sottocategorie
  categories: ProductCategory[] = [];

  //sottocategorie di una categoria
  currentSubCategories: ProductCategory[] = [];

  totalItemsInCart: number = 10;


  products: Product[] = [];
  salesOrderHeader: SalesOrderHeader[] = [];
  salesOrderDetails: SalesOrderDetail[] = [];
  user: NewCustomer | null = null;

  
  @ViewChild(CartComponent) cartComponent!: CartComponent;

  private cartSubscription: Subscription = new Subscription();
  private authSubscription: Subscription = new Subscription();

  constructor(
    public auth: AuthService,
    private categoryService: CategorieshttpService,
    private router: Router,
    private cartService: CartService,
  ) { }

  ngOnInit(): void {

    //sottoscrizione per prendere le categorie dal service
    this.categoryService.categories$.subscribe(categories => {
      this.categories = categories
    });

    this.cartSubscription = this.cartService.getTotalProducts().subscribe(total => {
      this.totalItemsInCart = total; // Aggiorna il totale dei prodotti nel carrello
    });

      this.cartService.loadCartData();

  }

  //ottiene subcategorie
  showSubCategories(category: ProductCategory): void {
    this.currentSubCategories = category.inverseParentProductCategory;
  }

  //nasconde le sottocategorie
  hideCategories() {
    this.currentSubCategories = [];
  }


  //funzione per inviare a productsearched la parola per la ricerca
  onSearch() {
    if (this.searchText.trim()) {
      var searchText = this.searchText.trim();
      this.searchText = "";
      this.router.navigate(['/productsearched'], { queryParams: { query: searchText } });
    }
  }

  // Metodo per codificare l'URL
  encodeUrl(value: string): string {
    return encodeURIComponent(value);
  }

  ngOnDestroy(): void {
    // Annulla la sottoscrizione per evitare memory leak
    this.cartSubscription.unsubscribe();
    //this.authSubscription.unsubscribe();
  }


}
