using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DesarrollosQAS.Model;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace DesarrollosQAS.Pages
{
    public partial class Roles : System.Web.UI.Page
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
            var repo = new RolesRepository();
            List<Rol> roles = repo.ObtenerRoles();
            gridRoles.DataSource = roles;
            gridRoles.DataBind();
        }

        protected void gridRoles_DataBinding(object sender, EventArgs e)
        {
            var repo = new RolesRepository();
            gridRoles.DataSource = repo.ObtenerRoles().OrderBy(p => p.id_rol).ToList();
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

        protected void gridRoles_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                ASPxFormLayout formLayout = gridRoles.FindEditFormTemplateControl("FormLayoutRoles") as ASPxFormLayout;
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
                    MostrarError("Error el nombre del Rol es requerido.");
                    return;
                }

                int idRol = Convert.ToInt32(e.Keys["id_rol"]);

                // Validar nombre duplicado en code-behind
                var repo = new RolesRepository();
                if (repo.ExisteRolConNombre(nombre, idRol))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe el rol con el nombre \"{nombre}\".");
                    return;
                }

                bool activo = chkActivo != null ? chkActivo.Checked : false;

                var rol = new Rol
                {
                    id_rol = idRol,
                    nombre = nombre,
                    descripcion = descripcion,
                    activo = activo
                };

                if (!repo.ActualizarRol(rol))
                {
                    throw new ApplicationException($"No se ha podido actualizar el rol {nombre}.");
                }

                e.Cancel = true;
                gridRoles.DataBind();
                MostrarExitoConCierre($"Proceso exitoso al actualizar el rol {nombre}.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error en RowUpdating Rol: {0}", ex);
                e.Cancel = true;
                MostrarError($"Proceso no exitoso al actualizar el rol: {ex.Message}");
            }
        }

        protected void gridRoles_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                ASPxFormLayout formLayout = gridRoles.FindEditFormTemplateControl("FormLayoutRoles") as ASPxFormLayout;
                if (formLayout == null)
                {
                    e.Cancel = true;
                    MostrarError("No se pudo obtener el formulario de edición.");
                    return;
                }

                ASPxTextBox txtNombre = FindControlRecursive(formLayout, "txtNombre") as ASPxTextBox;
                ASPxMemo txtDescripcion = FindControlRecursive(formLayout, "txtDescripcion") as ASPxMemo;
                ASPxCheckBox chkActivo = FindControlRecursive(formLayout, "chkActivo") as ASPxCheckBox;

                string nombreUsuario = Environment.UserName;

                // Obtener el ID del usuario logueado
                int idUsuarioActual = AuthHelper.GetCurrentUserId();

                string nombre = txtNombre?.Text?.Trim();
                string descripcion = txtDescripcion?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    e.Cancel = true;
                    MostrarError("El nombre del rol es requerido.");
                    return;
                }

                // Validar nombre duplicado en code-behind
                var repo = new RolesRepository();
                if (repo.ExisteRolConNombre(nombre))
                {
                    e.Cancel = true;
                    MostrarError($"Ya existe un rol con el nombre \"{nombre}\".");
                    return;
                }

                bool activo = chkActivo != null ? chkActivo.Checked : true;

                var rol = new Rol
                {
                    nombre = nombre,
                    descripcion = descripcion,
                    creado_por = idUsuarioActual > 0 ? (int?)idUsuarioActual : null,
                };

                if (!repo.CrearRol(rol))
                {
                    throw new ApplicationException("No se ha podido crear el rol.");
                }

                e.Cancel = true;
                gridRoles.DataBind();
                MostrarExitoConCierre($"Proceso exitoso al crear el Rol {nombre}.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error en RowInserting Rol: {0}", ex);
                e.Cancel = true;
                MostrarError($"Proceso no exitoso al crear el Rol: {ex.Message}");
            }
        }

        protected void gridRoles_RowDeleting(object sender, EventArgs e)
        {
        }

        protected void gridRoles_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            if (e.Parameters.StartsWith("DELETE|"))
            {
                string[] parts = e.Parameters.Split('|');
                int visibleIndex = Convert.ToInt32(parts[1]);
                int id = Convert.ToInt32(gridRoles.GetRowValues(visibleIndex, "id_rol"));
                string nombre = gridRoles.GetRowValues(visibleIndex, "nombre")?.ToString();

                try
                {
                    var repo = new RolesRepository();
                    repo.EliminarRol(id);

                    gridRoles.DataBind();
                    MostrarExito($"Proceso exitoso al eliminar el Rol {nombre}.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Error al eliminar rol: {0}", ex);
                    MostrarError($"Proceso no exitoso al eliminar el Rol {nombre}: {ex.Message}");
                }
            }
        }

        #region Métodos de Mensajes

        private void MostrarExito(string mensaje)
        {
            gridRoles.JSProperties["cpMessageType"] = "success";
            gridRoles.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarExitoConCierre(string mensaje)
        {
            gridRoles.JSProperties["cpMessageType"] = "success";
            gridRoles.JSProperties["cpMessage"] = mensaje;
            gridRoles.JSProperties["cpShouldCloseEdit"] = true;
        }

        private void MostrarError(string mensaje)
        {
            gridRoles.JSProperties["cpMessageType"] = "error";
            gridRoles.JSProperties["cpMessage"] = mensaje;
        }

        #endregion

        protected void gridRoles_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            ASPxFormLayout formLayout = gridRoles.FindEditFormTemplateControl("FormLayoutRoles") as ASPxFormLayout;
            if (formLayout != null){
                LayoutItem layoutItem = formLayout.FindItemByFieldName("layoutItemActivo");
                if(gridRoles.IsNewRowEditing)
                {
                    layoutItem.Visible = false;
                }
                else
                {
                    layoutItem.Visible = true;
                }
            }
        }

    }
}