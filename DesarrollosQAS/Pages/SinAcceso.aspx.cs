using System;

namespace DesarrollosQAS
{
    public partial class SinAcceso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = Environment.UserName;
        }
    }
}