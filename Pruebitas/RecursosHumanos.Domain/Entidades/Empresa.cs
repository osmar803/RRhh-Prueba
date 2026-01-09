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

    // --- ¡ESTO ES LO QUE FALTA! ---
    public virtual Municipio? Municipio { get; private set; }
    // ------------------------------

    // Relación con Colaboradores (ya la debías tener o la agregamos por si acaso)
    private readonly List<ColaboradorEmpresa> _colaboradores = new();
    public IReadOnlyCollection<ColaboradorEmpresa> Colaboradores => _colaboradores.AsReadOnly();

    private Empresa() { }

    public Empresa(string nit, string razonSocial, string nombreComercial, string telefono, string correo, Guid municipioId)
    {
        Id = Guid.NewGuid();
        CambiarNit(nit);
        CambiarRazonSocial(razonSocial);
        CambiarNombreComercial(nombreComercial);
        CambiarTelefono(telefono);
        CambiarCorreo(correo);
        CambiarMunicipio(municipioId);
    }

   
    
    public void CambiarNit(string nit) { 
        if(string.IsNullOrWhiteSpace(nit)) throw new ReglaNegocioException("NIT obligatorio");
        Nit = nit; 
    }
    public void CambiarRazonSocial(string razon) { 
        if(string.IsNullOrWhiteSpace(razon)) throw new ReglaNegocioException("Razón Social obligatoria");
        RazonSocial = razon; 
    }
    public void CambiarNombreComercial(string nombre) { 
        if(string.IsNullOrWhiteSpace(nombre)) throw new ReglaNegocioException("Nombre Comercial obligatorio");
        NombreComercial = nombre; 
    }
    public void CambiarTelefono(string tel) { 
        if(string.IsNullOrWhiteSpace(tel)) throw new ReglaNegocioException("Teléfono obligatorio");
        Telefono = tel; 
    }
    public void CambiarCorreo(string correo) { 
        if(string.IsNullOrWhiteSpace(correo)) throw new ReglaNegocioException("Correo obligatorio");
        CorreoElectronico = correo; 
    }

    public void CambiarMunicipio(Guid municipioId)
    {
        if (municipioId == Guid.Empty)
            throw new ReglaNegocioException("El municipio es obligatorio");
        MunicipioId = municipioId;
    }
}