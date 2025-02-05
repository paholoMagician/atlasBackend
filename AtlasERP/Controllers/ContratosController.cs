using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/Contratos")]
    [ApiController]
    public class ContratosController : ControllerBase
    {

        private readonly atlasErpContext _context;
        public ContratosController (atlasErpContext context )
        {
            _context = context;
        }

        [HttpPost]
        [Route("GuardarContratos")]
        public async Task<IActionResult> GuardarContratos([FromBody] Contrato model)
        {

            if (ModelState.IsValid)
            {
                _context.Contratos.Add(model);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest("Contrato incorrecto");
                }
            }
            else
            {
                return BadRequest("ERROR");
            }
        }


        [HttpGet("obtenerContratos/{codcia}")]
        public async Task<IActionResult> obtenerContratos([FromRoute] string codcia)
        {

            string Sentencia = "exec ObtenerContratos @ccia";

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
