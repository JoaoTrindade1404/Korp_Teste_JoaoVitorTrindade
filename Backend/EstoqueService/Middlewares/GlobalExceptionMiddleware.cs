using System.Text.Json;

namespace EstoqueService.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (BadHttpRequestException ex) when (ex.InnerException is JsonException jsonEx)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            
            var campo = jsonEx.Path?.Replace("$.", "") ?? "desconhecido";
            
            await context.Response.WriteAsJsonAsync(new 
            { 
                erro = $"O campo '{campo}' possui um tipo de dado inválido. Verifique se você o tipo correto." 
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 400; 
            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsJsonAsync(new { erro = ex.Message });
        }
    }
}