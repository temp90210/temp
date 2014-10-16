using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Convocatoria
{
    public partial class EvaluacionConvocatoria : Negocio.Base_Page
    {
        public String codConvocatoria;

        private DataTable campo;
        private DataTable orden;
        //private DataTable justificacion;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if(!IsPostBack)
            //{
                datos();
                llenadoDinamico();
            //}
        }

        private void datos()
        {
            try
            {
                codConvocatoria = Session["Id_EvalConvocatoria"].ToString();
            }
            catch (Exception) { }
        }

        private void llenadoDinamico()
        {
            datos();
            llenarCampo();

            llenarOrden();

            //llenarjustificacion();

            imprimir();
        }

        private void llenarCampo()
        {
            campo = new DataTable();

            campo.Columns.Add("id_Campo");
            campo.Columns.Add("Campo");
            campo.Columns.Add("Puntaje");
            campo.Columns.Add("indice");

            String sql;
            sql = @"SELECT [id_Campo], [Campo], [Puntaje]
                    FROM [Fonade].[dbo].[Campo] AS C, [Fonade].[dbo].[ConvocatoriaCampo] AS CC
                    WHERE C.[id_Campo] = CC.[codCampo] AND C.[codCampo] IS NULL AND [codConvocatoria] = " + codConvocatoria + " order by id_Campo";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int i = 0;
                while (reader.Read())
                {
                    DataRow item = campo.NewRow();
                    item["id_Campo"] = reader["id_Campo"].ToString();
                    item["Campo"] = reader["Campo"].ToString();
                    item["Puntaje"] = reader["Puntaje"].ToString();
                    item["indice"] = i;
                    campo.Rows.Add(item);
                    i += 1;
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
        }

        private void llenarOrden()
        {
            orden = new DataTable();

            orden.Columns.Add("id_campo");
            orden.Columns.Add("campo");
            orden.Columns.Add("orden");
            orden.Columns.Add("Maximo");
            orden.Columns.Add("indice");
            orden.Columns.Add("indice2");

            for (int i = 0; i < campo.Rows.Count; i++)
            {
                String sql;

                sql = "SELECT C.id_campo, c.campo, case when cc.Puntaje is null then c.campo else p.campo end orden, cc.Puntaje Maximo " +
                        "FROM CAMPO C LEFT JOIN CAMPO P ON c.codcampo=p.id_campo  " +
                        "INNER JOIN ConvocatoriaCampo CC ON C.id_campo = CC.codCampo AND  " +
                        "C.Inactivo = 0 and codconvocatoria=" + codConvocatoria + " AND C.codCampo is NOT null and P.codcampo = " + campo.Rows[i]["id_Campo"].ToString() + " " +
                        "ORDER BY C.id_campo, orden, maximo";

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        DataRow item = orden.NewRow();
                        item["id_campo"] = reader["id_campo"].ToString();
                        item["campo"] = reader["campo"].ToString();
                        item["orden"] = reader["orden"].ToString();
                        item["Maximo"] = reader["Maximo"].ToString();
                        item["indice"] = campo.Rows[i]["indice"].ToString();
                        item["indice2"] = i;
                        orden.Rows.Add(item);
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
            }
        }

        //private void llenarjustificacion()
        //{
        //    justificacion = new DataTable();

        //    justificacion.Columns.Add("puntaje");
        //    justificacion.Columns.Add("indice");
        //    justificacion.Columns.Add("indice2");

        //    for (int i = 0; i < orden.Rows.Count; i++)
        //    {
        //        String sql;

        //        sql = "select puntaje from evaluacioncampo e where  e.codcampo=" + orden.Rows[i]["id_campo"].ToString() + " and e.codconvocatoria=" + codConvocatoria;

        //        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //        SqlCommand cmd = new SqlCommand(sql, conn);

        //        cmd = new SqlCommand(sql, conn);

        //        try
        //        {
        //            conn.Open();
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                DataRow item = justificacion.NewRow();
        //                item["puntaje"] = reader["puntaje"].ToString();
        //                item["indice"] = orden.Rows[i]["indice"].ToString();
        //                item["indice2"] = orden.Rows[i]["indice2"].ToString();
        //                justificacion.Rows.Add(item);
        //            }
        //            reader.Close();
        //        }
        //        catch (SqlException)
        //        {
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

        private void imprimir()
        {
            String txtJustificacion = "";

            for (int i = 0; i < campo.Rows.Count; i++)
            {
                Panel panelPrincipal = new Panel();
                panelPrincipal.ID = "P_" + campo.Rows[i]["Campo"].ToString();

                Label labelCampo = new Label();
                labelCampo.ID = "L_" + campo.Rows[i]["Campo"].ToString();
                labelCampo.Text = campo.Rows[i]["Campo"].ToString();

                labelCampo.Width = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2) - 150;

                labelCampo.CssClass = "fondo";
                labelCampo.Font.Bold = true;

                panelPrincipal.Controls.Add(labelCampo);

                TextBox textbospuntajeMax = new TextBox();

                textbospuntajeMax.ID = "L_" + campo.Rows[i]["Campo"].ToString() + "Max";
                textbospuntajeMax.Text = campo.Rows[i]["Puntaje"].ToString();
                textbospuntajeMax.CssClass = campo.Rows[i]["id_campo"].ToString();
                textbospuntajeMax.CssClass = "fondo";
                textbospuntajeMax.Font.Bold = true;
                textbospuntajeMax.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");

                textbospuntajeMax.Width = 100;

                panelPrincipal.Controls.Add(textbospuntajeMax);

                String txtVariable = "";
                for (int j = 0; j < orden.Rows.Count; j++)
                {
                    if (campo.Rows[i]["indice"].ToString().Equals(orden.Rows[j]["indice"].ToString()))
                    {
                        if (!txtVariable.Equals(orden.Rows[j]["orden"].ToString()))
                        {
                            txtVariable = orden.Rows[j]["orden"].ToString();

                            Label labelOrden = new Label();

                            labelOrden.ID = "L_" + orden.Rows[j]["orden"].ToString();
                            labelOrden.Text = orden.Rows[j]["orden"].ToString();

                            labelOrden.Width = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2);
                            labelOrden.CssClass = "clasemas";
                            labelOrden.Font.Bold = true;

                            panelPrincipal.Controls.Add(labelOrden);
                        }
                        txtJustificacion = orden.Rows[j]["Campo"].ToString();

                        TextBox textboxJustificacion = new TextBox();

                        textboxJustificacion.ID = "L_" + orden.Rows[j]["Campo"].ToString() + "" + j;
                        textboxJustificacion.Text = orden.Rows[j]["Campo"].ToString();
                        
                        textboxJustificacion.BackColor = System.Drawing.Color.White;
                        textboxJustificacion.Width = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2) - 150;
                        textboxJustificacion.Height = 50;
                        textboxJustificacion.ForeColor = System.Drawing.Color.Black;
                        textboxJustificacion.TextMode = TextBoxMode.MultiLine;
                        textboxJustificacion.Enabled = false;

                        panelPrincipal.Controls.Add(textboxJustificacion);

                        TextBox textbospuntaje = new TextBox();

                        textbospuntaje.ID = "L_" + orden.Rows[j]["Campo"].ToString() + "Pun" + j;

                        string txtSQL = "select puntaje from evaluacioncampo e where  e.codcampo=" + orden.Rows[j]["id_campo"].ToString() + " and e.codconvocatoria=" + codConvocatoria;

                        SqlDataReader reader = ejecutaReader(txtSQL, 1);

                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                if (!string.IsNullOrEmpty(reader["puntaje"].ToString()))
                                {
                                    try
                                    {
                                        if (Convert.ToInt32(reader["puntaje"].ToString()) > 0)
                                        {
                                            textbospuntaje.Enabled = false;
                                        }
                                    }
                                    catch (FormatException) { }
                                }
                            }
                        }

                        textbospuntaje.Text = orden.Rows[j]["Maximo"].ToString();
                        textbospuntaje.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
                        textbospuntaje.CssClass = orden.Rows[j]["id_campo"].ToString();

                        textbospuntaje.Width = 100;
                        textbospuntaje.Height = 10;

                        panelPrincipal.Controls.Add(textbospuntaje);
                    }
                }

                TableRow fila = new TableRow();
                T_Observaciones.Rows.Add(fila);

                TableCell celda = new TableCell();
                celda.Controls.Add(panelPrincipal);

                fila.Cells.Add(celda);
            }
        }

        protected void btnActualizat_Click(object sender, EventArgs e)
        {
            foreach (TableRow tr in T_Observaciones.Rows)
            {
                foreach (TableCell tc in tr.Cells)
                {
                    foreach (Control control in tc.Controls)
                    {
                        if (control is Panel)
                        {
                            foreach (Control ctl in ((Panel)control).Controls)
                            {
                                if (ctl is TextBox)
                                {
                                    try
                                    {
                                        TextBox textbospuntajeMax = (TextBox)ctl;
                                        ejecutaActualiza(textbospuntajeMax.Text, textbospuntajeMax.CssClass);
                                    }
                                    catch (NullReferenceException) { }
                                }
                            }
                        }
                    }
                }
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Evaluación actualizada')", true);
        }

        private void ejecutaActualiza(string objeto, string id)
        {
            string txtSQL;

            txtSQL = "UPDATE ConvocatoriaCampo " +
               "SET Puntaje = " + objeto +
               " WHERE codCampo = " + id + " AND  codConvocatoria= " + codConvocatoria;

            ejecutaReader(txtSQL, 2);
        }

        public SqlDataReader ejecutaReader(String sql, int obj)
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

        protected void lnkadicionar_Click(object sender, EventArgs e)
        {
            Session["Id_EvalConvocatoriaAS"] = codConvocatoria;

            Redirect(null, "FrameAspectos.aspx", "_Blank", "width=990,height=700");
        }
    }
}