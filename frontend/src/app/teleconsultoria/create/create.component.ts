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
