using RecursosHumanos.Domain.Exceptions;

namespace RecursosHumanos.Domain;

public class Colaborador
{
    public Guid Id { get; private set; }
    public string NombreCompleto { get; private set; }
    public int Edad { get; private set; }
    public string Telefono { get; private set; }
    public string CorreoElectronico { get; private set; }

    private readonly List<ColaboradorEmpresa> _empresas;
    public IReadOnlyCollection<ColaboradorEmpresa> Empresas => _empresas;

    private Colaborador() { }

    public Colaborador(string nombreCompleto, int edad, string telefono, string correoElectronico)
    {
        Id = Guid.NewGuid();
        CambiarNombre(nombreCompleto);
        CambiarEdad(edad);
        CambiarTelefono(telefono);
        CambiarCorreo(correoElectronico);

        _empresas = new List<ColaboradorEmpresa>();
    }

    public void AsignarEmpresa(Guid empresaId)
    {
        if (_empresas.Any(e => e.EmpresaId == empresaId))
            throw new ConflictoDominioException("El colaborador ya pertenece a esta empresa");

        _empresas.Add(new ColaboradorEmpresa(Id, empresaId));
    }

    public void CambiarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaNegocioException("El nombre completo es obligatorio");

        NombreCompleto = nombre.Trim();
    }

    public void CambiarEdad(int edad)
    {
        if (edad < 18)
            throw new ReglaNegocioException("El colaborador debe ser mayor de edad");

        Edad = edad;
    }

    public void CambiarTelefono(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
            throw new ReglaNegocioException("El teléfono es obligatorio");

        Telefono = telefono.Trim();
    }

    public void CambiarCorreo(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
            throw new ReglaNegocioException("Correo electrónico inválido");

        CorreoElectronico = correo.Trim();
    }
}