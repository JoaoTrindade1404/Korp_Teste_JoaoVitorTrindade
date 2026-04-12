using EstoqueService.Data;
using EstoqueService.DTOs;
using EstoqueService.Middlewares;
using EstoqueService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseSqlite("Data Source=estoque.db"));

builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAngular");

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

app.MapGet("/produtos", async (int page, int pageSize, IProdutoService service) =>
{
   return Results.Ok(await service.ListarProdutosAsync(page, pageSize)); 
});

app.MapGet("/produtos/{id:guid}", async (Guid id ,IProdutoService service) =>
{
   return Results.Ok(await service.BuscarProdutoPorIdAsync(id)); 
});

app.MapPut("/produtos/{id:guid}", async (Guid id, ProdutoCreateDTO dto, IProdutoService service) =>
{
    return Results.Ok(await service.AtualizarProdutoAsync(id, dto));
});

app.MapDelete("/produtos/{id:guid}", async (Guid id, IProdutoService service) =>
{
    await service.DeletarProdutoAsync(id);

    return Results.NoContent();
});

app.MapPatch("/produtos/baixar-estoque", async (RequisicaoBaixaDTO requisicao, IProdutoService service) =>
{
    await service.BaixarEstoqueAsync(requisicao);

    return Results.NoContent();
});

app.Run();