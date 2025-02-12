import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterModule, FormsModule, CommonModule],
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent {

  onSubscribe(newsletterForm: NgForm): void {
    if (newsletterForm.valid) {
      // Se il form è valido, resetta il form e mostra un messaggio di conferma
      newsletterForm.reset();
      alert('You have successfully subscribed to the newsletter!');
    } else {
      // Se il form non è valido, mostra un messaggio di errore
      alert('Please enter a valid email address.');
    }
  }
}
