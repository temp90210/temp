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
    public partial class ayudaArancel : System.Web.UI.Page
    {
        DataTable datatable = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            datatable.Columns.Add("PosicionArancelaria");
            datatable.Columns.Add("Descripcion");
        }

        public DataTable resultado()
        {
            return datatable;
        }

        protected void HL_Direccionar_DataBinding(object sender, EventArgs e)
        {
            
        }

        private void Buscar(int consulta)
        {
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            try
            {

                if (consulta == 1)
                    cmd = new SqlCommand("SELECT DISTINCT [PosicionArancelaria],[Descripcion]  FROM [Fonade].[dbo].[PosicionArancelaria]  WHERE [PosicionArancelaria] LIKE '" + TB_Codigo.Text + "%';", conn);
                else
                    cmd = new SqlCommand("SELECT DISTINCT [PosicionArancelaria],[Descripcion]  FROM [Fonade].[dbo].[PosicionArancelaria]  WHERE [Descripcion] LIKE '" + TB_Codigo.Text + "%';", conn);
            }
            catch (Exception) {
                cmd = new SqlCommand("SELECT DISTINCT [PosicionArancelaria],[Descripcion]  FROM [Fonade].[dbo].[PosicionArancelaria]  WHERE [Descripcion] LIKE '" + TB_Codigo.Text + "%';", conn);
            }

            datatable = new DataTable();
            datatable.Columns.Add("PosicionArancelaria");
            datatable.Columns.Add("Descripcion");

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();

                    fila["PosicionArancelaria"] = reader["PosicionArancelaria"].ToString();
                    fila["Descripcion"] = reader["Descripcion"].ToString();

                    datatable.Rows.Add(fila);
                }
                reader.Close();
            }
            catch (SqlException se)
            {
                throw se;
            }
            finally
            {
                conn.Close();
            }
        }

        protected void RB_Buscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            String value = RB_Buscar.SelectedValue;

            if (value == "1") Buscar(1);
            else Buscar(2);

            GridView1.DataSource = datatable;
            GridView1.DataBind();
        }

        protected void HL_Direccionar_Click(object sender, EventArgs e)
        {
            try
            {
                ClientScriptManager cm = this.ClientScript;

                var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
                GridViewRow GVInventario = GridView1.Rows[indicefila];

                LinkButton TBCantidades = (LinkButton)GVInventario.FindControl("HL_Direccionar");
                String codigo = TBCantidades.Text;
                String desccripcion = GridView1.DataKeys[GVInventario.RowIndex].Value.ToString();

                Session["txtcodigo"] = codigo;
                Session["desccripcion"] = desccripcion;

                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
            }
            catch (Exception) { }
        }
    }
}