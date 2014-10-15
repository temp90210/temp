using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.Controles
{
    public partial class Alert : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void Ver(string texto, bool mostrar)
        {
            lbl_popup.Visible = mostrar;
            lbl_popup.Text = texto;
            mpe1.Enabled = mostrar;
            mpe1.Show();
        }
    }
}