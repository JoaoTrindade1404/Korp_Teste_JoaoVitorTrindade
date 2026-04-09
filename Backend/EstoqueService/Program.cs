using EstoqueService.Data;
using EstoqueService.DTOs;
using EstoqueService.Middlewares;
using EstoqueService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseSqlite("Data Source=estoque.db"));

builder.Services.AddScoped<IProdutoService, ProdutoService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "Microsserviço de Estoque Rodando!");

app.MapPost("/produtos", async (ProdutoCreateDTO dtoCreate, IProdutoService service) =>
{
    var resultado = await service.CadastrarProdutoAsync(dtoCreate); 
    
    return Results.Created($"/produtos/{resultado.Id}", resultado);
});

app.MapGet("/produtos", async (IProdutoService service) =>
{
   return Results.Ok(await service.ListarProdutosAsync()); 
});

app.MapGet("/produtos/{id:guid}", async (Guid id ,IProdutoService service) =>
{
   return Results.Ok(await service.BuscarProdutoPorIdAsync(id)); 
});

app.Run();