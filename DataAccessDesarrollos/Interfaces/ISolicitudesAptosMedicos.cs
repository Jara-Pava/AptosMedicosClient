using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos.Interfaces
{
    public interface ISolicitudesAptosMedicos
    {
        ResultadoConsulta ConsultarPorId(string id);
        ResultadoConsulta ConsultarPorIdGlobal(string idGlobal);
        ResultadoConsulta ConsultarPorFechaSolicitud(string fecha);
        ResultadoConsulta ConsultarPorFechas(string fecha_inicio, string fecha_fin);

    }
}
