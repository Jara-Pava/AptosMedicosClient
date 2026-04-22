using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos
{
    public class Rol
    {
        public int id_rol { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
        public int? creado_por { get; set; }
        public string nombre_creador { get; set; }
    }
}
