using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/AsignarProductoBodega")]
    [ApiController]
    public class ProdBodAsigController : ControllerBase
    {

        readonly atlasErpContext _context;

        public ProdBodAsigController(atlasErpContext context ) {
        
            _context = context;
        
        }

        [HttpPost]
        [Route("guardarProductoBodega")]
        public async Task<IActionResult> guardarProductoBodega([FromBody] Prodbodegasigna model)
        {

            //if (_context.Prodbodegasigna.Any(c => c.Codmaquinariabodega == model.Codmaquinariabodega))
            //{
            //    return BadRequest("Este producto ya existe en esta bodega");
            //}

            if (ModelState.IsValid)
            {
                _context.Prodbodegasignas.Add(model);
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

        [HttpGet("obtenerItemsBodega/{codbod}/{estado}")]
        public async Task<IActionResult> obtenerItemsBodega([FromRoute] int codbod, [FromRoute] int estado)
        {

            string Sentencia = " exec ObtenerItemsBodegas @codbodega, @state ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codbodega", codbod));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@state", estado));
                    adapter.Fill(dt);
                }
            }

            if (dt == null) { 
                return NotFound("No se ha podido crear..."); 
            }

            return Ok(dt);

        }

        [HttpGet("eliminarItemBodega/{codbod}")]
        public async Task<IActionResult> eliminarItemBodega([FromRoute] int codbod)
        {

            string Sentencia = " delete from prodbodegasigna where codmaquinariabodega = @codmaquinariabodega ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codmaquinariabodega", codbod));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido eliminar...");
            }

            return Ok(dt);

        }


    }
}
