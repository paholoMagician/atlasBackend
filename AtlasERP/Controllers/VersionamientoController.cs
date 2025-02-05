using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/versionamiento")]
    [ApiController]
    public class VersionamientoController : ControllerBase
    {


        private readonly atlasErpContext _context;

        public VersionamientoController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpGet("obtenerVersionamiento/{version}")]
        public async Task<IActionResult> obtenerVersionamiento([FromRoute] string version )
        {

            string Sentencia = " select * from versionamiento where version = @version";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@version", version));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("Problemas al traer el versionamiento...");
            }

            return Ok(dt);

        }


        [HttpGet("obtenerVersionCMS")]
        public async Task<IActionResult> obtenerVersionCMS()
        {

            string Sentencia = " select * from versionCMS";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("Problemas al traer la VersionCMS...");
            }

            return Ok(dt);

        }


    }
}
