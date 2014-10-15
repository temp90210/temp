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

namespace Fonade.FONADE.AdministrarPerfiles
{
    public partial class FiltroEmprendedorInactivo : Negocio.Base_Page
    {
        public const int PAGE_SIZE = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("EMPRENDEDORES INACTIVOS");
            }
        }

        protected void Image1_Click(object sender, ImageClickEventArgs e)
        {
            //lds_Emprend_Selecting(lds_emprendedores, );
            this.lds_emprendedores.DataBind();
            this.GVEmprendedoresInac.DataBind();
        }

        protected void lds_Emprend_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            // LINQ query

            try
            {
                var query = from P in consultas.VerEmprendedoresInact(usuario.CodInstitucion, Constantes.CONST_RolEmprendedor, FiltroBuscar.SelectedValue, TextBoxfiltro.Text)
                            select P;

                switch (GVEmprendedoresInac.SortExpression)
                {
                    case "Nombres":
                        if (GVEmprendedoresInac.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.Nombres);
                        else
                            query = query.OrderByDescending(t => t.Nombres);
                        break;
                    case "apellidos":
                        if (GVEmprendedoresInac.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.apellidos);
                        else
                            query = query.OrderByDescending(t => t.Nombres);
                        break;
                    case "email":
                        if (GVEmprendedoresInac.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.email);
                        else
                            query = query.OrderByDescending(t => t.Nombres);
                        break;
                    case "identificacion":
                        if (GVEmprendedoresInac.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.identificacion);
                        else
                            query = query.OrderByDescending(t => t.Nombres);
                        break;
                }


                e.Arguments.TotalRowCount = query.Count();

                // Get only the rows we need for the page requested
                query = query.Skip(GVEmprendedoresInac.PageIndex * PAGE_SIZE).Take(PAGE_SIZE);

                e.Result = query;
            }
            catch (Exception exc)
            { }

        }

        protected void FiltroBuscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (FiltroBuscar.SelectedValue == "Todo")
            {
                this.lds_emprendedores.DataBind();
                this.GVEmprendedoresInac.DataBind();
                TextBoxfiltro.Text = "";
            }
            
        }
    }
}