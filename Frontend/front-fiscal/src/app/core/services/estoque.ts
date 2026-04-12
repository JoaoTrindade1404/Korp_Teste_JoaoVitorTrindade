import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProdutoCreateDTO, ProdutoResponseDTO } from '../models/produto.models';
import { PagedResult } from '../models/paged-result.models';

@Injectable({
  providedIn: 'root',
})
export class EstoqueService {
  private apiUrl = 'http://localhost:5225/produtos';

  constructor(private http: HttpClient) {}

  listarProdutos(page: number, pageSize: number): Observable<PagedResult<ProdutoResponseDTO>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<ProdutoResponseDTO>>(this.apiUrl, { params });
  }

  cadastrarProduto(produto: ProdutoCreateDTO): Observable<ProdutoResponseDTO> {
    return this.http.post<ProdutoResponseDTO>(this.apiUrl, produto);
  }

  atualizarProduto(id: string, produto: ProdutoCreateDTO): Observable<ProdutoResponseDTO> {
    return this.http.put<ProdutoResponseDTO>(`${this.apiUrl}/${id}`, produto);
  }

  deletarProduto(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
