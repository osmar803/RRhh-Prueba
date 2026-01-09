using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Domain;

namespace RecursosHumanos.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Definimos las tablas
    public DbSet<Pais> Paises { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Municipio> Municipios { get; set; }
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Colaborador> Colaboradores { get; set; }
    public DbSet<ColaboradorEmpresa> ColaboradoresEmpresas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Esto busca automáticamente todas las configuraciones que crearemos en el siguiente paso
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}