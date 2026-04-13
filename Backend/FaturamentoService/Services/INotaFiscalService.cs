using FaturamentoService.DTOs;

namespace FaturamentoService.Services;

public interface INotaFiscalService
{
    Task ImprimirNotaAsync(Guid id);

    Task<NotaFiscalResponseDTO> CriarNotaFiscalAsync(NotaFiscalCreateDTO dto);

    Task<PagedResultDTO<NotaFiscalResponseDTO>> ListarNotasAsync(int page, int pageSize);

    Task<NotaFiscalResponseDTO> BuscarNotaPorIdAsync(Guid id);
}