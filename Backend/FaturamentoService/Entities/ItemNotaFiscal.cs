namespace FaturamentoService.Entities;

public class ItemNotaFiscal
{
    public Guid Id { get; private set; }

    public Guid ProdutoId { get; private set; }
    public int Quantidade { get; private set; }

    public ItemNotaFiscal (Guid produtoId, int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("A quantidade inicial não pode ser menor que 0");

        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        Quantidade = quantidade;
    }

    protected ItemNotaFiscal() {}

}