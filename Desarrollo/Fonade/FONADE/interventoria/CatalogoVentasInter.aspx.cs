using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Datos;
using Fonade.Negocio;

namespace Fonade.FONADE.interventoria
{
    public partial class CatalogoVentasInter : Negocio.Base_Page
    {
        String CodProduccion;
        int Mes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CodProduccion = Session["CodProduccion"] != null && !string.IsNullOrEmpty(Session["CodProduccion"].ToString()) ? Session["CodProduccion"].ToString() : "0";
                Mes = (int)
                (!string.IsNullOrEmpty(Session["MesDelProducto_Venta_Seleccionado"].ToString())
                     ? Convert.ToInt64(Session["MesDelProducto_Venta_Seleccionado"])
                     : 0);

                ValidarGrupo();
            }
        }

        private void ValidarGrupo()
        {
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
            }

            try
            {
                consultas.Parameters = null;

                consultas.Parameters = new[]
                                           {
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@CodProduccion",
                                                       Value = CodProduccion
                                                   },
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@Mes",
                                                       Value = Mes
                                                   }
                                           };

                var dtDocumentos = consultas.ObtenerDataTable("MD_Consultar_Documento_Ventas");

                if (dtDocumentos.Rows.Count != 0)
                {
                    Session["dtDocumentos"] = dtDocumentos;
                    GrvDocumentos.DataSource = dtDocumentos;
                    GrvDocumentos.DataBind();
                }
                else
                {
                    GrvDocumentos.DataSource = dtDocumentos;
                    GrvDocumentos.DataBind();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            var sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;

                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        protected void GrvDocumentosPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvDocumentos.PageIndex = e.NewPageIndex;
            GrvDocumentos.DataSource = Session["dtDocumentos"];
            GrvDocumentos.DataBind();
        }

        protected void GrvDocumentosSorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GrvDocumentos.DataSource = Session["dtDocumentos"];
                GrvDocumentos.DataBind();
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Redirect(null, "../evaluacion/CatalogoVentasPOInterventoria.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }
    }
}