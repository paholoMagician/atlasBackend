
using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AtlasERP.Controllers
{
    [Route("api/Repuestos")]
    [ApiController]
    public class RepuestosController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public RepuestosController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("guardarRepuestos")]
        public async Task<IActionResult> guardarRepuestos([FromBody] Repuesto model)
        {
            if (ModelState.IsValid)
            {
                _context.Repuestos.Add(model);
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

        [HttpGet("ObtenerMarcaRepuetos")]
        public async Task<IActionResult> ObtenerMarcaRepuetos()
        {

            var res = _context.MarcaRepuestos.ToList();

            if (res.Count > 0) {
              return Ok(res);
            } else {
              return BadRequest("No hay marca de repuestos registradas");
            }

        }


        [HttpGet("ObtenerModeloMarcaRepuetos/{idMarcaRep}")]
        public async Task<IActionResult> ObtenerModeloMarcaRepuetos( int idMarcaRep )
        {

            var res = _context.ModeloMarcaRepuestos.Where(x => x.IdMarcaRepuestos == idMarcaRep).ToList();
            if (res.Count > 0)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest("No hay marca de repuestos registradas");
            }

        }

        [HttpPut]
        [Route("EditarMarcaRepuestos/{id}")]
        public async Task<IActionResult> EditarMarcaRepuestos([FromRoute] int id, [FromBody] MarcaRepuesto model)
        {

            if (id != model.Id)
            {
                return BadRequest("No existe la marca del repuesto");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }

        [HttpDelete]
        [Route("EliminarMarcaRepuestos/{id}")]
        public async Task<IActionResult> DeleteMarcaRepuestos([FromRoute] int id)
        {
            var marcaRepuesto = await _context.MarcaRepuestos.FindAsync(id);
            if (marcaRepuesto == null)
            {
                return NotFound(new { message = "La marca de repuestos no existe." });
            }

            _context.MarcaRepuestos.Remove(marcaRepuesto);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Marca de repuestos eliminada correctamente" });
        }


        [HttpPost("GuardarMarcaRepuestos")]
        public async Task<IActionResult> GuardarMarcaRepuestos( [FromBody] MarcaRepuesto model )
        {

            var res = validateDboExist(model);
            if (res != null) {
            
                return BadRequest("Datos duplicados");
            
            }

            if (!ModelState.IsValid) {
            
                return BadRequest("Modelo de datos inválido");
            
            }

            _context.MarcaRepuestos.Add(model);
            if (await _context.SaveChangesAsync() > 0) {

                var dboModelSave = validateDboExist(model);
                return Ok(dboModelSave);
            
            }

            return BadRequest("No se pudo guardar la información");

        }

        private MarcaRepuesto validateDboExist(MarcaRepuesto model) => _context.MarcaRepuestos.FirstOrDefault(x => x.NombreMarcaRep == model.NombreMarcaRep);


        [HttpGet("obtenerRepuestos/{usc}/{codcia}")]
        public async Task<IActionResult> obtenerRepuestos([FromRoute] string usc, [FromRoute] string codcia)
        {

            string Sentencia = "exec ObtenerRepuestos @usercrea, @ccia";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@usercrea", usc));
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

        [HttpGet("eliminarRepuestos/{usc}/{codrep}")]
        public async Task<IActionResult> eliminarRepuestos([FromRoute] string usc, [FromRoute] string codrep)
        {

            string Sentencia = "delete from repuestos where usercrea = @usercrea and codrep = @codrepu";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@usercrea", usc));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@codrepu", codrep));
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
        [Route("EditarRepuestos/{codrep}")]
        public async Task<IActionResult> EditarRepuestos([FromRoute] string codrep, [FromBody] Repuesto model)
        {

            if (codrep != model.Codrep)
            {
                return BadRequest("No existe el repuesto");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }


    }

}
