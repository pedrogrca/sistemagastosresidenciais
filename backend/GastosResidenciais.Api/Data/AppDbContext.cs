using GastosResidenciais.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Api.Data;

/// <summary>
/// Contexto do Entity Framework Core. É a ponte entre as classes de domínio
/// (Pessoa, Transacao) e o banco de dados SQLite.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Tabela de pessoas.</summary>
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();

    /// <summary>Tabela de transações.</summary>
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    /// <summary>
    /// Configuração do mapeamento objeto-relacional (colunas, restrições e relacionamentos).
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nome)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(p => p.Idade)
                  .IsRequired();

            // Relacionamento 1:N — uma pessoa possui várias transações.
            // OnDelete(Cascade): ao excluir a pessoa, o banco apaga automaticamente
            // todas as transações vinculadas a ela (regra do desafio).
            entity.HasMany(p => p.Transacoes)
                  .WithOne(t => t.Pessoa)
                  .HasForeignKey(t => t.PessoaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Descricao)
                  .IsRequired()
                  .HasMaxLength(250);

            entity.Property(t => t.Valor)
                  .IsRequired();

            // Salva o enum como texto ("Despesa"/"Receita") em vez de número,
            // deixando o banco mais legível.
            entity.Property(t => t.Tipo)
                  .IsRequired()
                  .HasConversion<string>()
                  .HasMaxLength(20);

            // A categoria também é salva como texto legível no banco.
            entity.Property(t => t.Categoria)
                  .IsRequired()
                  .HasConversion<string>()
                  .HasMaxLength(30);
        });
    }
}
