

using AtlasERP.Models;
using Microsoft.AspNetCore.Mvc;

namespace AtlasERP.Controllers
{
    [Route("api/Asesor")]
    [ApiController]
    public class AsesorClienteController : Controller
    {

        readonly atlasErpContext _context;

        public AsesorClienteController (atlasErpContext context )
        {
            _context = context;
        }

        [HttpPost]
        [Route("guardarAsignacionAsesor")]
        public async Task<IActionResult> guardarAsignacionAsesor([FromBody] Asesorcliente model)
        {

            if (ModelState.IsValid)
            {
                _context.Asesorclientes.Add(model);
                if (await _context.SaveChangesAsync() > 0) {
                    return Ok(model);
                }

                else {
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
