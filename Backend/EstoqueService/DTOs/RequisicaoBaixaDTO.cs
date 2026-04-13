using System.ComponentModel.DataAnnotations;

namespace EstoqueService.DTOs;

public class RequisicaoBaixaDTO
{
    [Required(ErrorMessage = "ID da nota fiscal é obrigatório")]
    public Guid NotaFiscalId { get; set; }

    [Required(ErrorMessage = "Itens não podem estar vazios")]
    [MinLength(1, ErrorMessage = "Deve conter no mínimo 1 item")]
    public List<BaixaEstoqueDTO> Itens { get; set; } = [];
}