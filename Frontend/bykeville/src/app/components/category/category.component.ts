import { Component, OnInit } from '@angular/core';
import { CategorieshttpService, ProductCategory } from '../../shared/httpservices/categorieshttp.service';
import { CommonModule } from '@angular/common';
import { LogshttpService } from '../../shared/httpservices/logshttp.service';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})

export class CategoriesComponent implements OnInit {

  //Categoria selezionata
  selectedCategory: ProductCategory | null = null;

  //Lista categorie principali
  categories: ProductCategory[] = [];

  constructor(private categoryService: CategorieshttpService, private logHttpService: LogshttpService) { }


  ngOnInit() {

    //Sottoscrizione per ottenere le categorie dal service:
    this.categoryService.categories$.subscribe(categories => {

      //aggiorna la lista categorie di questo componente
      this.categories = categories;
    },
      (error) => {
        this.logHttpService.postLog(error.message).subscribe();
      }
    );

    //sottoscrizione per la categoria selezionata
    this.categoryService.selectedCategory$.subscribe(category => {
      this.selectedCategory = category;
    },
      (error) => {
        this.logHttpService.postLog(error.message).subscribe();
      }
    );
  }

  // Metodo per selezionare una categoria
  onCategorySelected(category: ProductCategory): void {
    this.categoryService.setCategory(category);
  }

}
