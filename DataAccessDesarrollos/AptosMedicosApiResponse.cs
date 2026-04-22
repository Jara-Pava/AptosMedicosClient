using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos
{
    public class AptosMedicosApiResponse
    {
        public int TotalSolicitudes { get; set; }
        public List<SolicitudAptoMedico> Solicitudes { get; set; }
    }
}
