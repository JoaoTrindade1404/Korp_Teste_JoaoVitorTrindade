# 📦 Sistema de Emissão de Notas Fiscais (Teste Korp)

Este projeto é uma solução Full-Stack desenvolvida para o desafio técnico da Korp. A aplicação apresenta um ecossistema robusto estruturado em **Microsserviços**, planejado não apenas para atender às regras de negócio de um ERP padrão, mas focado em **Prevenção de Falhas, Resiliência, Integridade de Dados em Ambientes Distribuídos e Experiência do Usuário (UX)**.

---

## 🚀 Tecnologias Utilizadas

### Backend (Microsserviços)
* **C# / .NET 8:** Minimal APIs e Arquitetura Limpa.
* **Entity Framework Core & SQLite:** Bancos de dados físicos isolados por serviço.
* **Polly:** Biblioteca para políticas de resiliência e tratamento de falhas transientes de rede.
* **Integração IA:** Google Gemini API.

### Frontend (SPA)
* **Angular 14+:** Arquitetura 100% *Standalone Components* (sem NgModules).
* **Signals:** Gerenciamento de estado reativo e moderno.
* **Angular Material & Reactive Forms:** Componentes visuais fluidos e validações rigorosas *client-side*.

---

## 🛡️ Destaques Técnicos e Engenharia Avançada

O projeto foi além do "caminho feliz", aplicando padrões de arquitetura de software para resolver problemas do mundo real:

### 1. Separação de Contextos (DDD)
O sistema está dividido em dois domínios isolados que se comunicam via HTTP:
* **Serviço de Estoque (`localhost:5225`):** Gerencia inventário e protege saldos com restrições rigorosas (Check Constraints).
* **Serviço de Faturamento (`localhost:5010`):** Orquestra o ciclo de vida das Notas Fiscais e a comunicação distribuída.

### 2. Resiliência de Rede (Exponential Backoff)
Caso o Serviço de Estoque fique indisponível durante a impressão de uma nota, o sistema não quebra. Através do **Polly**, a requisição é interceptada e retentada 3 vezes com espaçamento progressivo de tempo. Se a falha persistir, a aplicação falha graciosamente com um *feedback* claro para o usuário.

### 3. Tratamento de Concorrência (Optimistic Concurrency)
Para o cenário crítico onde um produto com saldo unitário é disputado simultaneamente por duas notas, implementamos *Concorrência Otimista* com `[Timestamp]` (RowVersion). O banco assegura a trava lógica e evita inconsistências físicas sem onerar a performance com locks pessimistas.

### 4. Idempotência e Transações Seguras
O Estoque possui uma tabela de controle de **Transações Processadas**. Isso imuniza o sistema contra duplicação acidental de baixas de estoque causadas por retentativas de rede (*retry*) ou cliques duplos no frontend, garantindo que o saldo nunca seja deduzido em dobro para uma mesma Nota Fiscal.

### 5. ✨ Digitação Inteligente com IA (Diferencial)
Na tela de criação de Notas Fiscais, foi integrada uma funcionalidade de **Inteligência Artificial** (via Google Gemini) capaz de ler pedidos em linguagem natural (ex: *"Quero 2 teclados e 1 mouse"*) e extrair o payload JSON exato com os IDs e quantidades. 
A engenharia de prompt foi estritamente configurada para **tratar ambiguidades**: se o usuário pedir "um teclado" e existirem múltiplos modelos no banco, a IA recusa a operação e devolve uma pergunta para esclarecimento, garantindo que o inventário nunca sofra baixas incorretas.

---

## 🛠️ Como rodar o projeto localmente

### Pré-requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/)
* [Node.js](https://nodejs.org/) (v18+) e Angular CLI

### 🤖 Passo 1: Configuração da IA (Obrigatório para testar a funcionalidade com ia)
Para que a extração inteligente de itens funcione, é necessário inserir uma chave da API do Google Gemini.
1. Crie uma chave gratuita no [Google AI Studio](https://aistudio.google.com/app/apikey).
2. Vá até o diretório `Backend/FaturamentoService/`.
3. Abra ou crie o arquivo `appsettings.Development.json` e adicione a chave:
```json
{
  "GeminiApiKey": "COLE_SUA_CHAVE_AQUI"
}
