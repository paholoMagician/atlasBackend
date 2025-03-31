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
                // 1. Guardar la apertura
                _context.AperturaPuntoVenta.Add(model);

                // 2. Cambiar el estado del punto de venta a "1" (Abierto)
                var puntoVenta = await _context.PuntoDeVenta.FindAsync(model.Idpuntoventa);
                if (puntoVenta == null)
                {
                    return NotFound("Punto de venta no encontrado");
                }

                puntoVenta.Estado = 1;
                _context.PuntoDeVenta.Update(puntoVenta);

                // 3. Guardar ambos cambios en una transacción
                await _context.SaveChangesAsync(); // Guarda apertura + actualización de estado

                return Ok(model);
            }
            else
            {
                return BadRequest("ERROR: Modelo inválido");
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
                Console.WriteLine("///////////////////////////////////////");
                Console.WriteLine("///////////////////////////////////////");
                Console.WriteLine(request.Idpuntoventa);
                Console.WriteLine("///////////////////////////////////////");
                Console.WriteLine("///////////////////////////////////////");
                var puntoVentaRes = await _context.PuntoDeVenta.FindAsync(request.Idpuntoventa);
                if (puntoVentaRes == null)
                {
                    return NotFound("Punto de venta no encontrado");
                }
                puntoVentaRes.Estado = 0;
                _context.PuntoDeVenta.Update(puntoVentaRes);

                // 3. Guardar ambos cambios en una transacción
                await _context.SaveChangesAsync(); // Guarda apertura + actualización de estado

                return Ok(new { message = "Punto de venta cerrado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }



    }
}
