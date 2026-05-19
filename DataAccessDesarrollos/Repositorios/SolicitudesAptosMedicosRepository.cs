using DataAccessDesarrollos.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace DataAccessDesarrollos.Repositorios
{
    public class SolicitudesAptosMedicosRepository : ISolicitudesAptosMedicos
    {
        private static readonly string ApiBaseUrl =
            ConfigurationManager.AppSettings["AptosMedicos.ApiBaseUrl"]
            ?? "http://teimxwas125:5050/api/AptosMedicos/";

        private static readonly HttpClient _httpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var url = ConfigurationManager.AppSettings["AptosMedicos.ApiBaseUrl"] ?? string.Empty;
            if (url.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, cert, chain, errors) => true;
            }
            return new HttpClient();
        }

        // ── Helper central ────────────────────────────────────────────────────────
        private ResultadoConsulta ConsultarApi(string url)
        {
            // Creamos una tarea para realizar la consulta HTTP
            var task = _httpClient.GetAsync(url);
            // Esperamos a que la tarea se complete
            task.Wait();
            // Obtenemos la respuesta
            var response = task.Result;
            // Leemos el contenido de la respuesta como string (JSON)
            var readTask = response.Content.ReadAsStringAsync();
            // Esperamos a que la lectura se complete
            readTask.Wait();
            // Obtenemos el JSON como string
            var json = readTask.Result;
            // Si el código de estado es 404, deserializamos el mensaje de "no encontrado" y lo devolvemos en el resultado
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var notFound = JsonConvert.DeserializeObject<NotFoundResponse>(json);
                return new ResultadoConsulta
                {
                    JsonRespuesta = json,
                    Mensaje = notFound?.Mensaje ?? "No se encontraron registros."
                };
            }
            // Si el código de estado no es exitoso (200-299), intentamos deserializar un mensaje de error y lanzamos una excepción con ese mensaje
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponseAptosMedicos>(json);
                var mensaje = "Errores";
                //    ? error.Mensaje
                //    : string.Format("Error {0} al consultar el API.", (int)response.StatusCode);
                if (error?.Errores != null && error.Errores.Count > 0)
                    mensaje += ": " + string.Join(", ", error.Errores);
                // Lanzamos una excepción con el mensaje de error y adjuntamos el JSON completo en los datos de la excepción para referencia
                var ex = new Exception(mensaje);
                ex.Data["JsonRespuesta"] = json;
                throw ex;
            }
            // Si la respuesta es exitosa, deserializamos el JSON a un objeto de tipo AptosMedicosApiResponse (que contiene una lista de solicitudes)
            var respuesta = JsonConvert.DeserializeObject<AptosMedicosApiResponse>(json);
            return new ResultadoConsulta
            {
                Solicitudes = respuesta?.Solicitudes ?? new List<SolicitudAptoMedico>(),
                JsonRespuesta = json
            };
        }

        // ── Métodos que retornan ResultadoConsultado
        public ResultadoConsulta ConsultarPorId(string id)
        {
            var segmento = string.IsNullOrWhiteSpace(id) ? "null" : id;
            return ConsultarApi(ApiBaseUrl + "solicitud/" + segmento);
        }

        public ResultadoConsulta ConsultarPorIdGlobal(string idGlobal)
        {
            var segmento = string.IsNullOrWhiteSpace(idGlobal) ? "null" : Uri.EscapeDataString(idGlobal);
            return ConsultarApi(ApiBaseUrl + "global/" + segmento);
        }

        public ResultadoConsulta ConsultarPorFechaSolicitud(string fecha)
        {
            var segmento = string.IsNullOrWhiteSpace(fecha) ? "null" : fecha;
            return ConsultarApi(ApiBaseUrl + "fecha/" + segmento);
        }

        public ResultadoConsulta ConsultarPorFechas(string fechaInicio, string fechaFin)
        {
            var ini = string.IsNullOrWhiteSpace(fechaInicio) ? "null" : fechaInicio;
            var fin = string.IsNullOrWhiteSpace(fechaFin)    ? "null" : fechaFin;
            return ConsultarApi(ApiBaseUrl + "fecha/" + ini + "/" + fin);
        }

        public ResultadoConsulta ConsultarTodas()
            => ConsultarApi(ApiBaseUrl);
    }
}