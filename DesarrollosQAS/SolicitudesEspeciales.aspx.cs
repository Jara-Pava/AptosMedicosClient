using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DesarrollosQAS
{
    public partial class SolicitudesEspeciales : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        { 
            // Los catálogos SIEMPRE deben cargarse para que los ComboBox funcionen correctamente
            CargarCatalogos();

            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void gridSolicitudesEspeciales_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters.StartsWith("DELETE|"))
            {
                string[] parts = e.Parameters.Split('|');
                int visibleIndex = Convert.ToInt32(parts[1]);

                int id = Convert.ToInt32(gridSolicitudesEspeciales.GetRowValues(visibleIndex, "ID_Solicitud"));
                try
                {
                    var repo = new SolicitudRHRepository();
                    repo.EliminarSolicitudRH(id);

                    gridSolicitudesEspeciales.DataBind();
                    MostrarExito($"Proceso exitoso al eliminar la solicitud con el Folio {id}.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error al eliminar solicitud: {0}", ex);
                    MostrarError($"Proceso no exitoso al eliminar la solicitud con el Folio {id}: {ex.Message}");
                }
            }
        }

        private void CargarCatalogos()
        {
            try
            {
                var repo = new SolicitudRHRepository();

                // Cargar tipos de solicitud
                var tipoSolicitudColumn = gridSolicitudesEspeciales.Columns["id_TipoSolicitud"] as GridViewDataComboBoxColumn;
                if (tipoSolicitudColumn != null)
                {
                    tipoSolicitudColumn.PropertiesComboBox.DataSource = repo.ObtenerTiposSolicitud();
                    tipoSolicitudColumn.PropertiesComboBox.TextField = "Visita";
                    tipoSolicitudColumn.PropertiesComboBox.ValueField = "ID_TipoVisita";
                }

                // Cargar proyectos
                var proyectoColumn = gridSolicitudesEspeciales.Columns["id_Proyecto"] as GridViewDataComboBoxColumn;
                if (proyectoColumn != null)
                {
                    proyectoColumn.PropertiesComboBox.DataSource = repo.ObtenerProyectos();
                    proyectoColumn.PropertiesComboBox.TextField = "NombreProyecto";
                    proyectoColumn.PropertiesComboBox.ValueField = "ID_Proyecto";
                }

                // Cargar plantas
                var plantaColumn = gridSolicitudesEspeciales.Columns["id_Planta"] as GridViewDataComboBoxColumn;
                if (plantaColumn != null)
                {
                    plantaColumn.PropertiesComboBox.DataSource = repo.ObtenerPlantas();
                    plantaColumn.PropertiesComboBox.TextField = "NombrePlanta";
                    plantaColumn.PropertiesComboBox.ValueField = "ID_Planta";
                }

                // Cargar contratistas
                var contratistaColumn = gridSolicitudesEspeciales.Columns["id_Contratista"] as GridViewDataComboBoxColumn;
                if (contratistaColumn != null)
                {
                    contratistaColumn.PropertiesComboBox.DataSource = repo.ObtenerContratistas();
                    contratistaColumn.PropertiesComboBox.TextField = "Responsable";
                    contratistaColumn.PropertiesComboBox.ValueField = "id_contratista";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error al cargar catálogos: {0}", ex);
                MostrarError($"Proceso no exitoso al cargar los catalagos: {ex.Message}");
            }
        }

        private void BindGrid()
        {
            var repo = new SolicitudRHRepository();
            List<SolicitudRH> solicitudes = repo.ObtenerTodasSolicitudesRH();
            gridSolicitudesEspeciales.DataSource = solicitudes;
            gridSolicitudesEspeciales.DataBind();
        }

        protected void gridSolicitudesEspeciales_DataBinding(object sender, EventArgs e)
        {
            var repo = new SolicitudRHRepository();
            gridSolicitudesEspeciales.DataSource = repo.ObtenerTodasSolicitudesRH();
        }

        protected void gridSolicitudesEspeciales_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            // Este método ya no elimina directamente, solo se usa para el evento del cliente
        }

        protected void gridSolicitudesEspeciales_DataBound(object sender, EventArgs e) {
            SetColummnsWidth(sender as ASPxGridView);
        }

        private void SetColummnsWidth(ASPxGridView grid) {
            var demoAreaWidth = 894;
            var columnWidth = Math.Max(115, demoAreaWidth / grid.Columns.Count);
            for (var i = 1; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].MinWidth = columnWidth;
            }
            grid.Columns[0].MinWidth = demoAreaWidth - (grid.Columns.Count - 1) * columnWidth;
        }

        #region Métodos de Mensajes

        private void MostrarExito(string mensaje)
        {
            gridSolicitudesEspeciales.JSProperties["cpMessageType"] = "success";
            gridSolicitudesEspeciales.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarError(string mensaje)
        {
            gridSolicitudesEspeciales.JSProperties["cpMessageType"] = "error";
            gridSolicitudesEspeciales.JSProperties["cpMessage"] = mensaje;
        }
        #endregion
    }
}