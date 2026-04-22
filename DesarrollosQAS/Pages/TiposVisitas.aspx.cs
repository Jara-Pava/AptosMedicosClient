using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace DesarrollosQAS.Pages
{
    public partial class TiposVisita : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            var repo = new SolicitudRHRepository();
            gridTiposVisita.DataSource = repo.ObtenerTiposSolicitud().OrderBy(p => p.ID_TipoVisita).ToList();
            gridTiposVisita.DataBind();
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id) return root;
            foreach (Control child in root.Controls)
            {
                Control found = FindControlRecursive(child, id);
                if (found != null) return found;
            }
            return null;
        }

        private TipoVisitante ObtenerValoresFormulario()
        {
            ASPxFormLayout formLayout = gridTiposVisita.FindEditFormTemplateControl("FormLayoutTipoVisita") as ASPxFormLayout;
            if (formLayout == null) return null;

            var txtVisita = FindControlRecursive(formLayout, "txtVisita") as ASPxTextBox;
            var txtEstancia = FindControlRecursive(formLayout, "txtEstancia") as ASPxTextBox;
            var chkActivo = FindControlRecursive(formLayout, "chkActivo") as ASPxCheckBox;

            return new TipoVisitante
            {
                Visita = txtVisita?.Text?.Trim(),
                Estancia = txtEstancia?.Text?.Trim(),
                Activo = chkActivo != null ? chkActivo.Checked : true
            };
        }

        private bool ValidarFormulario(TipoVisitante modelo, out List<string> errores)
        {
            errores = new List<string>();
            if (string.IsNullOrWhiteSpace(modelo?.Visita))
                errores.Add("Nombre del Tipo de Visita");

            // Validar que el campo Estancia no esté vacío
            if (string.IsNullOrWhiteSpace(modelo?.Estancia))
                errores.Add("Estancia");

            return errores.Count == 0;
        }

        protected void gridTiposVisita_DataBinding(object sender, EventArgs e)
        {
            var repo = new SolicitudRHRepository();
            gridTiposVisita.DataSource = repo.ObtenerTiposSolicitud();
        }

        protected void gridTiposVisita_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                TipoVisitante tipoFormulario = ObtenerValoresFormulario();

                if (!ValidarFormulario(tipoFormulario, out List<string> errores))
                {
                    e.Cancel = true;
                    MostrarError("Por favor, complete los siguientes campos: " + string.Join(", ", errores));
                    return;
                }

                int idTipoVisita = Convert.ToInt32(e.Keys["ID_TipoVisita"]);

                var repo = new SolicitudRHRepository();

                if (repo.ExisteTipoVisitaConNombre(tipoFormulario.Visita, idTipoVisita))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe el Tipo de Visita con el nombre \"{tipoFormulario.Visita}\".");
                    return;
                }

                TipoVisitante tipoActualizado = new TipoVisitante
                {
                    ID_TipoVisita = idTipoVisita,
                    Visita = tipoFormulario.Visita,
                    Estancia = tipoFormulario.Estancia,
                    Activo = tipoFormulario.Activo
                };

                if (!repo.ActualizarTipoVisita(tipoActualizado))
                    throw new ApplicationException("No se ha podido actualizar el tipo de visita.");

                e.Cancel = true;
                gridTiposVisita.DataBind();
                MostrarExitoConCierre($"Se ha actualizado el tipo de visita {tipoActualizado.Visita}.");
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MostrarError("Ocurrió un error al actualizar el tipo de visita: " + ex.Message);
            }
        }

        protected void gridTiposVisita_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            TipoVisitante tipoFormulario = ObtenerValoresFormulario();

            if (tipoFormulario == null)
            {
                e.Cancel = true;
                MostrarError("No se pudieron obtener los datos del formulario.");
                return;
            }

            try
            {
                if (!ValidarFormulario(tipoFormulario, out List<string> errores))
                {
                    e.Cancel = true;
                    MostrarError("Error, complete los siguientes campos: " + string.Join(", ", errores));
                    return;
                }

                var repo = new SolicitudRHRepository();

                if (repo.ExisteTipoVisitaConNombre(tipoFormulario.Visita))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe el Tipo de Visita con el nombre \"{tipoFormulario.Visita}\".");
                    return;
                }

                TipoVisitante nuevoTipo = new TipoVisitante
                {
                    Visita = tipoFormulario.Visita,
                    Estancia = tipoFormulario.Estancia
                };

                if (!repo.CrearTipoVisita(nuevoTipo))
                    throw new ApplicationException("No se ha podido crear el tipo de visita.");

                e.Cancel = true;
                gridTiposVisita.DataBind();
                MostrarExitoConCierre($"Se ha creado el tipo de visita {nuevoTipo.Visita}.");
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MostrarError($"Proceso no exitoso al crear el tipo de visita {tipoFormulario.Visita}: {ex.Message}");
            }
        }

        protected void gridTiposVisita_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters.StartsWith("DELETE|"))
            {
                string[] parts = e.Parameters.Split('|');
                int visibleIndex = Convert.ToInt32(parts[1]);
                int id = Convert.ToInt32(gridTiposVisita.GetRowValues(visibleIndex, "ID_TipoVisita"));
                string nombre = gridTiposVisita.GetRowValues(visibleIndex, "Visita")?.ToString();

                try
                {
                    var repo = new SolicitudRHRepository();
                    repo.EliminarTipoVisita(id);
                    gridTiposVisita.DataBind();
                    MostrarExito($"Se ha eliminado el Tipo de Visita {nombre}.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error al eliminar TipoVisita: {0}", ex);
                    MostrarError($"No se ha podido eliminar el Tipo de Visita {nombre}: {ex.Message}");
                }
            }
        }

        protected void gridTiposVisita_HtmlEditFormCreated(object sender, DevExpress.Web.ASPxGridViewEditFormEventArgs e)
        {
            //ASPxFormLayout formLayout = gridTiposVisita.FindEditFormTemplateControl("FormLayoutTipoVisita") as ASPxFormLayout;
            //if (formLayout != null)
            //{
            //    LayoutItem layoutItem = formLayout.FindItemByFieldName("layoutItemActivo");
            //    if (layoutItem != null)
            //        layoutItem.Visible = !gridTiposVisita.IsNewRowEditing;
            //}
        }

        #region Métodos de Mensajes

        private void MostrarExito(string mensaje)
        {
            gridTiposVisita.JSProperties["cpMessageType"] = "success";
            gridTiposVisita.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarExitoConCierre(string mensaje)
        {
            gridTiposVisita.JSProperties["cpMessageType"] = "success";
            gridTiposVisita.JSProperties["cpMessage"] = mensaje;
            gridTiposVisita.JSProperties["cpShouldCloseEdit"] = true;
        }

        private void MostrarError(string mensaje)
        {
            gridTiposVisita.JSProperties["cpMessageType"] = "error";
            gridTiposVisita.JSProperties["cpMessage"] = mensaje;
        }

        #endregion
    }
}