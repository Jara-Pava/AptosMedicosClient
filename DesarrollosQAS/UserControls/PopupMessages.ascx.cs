using System;
using System.Web.UI;

namespace DesarrollosQAS.UserControls
{
    public partial class PopupMessages : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Muestra un popup de confirmación
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        /// <param name="callbackConfirmar">Función JavaScript a ejecutar al confirmar</param>
        public void MostrarConfirmacion(string mensaje, string callbackConfirmar)
        {
            string script = string.Format(@"
                lblMensajeConfirmacion.SetText('{0}');
                btnConfirmar.SetClientSideEvents({{
                    Click: function(s, e) {{ 
                        {1}; 
                        pcConfirmarAccion.Hide(); 
                    }}
                }});
                pcConfirmarAccion.Show();
            ", EscaparJavaScript(mensaje), callbackConfirmar);

            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarConfirmacion", script, true);
        }

        /// <summary>
        /// Muestra un mensaje de éxito
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        /// <param name="redirectUrl">URL opcional para redireccionar después de cerrar</param>
        public void MostrarExito(string mensaje, string redirectUrl = null)
        {
            string redirectScript = string.IsNullOrEmpty(redirectUrl)
                ? "pcMensajeExito.Hide();"
                : string.Format("window.location.href='{0}';", redirectUrl);

            string script = string.Format(@"
                lblMensajeExito.SetText('{0}');
                btnCerrarExito.SetClientSideEvents({{
                    Click: function(s, e) {{ {1} }}
                }});
                pcMensajeExito.Show();
            ", EscaparJavaScript(mensaje), redirectScript);

            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarExito", script, true);
        }

        /// <summary>
        /// Muestra un mensaje de error
        /// </summary>
        /// <param name="mensaje">Mensaje de error a mostrar</param>
        public void MostrarError(string mensaje)
        {
            string script = string.Format(@"
                lblMensajeError.SetText('{0}');
                pcMensajeError.Show();
            ", EscaparJavaScript(mensaje));

            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarError", script, true);
        }

        /// <summary>
        /// Escapa caracteres especiales para JavaScript
        /// </summary>
        private string EscaparJavaScript(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            return texto
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", "\\n");
        }
    }
}

