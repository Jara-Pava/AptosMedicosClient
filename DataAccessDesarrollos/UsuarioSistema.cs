using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public string sigla_red { get; set; }
        public bool activo { get; set; }
        public string Email { get; set; }
        public string creado_por { get; set; }
        public string modificado_por { get; set; }
    }

    public class UsuarioRol
    {
        public int id_usuario_rol { get; set; }
        public int id_usuario { get; set; }
        public int id_rol { get; set; }
        public string nombre_usuario { get; set; }
        public string nombre_rol { get; set; }
        public DateTime fecha_asignacion { get; set; }
        public string asignado_por { get; set; }
    }

    public class Catalogo
    {
        public int id_catalogo { get; set; }
        public string nombre_catalogo { get; set; }
        public string descripcion { get; set; }
        public string url { get; set; }
        public string icono { get; set; }
        public int orden { get; set; }
        public bool activo { get; set; }
    }

    public class RolCatalogo
    {
        public int id_catalogo { get; set; }
        public string nombre_catalogo { get; set; }
        public string descripcion { get; set; }
        public string url { get; set; }
        public bool puede_ver { get; set; }
        public bool puede_crear { get; set; }
        public bool puede_editar { get; set; }
        public bool puede_eliminar { get; set; }
    }
}
