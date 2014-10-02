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
    public partial class CatalogoEvaluacionConceptos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //L_Fecha.Text = "" + DateTime.Now.Day + " Del Mes " + DateTime.Now.Month + " De " + DateTime.Now.Year;
        }

        public DataTable contacto()
        {
            DataTable datatable = new DataTable();

            datatable.Columns.Add("Id");
            datatable.Columns.Add("Nombre");

            String sql = "select id_EvaluacionConceptos, nomEvaluacionConceptos from EvaluacionConceptos";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["Id"] = reader["id_EvaluacionConceptos"].ToString();
                    fila["Nombre"] = reader["nomEvaluacionConceptos"].ToString();
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

        public void eliminar(int Id)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand("DELETE FROM EvaluacionConceptos WHERE Id_EvaluacionConceptos=" + Id, conn);
            try
            {

                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }
        }

        public void modificar(Int32 Id, String Nombre)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand("Update EvaluacionConceptos set NomEvaluacionConceptos ='" + Nombre + "' WHERE Id_EvaluacionConceptos=" + Id, conn);
            try
            {

                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }
        }

        protected void IB_AgregarIndicador_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("CatalogoEvaluacionConceptosAdicionar.aspx");
        }

        protected void btn_agregar_Click(object sender, EventArgs e)
        {
            Response.Redirect("CatalogoEvaluacionConceptosAdicionar.aspx");
        }
    }
}