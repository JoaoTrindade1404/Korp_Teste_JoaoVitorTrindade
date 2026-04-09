using EstoqueService.DTOs;

namespace EstoqueService.Services;

public interface IProdutoService
{
    Task<ProdutoResponseDTO> CadastrarProdutoAsync(ProdutoCreateDTO dto);
}