using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/Empresa")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public EmpresaController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpGet("ObtenerEmpresa")]
        public async Task<IActionResult> ObtenerEmpresa()
        {
            string Sentencia = " exec ObtenerEmpresa ";
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database
                                                                        .GetDbConnection()
                                                                        .ConnectionString))
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
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }


        [HttpPost]
        [Route("guardarEmpresa")]
        public async Task<IActionResult> guardarEmpresa([FromBody] Empresa model)
        {
            var rescontext = _context.Empresas.FirstOrDefault(x => x.Codcia == model.Codcia );

            if (rescontext == null)
            {
                // guarda
                if (ModelState.IsValid)
                {
                    _context.Empresas.Add(model);
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
            else
            {
                return BadRequest("Ya existe esta empresa");
            }
            
        }


        [HttpPut]
        [Route("EditarEmpresa/{codcia}")]
        public async Task<IActionResult> EditarEmpresa([FromRoute] string codcia, [FromBody] Empresa model)
        {

            if (codcia != model.Codcia)
            {
                return BadRequest("No existe la garantia");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }



    }
}
