using AutoMapper;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Domain;

namespace RecursosHumanos.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 1. Geografía - Paises
        CreateMap<Pais, PaisResponseDto>();
        
        // 2. Geografía - Departamentos
        CreateMap<Departamento, DepartamentoResponseDto>()
            // CORREGIDO: Ahora sacamos el nombre desde la navegación "Pais"
            .ForMember(dest => dest.PaisNombre, opt => opt.MapFrom(src => src.Pais != null ? src.Pais.Nombre : "Sin País")); 

        // 3. Geografía - Municipios
        CreateMap<Municipio, MunicipioResponseDto>()
            // CORREGIDO: Ahora sacamos el nombre desde la navegación "Departamento"
            .ForMember(dest => dest.DepartamentoNombre, opt => opt.MapFrom(src => src.Departamento != null ? src.Departamento.Nombre : "Sin Depto"));

        // 4. Empresas
        // Dentro del constructor de MappingProfile:

CreateMap<Empresa, EmpresaResponseDto>()
    .ForMember(dest => dest.MunicipioNombre, opt => opt.MapFrom(src => src.Municipio != null ? src.Municipio.Nombre : "Sin Municipio"));
        // 5. Colaboradores
        CreateMap<Colaborador, ColaboradorResponseDto>();
    }
}