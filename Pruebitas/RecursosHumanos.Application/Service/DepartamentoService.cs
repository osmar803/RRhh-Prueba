using AutoMapper;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Exceptions;
using RecursosHumanos.Domain.Repositories;

namespace RecursosHumanos.Application.Services;

public class DepartamentoService
{
    private readonly IDepartamentoRepository _repository;
    private readonly IMapper _mapper;

    public DepartamentoService(IDepartamentoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<DepartamentoResponseDto>> ObtenerPorPaisAsync(Guid paisId)
    {
        var lista = await _repository.ObtenerPorPaisAsync(paisId);
        return _mapper.Map<List<DepartamentoResponseDto>>(lista);
    }
    public async Task<List<DepartamentoResponseDto>> ObtenerTodosAsync() {
    // Asegúrate de que tu IRepository tenga este método, si no, usa el de obtener por país
    var lista = await _repository.ObtenerTodosAsync();
    return _mapper.Map<List<DepartamentoResponseDto>>(lista);
}
    public async Task<Guid> CrearAsync(DepartamentoCreateDto dto)
{
    // Verifica que dto.PaisId no venga vacío
    if (dto.PaisId == Guid.Empty) 
        throw new ArgumentException("El ID del país no puede estar vacío.");

    var nuevo = new Departamento(dto.Nombre, dto.PaisId);
    await _repository.AgregarAsync(nuevo);
    return nuevo.Id;
}

    public async Task ActualizarAsync(DepartamentoUpdateDto dto)
    {
        var entidad = await _repository.ObtenerPorIdAsync(dto.Id);
        if (entidad == null) throw new EntidadNoEncontradaException("Departamento", dto.Id);

        entidad.CambiarNombre(dto.Nombre);
        entidad.CambiarPais(dto.PaisId);
        
        await _repository.ActualizarAsync(entidad);
    }

    public async Task EliminarAsync(Guid id)
    {
        await _repository.EliminarAsync(id);
    }
}