using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos
{
    // ── Resultado con JSON crudo incluido ─────────────────────────────────────
    public class ResultadoConsulta
    {
        public List<SolicitudAptoMedico> Solicitudes { get; set; } = new List<SolicitudAptoMedico>();
        public string JsonRespuesta { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }

    public class NotFoundResponse
    {
        public string Mensaje { get; set; } = string.Empty;
    }
}
