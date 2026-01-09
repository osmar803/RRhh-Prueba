using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // <--- NECESARIO PARA LEER JSON
using Microsoft.Extensions.DependencyInjection;
using RecursosHumanos.Application.Mappings;
using RecursosHumanos.Application.Services;
using RecursosHumanos.Domain.Repositories;
using RecursosHumanos.Infrastructure.Data;
using RecursosHumanos.Infrastructure.Repositories;
using RecursosHumanos.WinForms;
using System.IO; // <--- NECESARIO PARA Directory.GetCurrentDirectory()

namespace RecursosHumanos.WinForms;

static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // --- 1. CONFIGURACIÓN DEL APPSETTINGS.JSON ---
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();
        // ---------------------------------------------

        var services = new ServiceCollection();
        
        // Pasamos la configuración al método para usarla dentro
        ConfigureServices(services, configuration);

        ServiceProvider = services.BuildServiceProvider();

        try 
        {
            var mainForm = ServiceProvider.GetRequiredService<Form1>();
            System.Windows.Forms.Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al iniciar: {ex.Message}");
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 2. Obtener la cadena de conexión del archivo JSON
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 3. Base de Datos (Usando la variable connectionString)
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString,
            b => b.MigrationsAssembly("RecursosHumanos.Infrastructure")));

        // 4. AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // 5. Repositorios
        services.AddScoped<IPaisRepository, PaisRepository>();
        services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
        services.AddScoped<IMunicipioRepository, MunicipioRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IColaboradorRepository, ColaboradorRepository>();

        // 6. Servicios
        services.AddScoped<PaisService>();
        services.AddScoped<DepartamentoService>();
        services.AddScoped<MunicipioService>();
        services.AddScoped<EmpresaService>();
        services.AddScoped<ColaboradorService>();

        // 7. FORMULARIOS
        services.AddTransient<Form1>();
        services.AddTransient<FrmPaises>();
        services.AddTransient<FrmDepartamentos>();
        services.AddTransient<FrmMunicipios>();
        services.AddTransient<FrmEmpresas>();
        services.AddTransient<FrmColaboradores>();
    }
}