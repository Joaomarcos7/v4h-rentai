import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

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
  role = 1;
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
