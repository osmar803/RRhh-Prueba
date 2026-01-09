using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecursosHumanos.Domain;

namespace RecursosHumanos.Infrastructure.Data.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nit).IsRequired().HasMaxLength(20);
        builder.Property(e => e.RazonSocial).IsRequired().HasMaxLength(150);
        builder.Property(e => e.NombreComercial).IsRequired().HasMaxLength(150);
        
        // Relación: Empresa pertenece a un Municipio
        builder.HasOne<Municipio>()
            .WithMany()
            .HasForeignKey(e => e.MunicipioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración para la lista de Colaboradores (navegación)
        builder.HasMany(e => e.Colaboradores)
            .WithOne()
            .HasForeignKey(ce => ce.EmpresaId);
    }
}

public class ColaboradorConfiguration : IEntityTypeConfiguration<Colaborador>
{
    public void Configure(EntityTypeBuilder<Colaborador> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.NombreCompleto).IsRequired().HasMaxLength(200);
        builder.Property(c => c.CorreoElectronico).IsRequired().HasMaxLength(100);
        
        // Configuración para la lista de Empresas (navegación)
        builder.HasMany(c => c.Empresas)
            .WithOne()
            .HasForeignKey(ce => ce.ColaboradorId);
    }
}

public class ColaboradorEmpresaConfiguration : IEntityTypeConfiguration<ColaboradorEmpresa>
{
    public void Configure(EntityTypeBuilder<ColaboradorEmpresa> builder)
    {
        // Llave compuesta (Muchos a Muchos)
        builder.HasKey(ce => new { ce.ColaboradorId, ce.EmpresaId });
    }
}