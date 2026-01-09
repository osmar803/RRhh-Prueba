using AutoMapper;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Exceptions;
using RecursosHumanos.Domain.Repositories;

namespace RecursosHumanos.Application.Services;

public class PaisService
{
    private readonly IPaisRepository _repository;
    private readonly IMapper _mapper;

    public PaisService(IPaisRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<PaisResponseDto>> ObtenerTodosAsync()
    {
        var lista = await _repository.ObtenerTodosAsync();
        return _mapper.Map<List<PaisResponseDto>>(lista);
    }

    public async Task<PaisResponseDto> ObtenerPorIdAsync(Guid id)
    {
        var entidad = await _repository.ObtenerPorIdAsync(id);
        if (entidad == null) throw new EntidadNoEncontradaException("País", id);
        return _mapper.Map<PaisResponseDto>(entidad);
    }

    public async Task<Guid> CrearAsync(PaisCreateDto dto)
    {
        // Validación de negocio: Nombre único
        var existente = await _repository.ObtenerPorNombreAsync(dto.Nombre);
        if (existente != null) throw new ConflictoDominioException("Ya existe un país con ese nombre.");

        // Crear usando el constructor del Dominio para validar reglas
        var nuevoPais = new Pais(dto.Nombre);
        
        await _repository.CrearAsync(nuevoPais);
        return nuevoPais.Id;
    }

    public async Task ActualizarAsync(PaisUpdateDto dto)
    {
        var entidad = await _repository.ObtenerPorIdAsync(dto.Id);
        if (entidad == null) throw new EntidadNoEncontradaException("País", dto.Id);

        // Validar duplicados al editar
        var duplicado = await _repository.ObtenerPorNombreAsync(dto.Nombre);
        if (duplicado != null && duplicado.Id != dto.Id)
            throw new ConflictoDominioException("Ya existe otro país con ese nombre.");

        entidad.CambiarNombre(dto.Nombre);
        await _repository.ActualizarAsync(entidad);
    }

    public async Task EliminarAsync(Guid id)
    {
        var entidad = await _repository.ObtenerPorIdAsync(id);
        if (entidad == null) throw new EntidadNoEncontradaException("País", id);
        
        // Aquí podrías validar si tiene departamentos asociados antes de borrar
        await _repository.EliminarAsync(entidad);
    }
}