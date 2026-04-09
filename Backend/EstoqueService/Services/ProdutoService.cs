using EstoqueService.Data;
using EstoqueService.DTOs;
using EstoqueService.Entities;
using Microsoft.EntityFrameworkCore;

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
        bool codigoJaExiste = await _db.Produtos.AnyAsync(p => p.Codigo == dto.Codigo);

        if (codigoJaExiste)
        {
            throw new Exception($"Já existe um produto cadastrado com o código {dto.Codigo}");
        }
        
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

    public async Task<IEnumerable<ProdutoResponseDTO>> ListarProdutosAsync()
    {
       return await _db.Produtos.Select(p => new ProdutoResponseDTO
       {
           Id = p.Id,
           Codigo = p.Codigo,
           Descricao = p.Descricao,
           Saldo = p.Saldo
       }).ToListAsync();
    }

    public async Task<ProdutoResponseDTO> BuscarProdutoPorIdAsync(Guid id)
    {
        return await _db.Produtos
        .Where(p => p.Id == id)
        .Select(p => new ProdutoResponseDTO
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Descricao = p.Descricao,
            Saldo = p.Saldo
        }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
    }
}