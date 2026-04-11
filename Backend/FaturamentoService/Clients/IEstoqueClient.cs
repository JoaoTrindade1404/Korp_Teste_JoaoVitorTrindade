namespace FaturamentoService.Clients;

public interface IEstoqueClient
{
    Task DarBaixaEstoqueAsync(RequisicaoBaixaDTO requisicao);
}

public record PedidoBaixaEstoqueDTO(Guid Id, int Quantidade);

public record ErroRespostaDTO(string Erro);

public record RequisicaoBaixaDTO(Guid NotaFiscalId, List<PedidoBaixaEstoqueDTO> Itens);

