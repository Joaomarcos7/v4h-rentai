# Teleconsultoria — Plan 2: Frontend (Angular 17)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the Angular 17 frontend — auth (login/register), teleconsultoria dashboard with filters, create form, detail view with timeline and opinions, real-time SignalR notifications, and PDF download.

**Architecture:** Angular 17 standalone components. `AuthInterceptor` injects Bearer token on every request. `RoleGuard` restricts routes. `TeleconsultoriaService` and `AuthService` wrap API calls. SignalR client (`@microsoft/signalr`) connects to `/hubs/notifications` and emits toast + status refresh on `NewOpinion` event.

**Tech Stack:** Angular 17, @microsoft/signalr, Angular Material (optional — use plain CSS if time-constrained), RxJS, Jasmine/Karma

---

## File Map

```
frontend/
├── angular.json
├── package.json
├── tsconfig.json
└── src/
    ├── main.ts
    ├── app/
    │   ├── app.config.ts
    │   ├── app.routes.ts
    │   ├── core/
    │   │   ├── models/
    │   │   │   ├── user.model.ts
    │   │   │   ├── teleconsultoria.model.ts
    │   │   │   ├── document.model.ts
    │   │   │   └── opinion.model.ts
    │   │   ├── services/
    │   │   │   ├── auth.service.ts
    │   │   │   ├── teleconsultoria.service.ts
    │   │   │   └── notification.service.ts
    │   │   ├── interceptors/
    │   │   │   └── auth.interceptor.ts
    │   │   └── guards/
    │   │       ├── auth.guard.ts
    │   │       └── role.guard.ts
    │   ├── auth/
    │   │   ├── login/
    │   │   │   ├── login.component.ts
    │   │   │   └── login.component.html
    │   │   └── register/
    │   │       ├── register.component.ts
    │   │       └── register.component.html
    │   ├── dashboard/
    │   │   ├── dashboard.component.ts
    │   │   └── dashboard.component.html
    │   └── teleconsultoria/
    │       ├── create/
    │       │   ├── create.component.ts
    │       │   └── create.component.html
    │       └── detail/
    │           ├── detail.component.ts
    │           └── detail.component.html
    └── environments/
        └── environment.ts
```

---

### Task 1: Angular Project Setup

**Files:**
- Create: `frontend/` (Angular project scaffold)
- Create: `frontend/src/environments/environment.ts`

- [ ] **Step 1: Scaffold Angular project**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
npx @angular/cli@17 new frontend --standalone --routing --style=css --skip-git --no-interactive
```

- [ ] **Step 2: Install packages**

```bash
cd frontend
npm install @microsoft/signalr
npm install --save-dev @types/node
```

- [ ] **Step 3: Set up environment**

`frontend/src/environments/environment.ts`:
```typescript
export const environment = {
  apiUrl: 'http://localhost:5000/api',
  hubUrl: 'http://localhost:5000/hubs/notifications'
};
```

- [ ] **Step 4: Verify dev server starts**

```bash
cd frontend
ng serve --port 4200
```
Expected: Application bundle generation complete. Navigate to `http://localhost:4200/`.

- [ ] **Step 5: Commit**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git add frontend/
git commit -m "chore(frontend): Angular 17 project scaffold"
```

---

### Task 2: Core — Models + Auth Service + Interceptor + Guards

**Files:**
- Create: `frontend/src/app/core/models/*.ts`
- Create: `frontend/src/app/core/services/auth.service.ts`
- Create: `frontend/src/app/core/interceptors/auth.interceptor.ts`
- Create: `frontend/src/app/core/guards/auth.guard.ts`
- Create: `frontend/src/app/core/guards/role.guard.ts`

- [ ] **Step 1: Create models**

`frontend/src/app/core/models/user.model.ts`:
```typescript
export type UserRole = 'Solicitante' | 'Especialista';

export interface AuthResult {
  token: string;
  userId: string;
  name: string;
  email: string;
  role: UserRole;
}
```

`frontend/src/app/core/models/document.model.ts`:
```typescript
export interface DocumentModel {
  id: string;
  fileName: string;
  validationScore: number;
  isApproved: boolean;
  validatedAt: string;
}
```

`frontend/src/app/core/models/opinion.model.ts`:
```typescript
export interface OpinionModel {
  id: string;
  specialistName: string;
  content: string;
  createdAt: string;
}
```

`frontend/src/app/core/models/teleconsultoria.model.ts`:
```typescript
import { DocumentModel } from './document.model';
import { OpinionModel } from './opinion.model';

export type Specialty =
  | 'Cardiologia' | 'CirurgiaRobotica' | 'Odontologia'
  | 'DoencasRaras' | 'Oxigenoterapia';

export type TeleconsultoriaStatus =
  | 'Pendente' | 'EmAndamento' | 'Concluida' | 'Cancelada';

export interface TeleconsultoriaListItem {
  id: string;
  patientName: string;
  specialty: string;
  status: TeleconsultoriaStatus;
  requesterName: string;
  createdAt: string;
}

export interface TeleconsultoriaDetail {
  id: string;
  patientName: string;
  birthDate: string;
  specialty: string;
  diagnosticHypothesis: string;
  clinicalHistory: string;
  status: TeleconsultoriaStatus;
  requesterName: string;
  createdAt: string;
  updatedAt: string;
  documents: DocumentModel[];
  opinions: OpinionModel[];
}

export interface CreateTeleconsultoriaDto {
  patientName: string;
  birthDate: string;
  specialty: number;
  diagnosticHypothesis: string;
  clinicalHistory: string;
}
```

- [ ] **Step 2: Create AuthService**

`frontend/src/app/core/services/auth.service.ts`:
```typescript
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
```

- [ ] **Step 3: Create AuthInterceptor**

`frontend/src/app/core/interceptors/auth.interceptor.ts`:
```typescript
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(req);
};
```

- [ ] **Step 4: Create guards**

`frontend/src/app/core/guards/auth.guard.ts`:
```typescript
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return auth.isLoggedIn() ? true : router.createUrlTree(['/login']);
};
```

`frontend/src/app/core/guards/role.guard.ts`:
```typescript
import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const requiredRole: string = route.data['role'];
  return auth.hasRole(requiredRole) ? true : router.createUrlTree(['/dashboard']);
};
```

- [ ] **Step 5: Write guard tests**

`frontend/src/app/core/guards/auth.guard.spec.ts`:
```typescript
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { authGuard } from './auth.guard';
import { AuthService } from '../services/auth.service';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

describe('authGuard', () => {
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    authService = jasmine.createSpyObj('AuthService', ['isLoggedIn']);
    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: { createUrlTree: (c: any) => c } }
      ]
    });
  });

  it('returns true when logged in', () => {
    authService.isLoggedIn.and.returnValue(true);
    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot));
    expect(result).toBeTrue();
  });

  it('redirects to /login when not logged in', () => {
    authService.isLoggedIn.and.returnValue(false);
    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot));
    expect(result).toEqual(['/login']);
  });
});
```

- [ ] **Step 6: Run guard tests**

```bash
cd frontend
npx ng test --include="**/auth.guard.spec.ts" --watch=false --browsers=ChromeHeadless
```
Expected: 2 tests pass.

- [ ] **Step 7: Commit**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git add frontend/src/app/core/
git commit -m "feat(frontend): core models, auth service, interceptor, guards"
```

---

### Task 3: Core — TeleconsultoriaService + NotificationService

**Files:**
- Create: `frontend/src/app/core/services/teleconsultoria.service.ts`
- Create: `frontend/src/app/core/services/notification.service.ts`

- [ ] **Step 1: Create TeleconsultoriaService**

`frontend/src/app/core/services/teleconsultoria.service.ts`:
```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  TeleconsultoriaListItem,
  TeleconsultoriaDetail,
  CreateTeleconsultoriaDto
} from '../models/teleconsultoria.model';

@Injectable({ providedIn: 'root' })
export class TeleconsultoriaService {
  private readonly base = `${environment.apiUrl}/teleconsultorias`;

  constructor(private http: HttpClient) {}

  list(filters: {
    specialty?: string;
    patient?: string;
    status?: string;
    dateFrom?: string;
    dateTo?: string;
  } = {}) {
    let params = new HttpParams();
    if (filters.specialty) params = params.set('specialty', filters.specialty);
    if (filters.patient) params = params.set('patient', filters.patient);
    if (filters.status) params = params.set('status', filters.status);
    if (filters.dateFrom) params = params.set('dateFrom', filters.dateFrom);
    if (filters.dateTo) params = params.set('dateTo', filters.dateTo);
    return this.http.get<TeleconsultoriaListItem[]>(this.base, { params });
  }

  getById(id: string) {
    return this.http.get<TeleconsultoriaDetail>(`${this.base}/${id}`);
  }

  create(dto: CreateTeleconsultoriaDto) {
    return this.http.post<{ id: string }>(this.base, dto);
  }

  updateStatus(id: string, status: number, notes?: string) {
    return this.http.put(`${this.base}/${id}/status`, { status, notes });
  }

  uploadDocument(id: string, file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<{ id: string }>(`${this.base}/${id}/documents`, form);
  }

  registerOpinion(id: string, content: string) {
    return this.http.post<{ id: string }>(`${this.base}/${id}/opinions`, { content });
  }

  exportPdfUrl(id: string): string {
    return `${this.base}/${id}/export/pdf`;
  }
}
```

- [ ] **Step 2: Create NotificationService (SignalR)**

`frontend/src/app/core/services/notification.service.ts`:
```typescript
import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

export interface NewOpinionPayload {
  teleconsultoriaId: string;
  opinionId: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private connection?: signalR.HubConnection;
  readonly lastOpinionNotification = signal<NewOpinionPayload | null>(null);

  constructor(private auth: AuthService) {}

  connect() {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        accessTokenFactory: () => this.auth.getToken() ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('NewOpinion', (payload: NewOpinionPayload) => {
      this.lastOpinionNotification.set(payload);
    });

    this.connection.start().catch(err => console.error('SignalR error:', err));
  }

  disconnect() {
    this.connection?.stop();
    this.connection = undefined;
  }
}
```

- [ ] **Step 3: Commit**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git add frontend/src/app/core/services/
git commit -m "feat(frontend): teleconsultoria service, SignalR notification service"
```

---

### Task 4: App Config + Routes

**Files:**
- Modify: `frontend/src/app/app.config.ts`
- Modify: `frontend/src/app/app.routes.ts`

- [ ] **Step 1: Configure app**

`frontend/src/app/app.config.ts`:
```typescript
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor]))
  ]
};
```

`frontend/src/app/app.routes.ts`:
```typescript
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'teleconsultorias/new',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Solicitante' },
    loadComponent: () => import('./teleconsultoria/create/create.component').then(m => m.CreateComponent)
  },
  {
    path: 'teleconsultorias/:id',
    canActivate: [authGuard],
    loadComponent: () => import('./teleconsultoria/detail/detail.component').then(m => m.DetailComponent)
  },
  { path: '**', redirectTo: 'dashboard' }
];
```

- [ ] **Step 2: Update app.component.ts to use router-outlet**

`frontend/src/app/app.component.ts`:
```typescript
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: '<router-outlet />'
})
export class AppComponent {}
```

- [ ] **Step 3: Commit**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git add frontend/src/app/app.config.ts frontend/src/app/app.routes.ts frontend/src/app/app.component.ts
git commit -m "feat(frontend): app config, lazy routes, router-outlet"
```

---

### Task 5: Auth Components — Login + Register

**Files:**
- Create: `frontend/src/app/auth/login/login.component.ts`
- Create: `frontend/src/app/auth/login/login.component.html`
- Create: `frontend/src/app/auth/register/register.component.ts`
- Create: `frontend/src/app/auth/register/register.component.html`

- [ ] **Step 1: Create LoginComponent**

`frontend/src/app/auth/login/login.component.ts`:
```typescript
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  email = '';
  password = '';
  error = signal<string | null>(null);
  loading = signal(false);

  constructor(
    private auth: AuthService,
    private notifications: NotificationService,
    private router: Router
  ) {}

  submit() {
    this.error.set(null);
    this.loading.set(true);
    this.auth.login(this.email, this.password).subscribe({
      next: () => {
        this.notifications.connect();
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.error.set('Credenciais inválidas.');
        this.loading.set(false);
      }
    });
  }
}
```

`frontend/src/app/auth/login/login.component.html`:
```html
<div class="auth-container">
  <div class="auth-card">
    <h1>V4H ReNTAI</h1>
    <h2>Login</h2>

    @if (error()) {
      <div class="alert alert-error">{{ error() }}</div>
    }

    <form (ngSubmit)="submit()">
      <div class="field">
        <label for="email">E-mail</label>
        <input id="email" type="email" [(ngModel)]="email" name="email" required />
      </div>
      <div class="field">
        <label for="password">Senha</label>
        <input id="password" type="password" [(ngModel)]="password" name="password" required />
      </div>
      <button type="submit" [disabled]="loading()">
        {{ loading() ? 'Entrando...' : 'Entrar' }}
      </button>
    </form>

    <p>Não tem conta? <a routerLink="/register">Registrar</a></p>
  </div>
</div>
```

- [ ] **Step 2: Create RegisterComponent**

`frontend/src/app/auth/register/register.component.ts`:
```typescript
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  name = '';
  email = '';
  password = '';
  role = 1; // 1 = Solicitante, 2 = Especialista
  error = signal<string | null>(null);
  loading = signal(false);

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.error.set(null);
    this.loading.set(true);
    this.auth.register(this.name, this.email, this.password, this.role).subscribe({
      next: () => this.router.navigate(['/login']),
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Erro ao registrar.');
        this.loading.set(false);
      }
    });
  }
}
```

`frontend/src/app/auth/register/register.component.html`:
```html
<div class="auth-container">
  <div class="auth-card">
    <h1>V4H ReNTAI</h1>
    <h2>Criar Conta</h2>

    @if (error()) {
      <div class="alert alert-error">{{ error() }}</div>
    }

    <form (ngSubmit)="submit()">
      <div class="field">
        <label for="name">Nome</label>
        <input id="name" type="text" [(ngModel)]="name" name="name" required />
      </div>
      <div class="field">
        <label for="email">E-mail</label>
        <input id="email" type="email" [(ngModel)]="email" name="email" required />
      </div>
      <div class="field">
        <label for="password">Senha</label>
        <input id="password" type="password" [(ngModel)]="password" name="password" required />
      </div>
      <div class="field">
        <label for="role">Perfil</label>
        <select id="role" [(ngModel)]="role" name="role">
          <option [value]="1">Solicitante (APS)</option>
          <option [value]="2">Especialista</option>
        </select>
      </div>
      <button type="submit" [disabled]="loading()">
        {{ loading() ? 'Registrando...' : 'Registrar' }}
      </button>
    </form>

    <p>Já tem conta? <a routerLink="/login">Entrar</a></p>
  </div>
</div>
```

- [ ] **Step 3: Add global CSS**

`frontend/src/styles.css` (append):
```css
* { box-sizing: border-box; margin: 0; padding: 0; }
body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; background: #f5f7fa; color: #333; }

.auth-container { display: flex; align-items: center; justify-content: center; min-height: 100vh; }
.auth-card { background: white; padding: 2rem; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); width: 100%; max-width: 400px; }
.auth-card h1 { color: #1a73e8; font-size: 1.5rem; margin-bottom: 0.25rem; }
.auth-card h2 { font-size: 1.1rem; color: #555; margin-bottom: 1.5rem; }

.field { margin-bottom: 1rem; }
.field label { display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.3rem; }
.field input, .field select { width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #ddd; border-radius: 4px; font-size: 1rem; }
.field input:focus, .field select:focus { outline: none; border-color: #1a73e8; }

button[type="submit"] { width: 100%; padding: 0.625rem; background: #1a73e8; color: white; border: none; border-radius: 4px; font-size: 1rem; cursor: pointer; margin-top: 0.5rem; }
button[type="submit"]:disabled { background: #93b9f5; cursor: not-allowed; }
button[type="submit"]:hover:not(:disabled) { background: #1557b0; }

.alert { padding: 0.75rem 1rem; border-radius: 4px; margin-bottom: 1rem; font-size: 0.875rem; }
.alert-error { background: #fce8e6; color: #c5221f; border: 1px solid #f5c6c4; }
.alert-success { background: #e6f4ea; color: #1e7e34; border: 1px solid #c3e6cb; }

.navbar { background: #1a73e8; color: white; padding: 0.75rem 1.5rem; display: flex; align-items: center; justify-content: space-between; }
.navbar h1 { font-size: 1.1rem; font-weight: 600; }
.navbar button { background: transparent; border: 1px solid rgba(255,255,255,0.5); color: white; padding: 0.3rem 0.75rem; border-radius: 4px; cursor: pointer; font-size: 0.875rem; }
.navbar button:hover { background: rgba(255,255,255,0.1); }

.container { max-width: 1100px; margin: 0 auto; padding: 1.5rem; }
.page-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 1.5rem; }
.page-header h2 { font-size: 1.25rem; }

.btn { display: inline-flex; align-items: center; padding: 0.5rem 1rem; border: none; border-radius: 4px; cursor: pointer; font-size: 0.875rem; font-weight: 500; text-decoration: none; }
.btn-primary { background: #1a73e8; color: white; }
.btn-primary:hover { background: #1557b0; }
.btn-secondary { background: #f1f3f4; color: #333; }
.btn-secondary:hover { background: #e2e5e8; }
.btn-sm { padding: 0.3rem 0.6rem; font-size: 0.8rem; }

.filters { background: white; padding: 1rem; border-radius: 6px; box-shadow: 0 1px 3px rgba(0,0,0,0.08); margin-bottom: 1rem; display: flex; gap: 0.75rem; flex-wrap: wrap; align-items: flex-end; }
.filters .field { margin-bottom: 0; min-width: 150px; }
.filters label { font-size: 0.8rem; }
.filters input, .filters select { padding: 0.35rem 0.5rem; font-size: 0.875rem; }

table { width: 100%; border-collapse: collapse; background: white; border-radius: 6px; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.08); }
th { background: #f8f9fa; text-align: left; padding: 0.75rem 1rem; font-size: 0.8rem; text-transform: uppercase; letter-spacing: 0.05em; color: #555; border-bottom: 1px solid #dee2e6; }
td { padding: 0.75rem 1rem; border-bottom: 1px solid #f0f0f0; font-size: 0.9rem; }
tr:last-child td { border-bottom: none; }
tr:hover td { background: #f8f9fa; }

.badge { display: inline-block; padding: 0.25rem 0.6rem; border-radius: 12px; font-size: 0.75rem; font-weight: 500; }
.badge-pending { background: #fff3cd; color: #856404; }
.badge-inprogress { background: #cce5ff; color: #004085; }
.badge-done { background: #d4edda; color: #155724; }
.badge-cancelled { background: #f8d7da; color: #721c24; }

.card { background: white; border-radius: 8px; box-shadow: 0 1px 4px rgba(0,0,0,0.1); padding: 1.5rem; margin-bottom: 1rem; }
.card h3 { font-size: 1rem; margin-bottom: 0.75rem; color: #1a73e8; }
.detail-row { display: flex; gap: 0.5rem; margin-bottom: 0.4rem; font-size: 0.9rem; }
.detail-label { font-weight: 600; min-width: 160px; color: #555; }

.opinion-item { border-left: 3px solid #1a73e8; padding-left: 0.75rem; margin-bottom: 0.75rem; }
.opinion-meta { font-size: 0.8rem; color: #777; margin-bottom: 0.25rem; }

.toast { position: fixed; top: 1rem; right: 1rem; background: #1a73e8; color: white; padding: 0.75rem 1.25rem; border-radius: 6px; box-shadow: 0 2px 8px rgba(0,0,0,0.15); z-index: 9999; font-size: 0.9rem; animation: slideIn 0.3s ease; }
@keyframes slideIn { from { transform: translateX(100%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

.form-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 4px rgba(0,0,0,0.1); max-width: 700px; }
```

- [ ] **Step 4: Commit**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git add frontend/src/app/auth/ frontend/src/styles.css
git commit -m "feat(frontend): login and register components with global styles"
```

---

### Task 6: Dashboard Component

**Files:**
- Create: `frontend/src/app/dashboard/dashboard.component.ts`
- Create: `frontend/src/app/dashboard/dashboard.component.html`

- [ ] **Step 1: Create DashboardComponent**

`frontend/src/app/dashboard/dashboard.component.ts`:
```typescript
import { Component, OnInit, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { TeleconsultoriaService } from '../core/services/teleconsultoria.service';
import { NotificationService } from '../core/services/notification.service';
import { TeleconsultoriaListItem } from '../core/models/teleconsultoria.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  items = signal<TeleconsultoriaListItem[]>([]);
  loading = signal(true);
  toast = signal<string | null>(null);

  filters = { patient: '', specialty: '', status: '' };

  constructor(
    public auth: AuthService,
    private tc: TeleconsultoriaService,
    private notifications: NotificationService
  ) {
    effect(() => {
      const notification = notifications.lastOpinionNotification();
      if (notification) {
        this.showToast('Novo parecer recebido!');
        this.loadList();
      }
    });
  }

  ngOnInit() {
    this.loadList();
  }

  loadList() {
    this.loading.set(true);
    this.tc.list({
      patient: this.filters.patient || undefined,
      specialty: this.filters.specialty || undefined,
      status: this.filters.status || undefined
    }).subscribe({
      next: (data) => { this.items.set(data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  applyFilters() {
    this.loadList();
  }

  clearFilters() {
    this.filters = { patient: '', specialty: '', status: '' };
    this.loadList();
  }

  statusBadgeClass(status: string): string {
    return {
      Pendente: 'badge badge-pending',
      EmAndamento: 'badge badge-inprogress',
      Concluida: 'badge badge-done',
      Cancelada: 'badge badge-cancelled'
    }[status] ?? 'badge';
  }

  logout() { this.auth.logout(); }

  private showToast(msg: string) {
    this.toast.set(msg);
    setTimeout(() => this.toast.set(null), 4000);
  }
}
```

`frontend/src/app/dashboard/dashboard.component.html`:
```html
<nav class="navbar">
  <h1>V4H ReNTAI — Teleconsultorias</h1>
  <div style="display:flex;align-items:center;gap:1rem">
    <span style="font-size:0.875rem">{{ auth.currentUser()?.name }} ({{ auth.currentUser()?.role }})</span>
    <button (click)="logout()">Sair</button>
  </div>
</nav>

@if (toast()) {
  <div class="toast">{{ toast() }}</div>
}

<div class="container">
  <div class="page-header">
    <h2>Teleconsultorias</h2>
    @if (auth.hasRole('Solicitante')) {
      <a routerLink="/teleconsultorias/new" class="btn btn-primary">+ Nova Solicitação</a>
    }
  </div>

  <div class="filters">
    <div class="field">
      <label>Paciente</label>
      <input type="text" [(ngModel)]="filters.patient" placeholder="Buscar paciente..." />
    </div>
    <div class="field">
      <label>Especialidade</label>
      <select [(ngModel)]="filters.specialty">
        <option value="">Todas</option>
        <option value="1">Cardiologia</option>
        <option value="2">Cirurgia Robótica</option>
        <option value="3">Odontologia</option>
        <option value="4">Doenças Raras</option>
        <option value="5">Oxigenoterapia</option>
      </select>
    </div>
    <div class="field">
      <label>Status</label>
      <select [(ngModel)]="filters.status">
        <option value="">Todos</option>
        <option value="1">Pendente</option>
        <option value="2">Em Andamento</option>
        <option value="3">Concluída</option>
        <option value="4">Cancelada</option>
      </select>
    </div>
    <button class="btn btn-primary btn-sm" (click)="applyFilters()">Filtrar</button>
    <button class="btn btn-secondary btn-sm" (click)="clearFilters()">Limpar</button>
  </div>

  @if (loading()) {
    <p>Carregando...</p>
  } @else if (items().length === 0) {
    <p style="color:#777;margin-top:2rem;text-align:center">Nenhuma teleconsultoria encontrada.</p>
  } @else {
    <table>
      <thead>
        <tr>
          <th>Paciente</th>
          <th>Especialidade</th>
          <th>Status</th>
          <th>Solicitante</th>
          <th>Data</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        @for (item of items(); track item.id) {
          <tr>
            <td>{{ item.patientName }}</td>
            <td>{{ item.specialty }}</td>
            <td><span [class]="statusBadgeClass(item.status)">{{ item.status }}</span></td>
            <td>{{ item.requesterName }}</td>
            <td>{{ item.createdAt | date:'dd/MM/yyyy' }}</td>
            <td><a [routerLink]="['/teleconsultorias', item.id]" class="btn btn-secondary btn-sm">Ver</a></td>
          </tr>
        }
      </tbody>
    </table>
  }
</div>
```

- [ ] **Step 2: Commit**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git add frontend/src/app/dashboard/
git commit -m "feat(frontend): dashboard with filters, list, real-time toast"
```

---

### Task 7: Teleconsultoria Create + Detail Components

**Files:**
- Create: `frontend/src/app/teleconsultoria/create/create.component.ts`
- Create: `frontend/src/app/teleconsultoria/create/create.component.html`
- Create: `frontend/src/app/teleconsultoria/detail/detail.component.ts`
- Create: `frontend/src/app/teleconsultoria/detail/detail.component.html`

- [ ] **Step 1: Create CreateComponent**

`frontend/src/app/teleconsultoria/create/create.component.ts`:
```typescript
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TeleconsultoriaService } from '../../core/services/teleconsultoria.service';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  templateUrl: './create.component.html'
})
export class CreateComponent {
  form = {
    patientName: '',
    birthDate: '',
    specialty: 1,
    diagnosticHypothesis: '',
    clinicalHistory: ''
  };

  error = signal<string | null>(null);
  loading = signal(false);

  constructor(private tc: TeleconsultoriaService, private router: Router) {}

  submit() {
    this.error.set(null);
    this.loading.set(true);
    this.tc.create(this.form).subscribe({
      next: (res) => this.router.navigate(['/teleconsultorias', res.id]),
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Erro ao criar teleconsultoria.');
        this.loading.set(false);
      }
    });
  }
}
```

`frontend/src/app/teleconsultoria/create/create.component.html`:
```html
<nav class="navbar">
  <h1>Nova Teleconsultoria</h1>
  <a routerLink="/dashboard" class="btn btn-secondary btn-sm">← Voltar</a>
</nav>

<div class="container">
  @if (error()) {
    <div class="alert alert-error">{{ error() }}</div>
  }

  <div class="form-card">
    <form (ngSubmit)="submit()">
      <div class="field">
        <label>Nome do Paciente *</label>
        <input type="text" [(ngModel)]="form.patientName" name="patientName" required />
      </div>
      <div class="field">
        <label>Data de Nascimento *</label>
        <input type="date" [(ngModel)]="form.birthDate" name="birthDate" required />
      </div>
      <div class="field">
        <label>Especialidade *</label>
        <select [(ngModel)]="form.specialty" name="specialty">
          <option [value]="1">Cardiologia</option>
          <option [value]="2">Cirurgia Robótica</option>
          <option [value]="3">Odontologia</option>
          <option [value]="4">Doenças Raras</option>
          <option [value]="5">Oxigenoterapia</option>
        </select>
      </div>
      <div class="field">
        <label>Hipótese Diagnóstica *</label>
        <textarea [(ngModel)]="form.diagnosticHypothesis" name="diagnosticHypothesis"
          rows="3" style="width:100%;padding:0.5rem;border:1px solid #ddd;border-radius:4px" required></textarea>
      </div>
      <div class="field">
        <label>Histórico Clínico *</label>
        <textarea [(ngModel)]="form.clinicalHistory" name="clinicalHistory"
          rows="5" style="width:100%;padding:0.5rem;border:1px solid #ddd;border-radius:4px" required></textarea>
      </div>
      <div style="display:flex;gap:0.75rem;margin-top:1rem">
        <button type="submit" class="btn btn-primary" [disabled]="loading()">
          {{ loading() ? 'Salvando...' : 'Criar Teleconsultoria' }}
        </button>
        <a routerLink="/dashboard" class="btn btn-secondary">Cancelar</a>
      </div>
    </form>
  </div>
</div>
```

- [ ] **Step 2: Create DetailComponent**

`frontend/src/app/teleconsultoria/detail/detail.component.ts`:
```typescript
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeleconsultoriaService } from '../../core/services/teleconsultoria.service';
import { AuthService } from '../../core/services/auth.service';
import { TeleconsultoriaDetail } from '../../core/models/teleconsultoria.model';

@Component({
  selector: 'app-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './detail.component.html'
})
export class DetailComponent implements OnInit {
  tc = signal<TeleconsultoriaDetail | null>(null);
  loading = signal(true);
  opinionContent = '';
  selectedFile: File | null = null;
  actionLoading = signal(false);
  message = signal<{ type: 'success' | 'error'; text: string } | null>(null);
  newStatus = signal(2);

  constructor(
    private route: ActivatedRoute,
    private tcService: TeleconsultoriaService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.load();
  }

  private get id(): string {
    return this.route.snapshot.paramMap.get('id')!;
  }

  load() {
    this.loading.set(true);
    this.tcService.getById(this.id).subscribe({
      next: (data) => { this.tc.set(data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  uploadDoc() {
    if (!this.selectedFile) return;
    this.actionLoading.set(true);
    this.tcService.uploadDocument(this.id, this.selectedFile).subscribe({
      next: () => {
        this.message.set({ type: 'success', text: 'Documento enviado com sucesso!' });
        this.selectedFile = null;
        this.actionLoading.set(false);
        this.load();
      },
      error: (err) => {
        const score = err?.error?.extra?.score;
        this.message.set({ type: 'error', text: score
          ? `Documento rejeitado. Score: ${score}`
          : (err?.error?.error ?? 'Erro ao enviar documento.') });
        this.actionLoading.set(false);
      }
    });
  }

  submitOpinion() {
    if (!this.opinionContent.trim()) return;
    this.actionLoading.set(true);
    this.tcService.registerOpinion(this.id, this.opinionContent).subscribe({
      next: () => {
        this.message.set({ type: 'success', text: 'Parecer registrado!' });
        this.opinionContent = '';
        this.actionLoading.set(false);
        this.load();
      },
      error: (err) => {
        this.message.set({ type: 'error', text: err?.error?.error ?? 'Erro ao registrar parecer.' });
        this.actionLoading.set(false);
      }
    });
  }

  updateStatus() {
    this.actionLoading.set(true);
    this.tcService.updateStatus(this.id, this.newStatus()).subscribe({
      next: () => {
        this.message.set({ type: 'success', text: 'Status atualizado!' });
        this.actionLoading.set(false);
        this.load();
      },
      error: (err) => {
        this.message.set({ type: 'error', text: err?.error?.error ?? 'Erro ao atualizar status.' });
        this.actionLoading.set(false);
      }
    });
  }

  pdfUrl(): string {
    return this.tcService.exportPdfUrl(this.id);
  }

  statusBadgeClass(status: string): string {
    return {
      Pendente: 'badge badge-pending',
      EmAndamento: 'badge badge-inprogress',
      Concluida: 'badge badge-done',
      Cancelada: 'badge badge-cancelled'
    }[status] ?? 'badge';
  }
}
```

`frontend/src/app/teleconsultoria/detail/detail.component.html`:
```html
<nav class="navbar">
  <h1>Detalhes da Teleconsultoria</h1>
  <a routerLink="/dashboard" class="btn btn-secondary btn-sm">← Voltar</a>
</nav>

<div class="container">
  @if (loading()) {
    <p>Carregando...</p>
  } @else if (!tc()) {
    <p>Não encontrado.</p>
  } @else {
    @let item = tc()!;

    @if (message()) {
      <div class="alert" [class.alert-success]="message()!.type === 'success'" [class.alert-error]="message()!.type === 'error'">
        {{ message()!.text }}
      </div>
    }

    <div class="card">
      <div style="display:flex;justify-content:space-between;align-items:flex-start;margin-bottom:1rem">
        <div>
          <h2 style="font-size:1.25rem">{{ item.patientName }}</h2>
          <p style="color:#777;font-size:0.875rem">{{ item.specialty }}</p>
        </div>
        <div style="display:flex;gap:0.5rem;align-items:center">
          <span [class]="statusBadgeClass(item.status)">{{ item.status }}</span>
          <a [href]="pdfUrl()" target="_blank" class="btn btn-secondary btn-sm">⬇ PDF</a>
        </div>
      </div>

      <div class="detail-row"><span class="detail-label">Nascimento:</span>{{ item.birthDate }}</div>
      <div class="detail-row"><span class="detail-label">Solicitante:</span>{{ item.requesterName }}</div>
      <div class="detail-row"><span class="detail-label">Criado em:</span>{{ item.createdAt | date:'dd/MM/yyyy HH:mm' }}</div>
      <div class="detail-row"><span class="detail-label">Atualizado em:</span>{{ item.updatedAt | date:'dd/MM/yyyy HH:mm' }}</div>
      <div style="margin-top:0.75rem">
        <p class="detail-label">Hipótese Diagnóstica:</p>
        <p style="margin-top:0.25rem">{{ item.diagnosticHypothesis }}</p>
      </div>
      <div style="margin-top:0.75rem">
        <p class="detail-label">Histórico Clínico:</p>
        <p style="margin-top:0.25rem">{{ item.clinicalHistory }}</p>
      </div>
    </div>

    <!-- Documents section (Solicitante) -->
    @if (auth.hasRole('Solicitante')) {
      <div class="card">
        <h3>Documentos</h3>
        @for (doc of item.documents; track doc.id) {
          <div style="display:flex;justify-content:space-between;padding:0.5rem 0;border-bottom:1px solid #f0f0f0">
            <span>{{ doc.fileName }}</span>
            <span style="font-size:0.8rem;color:#777">Score: {{ doc.validationScore | number:'1.2-2' }} — {{ doc.isApproved ? '✅' : '❌' }}</span>
          </div>
        }
        <div style="margin-top:1rem">
          <input type="file" (change)="onFileChange($event)" accept=".pdf,.jpg,.jpeg,.png" />
          <button class="btn btn-primary btn-sm" style="margin-top:0.5rem" (click)="uploadDoc()" [disabled]="!selectedFile || actionLoading()">
            {{ actionLoading() ? 'Enviando...' : 'Enviar Documento' }}
          </button>
        </div>
      </div>
    }

    <!-- Opinions section -->
    <div class="card">
      <h3>Pareceres</h3>
      @if (item.opinions.length === 0) {
        <p style="color:#777;font-size:0.875rem">Nenhum parecer registrado.</p>
      }
      @for (op of item.opinions; track op.id) {
        <div class="opinion-item">
          <p class="opinion-meta">{{ op.specialistName }} — {{ op.createdAt | date:'dd/MM/yyyy HH:mm' }}</p>
          <p>{{ op.content }}</p>
        </div>
      }

      @if (auth.hasRole('Especialista')) {
        <div style="margin-top:1rem">
          <label style="font-weight:600;font-size:0.875rem">Registrar Parecer</label>
          <textarea [(ngModel)]="opinionContent" rows="4"
            style="width:100%;padding:0.5rem;border:1px solid #ddd;border-radius:4px;margin-top:0.5rem"
            placeholder="Descreva o parecer clínico..."></textarea>
          <button class="btn btn-primary btn-sm" style="margin-top:0.5rem"
            (click)="submitOpinion()" [disabled]="!opinionContent.trim() || actionLoading()">
            {{ actionLoading() ? 'Salvando...' : 'Registrar Parecer' }}
          </button>
        </div>
      }
    </div>

    <!-- Status update (Especialista) -->
    @if (auth.hasRole('Especialista')) {
      <div class="card">
        <h3>Atualizar Status</h3>
        <div style="display:flex;gap:0.75rem;align-items:center">
          <select [(ngModel)]="newStatus" style="padding:0.4rem 0.6rem;border:1px solid #ddd;border-radius:4px">
            <option [value]="1">Pendente</option>
            <option [value]="2">Em Andamento</option>
            <option [value]="3">Concluída</option>
            <option [value]="4">Cancelada</option>
          </select>
          <button class="btn btn-primary btn-sm" (click)="updateStatus()" [disabled]="actionLoading()">
            Atualizar
          </button>
        </div>
      </div>
    }
  }
</div>
```

- [ ] **Step 3: Commit**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git add frontend/src/app/teleconsultoria/
git commit -m "feat(frontend): create and detail teleconsultoria components"
```

---

### Task 8: Build Verification

- [ ] **Step 1: Build production bundle**

```bash
cd frontend
npx ng build --configuration production
```
Expected: Application bundle generation complete. No errors.

- [ ] **Step 2: Run all tests**

```bash
npx ng test --watch=false --browsers=ChromeHeadless
```
Expected: Executed N specs, 0 failures.

- [ ] **Step 3: Run dev server and do manual smoke test**

```bash
npx ng serve
```

With backend running, test:
1. Navigate to `http://localhost:4200/login`
2. Register as Solicitante, login → dashboard shows
3. Create new teleconsultoria → redirects to detail
4. Upload PDF → success toast
5. Login as Especialista in another tab → dashboard shows the record
6. Register opinion → status updates to Concluída
7. Solicitante tab receives real-time toast notification
8. Click PDF button → PDF downloads

- [ ] **Step 4: Final commit**

```bash
cd C:\Users\joaom\Dev\v4h-rentai
git add frontend/
git commit -m "feat(frontend): complete Angular 17 frontend — all features verified"
```
