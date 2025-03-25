using AtlasERP.DTO;
using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/aperturaPuntoVenta")]
    [ApiController]
    public class aperturaPuntoVentaController : ControllerBase
    {
        private readonly atlasErpContext _context;

        public aperturaPuntoVentaController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GuardarAperturaPuntoVenta")]
        public async Task<IActionResult> GuardarAperturaPuntoVenta([FromBody] AperturaPuntoVentum model)
        {


            if (ModelState.IsValid)
            {
                _context.AperturaPuntoVenta.Add(model);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest("Datos incorrectos");
                }
            }
            else
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPut("cerrarPuntoDeVenta")]
        public async Task<IActionResult> CerrarPuntoDeVenta([FromBody] AperturaPuntoVentum request)
        {
            try
            {
                // Validación de entrada
                if (request.Id == null || string.IsNullOrEmpty(request.Ccia))
                {
                    return BadRequest("ID de la accion y Ccia son obligatorios.");
                }

                // Buscar el punto de venta en la base de datos
                var puntoVenta = await _context.AperturaPuntoVenta
                    .FirstOrDefaultAsync(pv => pv.Id== request.Id && pv.Ccia == request.Ccia);

                if (puntoVenta == null)
                {
                    return NotFound("Punto de venta no encontrado.");
                }

                // Actualizar solo los campos requeridos
                puntoVenta.Fecierre = request.Fecierre;
                puntoVenta.Observacion = request.Observacion;

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok(new { message = "Punto de venta cerrado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }



    }
}
