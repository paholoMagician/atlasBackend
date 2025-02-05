
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.SignalR;
using AtlasERP.Models;


namespace AtlasERP.Controllers
{
    [Route("api/ClienteAgencia")]
    [ApiController]
    public class ClientesAgenciasController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public ClientesAgenciasController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpGet("obtenerClientes/{ccia}/{tp}")]
        public async Task<IActionResult> obtenerClientes([FromRoute] string ccia, [FromRoute] int tp)
        {

            string Sentencia = " exec ObtenerCLientes @codcia, @type ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codcia", ccia));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@type", tp));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }

        [HttpGet("eliminarClientes/{codcli}/{codcia}")]
        public async Task<IActionResult> eliminarClientes([FromRoute] string codcli, [FromRoute] string codcia )
        {

            string Sentencia = " delete from cliente where codcliente = @ccli and codcia = @ccia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using ( SqlCommand cmd = new SqlCommand(Sentencia, connection) )
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccli", codcli));
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

        [HttpGet("eliminarAgencia/{codagencia}/{codcia}")]
        public async Task<IActionResult> eliminarAgencia([FromRoute] string codagencia, [FromRoute] string codcia )
        {

            string Sentencia = " delete from agencia where codagencia = @ccagen and codcia = @ccia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccagen", codagencia));
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
        
        [HttpGet("obtenerAgencias/{codcia}/{fil}/{prov}")]
        public async Task<IActionResult> obtenerAgencias([FromRoute] string codcia, [FromRoute] string fil, [FromRoute] string prov )
        {

            string Sentencia = " exec ObtenerAgencias @ccia, @Filter, @codProv ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection)) {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccia",    codcia));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@Filter",  fil));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codProv", prov));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }

        [HttpGet("obtenerAgenciasMaquinarias/{ccli}/{codcia}")]
        public async Task<IActionResult> obtenerAgenciasMaquinarias([FromRoute] string ccli, [FromRoute] string codcia )
        {

            string Sentencia = " exec MaquinariaBodegaCliente @codcli, @codcia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codcli", ccli));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codcia",   codcia));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }

        [HttpPut]
        [Route("EditarAgencia/{codagencia}/{ccia}")]
        public async Task<IActionResult> EditarAgencia([FromRoute] string codagencia, [FromRoute] string ccia, [FromBody] Agencium model)
        {

            if (codagencia != model.Codagencia && ccia != model.Codcia)
            {
                return BadRequest("No existe el usuario");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }

        [HttpPut]
        [Route("EditarCliente/{codcli}/{ccia}")]
        public async Task<IActionResult> EditarCliente([FromRoute] string codcli, [FromRoute] string ccia, [FromBody] Cliente model)
        {

            if (codcli != model.Codcliente && ccia != model.Codcia)
            {
                return BadRequest("No existe el usuario");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }

        [HttpPost]
        [Route("guardarCliente")]
        public async Task<IActionResult> guardarCliente([FromBody] Cliente model)
        {

            if (_context.Clientes.Any(c => c.Ruc == model.Ruc))
            {
                return BadRequest("El RUC ya está registrado");
            }

            if (ModelState.IsValid)
            {
                _context.Clientes.Add(model);
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

        [HttpPost]
        [Route("guardarAgencia")]
        public async Task<IActionResult> guardarAgencia([FromBody] Agencium model)
        {
            if (ModelState.IsValid)
            {
                _context.Agencia.Add(model);
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

    }

}
