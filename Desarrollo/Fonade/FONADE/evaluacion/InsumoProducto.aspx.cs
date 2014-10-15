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
    public partial class InsumoProducto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String valor = Request["Aporte"];
            String sql;
            sql = @"SELECT [NomProducto], [NomTipoInsumo], [nomInsumo]
                  FROM [Fonade].[dbo].[ProyectoProducto], [Fonade].[dbo].[ProyectoInsumo], [Fonade].[dbo].[ProyectoProductoInsumo], [Fonade].[dbo].[TipoInsumo]
                  WHERE [Id_Producto] = [CodProducto] AND [Id_Insumo] = [CodInsumo] AND [Id_TipoInsumo] = [codTipoInsumo] AND [Id_Producto] = " + valor;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if(reader.Read())
                {
                    NomProducto.Text = reader["NomProducto"].ToString();
                    NomTipoInsumo.Text = reader["NomTipoInsumo"].ToString();
                    nomInsumo.Text = reader["nomInsumo"].ToString();
                }
                reader.Close();
            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }
        }
    }
}