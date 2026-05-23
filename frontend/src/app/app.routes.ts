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
