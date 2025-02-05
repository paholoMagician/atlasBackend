using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/Mantenimiento")]
    [ApiController]
    public class MantenimientoController : ControllerBase
    {

        private readonly atlasErpContext _context;
        public MantenimientoController (atlasErpContext context )
        {
            _context = context;
        }


        [HttpPost]
        [Route("guardarMantenimiento")]
        public async Task<IActionResult> guardarMantenimiento([FromBody] Mantemaqcro model)
        {
            //var res = _context.Cronograma.Where( x => x.Codcrono == model.Codcrono );

            if (ModelState.IsValid)
            {
                _context.Mantemaqcros.Add(model);
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


        [HttpGet("ObtenerMantenimientoCrono/{cagencia}/{m}/{a}/{l}")]
        public async Task<IActionResult> ObtenerMantenimientoCrono([FromRoute] string cagencia, [FromRoute] string m, [FromRoute] string a, [FromRoute] string l)
        {

            string Sentencia = " exec ObtenerMantenimiento @codagencia, @mes, @anio, @local ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codagencia", cagencia));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@mes",   m));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@anio",  a));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@local", l));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }


        [HttpGet("eliminarMantenimiento/{id}")]
        public async Task<IActionResult> eliminarMantenimiento([FromRoute] int id)
        {

            string Sentencia = " delete from mantemaqcro where idmantenimiento = @idmante  ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@idmante", id));
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
