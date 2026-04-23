using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Web.UI;

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
                // DataBind() → dispara DataBinding → carga todos los registros
                gridSolicitudesAptosMedicos.DataBind();
            }
        }

        // ── ÚNICO lugar donde se asigna DataSource ────────────────────────────────
        // Se dispara en: paginación, ordenamiento, DataBind() explícito, etc.
        protected void gridSolicitudesAptosMedicos_DataBinding(object sender, EventArgs e)
        {
            var p = Session[SessionKeyBusqueda] as BusquedaParams
                    ?? new BusquedaParams { Tipo = "NONE" };

            var repo = new SolicitudesAptosMedicosRepository();
            List<SolicitudAptoMedico> resultado;

            try
            {
                switch (p.Tipo)
                {
                    case "ID_SOLICITUD":
                        resultado = repo.ObtenerSolicitudAptoMedicoPorIdStr(p.ValorTexto);
                        break;
                    case "ID_GLOBAL":
                        resultado = repo.ObtenerSolicitudesAptosMedicosPorIdGlobal(p.ValorTexto);
                        break;
                    case "FECHA_SOLICITUD":
                        resultado = repo.ObtenerSolicitudesAptosMedicosPorFechaSolicitudStr(p.FechaSol);
                        break;
                    case "RANGO_FECHAS":
                        resultado = repo.ObtenerSolicitudesAptosMedicosPorFechasStr(p.FechaIni, p.FechaFin);
                        break;
                    default:
                        resultado = new List<SolicitudAptoMedico>();
                        break;
                }
            }
            catch (Exception)
            {
                resultado = new List<SolicitudAptoMedico>();
            }

            gridSolicitudesAptosMedicos.DataSource = resultado;
        }

        // ── CustomCallback: guarda en Session y dispara DataBind ─────────────────
        protected void gridSolicitudesAptosMedicos_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            var parts = (e.Parameters ?? string.Empty).Split('|');
            if (parts.Length < 2 || parts[0] != "SEARCH") return;

            var p = new BusquedaParams
            {
                Tipo = parts.Length > 1 ? parts[1] : "NONE",
                ValorTexto = parts.Length > 2 ? parts[2] : string.Empty,
                FechaSol = parts.Length > 3 ? parts[3] : string.Empty,
                FechaIni = parts.Length > 4 ? parts[4] : string.Empty,
                FechaFin = parts.Length > 5 ? parts[5] : string.Empty,
            };

            // Guardar en Session ANTES del DataBind para que DataBinding los lea
            Session[SessionKeyBusqueda] = p;

            try
            {
                gridSolicitudesAptosMedicos.DataBind();
            }
            catch (Exception ex)
            {
                gridSolicitudesAptosMedicos.JSProperties["cpMessageType"] = "error";
                gridSolicitudesAptosMedicos.JSProperties["cpMessage"] = ex.Message;
                Session.Remove(SessionKeyBusqueda);
                gridSolicitudesAptosMedicos.DataBind();
            }
        }

        protected void gridSolicitudesAptosMedicos_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
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