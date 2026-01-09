using AutoMapper;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Exceptions;
using RecursosHumanos.Domain.Repositories;

namespace RecursosHumanos.Application.Services;

public class ColaboradorService
{
    private readonly IColaboradorRepository _repository;
    private readonly IMapper _mapper;

    public ColaboradorService(IColaboradorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ColaboradorResponseDto>> ObtenerTodosAsync()
    {
        var lista = await _repository.ObtenerTodosAsync();
        return _mapper.Map<List<ColaboradorResponseDto>>(lista);
    }

    public async Task<ColaboradorResponseDto> ObtenerPorIdAsync(Guid id)
    {
        var entidad = await _repository.ObtenerPorIdAsync(id);
        if (entidad == null) throw new EntidadNoEncontradaException("Colaborador", id);
        return _mapper.Map<ColaboradorResponseDto>(entidad);
    }

    public async Task<Guid> CrearAsync(ColaboradorCreateDto dto)
    {
        var nuevo = new Colaborador(
            dto.NombreCompleto,
            dto.Edad,
            dto.Telefono,
            dto.CorreoElectronico
        );

        await _repository.AgregarAsync(nuevo);
        return nuevo.Id;
    }

    public async Task ActualizarAsync(ColaboradorUpdateDto dto)
    {
        var entidad = await _repository.ObtenerPorIdAsync(dto.Id);
        if (entidad == null) throw new EntidadNoEncontradaException("Colaborador", dto.Id);

        entidad.CambiarNombre(dto.NombreCompleto);
        entidad.CambiarEdad(dto.Edad);
        entidad.CambiarTelefono(dto.Telefono);
        entidad.CambiarCorreo(dto.CorreoElectronico);

        await _repository.ActualizarAsync(entidad);
    }

    public async Task EliminarAsync(Guid id)
    {
        await _repository.EliminarAsync(id);
    }

    public async Task AsignarEmpresaAsync(Guid colaboradorId, Guid empresaId)
    {
        var colaborador = await _repository.ObtenerPorIdAsync(colaboradorId);
        if (colaborador == null) throw new EntidadNoEncontradaException("Colaborador", colaboradorId);

        // La lógica de validación (si ya existe la relación) está dentro de la entidad
        colaborador.AsignarEmpresa(empresaId);

        await _repository.ActualizarAsync(colaborador);
    }
}