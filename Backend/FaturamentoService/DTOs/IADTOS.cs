namespace FaturamentoService.DTOs;

public record TextoUsuarioIADTO(string Texto, List<ProdutoSimplesDTO> ProdutosDisponiveis);

public record ProdutoSimplesDTO(Guid Id, string Descricao);

public class ItemExtraidoIADTO 
{ 
    public Guid ProdutoId { get; set; } 
    public int Quantidade { get; set; } 
}

public class RespostaIADTO 
{ 
    public bool Sucesso { get; set; } 
    public string Mensagem { get; set; } = string.Empty;
    public List<ItemExtraidoIADTO> Itens { get; set; } = []; 
}