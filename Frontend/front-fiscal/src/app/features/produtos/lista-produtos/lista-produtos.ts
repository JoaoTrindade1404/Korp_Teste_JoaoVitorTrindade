import { CommonModule } from '@angular/common';
import { Component, OnInit, signal, computed } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ProdutoCreateDTO, ProdutoResponseDTO } from '../../../core/models/produto.models';
import { EstoqueService } from '../../../core/services/estoque';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import {
  LucideAngularModule,
  Plus,
  Save,
  Pencil,
  X,
  Database,
  Search,
  Trash2,
} from 'lucide-angular';

@Component({
  selector: 'app-lista-produtos',
  standalone: true,
  imports: [
    MatButtonModule,
    MatTableModule,
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatTooltipModule,
    MatSnackBarModule,
    MatCardModule,
    LucideAngularModule,
    MatPaginatorModule,
  ],
  templateUrl: './lista-produtos.html',
  styleUrl: './lista-produtos.css',
})
export class ListaProdutos implements OnInit {
  readonly PlusIcon = Plus;
  readonly SaveIcon = Save;
  readonly PencilIcon = Pencil;
  readonly CancelIcon = X;
  readonly StockIcon = Database;
  readonly SearchIcon = Search;
  readonly TrashIcon = Trash2;

  produtos = signal<ProdutoResponseDTO[]>([]);

  produtoEditandoId = signal<string | null>(null);

  produtoDeleteId = signal<string | null>(null);

  totalRegistros = signal<number>(0);

  paginaAtual = signal<number>(1);

  itensPorPagina = signal<number>(10);

  termoBusca = signal<string>('');

  produtosFiltrados = computed(() => {
    const termo = this.termoBusca().toLowerCase().trim();
    if (!termo) return this.produtos();
    return this.produtos().filter(p =>
      p.codigo.toLowerCase().includes(termo) ||
      p.descricao.toLowerCase().includes(termo)
    );
  });

  colunas: string[] = ['codigo', 'descricao', 'saldo', 'acoes'];

  constructor(
    private estoqueService: EstoqueService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
  ) {}

  produtoForm!: FormGroup;

  ngOnInit(): void {
    this.carregarProdutos();

    this.produtoForm = this.fb.group({
      codigo: ['', [Validators.required]],
      descricao: ['', [Validators.required]],
      saldo: [0, [Validators.required, Validators.min(1)]],
    });
  }

  carregarProdutos() {
    this.estoqueService
      .listarProdutos(this.paginaAtual(), this.itensPorPagina())
      .subscribe((dados) => {
        this.produtos.set(dados.items);

        this.totalRegistros.set(dados.totalCount);
      });
  }

  salvar() {
    if (this.produtoForm.invalid) return;

    const dados: ProdutoCreateDTO = this.produtoForm.value;
    const idEditando = this.produtoEditandoId();

    if (idEditando) {
      this.estoqueService.atualizarProduto(idEditando, dados).subscribe({
        next: (produtoAtualizando) => {
          this.produtos.update((listaAtual) =>
            listaAtual.map((p) => (p.id === idEditando ? produtoAtualizando : p)),
          );

          this.limparForm();
        },
        error: (err: HttpErrorResponse) => this.mostrarErro(err),
      });
    } else {
      this.estoqueService.cadastrarProduto(dados).subscribe({
        next: (produtoCriado) => {
          this.produtos.update((listaAtual) => [...listaAtual, produtoCriado]);

          this.produtoForm.reset();
        },
        error: (err: HttpErrorResponse) => this.mostrarErro(err),
      });
    }
  }

  prepararEdicao(produto: ProdutoResponseDTO) {
    this.produtoEditandoId.set(produto.id);

    this.produtoForm.patchValue(produto);
  }

  cancelarEdicao() {
    this.limparForm();
  }

  limparForm() {
    this.produtoForm.reset();
    this.produtoEditandoId.set(null);
  }

  mostrarErro(err: HttpErrorResponse) {
    const mensagemErro = err.error?.erro || 'Ocorreu um erro inesperado';
    this.snackBar.open(mensagemErro, 'Fechar', { duration: 5000 });
  }

  abrirModalExclusao(id: string) {
    this.produtoDeleteId.set(id);
  }

  confirmarExclusao() {
    const id = this.produtoDeleteId();

    if (id) {
      this.estoqueService.deletarProduto(id).subscribe({
        next: (produtoDeletado) => {
          this.produtos.update((listaAtual) => listaAtual.filter((p) => p.id !== id));

          this.produtoDeleteId.set(null);
        },
        error: (err: HttpErrorResponse) => this.mostrarErro(err),
      });
    }
  }

  mudarPagina(event: PageEvent) {
    this.paginaAtual.set(event.pageIndex + 1);
    this.itensPorPagina.set(event.pageSize);
    this.carregarProdutos();
  }
}
