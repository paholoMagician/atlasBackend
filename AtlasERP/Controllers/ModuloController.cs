using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing;

namespace AtlasERP.Controllers
{
    [Route("api/modulos")]
    [ApiController]
    public class ModuloController : ControllerBase
    {

        private readonly atlasErpContext _context;

        public ModuloController(atlasErpContext context)
        {
            _context = context;
        }


        [HttpGet("GetModulos/{userCod}")]
        public async Task<IActionResult> GetModulos([FromRoute] string userCod)
        {
            var res = (from asmod in _context.AsignModUsers
                       join mod in _context.Modulos on asmod.CodMod equals mod.Id.ToString() into ModJoin
                       from mod in ModJoin.DefaultIfEmpty()
                       where asmod.CodUser == userCod
                       select new
                       {
                           permisos = asmod.Permisos,
                           cod_user = asmod.CodUser,
                           id = mod.Id,
                           categoria= mod.Categoria,
                           moduleName = mod.ModuleName,
                           moduleDescription = mod.ModuleDescription,
                           icon = mod.Icon,
                           color = mod.Color
                       }).ToList();

            if (res != null && res.Count > 0)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest("Usuario inexistente");
            }
        }


        [HttpGet("EditarPermisosModulos/{permmod}/{codmod}/{userCod}")]
        public async Task<IActionResult> EditarPermisosModulos([FromRoute] string permmod, [FromRoute] string codmod, [FromRoute] string userCod)
        {

            string Sentencia = " exec UpdateEstadoModulo @permiso, @idmod, @coduser ";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(Sentencia, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand.CommandType = CommandType.Text;
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@permiso", permmod));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@idmod", codmod));
                    adapter.SelectCommand.Parameters.Add(new SqlParameter("@coduser", userCod));
                    adapter.Fill(dt);
                }
            }

            if (dt == null)
            {
                return NotFound("No se encontro este WebUser...");
            }

            return Ok(dt);

        }

    }
}
