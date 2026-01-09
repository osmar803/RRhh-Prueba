using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RecursosHumanos.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        
        // CORRECCIÓN: Usar SQLite con el mismo nombre de archivo que en Program.cs
        // Esto permite que las migraciones se generen correctamente para el formato de SQLite
        builder.UseSqlite("Data Source=RecursosHumanos.db");

        return new AppDbContext(builder.Options);
    }
}