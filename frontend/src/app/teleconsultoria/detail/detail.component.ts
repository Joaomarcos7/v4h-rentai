import { Component, OnInit, effect, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeleconsultoriaService } from '../../core/services/teleconsultoria.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
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
  newStatus = 2;

  constructor(
    private route: ActivatedRoute,
    private tcService: TeleconsultoriaService,
    public auth: AuthService,
    notifications: NotificationService
  ) {
    effect(() => {
      const n = notifications.lastOpinionNotification();
      this.onNotification(n);
    }, { allowSignalWrites: true });
  }

  onNotification(payload: { teleconsultoriaId: string; opinionId: string } | null) {
    if (payload && payload.teleconsultoriaId === this.id) {
      this.load();
    }
  }

  ngOnInit() { this.load(); }

  private get id(): string {
    return this.route.snapshot.paramMap.get('id')!;
  }

  private static readonly statusMap: Record<string, number> = {
    Pendente: 1, EmAndamento: 2, Concluida: 3, Cancelada: 4
  };

  load() {
    this.loading.set(true);
    this.tcService.getById(this.id).subscribe({
      next: (data) => {
        this.tc.set(data);
        this.newStatus = DetailComponent.statusMap[data.status] ?? 1;
        this.loading.set(false);
      },
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
    this.tcService.updateStatus(this.id, this.newStatus).subscribe({
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

  downloadPdf() {
    this.tcService.exportPdf(this.id).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `teleconsultoria-${this.id}.pdf`;
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  statusBadgeClass(status: string): string {
    return ({
      Pendente: 'badge badge-pending',
      EmAndamento: 'badge badge-inprogress',
      Concluida: 'badge badge-done',
      Cancelada: 'badge badge-cancelled'
    } as Record<string, string>)[status] ?? 'badge';
  }
}
