using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/MaquinaAgencia")]
    [ApiController]
    public class AsignacionMaquinariaAgenciaController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public AsignacionMaquinariaAgenciaController(atlasErpContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("GuardarMaquinaAgencia")]
        public async Task<IActionResult> GuardarMaquinaAgencia([FromBody] Asignacionmaquinacliente model)
        {


            if (ModelState.IsValid)
            {
                _context.Asignacionmaquinaclientes.Add(model);
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

        [HttpGet]
        [Route("obtenerAsignacionmaquinacliente/{cagencia}/{codcia}")]
        public async Task<IActionResult> obtenerAsignacionmaquinacliente([FromRoute] string cagencia, [FromRoute] string codcia)
        {

            string Sentencia = " select mq.codmaquina, mq.nserie, mt1.nombre as tipomaquina, mc.nombremarca, md.nombremodelo from asignacionmaquinacliente as asm " +
                               " left join maquinaria as mq on mq.codmaquina = asm.codprod " +
                               " left join MasterTable as mt1 on mt1.codigo = mq.codtipomaquina and mt1.master = 'MQT' " +
                               " left join marca as mc on mc.codmarca = mq.marca and mc.codigotipomaq = mq.codtipomaquina " +
                               " left join modelo as md on md.codmodelo = mq.modelo and md.codmarca = mc.codmarca and md.codigotipomaq = mc.codigotipomaq " +
                               " where asm.codagencia = @codagencia and asm.ccia = @ccia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codagencia", cagencia));
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


        [HttpGet]
        [Route("elimianarAsignacionmaquinacliente/{cprod}/{codcia}")]
        public async Task<IActionResult> elimianarAsignacionmaquinacliente([FromRoute] string cprod, [FromRoute] string codcia )
        {

            string Sentencia = " delete from asignacionmaquinacliente where codprod = @codprod and ccia = @ccia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codprod", cprod));
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
