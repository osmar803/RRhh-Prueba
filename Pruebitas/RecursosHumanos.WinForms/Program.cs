using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecursosHumanos.Application.Mappings;
using RecursosHumanos.Application.Services;
using RecursosHumanos.Domain.Repositories;
using RecursosHumanos.Infrastructure.Data;
using RecursosHumanos.Infrastructure.Repositories;
// ¡IMPORTANTE! Asegúrate de que este using esté presente para encontrar los formularios
using RecursosHumanos.WinForms; 

namespace RecursosHumanos.WinForms;

static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);

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

    private static void ConfigureServices(IServiceCollection services)
    {
        // 1. Base de Datos
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer("Server=localhost;Database=RecursosHumanosDB;Trusted_Connection=True;TrustServerCertificate=True;",
            b => b.MigrationsAssembly("RecursosHumanos.Infrastructure")));

        // 2. AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // 3. Repositorios
        services.AddScoped<IPaisRepository, PaisRepository>();
        services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
        services.AddScoped<IMunicipioRepository, MunicipioRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IColaboradorRepository, ColaboradorRepository>();

        // 4. Servicios
        services.AddScoped<PaisService>();
        services.AddScoped<DepartamentoService>();
        services.AddScoped<MunicipioService>();
        services.AddScoped<EmpresaService>();
        services.AddScoped<ColaboradorService>();

        // 5. FORMULARIOS (¡Aquí estaba el error!)
        // Debes registrar CADA formulario nuevo que crees
        services.AddTransient<Form1>();
        services.AddTransient<FrmPaises>();        // <--- Faltaba esto
        services.AddTransient<FrmDepartamentos>(); // <--- Faltaba esto
        services.AddTransient<FrmMunicipios>();    // <--- Faltaba esto
        services.AddTransient<FrmEmpresas>();      // <--- Faltaba esto
        services.AddTransient<FrmColaboradores>(); // <--- Faltaba esto
    }
}