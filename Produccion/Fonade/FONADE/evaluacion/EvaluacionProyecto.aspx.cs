using Datos;
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
    public partial class EvaluacionProyecto : Negocio.Base_Page
    {
        #region Variables globales.

        public String codProyecto;
        public int txtTab = Constantes.ConstSubEvaluacionProyecto;
        public String codConvocatoria;
        private ProyectoMercadoProyeccionVenta pm;
        String selectIndex;
        DataTable TipoSupuesto;
        DataTable EvaluacionProyectoSupuesto;
        DataTable EvaluacionIndicadorFinancieroProyecto;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si "está" o "no" realizado...
        /// </summary>
        public Boolean bRealizado;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                codProyecto = Session["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
                codConvocatoria = Session["codConvocatoria"].ToString();

                codProyecto = Request.QueryString["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
            }
            catch (Exception) { }

            inicioEncabezado(codProyecto, codConvocatoria, Constantes.ConstSubFlujoCaja);

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codProyecto, "");

            if (esMiembro && !bRealizado)
                if (!(esMiembro && !bRealizado))
                { this.div_Post_It1.Visible = true; }

            if (!(esMiembro && usuario.CodGrupo == Constantes.CONST_Evaluador) || bRealizado)
            { DD_TiempoProyeccion.Enabled = false; }

            if (esMiembro && usuario.CodGrupo == Constantes.CONST_Evaluador && !bRealizado)
            { B_Guardar.Visible = true; }

            if (esMiembro && !bRealizado && usuario.CodGrupo == Constantes.CONST_Evaluador)
            { B_ActualizarSupuesto.Visible = true; B_ActualizarIndicador.Visible = true; }

            if (!IsPostBack)
            {
                cargarDatos();
                ObtenerDatosUltimaActualizacion();
            }
            //Como son controles dinámicos, se deben llamar en cada POSTBACK!
            cargarSupuestos();
            cargarIndicador();
        }

        protected void B_Guardar_Click(object sender, EventArgs e)
        {
            //if (selectIndex != DD_TiempoProyeccion.SelectedValue)
            //{
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            String sql;


            try
            {
                sql = "DELETE FROM [Fonade].[dbo].[EvaluacionProyectoSupuestoValor] WHERE [CodSupuesto] IN (SELECT [Id_EvaluacionProyectoSupuesto] FROM [Fonade].[dbo].[EvaluacionProyectoSupuesto] WHERE [CodProyecto] = " + codProyecto + " AND[CodConvocatoria] = " + codConvocatoria + ")";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();

                sql = "DELETE FROM [Fonade].[dbo].[EvaluacionIndicadorFinancieroValor] WHERE [CodEvaluacionIndicadorFinancieroProyecto] IN (SELECT [Id_EvaluacionIndicadorFinancieroProyecto] FROM [Fonade].[dbo].[EvaluacionIndicadorFinancieroProyecto] WHERE [CodProyecto] = " + codProyecto + " AND[CodConvocatoria] = " + codConvocatoria + ")";
                cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();

                sql = "DELETE FROM [Fonade].[dbo].[EvaluacionRubroValor] WHERE [CodEvaluacionRubroProyecto] IN (SELECT [Id_EvaluacionRubroProyecto] FROM [Fonade].[dbo].[EvaluacionRubroProyecto] WHERE [CodProyecto] = " + codProyecto + " AND[CodConvocatoria] = " + codConvocatoria + ")";
                cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();

                sql = "SELECT COUNT(*) AS TOTAL FROM [Fonade].[dbo].[EvaluacionObservacion] WHERE [CodProyecto] = " + codProyecto + " AND[CodConvocatoria] = " + codConvocatoria;
                cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();

                if (Int32.Parse(reader["TOTAL"].ToString()) > 0)
                {
                    sql = "UPDATE [Fonade].[dbo].[EvaluacionObservacion] SET [TiempoProyeccion] = " + DD_TiempoProyeccion.SelectedValue + " WHERE [CodProyecto] = " + codProyecto + " AND [CodConvocatoria] = " + codConvocatoria;
                }
                else
                {
                    sql = "INSERT INTO [Fonade].[dbo].[EvaluacionObservacion] ([CodProyecto], [CodConvocatoria], [TiempoProyeccion]) VALUES (" + codProyecto + "," + codConvocatoria + "," + DD_TiempoProyeccion.SelectedValue + ")";
                }

                conn.Close();
                cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.ExecuteReader();

            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }
            //}
            Response.Redirect("EvaluacionProyecto.aspx");
        }

        private void cargarDatos()
        {
            String sql;
            sql = "SELECT [TiempoProyeccion] FROM [Fonade].[dbo].[EvaluacionObservacion] WHERE [CodProyecto] = " + codProyecto + " AND [CodConvocatoria] = " + codConvocatoria;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    if (!String.IsNullOrEmpty(reader["TiempoProyeccion"].ToString()))
                    {
                        if (!IsPostBack)
                            DD_TiempoProyeccion.SelectedIndex = (Int32.Parse(reader["TiempoProyeccion"].ToString()) - 3);
                        selectIndex = reader["TiempoProyeccion"].ToString();
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

        private void cargarSupuestos()
        {
            selectIndex = DD_TiempoProyeccion.SelectedValue;
            llenarTipoSupuesto();
            llenarEvaluacionProyectoSupuesto();

            if (String.IsNullOrEmpty(selectIndex)) selectIndex = "3";

            for (int i = 0; i < TipoSupuesto.Rows.Count; i++)
            {
                Panel panelPrincipal = new Panel();
                panelPrincipal.ID = "P_" + TipoSupuesto.Rows[i]["NomTipoSupuesto"].ToString();

                Label labelCampo = new Label();
                labelCampo.ID = "L_Supuesto" + TipoSupuesto.Rows[i]["Id_TipoSupuesto"].ToString();
                labelCampo.Text = "Supuestos " + TipoSupuesto.Rows[i]["NomTipoSupuesto"].ToString();
                labelCampo.Width = 31 * Int32.Parse(selectIndex) + 251;
                labelCampo.Font.Bold = true;

                panelPrincipal.Controls.Add(labelCampo);

                Table tablatitullo = new Table();
                tablatitullo.Width = 31 * Int32.Parse(selectIndex) + 251;
                tablatitullo.CssClass = "Grilla";
                TableHeaderRow cabezafila = new TableHeaderRow();

                for (int j = 0; j <= Int32.Parse(selectIndex); j++)
                {
                    TableHeaderCell celda = new TableHeaderCell();
                    Label titulo = new Label();
                    titulo.ID = "L_tituloSupuestos" + TipoSupuesto.Rows[i]["Id_TipoSupuesto"].ToString() + j + 1;

                    if (j == 0)
                    {
                        titulo.Text = "Variable / Periodo";
                        titulo.Width = 250;
                    }
                    else
                    {
                        titulo.Text = "" + j;
                        titulo.Width = 78;
                        titulo.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                    }
                    celda.Controls.Add(titulo);
                    cabezafila.Cells.Add(celda);
                }
                tablatitullo.Rows.Add(cabezafila);

                TableRow fila = new TableRow();
                for (int j = 0; j < EvaluacionProyectoSupuesto.Rows.Count; j++)
                {
                    if (TipoSupuesto.Rows[i]["Id_TipoSupuesto"].ToString().Equals(EvaluacionProyectoSupuesto.Rows[j]["CodTipoSupuesto"].ToString()))
                    {
                        fila = new TableRow();
                        for (int k = 0; k <= Int32.Parse(selectIndex); k++)
                        {
                            TableCell celda = new TableCell();
                            if (k == 0)
                            {
                                Label titulo = new Label();
                                titulo.ID = "L_EvaluacionProyectoSupuesto" + EvaluacionProyectoSupuesto.Rows[j]["Id_EvaluacionProyectoSupuesto"].ToString() + k;
                                titulo.Text = EvaluacionProyectoSupuesto.Rows[j]["NomEvaluacionProyectoSupuesto"].ToString();
                                titulo.Width = 250;
                                celda.Controls.Add(titulo);
                            }
                            else
                            {
                                TextBox textbox = new TextBox();
                                if (esMiembro && !bRealizado && usuario.CodGrupo == Constantes.CONST_Evaluador)
                                { textbox.Enabled = true; }
                                else { textbox.Enabled = false; }
                                textbox.ID = "TB_EvaluacionProyectoSupuesto" + EvaluacionProyectoSupuesto.Rows[j]["Id_EvaluacionProyectoSupuesto"].ToString() + k;

                                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                                SqlCommand cmd = new SqlCommand("SELECT [Valor] FROM [Fonade].[dbo].[EvaluacionProyectoSupuestoValor] WHERE CodSupuesto = " + EvaluacionProyectoSupuesto.Rows[j]["Id_EvaluacionProyectoSupuesto"].ToString() + " AND Periodo = " + k, conn);

                                try
                                {
                                    conn.Open();
                                    SqlDataReader reader = cmd.ExecuteReader();

                                    textbox.Text = "";
                                    if (reader.Read())
                                        textbox.Text = reader["Valor"].ToString();
                                    else
                                        textbox.Text = "0";

                                    reader.Close();
                                }
                                catch (SqlException)
                                {
                                    textbox.Text = "0";
                                }
                                finally
                                {
                                    conn.Close();
                                }

                                textbox.Width = 30;
                                celda.Controls.Add(textbox);
                            }
                            fila.Cells.Add(celda);
                        }
                        tablatitullo.Rows.Add(fila);
                    }
                }
                panelPrincipal.Controls.Add(tablatitullo);

                TableRow filatablaprincipal = new TableRow();
                T_Supuestos.Rows.Add(filatablaprincipal);

                TableCell celdatablaprincipal = new TableCell();
                celdatablaprincipal.Controls.Add(panelPrincipal);

                filatablaprincipal.Cells.Add(celdatablaprincipal);
            }
        }

        private void cargarIndicador()
        {
            selectIndex = DD_TiempoProyeccion.SelectedValue;

            llenarEvaluacionIndicadorFinancieroProyecto();

            if (String.IsNullOrEmpty(selectIndex)) selectIndex = "3";

            Panel panelPrincipal = new Panel();
            panelPrincipal.ID = "P_NomIndicadorFinanciero";

            Table tablatitullo = new Table();
            tablatitullo.Width = 31 * Int32.Parse(selectIndex) + 251 + 126;
            tablatitullo.CssClass = "Grilla";
            TableHeaderRow cabezafila = new TableHeaderRow();

            for (int j = 0; j <= Int32.Parse(selectIndex) + 1; j++)
            {
                TableHeaderCell celda = new TableHeaderCell();
                Label titulo = new Label();
                titulo.ID = "L_tituloIndicador" + j;

                if (j == 0)
                {
                    titulo.Text = "Variable / Periodo";
                    titulo.Width = 250;
                }
                else
                {
                    if (j == (Int32.Parse(selectIndex) + 1))
                    {
                        titulo.Text = "Promedio Sector";
                        titulo.Width = 125;
                    }
                    else
                    {
                        titulo.Text = "" + j;
                        titulo.Width = 78;
                        titulo.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                    }
                }
                celda.Controls.Add(titulo);
                cabezafila.Cells.Add(celda);
            }
            tablatitullo.Rows.Add(cabezafila);

            TableRow fila = new TableRow();
            for (int i = 0; i < EvaluacionIndicadorFinancieroProyecto.Rows.Count; i++)
            {
                fila = new TableRow();
                for (int k = 0; k <= Int32.Parse(selectIndex) + 1; k++)
                {
                    TableCell celda = new TableCell();
                    if (k == 0)
                    {
                        Label titulo = new Label();
                        titulo.ID = "L_EvaluacionIndicadorFinancieroProyecto" + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + k;
                        titulo.Text = EvaluacionIndicadorFinancieroProyecto.Rows[i]["Descripcion"].ToString();
                        titulo.Width = 250;
                        celda.Controls.Add(titulo);
                    }
                    else
                    {
                        TextBox textbox = new TextBox();
                        if (esMiembro && !bRealizado && usuario.CodGrupo == Constantes.CONST_Evaluador)
                        { textbox.Enabled = true; }
                        else { textbox.Enabled = false; }
                        textbox.ID = "TB_EvaluacionIndicadorFinancieroProyecto" + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + k;

                        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                        SqlCommand cmd;

                        if (k == (Int32.Parse(selectIndex) + 1))
                        {
                            cmd = new SqlCommand("SELECT [Valor] FROM [Fonade].[dbo].[EvaluacionIndicadorFinancieroValor]  WHERE [CodEvaluacionIndicadorFinancieroProyecto] = " + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + " AND Periodo = 0", conn);
                        }
                        else
                        {
                            cmd = new SqlCommand("SELECT [Valor] FROM [Fonade].[dbo].[EvaluacionIndicadorFinancieroValor]  WHERE [CodEvaluacionIndicadorFinancieroProyecto] = " + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + " AND Periodo = " + k, conn);
                        }

                        try
                        {
                            conn.Open();
                            SqlDataReader reader = cmd.ExecuteReader();

                            textbox.Text = "";
                            if (reader.Read())
                                textbox.Text = reader["Valor"].ToString();
                            else
                                textbox.Text = "0";

                            reader.Close();
                        }
                        catch (SqlException)
                        {
                            textbox.Text = "0";
                        }
                        finally
                        {
                            conn.Close();
                        }

                        textbox.Width = 30;
                        celda.Controls.Add(textbox);
                    }
                    fila.Cells.Add(celda);
                }
                tablatitullo.Rows.Add(fila);
            }

            panelPrincipal.Controls.Add(tablatitullo);

            TableRow filatablaprincipal = new TableRow();
            T_Indicadores.Rows.Add(filatablaprincipal);

            TableCell celdatablaprincipal = new TableCell();
            celdatablaprincipal.Controls.Add(panelPrincipal);

            filatablaprincipal.Cells.Add(celdatablaprincipal);
        }

        private void llenarTipoSupuesto()
        {
            TipoSupuesto = new DataTable();

            TipoSupuesto.Columns.Add("Id_TipoSupuesto");
            TipoSupuesto.Columns.Add("NomTipoSupuesto");

            String sql;
            sql = "SELECT * FROM [Fonade].[dbo].[TipoSupuesto]";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = TipoSupuesto.NewRow();
                    fila["Id_TipoSupuesto"] = reader["Id_TipoSupuesto"].ToString();
                    fila["NomTipoSupuesto"] = reader["NomTipoSupuesto"].ToString();
                    TipoSupuesto.Rows.Add(fila);
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

        private void llenarEvaluacionProyectoSupuesto()
        {
            EvaluacionProyectoSupuesto = new DataTable();

            EvaluacionProyectoSupuesto.Columns.Add("Id_EvaluacionProyectoSupuesto");
            EvaluacionProyectoSupuesto.Columns.Add("NomEvaluacionProyectoSupuesto");
            EvaluacionProyectoSupuesto.Columns.Add("CodTipoSupuesto");
            EvaluacionProyectoSupuesto.Columns.Add("CodProyecto");
            EvaluacionProyectoSupuesto.Columns.Add("CodConvocatoria");

            String sql;
            sql = "SELECT * FROM [Fonade].[dbo].[EvaluacionProyectoSupuesto] WHERE [CodProyecto] = " + codProyecto + " AND [CodConvocatoria] = " + codConvocatoria + " ORDER BY [CodTipoSupuesto]";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = EvaluacionProyectoSupuesto.NewRow();
                    fila["Id_EvaluacionProyectoSupuesto"] = reader["Id_EvaluacionProyectoSupuesto"].ToString();
                    fila["NomEvaluacionProyectoSupuesto"] = reader["NomEvaluacionProyectoSupuesto"].ToString();
                    fila["CodTipoSupuesto"] = reader["CodTipoSupuesto"].ToString();
                    fila["CodProyecto"] = reader["CodProyecto"].ToString();
                    fila["CodConvocatoria"] = reader["CodConvocatoria"].ToString();
                    EvaluacionProyectoSupuesto.Rows.Add(fila);
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

        private void llenarEvaluacionIndicadorFinancieroProyecto()
        {
            EvaluacionIndicadorFinancieroProyecto = new DataTable();

            EvaluacionIndicadorFinancieroProyecto.Columns.Add("Id_EvaluacionIndicadorFinancieroProyecto");
            EvaluacionIndicadorFinancieroProyecto.Columns.Add("Descripcion");
            EvaluacionIndicadorFinancieroProyecto.Columns.Add("CodProyecto");
            EvaluacionIndicadorFinancieroProyecto.Columns.Add("CodConvocatoria");

            String sql;
            sql = "SELECT * FROM [Fonade].[dbo].[EvaluacionIndicadorFinancieroProyecto] WHERE codProyecto = " + codProyecto + " AND codConvocatoria = " + codConvocatoria;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = EvaluacionIndicadorFinancieroProyecto.NewRow();
                    fila["Id_EvaluacionIndicadorFinancieroProyecto"] = reader["Id_EvaluacionIndicadorFinancieroProyecto"].ToString();
                    fila["Descripcion"] = reader["Descripcion"].ToString();
                    fila["CodProyecto"] = reader["CodProyecto"].ToString();
                    fila["CodConvocatoria"] = reader["CodConvocatoria"].ToString();
                    EvaluacionIndicadorFinancieroProyecto.Rows.Add(fila);
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

        protected void B_ActualizarSupuesto_Click(object sender, EventArgs e)
        {
            selectIndex = DD_TiempoProyeccion.SelectedValue;
            llenarTipoSupuesto();
            llenarEvaluacionProyectoSupuesto();

            for (int i = 0; i < TipoSupuesto.Rows.Count; i++)
            {
                for (int j = 0; j < EvaluacionProyectoSupuesto.Rows.Count; j++)
                {
                    if (TipoSupuesto.Rows[i]["Id_TipoSupuesto"].ToString().Equals(EvaluacionProyectoSupuesto.Rows[j]["CodTipoSupuesto"].ToString()))
                    {
                        for (int k = 1; k <= Int32.Parse(selectIndex); k++)
                        {
                            String objetoTextBox;
                            objetoTextBox = "TB_EvaluacionProyectoSupuesto" + EvaluacionProyectoSupuesto.Rows[j]["Id_EvaluacionProyectoSupuesto"].ToString() + k;

                            TextBox controlSupuesto = (TextBox)this.FindControl(objetoTextBox);

                            try
                            {
                                decimal numero = decimal.Parse(controlSupuesto.Text);
                            }
                            catch (Exception ex)
                            {
                                if (ex is FormatException)
                                {
                                    ClientScriptManager cm = this.ClientScript;
                                    cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Formato numérico no válido ( " + controlSupuesto.Text + ")');</script>");
                                    return;
                                }
                                else
                                {
                                    ClientScriptManager cm = this.ClientScript;
                                    cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Error desconocido.');</script>");
                                    return;
                                }
                            }

                            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                            SqlCommand cmd = new SqlCommand("SELECT [Valor] FROM [Fonade].[dbo].[EvaluacionProyectoSupuestoValor] WHERE CodSupuesto = " + EvaluacionProyectoSupuesto.Rows[j]["Id_EvaluacionProyectoSupuesto"].ToString() + " AND Periodo = " + k, conn);

                            try
                            {
                                String sql;
                                conn.Open();
                                SqlDataReader reader = cmd.ExecuteReader();

                                if (reader.Read())
                                    sql = "UPDATE [Fonade].[dbo].[EvaluacionProyectoSupuestoValor] SET [Valor] = " + controlSupuesto.Text + " WHERE [CodSupuesto] = " + EvaluacionProyectoSupuesto.Rows[j]["Id_EvaluacionProyectoSupuesto"].ToString() + " AND [Periodo] = " + k;
                                else
                                    sql = "INSERT INTO [Fonade].[dbo].[EvaluacionProyectoSupuestoValor] ([CodSupuesto], [Periodo], [Valor]) VALUES (" + EvaluacionProyectoSupuesto.Rows[j]["Id_EvaluacionProyectoSupuesto"].ToString() + ", " + k + ", " + controlSupuesto.Text + ")";

                                reader.Close();
                                conn.Close();

                                #region COMENTARIOS NO BORRAR!.
                                //cmd = new SqlCommand(sql, conn);
                                //conn.Open();
                                //cmd.ExecuteReader(); 
                                #endregion

                                //Ejecutar consulta SQL.
                                ejecutaReader(sql, 2);
                            }
                            catch (SqlException) { }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                }
            }

            prActualizarTabEval(txtTab.ToString(), codProyecto, codConvocatoria);
            ObtenerDatosUltimaActualizacion();
        }

        protected void B_ActualizarIndicador_Click(object sender, EventArgs e)
        {
            selectIndex = DD_TiempoProyeccion.SelectedValue;
            llenarEvaluacionIndicadorFinancieroProyecto();

            for (int i = 0; i < EvaluacionIndicadorFinancieroProyecto.Rows.Count; i++)
            {
                for (int k = 1; k <= Int32.Parse(selectIndex) + 1; k++)
                {
                    String objetoTextBox;
                    objetoTextBox = "TB_EvaluacionIndicadorFinancieroProyecto" + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + k;

                    TextBox controlSupuesto = (TextBox)this.FindControl(objetoTextBox);

                    try
                    {
                        decimal numero = decimal.Parse(controlSupuesto.Text);
                    }
                    catch (Exception ex)
                    {
                        if (ex is FormatException)
                        {
                            ClientScriptManager cm = this.ClientScript;
                            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Formato numérico no válido ( " + controlSupuesto.Text + ")');</script>");
                            return;
                        }
                        else
                        {
                            ClientScriptManager cm = this.ClientScript;
                            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Error desconocido.');</script>");
                            return;
                        }
                    }

                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                    SqlCommand cmd;

                    int preiodoValor;
                    if (k == (Int32.Parse(selectIndex) + 1))
                    {
                        cmd = new SqlCommand("SELECT [Valor] FROM [Fonade].[dbo].[EvaluacionIndicadorFinancieroValor]  WHERE [CodEvaluacionIndicadorFinancieroProyecto] = " + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + " AND Periodo = 0", conn);
                        preiodoValor = 0;
                    }
                    else
                    {
                        cmd = new SqlCommand("SELECT [Valor] FROM [Fonade].[dbo].[EvaluacionIndicadorFinancieroValor]  WHERE [CodEvaluacionIndicadorFinancieroProyecto] = " + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + " AND Periodo = " + k, conn);
                        preiodoValor = k;
                    }

                    try
                    {
                        String sql;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                            sql = "UPDATE [Fonade].[dbo].[EvaluacionIndicadorFinancieroValor] SET [Valor] = " + controlSupuesto.Text + " WHERE [CodEvaluacionIndicadorFinancieroProyecto] = " + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + " AND [Periodo] = " + preiodoValor;
                        else
                            sql = "INSERT INTO [Fonade].[dbo].[EvaluacionIndicadorFinancieroValor] ([CodEvaluacionIndicadorFinancieroProyecto], [Periodo], [Valor]) VALUES (" + EvaluacionIndicadorFinancieroProyecto.Rows[i]["Id_EvaluacionIndicadorFinancieroProyecto"].ToString() + ", " + preiodoValor + ", " + controlSupuesto.Text + ")";

                        reader.Close();
                        conn.Close();

                        #region COMENTARIOS NO BORRAR!.
                        //cmd = new SqlCommand(sql, conn);
                        //conn.Open();
                        //cmd.ExecuteReader(); 
                        #endregion

                        //Ejecutar consulta SQL.
                        ejecutaReader(sql, 2);
                    }
                    catch (SqlException) { }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            prActualizarTabEval(txtTab.ToString(), codProyecto, codConvocatoria);
            ObtenerDatosUltimaActualizacion();
        }

        #region Métodos de Mauricio Arias Olave.

        /// <summary>
        /// Establecer el primer valor en mayúscula, retornando un string con la primera en maýsucula.
        /// </summary>
        /// <param name="s">String a procesar</param>
        /// <returns>String procesado.</returns>
        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 06/06/2014.
        /// Obtener la información acerca de la última actualización realizada, ási como la habilitación del 
        /// CheckBox para el usuario dependiendo de su grupo / rol.
        /// </summary>
        private void ObtenerDatosUltimaActualizacion()
        {
            //Inicializar variables.
            String txtSQL;
            DateTime fecha = new DateTime();
            DataTable tabla = new DataTable();
            bool bNuevo = true; //Indica si las aprobaciones de las pestañas pueden ser levantadas por el evaluador.
            bool bRealizado = false;
            bool bEnActa = false; //Determinar si el proyecto esta incluido en un acta de comite evaluador.
            bool EsMiembro = false;
            Int32 CodigoEstado = 0;

            try
            {
                //Consultar si es "Nuevo".
                bNuevo = es_bNuevo(codProyecto.ToString());

                //Determinar si "está en acta".
                bEnActa = es_EnActa(codProyecto.ToString(), codConvocatoria.ToString());

                //Consultar si es "Miembro".
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto.ToString());

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(Constantes.ConstSubEvaluacionProyecto.ToString(), codProyecto, codConvocatoria);

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + codProyecto + " And CodContacto = " + usuario.IdContacto +
                         " and inactivo=0 and FechaInicio<=getdate() and FechaFin is null ";

                //Asignar variables a DataTable.
                var rs = consultas.ObtenerDataTable(txtSQL, "text");

                if (rs.Rows.Count > 0)
                {
                    //Crear la variable de sesión.
                    Session["CodRol"] = rs.Rows[0]["CodRol"].ToString();
                }

                //Destruir la variable.
                rs = null;

                #endregion

                //Consultar los datos a mostrar en los campos correspondientes a la actualización.
                txtSQL = " select nombres+' '+apellidos as nombre, fechamodificacion, realizado  " +
                         " from tabEvaluacionproyecto, contacto " +
                         " where id_contacto = codcontacto and codtabEvaluacion = " + Constantes.ConstSubEvaluacionProyecto +
                         " and codproyecto = " + codProyecto +
                         " and codconvocatoria = " + codConvocatoria;

                //Asignar resultados de la consulta a variable DataTable.
                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                //Si tiene datos "y debe tenerlos" ejecuta el siguiente código.
                if (tabla.Rows.Count > 0)
                {
                    //Nombre del usuario quien hizo la actualización.
                    lbl_nombre_user_ult_act.Text = tabla.Rows[0]["nombre"].ToString().ToUpperInvariant();

                    #region Formatear la fecha.

                    //Convertir fecha.
                    try { fecha = Convert.ToDateTime(tabla.Rows[0]["FechaModificacion"].ToString()); }
                    catch { fecha = DateTime.Today; }

                    //Obtener el nombre del mes (las primeras tres letras).
                    string sMes = fecha.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                    //Obtener la hora en minúscula.
                    string hora = fecha.ToString("hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant();

                    //Reemplazar el valor "am" o "pm" por "a.m" o "p.m" respectivamente.
                    if (hora.Contains("am")) { hora = hora.Replace("am", "a.m"); } if (hora.Contains("pm")) { hora = hora.Replace("pm", "p.m"); }

                    //Formatear la fecha según manejo de FONADE clásico. "Ej: Nov 19 de 2013 07:36:26 p.m.".
                    lbl_fecha_formateada.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year + " " + hora + ".";

                    #endregion

                    //Valor "bRealizado".
                    bRealizado = Convert.ToBoolean(tabla.Rows[0]["Realizado"].ToString());
                }

                //Asignar check de acuerdo al valor obtenido en "bRealizado".
                chk_realizado.Checked = bRealizado;

                //Evaluar "habilitación" del CheckBox.
                if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                { chk_realizado.Enabled = false; }

                if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString() && lbl_nombre_user_ult_act.Text.Trim() != "" && CodigoEstado == Constantes.CONST_Evaluacion && (!bEnActa))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                //Destruir variables.
                tabla = null;
                txtSQL = null;
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
                //Destruir variables.
                tabla = null;
                txtSQL = null;
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 06/06/2014.
        /// Obtener el número "numPostIt" usado en la condicional de "obtener última actualización".
        /// El código se encuentra en "Base_Page" línea "116", método "inicioEncabezado".
        /// Ya se le están enviado por parámetro en el método el código del proyecto y la constante "CONST_PostIt".
        /// </summary>
        /// <returns>numPostIt.</returns>
        private int Obtener_numPostIt()
        {
            Int32 numPosIt = 0;

            //Hallar numero de post it por tab
            var query = from tur in consultas.Db.TareaUsuarioRepeticions
                        from tu in consultas.Db.TareaUsuarios
                        from tp in consultas.Db.TareaProgramas
                        where tp.Id_TareaPrograma == tu.CodTareaPrograma
                        && tu.Id_TareaUsuario == tur.CodTareaUsuario
                        && tu.CodProyecto == Convert.ToInt32(codProyecto)
                        && tp.Id_TareaPrograma == Constantes.CONST_PostIt
                        && tur.FechaCierre == null
                        select tur;

            numPosIt = query.Count();

            return numPosIt;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/06/2014.
        /// Guardar la información "Ultima Actualización".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_guardar_ultima_actualizacion_Click(object sender, EventArgs e)
        { prActualizarTabEval(Constantes.ConstSubEvaluacionProyecto.ToString(), codProyecto, codConvocatoria); Marcar(Constantes.ConstSubEvaluacionProyecto.ToString(), codProyecto, codConvocatoria, chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

        #endregion
    }
}