using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Administracion
{
    public partial class PlanesaAcreditar : Negocio.Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void gvplanes_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.HasControls())
                    {
                        LinkButton lnk = (LinkButton)tc.Controls[0];
                        if (lnk != null && gvplanes.SortExpression == lnk.CommandArgument)
                        {
                            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                            img.ImageUrl = "/Images/ImgFlechaOrden" + (gvplanes.SortDirection == SortDirection.Ascending ? "Up" : "Down") + ".gif";
                            tc.Controls.Add(new LiteralControl(" "));
                            tc.Controls.Add(img);
                        }
                    }
                }
            }
        }

        protected void LinqDataSource1_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var result = from p in consultas.Db.MD_PlanesDeNegocioAcreditar(usuario.IdContacto)
                         select new
                         {
                             p.ID_PROYECTO,
                             p.NOMPROYECTO,
                             p.FECHAASIGNACION,
                             p.DIAS,
                             p.ESTADO,
                             p.CODCONVOCATORIA
                         };

            e.Result = result.ToList();
                
        }

        protected void gvplanes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("frameset"))
            {
                Session["codProyecto"] = e.CommandArgument;
                Response.Redirect("~/FONADE/Proyecto/ProyectoFrameSet.aspx?codProyecto=" + e.CommandArgument.ToString());
            }
            else
            {
                if (e.CommandName.Equals("proyectoacreditar"))
                {
                    Session["ID_PROYECTOAcreditar"] = e.CommandArgument.ToString().Split(';')[0];
                    Session["CODCONVOCATORIAAcreditar"] = e.CommandArgument.ToString().Split(';')[1];

                    Response.Redirect("ProyectoAcreditacion.aspx");
                }
            }
        }
    }
}