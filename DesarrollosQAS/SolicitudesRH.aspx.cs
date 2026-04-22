using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using System;
using System.Collections.Generic;

namespace DesarrollosQAS
{
    public partial class SolicitudesRH : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Los catálogos SIEMPRE deben cargarse para que los ComboBox funcionen correctamente
            CargarCatalogos();

            if (!IsPostBack)
            {
                BindGrid();
            }

            // Aplicar estilos al popup en cada carga
            gridSolicitudesRH.StylesPopup.EditForm.Header.BackColor = System.Drawing.ColorTranslator.FromHtml("#353943");
            gridSolicitudesRH.StylesPopup.EditForm.Header.ForeColor = System.Drawing.Color.White;
            gridSolicitudesRH.StylesPopup.EditForm.Header.Font.Bold = true;
        }

        protected void gridSolicitudesRH_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters.StartsWith("DELETE|"))
            {
                try
                {
                    string[] parts = e.Parameters.Split('|');
                    int visibleIndex = Convert.ToInt32(parts[1]);

                    int id = Convert.ToInt32(gridSolicitudesRH.GetRowValues(visibleIndex, "ID_Solicitud"));
                    var repo = new SolicitudRHRepository();
                    repo.EliminarSolicitudRH(id);

                    gridSolicitudesRH.DataBind();
                    MostrarExito("Solicitud eliminada exitosamente.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error al eliminar solicitud: {0}", ex);
                    MostrarError($"Error al eliminar: {ex.Message}");
                }
            }
        }

        private void CargarCatalogos()
        {
            try
            {
                var repo = new SolicitudRHRepository();

                // Cargar tipos de solicitud
                var tipoSolicitudColumn = gridSolicitudesRH.Columns["id_TipoSolicitud"] as GridViewDataComboBoxColumn;
                if (tipoSolicitudColumn != null)
                {
                    tipoSolicitudColumn.PropertiesComboBox.DataSource = repo.ObtenerTiposSolicitud();
                    tipoSolicitudColumn.PropertiesComboBox.TextField = "Visita";
                    tipoSolicitudColumn.PropertiesComboBox.ValueField = "ID_TipoVisita";
                }

                // Cargar proyectos
                var proyectoColumn = gridSolicitudesRH.Columns["id_Proyecto"] as GridViewDataComboBoxColumn;
                if (proyectoColumn != null)
                {
                    proyectoColumn.PropertiesComboBox.DataSource = repo.ObtenerProyectos();
                    proyectoColumn.PropertiesComboBox.TextField = "NombreProyecto";
                    proyectoColumn.PropertiesComboBox.ValueField = "ID_Proyecto";
                }

                // Cargar plantas
                var plantaColumn = gridSolicitudesRH.Columns["id_Planta"] as GridViewDataComboBoxColumn;
                if (plantaColumn != null)
                {
                    plantaColumn.PropertiesComboBox.DataSource = repo.ObtenerPlantas();
                    plantaColumn.PropertiesComboBox.TextField = "NombrePlanta";
                    plantaColumn.PropertiesComboBox.ValueField = "ID_Planta";
                }

                // Cargar contratistas
                var contratistaColumn = gridSolicitudesRH.Columns["id_Contratista"] as GridViewDataComboBoxColumn;
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
                MostrarMensaje($"Error al cargar catálogos: {ex.Message}", false);
            }
        }

        private void BindGrid()
        {
            var repo = new SolicitudRHRepository();
            List<SolicitudRH> solicitudes = repo.ObtenerTodasSolicitudesRH();
            gridSolicitudesRH.DataSource = solicitudes;
            gridSolicitudesRH.DataBind();
        }

        protected void gridUsuarios_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            // Cargar catálogos en los ComboBox del EditForm template
            var repo = new SolicitudRHRepository();

            // Tipo Solicitud
            ASPxComboBox cboTipoSolicitud = gridSolicitudesRH.FindEditFormTemplateControl("cboTipoSolicitudEdit") as ASPxComboBox;
            if (cboTipoSolicitud != null)
            {
                cboTipoSolicitud.DataSource = repo.ObtenerTiposSolicitud();
                cboTipoSolicitud.TextField = "Visita";
                cboTipoSolicitud.ValueField = "ID_TipoVisita";
            }

            // Proyecto
            ASPxComboBox cboProyecto = gridSolicitudesRH.FindEditFormTemplateControl("cboProyectoEdit") as ASPxComboBox;
            if (cboProyecto != null)
            {
                cboProyecto.DataSource = repo.ObtenerProyectos();
                cboProyecto.TextField = "NombreProyecto";
                cboProyecto.ValueField = "ID_Proyecto";
            }

            // Planta
            ASPxComboBox cboPlanta = gridSolicitudesRH.FindEditFormTemplateControl("cboPlantaEdit") as ASPxComboBox;
            if (cboPlanta != null)
            {
                cboPlanta.DataSource = repo.ObtenerPlantas();
                cboPlanta.TextField = "NombrePlanta";
                cboPlanta.ValueField = "ID_Planta";
            }

            // Contratista
            ASPxComboBox cboContratista = gridSolicitudesRH.FindEditFormTemplateControl("cboContratistaEdit") as ASPxComboBox;
            if (cboContratista != null)
            {
                cboContratista.DataSource = repo.ObtenerContratistas();
                cboContratista.TextField = "Responsable";
                cboContratista.ValueField = "id_contratista";
            }

            // Configurar evento Click del botón Guardar
            ASPxButton btnUpdate = gridSolicitudesRH.FindEditFormTemplateControl("btnUpdate") as ASPxButton;
            if (btnUpdate != null)
            {
                btnUpdate.Click += BtnUpdate_Click;
            }

            // Controlar visibilidad del checkbox Aprobado según el modo
            ASPxCheckBox chkAprobado = gridSolicitudesRH.FindEditFormTemplateControl("chkAprobadoEdit") as ASPxCheckBox;
            ASPxLabel lblAprobado = gridSolicitudesRH.FindEditFormTemplateControl("lblAprobadoEdit") as ASPxLabel;
            if (chkAprobado != null)
            {
                // Ocultar en modo inserción, mostrar en modo edición
                chkAprobado.Visible = !gridSolicitudesRH.IsNewRowEditing;
                if (lblAprobado != null)
                {
                    lblAprobado.Visible = !gridSolicitudesRH.IsNewRowEditing;
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener valores del formulario
                ASPxComboBox cboTipoSolicitud = gridSolicitudesRH.FindEditFormTemplateControl("cboTipoSolicitudEdit") as ASPxComboBox;
                ASPxTextBox txtVisitante = gridSolicitudesRH.FindEditFormTemplateControl("txtVisitanteEdit") as ASPxTextBox;
                ASPxComboBox cboProyecto = gridSolicitudesRH.FindEditFormTemplateControl("cboProyectoEdit") as ASPxComboBox;
                ASPxComboBox cboPlanta = gridSolicitudesRH.FindEditFormTemplateControl("cboPlantaEdit") as ASPxComboBox;
                ASPxComboBox cboContratista = gridSolicitudesRH.FindEditFormTemplateControl("cboContratistaEdit") as ASPxComboBox;
                ASPxTextBox txtAreaTrabajo = gridSolicitudesRH.FindEditFormTemplateControl("txtAreaTrabajoEdit") as ASPxTextBox;
                ASPxMemo txtActividad = gridSolicitudesRH.FindEditFormTemplateControl("txtActividadEdit") as ASPxMemo;
                ASPxTextBox txtResponsable = gridSolicitudesRH.FindEditFormTemplateControl("txtResponsableEdit") as ASPxTextBox;
                ASPxTextBox txtEstancia = gridSolicitudesRH.FindEditFormTemplateControl("txtEstanciaEdit") as ASPxTextBox;
                ASPxTextBox txtRFC = gridSolicitudesRH.FindEditFormTemplateControl("txtRFCEdit") as ASPxTextBox;
                ASPxDateEdit dteFechaInicio = gridSolicitudesRH.FindEditFormTemplateControl("dteFechaInicioEdit") as ASPxDateEdit;
                ASPxDateEdit dteFechaFin = gridSolicitudesRH.FindEditFormTemplateControl("dteFechaFinEdit") as ASPxDateEdit;
                ASPxCheckBox chkAprobado = gridSolicitudesRH.FindEditFormTemplateControl("chkAprobadoEdit") as ASPxCheckBox;

                var solicitud = new SolicitudRH
                {
                    id_TipoSolicitud = cboTipoSolicitud.Value != null ? Convert.ToInt32(cboTipoSolicitud.Value) : 0,
                    id_Solicitante = 0,
                    id_Proyecto = cboProyecto.Value != null ? Convert.ToInt32(cboProyecto.Value) : 0,
                    id_Planta = cboPlanta.Value != null ? Convert.ToInt32(cboPlanta.Value) : 0,
                    Visitante = txtVisitante.Text.Trim(),
                    FechaInicio = dteFechaInicio.Value != null ? Convert.ToDateTime(dteFechaInicio.Value) : default(DateTime),
                    FechaFin = dteFechaFin.Value != null ? Convert.ToDateTime(dteFechaFin.Value) : default(DateTime),
                    RFC = txtRFC.Text.Trim(),
                    id_Contratista = cboContratista.Value != null ? Convert.ToInt32(cboContratista.Value) : 0,
                    Responsable = txtResponsable.Text.Trim(),
                    AreaTrabajo = txtAreaTrabajo.Text.Trim(),
                    Actividad = txtActividad.Text.Trim(),
                    Estancia = txtEstancia.Text.Trim(),
                    aprobado = chkAprobado != null ? chkAprobado.Checked : false
                };

                var repo = new SolicitudRHRepository();
                bool resultado = false;
                string mensaje = string.Empty;

                if (gridSolicitudesRH.IsNewRowEditing)
                {
                    // Modo creación
                    solicitud.FechaSolicitud = DateTime.Now;
                    resultado = repo.CrearSolicitudRH(solicitud);
                    mensaje = resultado
                        ? $"Proceso exitoso al crear la solicitud con el Folio: {solicitud.ID_Solicitud}"
                        : $"Proceso no exitoso al crear la solicitud, verifique que los campos tengan la información correcta.";
                }
                else
                {
                    // Modo edición
                    solicitud.ID_Solicitud = Convert.ToInt32(gridSolicitudesRH.GetRowValues(gridSolicitudesRH.EditingRowVisibleIndex, "ID_Solicitud"));
                    resultado = repo.ActualizarSolicitudRH(solicitud);
                    mensaje = resultado
                        ? $"Proceso exitoso al actualizar solicitud con el Folio: {solicitud.ID_Solicitud}"
                        : $"Proceso no exitoso al actualizar la solicitud con el folio: {solicitud.ID_Solicitud}.";
                }

                // Cancelar edición y actualizar grid
                gridSolicitudesRH.CancelEdit();
                gridSolicitudesRH.DataBind();

                // Mostrar mensaje usando el mismo método que SolicitudEspecial
                MostrarMensaje(mensaje, resultado);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error al guardar solicitud: {0}", ex);
                gridSolicitudesRH.CancelEdit();
                MostrarMensaje($"Proceso no exitoso al guardar la solicitud: {ex.Message}", false);
            }
        }

        protected void gridSolicitudesRH_DataBinding(object sender, EventArgs e)
        {
            var repo = new SolicitudRHRepository();
            gridSolicitudesRH.DataSource = repo.ObtenerTodasSolicitudesRH();
        }

        protected void gridSolicitudesRH_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            // Este método ya no elimina directamente, solo se usa para el evento del cliente
        }

        protected void gridSolicitudesRH_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            // Este evento ya no se usa, se maneja en BtnUpdate_Click
            e.Cancel = true;
        }

        protected void gridSolicitudesRH_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            // Este evento ya no se usa, se maneja en BtnUpdate_Click
            e.Cancel = true;
        }

        #region Métodos de Mensajes

        private void MostrarExito(string mensaje)
        {
            gridSolicitudesRH.JSProperties["cpMessageType"] = "success";
            gridSolicitudesRH.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarError(string mensaje)
        {
            gridSolicitudesRH.JSProperties["cpMessageType"] = "error";
            gridSolicitudesRH.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarMensaje(string mensaje, bool esExito)
        {
            MostrarPopup(mensaje, esExito);
        }

        private void MostrarPopup(string mensaje, bool esExito)
        {
            // Escapar caracteres especiales para JavaScript
            string mensajeEscapado = mensaje
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", "\\n");

            string script = string.Empty;

            if (esExito)
            {
                script = string.Format(@"
                    window.onload = function() {{
                        setTimeout(function() {{
                            if (typeof lblMensajeExito !== 'undefined' && lblMensajeExito) {{
                                lblMensajeExito.SetText('{0}');
                                pcMensajeExito.Show();
                            }} else {{
                                console.error('lblMensajeExito no está definido');
                            }}
                        }}, 100);
                    }};
                ", mensajeEscapado);
            }
            else
            {
                script = string.Format(@"
                    window.onload = function() {{
                        setTimeout(function() {{
                            if (typeof lblMensajeError !== 'undefined' && lblMensajeError) {{
                                lblMensajeError.SetText('{0}');
                                pcMensajeError.Show();
                            }} else {{
                                console.error('lblMensajeError no está definido');
                            }}
                        }}, 100);
                    }};
                ", mensajeEscapado);
            }

            // Usar Page.ClientScript que funciona mejor con Master Pages
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "MostrarPopup_" + Guid.NewGuid().ToString(),
                script,
                true
            );
        }

        #endregion
    }
}