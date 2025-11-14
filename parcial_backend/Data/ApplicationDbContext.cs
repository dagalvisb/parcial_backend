using Microsoft.EntityFrameworkCore;
using parcial_backend.Models;

namespace parcial_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Teams> Teams { get; set; }
        public DbSet<Players> Players { get; set; }
        public DbSet<Matches> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === Configuración de Teams ===
            modelBuilder.Entity<Teams>(entity =>
            {
                entity.ToTable("teams");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Ciudad).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Estadio).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Fundacion).IsRequired();
            });

            // === Configuración de Players ===
            modelBuilder.Entity<Players>(entity =>
            {
                entity.ToTable("players");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Posicion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Edad).IsRequired();

                // RELACIÓN: Jugador → Equipo
                entity.HasOne(p => p.Equipo)
                      .WithMany(t => t.Jugadores)
                      .HasForeignKey(p => p.EquipoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // === Configuración de Matches ===
            modelBuilder.Entity<Matches>(entity =>
            {
                entity.ToTable("matches");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.GolesLocal).HasDefaultValue(0);
                entity.Property(e => e.GolesVisitante).HasDefaultValue(0);

                // RELACIÓN: Partido → Equipo Local
                entity.HasOne(m => m.EquipoLocal)
                      .WithMany(t => t.PartidosLocal)
                      .HasForeignKey(m => m.EquipoLocalId)
                      .OnDelete(DeleteBehavior.Restrict);

                // RELACIÓN: Partido → Equipo Visitante
                entity.HasOne(m => m.EquipoVisitante)
                      .WithMany(t => t.PartidosVisitante)
                      .HasForeignKey(m => m.EquipoVisitanteId)
                      .OnDelete(DeleteBehavior.Restrict);

                // CHECK CONSTRAINT para evitar que un equipo juegue contra sí mismo
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Matches_EquiposDiferentes",
                    "\"EquipoLocalId\" != \"EquipoVisitanteId\""
                ));
            });
        }
    }
}