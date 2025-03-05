using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/PuntosDeVentaEstantes")]
    [ApiController]
    public class PuntosDeVentaEstantesController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public PuntosDeVentaEstantesController(atlasErpContext context)
        {
            _context = context;
        }




        [HttpPost]
        [Route("guardarPuntosDeVentaEstantes")]
        public async Task<IActionResult> guardarPuntosDeVentaEstantes([FromBody] PuntoDeVentaEstante model) {
            if (ModelState.IsValid) {
                _context.PuntoDeVentaEstantes.Add(model);
                if (await _context.SaveChangesAsync() > 0) {
                    var res = _context.PuntoDeVentaEstantes.FirstOrDefault(x => x.TagDescription == model.TagDescription);
                    return Ok(res);
                } else {
                    return BadRequest("Datos incorrectos");
                }
            }
            else {
                return BadRequest("ERROR");
            }
        }


        [HttpGet]
        [Route("ObtenerEstantesPuntosDeVenta/{idPv}")]
        public IActionResult ObtenerEstantesPuntosDeVenta([FromRoute] int idPv)
        {
            if (idPv <= 0)
            {
                return BadRequest("El ID del punto de venta no es válido.");
            }

            try
            {
                var estantes = _context.PuntoDeVentaEstantes
                    .Where(x => x.IdPuntoVenta == idPv)
                    .AsNoTracking()
                    .ToList();

                if (estantes == null || !estantes.Any())
                {
                    return Ok("No se encontraron estantes para el punto de venta especificado.");
                }
                return Ok(estantes);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) aquí
                return StatusCode(500, "Ocurrió un error interno al procesar la solicitud.");
            }
        }

    }
}
