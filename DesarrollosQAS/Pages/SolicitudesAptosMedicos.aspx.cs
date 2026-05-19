using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Xml;

namespace DesarrollosQAS.Pages
{
    public partial class SolicitudesAptosMedicos : System.Web.UI.Page
    {
        private const string SessionKeyBusqueda = "SolicitudesAptosMedicos_Busqueda";

        [Serializable]
        private class BusquedaParams
        {
            public string Tipo { get; set; }
            public string ValorTexto { get; set; }
            public string FechaSol { get; set; }
            public string FechaIni { get; set; }
            public string FechaFin { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove(SessionKeyBusqueda);
                gridSolicitudesAptosMedicos.DataBind();
            }
        }

        protected void gridSolicitudesAptosMedicos_DataBinding(object sender, EventArgs e)
        {
            // Si hay parámetros de búsqueda en sesión, los usamos para mostrar resultados al recargar la página
            var p = Session[SessionKeyBusqueda] as BusquedaParams
                    ?? new BusquedaParams { Tipo = "NONE" };

            var repo = new SolicitudesAptosMedicosRepository();
            List<SolicitudAptoMedico> resultado;

            try     
            {
                switch (p.Tipo)
                {
                    case "ID_SOLICITUD": resultado = repo.ConsultarPorId(p.ValorTexto).Solicitudes; break;
                    case "ID_GLOBAL": resultado = repo.ConsultarPorIdGlobal(p.ValorTexto).Solicitudes; break;
                    case "FECHA_SOLICITUD": resultado = repo.ConsultarPorFechaSolicitud(p.FechaSol).Solicitudes; break;
                    case "RANGO_FECHAS": resultado = repo.ConsultarPorFechas(p.FechaIni, p.FechaFin).Solicitudes; break;
                    default: resultado = new List<SolicitudAptoMedico>(); break;
                }
            }
            catch (Exception)
            {
                resultado = new List<SolicitudAptoMedico>();
            }

            gridSolicitudesAptosMedicos.DataSource = resultado;
        }

        protected void gridSolicitudesAptosMedicos_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            var parts = (e.Parameters ?? string.Empty).Split('|');

            var p = new BusquedaParams
            {
                Tipo = parts.Length > 1 ? parts[1] : "NONE",
                ValorTexto = parts.Length > 2 ? parts[2] : string.Empty,
                FechaSol = parts.Length > 3 ? parts[3] : string.Empty,
                FechaIni = parts.Length > 4 ? parts[4] : string.Empty,
                FechaFin = parts.Length > 5 ? parts[5] : string.Empty,
            };

            Session[SessionKeyBusqueda] = p;

            var repo = new SolicitudesAptosMedicosRepository();
            ResultadoConsulta resultado;

            try
            {
                switch (p.Tipo)
                {
                    case "ID_SOLICITUD": resultado = repo.ConsultarPorId(p.ValorTexto); break;
                    case "ID_GLOBAL": resultado = repo.ConsultarPorIdGlobal(p.ValorTexto); break;
                    case "FECHA_SOLICITUD": resultado = repo.ConsultarPorFechaSolicitud(p.FechaSol); break;
                    case "RANGO_FECHAS": resultado = repo.ConsultarPorFechas(p.FechaIni, p.FechaFin); break;
                    default: resultado = repo.ConsultarTodas(); break;
                }

                var sinResultados = !string.IsNullOrEmpty(resultado.Mensaje);
                gridSolicitudesAptosMedicos.JSProperties["cpEstadoBusqueda"] = sinResultados ? "sin_resultados" : "ok";
                gridSolicitudesAptosMedicos.JSProperties["cpTotalRegistros"] = resultado.Solicitudes.Count;
                gridSolicitudesAptosMedicos.JSProperties["cpJsonRespuesta"] = resultado.JsonRespuesta;
                if (sinResultados)
                    gridSolicitudesAptosMedicos.JSProperties["cpMensajeError"] = resultado.Mensaje;
                gridSolicitudesAptosMedicos.DataSource = resultado.Solicitudes;
            }
            catch (Exception ex)
            {
                var jsonError = ex.Data.Contains("JsonRespuesta")
                    ? ex.Data["JsonRespuesta"]?.ToString()
                    : string.Format("{{\"error\":\"{0}\"}}", ex.Message.Replace("\"", "\\\""));

                gridSolicitudesAptosMedicos.JSProperties["cpEstadoBusqueda"] = "error";
                gridSolicitudesAptosMedicos.JSProperties["cpMensajeError"] = ex.Message;
                gridSolicitudesAptosMedicos.JSProperties["cpJsonRespuesta"] = jsonError;
                Session.Remove(SessionKeyBusqueda);
                gridSolicitudesAptosMedicos.DataSource = new List<SolicitudAptoMedico>();
            }

            gridSolicitudesAptosMedicos.DataBind();
        }

        protected void gridSolicitudesAptosMedicos_DataBound(object sender, EventArgs e)
        {
            SetColummnsWidth(sender as ASPxGridView);
        }

        private void SetColummnsWidth(ASPxGridView grid)
        {
            var demoAreaWidth = 894;
            var columnWidth = Math.Max(115, demoAreaWidth / grid.Columns.Count);
            for (var i = 1; i < grid.Columns.Count; i++)
                grid.Columns[i].MinWidth = columnWidth;
            grid.Columns[0].MinWidth = demoAreaWidth - (grid.Columns.Count - 1) * columnWidth;
        }
    }
}