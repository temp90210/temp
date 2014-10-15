#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>08 - 03 - 2014</Fecha>
// <Archivo>EvaluacionTablaFrame.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Web.UI;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class EvaluacionTablaFrame : Page
    {
        public int AspectoId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
                CargarCampos();
            }
        }

        public void CargarCampos()
        {
            AspectoId = !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Convert.ToInt32(Session["CodProyecto"].ToString()) : 0;
        }
    }
}