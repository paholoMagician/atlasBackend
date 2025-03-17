
using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtlasERP.Controllers.fileController
{
    [Route("api/AnyFiles")]
    [ApiController]
    public class AnyFilesController : ControllerBase
    {

        private readonly atlasErpContext _context;
        private readonly IWebHostEnvironment _environment;

        public AnyFilesController (atlasErpContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("fysicfile/{nombreArchivo}")]
        public IActionResult DescargarArchivo(string nombreArchivo)
        {
            string file = nombreArchivo + ".pdf";
            string rutaArchivo = Path.Combine(_environment.ContentRootPath, "fileModel/Assets/" + nombreArchivo, file);

            if (System.IO.File.Exists(rutaArchivo))
            {
                var archivoBytes = System.IO.File.ReadAllBytes(rutaArchivo);
                return File(archivoBytes, "application/octet-stream", file);
            }

            return NotFound("Ruta: " + rutaArchivo);
        }

    }  
}
