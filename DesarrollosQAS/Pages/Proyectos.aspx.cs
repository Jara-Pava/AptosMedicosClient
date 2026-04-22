using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace DesarrollosQAS.Pages
{
    public partial class Proyectos : System.Web.UI.Page
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
            gridProyectos.DataSource = repo.ObtenerProyectos();
            gridProyectos.DataBind();
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

        private Proyecto ObtenerValoresFormulario()
        {
            ASPxFormLayout formLayout = gridProyectos.FindEditFormTemplateControl("FormLayoutProyecto") as ASPxFormLayout;
            if (formLayout == null) return null;

            var txtProyecto = FindControlRecursive(formLayout, "txtProyecto") as ASPxTextBox;
            var chkActivo = FindControlRecursive(formLayout, "chkActivo") as ASPxCheckBox;

            return new Proyecto
            {
                NombreProyecto = txtProyecto?.Text?.Trim(),
                Activo = chkActivo != null ? chkActivo.Checked : true
            };
        }

        private bool ValidarFormulario(Proyecto modelo, out List<string> errores)
        {
            errores = new List<string>();
            if (string.IsNullOrWhiteSpace(modelo?.NombreProyecto))
                errores.Add("Nombre del Proyecto");
            return errores.Count == 0;
        }

        protected void gridProyectos_DataBinding(object sender, EventArgs e)
        {
            var repo = new SolicitudRHRepository();
            gridProyectos.DataSource = repo.ObtenerProyectos();
        }

        protected void gridProyectos_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                Proyecto proyectoFormulario = ObtenerValoresFormulario();

                if (!ValidarFormulario(proyectoFormulario, out List<string> errores))
                {
                    e.Cancel = true;
                    MostrarError("Por favor, complete los siguientes campos: " + string.Join(", ", errores));
                    return;
                }

                int idProyecto = Convert.ToInt32(e.Keys["ID_Proyecto"]);

                var repo = new SolicitudRHRepository();

                if (repo.ExisteProyectoConNombre(proyectoFormulario.NombreProyecto, idProyecto))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe el Proyecto con el nombre \"{proyectoFormulario.NombreProyecto}\".");
                    return;
                }

                Proyecto proyectoActualizado = new Proyecto
                {
                    ID_Proyecto = idProyecto,
                    NombreProyecto = proyectoFormulario.NombreProyecto,
                    Activo = proyectoFormulario.Activo
                };

                if (!repo.ActualizarProyecto(proyectoActualizado))
                    throw new ApplicationException("No se ha podido actualizar el proyecto.");

                e.Cancel = true;
                gridProyectos.DataBind();
                MostrarExitoConCierre($"Se ha actualizado el proyecto {proyectoActualizado.NombreProyecto}.");
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MostrarError("Ocurrió un error al actualizar el proyecto: " + ex.Message);
            }
        }

        protected void gridProyectos_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            Proyecto proyectoFormulario = ObtenerValoresFormulario();

            if (proyectoFormulario == null)
            {
                e.Cancel = true;
                MostrarError("No se pudieron obtener los datos del formulario.");
                return;
            }

            try
            {
                if (!ValidarFormulario(proyectoFormulario, out List<string> errores))
                {
                    e.Cancel = true;
                    MostrarError("Error, complete los siguientes campos: " + string.Join(", ", errores));
                    return;
                }

                var repo = new SolicitudRHRepository();

                if (repo.ExisteProyectoConNombre(proyectoFormulario.NombreProyecto))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe el Proyecto con el nombre \"{proyectoFormulario.NombreProyecto}\".");
                    return;
                }

                Proyecto nuevoProyecto = new Proyecto
                {
                    NombreProyecto = proyectoFormulario.NombreProyecto
                };

                if (!repo.CrearProyecto(nuevoProyecto))
                    throw new ApplicationException("No se ha podido crear el proyecto.");

                e.Cancel = true;
                gridProyectos.DataBind();
                MostrarExitoConCierre($"Se ha creado el proyecto {nuevoProyecto.NombreProyecto}.");
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MostrarError($"Proceso no exitoso al crear el proyecto {proyectoFormulario.NombreProyecto}: {ex.Message}");
            }
        }

        protected void gridProyectos_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters.StartsWith("DELETE|"))
            {
                string[] parts = e.Parameters.Split('|');
                int visibleIndex = Convert.ToInt32(parts[1]);
                int id = Convert.ToInt32(gridProyectos.GetRowValues(visibleIndex, "ID_Proyecto"));
                string nombre = gridProyectos.GetRowValues(visibleIndex, "NombreProyecto")?.ToString();

                try
                {
                    var repo = new SolicitudRHRepository();
                    repo.EliminarProyecto(id);
                    gridProyectos.DataBind();
                    MostrarExito($"Se ha eliminado el Proyecto {nombre}.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error al eliminar Proyecto: {0}", ex);
                    MostrarError($"Proceso no exitoso al eliminar el Proyecto {nombre}: {ex.Message}");
                }
            }
        }

        protected void gridProyectos_HtmlEditFormCreated(object sender, DevExpress.Web.ASPxGridViewEditFormEventArgs e)
        {
            //ASPxFormLayout formLayout = gridProyectos.FindEditFormTemplateControl("FormLayoutProyecto") as ASPxFormLayout;
            //if (formLayout != null)
            //{
            //    LayoutItem layoutItem = formLayout.FindItemByFieldName("layoutItemActivo");
            //    if (layoutItem != null)
            //        layoutItem.Visible = !gridProyectos.IsNewRowEditing;
            //}
        }

        #region Métodos de Mensajes

        private void MostrarExito(string mensaje)
        {
            gridProyectos.JSProperties["cpMessageType"] = "success";
            gridProyectos.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarExitoConCierre(string mensaje)
        {
            gridProyectos.JSProperties["cpMessageType"] = "success";
            gridProyectos.JSProperties["cpMessage"] = mensaje;
            gridProyectos.JSProperties["cpShouldCloseEdit"] = true;
        }

        private void MostrarError(string mensaje)
        {
            gridProyectos.JSProperties["cpMessageType"] = "error";
            gridProyectos.JSProperties["cpMessage"] = mensaje;
        }

        #endregion
    }
}