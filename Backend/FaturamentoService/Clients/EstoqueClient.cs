namespace FaturamentoService.Clients;

public class EstoqueClient : IEstoqueClient
{
    private readonly HttpClient _httpClient;

    public EstoqueClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task DarBaixaEstoqueAsync(RequisicaoBaixaDTO requisicao)
    {
        var response = await _httpClient.PatchAsJsonAsync("/produtos/baixar-estoque", requisicao);

        if (!response.IsSuccessStatusCode)
        {
            var respostaDeErro = await response.Content.ReadFromJsonAsync<ErroRespostaDTO>(); 
            
            throw new InvalidOperationException(respostaDeErro?.Erro ?? "Erro desconhecido do Estoque."); 
        }
    }
}