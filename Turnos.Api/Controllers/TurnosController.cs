
using Microsoft.AspNetCore.Mvc;
using Turnos.Api.Common;
using Turnos.Api.DTOs;
using Turnos.Api.Services;

namespace Turnos.Api.Controllers
{
    /// <summary>Endpoints para Turnos.</summary>
    [ApiController]
    [Route("turnos")]
    public class TurnosController : ControllerBase
    {
        private readonly ITurnoService _service;

        public TurnosController(ITurnoService service)
        {
            _service = service;
        }

        /// <summary>GET /turnos — lista com filtros (querystring).</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<TurnoResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] TurnoFilterQuery filtro)
        {
            var result = await _service.ListarAsync(filtro);
            return Ok(result);
        }

        /// <summary>POST /turnos — cria novo turno validando regras.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TurnoResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<TurnoResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] TurnoCreateRequest req)
        {
            var result = await _service.CriarAsync(req);
            if (!result.Success)
                return BadRequest(result);

            return Created($"/turnos/{result.Data!.Id}", result);
        }
    }
}
