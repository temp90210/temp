using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.evaluacion
{
    public partial class ReportesEvaluacion : Negocio.Base_Page
    {
        public String codProyecto;
        public String codConvocatoria;
        public int txtTab = Constantes.CONST_RolEvaluador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                codProyecto = Session["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
                codConvocatoria = Session["codConvocatoria"].ToString();

                codProyecto = Request.QueryString["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
            }
            catch (Exception) { }

            //L_Fecha.Text = "" + DateTime.Now.Day + " Del Mes " + DateTime.Now.Month + " De " + DateTime.Now.Year;
        }

        public DataTable Convocatoria()
        {
            DataTable datatable = new DataTable();

            datatable.Columns.Add("Id_Convocatoria");
            datatable.Columns.Add("NomConvocatoria");

            String sql;
            sql = "SELECT [Id_Convocatoria],[NomConvocatoria] FROM [Fonade].[dbo].[Convocatoria] order by [NomConvocatoria]";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["Id_Convocatoria"] = reader["Id_Convocatoria"].ToString();
                    fila["NomConvocatoria"] = reader["NomConvocatoria"].ToString();
                    datatable.Rows.Add(fila);
                }
                reader.Close();
            }
            catch (SqlException)
            {
            }
            finally
            {
                conn.Close();
            }

            return datatable;
        }

        public DataTable Departamento()
        {
            DataTable datatable = new DataTable();

            datatable.Columns.Add("Id_Departamento");
            datatable.Columns.Add("NomDepartamento");

            String sql;
            sql = "SELECT [Id_Departamento],[NomDepartamento] FROM [Fonade].[dbo].[departamento] order by [NomDepartamento]";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["Id_Departamento"] = reader["Id_Departamento"].ToString();
                    fila["NomDepartamento"] = reader["NomDepartamento"].ToString();
                    datatable.Rows.Add(fila);
                }
                reader.Close();
            }
            catch (SqlException)
            {
            }
            finally
            {
                conn.Close();
            }

            return datatable;
        }

        public DataTable Sector()
        {
            DataTable datatable = new DataTable();

            datatable.Columns.Add("Id_Sector");
            datatable.Columns.Add("NomSector");

            String sql;
            sql = "SELECT [Id_Sector] ,[NomSector] FROM [Fonade].[dbo].[Sector] ORDER BY [NomSector]";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["Id_Sector"] = reader["Id_Sector"].ToString();
                    fila["NomSector"] = reader["NomSector"].ToString();
                    datatable.Rows.Add(fila);
                }
                reader.Close();
            }
            catch (SqlException)
            {
            }
            finally
            {
                conn.Close();
            }
            return datatable;
        }

        protected void B_Buscar1_Click(object sender, EventArgs e)
        {
            Session["idConvocatoriaEval"] = DDL_Convocatoria.SelectedValue;
            //Session["idDepartamentoEval"] = LB_Departamento.SelectedValue;
            Session["idDepartamentoEval"] = "";
            Session["idNombreConvocatoria"] = DDL_Convocatoria.SelectedItem;

            foreach (ListItem item in LB_Departamento.Items)
            {
                if (item.Selected)
                {
                    Session["idDepartamentoEval"] = Session["idDepartamentoEval"].ToString() + " " + item.Value;
                }
            }

            Response.Redirect("ReporteEvaluadores.aspx");
        }

        protected void B_Buscar2_Click(object sender, EventArgs e)
        {
            Session["idConvocatoriaEval"] = DDL_ConvocatoriaSector.SelectedValue;
            Session["idCIIUEval"] = "";
            Session["idNombreConvocatoria"] = DDL_ConvocatoriaSector.SelectedItem;
            Session["idViable"] = RB_Viavilidad.SelectedValue;

            foreach (ListItem item in LB_CIIU.Items)
            {
                if (item.Selected)
                {
                    Session["idCIIUEval"] = Session["idCIIUEval"].ToString() + " " + item.Value;
                }
            }
            Response.Redirect("ReporteSectores.aspx");
           // Response.Redirect("ImprimirPlanOperativos.apsx");

        }

        protected void B_Buscar3_Click(object sender, EventArgs e)
        {
            Session["idConvocatoriaEval"] = DDL_ConvocatoriaEvaluadoresSector.SelectedValue;
            Session["idNombreConvocatoria"] = DDL_ConvocatoriaEvaluadoresSector.SelectedItem;

           Response.Redirect("ReporteEvaluadoresSector.aspx");
           // Response.Redirect("ImprimirPlanOperativos.apsx");
        }

        protected void B_Buscar4_Click(object sender, EventArgs e)
        {
            Session["idConvocatoriaEval"] = DDL_ConvocatoriaFechas.SelectedValue;
            Session["idNombreConvocatoria"] = DDL_ConvocatoriaFechas.SelectedItem;

            Response.Redirect("ReporteFechas.aspx");
        }

        protected void LB_DepartamentoSector_Click(object sender, EventArgs e)
        {
            Session["idConvocatoriaEval"] = DDL_Empleos.SelectedValue;
            Session["idNombreConvocatoria"] = DDL_Empleos.SelectedItem;

            Response.Redirect("ReporteEmpleo.aspx?codPagina=1");
        }

        protected void LB_Consolidado_Click(object sender, EventArgs e)
        {
            Session["idConvocatoriaEval"] = DDL_Empleos.SelectedValue;
            Session["idNombreConvocatoria"] = DDL_Empleos.SelectedItem;

            Response.Redirect("ReporteEmpleo.aspx?codPagina=2");
        }

        protected void LB_ConsolidadoNacionalSector_Click(object sender, EventArgs e)
        {
            Session["idConvocatoriaEval"] = DDL_Empleos.SelectedValue;
            Session["idNombreConvocatoria"] = DDL_Empleos.SelectedItem;

            Response.Redirect("ReporteEmpleo.aspx?codPagina=3");
        }
    }
}