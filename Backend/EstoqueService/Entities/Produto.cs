using System.ComponentModel.DataAnnotations;
using EstoqueService.Exceptions;


namespace EstoqueService.Entities;

public class Produto
{
    public Guid Id { get; private set; } 
    
    public string Codigo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public int Saldo { get; private set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public Produto(string codigo, string descricao, int saldo)
    {
        if (saldo < 0) throw new ArgumentException("O saldo inicial não pode ser menor que 0");

        Id = Guid.NewGuid();
        Codigo = codigo;
        Descricao = descricao;
        Saldo = saldo;
    }

    protected Produto() {}

    public void AtualizarDetalhes(string codigo, string descricao, int saldo)
    {
        if (saldo < 0) throw new ArgumentException("O saldo não pode ser negativo.");

        Codigo = codigo;
        Descricao = descricao;
        Saldo = saldo;
    }

    public void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException($"Quantidade inválida.");
        if (Saldo < quantidade) throw new RegraDeNegocioException($"Estoque insuficiente do produto {Descricao}. Estoque atual: {Saldo}");
        Saldo -= quantidade;
    }
}