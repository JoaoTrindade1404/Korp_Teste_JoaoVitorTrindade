namespace EstoqueService.Entities;

public class TransacaoProcessada
{
    public Guid Id { get; private set; } 
    public DateTime DataProcessamento { get; private set; }

    public TransacaoProcessada(Guid id)
    {
        Id = id;
        DataProcessamento = DateTime.UtcNow;
    }

    protected TransacaoProcessada() {}
}
