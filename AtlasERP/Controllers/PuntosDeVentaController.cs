using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        [HttpPut]
        [Route("EditarPuntosDeVenta/{id}")]
        public async Task<IActionResult> EditarPuntosDeVenta([FromRoute] int id, [FromBody] PuntoDeVentum model)
        {

            if (id != model.IdPuntoVenta)
            {
                return BadRequest("No existe el usuario");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }


        [HttpGet]
        [Route("ObtenerPuntosDeVenta/{ccia}")]
        public IActionResult ObtenerPuntosDeVenta([FromRoute] string ccia)
        {
            var res = (from pv in _context.PuntoDeVenta
                       join mt1 in _context.MasterTables on pv.CodProv equals mt1.Codigo into provJoin
                       from mt1 in provJoin.Where(m => m.Master == "PRV00").DefaultIfEmpty() // Filtro para master = "PRV00"
                       join mt2 in _context.MasterTables on new { codigo = pv.CodCanton, master = pv.CodProv } equals new { codigo = mt2.Codigo, master = mt2.Master } into cantonJoin
                       from mt2 in cantonJoin.DefaultIfEmpty()
                       join us in _context.Usuarios on pv.Usercrea equals us.Coduser into userJoin
                       from us in userJoin.DefaultIfEmpty()
                       where pv.CodCia == ccia
                       select new
                       {
                           id_punto_venta = pv.IdPuntoVenta,
                           nombre_punto_venta = pv.NombrePuntoVenta,
                           cod_prov = pv.CodProv,
                           cod_canton = pv.CodCanton,
                           pv.Direccion,
                           pv.Telefono,
                           cod_cia = pv.CodCia,
                           fecrea = pv.Fecrea,
                           usercrea = pv.Usercrea,
                           nombreProvincia = mt1.Nombre,
                           nombreCanton = mt2.Nombre,
                           nUsuario = us.Nombre + " " + us.Apellido
                       }).ToList();

            if (res.Any())
            {
                return Ok(res);
            }
            else
            {
                return NotFound("No existen datos para la compañía proporcionada.");
            }
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
                    var res = (from pv in _context.PuntoDeVenta
                               join mt1 in _context.MasterTables on pv.CodProv equals mt1.Codigo into provJoin
                               from mt1 in provJoin.Where(m => m.Master == "PRV00").DefaultIfEmpty() // Filtro para master = "PRV00"
                               join mt2 in _context.MasterTables on new { codigo = pv.CodCanton, master = pv.CodProv } equals new { codigo = mt2.Codigo, master = mt2.Master } into cantonJoin
                               from mt2 in cantonJoin.DefaultIfEmpty()
                               join us in _context.Usuarios on pv.Usercrea equals us.Coduser into userJoin
                               from us in userJoin.DefaultIfEmpty()
                               where pv.NombrePuntoVenta == model.NombrePuntoVenta
                               select new
                               {
                                   id_punto_venta = pv.IdPuntoVenta,
                                   nombre_punto_venta = pv.NombrePuntoVenta,
                                   cod_prov = pv.CodProv,
                                   cod_canton = pv.CodCanton,
                                   pv.Direccion,
                                   pv.Telefono,
                                   cod_cia = pv.CodCia,
                                   fecrea = pv.Fecrea,
                                   usercrea = pv.Usercrea,
                                   nombreProvincia = mt1.Nombre,
                                   nombreCanton = mt2.Nombre,
                                   nUsuario = us.Nombre + " " + us.Apellido
                               }).FirstOrDefault();

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
