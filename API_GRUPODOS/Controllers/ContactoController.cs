using API_GRUPODOS.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_GRUPODOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController(IConfiguration _config) : ControllerBase
    {

        #region Mensaje de Contacto

        [HttpPost("RegistrarMensajeAPI")]
        public IActionResult RegistrarMensajeAPI(MensajeContactoRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@Nombre", model.Nombre);
            parameters.Add("@Email", model.Email);
            parameters.Add("@Asunto", model.Asunto);
            parameters.Add("@Mensaje", model.Mensaje);

            var response = context.Execute(
                "SP_RegistrarMensajeContacto",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Mensaje enviado correctamente");

            return BadRequest("No se pudo enviar el mensaje, intenta nuevamente");
        }

        #endregion
    }
}