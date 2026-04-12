import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PagedResult } from '../models/paged-result.models';
import { Observable } from 'rxjs';
import { NotaFiscalCreateDTO, NotaFiscalResponseDTO } from '../models/faturamento.models';

@Injectable({
  providedIn: 'root',
})
export class FaturamentoService {
  private apiUrl = 'http://localhost:5010/notas';

  constructor(private http: HttpClient) {}

  listarNotas(page: number, pageSize: number): Observable<PagedResult<NotaFiscalResponseDTO>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<NotaFiscalResponseDTO>>(this.apiUrl, { params });
  }

  cadastrarNota(nota: NotaFiscalCreateDTO): Observable<NotaFiscalResponseDTO> {
    return this.http.post<NotaFiscalResponseDTO>(this.apiUrl, nota)
  }

  imprimirNota(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/imprimir`, null)
  }

  buscarNotaPorId(id: string): Observable<NotaFiscalResponseDTO> {
    return this.http.get<NotaFiscalResponseDTO>(`${this.apiUrl}/${id}`);
  }
}
