using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtlasERP.Controllers
{
    [Route("api/denominacionesCaja")]
    [ApiController]
    public class denominacionesCajaController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public denominacionesCajaController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("guardardenominacionesCaja")]
        public async Task<IActionResult> guardardenominacionesCaja([FromBody] DenominacionesCaja model)
        {
            if (ModelState.IsValid)
            {
                _context.DenominacionesCajas.Add(model);;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            else
            {
                return BadRequest("ERROR: Modelo inválido");
            }
        }

    }
}
