using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtlasERP.Controllers
{
    [Route("api/Repbodasigna")]
    [ApiController]
    public class ProdRepAsignaController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public ProdRepAsignaController(atlasErpContext context)
        {
            _context = context;
        }

        [HttpPost("GuardarRepbodasigna")]
        public async Task<IActionResult> GuardarRepbodasigna([FromBody] Repbodasigna model)
        {

            var res = validateDboExist(model);
            if (res != null)
            {

                return BadRequest("Datos duplicados");

            }

            if (!ModelState.IsValid)
            {

                return BadRequest("Modelo de datos inválido");

            }

            _context.Repbodasignas.Add(model);
            if (await _context.SaveChangesAsync() > 0)
            {

                var dboModelSave = validateDboExist(model);
                return Ok(dboModelSave);

            }

            return BadRequest("No se pudo guardar la información");

        }

        private Repbodasigna validateDboExist(Repbodasigna model) => _context.Repbodasignas.FirstOrDefault(x => x.Codrepbodega == model.Codrepbodega && x.Codbodega == model.Codbodega);



    }
}
