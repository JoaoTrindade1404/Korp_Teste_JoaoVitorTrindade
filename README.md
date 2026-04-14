# Sistema de Emissão de Notas Fiscais

> Desafio Técnico — Korp | Candidato: João Vitor Trindade

Solução Full-Stack para emissão e gestão de notas fiscais, construída sobre uma arquitetura de **microsserviços independentes**, com foco em resiliência, integridade de dados e experiência do usuário.

---

## Visão Geral da Arquitetura

```
┌─────────────────────────────────┐
│        Angular 21 (SPA)         │  localhost:4200
│  Signals · Material · Tailwind  │
└──────────────┬──────────────────┘
               │ HTTP REST
       ┌───────┴────────┐
       │                │
┌──────▼──────┐  ┌──────▼──────┐
│   Estoque   │  │ Faturamento │
│  Service    │◄─┤   Service   │
│  :5225      │  │   :5010     │
│  estoque.db │  │ faturamento │
└─────────────┘  │    .db      │
                 └─────────────┘
```

Dois microsserviços isolados, cada um com seu próprio banco de dados SQLite. O Faturamento orquestra a comunicação com o Estoque durante a impressão de notas.

---

## Stack de Tecnologias

### Backend

| Tecnologia | Versão | Função |
|---|---|---|
| C# / .NET | 10.0 | Runtime e SDK principal |
| ASP.NET Core Minimal APIs | 10.0 | Endpoints HTTP dos microsserviços |
| Entity Framework Core | 10.0.5 | ORM Code-First com Fluent API |
| EF Core SQLite | 10.0.5 | Persistência física isolada por serviço |
| Microsoft.Extensions.Http.Polly | 10.0.5 | Resiliência HTTP com exponential backoff |

### Frontend

| Tecnologia | Versão | Função |
|---|---|---|
| Angular | 21.x | Framework SPA (100% Standalone Components) |
| Angular Material | 21.x | Componentes visuais (tabelas, forms, snackbar) |
| Tailwind CSS | 4.x | Estilização utilitária e layout |
| Lucide Angular | 1.x | Ícones SVG modernos |
| RxJS | 7.8 | Comunicação HTTP reativa |

---

## Funcionalidades

- **Gestão de Produtos** — CRUD completo com paginação server-side, busca e validação inline
- **Emissão de Notas Fiscais** — Criação com múltiplos itens e numeração sequencial automática
- **Faturamento** — Impressão com atualização de status e baixa automática de estoque
- **Inteligência Artificial** — Adição de itens à nota por linguagem natural via Google Gemini
- **Indicador de processamento** — Spinner por nota individual durante o faturamento

---

## Diferenciais Técnicos

### Resiliência de Rede — Exponential Backoff
Se o Serviço de Estoque ficar indisponível durante o faturamento, o sistema não quebra. O **Polly** retenta a requisição 3 vezes com espera progressiva (2s → 4s → 8s). Após esgotar as tentativas, a nota **permanece Aberta** e o usuário recebe feedback claro — sem corrupção de dados.

### Concorrência Otimista — RowVersion
A entidade `Produto` possui um campo `[Timestamp]` que o EF Core mapeia como `RowVersion` no banco. Dois faturamentos simultâneos do mesmo produto geram um `DbUpdateConcurrencyException` → 409 Conflict → retry automático pelo Polly. O segundo faturamento é executado com sucesso na tentativa seguinte.

### Idempotência — Transações Processadas
O Serviço de Estoque mantém uma tabela `TransacoesProcessadas` com o `NotaFiscalId` como chave primária. Antes de deduzir qualquer estoque, verifica se aquela nota já foi processada. Retries de rede, duplo-clique ou reenvios acidentais nunca duplicam a baixa de estoque.

### Rich Domain Model
As entidades encapsulam suas próprias regras de negócio com setters privados:

```csharp
// O saldo não pode ficar negativo — regra protegida pela própria entidade
public void BaixarEstoque(int quantidade)
{
    if (Saldo < quantidade) throw new RegraDeNegocioException($"Estoque insuficiente...");
    Saldo -= quantidade;
}
```

Adicionalmente, o banco possui um `Check Constraint` (`Saldo >= 0`) como última linha de defesa.

### Tratamento de Erros em 3 Camadas
1. **Entidade** — lança exceção semântica (`RegraDeNegocioException`, `KeyNotFoundException`, `InvalidOperationException`)
2. **GlobalExceptionMiddleware** — intercepta e converte para HTTP com status code correto (404, 409, 422, 500)
3. **Frontend** — lê o campo `{ "erro": "..." }` da resposta e exibe via `MatSnackBar`

### Inteligência Artificial — Google Gemini
```
Usuário digita: "Quero 3 teclados e 2 mouses"
              ↓
     Gemini recebe texto + catálogo de produtos
              ↓
     Retorna JSON com IDs e quantidades mapeados
              ↓
     Itens adicionados automaticamente à nota
```
Em caso de ambiguidade (ex: "um teclado" com múltiplos modelos cadastrados), a IA **recusa a operação** e solicita esclarecimento — nunca adivinha. `temperature: 0` garante respostas determinísticas.

---

## Como Rodar Localmente

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`

---

### Passo 1 — Configurar a chave da API Gemini (para usar a feature de IA)

A chave é opcional para rodar o projeto — apenas a funcionalidade de IA ficará indisponível sem ela.

1. Crie uma chave gratuita em [Google AI Studio](https://aistudio.google.com/app/apikey)
2. Navegue até `Backend/FaturamentoService/`
3. Crie o arquivo `appsettings.Development.json`:

```json
{
  "GeminiApiKey": "SUA_CHAVE_AQUI"
}
```

---

### Passo 2 — Iniciar o Serviço de Estoque

```bash
cd Backend/EstoqueService
dotnet run
# Rodando em http://localhost:5225
```

---

### Passo 3 — Iniciar o Serviço de Faturamento

```bash
cd Backend/FaturamentoService
dotnet run
# Rodando em http://localhost:5010
```

> Os bancos de dados SQLite (`estoque.db` e `faturamento.db`) são criados automaticamente no primeiro startup via `EnsureCreated()`.

---

### Passo 4 — Iniciar o Frontend

```bash
cd Frontend/front-fiscal
npm install
ng serve
# Disponível em http://localhost:4200
```

---

## Estrutura do Repositório

```
Korp_Teste_JoaoVitorTrindade/
├── Backend/
│   ├── EstoqueService/         # Microsserviço de Estoque (porta 5225)
│   │   ├── Entities/           # Produto, TransacaoProcessada (Rich Domain)
│   │   ├── Services/           # IProdutoService, ProdutoService
│   │   ├── DTOs/               # ProdutoCreateDTO, ProdutoResponseDTO, PagedResultDTO
│   │   ├── Data/               # EstoqueDbContext (Fluent API)
│   │   ├── Middlewares/        # GlobalExceptionMiddleware
│   │   ├── Exceptions/         # RegraDeNegocioException
│   │   └── Program.cs          # Minimal API endpoints
│   │
│   └── FaturamentoService/     # Microsserviço de Faturamento (porta 5010)
│       ├── Entities/           # NotaFiscal, ItemNotaFiscal
│       ├── Services/           # INotaFiscalService, NotaFiscalService, IAService
│       ├── Clients/            # IEstoqueClient, EstoqueClient (Polly)
│       ├── DTOs/               # NotaFiscalCreateDTO, NotaFiscalResponseDTO, IADTOs
│       ├── Data/               # FaturamentoDbContext
│       ├── Middlewares/        # GlobalExceptionMiddleware
│       └── Program.cs          # Minimal API endpoints
│
└── Frontend/
    └── front-fiscal/
        └── src/app/
            ├── core/
            │   ├── models/     # Interfaces TypeScript (DTOs espelhados)
            │   └── services/   # EstoqueService, FaturamentoService
            └── features/
                ├── produtos/   # Lista, criação e edição de produtos
                └── notas/      # Lista de notas e criação de nota (com IA)
```

---

## Requisitos Implementados

### Obrigatórios

- [x] Cadastro de Produtos (Código, Descrição, Saldo)
- [x] Cadastro de Notas Fiscais com numeração sequencial e status inicial `Aberta`
- [x] Inclusão de múltiplos produtos com quantidades
- [x] Impressão com indicador de processamento (spinner)
- [x] Status atualizado para `Fechada` após impressão
- [x] Bloqueio de reimpressão de notas já fechadas (frontend + backend)
- [x] Dedução de saldo dos produtos ao imprimir
- [x] Arquitetura de microsserviços (mínimo 2 serviços)
- [x] Banco de dados físico com persistência real
- [x] Tratamento de falha de microsserviço com recuperação e feedback ao usuário

### Opcionais

- [x] **Tratamento de Concorrência** — Concorrência Otimista com `[Timestamp]` RowVersion
- [x] **Uso de Inteligência Artificial** — Google Gemini para extração de itens por linguagem natural
- [x] **Idempotência** — Tabela `TransacoesProcessadas` imuniza contra dedução duplicada de estoque

---

*Desenvolvido por João Vitor Trindade para o processo seletivo Korp.*
