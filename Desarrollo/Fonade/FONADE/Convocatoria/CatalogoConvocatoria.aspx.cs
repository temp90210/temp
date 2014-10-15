using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Fonade.Account;
using LinqKit;
using AjaxControlToolkit;
using System.ComponentModel;

namespace Fonade.FONADE.Convocatoria
{
    public partial class CatalogoConvocatoria1 : Negocio.Base_Page
    {
        public const int PAGE_SIZE = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("CONVOCATORIAS");
            }
        }

        protected void lds_listadoConvoct_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

            try
            {
                var query = from P in consultas.VerListadoConvocatorias()
                            select P;

                

                switch (GridViewConvoct.SortExpression)
                {
                    case "nomConvocatoria":
                        if (GridViewConvoct.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.nomConvocatoria);
                        else
                            query = query.OrderByDescending(t => t.nomConvocatoria);
                        break;
                    case "FechaInicio":
                        if (GridViewConvoct.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.FechaInicio);
                        else
                            query = query.OrderByDescending(t => t.nomConvocatoria);
                        break;
                    case "FechaFin":
                        if (GridViewConvoct.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.FechaFin);
                        else
                            query = query.OrderByDescending(t => t.nomConvocatoria);
                        break;
                    case "Publicado":
                        if (GridViewConvoct.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.Publicado);
                        else
                            query = query.OrderByDescending(t => t.nomConvocatoria);
                        break;
                    default:
                        query = query.OrderByDescending(t => t.nomConvocatoria);
                        break;
                }  


                e.Arguments.TotalRowCount = query.Count();

                query = query.Skip(GridViewConvoct.PageIndex * PAGE_SIZE).Take(PAGE_SIZE);

                e.Result = query;
            }
            catch (Exception exc)
            { }

        }

        protected void GridViewConvoct_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToString().Equals("VerProyectosConvatoria"))
            {
                string IdConvoct = e.CommandArgument.ToString();
                Session["Id_ProyPorConvoct"] = IdConvoct;
                Response.Redirect("ProyectosPorConvocatoria.aspx");
            }
            if (e.CommandName.ToString().Equals("VerConvocatoria"))
            {
                string IdConvoct = e.CommandArgument.ToString();
                Session["IdConvocatoria"] = IdConvoct;
                Response.Redirect("Convocatoria.aspx");
            }
            if (e.CommandName.ToString().Equals("VerEvalConvatoria"))
            {
                string IdConvoct = e.CommandArgument.ToString();
                Session["Id_EvalConvocatoria"] = IdConvoct;
                Response.Redirect("EvaluacionConvocatoria.aspx");
            }
            
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["IdConvocatoria"] = "0";
            Response.Redirect("Convocatoria.aspx");
        }

    }


}