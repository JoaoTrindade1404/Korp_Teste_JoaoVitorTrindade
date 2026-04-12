import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs'; 
import { ProdutoCreateDTO, ProdutoResponseDTO } from '../models/produto.models';

@Injectable({
  providedIn: 'root' 
})
export class EstoqueService {

  private apiUrl = 'http://localhost:5225/produtos';

  constructor(private http: HttpClient) { }

  listarProdutos(): Observable<ProdutoResponseDTO[]> {
    return this.http.get<ProdutoResponseDTO[]>(this.apiUrl);
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
