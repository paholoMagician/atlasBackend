using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        [HttpDelete("EliminarPuntoDeVenta/{id}")]
        public IActionResult EliminarPuntoDeVenta([FromRoute] int id)
        {
            
            var delete = _context.PuntoDeVenta
                          .Where(b => b.Id.Equals(id))
                          .ExecuteDelete();

            return (delete != 0) ? Ok() : BadRequest();

        }


        [HttpPut]
        [Route("EditarPuntosDeVenta/{id}")]
        public async Task<IActionResult> EditarPuntosDeVenta([FromRoute] int id, [FromBody] PuntoDeVentum model)
        {   
            Console.WriteLine(model.Id);
            Console.WriteLine(id);

            if (id != model.Id)
            {
                return BadRequest("No existe el usuario");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }


        [HttpGet("ObtenerPuntosDeVenta/{codcia}")]
        public async Task<IActionResult> ObtenerPuntosDeVenta([FromRoute] string codcia)
        {

            string Sentencia = " exec ObtenerPuntosDeVenta @ccia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
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



        //[HttpGet]
        //[Route("ObtenerPuntosDeVenta/{ccia}")]
        //public IActionResult ObtenerPuntosDeVenta([FromRoute] string ccia)
        //{
        //    var res = (from pv in _context.PuntoDeVenta
        //               join mt1 in _context.MasterTables on pv.CodProv equals mt1.Codigo into provJoin
        //               from mt1 in provJoin.Where(m => m.Master == "PRV00").DefaultIfEmpty() // Filtro para master = "PRV00"
        //               join mt2 in _context.MasterTables on new { codigo = pv.CodCanton, master = pv.CodProv } equals new { codigo = mt2.Codigo, master = mt2.Master } into cantonJoin
        //               from mt2 in cantonJoin.DefaultIfEmpty()
        //               join us in _context.Usuarios on pv.Usercrea equals us.Coduser into userJoin
        //               from us in userJoin.DefaultIfEmpty()
        //               where pv.CodCia == ccia
        //               select new
        //               {
        //                   id = pv.Id,
        //                   nombre_punto_venta = pv.NombrePuntoVenta,
        //                   cod_prov = pv.CodProv,
        //                   cod_canton = pv.CodCanton,
        //                   pv.Direccion,
        //                   pv.Telefono,
        //                   cod_cia = pv.CodCia,
        //                   fecrea = pv.Fecrea,
        //                   usercrea = pv.Usercrea,
        //                   nombreProvincia = mt1.Nombre,
        //                   nombreCanton = mt2.Nombre,
        //                   nUsuario = us.Nombre + " " + us.Apellido
        //               }).ToList();

        //    if (res.Any())
        //    {
        //        return Ok(res);
        //    }
        //    else
        //    {
        //        return NotFound("No existen datos para la compañía proporcionada.");
        //    }
        //}

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
                                   pv.Id,
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
