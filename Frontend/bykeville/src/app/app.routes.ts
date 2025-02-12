import { Routes } from '@angular/router';
import { LoginComponent } from './core/login/login.component';
import { LogoutComponent } from './core/logout/logout.component';
import { SignUpComponent } from './components/signup/signup.component';
import { ProductSearchedComponent } from './components/product-searched/product-searched.component';
import { ProductNewsComponent } from './components/product-news/product-news.component';
import { ProductCardComponent } from './shared/product-card/product-card.component';
import { ProductSingleComponent } from './components/product-single/product-single.component';
import {ProductByCatComponent} from './components/product-by-cat/product-by-cat.component';
import { CartComponent } from './components/cart/cart.component';
import { SalesDataComponent } from './components/sales-data/sales-data.component';
import { SalesCompletedComponent } from './components/sales-completed/sales-completed.component';
import { AboutUsComponent } from './components/aboutus/aboutus.component';
import { UserInfoComponent } from './components/user-info/user-info.component';
import { AccountComponent } from './components/account/account.component';
import { UserAddressesComponent } from './components/user-addresses/user-addresses.component';
import { UserOrdersComponent } from './components/user-orders/user-orders.component';
import { ContattiComponent } from './components/contatti/contatti.component';
import { GaranziaComponent } from './components/garanzia/garanzia.component';
import { SecondamanoComponent } from './components/secondamano/secondamano.component';
import { AcquistiComponent } from './components/acquisti/acquisti.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'logout', component: LogoutComponent },
    { path: 'signup', component: SignUpComponent},
    { path: 'productsearched', component: ProductSearchedComponent},
    { path: 'productnews', component: ProductNewsComponent},
    { path: 'productcard', component: ProductCardComponent},
    { path: 'productsingle/:id', component: ProductSingleComponent},
    { path: 'products/:categoryName', component: ProductByCatComponent},
    { path: 'cart', component: CartComponent},
    { path: 'salesdata', component: SalesDataComponent},
    { path: 'salescompleted', component: SalesCompletedComponent},
    { path: 'aboutus', component: AboutUsComponent},
    { path: 'user-info', component: UserInfoComponent},
    { path: 'user-addresses', component: UserAddressesComponent},
    { path: 'user-orders', component: UserOrdersComponent},
    { path: 'account', component: AccountComponent},
    { path : 'contatti', component: ContattiComponent},
    { path : 'acquisti', component: AcquistiComponent},
    { path :'garanzia', component: GaranziaComponent},
    { path:'secondamano', component: SecondamanoComponent},
    { path: '', redirectTo: 'productnews', pathMatch: 'full' }
];
