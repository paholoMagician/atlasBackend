using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/ModulosEstantes")]
    [ApiController]
    public class ModulosEstantesController : ControllerBase
    {

        private readonly atlasErpContext _context;
        public ModulosEstantesController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GuardarModulosEstantes")]
        public async Task<IActionResult> GuardarModulosEstantes([FromBody] ModulosEstante model)
        {
            if (ModelState.IsValid)
            {
                _context.ModulosEstantes.Add(model);
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

        [HttpGet("obtenerModulosEstantes/{idE}")]
        public async Task<IActionResult> obtenerModulosEstantes([FromRoute] string idE)
        {

            string Sentencia = " exec ObtenerModulosEstantes @idEstantes ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection)) 
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@idEstantes", idE));
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
        [Route("EditarModulosEstantes/{id}")]
        public async Task<IActionResult> EditarModulosEstantes([FromRoute] int id, [FromBody] ModulosEstante model)
        {
            
            if (id != model.Id)
            {
                return BadRequest("No existe el modulos estante");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }


    }
}
