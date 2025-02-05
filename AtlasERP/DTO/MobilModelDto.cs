using Newtonsoft.Json;

namespace AtlasERP.Controllers.DTO
{
    public class MobilModelDto
    {
        public string CodCrono { get; set; }
        public string CodUserTecnico { get; set; }
        public string CodAgencia { get; set; }
        public string Nombre { get; set; }
        public string CodCliente { get; set; }
        public string HorarioAtencionD { get; set; }
        public string ImagenCliente { get; set; }
        public string HorarioAtencionHM { get; set; }
        public string Nombre1 { get; set; }
        public string ProvinciaAgencia { get; set; }
        public string CantonAgencia { get; set; }
        public string Observacion { get; set; }
        public DateTime? FecCrea { get; set; }
        public string CodUserCreaCrono { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public int Anio { get; set; }
        public DateTime? FechaMantenimiento { get; set; }
        public string? CodLocalidad { get; set; }
        public int? Estado { get; set; }
        public int CantidadAsignaciones { get; set; }

        //public static MobilModelDto FromJson(string json)
        //{
        //    return JsonConvert.DeserializeObject<MobilModelDto>(json);
        //}

    }
}
