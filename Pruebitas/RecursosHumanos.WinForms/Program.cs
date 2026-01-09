using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using RecursosHumanos.Application.Mappings;
using RecursosHumanos.Application.Services;
using RecursosHumanos.Domain.Repositories;
using RecursosHumanos.Infrastructure.Data;
using RecursosHumanos.Infrastructure.Repositories;
using System.IO; 
using System.Windows.Forms; // Necesario para MessageBox y Application

namespace RecursosHumanos.WinForms;

static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    static void Main()
    {
        // 1. Inicialización de interfaz visual
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

        // 2. Configuración
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        // 3. Inyección de Dependencias
        var services = new ServiceCollection();
        ConfigureServices(services, configuration);

        ServiceProvider = services.BuildServiceProvider();

        try 
        {
            // --- PORTABILIDAD: CREACIÓN AUTOMÁTICA DE LA BASE DE DATOS ---
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // Esto crea el archivo RecursosHumanos.db y las tablas si no existen
                context.Database.EnsureCreated(); 
            }

            var mainForm = ServiceProvider.GetRequiredService<Form1>();
            System.Windows.Forms.Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al iniciar la aplicación: {ex.Message}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 4. CONFIGURACIÓN SQLITE
        var connectionString = "Data Source=RecursosHumanos.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString, // <--- CORREGIDO: UseSqlite en lugar de use
            b => b.MigrationsAssembly("RecursosHumanos.Infrastructure")));

        services.AddAutoMapper(typeof(MappingProfile));

        // Repositorios
        services.AddScoped<IPaisRepository, PaisRepository>();
        services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
        services.AddScoped<IMunicipioRepository, MunicipioRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IColaboradorRepository, ColaboradorRepository>();

        // Servicios
        services.AddScoped<PaisService>();
        services.AddScoped<DepartamentoService>();
        services.AddScoped<MunicipioService>();
        services.AddScoped<EmpresaService>();
        services.AddScoped<ColaboradorService>();

        // Formularios
        services.AddTransient<Form1>();
        services.AddTransient<FrmPaises>();
        services.AddTransient<FrmDepartamentos>();
        services.AddTransient<FrmMunicipios>();
        services.AddTransient<FrmEmpresas>();
        services.AddTransient<FrmColaboradores>();
    }
}