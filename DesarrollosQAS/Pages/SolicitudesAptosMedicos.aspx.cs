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
    public partial class SolicitudesAptosMedicos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid() {
            var repo = new SolicitudesAptosMedicosRepository();
            List<SolicitudAptoMedico> solicitudes = repo.ObtenerTodasSolicitudesAptosMedicos();
            gridSolicitudesAptosMedicos.DataSource = solicitudes;
            gridSolicitudesAptosMedicos.DataBind();
        }

        protected void gridSolicitudesAptosMedicos_DataBinding(object sender, EventArgs e)
        {
            var repo = new SolicitudesAptosMedicosRepository();
            List<SolicitudAptoMedico> solicitudes = repo.ObtenerTodasSolicitudesAptosMedicos();
            gridSolicitudesAptosMedicos.DataSource = solicitudes;
        }

        protected void gridSolicitudesAptosMedicos_CustomButtonCallback(object sender, DevExpress.Web.ASPxGridViewCustomButtonCallbackEventArgs e)
        {

        }

        protected void gridSolicitudesAptosMedicos_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
        {

        }

        protected void gridSolicitudesAptosMedicos_DataBound(object sender, EventArgs e)
        {
            SetColummnsWidth(sender as ASPxGridView);
        }
        private void SetColummnsWidth(ASPxGridView grid)
        {
            var demoAreaWidth = 894;
            var columnWidth = Math.Max(115, demoAreaWidth / grid.Columns.Count);
            for (var i = 1; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].MinWidth = columnWidth;
            }
            grid.Columns[0].MinWidth = demoAreaWidth - (grid.Columns.Count - 1) * columnWidth;
        }
    }
}