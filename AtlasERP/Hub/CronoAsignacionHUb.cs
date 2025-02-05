using AtlasERP.Controllers.DTO;
using Microsoft.AspNetCore.SignalR;

namespace AtlasERP
{    public class CronoAsignacionHUb: Hub
    {
        public async Task CronoAsignacion(MobilModelDto model)
        {
            await Clients.All.SendAsync("CronoAsignacion", model);
        }
    }
}
