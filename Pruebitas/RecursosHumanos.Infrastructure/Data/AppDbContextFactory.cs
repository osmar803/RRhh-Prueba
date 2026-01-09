using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; // <--- Importante
using System.IO;

namespace RecursosHumanos.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Construir la configuración para leer el appsettings.json
        // Nota: Asumimos que el archivo está en la carpeta de salida del proyecto ejecutable
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false) // Debe existir
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Leer la cadena
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseSqlServer(connectionString);

        return new AppDbContext(builder.Options);
    }
}