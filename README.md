# 📦 Microsserviço de Estoque (Teste Korp)

Projeto backend construído para o gerenciamento de estoque, focado no cadastro de produtos e processamento de notas fiscais.

## 🚀 Tecnologias Utilizadas

* **C# / .NET** (Arquitetura de Minimal APIs)
* **Entity Framework Core** (ORM)
* **SQLite** (Banco de dados relacional leve)

## ⚙️ Funcionalidades Atuais

* **Cadastro de Produtos (`POST /produtos`)**: Permite cadastrar um novo produto com Código, Descrição e Saldo inicial. O sistema já conta com validação para impedir códigos duplicados.

## 🛠️ Como rodar o projeto localmente

1. Certifique-se de ter o [.NET SDK](https://dotnet.microsoft.com/) instalado na sua máquina.
2. Clone este repositório.
3. Abra o terminal e navegue até a pasta do microsserviço:
   ```bash
   cd Backend/EstoqueService
   ```
4. Inicie o servidor:
   ```bash
   dotnet run
   ```
5. O Entity Framework irá criar automaticamente o banco de dados estoque.db na primeira execução.

Desenvolvido por João Vitor Trindade.
