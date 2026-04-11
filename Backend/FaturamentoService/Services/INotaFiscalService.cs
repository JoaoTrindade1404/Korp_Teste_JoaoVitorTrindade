using FaturamentoService.DTOs;

public interface INotaFiscalService
{
    Task ImprimirNotaAsync(Guid id);

    Task<NotaFiscalResponseDTO> CriarNotaFiscalAsync(NotaFiscalCreateDTO dto);

    Task<IEnumerable<NotaFiscalResponseDTO>> ListarNotasAsync();
}