import { DocumentModel } from './document.model';
import { OpinionModel } from './opinion.model';

export interface StatusHistoryItem {
  id: string;
  oldStatus: string;
  newStatus: string;
  changedAt: string;
  changedByName: string;
  notes: string | null;
}

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
  statusHistories: StatusHistoryItem[];
}

export interface CreateTeleconsultoriaDto {
  patientName: string;
  birthDate: string;
  specialty: number;
  diagnosticHypothesis: string;
  clinicalHistory: string;
}
