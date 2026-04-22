using DataAccessDesarrollos.Repositorios;
using System;
using System.Linq;
using System.Web;

namespace DesarrollosQAS.Model
{
    public class ApplicationUser
    {
        public int IdUsuario { get; set; }
        public string Nombre { get;set; }
        public string Sigla_red { get; set; }
        public bool Activo { get; set; }
        public string Email { get; set; }
        public string Creado_por { get; set; }
        public string Modificado_por { get; set; }
    }

    public static class AuthHelper
    {
        /// <summary>
        /// Autentica al usuario buscándolo en SEC_Usuarios por sigla_red
        /// </summary>
        public static bool SignIn(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            try
            {
                var repo = new UsuarioSistemaRepository();
                var usuarios = repo.ObtenerTodosUsuarios();
                var usuario = usuarios.FirstOrDefault(u =>
                    u.sigla_red != null &&
                    u.sigla_red.Trim().Equals(userName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    u.activo);

                if (usuario == null)
                    return false;

                if(!usuario.activo)
                    return false;
                // Guardar el usuario autenticado en sesión
                HttpContext.Current.Session["User"] = new ApplicationUser
                {
                    IdUsuario = usuario.id_usuario,
                    Nombre = usuario.nombre,
                    Sigla_red = usuario.sigla_red,
                    Email = usuario.Email,
                    Activo = usuario.activo,
                    Creado_por = usuario.creado_por,
                    Modificado_por = usuario.modificado_por
                };

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error en SignIn: {0}", ex);
                return false;
            }
        }

        public static void SignOut()
        {
            HttpContext.Current.Session["User"] = null;
        }

        public static bool IsAuthenticated()
        {
            return GetLoggedInUserInfo() != null;
        }

        public static ApplicationUser GetLoggedInUserInfo()
        {
            return HttpContext.Current.Session["User"] as ApplicationUser;
        }

        /// <summary>
        /// Obtiene el ID del usuario logueado actualmente
        /// </summary>
        public static int GetCurrentUserId()
        {
            var user = GetLoggedInUserInfo();
            return user?.IdUsuario ?? 0;
        }
    }
}