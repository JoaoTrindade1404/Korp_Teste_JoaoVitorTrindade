using System.ComponentModel.DataAnnotations;

namespace FaturamentoService.DTOs;

public class ItemNotaFiscalCreateDTO
{
    [Required(ErrorMessage = "O ID do produto é obrigatório")]
    public Guid ProdutoId { get; set; }

    [Range(1, 999, ErrorMessage = "A quantidade deve ser entre 1 e 999")]
    public int Quantidade { get; set; }

    [Required(ErrorMessage = "Nome do produto é obrigatório")]
    [StringLength(150, ErrorMessage = "Nome do produto não pode ter mais de 150 caracteres")]
    public string NomeProduto { get; set; } = string.Empty;
}