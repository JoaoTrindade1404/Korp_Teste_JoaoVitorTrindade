# 📦 ERP Arquitetura de Microsserviços (Teste Korp)

Este projeto apresenta um robusto ecossistema de backend estruturado em **Microsserviços**, planejado não apenas para atender aos requisitos essenciais de regras de negócios, mas focado na **Prevenção de Falhas, Resiliência e Integridade de Dados em Ambientes Distribuídos**.

## 🚀 Tecnologias e Arquitetura

* **C# / .NET 8** (Minimal APIs e Arquitetura Limpa/Pragmática)
* **Entity Framework Core** (ORM) e **SQLite** (Bancos de dados físicos e independentes por serviço)
* **Polly** (Políticas de Resiliência e Exponential Backoff para falhas de rede)

## 🧩 O Ecossistema de Microsserviços

O projeto está dividido em dois domínios isolados que se comunicam via HTTP e Rest:

### 1. Serviço de Estoque (`localhost:5225`)
Gerencia o inventário e protege os saldos contra inconsistências através de validações rigorosas a nível de Banco de Dados.
* **Cadastro de Produtos** (Impedindo códigos duplicados via regras de restrição do EF Core)
* **Dedução de Saldo** (Blindado por check constraints que impedem saldos negativos físicos)

### 2. Serviço de Faturamento (`localhost:5010`)
O orquestrador. Responsável pelo ciclo de vida das Notas Fiscais (Draft/Aberta → Faturamento/Fechada).
* **Cadastro de Notas Fiscais** (Permite adição de itens e geração de sequencial automático)
* **Impressão (Faturamento)** (Processo que interage com o Estoque e possui mecanismo de segurança para garantir a consistência).

---

## 🛡️ Destaques Técnicos e Engenharia Avançada

Este projeto focou pesadamente em aplicar soluções da engenharia de software do mundo real para requisitos desafiadores:

### ⚡ Resiliência e Tratamento de Falhas (Polly)
Caso o Serviço de Estoque fique indisponível (Erro 500 ou Queda de Rede) durante a impressão de uma nota, o Faturamento não repassa o erro para o usuário imediatamente. Utilizamos a biblioteca **Polly** para aplicar uma política de **Exponential Backoff**: a requisição é interceptada e silenciosamente retentada 3 vezes com espaçamento progressivo de tempo (2s, 4s, 8s). Caso as tentativas se esgotem, o sistema falha graciosamente enviando uma notificação limpa para o front-end, preservando o estado da nota.

### 🔒 Tratamento de Concorrência (RowVersion)
Para resolver o temido cenário onde "o mesmo produto é disputado milimetricamente por duas notas ao mesmo tempo", implementamos a **Concorrência Otimista**.
Através do uso de anotações `[Timestamp]` (RowVersion), o banco de dados assegura uma trava lógica. A última requisição a chegar receberá uma notificação `DbUpdateConcurrencyException`, o estoque barrará o processo, e nosso Middleware traduzirá isso num feedback limpo na tela do usuário.

### 🧠 Idempotência e Transações Seguras
O sistema está imune à "duplicação acidental" de baixas de estoque por falhas de rede ou "duplo-clique" frenético do usuário.
O Estoque possui uma tabela interna de **Transações Processadas**. Toda instrução de baixa carrega consigo um rastreador (A Id da Nota Fiscal). Se o Estoque receber o pacote duas vezes, a aplicação acusa `já processado` no banco e finge um sinal de "204 Sucesso" para a requisição clonada. Sem exceções, sem estourar nada, e principalmente, **sem abater saldo em dobro**.

## 🛠️ Como rodar o projeto localmente

1. Certifique-se de ter o [.NET SDK](https://dotnet.microsoft.com/) instalado na sua máquina.
2. Clone este repositório.
3. Serão necessários dois terminais diferentes para rodar o ecossistema.
4. **Terminal 1 (Estoque):**
   ```bash
   cd Backend/EstoqueService
   dotnet run
   ```
5. **Terminal 2 (Faturamento):**
   ```bash
   cd Backend/FaturamentoService
   dotnet run
   ```
*Os arquivos `estoque.db` e `faturamento.db` do SQLite serão criados de forma automática no primeiro disparo das aplicações.*

Desenvolvido por João Vitor Trindade.
