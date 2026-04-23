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

            // Mostrar el usuario de Windows directamente, sin autenticación
            lblUsuario.Text = Environment.UserName;

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