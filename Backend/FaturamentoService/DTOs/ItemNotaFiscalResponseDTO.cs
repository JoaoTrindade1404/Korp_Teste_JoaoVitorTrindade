namespace FaturamentoService.DTOs;

public class ItemNotaFiscalResponseDTO {
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
}