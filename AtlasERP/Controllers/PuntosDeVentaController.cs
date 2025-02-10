using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtlasERP.Controllers
{
    [Route("api/PuntosDeVenta")]
    [ApiController]
    public class PuntosDeVentaController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public PuntosDeVentaController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("guardarPuntosDeVenta")]
        public async Task<IActionResult> guardarPuntosDeVenta([FromBody] PuntoDeVentum model)
        {
            if (ModelState.IsValid)
            {
                _context.PuntoDeVenta.Add(model);
                if (await _context.SaveChangesAsync() > 0)
                {
                    var res = _context.PuntoDeVenta.FirstOrDefault(x => x.NombrePuntoVenta == model.NombrePuntoVenta);
                    return Ok(res);
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
