#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>23 - 03 - 2014</Fecha>
// <Archivo>CrifIngresados.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Web.UI;
using Fonade.Negocio;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class CrifIngresados : Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCrif();
            }
        }

        private void CargarCrif()
        {
            try
            {
                lfecha.Text = DateTime.Now.ToShortDateString();

                if (Session["dtcrif"]!=null)
                {
                    GrvCrif.DataSource = Session["dtcrif"];
                    GrvCrif.DataBind();
                }else
                {
                    GrvCrif.DataSource = Session["dtcrif"];
                    GrvCrif.DataBind();
                }

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        protected void GrvNotificaciones_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            GrvCrif.PageIndex = e.NewPageIndex;
            GrvCrif.DataSource = Session["dtcrif"];
            GrvCrif.DataBind();
        }

        protected void btncerrar_Click(object sender, EventArgs e)
        {
            Session["dtcrif"] = null;
            RedirectPage(false,string.Empty,"cerrar");
           
        }
    }
}