
using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/Bodegas")]
    [ApiController]
    public class BodegasController : ControllerBase
    {

        private readonly atlasErpContext _context;
        public BodegasController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("guardarBodegas")]
        public async Task<IActionResult> guardarBodegas([FromBody] Bodega model)
        {

            if (_context.Bodegas.Any(c => c.Nombrebodega == model.Nombrebodega)) {
                return BadRequest("Esta bodega ya se registró");
            }

            if (ModelState.IsValid) {
                _context.Bodegas.Add(model);
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

        [HttpGet("obtenerBodegas/{codcia}")]
        public async Task<IActionResult> obtenerBodegas([FromRoute] string codcia)
        {

            string Sentencia = " exec obtenerBodegas @ccia ";

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


        [HttpGet("eliminarBodegas/{id}")]
        public async Task<IActionResult> eliminarBodegas([FromRoute] int id)
        {

            string Sentencia = " delete from bodegas where id = @Ids; " +
                " delete from prodbodegasigna where codbodega = @Ids;  ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@Ids", id));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido eliminar...");
            }

            return Ok(dt);

        }

        [HttpPut]
        [Route("EditarBodegas/{id}")]
        public async Task<IActionResult> EditarBodegas([FromRoute] int id, [FromBody] Bodega model)
        {

            if (id != model.Id)
            {
                return BadRequest("No existe el usuario");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }


    }
}
