using RecursosHumanos.Domain.Exceptions;

namespace RecursosHumanos.Domain;

public class Empresa
{
    public Guid Id { get; private set; }
    public string Nit { get; private set; }
    public string RazonSocial { get; private set; }
    public string NombreComercial { get; private set; }
    public string Telefono { get; private set; }
    public string CorreoElectronico { get; private set; }
    public Guid MunicipioId { get; private set; }

    private readonly List<ColaboradorEmpresa> _colaboradores;
    public IReadOnlyCollection<ColaboradorEmpresa> Colaboradores => _colaboradores;

    private Empresa() { }

    public Empresa(
        string nit,
        string razonSocial,
        string nombreComercial,
        string telefono,
        string correoElectronico,
        Guid municipioId)
    {
        Id = Guid.NewGuid();
        CambiarNit(nit);
        CambiarRazonSocial(razonSocial);
        CambiarNombreComercial(nombreComercial);
        CambiarTelefono(telefono);
        CambiarCorreo(correoElectronico);
        CambiarMunicipio(municipioId);

        _colaboradores = new List<ColaboradorEmpresa>();
    }

    public void AsignarColaborador(Guid colaboradorId)
    {
        if (_colaboradores.Any(c => c.ColaboradorId == colaboradorId))
            throw new ConflictoDominioException("El colaborador ya está asignado a esta empresa");

        _colaboradores.Add(new ColaboradorEmpresa(colaboradorId, Id));
    }

    public void CambiarNit(string nit)
    {
        if (string.IsNullOrWhiteSpace(nit))
            throw new ReglaNegocioException("El NIT es obligatorio");

        Nit = nit.Trim();
    }

    public void CambiarRazonSocial(string razonSocial)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new ReglaNegocioException("La razón social es obligatoria");

        RazonSocial = razonSocial.Trim();
    }

    public void CambiarNombreComercial(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaNegocioException("El nombre comercial es obligatorio");

        NombreComercial = nombre.Trim();
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

    public void CambiarMunicipio(Guid municipioId)
    {
        if (municipioId == Guid.Empty)
            throw new ReglaNegocioException("El municipio es obligatorio");

        MunicipioId = municipioId;
    }
}