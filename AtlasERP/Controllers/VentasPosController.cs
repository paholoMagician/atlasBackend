using AtlasERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/ventasPos")]
    [ApiController]
    public class VentasPosController : Controller
    {

        readonly atlasErpContext _context;

        public VentasPosController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("guardarVentasPos")]
        public async Task<IActionResult> guardarVentasPos([FromBody] VentasposCab model)
        {

            if (ModelState.IsValid)
            {
                _context.VentasposCabs.Add(model);
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

        [HttpGet("obtenerVentasPos/{codcia}")]
        public async Task<IActionResult> obtenerVentasPos([FromRoute] string codcia)
        {

            string Sentencia = " exec obtenerVentasPos @ccia";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccia", codcia));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }

        [HttpGet("obtenerVentasCabTran/{id}/{type}")]
        public async Task<IActionResult> obtenerVentasCabTran([FromRoute] int id, [FromRoute] int type)
        {

            string Sentencia = " exec ObtebnerVentasCabTran @IDS, @tp";
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@IDS", id));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@tp", type));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }

        [HttpGet("ObtenerVentasDetTran/{tp}/{id}")]
        public async Task<IActionResult> ObtenerVentasDetTran([FromRoute] string tp,[FromRoute] int id)
        {

            string Sentencia = " exec ObtenerVentasDetTran @type, @IDS";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@type", tp));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@IDS", id));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }


        //[HttpGet]
        //[Route("ActualizarEstadoPrinterCab/{idCab}/{estado}")]
        //public async Task<IActionResult> ActualizarEstadoPrinterCab([FromRoute] int idCab, [FromRoute] int estado)
        //{
        //    try
        //    {
        //        // Buscar el repuesto por su código
        //        var cabTran = await _context.VentasposCabs
        //            .FirstOrDefaultAsync(r => r.Id == idCab);

        //        if (cabTran == null)
        //        {
        //            return NotFound($"Repuesto con código {idCab} no encontrado");
        //        }



        //        // Actualizar la cantidad de repuestos
        //        cabTran.EstadoPrint = estado;

        //        // Guardar los cambios en la base de datos
        //        _context.Repuestos.Update(cabTran);
        //        await _context.SaveChangesAsync();

        //        // Retornar respuesta con información detallada
        //        return Ok(cabTran);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Loggear el error (deberías implementar un sistema de logging)
        //        return StatusCode(500, $"Error interno al actualizar el stock: {ex.Message}");
        //    }
        //}


        [HttpGet]
        [Route("ActualizarEstadoPrinterCab/{idCab}/{estado}")]
        public async Task<IActionResult> ActualizarEstadoPrinterCab([FromRoute] int idCab, [FromRoute] int estado)
        {
            try
            {
                // Validar parámetros
                if (idCab <= 0)
                {
                    return BadRequest("El ID de cabecera debe ser mayor que cero");
                }

                // Buscar la cabecera transaccional por su ID
                var cabTran = await _context.VentasposCabs
                    .FirstOrDefaultAsync(r => r.Id == idCab);

                if (cabTran == null)
                {
                    return NotFound($"Cabecera transaccional con ID {idCab} no encontrada");
                }

                // Actualizar el estado de impresión
                cabTran.EstadoPrint = estado;

                // Marcar como modificado (no necesitas Update si primero hiciste FirstOrDefault)
                _context.Entry(cabTran).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Retornar respuesta con información detallada
                return Ok(cabTran);
            }
            catch (Exception ex)
            {
                // Loggear el error (considera usar un logger real como ILogger)
                return StatusCode(500, $"Error interno al actualizar el estado de impresión: {ex.Message}");
            }
        }


    }
}
