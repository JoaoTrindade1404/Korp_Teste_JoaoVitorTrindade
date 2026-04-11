namespace FaturamentoService.Clients;

public record PedidoBaixaEstoqueDTO(Guid ProdutoId, int Quantidade);

public record ErroRespostaDTO(string Erro);

public interface IEstoqueClient
{
    Task DarBaixaEstoqueAsync(List<PedidoBaixaEstoqueDTO> itens);
}