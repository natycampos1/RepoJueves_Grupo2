namespace API_GRUPODOS.Services
{
    public interface IUtilesService
    {
        string GenerarContrasena();
        Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml);

        string GenerarToken(int idUsuario, int idRol, string nombre);
        int ObtenerConsecutivoToken();
        int ObtenerIdRolToken();
        string ObtenerNombreToken();
    }
}