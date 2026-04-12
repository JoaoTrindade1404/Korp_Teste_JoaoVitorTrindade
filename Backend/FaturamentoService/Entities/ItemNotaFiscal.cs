namespace FaturamentoService.Entities;

public class ItemNotaFiscal
{
    public Guid Id { get; private set; }

    public Guid ProdutoId { get; private set; }
    public int Quantidade { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;

    public ItemNotaFiscal (Guid produtoId, int quantidade, string nomeProduto)
    {
        if (quantidade <= 0) throw new ArgumentException("A quantidade inicial não pode ser menor que 0");

        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        Quantidade = quantidade;
        NomeProduto = nomeProduto;
    }

    protected ItemNotaFiscal() {}

}