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
           entidade.Property(p => p.RowVersion).IsRowVersion(); 


           entidade.ToTable(t => t.HasCheckConstraint(
            name: "CK_Produto_Estoque_NaoNegativo", 
            sql: "\"Saldo\" >= 0"
            ));
        });
    }
}