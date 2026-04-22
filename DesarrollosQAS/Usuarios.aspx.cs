using DataAccessDesarrollos;
using DataAccessDesarrollos.Repositorios;
using DesarrollosQAS.Model;
using DevExpress.Web;
using DevExpress.XtraExport.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DesarrollosQAS
{
    public partial class Usuarios : System.Web.UI.Page
    {
        // Mantener el filtro seleccionado entre postbacks
        private string FiltroEstado
        {
            get { return ViewState["FiltroEstado"] as string ?? "activos"; }
            set { ViewState["FiltroEstado"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FiltroEstado = "default";
                BindGrid();
            }

            // Aplicar estilos al popup en cada carga
            gridUsuarios.StylesPopup.EditForm.Header.BackColor = System.Drawing.ColorTranslator.FromHtml("#353943");
            gridUsuarios.StylesPopup.EditForm.Header.ForeColor = System.Drawing.Color.White;
            gridUsuarios.StylesPopup.EditForm.Header.Font.Bold = true;
        }

        /// <summary>
        /// Obtiene la sigla de red del usuario logueado actualmente
        /// </summary>
        private string ObtenerUsuarioActual()
        {
            var user = AuthHelper.GetLoggedInUserInfo();
            return user?.Sigla_red ?? "Sistema";
        }

        private void BindGrid()
        {
            var repo = new UsuarioSistemaRepository();
            var usuarios = repo.ObtenerTodosUsuarios().OrderBy(p => p.id_usuario).ToList();

            switch (FiltroEstado)
            {
                case "activos":
                    gridUsuarios.DataSource = usuarios.Where(u => u.activo).ToList();
                    break;
                case "inactivos":
                    gridUsuarios.DataSource = usuarios.Where(u => !u.activo).ToList();
                    break;
                default: // "todos"
                    gridUsuarios.DataSource = usuarios;
                    break;
            }

            gridUsuarios.DataBind();
        }

        protected void gridUsuarios_DataBinding(object sender, EventArgs e)
        {
            var repo = new UsuarioSistemaRepository();
            var usuarios = repo.ObtenerTodosUsuarios().OrderBy(p => p.id_usuario).ToList();

            switch (FiltroEstado)
            {
                case "activos":
                    gridUsuarios.DataSource = usuarios.Where(u => u.activo).ToList();
                    break;
                case "inactivos":
                    gridUsuarios.DataSource = usuarios.Where(u => !u.activo).ToList();
                    break;
                default:
                    gridUsuarios.DataSource = usuarios;
                    break;
            }
        }

        protected void gridUsuarios_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            // Obtener los controles del formulario
            ASPxTextBox txtSiglaRed = gridUsuarios.FindEditFormTemplateControl("txtSiglaRed") as ASPxTextBox;
            ASPxLabel lblActivo = gridUsuarios.FindEditFormTemplateControl("lblActivo") as ASPxLabel;
            ASPxCheckBox chkActivo = gridUsuarios.FindEditFormTemplateControl("chkActivo") as ASPxCheckBox;

            if (gridUsuarios.IsNewRowEditing)
            {
                // Modo CREACIÓN: Ocultar campo Activo
                if (lblActivo != null)
                {
                    lblActivo.Visible = false;
                }
                if (chkActivo != null)
                {
                    chkActivo.Visible = false;
                    chkActivo.Checked = true; // Por defecto activo en creación
                }

                // Habilitar sigla_red en modo creación
                if (txtSiglaRed != null)
                {
                    txtSiglaRed.ClientEnabled = true;
                    txtSiglaRed.BackColor = System.Drawing.Color.White;
                }
            }
            else
            {
                // Modo EDICIÓN: Mostrar campo Activo y deshabilitar sigla_red
                if (lblActivo != null)
                {
                    lblActivo.Visible = true;
                }
                if (chkActivo != null)
                {
                    chkActivo.Visible = true;
                    chkActivo.Enabled = true; // Permitir editar el estado
                }

                // Deshabilitar sigla_red en modo edición
                if (txtSiglaRed != null)
                {
                    txtSiglaRed.ClientEnabled = false;
                    txtSiglaRed.BackColor = System.Drawing.ColorTranslator.FromHtml("#f0f0f0");
                }
            }
        }

        protected void gridUsuarios_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            // Manejar filtro de estado
            if (e.Parameters.StartsWith("FILTER|"))
            {
                string[] parts = e.Parameters.Split('|');
                FiltroEstado = parts[1];
                BindGrid();
                return;
            }

            // Manejar eliminación (soft delete)
            if (e.Parameters.StartsWith("DELETE|"))
            {
                string[] parts = e.Parameters.Split('|');
                int visibleIndex = Convert.ToInt32(parts[1]);
                int id = Convert.ToInt32(gridUsuarios.GetRowValues(visibleIndex, "id_usuario"));
                string siglaRed = gridUsuarios.GetRowValues(visibleIndex, "sigla_red").ToString();
                try
                {
                    var repo = new UsuarioSistemaRepository();
                    repo.EliminarUsuario(id);

                    BindGrid();
                    MostrarExito($"Proceso exitoso al eliminar el usuario {siglaRed}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Proceso no exitoso, no se ha eliminado el usuario: {0}", ex);
                    MostrarError($"Proceso no exitoso al eliminar el usuario {siglaRed}: {ex.Message}");
                }
            }
        }

        protected void gridUsuarios_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            // Este método ya no elimina directamente, solo se usa para el evento del cliente
        }
        protected void gridUsuarios_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            string nombre = e.NewValues["nombre"]?.ToString()?.Trim();
            string siglaRed = e.NewValues["sigla_red"]?.ToString()?.Trim();
            string email = e.NewValues["Email"]?.ToString()?.Trim();
            try
            {
                // En modo creación, siempre crear como activo
                bool activo = true;

                if (!ValidarNombre(nombre, out string errorNombre))
                {
                    e.Cancel = true;
                    MostrarErrorConFormulario(errorNombre, true, -1);
                    return;
                }

                if (!ValidarSigla(siglaRed, out string errorSigla))
                {
                    e.Cancel = true;
                    MostrarErrorConFormulario(errorSigla, true, -1);
                    return;
                }

                if (!ValidarEmail(email, out string errorEmail))
                {
                    e.Cancel = true;
                    MostrarErrorConFormulario(errorEmail, true, -1);
                    return;
                }

                var repo = new UsuarioSistemaRepository();
                if (repo.ExisteUsuario(siglaRed, email, out string mensajeExistencia))
                {
                    e.Cancel = true;
                    MostrarErrorConFormulario(mensajeExistencia, true, -1);
                    return;
                }

                var usuario = new Usuario
                {
                    nombre = nombre,
                    sigla_red = siglaRed,
                    Email = email,
                    activo = activo,
                    creado_por = ObtenerUsuarioActual()
                };

                if (!repo.CrearUsuario(usuario))
                {
                    throw new ApplicationException($"No se ha podido crear el usuario {usuario.nombre}");
                }

                e.Cancel = true;
                gridUsuarios.CancelEdit();
                BindGrid();
                MostrarExito($"Proceso exitoso al crear el usuario con la sigla de red {usuario.sigla_red}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error en RowInserting: {0}", ex);
                e.Cancel = true;
                MostrarErrorConFormulario($"Proceso no exitoso al crear el usuario {siglaRed}: {ex.Message}", true, -1);
            }
        }

        protected void gridUsuarios_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            int editIndex = gridUsuarios.EditingRowVisibleIndex;

            try
            {
                string nombre = e.NewValues["nombre"]?.ToString()?.Trim();
                string email = e.NewValues["Email"]?.ToString()?.Trim();

                if (!ValidarNombre(nombre, out string errorNombre))
                {
                    e.Cancel = true;
                    MostrarErrorConFormulario(errorNombre, false, editIndex);
                    return;
                }

                if (!ValidarEmail(email, out string errorEmail))
                {
                    e.Cancel = true;
                    MostrarErrorConFormulario(errorEmail, false, editIndex);
                    return;
                }

                string siglaRedOriginal = e.OldValues["sigla_red"]?.ToString();

                // Obtener el valor del checkbox desde el template
                ASPxCheckBox chkActivo = gridUsuarios.FindEditFormTemplateControl("chkActivo") as ASPxCheckBox;
                bool activo = chkActivo != null ? chkActivo.Checked : false;

                var usuario = new Usuario
                {
                    id_usuario = Convert.ToInt32(e.Keys["id_usuario"]),
                    nombre = nombre,
                    sigla_red = siglaRedOriginal,
                    Email = email,
                    activo = activo,
                    modificado_por = ObtenerUsuarioActual()
                };

                var repo = new UsuarioSistemaRepository();

                if (repo.ExisteUsuarioEmail(siglaRedOriginal, email, out string mensajeExistencia))
                {
                    e.Cancel = true;
                    MostrarErrorConFormulario(mensajeExistencia, true, -1);
                    return;
                }

                if (!repo.ActualizarUsuario(usuario))
                {
                    throw new ApplicationException($"Proceso no exitoso al actualizar el usuario con la sigla de red {usuario.sigla_red}");
                }

                e.Cancel = true;
                gridUsuarios.CancelEdit();
                BindGrid();
                MostrarExito($"Proceso exitoso al actualizar el usuario con la sigla de red {usuario.sigla_red}.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error al actualizar usuario: {0}", ex);
                e.Cancel = true;
                MostrarErrorConFormulario($"Proceso no exitoso al actualizar el usuario por : {ex.Message}", false, editIndex);
            }
        }

        #region Métodos de Validación

        private bool ValidarNombre(string nombre, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensajeError = "Error el nombre no puede estar vacío o contener solo espacios.";
                return false;
            }

            if (Regex.IsMatch(nombre, @"^[\W\d_]+$"))
            {
                mensajeError = "Error el nombre debe contener al menos letras válidas.";
                return false;
            }

            return true;
        }

        private bool ValidarEmail(string email, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                mensajeError = "El email es requerido.";
                return false;
            }

            return true;
        }

        private bool ValidarSigla(string sigla, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(sigla))
            {
                mensajeError = "La sigla de red es requerida.";
                return false;
            }

            if (!Regex.IsMatch(sigla, @"^[a-zA-Z0-9]+$"))
            {
                mensajeError = "La sigla solo puede contener letras y números, sin espacios.";
                return false;
            }
            return true;
        }

        #endregion

        #region Métodos de Mensajes

        private void MostrarExito(string mensaje)
        {
            gridUsuarios.JSProperties["cpMessageType"] = "success";
            gridUsuarios.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarError(string mensaje)
        {
            gridUsuarios.JSProperties["cpMessageType"] = "error";
            gridUsuarios.JSProperties["cpMessage"] = mensaje;
        }

        private void MostrarErrorConFormulario(string mensaje, bool isNewRow, int editIndex)
        {
            gridUsuarios.JSProperties["cpMessageType"] = "error";
            gridUsuarios.JSProperties["cpMessage"] = mensaje;
            gridUsuarios.JSProperties["cpShouldReopenEdit"] = true;
            gridUsuarios.JSProperties["cpIsNewRow"] = isNewRow;
            gridUsuarios.JSProperties["cpEditIndex"] = editIndex;
        }
        #endregion
    }
}