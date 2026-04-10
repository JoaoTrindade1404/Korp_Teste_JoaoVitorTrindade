using EstoqueService.Entities;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Data;

public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entidade =>
        {
           entidade.HasKey(p => p.Id);

           entidade.HasIndex(p => p.Codigo).IsUnique(); 
        });
    }
}