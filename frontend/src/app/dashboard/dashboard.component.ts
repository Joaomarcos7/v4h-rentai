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

  applyFilters() { this.loadList(); }

  clearFilters() {
    this.filters = { patient: '', specialty: '', status: '' };
    this.loadList();
  }

  statusBadgeClass(status: string): string {
    return ({
      Pendente: 'badge badge-pending',
      EmAndamento: 'badge badge-inprogress',
      Concluida: 'badge badge-done',
      Cancelada: 'badge badge-cancelled'
    } as Record<string, string>)[status] ?? 'badge';
  }

  logout() { this.auth.logout(); }

  private showToast(msg: string) {
    this.toast.set(msg);
    setTimeout(() => this.toast.set(null), 4000);
  }
}
