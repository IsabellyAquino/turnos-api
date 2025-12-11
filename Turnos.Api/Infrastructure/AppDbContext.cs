
using Microsoft.EntityFrameworkCore;
using Turnos.Api.Domain;

namespace Turnos.Api.Infrastructure
{
    /// <summary>DbContext: mapeia entidades para tabelas e define relacionamentos.</summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
        
         // Tabelas do banco (DbSet = representa cada tabela)
        public DbSet<Analista> Analistas => Set<Analista>();
        public DbSet<Projeto> Projetos => Set<Projeto>();
        public DbSet<Turno> Turnos => Set<Turno>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Email único para Analista
            modelBuilder.Entity<Analista>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // Turno ↔ Analista (1:N), sem delete em cascata para preservar históricos
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Analista)
                .WithMany(a => a.Turnos)
                .HasForeignKey(t => t.AnalistaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Turno ↔ Projeto (1:N opcional), se projeto apagar, seta FK como nula
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Projeto)
                .WithMany(p => p.Turnos)
                .HasForeignKey(t => t.ProjetoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Enum como int no banco (padrão)
            modelBuilder.Entity<Turno>()
                .Property(t => t.Status)
                .HasConversion<int>();
        }
    }
}
