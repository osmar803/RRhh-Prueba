using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecursosHumanos.Domain;

namespace RecursosHumanos.Infrastructure.Data.Configurations;

public class PaisConfiguration : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
    }
}

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Nombre).IsRequired().HasMaxLength(100);

        // --- CORRECCIÓN AQUÍ ---
        // Permitir que al borrar un PAÍS, se borren sus DEPARTAMENTOS
        builder.HasOne(d => d.Pais) 
            .WithMany()
            .HasForeignKey(d => d.PaisId)
            .OnDelete(DeleteBehavior.Cascade); // <--- CAMBIADO A CASCADE
    }
}

public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Nombre).IsRequired().HasMaxLength(100);

        // --- CORRECCIÓN PREVIA ---
        // Permitir que al borrar un DEPARTAMENTO, se borren sus MUNICIPIOS
        builder.HasOne(m => m.Departamento)
            .WithMany()
            .HasForeignKey(m => m.DepartamentoId)
            .OnDelete(DeleteBehavior.Cascade); // <--- CAMBIADO A CASCADE
    }
}
