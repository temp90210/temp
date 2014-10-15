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
    public partial class EvaluadorHojaAvanceGerente : System.Web.UI.Page
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

            String sql = "SELECT id_contacto, Nombres +' '+ Apellidos AS nombre FROM Contacto WHERE (Id_Contacto IN (SELECT DISTINCT CodContacto FROM ProyectoContacto WHERE (CodRol = 5) AND (Inactivo = 0)))";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["Id"] = reader["id_contacto"].ToString();
                    fila["Nombre"] = reader["nombre"].ToString();
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

        protected void LB_Contacto_Click(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GV_fila = GV_Reporte.Rows[indicefila];

            Int64 idContacto = Int64.Parse(GV_Reporte.DataKeys[GV_fila.RowIndex].Value.ToString());

            Session["EvalContactoDetalle"] = idContacto;

            Response.Redirect("EvaluadorHojaAvanceGerenteDetalle.aspx");
        }
    }
}