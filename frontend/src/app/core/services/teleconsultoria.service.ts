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
