# Detalhamento Técnico — Sistema de Emissão de Notas Fiscais

**Candidato:** João Vitor Trindade  
**Repositório:** `Korp_Teste_JoaoVitorTrindade`

---

## 1. Ciclos de Vida do Angular Utilizados

### `ngOnInit`
Utilizado em todos os componentes do projeto para inicialização de dados:

- **`ListaProdutos`** — Carrega a lista paginada de produtos do Serviço de Estoque e inicializa o formulário reativo com as validações.
- **`ListaNotas`** — Carrega a lista paginada de notas fiscais do Serviço de Faturamento.
- **`CriarNota`** — Carrega os produtos disponíveis no Serviço de Estoque para popular o dropdown de seleção de itens na criação da nota.

O `ngOnInit` foi o ciclo de vida principal por ser o ponto adequado para disparar chamadas HTTP — diferente do `constructor`, que é reservado para injeção de dependências. Os componentes não necessitaram de `ngOnDestroy` pois as subscriptions HTTP são finitas (completam automaticamente após a resposta), eliminando risco de memory leaks.

---

## 2. Uso da Biblioteca RxJS

O **RxJS** é utilizado em toda a camada de comunicação HTTP do projeto, pois o `HttpClient` do Angular retorna `Observables` nativamente.

### Como foi utilizado:

- **`Observable<T>`** — Todos os métodos dos services (`EstoqueService` e `FaturamentoService`) retornam `Observable<T>`, mantendo o padrão reativo do Angular.
- **`.subscribe()`** — Utilizado nos componentes para consumir as respostas HTTP, com tratamento separado dos callbacks `next` e `error`.
- **Tipagem genérica** — `Observable<PagedResult<ProdutoResponseDTO>>` e `Observable<NotaFiscalResponseDTO>` para garantir type-safety end-to-end.

### Decisão arquitetural:
Para o estado local dos componentes (listas, flags de loading, filtros), optei por utilizar os **Signals** do Angular (API reativa introduzida no Angular 16+, estável a partir do 17) em vez de `BehaviorSubject` do RxJS. O RxJS ficou responsável exclusivamente pela comunicação assíncrona (HTTP), enquanto os Signals gerenciam o estado reativo da UI — como `signal()` para dados e `computed()` para valores derivados (ex: lista filtrada por termo de busca).

---

## 3. Outras Bibliotecas Utilizadas e Finalidade

### Frontend

| Biblioteca | Versão | Finalidade |
|---|---|---|
| **Angular** | 21.x | Framework principal do frontend (SPA) |
| **Angular Material** | 21.x | Componentes visuais (tabelas, formulários, botões, paginador, snackbar, tooltips) |
| **Tailwind CSS** | 4.x | Estilização utilitária para layout, espaçamento, cores e responsividade |
| **Lucide Angular** | 1.x | Biblioteca de ícones SVG modernos e leves |
| **RxJS** | 7.8 | Programação reativa para comunicação HTTP |

### Backend

| Biblioteca / Pacote NuGet | Versão | Finalidade |
|---|---|---|
| **.NET** | 10.0 | Runtime e SDK principal |
| **Entity Framework Core** | 10.0.5 | ORM para acesso ao banco de dados |
| **EF Core SQLite** | 10.0.5 | Provider do Entity Framework para SQLite |
| **Microsoft.Extensions.Http.Polly** | 10.0.5 | Políticas de resiliência (retry com exponential backoff) para comunicação HTTP entre microsserviços |

---

## 4. Componentes Visuais — Bibliotecas Utilizadas

A interface foi construída com a combinação de **Angular Material** e **Tailwind CSS**:

- **Angular Material** — Componentes estruturais: `MatTable` (tabelas de dados), `MatPaginator` (paginação), `MatFormField` + `MatInput` (formulários com validação visual), `MatSnackBar` (notificações/toasts), `MatButton` (botões com estilos Material Design), `MatTooltip` (dicas em hover), `MatSelect` (dropdown de seleção), `MatCard` (cards).
- **Tailwind CSS** — Utilizado para toda a estilização de layout: sidebar com dark theme, espaçamentos, cores customizadas, gradientes, estados hover, animações (como `animate-spin` para loading e `animate-pulse` para indicador de backend online), responsividade e design system geral.
- **Lucide Angular** — Ícones SVG em toda a interface (ações da tabela, sidebar, botões, badges de status).

---

## 5. Frameworks Utilizados no C#

- **ASP.NET Core** com **Minimal APIs** — Escolhi Minimal APIs (em vez de Controllers tradicionais) por ser um projeto de microsserviços focados, onde cada endpoint é simples e direto. As Minimal APIs reduzem a cerimônia de código sem sacrificar funcionalidade.
- **Entity Framework Core** — ORM com abordagem Code-First. As entidades definem a estrutura do banco, e o `EnsureCreated()` gera as tabelas automaticamente no primeiro startup. Configurações avançadas (índices únicos, check constraints, row versioning) foram definidas via Fluent API no `OnModelCreating`.

---

## 6. Tratamento de Erros e Exceções no Backend

O tratamento de erros segue uma arquitetura em **3 camadas**:

### Camada 1: Exceções Semânticas nas Entidades e Services
Cada tipo de erro lança uma exceção .NET específica que carrega significado semântico:

| Exceção | Quando é lançada | Exemplo |
|---|---|---|
| `KeyNotFoundException` | Recurso não encontrado | Produto ou Nota com ID inexistente |
| `ArgumentException` | Validação de entrada | Quantidade ≤ 0, saldo negativo |
| `InvalidOperationException` | Conflito ou operação inválida | Código de produto duplicado, nota já fechada, serviço indisponível |
| `RegraDeNegocioException` (customizada) | Violação de regra de negócio | Estoque insuficiente para a baixa |
| `DbUpdateConcurrencyException` | Conflito de concorrência (RowVersion) | Dois usuários alterando o mesmo produto |

### Camada 2: Middleware Global de Exceções (`GlobalExceptionMiddleware`)
Intercepta todas as exceções não tratadas e as converte em respostas HTTP padronizadas com status codes semânticos:

| Exceção | Status HTTP | Significado |
|---|---|---|
| `BadHttpRequestException` (JSON inválido) | `400 Bad Request` | Corpo da requisição malformado |
| `KeyNotFoundException` | `404 Not Found` | Recurso não existe |
| `InvalidOperationException` | `409 Conflict` | Conflito de estado ou operação |
| `RegraDeNegocioException` | `422 Unprocessable Entity` | Regra de negócio violada |
| `Exception` (qualquer outra) | `500 Internal Server Error` | Erro inesperado do servidor |

Todas as respostas de erro seguem o formato JSON padronizado: `{ "erro": "mensagem descritiva" }`.

### Camada 3: Feedback ao Usuário no Frontend
O Angular intercepta os erros HTTP e exibe notificações via `MatSnackBar`, extraindo a mensagem do campo `erro` da resposta JSON do backend.

---

## 7. Uso de LINQ

O LINQ foi utilizado extensivamente em todo o backend. Exemplos concretos:

### Consultas ao banco de dados (LINQ to Entities via EF Core):
- **`AnyAsync()`** — Verificar se existem registros (ex: verificar se já existe nota fiscal antes de calcular o `MaxAsync`)
- **`MaxAsync()`** — Obter o maior número sequencial para gerar o próximo
- **`FirstOrDefaultAsync()` com predicado** — Buscar entidades por ID com possibilidade de null
- **`CountAsync()`** — Contagem total para paginação
- **`Skip() / Take()`** — Implementação de paginação server-side
- **`OrderBy()`** — Ordenação para paginação estável e previsível
- **`Include()`** — Eager loading de relacionamentos (Itens da Nota Fiscal)

### Projeções e transformações (LINQ to Objects):
- **`Select()`** — Projeção de entidades para DTOs diretamente nas queries, evitando carregar dados desnecessários em memória
- **`ToListAsync()`** — Materialização assíncrona das queries
- **`Where()` / `filter()`** — Filtragem de coleções
- **Null-coalescing** — `?? throw new KeyNotFoundException(...)` como padrão fluente para validar existência

### Exemplo representativo:
```csharp
var items = await query
    .OrderBy(n => n.NumeroSequencial)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(n => new NotaFiscalResponseDTO
    {
        Id = n.Id,
        NumeroSequencial = n.NumeroSequencial,
        Status = n.Status.ToString(),
    }).ToListAsync();
```

---

## 8. Requisitos Opcionais Implementados

### a. Tratamento de Concorrência
Implementação de **Concorrência Otimista** utilizando o atributo `[Timestamp]` (RowVersion) na entidade `Produto`. Quando duas requisições tentam alterar o estoque do mesmo produto simultaneamente, o banco detecta que o `RowVersion` mudou e lança `DbUpdateConcurrencyException`. O middleware mapeia isso para um HTTP `409 Conflict`, e a política **Polly** no Serviço de Faturamento automaticamente retenta a requisição, permitindo que a segunda operação seja executada com sucesso na próxima tentativa.

### b. Uso de Inteligência Artificial
Integração com a **API Gemini** (Google) para extração inteligente de itens a partir de texto livre digitado pelo usuário. O usuário pode escrever algo como "2 teclados e 3 mouses" e a IA mapeia automaticamente para os produtos cadastrados no sistema, criando os itens da nota fiscal. O sistema trata ambiguidade — se existirem múltiplos produtos similares, a IA solicita esclarecimento ao invés de adivinhar.

### c. Implementação de Idempotência
O Serviço de Estoque mantém uma tabela de **TransacoesProcessadas** que registra o ID de cada nota fiscal cujo estoque já foi baixado. Antes de processar uma baixa, o serviço verifica se aquele ID já foi processado — se sim, retorna sucesso silenciosamente sem duplicar a dedução de estoque. Isso protege contra retries de rede, duplo-clique do usuário e reenvios acidentais.
