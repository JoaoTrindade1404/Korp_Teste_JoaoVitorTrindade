using FaturamentoService.DTOs;

namespace FaturamentoService.Services;

public interface IIAService
{
    Task<RespostaIADTO> ExtrairItensDoTextoAsync(TextoUsuarioIADTO request);
}