import { Component, OnInit, signal, computed } from '@angular/core';
import { FaturamentoService } from '../../../core/services/faturamento';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { NotaFiscalResponseDTO } from '../../../core/models/faturamento.models';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { 
  LucideAngularModule, 
  FileText, 
  Plus, 
  Printer, 
  Search,
  CheckCircle,
  Clock,
  Eye,
  X,
  Package,
  Loader2
} from 'lucide-angular';

@Component({
  selector: 'app-lista-notas',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSnackBarModule,
    MatTooltipModule,
    LucideAngularModule
  ],
  templateUrl: './lista-notas.html',
  styleUrl: './lista-notas.css',
})
export class ListaNotas implements OnInit {
  readonly NotaIcon = FileText;
  readonly PlusIcon = Plus;
  readonly ImprimirIcon = Printer;
  readonly SearchIcon = Search;
  readonly AbertaIcon = Clock;
  readonly FaturadaIcon = CheckCircle;
  readonly VerIcon = Eye;
  readonly FecharIcon = X;
  readonly ItemIcon = Package;
  readonly LoaderIcon = Loader2;

  notas = signal<NotaFiscalResponseDTO[]>([]);
  totalRegistros = signal<number>(0);
  paginaAtual = signal<number>(1);
  itensPorPagina = signal<number>(10);

  notaSelecionada = signal<NotaFiscalResponseDTO | null>(null);
  faturandoId = signal<string | null>(null);
  termoBusca = signal<string>('');

  notasFiltradas = computed(() => {
    const termo = this.termoBusca().toLowerCase().trim();
    if (!termo) return this.notas();
    return this.notas().filter(n =>
      n.numeroSequencial.toString().includes(termo)
    );
  });

  colunas: string[] = ['numero', 'status', 'acoes'];
  colunasItens: string[] = ['nomeProduto', 'quantidade'];

  constructor(
    private faturamentoService: FaturamentoService,
    private snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.carregarNotas();
  }

  carregarNotas() {
    this.faturamentoService.listarNotas(this.paginaAtual(), this.itensPorPagina()).subscribe({
      next: (resultado) => {
        this.notas.set(resultado.items);
        this.totalRegistros.set(resultado.totalCount);
      },
      error: (err: HttpErrorResponse) => this.mostrarErro(err),
    });
  }

  mudarPagina(event: PageEvent) {
    this.paginaAtual.set(event.pageIndex + 1);
    this.itensPorPagina.set(event.pageSize);
    this.carregarNotas();
  }

  verDetalhes(id: string) {
    this.faturamentoService.buscarNotaPorId(id).subscribe({
      next: (nota) => this.notaSelecionada.set(nota),
      error: (err: HttpErrorResponse) => this.mostrarErro(err)
    });
  }

  faturarNota(id: string) {
    this.faturandoId.set(id);
    this.faturamentoService.imprimirNota(id).subscribe({
      next: () => {
        this.faturandoId.set(null);
        this.snackBar.open('Nota Faturada e Impressa com sucesso!', 'Fechar', { duration: 3000 });
        if (this.notaSelecionada()?.id === id) {
          this.verDetalhes(id);
        }
        this.carregarNotas();
      },
      error: (err: HttpErrorResponse) => {
        this.faturandoId.set(null);
        this.mostrarErro(err);
      }
    });
  }

  mostrarErro(err: HttpErrorResponse) {
    const mensagemErro = err.error?.erro || 'Ocorreu um erro inesperado';
    this.snackBar.open(mensagemErro, 'Fechar', { duration: 5000 });
  }
}
