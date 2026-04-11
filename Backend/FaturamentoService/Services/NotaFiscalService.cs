using FaturamentoService.Clients;
using FaturamentoService.Data;
using FaturamentoService.DTOs;
using FaturamentoService.Entities;
using Microsoft.EntityFrameworkCore;

public class NotaFiscalService : INotaFiscalService {
    private readonly FaturamentoDbContext _db;
    private readonly IEstoqueClient _estoqueDbClient;

    public NotaFiscalService (FaturamentoDbContext db, IEstoqueClient estoqueClient)
    {
        _db = db;
        _estoqueDbClient = estoqueClient;
    }

    public async Task<NotaFiscalResponseDTO> CriarNotaFiscalAsync(NotaFiscalCreateDTO dto)
    {
        int ultimoNumero = await _db.NotasFiscais.AnyAsync() ? 
        await _db.NotasFiscais.MaxAsync(n => n.NumeroSequencial)
        : 0;

        int proximoNumero = ultimoNumero + 1;

        var notaFiscal = new NotaFiscal(proximoNumero);

        foreach (var itemDto in dto.Itens)
        {
            notaFiscal.AdicionarItem(itemDto.ProdutoId, itemDto.Quantidade);
        }
    
        _db.NotasFiscais.Add(notaFiscal);
        await _db.SaveChangesAsync();

        return new NotaFiscalResponseDTO
        {
          Id = notaFiscal.Id,
          NumeroSequencial = notaFiscal.NumeroSequencial,
          Status = notaFiscal.Status.ToString()  
        };
    }

    public Task ImprimirNotaAsync(Guid id)
    {
        
    }
}