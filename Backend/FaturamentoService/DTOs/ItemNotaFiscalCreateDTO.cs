using System.ComponentModel.DataAnnotations;

namespace FaturamentoService.DTOs;

public class ItemNotaFiscalCreateDTO
{
    [Required(ErrorMessage = "O ID do produto é obrigatório")]
    public Guid ProdutoId { get; set; }

    [Required(ErrorMessage = "A quantidade não pode ser vazia")]
    [Range(1, 999, ErrorMessage = "A quantidade não pode ser menor que 1 ou maior que 999")]
    public int Quantidade { get; set; }
}