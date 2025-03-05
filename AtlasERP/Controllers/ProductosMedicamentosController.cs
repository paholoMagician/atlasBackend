using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/ProductosMedicamentos")]
    [ApiController]
    public class ProductosMedicamentosController : ControllerBase
    {

        private readonly atlasErpContext _context;
        public ProductosMedicamentosController(atlasErpContext context)
        {
            _context = context;
        }


        [HttpGet("ObtenerMedicamentos/{codcia}")]
        public async Task<IActionResult> ObtenerMedicamentos([FromRoute] string codcia)
        {

            string Sentencia = " exec ObtenerMedicamentos @ccia ";

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

        //[HttpPost]
        //[Route("GuardarProductosMEdicamentos")]
        //public async Task<IActionResult> GuardarProductosMEdicamentos([FromBody] ProductosMedicamento model)
        //{


        //    if (ModelState.IsValid)
        //    {
        //        _context.ProductosMedicamentos.Add(model);
        //        if (await _context.SaveChangesAsync() > 0)
        //        {
        //            var res = _context.ProductosMedicamentos.FirstOrDefault(x => x.IdPuntoVenta == model.IdPuntoVenta && x.CodigoBarras == model.CodigoBarras);
        //            return Ok(res);
        //        }
        //        else
        //        {
        //            return BadRequest("Datos incorrectos");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("ERROR");
        //    }
        //}


        [HttpPost]
        [Route("GuardarProductosMEdicamentos")]
        public async Task<IActionResult> GuardarProductosMEdicamentos([FromBody] ProductosMedicamento model)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Datos inválidos", Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                // Agregar el modelo al contexto
                _context.ProductosMedicamentos.Add(model);

                // Guardar cambios en la base de datos
                if (await _context.SaveChangesAsync() > 0)
                {
                    // Devolver el objeto creado con el ID generado
                    return Ok(model);
                }
                else
                {
                    return BadRequest(new { Message = "No se pudo guardar el producto." });
                }
            }
            catch (Exception ex)
            {
                // Registrar el error (opcional)
                //_logger.LogError(ex, "Error al guardar el producto.");

                // Devolver un error 500 con un mensaje genérico
                return StatusCode(500, new { Message = "Ocurrió un error interno al procesar la solicitud." });
            }
        }

    }
}
