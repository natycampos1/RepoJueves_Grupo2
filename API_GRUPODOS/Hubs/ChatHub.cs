using Dapper;
using API_GRUPODOS.Models;
using API_GRUPODOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace API_GRUPODOS.Hubs
{
    [Authorize]
    public class ChatHub(IConfiguration _config, IUtilesService _utiles) : Hub
    {
        public async Task UnirseASala(int idPedido)
        {
            if (!TieneAcceso(idPedido))
                throw new HubException("Acceso denegado a esta sala.");

            await Groups.AddToGroupAsync(Context.ConnectionId, $"pedido-{idPedido}");
        }

        public async Task EnviarMensaje(int idPedido, string mensaje)
        {
            if (!TieneAcceso(idPedido))
                throw new HubException("Acceso denegado a esta sala.");

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdUsuario", _utiles.ObtenerConsecutivoToken());
            parameters.Add("@IdPedido", idPedido);
            parameters.Add("@Mensaje", mensaje);
            var idMensaje = context.QuerySingle<int>("SP_RegistrarMensaje", parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            var modelo = new MensajeResponseModel
            {
                IdMensaje = idMensaje,
                Mensaje = mensaje,
                FechaHora = DateTime.Now,
                IdUsuario = _utiles.ObtenerConsecutivoToken(),
                NombreUsuario = _utiles.ObtenerNombreToken()
            };

            await Clients.Group($"pedido-{idPedido}").SendAsync("RecibirMensaje", modelo);
        }

        private bool TieneAcceso(int idPedido)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@IdPedido", idPedido);
            parameters.Add("@IdUsuario", _utiles.ObtenerConsecutivoToken());
            parameters.Add("@IdRol", _utiles.ObtenerIdRolToken());
            var tieneAcceso = context.QueryFirstOrDefault<int?>("SP_ValidarAccesoChatPedido", parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return tieneAcceso > 0;
        }
    }
}