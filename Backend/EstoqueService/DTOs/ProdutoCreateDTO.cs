using System.ComponentModel.DataAnnotations;

namespace EstoqueService.DTOs;

public class ProdutoCreateDTO
{
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(20, ErrorMessage = "O código não pode ter mais que 20 caracteres.")]
    public string Codigo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Descrição é obrigatório")]
    [StringLength(150, ErrorMessage = "O código não pode ter mais que 150 caracteres.")]
    public string Descricao { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Saldo é obrigatório")]
    [Range(0, 10000, ErrorMessage = "O saldo inicial não pode ser negativo e deve ser no máximo 10.000.")]
    public int Saldo { get; set; }
}