using FaturamentoService.DTOs;

public interface INotaFiscalService
{
    Task ImprimirNotaAsync(Guid id);

    Task<NotaFiscalResponseDTO> CriarNotaFiscalAsync(NotaFiscalCreateDTO dto);

    Task<PagedResultDTO<NotaFiscalResponseDTO>> ListarNotasAsync(int page, int pageSize);
}