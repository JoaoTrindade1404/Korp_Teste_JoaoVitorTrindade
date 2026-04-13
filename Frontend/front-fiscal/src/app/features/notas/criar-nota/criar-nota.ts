import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';


import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';


import { EstoqueService } from '../../../core/services/estoque';
import { FaturamentoService } from '../../../core/services/faturamento';
import { ProdutoResponseDTO } from '../../../core/models/produto.models';
import { ItemNotaFiscalCreateDTO } from '../../../core/models/faturamento.models';
import { LucideAngularModule, ShoppingCart, Plus, Trash2, ArrowLeft, Save, FileText, Sparkles, Loader } from 'lucide-angular';

@Component({
  selector: 'app-criar-nota',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    MatSnackBarModule,
    MatCardModule,
    MatTooltipModule,
    LucideAngularModule,
    FormsModule
  ],
  templateUrl: './criar-nota.html',
})
export class CriarNota implements OnInit {
  readonly CartIcon = ShoppingCart;
  readonly PlusIcon = Plus;
  readonly TrashIcon = Trash2;
  readonly BackIcon = ArrowLeft;
  readonly SaveIcon = Save;
  readonly NotaIcon = FileText;
  readonly SparklesIcon = Sparkles;
  readonly LoaderIcon = Loader;

  private fb = inject(FormBuilder);
  private estoqueService = inject(EstoqueService);
  private faturamentoService = inject(FaturamentoService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);

  produtosDisponiveis = signal<ProdutoResponseDTO[]>([]);
  itensDaNota = signal<ItemNotaFiscalCreateDTO[]>([]);
  salvando = signal(false);
  textoIa = signal<string>('');
  processandoIa = signal<boolean>(false);

  podeSalvar = computed(() => this.itensDaNota().length > 0 && !this.salvando());

  colunas = ['produto', 'quantidade', 'acoes'];

  itemForm = this.fb.group({
    produtoId: ['', Validators.required],
    quantidade: [1, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.estoqueService.listarProdutos(1, 100).subscribe({
      next: (resultado) => this.produtosDisponiveis.set(resultado.items),
      error: (err: HttpErrorResponse) => this.mostrarErro(err)
    });
  }

  adicionarItem(): void {
    if (this.itemForm.invalid) return;

    const { produtoId, quantidade } = this.itemForm.value;

    const produtoSelecionado = this.produtosDisponiveis().find(p => p.id === produtoId);
    if (!produtoSelecionado) return;

    const novoItem: ItemNotaFiscalCreateDTO = {
      produtoId: produtoSelecionado.id,
      nomeProduto: produtoSelecionado.descricao,
      quantidade: quantidade!
    };

    this.itensDaNota.update(lista => [...lista, novoItem]);

    this.itemForm.reset({ produtoId: '', quantidade: 1 });
  }

  removerItem(indice: number): void {
    this.itensDaNota.update(lista => lista.filter((_, i) => i !== indice));
  }

  salvarNota(): void {
    if (!this.podeSalvar()) return;

    this.salvando.set(true);

    this.faturamentoService.cadastrarNota({ itens: this.itensDaNota() }).subscribe({
      next: (nota) => {
        this.snackBar.open(
          `Nota Fiscal Nº ${nota.numeroSequencial} criada com sucesso!`,
          'Fechar',
          { duration: 4000 }
        );
        this.router.navigate(['/notas']);
      },
      error: (err: HttpErrorResponse) => {
        this.mostrarErro(err);
        this.salvando.set(false);
      }
    });
  }

  obterNomeProduto(produtoId: string): string {
    return this.produtosDisponiveis().find(p => p.id === produtoId)?.descricao ?? produtoId;
  }

  private mostrarErro(err: HttpErrorResponse): void {
    const mensagem = err.error?.erro || 'Ocorreu um erro inesperado';
    this.snackBar.open(mensagem, 'Fechar', { duration: 5000 });
  }

  adicionarComIA(): void {
  const texto = this.textoIa().trim();
  if (!texto) return;

  this.processandoIa.set(true);

  this.faturamentoService.extrairItensComIA(texto, this.produtosDisponiveis()).subscribe({
    next: (resposta) => {
      this.processandoIa.set(false);

      if (resposta.sucesso) {
        let itensAdicionados = 0;

        resposta.itens.forEach(itemIA => {
          const produto = this.produtosDisponiveis().find(p => p.id === itemIA.produtoId);
          if (produto) {
            const novoItem: ItemNotaFiscalCreateDTO = {
              produtoId: produto.id,
              nomeProduto: produto.descricao,
              quantidade: itemIA.quantidade
            };
            this.itensDaNota.update(lista => [...lista, novoItem]);
            itensAdicionados++;
          }
        });

        this.textoIa.set(''); 
        this.snackBar.open(`🪄 ${itensAdicionados} item(ns) adicionado(s) magicamente!`, 'Fechar', { duration: 3000 });
      } else {
        this.snackBar.open(`🤖 Assistente: ${resposta.mensagem}`, 'Entendi', { duration: 6000 });
      }
    },
    error: (err: HttpErrorResponse) => {
      this.processandoIa.set(false);
      const mensagemErro = err.error?.erro || 'Erro na comunicação com a IA.';
      this.snackBar.open(`Erro: ${mensagemErro}`, 'Fechar', { duration: 5000 });
    }
  });
}
}
