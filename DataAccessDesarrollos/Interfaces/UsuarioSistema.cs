using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos.Interfaces
{
    public interface IUsuarioSistema
    {
        List<Usuario> ObtenerTodosUsuarios();
        bool CrearUsuario(Usuario usuario);
        bool ActualizarUsuario(Usuario usuario);
        bool EliminarUsuario(int id);
    }
}
