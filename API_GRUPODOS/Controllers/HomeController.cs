using API_GRUPODOS.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_GRUPODOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController(IConfiguration _config) : ControllerBase
    {

        #region Registro
        [HttpGet("ConsultarTiposDeIdentificacionAPI")]
        public IActionResult ConsultarTiposDeIdentificacionAPI()
        {

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<TipoIdentificacionModel>("SP_ConsultarTiposIdentificacion", commandType: System.Data.CommandType.StoredProcedure).ToList();

            if (response.Any())
            {
                return Ok(response);
            }
            return BadRequest("No se ha registrado su información correctamente, valide el que ya tenga esa identificacion registrada");
        }

        [HttpPost("RegistrarUsuarioAPI")]
        public IActionResult RegistrarUsuarioAPI(RegistroUsuarioRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            // Hasheo la contraseña antes de enviarla al Procedimiento almacenado
            string contrasenaHash = BCrypt.Net.BCrypt.HashPassword(model.Contrasena);

            var parametros = new
            {
                Identificacion = model.Identificacion,
                IdTipoIdentificacion = model.IdTipoIdentificacion,
                NombreCompleto = model.NombreCompleto,
                PrimerApellido = model.PrimerApellido,
                SegundoApellido = model.SegundoApellido,
                Genero = model.Genero,
                Direccion = model.Direccion,
                Nacionalidad = model.Nacionalidad,
                NumTelefono = model.NumTelefono,
                Email = model.Email,
                Contrasena = contrasenaHash  // Esta es la contraseña ya Hasheada
            };

            var response = context.Execute(
                "SP_RegistrarUsuario",
                parametros);

            if (response > 0)
                return Ok("Usuario registrado correctamente");

            return BadRequest("No se pudo completar el registro, verifica si ya tienes una cuenta asociada");
        }

        #endregion

        #region Inicio de sesión

        [HttpPost("IniciarSesionAPI")]
        public IActionResult IniciarSesionAPI(InicioSesionUsuarioRequestModel model)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            // Busco usuario por email
            var usuario = context.QueryFirstOrDefault<RegistroUsuarioRequestModel>(
                "SP_IniciarSesion",
                new { Email = model.Email },
                commandType: System.Data.CommandType.StoredProcedure
            );

            // Verifico que exista y que la contraseña coincida con el hash
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Contrasena, usuario.Contrasena))
                return NotFound("Credenciales incorrectas");

            // En caso de Login exitoso
            // Mando InfoVariableSesionUsuarioModel para no mandar la contraseña
            InfoVariableSesionUsuarioModel infoUsuario = new();
            infoUsuario.Identificacion = usuario.Identificacion;
            infoUsuario.IdTipoIdentificacion = usuario.IdTipoIdentificacion;
            infoUsuario.NombreCompleto = usuario.NombreCompleto;
            infoUsuario.PrimerApellido = usuario.PrimerApellido;
            infoUsuario.SegundoApellido = usuario.SegundoApellido;
            infoUsuario.Genero = usuario.Genero;
            infoUsuario.Direccion = usuario.Direccion;
            infoUsuario.Nacionalidad = usuario.Nacionalidad;
            infoUsuario.FechaRegistro = usuario.FechaRegistro;
            infoUsuario.NumTelefono = usuario.NumTelefono;
            infoUsuario.Email = usuario.Email;

            return Ok(infoUsuario);
        }


        #endregion
    }
}


