namespace EstoqueService.DTOs;

public record RequisicaoBaixaDTO(Guid NotaFiscalId, List<BaixaEstoqueDTO> Itens);
