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
        var produto = new Produto(dto.Codigo, dto.Descricao, dto.Saldo);
        _db.Produtos.Add(produto);

        try 
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Já existe um produto cadastrado com o código {dto.Codigo}", ex);
        }

        return new ProdutoResponseDTO { 
            Id = produto.Id, 
            Codigo = produto.Codigo, 
            Descricao = produto.Descricao, 
            Saldo = produto.Saldo
        };
    }

    public async Task<PagedResultDTO<ProdutoResponseDTO>> ListarProdutosAsync(int page, int pageSize)
    {
        var query = _db.Produtos.AsQueryable();

        var total = await query.CountAsync();

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(p => new ProdutoResponseDTO
       {
           Id = p.Id,
           Codigo = p.Codigo,
           Descricao = p.Descricao,
           Saldo = p.Saldo
       }).ToListAsync();

       return new PagedResultDTO<ProdutoResponseDTO>(items, total, page, pageSize);
    }

    public async Task<ProdutoResponseDTO> BuscarProdutoPorIdAsync(Guid id)
    {
        var produto = await ObterProdutoOuFalharAsync(id);

        return new ProdutoResponseDTO
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Descricao = produto.Descricao,
            Saldo = produto.Saldo
        };
    }

    public async Task<ProdutoResponseDTO> AtualizarProdutoAsync(Guid id, ProdutoCreateDTO dto)
    {
        var produto = await ObterProdutoOuFalharAsync(id);

        produto.AtualizarDetalhes(dto.Codigo, dto.Descricao, dto.Saldo);

        try
        {
            await _db.SaveChangesAsync();   
        }
        catch(DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Produto foi modificado por outro usuário. Tente novamente.", ex);
        }
        catch(DbUpdateException ex)
        {
            throw new InvalidOperationException($"Erro ao atualizar produto: {ex.InnerException?.Message}", ex);
        }

        return new ProdutoResponseDTO
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Descricao = produto.Descricao,
            Saldo = produto.Saldo
        };
    }

    public async Task DeletarProdutoAsync(Guid id)
    {
        var produto = await ObterProdutoOuFalharAsync(id);

        try
        {
            _db.Produtos.Remove(produto);
            await _db.SaveChangesAsync();   
        }
        catch(DbUpdateException ex)
        {
            throw new InvalidOperationException("Não é possível deletar este produto (pode estar em uso)", ex);
        }

        
    }

    public async Task BaixarEstoqueAsync(RequisicaoBaixaDTO requisicao)
    {

        bool jaFoiProcessado = await _db.TransacoesProcessadas.AnyAsync(t => t.Id == requisicao.NotaFiscalId);
        if (jaFoiProcessado) 
        {
            return; 
        }

        foreach (var item in requisicao.Itens)
        {
            var produto = await ObterProdutoOuFalharAsync(item.Id);
            produto.BaixarEstoque(item.Quantidade);
        }

        _db.TransacoesProcessadas.Add(new TransacaoProcessada(requisicao.NotaFiscalId));

        try
        {
            await _db.SaveChangesAsync();    
        }
        catch(DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Conflito de concorrência, outro usuário tentou alterar o estoque, Tente novamente.", ex);
        }
        catch(DbUpdateException ex)
        {
            throw new InvalidOperationException($"Erro ao processar baixa de estoque: {ex.InnerException?.Message}", ex);
        }
        
    }

    private async Task<Produto> ObterProdutoOuFalharAsync(Guid id)
    {
        return await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id) 
        ?? throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
    }
}