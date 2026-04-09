using EstoqueService.DTOs;

namespace EstoqueService.Services;

public interface IProdutoService
{
    Task<ProdutoResponseDTO> CadastrarProdutoAsync(ProdutoCreateDTO dto);

    Task<IEnumerable<ProdutoResponseDTO>> ListarProdutosAsync();

    Task<ProdutoResponseDTO> BuscarProdutoPorIdAsync(Guid id);

    Task<ProdutoResponseDTO> AtualizarProdutoAsync(Guid id, ProdutoCreateDTO dto);

    Task DeletarProdutoAsync(Guid id);

    Task BaixarEstoqueAsync(List<BaixaEstoqueDTO> itens);
}