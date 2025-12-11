
// Services/ITurnoService.cs
using Turnos.Api.Common;
using Turnos.Api.DTOs;

namespace Turnos.Api.Services
{
    /// <summary>Contrato do serviço de turnos (regras de negócio).</summary>
    public interface ITurnoService
    {
        Task<ApiResponse<TurnoResponse>> CriarAsync(TurnoCreateRequest req);
        Task<ApiResponse<List<TurnoResponse>>> ListarAsync(TurnoFilterQuery filtro);
    }
}
