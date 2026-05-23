import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthResult } from '../models/user.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'v4h_token';
  private readonly USER_KEY = 'v4h_user';

  readonly currentUser = signal<AuthResult | null>(this.loadUser());

  constructor(private http: HttpClient, private router: Router) {}

  register(name: string, email: string, password: string, role: number) {
    return this.http.post(`${environment.apiUrl}/auth/register`, { name, email, password, role });
  }

  login(email: string, password: string) {
    return this.http.post<AuthResult>(`${environment.apiUrl}/auth/login`, { email, password }).pipe(
      tap(result => {
        localStorage.setItem(this.TOKEN_KEY, result.token);
        localStorage.setItem(this.USER_KEY, JSON.stringify(result));
        this.currentUser.set(result);
      })
    );
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  hasRole(role: string): boolean {
    return this.currentUser()?.role === role;
  }

  private loadUser(): AuthResult | null {
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
