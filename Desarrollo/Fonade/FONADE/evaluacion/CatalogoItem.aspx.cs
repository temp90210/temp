using Datos;
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
    public partial class CatalogoItem : System.Web.UI.Page
    {
        public String codProyecto;
        public String codConvocatoria;
        public int txtTab = Constantes.CONST_RolEvaluador;
        public String Aspecto;

        protected void Page_Load(object sender, EventArgs e)
        {
            datos();
            //quemados
            //codProyecto = "51525";
            //codConvocatoria = "189";

            L_Fecha.Text = "" + DateTime.Now.Day + " Del Mes " + DateTime.Now.Month + " De " + DateTime.Now.Year;
        }

        private void datos()
        {
            try
            {
                Aspecto = Session["txtTabAspecto"].ToString();

                codProyecto = Session["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
                codConvocatoria = Session["codConvocatoria"].ToString();

                codProyecto = Request.QueryString["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
            }
            catch (Exception) { }
        }

        protected void B_Crear_Click(object sender, EventArgs e)
        {
            String NomItem = TB_NomItem.Text;
            Int32 ID_Item;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO [Fonade].[dbo].[Item] ( [NomItem], [CodTabEvaluacion] ) VALUES ('" + NomItem + "','" + Session["txtTabAspecto"].ToString() + "')", conn);
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();

                cmd = new SqlCommand("SELECT MAX([Id_Item]) AS MAXIMO FROM [Fonade].[dbo].[Item]", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    ID_Item = Int32.Parse(reader["MAXIMO"].ToString());
                else
                    ID_Item = 1;
                conn.Close();

                cmd = new SqlCommand("INSERT INTO [Fonade].[dbo].[EvaluacionEvaluador] ( [CodProyecto], [CodConvocatoria], [CodItem] ) VALUES('" + codProyecto + "','" + codConvocatoria + "','" + ID_Item + "')", conn);
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();

                for (int i = 1; i <= 5; i++)
                {
                    String objetoteesto = "Texto" + i;
                    String objetopuntaje = "Puntaje" + i;

                    TextBox textboxTexto = (TextBox)this.FindControl(objetoteesto);
                    TextBox textboxPuntaje = (TextBox)this.FindControl(objetopuntaje);

                    if (!String.IsNullOrEmpty(textboxTexto.Text))
                    {
                        if (String.IsNullOrEmpty(textboxPuntaje.Text))
                            textboxPuntaje.Text = "0";

                        cmd = new SqlCommand("INSERT INTO [Fonade].[dbo].[ItemEscala] ( [CodItem], [Texto], [Puntaje] ) VALUES ('" + ID_Item + "','" + textboxTexto.Text + "','" + textboxPuntaje.Text + "')", conn);
                        try
                        {
                            conn.Close();
                            conn.Open();
                            cmd.ExecuteReader();
                            conn.Close();
                        }catch(SqlException){}
                    }
                }
            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }

            redirigir();
        }

        private void redirigir()
        {
            switch (Int32.Parse(Session["txtTabAspecto"].ToString()))
            {
                case 1:
                    Response.Redirect("EvaluacionFinanciera.aspx?txtTab=1");
                    break;
                case 4:
                    Response.Redirect("EvaluacionFinanciera.aspx?txtTab=4");
                    break;
                case 15:
                    Response.Redirect("EvaluacionFinanciera.aspx?txtTab=15");
                    break;
            }
        }

        protected void B_Cancelar_Click(object sender, EventArgs e)
        {
            redirigir();
        }
    }
}