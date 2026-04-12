namespace FaturamentoService.DTOs;

public class NotaFiscalResponseDTO
{
    public Guid Id { get; set; }
    public int NumeroSequencial { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ItemNotaFiscalResponseDTO> Itens {get; set;} = [];
}