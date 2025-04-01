using AtlasERP.DTO;
using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/guardarVentasPosDet")]
    [ApiController]
    public class ventasPosDet : ControllerBase
    {

        readonly atlasErpContext _context;

        public ventasPosDet(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("guardarVentasPosDet")]
        public async Task<IActionResult> guardarVentasPosDet([FromBody] VentasposDet model)
        {

            if (ModelState.IsValid)
            {
                _context.VentasposDets.Add(model);
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

        [HttpGet]
        [Route("ObtenerDetalleVentas/{idApeVen}")]
        public async Task<IActionResult> ObtenerDetalleVentas([FromRoute] int idApeVen )
        {

            string Sentencia = "exec ObtenerDetallesVentaAperturaInterfaz @idAperturaVentas";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@idAperturaVentas", idApeVen));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }

        [HttpPost]
        [Route("ActualizarStock")]
        public async Task<IActionResult> editarStockProd([FromBody] ActualizarStockRequest request)
        {
            try
            {
                // Validar el modelo de entrada
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar el tipo de operación
                if (request.TipoOperacion != "A" && request.TipoOperacion != "D")
                {
                    return BadRequest("El campo 'TipoOperacion' debe ser 'A' (Agregar) o 'D' (Disminuir).");
                }

                // Validar que la cantidad sea positiva
                if (request.Cantidad <= 0)
                {
                    return BadRequest("La cantidad debe ser mayor que cero.");
                }

                // Buscar el producto
                var producto = await _context.ProductosMedicamentos.FindAsync(request.IdProducto);
                if (producto == null)
                {
                    return NotFound($"Producto con ID {request.IdProducto} no encontrado.");
                }

                // Operar según el tipo
                if (request.TipoOperacion == "A")
                {
                    producto.CantidadStock += request.Cantidad; // Sumar al stock
                }
                else if (request.TipoOperacion == "D")
                {
                    // Validar stock suficiente
                    if (producto.CantidadStock < request.Cantidad)
                    {
                        return BadRequest($"Stock insuficiente. Stock actual: {producto.CantidadStock}");
                    }
                    producto.CantidadStock -= request.Cantidad; // Restar al stock
                }

                // Guardar cambios
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"Stock actualizado. Nuevo stock: {producto.CantidadStock}",
                    StockActual = producto.CantidadStock
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error interno al actualizar el stock",
                    Error = ex.Message
                });
            }
        }

    }
}
