using AtlasERP.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        [HttpGet("ObtenerMedicamentos/{codcia}")]
        public async Task<IActionResult> ObtenerMedicamentos([FromRoute] string codcia)
        {

            string Sentencia = " exec ObtenerMedicamentos @ccia ";

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


        [HttpDelete("EliminarProductosMedicamentos/{id}")]
        public IActionResult EliminarProductosMedicamentos([FromRoute] int id)
        {

            var delete = _context.ProductosMedicamentos
                          .Where(b => b.IdMedicamento.Equals(id))
                          .ExecuteDelete();

            return (delete != 0) ? Ok() : BadRequest();

        }



        [HttpPost("EditarProductosMedicamentos")]
        public async Task<IActionResult> EditarProductosMedicamentos([FromBody] ProductosMedicamento model)
        {
            if (model == null)
                return BadRequest(new { message = "El modelo enviado es nulo." });

            if (model.IdMedicamento <= 0)
                return BadRequest(new { message = "El ID del medicamento no es válido." });

            try
            {
                await using SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                await using SqlCommand cmd = new SqlCommand("[sp_ActualizarProductoMedicamento]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Parámetros del procedimiento almacenado
                cmd.Parameters.AddWithValue("@idProducto", model.IdMedicamento);
                cmd.Parameters.AddWithValue("@nombre", model.Nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@principioActivo", model.PrincipioActivo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@concentracion", model.Concentracion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@formaFarmaceutica", model.FormaFarmaceutica ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@presentacion", model.Presentacion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@laboratorioFabricante", model.LaboratorioFabricante ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@paisOrigen", model.PaisOrigen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@registroSanitario", model.RegistroSanitario ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@lote", model.Lote ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@fechaFabricacion", model.FechaFabricacion);
                cmd.Parameters.AddWithValue("@fechaCaducidad", model.FechaCaducidad);
                cmd.Parameters.AddWithValue("@tipoMedicamento", model.TipoMedicamento ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@viaAdministracion", model.ViaAdministracion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@indicaciones", model.Indicaciones ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@contraindicaciones", model.Contraindicaciones ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@efectosSecundarios", model.EfectosSecundarios ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@cantidadStock", model.CantidadStock ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ubicacionAlmacen", model.UbicacionAlmacen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@temperaturaAlmacenamiento", model.TemperaturaAlmacenamiento ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@minimoStock", model.MinimoStock ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@precioUnitario", model.PrecioUnitario ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@requiereReceta", model.RequiereReceta ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@codigoBarras", model.CodigoBarras ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@imagenProducto", model.ImagenProducto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@idPuntoVenta", model.IdPuntoVenta ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@instruccionesUso", model.InstruccionesUso ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@valorBlister", model.ValorBlister ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@valorCaja", model.ValorCaja ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@observacion", model.Observacion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@precioCompra", model.PrecioCompra ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@descuento", model.Descuento ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@oImp", model.OImp ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@contNeto", model.ContNeto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@userRegister", model.UserRegister ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@unidadMedida", model.UnidadMedida ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@codcia", model.Codcia);
                cmd.Parameters.AddWithValue("@categoria", model.Categoria);

                int filasAfectadas = await cmd.ExecuteNonQueryAsync();

                return filasAfectadas > 0
                    ? Ok(new { message = "Producto actualizado correctamente." })
                    : NotFound(new { message = "No se encontró el producto o no se pudo actualizar." });
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, new { message = "Error en la base de datos.", error = sqlEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error en el servidor.", error = ex.Message });
            }

        }




        //[HttpPut]
        //[Route("EditarProductosMedicamentos/{id}")]
        //public async Task<IActionResult> EditarProductosMedicamentos([FromRoute] int id, [FromBody] ProductosMedicamento model)
        //{

        //    if (id != model.IdMedicamento)
        //    {
        //        return BadRequest("No existe el usuario");
        //    }

        //    _context.Entry(model).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //    return Ok(model);

        //}



        [HttpPost]
        [Route("GuardarProductosMEdicamentos")]
        public async Task<IActionResult> GuardarProductosMEdicamentos([FromBody] ProductosMedicamento model)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Datos inválidos", Errors = ModelState.Values.SelectMany(v => v.Errors) });
                }

                // Agregar el modelo al contexto
                _context.ProductosMedicamentos.Add(model);

                // Guardar cambios en la base de datos
                if (await _context.SaveChangesAsync() > 0)
                {
                    // Devolver el objeto creado con el ID generado
                    return Ok(model);
                }
                else
                {
                    return BadRequest(new { Message = "No se pudo guardar el producto." });
                }
            }
            catch (Exception ex)
            {
                // Registrar el error (opcional)
                //_logger.LogError(ex, "Error al guardar el producto.");

                // Devolver un error 500 con un mensaje genérico
                return StatusCode(500, new { Message = "Ocurrió un error interno al procesar la solicitud." });
            }
        }

    }
}
