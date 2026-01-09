using RecursosHumanos.Domain.Exceptions;

namespace RecursosHumanos.Domain;

public class ColaboradorEmpresa
{
    public Guid ColaboradorId { get; private set; }
    public Guid EmpresaId { get; private set; }

    private ColaboradorEmpresa() { }

    public ColaboradorEmpresa(Guid colaboradorId, Guid empresaId)
    {
        if (colaboradorId == Guid.Empty || empresaId == Guid.Empty)
            throw new ReglaNegocioException("Relación colaborador–empresa inválida");

        ColaboradorId = colaboradorId;
        EmpresaId = empresaId;
    }
}