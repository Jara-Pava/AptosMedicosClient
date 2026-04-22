using System;

namespace DataAccessDesarrollos
{
    public class ModuloCatalogo
    {
        public int id_modulo_catalogo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
        public int? creado_por { get; set; }
        public string nombre_creador { get; set; }
    }
}