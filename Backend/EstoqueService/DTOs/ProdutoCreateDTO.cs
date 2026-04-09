using System.ComponentModel.DataAnnotations;

namespace EstoqueService.DTOs;

public class ProdutoCreateDTO
{
    [Required(ErrorMessage = "Código é obrigatório")]
    public string Codigo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Descrição é obrigatório")]
    public string Descricao { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Saldo é obrigatório")]
    public int Saldo { get; set; }
}