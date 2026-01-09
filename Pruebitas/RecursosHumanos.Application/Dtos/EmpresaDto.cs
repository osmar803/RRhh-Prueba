namespace RecursosHumanos.Application.DTOs;

public record EmpresaCreateDto(
    string Nit,
    string RazonSocial,
    string NombreComercial,
    string Telefono,
    string CorreoElectronico,
    Guid MunicipioId
);

public record EmpresaUpdateDto(
    Guid Id,
    string Nit,
    string RazonSocial,
    string NombreComercial,
    string Telefono,
    string CorreoElectronico,
    Guid MunicipioId
);

public record EmpresaResponseDto(
    Guid Id,
    string Nit,
    string RazonSocial,
    string NombreComercial,
    string Telefono,
    string CorreoElectronico,
    Guid MunicipioId,
    string MunicipioNombre // Útil para mostrar en grillas
);