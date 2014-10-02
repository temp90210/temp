using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.FONADE.Proyecto
{
    public partial class ProyectoResumenFrame : Negocio.Base_Page
    {
        public string codProyecto;
        public string codConvocatoria;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region MyRegion
            //if (Request.QueryString["codProyecto"] != null)
            //{
            //    codProyecto = Request.QueryString["codProyecto"].ToString();
            //}
            //if (Request.QueryString["codConvocatoria"] != null)
            //{
            //    codConvocatoria = Request.QueryString["codConvocatoria"].ToString();
            //} 
            #endregion

            codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            codConvocatoria = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["CodConvocatoria"].ToString() : "0";
        }
    }
}