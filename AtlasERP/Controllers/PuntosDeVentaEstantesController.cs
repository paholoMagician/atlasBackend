using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AtlasERP.Controllers
{
    [Route("api/PuntosDeVentaEstantes")]
    [ApiController]
    public class PuntosDeVentaEstantesController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public PuntosDeVentaEstantesController(atlasErpContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("guardarPuntosDeVentaEstantes")]
        public async Task<IActionResult> guardarPuntosDeVentaEstantes([FromBody] PuntoDeVentaEstante model) {
            if (ModelState.IsValid) {
                _context.PuntoDeVentaEstantes.Add(model);
                if (await _context.SaveChangesAsync() > 0) {
                    var res = _context.PuntoDeVentaEstantes.FirstOrDefault(x => x.TagDescription == model.TagDescription);
                    return Ok(res);
                } else {
                    return BadRequest("Datos incorrectos");
                }
            }
            else {
                return BadRequest("ERROR");
            }
        }

    }
}
