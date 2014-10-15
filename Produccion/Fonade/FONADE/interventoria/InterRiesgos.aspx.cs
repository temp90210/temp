using Datos;
using Fonade.Clases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.interventoria
{
    public partial class InterRiesgos : Negocio.Base_Page
    {
        string CodProyecto;
        string CodEmpresa;
        string CodConvocatoria;
        string anioConvocatoria;
        string txtSQL;

        protected void Page_Load(object sender, EventArgs e)
        {
            datosEntrada();

            if (!IsPostBack)
            {
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    txtSQL = "SELECT COUNT(*) as contador FROM InterventorRiesgoTMP";

                    var rdt = consultas.ObtenerDataTable(txtSQL, "text");

                    if (rdt.Rows.Count > 0)
                        lblRiesgosAprobar.Text = rdt.Rows[0]["contador"].ToString();
                }
                else //Si es Coordinador Interventor...y otros roles...
                {
                    lblRiesgosAprobar.Visible = false;
                    lblrisgostotal.Visible = false;
                    IB_AgregarIndicador.Visible = false;
                    btn_agregar.Visible = false;
                    GridView1.Columns[0].Visible = false;
                }
            }
        }

        private void datosEntrada()
        {
            CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            CodEmpresa = Session["CodEmpresa"] != null && !string.IsNullOrEmpty(Session["CodEmpresa"].ToString()) ? Session["CodEmpresa"].ToString() : "0";

            txtSQL = "SELECT Max(CodConvocatoria) AS CodConvocatoria FROM ConvocatoriaProyecto WHERE CodProyecto = " + CodProyecto;

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            if (dt.Rows.Count > 0)
                CodConvocatoria = dt.Rows[0]["CodConvocatoria"].ToString();

            if (!string.IsNullOrEmpty(CodConvocatoria))
            {
                txtSQL = "select year(fechainicio) from convocatoria where id_Convocatoria=" + CodConvocatoria;

                dt = consultas.ObtenerDataTable(txtSQL, "text");

                if (dt.Rows.Count > 0)
                    anioConvocatoria = dt.Rows[0][0].ToString();
            }
        }

        protected void btn_agregar_Click(object sender, EventArgs e)
        {
            Redirect(null, "CatalogoRiesgoInter.aspx", "_Blank", "width=730,height=585");
        }

        protected void IB_AgregarIndicador_Click(object sender, ImageClickEventArgs e)
        {
            Redirect(null, "CatalogoRiesgoInter.aspx", "_Blank", "width=730,height=585");
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            RestringirLetras(0);
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lnk = e.Row.FindControl("LinkButton1") as LinkButton;
                var img = e.Row.FindControl("LB_eliminar") as Image;

                if (lnk != null && img != null)
                {
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                    {
                        lnk.Style.Add("text-decoration", "none");
                        lnk.ForeColor = System.Drawing.Color.Black;
                        lnk.Enabled = false;
                        img.Visible = false;
                    }
                }

                RestringirLetras(0);
            }

        }

        protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            RestringirLetras(0);
        }

        private void RestringirLetras(int serie)
        {
            try
            {
                GridViewRow filaGrillaInventario = GridView1.Rows[serie];
                TextBox cantidad = (TextBox)filaGrillaInventario.FindControl("TextBox1");
                cantidad.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
                RestringirLetras(serie + 1);
            }
            catch (Exception)
            {

            }
        }

        public DataTable resultado()
        {
            datosEntrada();
            txtSQL = @"SELECT id_Riesgo, Riesgo, Mitigacion, NomejeFuncional, Observacion, CodEjeFuncional " +
                    "FROM InterventorRiesgo, EjeFuncional " +
                    "WHERE id_ejefuncional=codejefuncional and codProyecto = " + CodProyecto +
                    "ORDER BY NomejeFuncional";

            return consultas.ObtenerDataTable(txtSQL, "text");
        }

        public DataTable ejefuncional()
        {
            Session["CodEjeFuncional"] = "1";

            return consultas.ObtenerDataTable("SELECT * FROM EjeFuncional ORDER BY NomEjeFuncional", "text");
        }

        public void eliminar(int id_Riesgo)
        {
            datosEntrada();
            txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;//1

            SqlDataReader reader = ejecutaReader(txtSQL, 1);
            if (reader != null)
            {
                if (reader.Read())
                {
                    int codInter = Convert.ToInt32(reader["CodCoordinador"].ToString());
                    txtSQL = "SELECT * FROM InterventorRiesgo  WHERE Id_riesgo=" + id_Riesgo;

                    reader = ejecutaReader(txtSQL, 1);


                    if (reader != null)
                    {
                        if (reader.Read())
                        {

                            txtSQL = @"Insert into InterventorRiesgoTMP (Id_riesgo,CodProyecto,Riesgo,Mitigacion,CodejeFuncional,Observacion,Tarea) " +
                                    "values (" + id_Riesgo + "," + CodProyecto + ",'" + reader["Riesgo"].ToString() + "','" + reader["Mitigacion"].ToString() + "', " + reader["CodEjeFuncional"].ToString() + ", '" + reader["Observacion"].ToString() + "','Borrar')";

                            AgendarTarea agenda = new AgendarTarea(
                                    codInter, "Borrar", reader["Riesgo"].ToString(), CodProyecto, id_Riesgo, "0", false, 1, false, false, usuario.IdContacto, "Borrar", "", "");
                            agenda.Agendar();

                            ejecutaReader(txtSQL, 2);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.');", true);
                            return;
                        }
                    }
                }
            }
        }

        public void modificar(int id_Riesgo, String Riesgo, String Mitigacion, String Observacion, String CodEjeFuncional)
        {
            datosEntrada();
            txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;

            SqlDataReader reader = ejecutaReader(txtSQL, 1);
            if (reader != null)
            {

                if (reader.Read())
                {
                    int codInter = Convert.ToInt32(reader["CodCoordinador"].ToString());

                    txtSQL = @"Insert into InterventorRiesgoTMP (Id_riesgo,CodProyecto,Riesgo,Mitigacion,CodejeFuncional,Observacion,Tarea) " +
                                    "values (" + id_Riesgo + "," + CodProyecto + ",'" + Riesgo + "','" + Mitigacion + "', " + Session["CodEjeFuncional"].ToString() + ", '" + Observacion + "','Modificar')";

                    AgendarTarea agenda = new AgendarTarea(
                                    codInter, "Modificar", Riesgo, CodProyecto, id_Riesgo, "0", false, 1, false, false, usuario.IdContacto, "Modificar", "", "");
                    agenda.Agendar();

                    ejecutaReader(txtSQL, 2);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.');", true);
                    return;
                }
            }
        }

        private SqlDataReader ejecutaReader(String sql, int obj)
        {
            SqlDataReader reader = null;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                        reader.Close();
                }

                if (conn != null)
                    conn.Close();

                conn.Open();

                if (obj == 1)
                    reader = cmd.ExecuteReader();
                else
                    cmd.ExecuteReader();
            }
            catch (SqlException se)
            {
                if (conn != null)
                    conn.Close();
                return null;
            }

            return reader;
        }

        protected void ddlejefuncional_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow gvAactualizar = GridView1.Rows[indicefila];
            DropDownList ejefuncionalSe = (DropDownList)gvAactualizar.FindControl("ddlejefuncional");

            Session["CodEjeFuncional"] = ejefuncionalSe.SelectedValue;
        }
    }
}