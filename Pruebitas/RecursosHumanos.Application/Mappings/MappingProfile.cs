using AutoMapper;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Domain;

namespace RecursosHumanos.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Geografía
        CreateMap<Pais, PaisResponseDto>();
        
        CreateMap<Departamento, DepartamentoResponseDto>()
            .ForMember(dest => dest.PaisNombre, opt => opt.MapFrom(src => src.Id)); 
            // Nota: Si quieres el nombre real del país aquí, necesitarías incluir la navegación en el Repositorio 
            // o cargar el nombre por separado. Por ahora lo dejamos simple.

        CreateMap<Municipio, MunicipioResponseDto>()
    .ForMember(dest => dest.DepartamentoNombre, opt => opt.MapFrom(src => src.DepartamentoId));

        // Empresa
        CreateMap<Empresa, EmpresaResponseDto>();

        // Colaborador
        CreateMap<Colaborador, ColaboradorResponseDto>();
    }
}