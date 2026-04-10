namespace FaturamentoService.Clients;

public class EstoqueClient : IEstoqueClient
{
    private readonly HttpClient _httpClient;

    public EstoqueClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task DarBaixaEstoqueAsync(List<PedidoBaixaEstoqueDTO> itens)
    {
        var response = await _httpClient.PatchAsJsonAsync("/produtos/baixar-estoque", itens);

        response.EnsureSuccessStatusCode();
    }
}