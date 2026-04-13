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
        try
        {
            var response = await _httpClient.PatchAsJsonAsync("/produtos/baixar-estoque", requisicao);

            if (!response.IsSuccessStatusCode)
            {
                var respostaDeErro = await response.Content.ReadFromJsonAsync<ErroRespostaDTO>(); 
            
                throw new InvalidOperationException(respostaDeErro?.Erro ?? "Erro desconhecido do Estoque."); 
            }
        }
        catch(TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new InvalidOperationException("Timeout: Serviço de Estoque não respondeu em 30 segundos.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Erro de conexão com o Serviço de Estoque.", ex);
        }
        
    }
}