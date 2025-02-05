using AtlasERP.Controllers.DTO;
using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace AtlasERP.Controllers
{
    [Route("api/Cronograma")]
    [ApiController]
    public class CronogramaController : ControllerBase
    {
        private readonly IHubContext<CronoAsignacionHUb> _cronoAsignacionHUb;
        private readonly atlasErpContext _context;

        public CronogramaController(atlasErpContext context, IHubContext<CronoAsignacionHUb> cronoAsignacionHUb)
        {
            _context = context;
            _cronoAsignacionHUb = cronoAsignacionHUb;
        }

        [HttpPost]
        [Route("GuardarCrono")]
        public async Task<IActionResult> GuardarCrono([FromBody] Cronograma model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Cronogramas.Add(model);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        var result = await (
                            from cr in _context.Cronogramas
                            join ag in _context.Agencia on cr.Codagencia equals ag.Codagencia
                            join cli in _context.Clientes on ag.Codcliente equals cli.Codcliente into cliJoin
                            from cli in cliJoin.DefaultIfEmpty()
                            join mt1 in _context.MasterTables on new { codigo = ag.CodProv, master = "PRV00" } equals new { codigo = mt1.Codigo, master = mt1.Master } into mt1Join
                            from mt1 in mt1Join.DefaultIfEmpty()
                            join mt2 in _context.MasterTables on new { codigo = ag.CodCanton, master = ag.CodProv } equals new { codigo = mt2.Codigo, master = mt2.Master } into mt2Join
                            from mt2 in mt2Join.DefaultIfEmpty()
                            join img1 in _context.ImgFiles on ag.Codcliente equals img1.Codentidad into img1Join
                            from img1 in img1Join.DefaultIfEmpty()
                            join mcro in _context.Mantemaqcros
                                on new { TecnicKey = cr.Codusertecnic, CronoKey = cr.Codcrono }
                                equals new { TecnicKey = mcro.Codtecnico, CronoKey = mcro.Codcrono } into mcroJoin
                            from mcro in mcroJoin.DefaultIfEmpty()
                            where cr.Codcrono == model.Codcrono
                            group new { cr, ag, cli, mt1, mt2, img1, mcro } by new
                            {
                                cr.Codcrono,
                                cr.Codagencia,
                                cr.Codusertecnic,
                                ag.Nombre,
                                ag.Codcliente,
                                ag.Horarioatenciond,
                                ImagenCliente = img1.Imagen ?? "",
                                ag.Horarioatencionhm,
                                ClienteNombre = cli.Nombre,
                                ProvinciaAgencia = mt1.Nombre ?? "S.A.",
                                CantonAgencia = mt2.Nombre ?? "S.A.",
                                cr.Observacion,
                                cr.Feccrea,
                                cr.Codusercreacrono,
                                Mes = cr.Mes ?? 0,
                                Dia = cr.Dia ?? 0,
                                Anio = cr.Anio ?? 0,
                                cr.Fechamantenimiento,
                                cr.Codlocalidad,
                                cr.Estado,
                            } into grouped
                            orderby grouped.Key.Anio, grouped.Key.Mes, grouped.Key.Dia
                            select new MobilModelDto
                            {
                                CodCrono = grouped.Key.Codcrono,
                                CodAgencia = grouped.Key.Codagencia,
                                CodUserTecnico = grouped.Key.Codusertecnic,
                                Nombre = grouped.Key.Nombre,
                                CodCliente = grouped.Key.Codcliente,
                                HorarioAtencionD = grouped.Key.Horarioatenciond,
                                ImagenCliente = grouped.Key.ImagenCliente,
                                HorarioAtencionHM = grouped.Key.Horarioatencionhm,
                                Nombre1 = grouped.Key.Nombre,
                                ProvinciaAgencia = grouped.Key.ProvinciaAgencia,
                                CantonAgencia = grouped.Key.CantonAgencia,
                                Observacion = grouped.Key.Observacion,
                                FecCrea = grouped.Key.Feccrea ?? DateTime.MinValue,
                                CodUserCreaCrono = grouped.Key.Codusercreacrono,
                                Mes = grouped.Key.Mes,
                                Dia = grouped.Key.Dia,
                                Anio = grouped.Key.Anio,
                                FechaMantenimiento = grouped.Key.Fechamantenimiento ?? DateTime.MinValue,
                                CodLocalidad = grouped.Key.Codlocalidad,
                                Estado = grouped.Key.Estado,
                                CantidadAsignaciones = grouped.Count(g => g.mcro != null)
                            })
                            .ToListAsync();

                        await _cronoAsignacionHUb.Clients.All.SendAsync("CronoAsignacion", result);

                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest("Datos incorrectos del cronograma");
                    }
                }
                else
                {
                    return BadRequest("ERROR");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        //[HttpPost]
        //[Route("GuardarCrono")]
        //public async Task<IActionResult> GuardarCrono([FromBody] Cronograma model)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _context.Cronograma.Add(model);
        //        if (await _context.SaveChangesAsync() > 0)
        //        {
        //            Console.WriteLine("===================");
        //            Console.WriteLine(model.Codcrono);
        //            Console.WriteLine("===================");
        //            await _cronoAsignacionHUb.Clients.All.SendAsync("CronoAsignacion", model);
        //            return Ok(model);
        //        }
        //        else
        //        {
        //            return BadRequest("Datos incorrectos del cronograma");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("ERROR");
        //    }
        //}

        [HttpGet("ObtenerCronograma/{codcia}/{an}/{me}/{loc}/{tp}/{userTecnic}")]
        public async Task<IActionResult> ObtenerCronograma([FromRoute] string codcia, [FromRoute] string an, [FromRoute] string me, [FromRoute] string loc, [FromRoute] int tp, [FromRoute] string userTecnic)
        {

            string Sentencia = " exec ObtenerCrono @ccia, @anio, @mes, @localidad, @tipo, @usTecnic ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection)) {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccia", codcia));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@anio", an));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@mes", me));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@localidad", loc));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@tipo", tp));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@usTecnic", userTecnic));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }
        
        [HttpGet("CalculoAsignacionCronograma/{codFecuency}/{codcrono}")]
        public async Task<IActionResult> CalculoAsignacionCronograma([FromRoute] string codFecuency, [FromRoute] string codcrono )
        {

            string Sentencia = " exec calculoAsignacionCronograma @cfrecuency, @ccrono ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection)) {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@cfrecuency", codFecuency));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccrono", codcrono));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }
        
        [HttpGet("ObtenerCronogramaMobilTecnico/{ctecnic}/{codcia}")]
        public async Task<IActionResult> ObtenerCronogramaMobilTecnico([FromRoute] string ctecnic,[FromRoute] string codcia)
        {

            string Sentencia = " exec ObtenerCronoMobilTecnico @cousertecnic,@ccia ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection)) {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@cousertecnic", ctecnic));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccia",         codcia));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }

        [HttpPut]
        [Route("EditarCrono/{codcrono}")]
        public async Task<IActionResult> EditarCrono([FromRoute] string codcrono, [FromBody] Cronograma model) {

            if (codcrono!= model.Codcrono) {
                return BadRequest("No existe el usuario");
            }

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);

        }

        [HttpGet("DeleteCronograma/{codcrono}")]
        public async Task<IActionResult> DeleteCronograma([FromRoute] string codcrono)
        {

            string Sentencia = " delete from cronograma where codcrono = @ccrono ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@ccrono", codcrono));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se ha podido crear...");
            }

            return Ok(dt);

        }

    }
}
