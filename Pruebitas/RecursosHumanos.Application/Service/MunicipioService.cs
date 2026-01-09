using AutoMapper;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Exceptions;
using RecursosHumanos.Domain.Repositories;

namespace RecursosHumanos.Application.Services;

public class MunicipioService
{
    private readonly IMunicipioRepository _repository;
    private readonly IMapper _mapper;

    public MunicipioService(IMunicipioRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<MunicipioResponseDto>> ObtenerPorDepartamentoAsync(Guid deptoId)
    {
        var lista = await _repository.ObtenerPorDepartamentoAsync(deptoId);
        return _mapper.Map<List<MunicipioResponseDto>>(lista);
    }

    public async Task<Guid> CrearAsync(MunicipioCreateDto dto)
    {
        var nuevo = new Municipio(dto.Nombre, dto.DepartamentoId);
        await _repository.AgregarAsync(nuevo);
        return nuevo.Id;
    }
   public async Task<List<MunicipioResponseDto>> ObtenerTodosAsync() {
    var lista = await _repository.ObtenerTodosAsync();
    return _mapper.Map<List<MunicipioResponseDto>>(lista);
}
    public async Task ActualizarAsync(MunicipioUpdateDto dto)
    {
        var entidad = await _repository.ObtenerPorIdAsync(dto.Id);
        if (entidad == null) throw new EntidadNoEncontradaException("Municipio", dto.Id);

        entidad.CambiarNombre(dto.Nombre);
        // Nota: El método CambiarDepartamento era privado en tu entidad original. 
        // Si necesitas mover un municipio de depto, deberías hacerlo público en Municipio.cs
        // entidad.CambiarDepartamento(dto.DepartamentoId); 
        
        await _repository.ActualizarAsync(entidad);
    }

    public async Task EliminarAsync(Guid id)
    {
        await _repository.EliminarAsync(id);
    }
}