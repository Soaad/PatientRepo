import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { UserLogin } from '../models/patient.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
 private baseUrl = 'http://localhost:5169/api/Auth';
  private tokenSubject = new BehaviorSubject<string | null>(localStorage.getItem('token'));

  token$ = this.tokenSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(user: UserLogin): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/token`, user).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        this.tokenSubject.next(res.token);
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.tokenSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
