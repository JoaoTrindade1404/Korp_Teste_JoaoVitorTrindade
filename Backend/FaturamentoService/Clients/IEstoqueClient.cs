namespace FaturamentoService.Clients;

public record PedidoBaixaEstoqueDTO(Guid ProdutoId, int Quantidade);

public interface IEstoqueClient
{
    Task DarBaixaEstoqueAsync(List<PedidoBaixaEstoqueDTO> itens);
}