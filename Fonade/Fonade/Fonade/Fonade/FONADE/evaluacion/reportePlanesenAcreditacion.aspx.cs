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
    public partial class reportePlanesenAcreditacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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

        protected void DDL_Convocatoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["idConvocatoriaEvalS"] = DDL_Convocatoria.SelectedValue;
            Session["idNombreConvocatoriaS"] = DDL_Convocatoria.SelectedItem;

            Response.Redirect("reportePlanesenAcreditacionDetalles.aspx");
        }

        protected void B_VerDetalles_Click(object sender, EventArgs e)
        {
            Session["idConvocatoriaEvalS"] = DDL_Convocatoria.SelectedValue;
            Session["idNombreConvocatoriaS"] = DDL_Convocatoria.SelectedItem;

            Response.Redirect("reportePlanesenAcreditacionDetalles.aspx");
        }
    }
}