using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RecursosHumanos.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // 1. AQUÍ VA TU CADENA DE CONEXIÓN PARA MIGRACIONES
        optionsBuilder.UseSqlServer("Server=localhost;Database=RecursosHumanosDB;Trusted_Connection=True;TrustServerCertificate=True;",
            b => b.MigrationsAssembly("RecursosHumanos.Infrastructure"));

        return new AppDbContext(optionsBuilder.Options);
    }
}