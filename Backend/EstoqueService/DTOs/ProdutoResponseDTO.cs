namespace EstoqueService.DTOs;

public class ProdutoResponseDTO
{
    public Guid Id { get; set; }
    
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    
    public int Saldo { get; set; }
}