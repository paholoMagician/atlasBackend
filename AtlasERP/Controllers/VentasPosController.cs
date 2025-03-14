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
        public async Task<IActionResult> guardarVentasPos([FromBody] Ventaspo model)
        {

            if (ModelState.IsValid)
            {
                _context.Ventaspos.Add(model);
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

    }
}
