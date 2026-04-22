using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using DevExpress.Web.Internal.Dialogs;
using DevExpress.Web.Internal.XmlProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DesarrollosQAS.Pages
{
    public partial class EmpresasContratistas : System.Web.UI.Page
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
            List<EmpresaContratista> empresasContratistas = repo.ObtenerContratistas().OrderBy(p => p.id_contratista).ToList();
            gridContratistas.DataSource = empresasContratistas;
            gridContratistas.DataBind();
        }

        // gridContratistas DataBinding para actualizar datos
        protected void gridContratistas_DataBinding(object sender, EventArgs e)
        {
            var repo = new SolicitudRHRepository();
            gridContratistas.DataSource = repo.ObtenerContratistas().OrderBy(p => p.id_contratista).ToList();
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

        protected void gridContratistas_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                EmpresaContratista contratistaFormulario = ObtenerValoresFormulario();

                // Validar formulario
                if (!ValidarFormulario(contratistaFormulario, out List<string> errores))
                {
                    e.Cancel = true;
                    string mensajeError = "Por favor, complete los siguientes campos: " + string.Join(", ", errores);
                    MostrarError(mensajeError);
                    return;
                }
                int idContratista = Convert.ToInt32(e.Keys["id_contratista"]);


                // Crear objeto con datos actualizados
                EmpresaContratista contratistaActualizado = new EmpresaContratista
                {
                    id_contratista = idContratista,
                    Nombre = contratistaFormulario.Nombre,
                    RFC = contratistaFormulario.RFC,
                    Responsable = contratistaFormulario.Responsable,
                    Telefono = contratistaFormulario.Telefono,
                    Email = contratistaFormulario.Email,
                    Activo = contratistaFormulario.Activo
                };

                var repo = new SolicitudRHRepository();

                if (repo.ExisteContratistaConNombre(contratistaFormulario.Nombre, idContratista))
                {
                    e.Cancel = true;
                    MostrarError($"Proceso no exitoso, ya existe el contratista con el nombre\"{contratistaFormulario.Nombre}\". ");
                    return;
                }

                if (!repo.ActualizarContratista(contratistaActualizado))
                {
                    throw new ApplicationException("No se ha podido crear el rol.");
                }

                e.Cancel = true;
                gridContratistas.DataBind();
                MostrarExitoConCierre($"Proceso Exitoso se ha actualizado al contratista {contratistaActualizado.id_contratista}");
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MostrarError("Ocurrió un error al actualizar el contratista: " + ex.Message);
            }
        }

        // Método para obtener los campos del formulario (helper)
        private EmpresaContratista ObtenerValoresFormulario()
        {
            // Obtener controles del formulario de edición
            ASPxFormLayout formLayout = gridContratistas.FindEditFormTemplateControl("FormLayoutContratistas") as ASPxFormLayout;

            if (formLayout == null) return null;

            // Obtener el ID del contratista si es una edición

            var txtNombre = FindControlRecursive(formLayout, "txtNombre") as ASPxTextBox;
            var txtRFC = FindControlRecursive(formLayout, "txtRFC") as ASPxTextBox;
            var txtResponsable = FindControlRecursive(formLayout, "txtResponsable") as ASPxTextBox;
            var txtTelefono = FindControlRecursive(formLayout, "txtTelefono") as ASPxTextBox;
            var txtEmail = FindControlRecursive(formLayout, "txtEmail") as ASPxTextBox;
            var chkActivo = FindControlRecursive(formLayout, "chkActivo") as ASPxCheckBox;

            var modelo = new EmpresaContratista
            {
                Nombre = txtNombre?.Text?.Trim(),
                RFC = txtRFC?.Text?.Trim(),
                Responsable = txtResponsable?.Text?.Trim(),
                Telefono = txtTelefono?.Text?.Trim(),
                Email = txtEmail?.Text?.Trim(),
                Activo = chkActivo != null ? chkActivo.Checked : true
            };

            return modelo;
        }

        private bool ValidarFormulario(EmpresaContratista modeloFormularioContratista, out List<string> errores)
        {
            errores = new List<string>();

            string txtNombre = modeloFormularioContratista?.Nombre;
            string txtRFC = modeloFormularioContratista?.RFC;
            string txtResponsable = modeloFormularioContratista?.Responsable;
            string txtTelefono = modeloFormularioContratista?.Telefono;
            string txtEmail = modeloFormularioContratista?.Email;
            bool chkActivo = modeloFormularioContratista != null ? modeloFormularioContratista.Activo : true;

            // validar el nombre de la empresa contratista
            if (txtNombre == null || string.IsNullOrWhiteSpace(txtNombre))
            {
                errores.Add("Nombre de la Empresa Contratista");
            }

            // Validar Responsable
            if (txtResponsable == null || string.IsNullOrWhiteSpace(txtResponsable))
            {
                errores.Add("Responsable");
            }

            // Validar RFC
            if (txtRFC == null || string.IsNullOrWhiteSpace(txtRFC))
            {
                errores.Add("RFC");
            }

            if (txtTelefono == null || string.IsNullOrWhiteSpace(txtTelefono))
            {
                errores.Add("Teléfono");
            }

            if (txtEmail == null || string.IsNullOrWhiteSpace(txtEmail))
            {
                errores.Add("Email");
            }

            return errores.Count == 0;
        }

        protected void gridContratistas_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            EmpresaContratista contratistaFormulario = ObtenerValoresFormulario();

            if (contratistaFormulario == null)
            {
                e.Cancel = true;
                MostrarError("No se pudieron obtener los datos del formulario.");
                return;
            }

            try
            {
                // Validar formulario
                if (!ValidarFormulario(contratistaFormulario, out List<string> errores))
                {
                    e.Cancel = true;
                    string mensajeError = "Error, complete los siguientes campos: " + string.Join(", ", errores);
                    MostrarError(mensajeError);
                    return;
                }

                var repo = new SolicitudRHRepository();
                if (repo.ExisteContratistaConNombre(contratistaFormulario.Nombre))
                {
                    e.Cancel = true;
                    MostrarError($"Proceso no exitoso, ya existe el contratista con el nombre\"{contratistaFormulario.Nombre}\". ");
                    return;
                }

                // Crear objeto con datos del nuevo contratista
                EmpresaContratista nuevoContratista = new EmpresaContratista
                {
                    Nombre = contratistaFormulario.Nombre,
                    RFC = contratistaFormulario.RFC,
                    Responsable = contratistaFormulario.Responsable,
                    Telefono = contratistaFormulario.Telefono,
                    Email = contratistaFormulario.Email,
                };

                if (!repo.CrearContratista(nuevoContratista))
                {
                    throw new ApplicationException("No se ha podido crear el contratista.");
                }

                e.Cancel = true;
                gridContratistas.DataBind();
                MostrarExitoConCierre($"Proceso exitoso se ha creado al contratista {nuevoContratista.Nombre}.");

            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MostrarError($"Proceso no exitoso al crear al contratista {contratistaFormulario.Nombre}: {ex.Message}");
            }
        }

        protected void gridContratistas_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters.StartsWith("DELETE|"))
            {
                string[] parts = e.Parameters.Split('|');
                int visibleIndex = Convert.ToInt32(parts[1]);
                int id = Convert.ToInt32(gridContratistas.GetRowValues(visibleIndex, "id_contratista"));
                string nombre = gridContratistas.GetRowValues(visibleIndex, "Nombre")?.ToString();

                try
                {
                    var repo = new SolicitudRHRepository();
                    repo.EliminarContratista(id);

                    gridContratistas.DataBind();
                    MostrarExito($"Proceso exitoso al eliminar al contratista {nombre}.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error al eliminar rol: {0}", ex);
                    MostrarError($"Proceso no exitoso al eliminar al contratista {nombre}: {ex.Message}");
                }
            }
        }

        protected void gridContratistas_HtmlEditFormCreated(object sender, DevExpress.Web.ASPxGridViewEditFormEventArgs e)
        {
            //ASPxFormLayout formLayout = gridContratistas.FindEditFormTemplateControl("FormLayoutContratistas") as ASPxFormLayout;
            //if (formLayout != null)
            //{
            //    LayoutItem layoutItem = formLayout.FindItemByFieldName("layoutItemActivo");
            //    if (gridContratistas.IsNewRowEditing)
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
            gridContratistas.JSProperties["cpMessageType"] = "success";
            gridContratistas.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarExitoConCierre(string mensaje)
        {
            gridContratistas.JSProperties["cpMessageType"] = "success";
            gridContratistas.JSProperties["cpMessage"] = mensaje;
            gridContratistas.JSProperties["cpShouldCloseEdit"] = true;
        }

        private void MostrarError(string mensaje)
        {
            gridContratistas.JSProperties["cpMessageType"] = "error";
            gridContratistas.JSProperties["cpMessage"] = mensaje;
        }

        #endregion
    }
}