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

        // CORREGIDO: Usamos (d => d.Pais) en lugar de <Pais>
        builder.HasOne(d => d.Pais) 
            .WithMany()
            .HasForeignKey(d => d.PaisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Nombre).IsRequired().HasMaxLength(100);

        // CORREGIDO: Usamos (m => m.Departamento) en lugar de <Departamento>
        builder.HasOne(m => m.Departamento)
            .WithMany()
            .HasForeignKey(m => m.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}