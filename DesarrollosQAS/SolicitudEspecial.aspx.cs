using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DesarrollosQAS.UserControls;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DesarrollosQAS
{
    public partial class SolicitudEspecial : System.Web.UI.Page
    {
        private int? IdSolicitud
        {
            get
            {
                if (ViewState["IdSolicitud"] != null)
                    return (int)ViewState["IdSolicitud"];
                return null;
            }
            set
            {
                ViewState["IdSolicitud"] = value;
            }
        }

        private PopupMessages popupMessages
        {
            get
            {
                return this.FindControl("popupMessages") as PopupMessages;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCatalogos();
                var layoutItemAprobado = FormLayoutSolicitudEspecial.FindItemByFieldName("layoutItemAprobado") as LayoutItem;
                // Verificar si viene un ID por querystring (modo edición)
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int id))
                {
                    IdSolicitud = id;

                    if (!CargarDatosSolicitud(id))
                    {
                        // Si no se encuentra la solicitud, redirigir
                        Response.Redirect("SolicitudesEspeciales.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                }
                else
                {
                    // Modo creación - limpiar formulario
                    layoutItemAprobado.Visible = true;
                    chkAprobado.Visible = false;
                    chkAprobado.Checked = false;
                    chkAprobado.Enabled = false;
                    layoutItemAprobado.Visible = false;
                    IdSolicitud = null;
                    LimpiarFormulario();
                }
            }

            // Configurar eventos de botones
            btnGuardar.Click += BtnGuardar_Click;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Verificar si hay mensaje para mostrar después de procesar todo
            if (hfMostrarMensaje.Value == "true")
            {
                MostrarPopup(hfTextoMensaje.Value, hfTipoMensaje.Value == "exito");
                hfMostrarMensaje.Value = "false";
            }
        }

        private void CargarCatalogos()
        {
            try
            {
                var repo = new SolicitudRHRepository();

                // Cargar tipos de solicitud
                cboTipoSolicitud.DataSource = repo.ObtenerTiposSolicitud();
                cboTipoSolicitud.TextField = "Visita";
                cboTipoSolicitud.ValueField = "ID_TipoVisita";
                cboTipoSolicitud.DataBind();

                // Cargar proyectos
                cboProyecto.DataSource = repo.ObtenerProyectos();
                cboProyecto.TextField = "NombreProyecto";
                cboProyecto.ValueField = "ID_Proyecto";
                cboProyecto.DataBind();

                // Cargar plantas
                cboPlanta.DataSource = repo.ObtenerPlantas();
                cboPlanta.TextField = "NombrePlanta";
                cboPlanta.ValueField = "ID_Planta";
                cboPlanta.DataBind();

                // Cargar Contratistas
                cboContratista.DataSource = repo.ObtenerContratistas();
                cboContratista.TextField = "Responsable";
                cboContratista.ValueField = "id_contratista";
                cboContratista.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error al cargar catálogos: {0}", ex);
                MostrarMensaje("Proceso no exitoso, no se ha podido cargar los catálogos: " + ex.Message, false);
            }
        }

        private bool CargarDatosSolicitud(int id)
        {
            try
            {
                var repo = new SolicitudRHRepository();
                var solicitud = repo.ObtenerSolicitudRHPorId(id);

                if (IdSolicitud.HasValue)
                {
                    chkAprobado.Visible = true;
                    chkAprobado.Checked = solicitud.aprobado;
                    chkAprobado.Enabled = true;
                }

                if (solicitud != null)
                {
                    cboTipoSolicitud.Value = solicitud.id_TipoSolicitud;
                    cboProyecto.Value = solicitud.id_Proyecto;
                    txtVisitante.Text = solicitud.Visitante;
                    cboPlanta.Value = solicitud.id_Planta;
                    cboContratista.Value = solicitud.id_Contratista;
                    txtAreaTrabajo.Text = solicitud.AreaTrabajo;
                    txtActividad.Text = solicitud.Actividad;
                    txtResponsable.Text = solicitud.Responsable;
                    txtEstancia.Text = solicitud.Estancia;
                    txtRFC.Text = solicitud.RFC;
                    dteFechaInicio.Value = solicitud.FechaInicio;
                    dteFechaFin.Value = solicitud.FechaFin;
                    chkAprobado.Checked = solicitud.aprobado;
                    return true;
                }
                else
                {
                    MostrarMensaje("Proceso no exitoso, no se ha podido encontrar la solicitud.", false);
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error al cargar solicitud: {0}", ex);
                MostrarMensaje("Proceso no exitoso, no se ha podido cargar los datos de la solicitud: " + ex.Message, false);
                return false;
            }
        }

        private void LimpiarFormulario()
        {
            cboTipoSolicitud.Value = null;
            cboProyecto.Value = null;
            txtVisitante.Text = string.Empty;
            cboPlanta.Value = null;
            cboContratista.Value = null;
            txtAreaTrabajo.Text = string.Empty;
            txtActividad.Text = string.Empty;
            txtResponsable.Text = string.Empty;
            txtEstancia.Text = string.Empty;
            txtRFC.Text = string.Empty;
            dteFechaInicio.Value = null;
            dteFechaFin.Value = null;
            chkAprobado.Checked = false;
        }

        /// <summary>
        /// Valida todos los campos del formulario antes de guardar
        /// </summary>
        /// <param name="errores">Lista de errores encontrados</param>
        /// <returns>True si todos los campos son válidos, false si hay errores</returns>
        private bool ValidarFormulario(out List<string> errores)
        {
            errores = new List<string>();

            // Validar Tipo de Solicitud
            if (cboTipoSolicitud.Value == null)
            {
                errores.Add("Tipo Solicitud");
            }

            // Validar Proyecto
            if (cboProyecto.Value == null)
            {
                errores.Add("Proyecto");
            }

            // Validar Visitante
            if (string.IsNullOrWhiteSpace(txtVisitante.Text))
            {
                errores.Add("Visitante");
            }

            // Validar Planta
            if (cboPlanta.Value == null)
            {
                errores.Add("Planta");
            }

            // Validar Contratista
            if (cboContratista.Value == null)
            {
                errores.Add("Contratista");
            }

            // Validar Área de Trabajo
            if (string.IsNullOrWhiteSpace(txtAreaTrabajo.Text))
            {
                errores.Add("Área de Trabajo");
            }

            // Validar Actividad
            if (string.IsNullOrWhiteSpace(txtActividad.Text))
            {
                errores.Add("Actividad");
            }

            // Validar Responsable
            if (string.IsNullOrWhiteSpace(txtResponsable.Text))
            {
                errores.Add("Responsable");
            }

            // Validar Estancia
            if (string.IsNullOrWhiteSpace(txtEstancia.Text))
            {
                errores.Add("Estancia");
            }

            // Validar RFC
            if (string.IsNullOrWhiteSpace(txtRFC.Text))
            {
                errores.Add("RFC");
            }

            // Validar Fecha Inicio
            if (dteFechaInicio.Value == null)
            {
                errores.Add("Fecha Inicio");
            }

            // Validar Fecha Fin
            if (dteFechaFin.Value == null)
            {
                errores.Add("Fecha Fin");
            }

            return errores.Count == 0;
        }

        /// <summary>
        /// Valida el RFC mexicano
        /// </summary>
        private bool ValidarRFC(string rfc, out string rfcNormalizado, out string mensajeError)
        {
            rfcNormalizado = string.Empty;
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(rfc))
            {
                mensajeError = "Error el RFC es requerido.";
                return false;
            }

            // Normalizar: convertir a mayúsculas y remover guiones
            rfcNormalizado = rfc.ToUpper().Replace("-", "");

            // Validar longitud (debe ser exactamente 13 caracteres sin guiones)
            if (rfcNormalizado.Length != 13)
            {
                mensajeError = $"Error el RFC debe tener 13 caracteres.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida que la fecha de inicio no sea mayor a la fecha de fin
        /// </summary>
        private bool ValidarFechas(out string mensajeError)
        {
            mensajeError = string.Empty;

            if (dteFechaInicio.Value != null && dteFechaFin.Value != null)
            {
                DateTime fechaInicio = Convert.ToDateTime(dteFechaInicio.Value);
                DateTime fechaFin = Convert.ToDateTime(dteFechaFin.Value);

                if (fechaInicio > fechaFin)
                {
                    mensajeError = "Error la fecha de inicio no puede ser mayor a la fecha de fin.";
                    return false;
                }
            }

            return true;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validar campos requeridos
                List<string> camposFaltantes;
                if (!ValidarFormulario(out camposFaltantes))
                {
                    if (camposFaltantes.Count == 1)
                    {
                        MostrarMensaje($"Error el campo '{camposFaltantes[0]}' es requerido.", false);
                    }
                    else
                    {
                        StringBuilder mensaje = new StringBuilder();
                        mensaje.AppendLine("Proceso no exitoso al guardar la solicitud, ingrese información en los siguientes campos:");
                        mensaje.AppendLine();
                        foreach (var campo in camposFaltantes)
                        {
                            mensaje.AppendLine($"• {campo}");
                        }
                        MostrarMensaje(mensaje.ToString(), false);
                    }
                    return;
                }

                // 2. Validar RFC
                string rfcNormalizado;
                string mensajeErrorRFC;
                if (!ValidarRFC(txtRFC.Text, out rfcNormalizado, out mensajeErrorRFC))
                {
                    MostrarMensaje(mensajeErrorRFC, false);
                    return;
                }

                // 3. Validar fechas
                string mensajeErrorFechas;
                if (!ValidarFechas(out mensajeErrorFechas))
                {
                    MostrarMensaje(mensajeErrorFechas, false);
                    return;
                }

                // 4. Si todas las validaciones pasaron, proceder a guardar
                var repo = new SolicitudRHRepository();

                var solicitud = new SolicitudRH
                {
                    id_TipoSolicitud = Convert.ToInt32(cboTipoSolicitud.Value),
                    id_Solicitante = 0,
                    id_Proyecto = Convert.ToInt32(cboProyecto.Value),
                    id_Planta = Convert.ToInt32(cboPlanta.Value),
                    Visitante = txtVisitante.Text.Trim(),
                    FechaInicio = Convert.ToDateTime(dteFechaInicio.Value),
                    FechaFin = Convert.ToDateTime(dteFechaFin.Value),
                    RFC = rfcNormalizado,
                    id_Contratista = Convert.ToInt32(cboContratista.Value),
                    Responsable = txtResponsable.Text.Trim(),
                    AreaTrabajo = txtAreaTrabajo.Text.Trim(),
                    Actividad = txtActividad.Text.Trim(),
                    Estancia = txtEstancia.Text.Trim(),
                    aprobado = chkAprobado.Checked
                };

                bool resultado = false;

                if (IdSolicitud.HasValue)
                {
                    // Modo edición
                    solicitud.ID_Solicitud = IdSolicitud.Value;
                    resultado = repo.ActualizarSolicitudRH(solicitud);
                    MostrarMensaje(
                        resultado
                            ? $"Proceso exitoso al actualizar solicitud con Folio: {solicitud.ID_Solicitud}"
                            : $"Proceso no exitoso al actualizar la solicitud con el folio: {solicitud.ID_Solicitud}.",
                        resultado
                    );
                }
                else
                {
                    // Modo creación
                    solicitud.FechaSolicitud = DateTime.Now;
                    resultado = repo.CrearSolicitudRH(solicitud);
                    MostrarMensaje(
                        resultado
                            ? $"Proceso exitoso en la creación de la solicitud con Folio: {solicitud.ID_Solicitud}."
                            : "Proceso no exitoso al crear la solicitud, verificar la información de los campos.",
                        resultado
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Proceso no exitoso al guardar la solicitud: {0}", ex);
                MostrarMensaje("Proceso no exitoso al guardar la solicitud: " + ex.Message, false);
            }
        }

        private void MostrarMensaje(string mensaje, bool esExito)
        {
            MostrarPopup(mensaje, esExito);
        }

        private void MostrarPopup(string mensaje, bool esExito)
        {
            // Escapar caracteres especiales para JavaScript y HTML
            string mensajeEscapado = mensaje
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", "<br/>");

            string script = string.Empty;

            if (esExito)
            {
                script = string.Format(@"
                    window.onload = function() {{
                        setTimeout(function() {{
                            if (typeof lblMensajeExitoSolicitud !== 'undefined' && lblMensajeExitoSolicitud) {{
                                lblMensajeExitoSolicitud.SetText('{0}');
                                pcMensajeExitoSolicitud.Show();
                            }} else {{
                                console.error('lblMensajeExitoSolicitud no está definido');
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
                            if (typeof lblMensajeErrorSolicitud !== 'undefined' && lblMensajeErrorSolicitud) {{
                                lblMensajeErrorSolicitud.SetText('{0}');
                                pcMensajeErrorSolicitud.Show();
                            }} else {{
                                console.error('lblMensajeErrorSolicitud no está definido');
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
    }
}