namespace FaturamentoService.Entities;

public enum StatusNota
{
    Aberta = 1,
    Fechada = 0
}

public class NotaFiscal
{
    public Guid Id { get; private set; }
    public int NumeroSequencial { get; private set; } 
    public StatusNota Status { get; private set; }
    public DateTime DataEmissao { get; private set; }


    private readonly List<ItemNotaFiscal> _itens = [];
    public IReadOnlyCollection<ItemNotaFiscal> Itens => _itens.AsReadOnly();

    public NotaFiscal (int numeroSequencial)
    {
        Id = Guid.NewGuid();
        NumeroSequencial = numeroSequencial;
        Status = StatusNota.Aberta;
        DataEmissao = DateTime.UtcNow;
    }

    protected NotaFiscal() {}

    public void AdicionarItem(Guid produtoId, int quantidade, string nomeProduto)
    {
        _itens.Add(new ItemNotaFiscal(produtoId, quantidade, nomeProduto));
    }

    public void ImprimirNota()
    {
        if (Status == StatusNota.Fechada) throw new InvalidOperationException("Uma nota fechada não pode ser impressa");

        Status = StatusNota.Fechada;
    }
}