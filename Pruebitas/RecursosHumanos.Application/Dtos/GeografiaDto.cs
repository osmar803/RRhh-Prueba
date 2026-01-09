namespace RecursosHumanos.Application.DTOs;

// --- PAIS ---
public record PaisCreateDto(string Nombre);
public record PaisUpdateDto(Guid Id, string Nombre);
public record PaisResponseDto(Guid Id, string Nombre);

// --- DEPARTAMENTO ---
public record DepartamentoCreateDto(string Nombre, Guid PaisId);
public record DepartamentoUpdateDto(Guid Id, string Nombre, Guid PaisId);
public record DepartamentoResponseDto(Guid Id, string Nombre, string PaisNombre, Guid PaisId);

// --- MUNICIPIO ---
public record MunicipioCreateDto(string Nombre, Guid DepartamentoId);
public record MunicipioUpdateDto(Guid Id, string Nombre, Guid DepartamentoId);
public record MunicipioResponseDto(
    Guid Id, 
    string Nombre, 
    Guid DepartamentoId, 
    string DepartamentoNombre // <--- ¡Esta es la clave! Queremos ver el nombre, no solo el ID
);