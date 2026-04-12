namespace FaturamentoService.DTOs;

public record PagedResultDTO<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
