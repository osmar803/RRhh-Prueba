using AutoMapper;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Domain;
using RecursosHumanos.Domain.Exceptions;
using RecursosHumanos.Domain.Repositories;

namespace RecursosHumanos.Application.Services;

public class EmpresaService
{
    private readonly IEmpresaRepository _repository;
    private readonly IMapper _mapper;

    public EmpresaService(IEmpresaRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<EmpresaResponseDto>> ObtenerTodosAsync()
    {
        var lista = await _repository.ObtenerTodosAsync();
        return _mapper.Map<List<EmpresaResponseDto>>(lista);
    }

    public async Task<EmpresaResponseDto> ObtenerPorIdAsync(Guid id)
    {
        var entidad = await _repository.ObtenerPorIdAsync(id);
        if (entidad == null) throw new EntidadNoEncontradaException("Empresa", id);
        return _mapper.Map<EmpresaResponseDto>(entidad);
    }

    public async Task<Guid> CrearAsync(EmpresaCreateDto dto)
    {
        var existeNit = await _repository.ObtenerPorNitAsync(dto.Nit);
        if (existeNit != null) throw new ConflictoDominioException($"El NIT {dto.Nit} ya está registrado.");

        var nueva = new Empresa(
            dto.Nit, 
            dto.RazonSocial, 
            dto.NombreComercial, 
            dto.Telefono, 
            dto.CorreoElectronico, 
            dto.MunicipioId
        );

        await _repository.AgregarAsync(nueva);
        return nueva.Id;
    }

    public async Task ActualizarAsync(EmpresaUpdateDto dto)
    {
        var entidad = await _repository.ObtenerPorIdAsync(dto.Id);
        if (entidad == null) throw new EntidadNoEncontradaException("Empresa", dto.Id);

        // Si cambió el NIT, verificar que no esté duplicado
        if (entidad.Nit != dto.Nit)
        {
             var existeNit = await _repository.ObtenerPorNitAsync(dto.Nit);
             if (existeNit != null) throw new ConflictoDominioException("El nuevo NIT ya pertenece a otra empresa.");
        }

        entidad.CambiarNit(dto.Nit);
        entidad.CambiarRazonSocial(dto.RazonSocial);
        entidad.CambiarNombreComercial(dto.NombreComercial);
        entidad.CambiarTelefono(dto.Telefono);
        entidad.CambiarCorreo(dto.CorreoElectronico);
        entidad.CambiarMunicipio(dto.MunicipioId);

        await _repository.ActualizarAsync(entidad);
    }

    public async Task EliminarAsync(Guid id)
    {
        await _repository.EliminarAsync(id);
    }
}