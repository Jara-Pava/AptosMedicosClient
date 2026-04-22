using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos.Interfaces
{
    public interface IRoles
    {
        // Obtenemos todos los roles
        List<Rol> ObtenerRoles();
        // Obtenemos un rol por su ID
        Rol ObtenerRolPorId(int id);

        // Creamos un nuevo rol
        bool CrearRol(Rol rol);

        // Actualizamos un rol existente
        bool ActualizarRol(Rol rol);

        // Eliminamos un rol por su ID
        void EliminarRol(int id);

        // Obtenemos rol por usuario
        Rol ObtenerRolPorUsuario(int idUsuario);
    }
}
