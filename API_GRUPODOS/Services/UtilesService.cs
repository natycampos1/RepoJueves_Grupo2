using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_GRUPODOS.Services
{
    public class UtilesService(IConfiguration _config, IHttpContextAccessor _httpContext) : IUtilesService
    {
        public string GenerarContrasena()
        {
            return Guid.NewGuid().ToString("N")[..10];
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var mensaje = new MimeMessage();
            var cuentaGmail = _config["Correos:CuentaGmail"]!;
            var contrasenaAplicacion = _config["Correos:ContrasenaAplicacion"]!;

            if (string.IsNullOrEmpty(contrasenaAplicacion))
                return;

            mensaje.From.Add(new MailboxAddress(string.Empty, cuentaGmail));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;

            mensaje.Body = new TextPart("html")
            {
                Text = cuerpoHtml
            };

            using var cliente = new SmtpClient();

            try
            {
                await cliente.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await cliente.AuthenticateAsync(cuentaGmail, contrasenaAplicacion);
                await cliente.SendAsync(mensaje);
            }
            finally
            {
                await cliente.DisconnectAsync(true);
            }
        }

        // genero el JWT con los 3 datos que necesitamos en cada request: quien es, que rol tiene, y su nombre (para el chat)
        public string GenerarToken(int idUsuario, int idRol, string nombre)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("idUsuario", idUsuario.ToString()),
                    new Claim("idRol", idRol.ToString()),
                    new Claim("nombre", nombre)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // leo el idUsuario desde el token del request actual, para no confiar en lo que mande el cliente por parametro
        public int ObtenerConsecutivoToken()
        {
            var valor = _httpContext.HttpContext?.User.FindFirstValue("idUsuario");
            return int.TryParse(valor, out var id) ? id : 0;
        }

        public int ObtenerIdRolToken()
        {
            var valor = _httpContext.HttpContext?.User.FindFirstValue("idRol");
            return int.TryParse(valor, out var id) ? id : 0;
        }

        public string ObtenerNombreToken()
        {
            return _httpContext.HttpContext?.User.FindFirstValue("nombre") ?? string.Empty;
        }
    }
}