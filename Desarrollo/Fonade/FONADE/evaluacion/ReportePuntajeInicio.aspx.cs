#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>16 - 03 - 2014</Fecha>
// <Archivo>ReportePuntajeInicio.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Data;
using Fonade.Negocio;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class ReportePuntajeInicio : Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargaConvocatorias();
            }
        }


        private void CargaConvocatorias()
        {
            DataTable dtConvocatoria =
                consultas.ObtenerDataTable("SELECT Id_Convocatoria, NomConvocatoria FROM Convocatoria", "text");

            if (dtConvocatoria.Rows.Count!=0)
            {
                Session["dtConvocatoria"] = dtConvocatoria;
                GrvConvocatorias.DataSource = Session["dtConvocatoria"];
                GrvConvocatorias.DataBind();
            }
        }

        protected void GrvConvocatorias_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            GrvConvocatorias.PageIndex = e.NewPageIndex;
            GrvConvocatorias.DataSource = Session["dtConvocatoria"];
            GrvConvocatorias.DataBind();
        }

        protected void GrvConvocatorias_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "detallado")
            {
                Session["codConvocatoria"] = e.CommandArgument;
                Response.Redirect("ReportePuntajeDetallado.aspx");
            }
            else if (e.CommandName == "descargar")
            {
                Session["codExcel"] = e.CommandArgument;
                Response.Redirect("ReportePuntajeDetallado.aspx");
            }
        }
    }
}