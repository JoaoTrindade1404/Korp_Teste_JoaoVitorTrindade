using EstoqueService.Entities;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Data;

public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
}