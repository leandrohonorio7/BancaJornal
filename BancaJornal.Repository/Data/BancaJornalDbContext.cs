using Microsoft.EntityFrameworkCore;
using BancaJornal.Model.Entities;

namespace BancaJornal.Repository.Data;

/// <summary>
/// Contexto do Entity Framework Core para acesso ao banco de dados.
/// Responsabilidade única: configuração e acesso aos dados.
/// </summary>
public class BancaJornalDbContext : DbContext
{
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<ItemVenda> ItensVenda { get; set; }

    public BancaJornalDbContext(DbContextOptions<BancaJornalDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("Produtos");
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(p => p.Descricao)
                .HasMaxLength(500);
            
            entity.Property(p => p.PrecoVenda)
                .HasPrecision(18, 2);
            
            entity.Property(p => p.CodigoBarras)
                .HasMaxLength(50);

            entity.HasIndex(p => p.CodigoBarras);
            entity.HasIndex(p => p.Nome);
        });

        // Configuração da entidade Venda
        modelBuilder.Entity<Venda>(entity =>
        {
            entity.ToTable("Vendas");
            entity.HasKey(v => v.Id);
            
            entity.Property(v => v.ValorTotal)
                .HasPrecision(18, 2);
            
            entity.Property(v => v.Observacao)
                .HasMaxLength(500);

            entity.HasMany(v => v.Itens)
                .WithOne(i => i.Venda)
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(v => v.DataVenda);
        });

        // Configuração da entidade ItemVenda
        modelBuilder.Entity<ItemVenda>(entity =>
        {
            entity.ToTable("ItensVenda");
            entity.HasKey(i => i.Id);
            
            entity.Property(i => i.NomeProduto)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(i => i.PrecoUnitario)
                .HasPrecision(18, 2);
            
            entity.Property(i => i.ValorTotal)
                .HasPrecision(18, 2);

            entity.HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
