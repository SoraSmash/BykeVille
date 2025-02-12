import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LogshttpService {
  constructor(private http: HttpClient) { }

  postLog(exception: string): Observable<any> {
    return this.http.post(`https://localhost:7117/Logs?exception=${exception}`, {
      observe: 'response'
    });
  }
}
