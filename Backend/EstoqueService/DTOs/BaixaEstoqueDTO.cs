using System.ComponentModel.DataAnnotations;

namespace EstoqueService.DTOs;

public class BaixaEstoqueDTO
{
    [Required(ErrorMessage = "ID do produto é obrigatório")]
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que 0")]
    public int Quantidade { get; set; }
}