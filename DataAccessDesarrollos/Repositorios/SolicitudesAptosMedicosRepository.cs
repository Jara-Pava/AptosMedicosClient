using DataAccessDesarrollos.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessDesarrollos.Repositorios
{
    public class SolicitudesAptosMedicosRepository : ISolicitudesAptosMedicos
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ApiBaseUrl = "http://teimxwas125:5050/api/AptosMedicos/";
        public SolicitudAptoMedico ObtenerSolicitudAptoMedicoPorId(int id)
        {
            throw new NotImplementedException();
        }

        public List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            throw new NotImplementedException();
        }

        public List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorFechaSolicitud(DateTime fechaSolicitud)
        {
            throw new NotImplementedException();
        }

        public List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorIdGlobal(string idGlobal)
        {
            throw new NotImplementedException();
        }

        public List<SolicitudAptoMedico> ObtenerTodasSolicitudesAptosMedicos()
        {
            try {
                var task = _httpClient.GetStringAsync(ApiBaseUrl);
                task.Wait();
                var jsonResponse = task.Result;
                var respuesta = JsonConvert.DeserializeObject<AptosMedicosApiResponse>(jsonResponse);
                return respuesta?.Solicitudes ?? new List<SolicitudAptoMedico>();
            }
            catch (Exception ex){
                Trace.TraceError("Error ObtenerTodasSolicitudesAptosMedicos: {0}", ex);
                return new List<SolicitudAptoMedico>();
            }
        }
    }
}
