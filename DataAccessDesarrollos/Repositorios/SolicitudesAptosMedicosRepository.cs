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
        // ── Helper central ────────────────────────────────────────────────────────
        // Realiza la petición GET. Si la API devuelve error, lanza Exception con el
        // mensaje que retorna el API (ErrorResponse.Mensaje + Errores).
        private List<SolicitudAptoMedico> ConsultarApi(string url)
        {
            var task = _httpClient.GetAsync(url);
            task.Wait();
            var response = task.Result;

            var readTask = response.Content.ReadAsStringAsync();
            readTask.Wait();
            var json = readTask.Result;

            // 404 = sin resultados, no es un error de parámetros
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new List<SolicitudAptoMedico>();

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponseAptosMedicos>(json);
                var mensaje = !string.IsNullOrWhiteSpace(error?.Mensaje)
                    ? error.Mensaje
                    : string.Format("Error {0} al consultar el API.", (int)response.StatusCode);
                if (error?.Errores != null && error.Errores.Count > 0)
                    mensaje += ": " + string.Join(", ", error.Errores);
                throw new Exception(mensaje);
            }

            var respuesta = JsonConvert.DeserializeObject<AptosMedicosApiResponse>(json);
            return respuesta?.Solicitudes ?? new List<SolicitudAptoMedico>();
        }

        // ── Métodos públicos ──────────────────────────────────────────────────────

        /// <summary>Busca por ID de solicitud (string crudo, el API valida el tipo).</summary>
        public List<SolicitudAptoMedico> ObtenerSolicitudAptoMedicoPorIdStr(string id)
        {
            return ConsultarApi(ApiBaseUrl + id);
        }

        /// <summary>Busca por ID Global.</summary>
        public List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorIdGlobal(string idGlobal)
        {
            return ConsultarApi(ApiBaseUrl + "global/" + Uri.EscapeDataString(idGlobal));
        }

        /// <summary>Busca por fecha de solicitud (string crudo yyyy-MM-dd, el API valida).</summary>
        public List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorFechaSolicitudStr(string fecha)
        {
            return ConsultarApi(ApiBaseUrl + "fecha/" + fecha);
        }

        /// <summary>Busca por rango de fechas (strings crudos yyyy-MM-dd, el API valida).</summary>
        public List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorFechasStr(string fechaInicio, string fechaFin)
        {
            return ConsultarApi(ApiBaseUrl + "fecha/" + fechaInicio + "/" + fechaFin);
        }

        /// <summary>Obtiene todas las solicitudes.</summary>
        public List<SolicitudAptoMedico> ObtenerTodasSolicitudesAptosMedicos()
        {
            return ConsultarApi(ApiBaseUrl);
        }

        // ── Métodos tipados (compatibilidad con la interfaz) ──────────────────────
        public SolicitudAptoMedico ObtenerSolicitudAptoMedicoPorId(int id)
        {
            return ConsultarApi(ApiBaseUrl + id).FirstOrDefault();
        }

        public List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorFechaSolicitud(DateTime fechaSolicitud)
        {
            return ConsultarApi(ApiBaseUrl + "fecha/" + fechaSolicitud.ToString("yyyy-MM-dd"));
        }

        public List<SolicitudAptoMedico> ObtenerSolicitudesAptosMedicosPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return ConsultarApi(ApiBaseUrl + "fecha/" + fechaInicio.ToString("yyyy-MM-dd") + "/" + fechaFin.ToString("yyyy-MM-dd"));
        }
    }
}