using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace DesarrollosQAS.Pages
{
    public partial class Modulo : System.Web.UI.Page
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
            var repo = new ModuloCatalogoRepository();
            List<ModuloCatalogo> lista = repo.ObtenerTodos().OrderBy(p => p.id_modulo_catalogo).ToList();
            gridModulo.DataSource = lista;
            gridModulo.DataBind();
        }

        protected void gridModulo_DataBinding(object sender, EventArgs e)
        {
            var repo = new ModuloCatalogoRepository();
            gridModulo.DataSource = repo.ObtenerTodos().OrderBy(p => p.id_modulo_catalogo).ToList();
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

        protected void gridModulo_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                ASPxFormLayout formLayout = gridModulo.FindEditFormTemplateControl("FormLayoutModulo") as ASPxFormLayout;
                if (formLayout == null)
                {
                    e.Cancel = true;
                    MostrarError("No se pudo obtener el formulario de edición.");
                    return;
                }

                ASPxTextBox txtNombre = FindControlRecursive(formLayout, "txtNombre") as ASPxTextBox;
                ASPxMemo txtDescripcion = FindControlRecursive(formLayout, "txtDescripcion") as ASPxMemo;
                ASPxCheckBox chkActivo = FindControlRecursive(formLayout, "chkActivo") as ASPxCheckBox;

                if (txtNombre == null || txtDescripcion == null)
                {
                    e.Cancel = true;
                    MostrarError("No se pudieron obtener los controles del formulario.");
                    return;
                }

                string nombre = txtNombre.Text?.Trim();
                string descripcion = txtDescripcion.Text?.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    e.Cancel = true;
                    MostrarError("El nombre es requerido.");
                    return;
                }

                int id = Convert.ToInt32(e.Keys["id_modulo_catalogo"]);

                var repo = new ModuloCatalogoRepository();
                if (repo.ExisteConNombre(nombre, id))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe otro módulo/catálogo con el nombre \"{nombre}\".");
                    return;
                }

                bool activo = chkActivo != null ? chkActivo.Checked : false;

                var item = new ModuloCatalogo
                {
                    id_modulo_catalogo = id,
                    nombre = nombre,
                    descripcion = descripcion,
                    activo = activo
                };

                if (!repo.Actualizar(item))
                {
                    throw new ApplicationException($"No se ha podido actualizar \"{nombre}\".");
                }

                e.Cancel = true;
                gridModulo.DataBind();
                MostrarExitoConCierre($"Proceso exitoso al actualizar \"{nombre}\".");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error en RowUpdating ModuloCatalogo: {0}", ex);
                e.Cancel = true;
                MostrarError($"Proceso no exitoso al actualizar: {ex.Message}");
            }
        }

        protected void gridModulo_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                ASPxFormLayout formLayout = gridModulo.FindEditFormTemplateControl("FormLayoutModulo") as ASPxFormLayout;
                if (formLayout == null)
                {
                    e.Cancel = true;
                    MostrarError("No se pudo obtener el formulario de edición.");
                    return;
                }

                ASPxTextBox txtNombre = FindControlRecursive(formLayout, "txtNombre") as ASPxTextBox;
                ASPxMemo txtDescripcion = FindControlRecursive(formLayout, "txtDescripcion") as ASPxMemo;

                string nombre = txtNombre?.Text?.Trim();
                string descripcion = txtDescripcion?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    e.Cancel = true;
                    MostrarError("El nombre es requerido.");
                    return;
                }

                var repo = new ModuloCatalogoRepository();
                if (repo.ExisteConNombre(nombre))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe un módulo/catálogo con el nombre \"{nombre}\".");
                    return;
                }

                int idUsuarioActual = Model.AuthHelper.GetCurrentUserId();

                var item = new ModuloCatalogo
                {
                    nombre = nombre,
                    descripcion = descripcion,
                    creado_por = idUsuarioActual > 0 ? (int?)idUsuarioActual : null
                };

                if (!repo.Crear(item))
                {
                    throw new ApplicationException("No se ha podido crear el módulo/catálogo.");
                }

                e.Cancel = true;
                gridModulo.DataBind();
                MostrarExitoConCierre($"Proceso exitoso al crear \"{nombre}\".");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error en RowInserting ModuloCatalogo: {0}", ex);
                e.Cancel = true;
                MostrarError($"Proceso no exitoso al crear: {ex.Message}");
            }
        }

        protected void gridModulo_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters.StartsWith("DELETE|"))
            {
                string[] parts = e.Parameters.Split('|');
                int visibleIndex = Convert.ToInt32(parts[1]);
                int id = Convert.ToInt32(gridModulo.GetRowValues(visibleIndex, "id_modulo_catalogo"));
                string nombre = gridModulo.GetRowValues(visibleIndex, "nombre")?.ToString();

                try
                {
                    var repo = new ModuloCatalogoRepository();
                    repo.Eliminar(id);

                    gridModulo.DataBind();
                    MostrarExito($"Proceso exitoso al eliminar \"{nombre}\".");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error al eliminar ModuloCatalogo: {0}", ex);
                    MostrarError($"Proceso no exitoso al eliminar \"{nombre}\": {ex.Message}");
                }
            }
        }

        protected void gridModulo_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            //ASPxFormLayout formLayout = gridModulo.FindEditFormTemplateControl("FormLayoutModulo") as ASPxFormLayout;
            //if (formLayout != null)
            //{
            //    LayoutItem layoutItem = formLayout.FindItemByFieldName("layoutItemActivo");
            //    if (gridModulo.IsNewRowEditing)
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
            gridModulo.JSProperties["cpMessageType"] = "success";
            gridModulo.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarExitoConCierre(string mensaje)
        {
            gridModulo.JSProperties["cpMessageType"] = "success";
            gridModulo.JSProperties["cpMessage"] = mensaje;
            gridModulo.JSProperties["cpShouldCloseEdit"] = true;
        }

        private void MostrarError(string mensaje)
        {
            gridModulo.JSProperties["cpMessageType"] = "error";
            gridModulo.JSProperties["cpMessage"] = mensaje;
        }

        #endregion
    }
}