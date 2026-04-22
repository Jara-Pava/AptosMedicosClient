using DevExpress.Web.ASPxTreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DesarrollosQAS.Pages
{
    public partial class RolModulo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void treeList_CustomDataCallback(object sender, DevExpress.Web.ASPxTreeList.TreeListCustomDataCallbackEventArgs e)
        {
            e.Result = treeList.SelectionCount.ToString();
        }
        protected void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void treeList_DataBound(object sender, EventArgs e)
        {
            SetNodeSelectionSettings();
        }

        void SetNodeSelectionSettings()
        {
            TreeListNodeIterator iterator = treeList.CreateNodeIterator();
            TreeListNode node;
            while (true)
            {
                node = iterator.GetNext();
                if (node == null) break;
            }
        }
    }
}