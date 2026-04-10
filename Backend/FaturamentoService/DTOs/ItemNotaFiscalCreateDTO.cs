using System.ComponentModel.DataAnnotations;

namespace FaturamentoService.DTOs;

public class ItemNotaFiscalCreateDTO
{
    [Required]
    public Guid ProdutoId { get; set; }

    [Required]
    [Range(1, 999)]
    public int Quantidade { get; set; }
}