using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoEvaluacionConceptosAdicionar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //L_Fecha.Text = "" + DateTime.Now.Day + " Del Mes " + DateTime.Now.Month + " De " + DateTime.Now.Year;
        }

        protected void B_Nuevo_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand("INSERT INTO EvaluacionConceptos (NomEvaluacionConceptos) VALUES ('" + TB_Nuevo.Text + "')", conn);
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

            Response.Redirect("CatalogoEvaluacionConceptos.aspx");
        }
    }
}