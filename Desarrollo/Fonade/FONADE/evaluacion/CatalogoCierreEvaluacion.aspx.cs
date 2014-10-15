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
    public partial class CatalogoCierreEvaluacion : System.Web.UI.Page
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

            String sql = "SELECT Id_Convocatoria, NomConvocatoria FROM Convocatoria";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["Id"] = reader["Id_Convocatoria"].ToString();
                    fila["Nombre"] = reader["NomConvocatoria"].ToString();
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


            DataTable datatable1 = new DataTable();

            datatable1.Columns.Add("id_Parametro");
            datatable1.Columns.Add("nomParametro");

            for (int i = 0; i < datatable.Rows.Count; i++)
            {
                String txtSQL = "select id_Parametro, nomParametro from Parametro WHERE NomParametro = 'FechaCierreEvaluacion" + datatable.Rows[i]["Nombre"].ToString().Trim() + "'";
                
                SqlCommand cmd1 = new SqlCommand(txtSQL, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader1 = cmd1.ExecuteReader();

                    if (reader1.Read())
                    {
                        try
                        {
                            DataRow fila11 = datatable1.NewRow();
                            fila11["id_Parametro"] = reader1["id_Parametro"].ToString();
                            fila11["nomParametro"] = reader1["nomParametro"].ToString();
                            datatable1.Rows.Add(fila11);
                        }
                        catch (IndexOutOfRangeException) { }
                        reader1.Close();
                    }
                    else
                    {
                        conn.Close();
                        txtSQL = "INSERT INTO Parametro (NomParametro, Valor) VALUES('FechaCierreEvaluacion" + datatable.Rows[i]["Nombre"].ToString().Trim() + "','')";
                        SqlCommand cmd2 = new SqlCommand(txtSQL, conn);

                        try
                        {

                            conn.Open();
                            cmd2.ExecuteReader();
                            conn.Close();
                        }
                        catch (SqlException se)
                        {
                        }
                        finally
                        {
                            conn.Close();
                        }
                        txtSQL = "select id_Parametro, nomParametro from Parametro WHERE NomParametro = 'FechaCierreEvaluacion" + datatable.Rows[i]["Nombre"].ToString().Trim() + "'";
                        cmd1 = new SqlCommand(txtSQL, conn);

                        try
                        {
                            conn.Open();
                            SqlDataReader reader2 = cmd.ExecuteReader();

                            if (reader2.Read())
                            {
                                try
                                {
                                    DataRow fila = datatable1.NewRow();
                                    fila["id_Parametro"] = reader2["id_Parametro"].ToString();
                                    fila["nomParametro"] = reader2["nomParametro"].ToString();
                                    datatable1.Rows.Add(fila);
                                }
                                catch (IndexOutOfRangeException) { }
                            } reader2.Close();
                        }
                        catch (SqlException)
                        {
                        }
                        finally
                        {
                            conn.Close();
                        }
                        
                    }
                }
                catch (SqlException)
                {
                }
                finally
                {
                    conn.Close();
                }
            }

                return datatable1;
        }

        public void Modificar(String nomParametro, Int32 id_Parametro)
        {

        }

        protected void LB_Editar_Nombre_Click(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow filaDta = GV_Reporte.Rows[indicefila];

            Int64 idNomParametro = Int64.Parse(GV_Reporte.DataKeys[filaDta.RowIndex].Value.ToString());

            Session["idNomParametro"] = idNomParametro;

            Response.Redirect("CatalogoCierreEvaluacionModificar.aspx");
        }
    }
}