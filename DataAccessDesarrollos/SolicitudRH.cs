using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos
{
    public class SolicitudRH
    {
        public int ID_Solicitud { get; set; }
        public int id_TipoSolicitud { get; set; }
        public int id_Solicitante { get; set; }
        public int id_Proyecto { get; set; }
        public int id_Planta { get; set; }
        public string Visitante { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string RFC { get; set; }
        public int id_Contratista { get; set; }
        public string Responsable { get; set; }
        public string AreaTrabajo { get; set; }
        public string Actividad { get; set; }
        public string Estancia { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public bool aprobado { get; set; }

        // Propiedades para mostrar nombres
        public string NombreTipoSolicitud { get; set; }
        public string NombreSolicitante { get; set; }
        public string NombreProyecto { get; set; }
        public string NombrePlanta { get; set; }
        public string NombreContratista { get; set; }
    }

    public class TipoVisitante
    {
        public int ID_TipoVisita { get; set; }
        public string Visita { get; set; }
        public bool Activo { get; set; }
        public string Estancia { get; set; }
    }

    public class Proyecto
    {
        public int ID_Proyecto { get; set; }
        public string NombreProyecto { get; set; }
        public bool Activo { get; set; }
    }

    public class Planta
    {
        public int ID_Planta { get; set; }
        public string NombrePlanta { get; set; }
        public bool Activo { get; set; }
    }

    public class EmpresaContratista
    {
        public int id_contratista { get; set; }
        public string Nombre { get; set; }
        public string RFC { get; set; }
        public string Responsable { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
    }
}
