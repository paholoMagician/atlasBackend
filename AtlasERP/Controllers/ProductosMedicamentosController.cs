using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtlasERP.Controllers
{
    [Route("api/ProductosMedicamentos")]
    [ApiController]
    public class ProductosMedicamentosController : ControllerBase
    {

        private readonly atlasErpContext _context;
        public ProductosMedicamentosController(atlasErpContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("GuardarProductosMEdicamentos")]
        public async Task<IActionResult> GuardarProductosMEdicamentos([FromBody] ProductosMedicamento model)
        {
            if (ModelState.IsValid)
            {
                _context.ProductosMedicamentos.Add(model);
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
