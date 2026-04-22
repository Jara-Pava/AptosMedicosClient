using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos.Interfaces
{
    public interface ISolicitudesAptosMedicos
    {
        List<SolicitudAptoMedico> ObtenerTodasSolicitudesAptosMedicos();
        SolicitudAptoMedico ObtenerSolicitudAptoMedicoPorId(int id);
        List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorIdGlobal(string idGlobal);
        List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorFechaSolicitud(DateTime fechaSolicitud);
        List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorFechas(DateTime fechaInicio, DateTime fechaFin);
    }
}
