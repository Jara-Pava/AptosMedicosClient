using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos
{
    public class SolicitudAptoMedico
    {
        public int Id_Solicitud { get; set; }
        public string Nombre_Solicitante { get; set; }
        public string Proyecto { get; set; }
        public string Id_Global { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string No_Identidad { get; set; }
        public DateTime? Fecha_Nacimiento { get; set; }
        public DateTime? Fecha_Solicitud { get; set; }
        public string nombre_contratista { get; set; }
        public string Puesto { get; set; }
        public string Medico_Asigna_Examen { get; set; }
        public string Medico_Apto { get; set; }
        public string Nombre_Tipo_Examen { get; set; }
        public string Apto { get; set; }
        public DateTime? Fecha_Diagnostico { get; set; }
        public DateTime? Fecha_Vigencia { get; set; }
        public string Motivo_Cancelacion { get; set; }
        public int? Edad { get; set; }
        public string Sexo { get; set; }
    }
}
