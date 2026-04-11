using FaturamentoService.Clients;
using FaturamentoService.Data;
using FaturamentoService.DTOs;
using FaturamentoService.Middlewares;
using Microsoft.EntityFrameworkCore;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IEstoqueClient, EstoqueClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5225"); 
}).AddTransientHttpErrorPolicy(policyBuilder => 
    policyBuilder
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.Conflict)
    .WaitAndRetryAsync(3, tentativa => TimeSpan.FromSeconds(Math.Pow(2, tentativa))));

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/notas", async (INotaFiscalService service) =>
{
    return Results.Ok(await service.ListarNotasAsync());
});

app.MapPost("/notas", async (NotaFiscalCreateDTO dto, INotaFiscalService service) =>
{
    var resultado = await service.CriarNotaFiscalAsync(dto);
    return Results.Created($"/notas/{resultado.Id}", resultado);
});

app.MapPost("/notas/{id:guid}/imprimir", async (Guid id, INotaFiscalService service) =>
{
    await service.ImprimirNotaAsync(id);
    return Results.NoContent(); 
});


app.Run();
