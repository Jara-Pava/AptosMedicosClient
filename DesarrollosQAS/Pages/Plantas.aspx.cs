using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DesarrollosQAS.Pages
{
    public partial class Plantas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        // BIND GRIDVIEW
        private void BindGrid()
        {
            var repo = new SolicitudRHRepository();
            List<Planta> plantas = repo.ObtenerPlantas().OrderBy(p => p.ID_Planta).ToList();
            gridPlantas.DataSource = plantas;
            gridPlantas.DataBind();
        }
        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control child in root.Controls)
            {
                Control found = FindControlRecursive(child, id);
                if (found != null)
                    return found;
            }

            return null;
        }

        // Método para obtener los campos del formulario (helper)
        private Planta ObtenerValoresFormulario()
        {
            // Obtener controles del formulario de edición
            ASPxFormLayout formLayout = gridPlantas.FindEditFormTemplateControl("FormLayoutPlanta") as ASPxFormLayout;

            if (formLayout == null) return null;

            // Obtener el ID del contratista si es una edición

            var txtPlanta = FindControlRecursive(formLayout, "txtPlanta") as ASPxTextBox;
            var chkActivo = FindControlRecursive(formLayout, "chkActivo") as ASPxCheckBox;

            var modelo = new Planta
            {
                NombrePlanta = txtPlanta?.Text?.Trim(),
                Activo = chkActivo != null ? chkActivo.Checked : true
            };

            return modelo;
        }

        private bool ValidarFormulario(Planta modeloPlantaFormulario, out List<string> errores)
        {
            errores = new List<string>();

            string txtNombre = modeloPlantaFormulario?.NombrePlanta;
            bool chkActivo = modeloPlantaFormulario != null ? modeloPlantaFormulario.Activo : true;

            // validar el nombre de la empresa contratista
            if (txtNombre == null || string.IsNullOrWhiteSpace(txtNombre))
            {
                errores.Add("Nombre de la Planta");
            }
            return errores.Count == 0;
        }

        protected void gridPlantas_DataBinding(object sender, EventArgs e)
        {
            var repo = new SolicitudRHRepository();
            gridPlantas.DataSource = repo.ObtenerPlantas().OrderBy(p => p.ID_Planta).ToList();
        }

        protected void gridPlantas_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                Planta plantaFormulario = ObtenerValoresFormulario();

                // Validar formulario
                if (!ValidarFormulario(plantaFormulario, out List<string> errores))
                {
                    e.Cancel = true;
                    string mensajeError = "Por favor, complete los siguientes campos: " + string.Join(", ", errores);
                    MostrarError(mensajeError);
                    return;
                }
                int idPlanta = Convert.ToInt32(e.Keys["ID_Planta"]);


                // Crear objeto con datos actualizados
                Planta plantaActualizado = new Planta
                {
                    ID_Planta = idPlanta,
                    NombrePlanta = plantaFormulario.NombrePlanta,
                    Activo = plantaFormulario.Activo
                };

                var repo = new SolicitudRHRepository();

                if (repo.ExisteContratistaConNombre(plantaFormulario.NombrePlanta, idPlanta))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe la Planta con el nombre\"{plantaFormulario.NombrePlanta}\". ");
                    return;
                }

                if (!repo.ActualizarPlanta(plantaActualizado))
                {
                    throw new ApplicationException("No se ha podido crear la planta.");
                }

                e.Cancel = true;
                gridPlantas.DataBind();
                MostrarExitoConCierre($"Se ha actualizado la planta {plantaActualizado.NombrePlanta}");
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MostrarError("Ocurrió un error al actualizar la planta: " + ex.Message);
            }
        }

        protected void gridPlantas_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            Planta plantaFormulario = ObtenerValoresFormulario();

            if (plantaFormulario == null)
            {
                e.Cancel = true;
                MostrarError("No se pudieron obtener los datos del formulario.");
                return;
            }

            try
            {
                // Validar formulario
                if (!ValidarFormulario(plantaFormulario, out List<string> errores))
                {
                    e.Cancel = true;
                    string mensajeError = "Error, complete los siguientes campos: " + string.Join(", ", errores);
                    MostrarError(mensajeError);
                    return;
                }

                var repo = new SolicitudRHRepository();
                if (repo.ExistePlantaConNombre(plantaFormulario.NombrePlanta))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe la Planta con el nombre\"{plantaFormulario.NombrePlanta}\". ");
                    return;
                }

                // Crear objeto con datos del nuevo contratista
                Planta nuevaPlanta = new Planta
                {
                    NombrePlanta = plantaFormulario.NombrePlanta
                };

                if (!repo.CrearPlanta(nuevaPlanta))
                {
                    throw new ApplicationException("No se ha podido crear la Planta.");
                }

                e.Cancel = true;
                gridPlantas.DataBind();
                MostrarExitoConCierre($"Se ha creado la Planta {nuevaPlanta.NombrePlanta}.");

            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MostrarError($"Proceso no exitoso al crear la Planta {plantaFormulario.NombrePlanta}: {ex.Message}");
            }
        }

        protected void gridPlantas_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters.StartsWith("DELETE|"))
            {
                string[] parts = e.Parameters.Split('|');
                int visibleIndex = Convert.ToInt32(parts[1]);
                int id = Convert.ToInt32(gridPlantas.GetRowValues(visibleIndex, "ID_Planta"));
                string nombre = gridPlantas.GetRowValues(visibleIndex, "NombrePlanta")?.ToString();

                try
                {
                    var repo = new SolicitudRHRepository();
                    repo.EliminarPlanta(id);

                    gridPlantas.DataBind();
                    MostrarExito($"Se ha eliminado la Planta {nombre}.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error al eliminar Planta: {0}", ex);
                    MostrarError($"Proceso no exitoso al eliminar la Planta {nombre}: {ex.Message}");
                }
            }
        }

        protected void gridPlantas_HtmlEditFormCreated(object sender, DevExpress.Web.ASPxGridViewEditFormEventArgs e)
        {
            //ASPxFormLayout formLayout = gridPlantas.FindEditFormTemplateControl("FormLayoutPlanta") as ASPxFormLayout;
            //if (formLayout != null)
            //{
            //    LayoutItem layoutItem = formLayout.FindItemByFieldName("layoutItemActivo");
            //    if (gridPlantas.IsNewRowEditing)
            //    {
            //        layoutItem.Visible = false;
            //    }
            //    else
            //    {
            //        layoutItem.Visible = true;
            //    }
            //}
        }
        #region Métodos de Mensajes

        private void MostrarExito(string mensaje)
        {
            gridPlantas.JSProperties["cpMessageType"] = "success";
            gridPlantas.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarExitoConCierre(string mensaje)
        {
            gridPlantas.JSProperties["cpMessageType"] = "success";
            gridPlantas.JSProperties["cpMessage"] = mensaje;
            gridPlantas.JSProperties["cpShouldCloseEdit"] = true;
        }

        private void MostrarError(string mensaje)
        {
            gridPlantas.JSProperties["cpMessageType"] = "error";
            gridPlantas.JSProperties["cpMessage"] = mensaje;
        }

        #endregion
    }
}