using System.Text.Json;
using FaturamentoService.DTOs;

namespace FaturamentoService.Services;

public class IAService : IIAService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public IAService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["GeminiApiKey"] ?? string.Empty;
    }

    public async Task<RespostaIADTO> ExtrairItensDoTextoAsync(TextoUsuarioIADTO request)
    {
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "COLE_SUA_CHAVE_AQUI")
        {
            throw new InvalidOperationException("Chave da API do Gemini não configurada no appsettings.json.");
        }

        var catalogoTexto = string.Join("\n", request.ProdutosDisponiveis
            .Select(p => $"- ID: {p.Id} | Nome: {p.Descricao}"));

        var prompt = $@"
            Você é um assistente de faturamento estrito. Mapeie o pedido do usuário EXATAMENTE para os IDs dos produtos disponíveis.

            PRODUTOS DISPONÍVEIS:
            {catalogoTexto}

            PEDIDO DO USUÁRIO:
            ""{request.Texto}""

            REGRAS:
            1. Correspondência Exata: Se o pedido for claro, extraia o ID e a quantidade e defina 'sucesso' como true.
            2. Ambiguidade: Se o usuário pedir um termo genérico (ex: 'teclado') e existirem múltiplos produtos correspondentes, NÃO adivinhe. Defina 'sucesso' como false e em 'mensagem' faça uma pergunta curta ao usuário citando as opções disponíveis.

            Retorne EXCLUSIVAMENTE um objeto JSON neste formato exato (sem formatação markdown):
            {{
            ""sucesso"": true,
            ""mensagem"": """",
            ""itens"": [ {{ ""produtoId"": ""guid"", ""quantidade"": 1 }} ]
            }}";

        var geminiRequest = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new
            {
                responseMimeType = "application/json",
                temperature = 0.0
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
        
        var response = await _httpClient.PostAsJsonAsync(url, geminiRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            var erroDoGoogle = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"\n--- ERRO DO GEMINI ---\nStatus: {response.StatusCode}\nDetalhes: {erroDoGoogle}\n----------------------\n");
            
            throw new InvalidOperationException($"O Google recusou a chamada. Status: {response.StatusCode}");
        }

        var geminiResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        try
        {
            var jsonResposta = geminiResult.GetProperty("candidates")[0]
                .GetProperty("content").GetProperty("parts")[0]
                .GetProperty("text").GetString();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var respostaIA = JsonSerializer.Deserialize<RespostaIADTO>(jsonResposta!, options);
            
            return respostaIA ?? throw new Exception("Deserialização retornou nulo");
        }
        catch
        {
            throw new InvalidOperationException("A Inteligência Artificial retornou um formato inesperado.");
        }
    }
}