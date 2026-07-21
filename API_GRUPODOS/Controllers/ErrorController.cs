using Dapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_GRUPODOS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController(IConfiguration _config) : ControllerBase
    {
        [Route("RegistrarError")]
        public IActionResult RegistrarError()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@Mensaje", ex!.Error.Message ?? string.Empty);
            parameters.Add("@Lugar", ex?.Path);
            parameters.Add("@FechaHora", DateTime.Now);
            parameters.Add("@ConsecutivoUsuario", 0);

            var response = context.Execute("SP_RegistrarError", parameters);

            return StatusCode(500, "Se presento un inconveniente");
        }
    }
}
