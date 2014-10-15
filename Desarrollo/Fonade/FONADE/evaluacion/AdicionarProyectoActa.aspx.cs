#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>15 - 03 - 2014</Fecha>
// <Archivo>AdicionarProyectoActa.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Linq;
using Datos;
using Fonade.Negocio;
using System.Web.UI;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class AdicionarProyectoActa : Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarProyecto();
            }
        }

        #region Metodos

        private void CargarProyecto()
        {
            try
            {

                ViewState["convocatoria"] = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["CodConvocatoria"].ToString() : "0";
                ViewState["actaId"] = Session["CodActa"] != null && !string.IsNullOrEmpty(Session["CodActa"].ToString()) ? Session["CodActa"].ToString() : "0";

                if (ViewState["convocatoria"] != null)
                {
                    int codigo = !string.IsNullOrEmpty(ViewState["convocatoria"].ToString()) ? Convert.ToInt32(ViewState["convocatoria"].ToString()) :
                    0;

                    consultas.Parameters = new[] { new SqlParameter
                                                   { 
                                                        ParameterName = "@codconvocatoria",
                                                        Value = codigo
                                                   }};
                    DataTable dtActas = consultas.ObtenerDataTable("MD_ObtenerProyectosNegociosActas");

                    if (dtActas.Rows.Count != 0)
                    {
                        Session["dtproyectoActa"] = dtActas;
                        GrvProyectoActas.DataSource = dtActas;
                        GrvProyectoActas.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        void Crearproyectoevaluacion()
        {
            try
            {
                if (GrvProyectoActas.Rows.Count != 0)
                {
                    foreach (GridViewRow rows in GrvProyectoActas.Rows)
                    {
                        var idproyecto = rows.FindControl("idproyecto") as Label;
                        var chkproyecto = rows.FindControl("cidproyecto") as CheckBox;
                        var evaluacionActa = new EvaluacionActaProyecto();
                        if (idproyecto != null)
                        {
                            if (chkproyecto != null && chkproyecto.Checked)
                            {
                                string txtSQL = "insert into evaluacionactaproyecto values (" + ViewState["actaId"].ToString() + "," + idproyecto.Text + ",0)";

                                //evaluacionActa.CodProyecto = Convert.ToInt32(idproyecto.Text);
                                //evaluacionActa.CodActa = Convert.ToInt32(ViewState["actaId"].ToString());
                                //evaluacionActa.Viable = false;
                                try
                                {
                                    ejecutaReader(txtSQL,1);
                                    //consultas.Db.EvaluacionActaProyectos.InsertOnSubmit(evaluacionActa);
                                    //consultas.Db.SubmitChanges();
                                }
                                catch (Exception)
                                {
                                    ClientScriptManager cm = this.ClientScript;
                                    cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Se ha producido un error en la base de datos');</script>");
                                }
                            }
                        }
                    }
                    RedirectPage(true, "Se Adicionaron correctamente!");
                }
                else
                {
                    RedirectPage(true, "");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Eventos Gridview

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

        protected void GrvActasPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvProyectoActas.PageIndex = e.NewPageIndex;
            GrvProyectoActas.DataSource = Session["dtproyectoActa"];
            GrvProyectoActas.DataBind();
        }

        protected void GrvProyectoActasSorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtproyectoActa"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GrvProyectoActas.DataSource = Session["dtproyectoActa"];
                GrvProyectoActas.DataBind();
            }
        }

        #endregion

        protected void Unnamed1_Click(object sender, EventArgs e)
        {
            Crearproyectoevaluacion();
        }
    }
}