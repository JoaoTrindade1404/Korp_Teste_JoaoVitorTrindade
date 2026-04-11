using FaturamentoService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Data;

public class FaturamentoDbContext : DbContext
{
    public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options) { }

    public DbSet<NotaFiscal> NotasFiscais { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotaFiscal>(entidade =>
        {
            entidade.HasKey(n => n.Id);

            entidade.Metadata
                .FindNavigation(nameof(NotaFiscal.Itens))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            entidade.HasMany(n => n.Itens)
                .WithOne() 
                .HasForeignKey("NotaFiscalId") 
                .OnDelete(DeleteBehavior.Cascade); 

            entidade.HasIndex(n => n.NumeroSequencial).IsUnique();
        });

        modelBuilder.Entity<ItemNotaFiscal>(entidade =>
        {
            entidade.HasKey(i => i.Id);
        });
    }

}