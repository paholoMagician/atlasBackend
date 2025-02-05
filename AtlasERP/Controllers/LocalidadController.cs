using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/localidad")]
    [ApiController]
    public class LocalidadController : ControllerBase
    {

        readonly private atlasErpContext _context;
        public LocalidadController (atlasErpContext context)
        {
            _context = context;
        }

        [HttpGet("MainLocalidad")]
        public IActionResult MainLocalidad()
        {

            string Sentencia = " select * from localidad ";

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
                return NotFound("No se encontro esta localidad...");
            }

            return Ok(dt);

        }


    }
}
