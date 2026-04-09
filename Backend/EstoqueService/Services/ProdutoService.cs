using EstoqueService.Data;
using EstoqueService.DTOs;
using EstoqueService.Entities;

namespace EstoqueService.Services;

public class ProdutoService : IProdutoService 
{
    private readonly EstoqueDbContext _db;

    public ProdutoService(EstoqueDbContext db)
    {
        _db = db;
    }

    public async Task<ProdutoResponseDTO> CadastrarProdutoAsync(ProdutoCreateDTO dto)
    {
        
        var produto = new Produto
        {
            Codigo = dto.Codigo,
            Saldo = dto.Saldo,
            Descricao = dto.Descricao
        };
        
        _db.Produtos.Add(produto);

        await _db.SaveChangesAsync();

        return new ProdutoResponseDTO { 
            Id = produto.Id, 
            Codigo = produto.Codigo, 
            Descricao = produto.Descricao, 
            Saldo = produto.Saldo
        };
    }
}