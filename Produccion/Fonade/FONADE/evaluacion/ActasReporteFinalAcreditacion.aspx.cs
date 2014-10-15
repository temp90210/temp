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
    public partial class ActasReporteFinalAcreditacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }
        public DataTable resultadoData()
        {
            DataTable datatable = new DataTable();

            datatable.Columns.Add("CODIGO");
            datatable.Columns.Add("NOMACTA");
            datatable.Columns.Add("NOMCONVOCATORIA");
            datatable.Columns.Add("FECHACREACION");
            datatable.Columns.Add("FECHATRANSMISION");
            datatable.Columns.Add("ID_CONVOCATORIA");
            
            String sql = "SELECT DISTINCT A.ID_ActaAcreditacionFinal 'CODIGO', A.NOMActaAcreditacionFinal 'NOMACTA', C.NOMCONVOCATORIA,A.FECHACREACION,A.FECHATRANSMISION,C.ID_CONVOCATORIA  FROM ActaAcreditacionFinal  A JOIN CONVOCATORIA C ON (C.ID_CONVOCATORIA= A.CODCONVOCATORIA)";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["CODIGO"] = reader["CODIGO"].ToString();
                    fila["NOMACTA"] = reader["NOMACTA"].ToString();
                    fila["NOMCONVOCATORIA"] = reader["NOMCONVOCATORIA"].ToString();
                    fila["FECHACREACION"] = reader["FECHACREACION"].ToString();
                    fila["FECHATRANSMISION"] = reader["FECHATRANSMISION"].ToString();
                    fila["ID_CONVOCATORIA"] = reader["ID_CONVOCATORIA"].ToString();
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
        public DataTable Convocatoria()
        {
            DataTable datatable = new DataTable();

            datatable.Columns.Add("Id_Convocatoria");
            datatable.Columns.Add("NomConvocatoria");

            String sql;
            sql = "SELECT DISTINCT C.ID_CONVOCATORIA, C.NomConvocatoria FROM CONVOCATORIA C JOIN CONVOCATORIAPROYECTO CP ON (CP.CODCONVOCATORIA= C.ID_CONVOCATORIA) JOIN PROYECTO P ON (CP.CODPROYECTO = P.ID_PROYECTO /*AND P.CODESTADO IN (3,10,11,12)*/ ) AND C.ID_CONVOCATORIA NOT IN(SELECT CODCONVOCATORIA FROM ActaAcreditacionFinal) ";
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
        protected void B_Crear_Click(object sender, EventArgs e)
        {
            String nombreActa = TB_NombreActa.Text;
            String idConvocatoria = DDL_Convocatoria.SelectedValue;

            String sql = "IF NOT EXISTS(SELECT CODCONVOCATORIA FROM ActaAcreditacionFinal WHERE CODCONVOCATORIA=" + idConvocatoria + ") INSERT INTO ActaAcreditacionFinal VALUES('" + nombreActa + "'," + idConvocatoria + ",GETDATE(),NULL)";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);
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

            divCrearActa.Visible = false;
            divCrearActa.Enabled = false;

            B_CrearApta.Visible = true;
            B_CrearApta.Enabled = true;
            GV_Actas.Visible = true;
            GV_Actas.Enabled = true;

            GV_Actas.DataBind();
            TB_NombreActa.Text = "";
        }
        protected void B_Ocultar_Click(object sender, EventArgs e)
        {
            divCrearActa.Visible = false;
            divCrearActa.Enabled = false;

            B_CrearApta.Visible = true;
            B_CrearApta.Enabled = true;
            GV_Actas.Visible = true;
            GV_Actas.Enabled = true;
        }
        protected void B_CrearApta_Click(object sender, EventArgs e)
        {
            divCrearActa.Visible = true;
            divCrearActa.Enabled = true;

            B_CrearApta.Visible = false;
            B_CrearApta.Enabled = false;
            GV_Actas.Visible = false;
            GV_Actas.Enabled = false;
        }
        protected void LB_Exportar_Click(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow filaGrilla = GV_Actas.Rows[indicefila];

            Int64 idConvocatoria = Int64.Parse(GV_Actas.DataKeys[filaGrilla.RowIndex].Value.ToString());

            ExporTabla tablaExpor = new ExporTabla("" + idConvocatoria);

            tablaExpor.llenarData();
            tablaExpor.crearTabla();

            Table TablaAExcel = new Table();

            tablaExpor.BorrarControl();

            TablaAExcel = tablaExpor._TablaAExcel1;

            exportar(TablaAExcel, "" + idConvocatoria);
        }
        private void exportar(Table TablaExportar, String idCOnvocatoria)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "inline;filename=ReporteFinalConvocatoria" + idCOnvocatoria + ".xls");
            Response.Charset = "";
            EnableViewState = false;
            var oStringWriter = new System.IO.StringWriter();
            var oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            TablaExportar.RenderControl(oHtmlTextWriter);
            Response.Write(oStringWriter.ToString());
            Response.End();
        }
        protected void RPF_ID_Click(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow filaGrilla = GV_Actas.Rows[indicefila];

            Int64 idConvocatoria = Int64.Parse(GV_Actas.DataKeys[filaGrilla.RowIndex].Value.ToString());

            Session["EvalidConvocatoria"] = idConvocatoria;

            Response.Redirect("ReporteFinalAcreditacion.aspx");
        }
    }
}