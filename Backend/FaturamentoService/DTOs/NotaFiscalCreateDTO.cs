using System.ComponentModel.DataAnnotations;

namespace FaturamentoService.DTOs;

public class NotaFiscalCreateDTO
{
    [Required(ErrorMessage = "A nota fiscal não pode ficar vazia")]
    [MinLength(1, ErrorMessage = "A nota fiscal deve conter no minimo um item")]
    public List<ItemNotaFiscalCreateDTO> Itens { get; set; } = [];
}