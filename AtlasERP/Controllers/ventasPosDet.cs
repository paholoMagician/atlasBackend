using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtlasERP.Controllers
{
    [Route("api/guardarVentasPosDet")]
    [ApiController]
    public class ventasPosDet : ControllerBase
    {

        readonly atlasErpContext _context;

        public ventasPosDet(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("guardarVentasPosDet")]
        public async Task<IActionResult> guardarVentasPosDet([FromBody] VentasposDet model)
        {

            if (ModelState.IsValid)
            {
                _context.VentasposDets.Add(model);
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
