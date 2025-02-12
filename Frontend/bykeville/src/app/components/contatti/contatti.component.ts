import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-contatti',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './contatti.component.html',
  styleUrl: './contatti.component.css'
})
export class ContattiComponent {
  contact = {
    name: '',
    email: '',
    message: ''
  };

  onSubmit(form: any) {
    form.reset();
  }
}
