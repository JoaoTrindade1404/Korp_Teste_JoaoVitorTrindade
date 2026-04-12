using EstoqueService.DTOs;

namespace EstoqueService.Services;

public interface IProdutoService
{
    Task<ProdutoResponseDTO> CadastrarProdutoAsync(ProdutoCreateDTO dto);

    Task<PagedResultDTO<ProdutoResponseDTO>> ListarProdutosAsync(int page, int pageSize);

    Task<ProdutoResponseDTO> BuscarProdutoPorIdAsync(Guid id);

    Task<ProdutoResponseDTO> AtualizarProdutoAsync(Guid id, ProdutoCreateDTO dto);

    Task DeletarProdutoAsync(Guid id);

    Task BaixarEstoqueAsync(RequisicaoBaixaDTO requisicao);
}