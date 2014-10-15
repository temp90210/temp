using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.FONADE.Proyecto
{
    public partial class ProyectoFormalizar : Negocio.Base_Page
    {
        public const int PAGE_SIZE = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("PROYECTOS A FORMALIZAR");
            }
        }

        protected void lds_proyectos_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            // LINQ query

            try
            {
                var query = from P in consultas.FormalizarProyecto(usuario.IdContacto, Constantes.CONST_Anexos, usuario.CodInstitucion, Constantes.CONST_Inscripcion)
                            select P;

                switch (GridViewProyectos.SortExpression)
                {
                    case "Lugar":
                        if (GridViewProyectos.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.Lugar);
                        else
                            query = query.OrderByDescending(t => t.nomconvocatoria);
                        break;
                    case "nomproyecto":
                        if (GridViewProyectos.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.nomproyecto);
                        else
                            query = query.OrderByDescending(t => t.nomconvocatoria);
                        break;
                    case "nomconvocatoria":
                        if (GridViewProyectos.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.nomconvocatoria);
                        else
                            query = query.OrderByDescending(t => t.nomconvocatoria);
                        break;
                }  


                e.Arguments.TotalRowCount = query.Count();

                // Get only the rows we need for the page requested
                query = query.Skip(GridViewProyectos.PageIndex * PAGE_SIZE).Take(PAGE_SIZE);

                e.Result = query;
            }
            catch (Exception exc)
            {}

        }

        protected void GridViewProyectos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Proyecto"))
            {
                Session["CodProyecto"] = e.CommandArgument.ToString();
                Response.Redirect("ProyectoFrameSet.aspx");
            }
            else
            {
                if (e.CommandName.Equals("adiocional"))
                {
                    Response.Redirect(e.CommandArgument.ToString());
                }
            }
        }

    }
}