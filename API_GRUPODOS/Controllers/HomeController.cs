using API_GRUPODOS.Models;
using API_GRUPODOS.Services;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_GRUPODOS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController(IConfiguration _config, IUtilesService _utiles) : ControllerBase
    {

        #region Registro
        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet("ConsultarGenerosAPI")]
        public IActionResult ConsultarGenerosAPI()
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var response = context.Query<GeneroModel>(
                "SP_ConsultarGeneros",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("RegistrarUsuarioAPI")]
        public IActionResult RegistrarUsuarioAPI(RegistroUsuarioRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            // Hasheo la contraseña antes de enviarla al Procedimiento almacenado
            string contrasenaHash = BCrypt.Net.BCrypt.HashPassword(model.Contrasena);

            var parameters = new DynamicParameters();
            parameters.Add("@Identificacion", model.Identificacion);
            parameters.Add("@IdTipoIdentificacion", model.IdTipoIdentificacion);
            parameters.Add("@NombreCompleto", model.NombreCompleto);
            parameters.Add("@PrimerApellido", model.PrimerApellido);
            parameters.Add("@SegundoApellido", model.SegundoApellido);
            parameters.Add("@Genero", model.Genero);
            parameters.Add("@Direccion", model.Direccion);
            parameters.Add("@Nacionalidad", model.Nacionalidad);
            parameters.Add("@NumTelefono", model.NumTelefono);
            parameters.Add("@Email", model.Email);
            parameters.Add("@Contrasena", contrasenaHash);  // Esta es la contraseña ya Hasheada

            var response = context.Execute(
                "SP_RegistrarUsuario",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            if (response > 0)
                return Ok("Usuario registrado correctamente");

            return BadRequest("No se pudo completar el registro, verifica si ya tienes una cuenta asociada");
        }

        #endregion

        #region Inicio de sesión

        [AllowAnonymous]
        [HttpPost("IniciarSesionAPI")]
        public IActionResult IniciarSesionAPI(InicioSesionUsuarioRequestModel model)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            // Busco usuario por email
            var parameters = new DynamicParameters();
            parameters.Add("@Email", model.Email);
            var usuario = context.QueryFirstOrDefault<RegistroUsuarioRequestModel>(
                "SP_IniciarSesion",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            // Verifico que exista y que la contraseña coincida con el hash
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Contrasena, usuario.Contrasena))
                return NotFound("Credenciales incorrectas");

            // En caso de Login exitoso
            // Mando InfoVariableSesionUsuarioModel para no mandar la contraseña
            InfoVariableSesionUsuarioModel infoUsuario = new();
            infoUsuario.IdUsuario = usuario.IdUsuario;
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
            infoUsuario.IdRol = usuario.IdRol;
            infoUsuario.Token = _utiles.GenerarToken(usuario.IdUsuario, usuario.IdRol, usuario.NombreCompleto);

            return Ok(infoUsuario);
        }

        [HttpGet("ConsultarInformacionUsuarioAPI")]
        public IActionResult ConsultarInformacionUsuarioAPI(string identificacion)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@Identificacion", identificacion);
            var response = context.QueryFirstOrDefault<UsuarioConsultaModel>(
                "SP_ConsultarUsuario",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response != null)
                return Ok(response);

            return BadRequest("No se encontró información para esa identificación");
        }

        #endregion

        #region Perfil

        [HttpPut("ActualizarPerfilAPI")]
        public IActionResult ActualizarPerfilAPI(PerfilRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@Identificacion", model.Identificacion);
            parameters.Add("@NombreCompleto", model.NombreCompleto);
            parameters.Add("@NumTelefono", model.NumTelefono);

            var response = context.Execute(
                "SP_ActualizarPerfil",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Perfil actualizado correctamente");

            return BadRequest("No se pudo actualizar el perfil");
        }

        [HttpGet("ValidarPedidoActivoAPI")]
        public IActionResult ValidarPedidoActivoAPI(string identificacion)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@Identificacion", identificacion);

            var cantidad = context.QueryFirstOrDefault<int>(
                "SP_ValidarPedidoActivoUsuario",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            return Ok(cantidad > 0);
        }

        [HttpPut("CambiarContrasenaPerfilAPI")]
        public IActionResult CambiarContrasenaPerfilAPI(CambiarContrasenaPerfilRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            string contrasenaHash = BCrypt.Net.BCrypt.HashPassword(model.NuevaContrasena);

            var parameters = new DynamicParameters();
            parameters.Add("@Identificacion", model.Identificacion);
            parameters.Add("@NuevaContrasena", contrasenaHash);

            var response = context.Execute(
                "SP_CambiarContrasenaPerfil",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (response > 0)
                return Ok("Contraseña actualizada correctamente");

            return BadRequest("No se pudo actualizar la contraseña");
        }

        #endregion

        #region Recuperar Acceso

        [AllowAnonymous]
        [HttpPost("RecuperarAccesoAPI")]
        public async Task<IActionResult> RecuperarAccesoAPI(RecuperarAccesoRequestModel model)
        {
            using var context = new SqlConnection(_config["ConnectionStrings:DefaultConnection"]);

            var parameters = new DynamicParameters();
            parameters.Add("@Email", model.Email);
            var usuario = context.QueryFirstOrDefault<UsuarioValidarCorreoModel>(
                "SP_ValidarCorreo",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (usuario == null)
                return NotFound("No se encontró una cuenta asociada a ese correo electrónico");

            //2. Generar una contraseña temporal
            var temporal = _utiles.GenerarContrasena();
            string contrasenaHash = BCrypt.Net.BCrypt.HashPassword(temporal);

            parameters = new DynamicParameters();
            parameters.Add("@IdUsuario", usuario.IdUsuario);
            parameters.Add("@Contrasena", contrasenaHash);
            parameters.Add("@IndicadorContrasenaTemp", true);
            var update = context.Execute(
                "SP_ActualizarContrasena",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );

            if (update > 0)
            {
                //3. Enviar la contraseña temporal al correo electrónico del usuario
                string ruta = Path.Combine(AppContext.BaseDirectory, "Templates", "RecuperarAcceso.html");
                string plantilla = System.IO.File.ReadAllText(ruta);

                plantilla = plantilla.Replace("{{TEMPORAL}}", temporal);
                plantilla = plantilla.Replace("{{NOMBRE}}", usuario.NombreCompleto);

                await _utiles.EnviarCorreoAsync(usuario.Email, "Recuperación de Acceso - RF Bakery", plantilla);

                return Ok(usuario);
            }

            return BadRequest("No se ha recuperado su acceso, intente nuevamente más tarde");
        }

        #endregion
    }
}