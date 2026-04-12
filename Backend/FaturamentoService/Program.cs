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

builder.Services.AddScoped<INotaFiscalService, NotaFiscalService>();

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
    var db = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/notas", async (int page, int pageSize, INotaFiscalService service) =>
{
    return Results.Ok(await service.ListarNotasAsync(page, pageSize));
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
