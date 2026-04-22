using DesarrollosQAS.Model;
using DevExpress.Web;
using System;
using System.Globalization;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace DesarrollosQAS
{
    public partial class Root : MasterPage
    {
        public bool EnableBackButton { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string language = "es-MX";
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);

            // Autenticar al usuario de Windows automáticamente (solo una vez por sesión)
            if (!AuthHelper.IsAuthenticated())
            {
                string windowsUser = Environment.UserName;

                if (!AuthHelper.SignIn(windowsUser, null))
                {
                    // El usuario de Windows no existe en SEC_Usuarios, redirigir
                    Response.Redirect("Pages/SinAcceso.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }

            // Mostrar el nombre del usuario autenticado
            var user = AuthHelper.GetLoggedInUserInfo();
            if (user != null)
            {
                lblUsuario.Text = user.Sigla_red;
            }
            else
            {
                lblUsuario.Text = Environment.UserName;
            }

            int collapseAtWindowInnerWidth = 1200;
            NavigationPanel.SettingsAdaptivity.CollapseAtWindowInnerWidth = collapseAtWindowInnerWidth;
            NavigationPanel.JSProperties["cpCollapseAtWindowInnerWidth"] = collapseAtWindowInnerWidth;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Registrar Site.css al final del <head> para que cargue después de los estilos de DevExpress
            var link = new HtmlLink();
            link.Href = ResolveUrl("~/Content/Site.css");
            link.Attributes.Add("rel", "stylesheet");
            link.Attributes.Add("type", "text/css");
            Page.Header.Controls.Add(link);
        }
    }
}