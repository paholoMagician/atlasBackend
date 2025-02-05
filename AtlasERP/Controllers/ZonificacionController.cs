using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/Zonificacion")]
    [ApiController]
    public class ZonificacionController : ControllerBase
    {
        private const string codMaster = "PRV00";
        readonly atlasErpContext _context;
  
        public ZonificacionController (atlasErpContext context )
        {
            _context = context;
        }

        public class AgenciaSimpleDTO
        {
            public string CodAgencia { get; set; }
            public string Nombre { get; set; }
            public string CodProv { get; set; }
            public string NombreProvincia { get; set; }
            public string NombreCliente { get; set; }
            public string NombreLocalidad { get; set; }
            public int? LocalidadCodigo { get; set; }
            public DateTime? FechaCreacion { get; set; }
        }

        [HttpGet]
        [Route("ObtenerAgenciasGeneral/{codcia}/{ccli}")]
        public async Task<IActionResult> ObtenerAgenciasGeneral([FromRoute] string codcia, [FromRoute] string ccli)
        {

            string Sentencia = " exec obtenerAgenciasCardinal @ccliente, @ccia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccliente", ccli));
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

        [HttpGet("ObtenerAgenciaZonificada/{codzone}/{codcli}")]
        public async Task<IActionResult> ObtenerAgenciaZonificada([FromRoute] string codzone, [FromRoute] string codcli )
        {

            string Sentencia = " exec ObtenerAgenciaZonificada @ccli, @czone ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccli",  codcli));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@czone", codzone));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha obtener la zonificacion...");
            }

            return Ok(dt);

        }

        [HttpGet("ObtenerAgenciasLocalidad/{codlocal}/{codzone}")]
        public async Task<IActionResult> ObtenerAgenciasLocalidad([FromRoute] int codlocal, [FromRoute] string codzone)
        {

            string Sentencia = " exec ObtenerAgenciasLocalidad @clocal, @czone ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@clocal",  codlocal));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@czone", codzone));
                    adapter.Fill(dt);
                }
            }

            if (dt == null) {

                return NotFound("No se ha obtener la zonificacion...");

            }

            return Ok(dt);

        }

        [HttpGet("ActualizarAgenciaEstado/{estado}/{cAgencia}")]
        public async Task<IActionResult> ActualizarAgenciaEstado([FromRoute] int estado, [FromRoute] string cAgencia)
        {

            string Sentencia = " exec ActualizarAgenciaEstado @st, @codAgencia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@st",  estado));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codAgencia", cAgencia));
                    adapter.Fill(dt);
                }
            }

            if (dt == null) {

                return NotFound("No se ha obtener la zonificacion...");

            }

            return Ok(dt);

        }

        [HttpPost]
        [Route("guardarZonificacion")]
        public async Task<IActionResult> guardarZonificacion([FromBody] Cardinal model)
        {

            if (ModelState.IsValid)
            {
                _context.Cardinals.Add(model);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest("Datos incorrectos del cronograma");
                }
            }
            else
            {
                return BadRequest("ERROR");
            }
        }


        [HttpGet("eliminarDataCardinal/{id}")]
        public async Task<IActionResult> eliminarDataCardinal([FromRoute] int id)
        {

            string Sentencia = " delete from cardinal where id = @IDS ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@IDS", id));
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
