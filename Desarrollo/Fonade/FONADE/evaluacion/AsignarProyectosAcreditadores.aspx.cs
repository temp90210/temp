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
    public partial class AsignarProyectosAcreditadores : Negocio.Base_Page //System.Web.UI.Page
    {
        String txtCondicion;
        String txtBuscar;

        DataTable llenadoGrilla;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarConvocatorias();

                try
                {
                    if (!String.IsNullOrEmpty(Session["SelectedIndex"].ToString()))
                    {
                        DDL_Convocatoria.SelectedIndex = Int32.Parse(Session["SelectedIndex"].ToString());
                        DDL_Convocatoria_SelectedIndexChanged(sender, e);
                        Session["SelectedIndex"] = null;

                        llenarGrilla("");
                    }
                }
                catch (Exception) { }
            }
        }

        #region Métodos anteriores comentados.
        //public DataTable Convocatoria()
        //{
        //    DataTable datatable = new DataTable();

        //    datatable.Columns.Add("Id_Convocatoria");
        //    datatable.Columns.Add("NomConvocatoria");

        //    String sql;
        //    sql = "SELECT DISTINCT C.ID_CONVOCATORIA, C.NomConvocatoria FROM CONVOCATORIA C JOIN CONVOCATORIAPROYECTO CP ON (CP.CODCONVOCATORIA= C.ID_CONVOCATORIA) JOIN PROYECTO P ON (CP.CODPROYECTO = P.ID_PROYECTO AND P.CODESTADO IN (3,10,11,12)) ORDER BY 2";
        //    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //    SqlCommand cmd = new SqlCommand(sql, conn);

        //    try
        //    {
        //        conn.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            DataRow fila = datatable.NewRow();
        //            fila["Id_Convocatoria"] = reader["Id_Convocatoria"].ToString();
        //            fila["NomConvocatoria"] = reader["NomConvocatoria"].ToString();
        //            datatable.Rows.Add(fila);
        //        }
        //        reader.Close();
        //    }
        //    catch (SqlException)
        //    {
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //    return datatable;
        //} 
        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 18/06/2014.
        /// Cargar el listado de convocatorias.
        /// </summary>
        private void CargarConvocatorias()
        {
            //Inicializar variables.
            String sqlConsulta = "";
            DataTable tabla = new DataTable();

            try
            {
                //Limpiar DropDownList "por si algo"...
                DDL_Convocatoria.Items.Clear();

                //Consulta:
                sqlConsulta = " SELECT DISTINCT C.ID_CONVOCATORIA, C.NomConvocatoria FROM CONVOCATORIA C JOIN CONVOCATORIAPROYECTO CP ON (CP.CODCONVOCATORIA= C.ID_CONVOCATORIA) JOIN PROYECTO P ON (CP.CODPROYECTO = P.ID_PROYECTO AND P.CODESTADO IN (3,10,11,12)) ORDER BY 2 ";

                //Asignar resultados de la consulta a variable DataTable.
                tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Crear ítem por defecto.
                ListItem item_default = new ListItem();
                item_default.Value = "";
                item_default.Text = "Seleccione la convocatoria para visualizar los planes disponibles";
                DDL_Convocatoria.Items.Add(item_default);

                //Recorrer la lista y generar ListItems.
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    ListItem item = new ListItem();
                    item.Value = tabla.Rows[i]["Id_Convocatoria"].ToString();
                    item.Text = tabla.Rows[i]["NomConvocatoria"].ToString();
                    DDL_Convocatoria.Items.Add(item);
                }
            }
            catch
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo cargar el listado de convocatorias.')", true);
                return;
            }
        }

        protected void Buscar_Click(object sender, EventArgs e)
        {
            txtBuscar = TB_Buscar.Text;
            llenarGrilla("");
            TB_Buscar.Text = "";
        }

        protected void B_SeleccionarAcreditador_Click(object sender, EventArgs e)
        {
            Int32 suma = 0;
            bool pass = false;
            foreach (GridViewRow GV_FilaGrid in GV_COnvocatoria.Rows)
            {
                //Instanciar CheckBox de la grilla.
                CheckBox checkbox = (CheckBox)GV_FilaGrid.FindControl("CB_Seleccionado");

                if (checkbox.Checked)
                {
                    suma++;
                    Int32 idCodigoPro = Int32.Parse(GV_COnvocatoria.DataKeys[GV_FilaGrid.RowIndex].Value.ToString());

                    try
                    {
                        if (String.IsNullOrEmpty(Session["idCodigoPro"].ToString()))
                        { Session["idCodigoPro"] = idCodigoPro + " "; }
                        else
                        { Session["idCodigoPro"] = Session["idCodigoPro"].ToString() + idCodigoPro + " "; }
                    }
                    catch (NullReferenceException) { Session["idCodigoPro"] = idCodigoPro + " "; }
                    pass = true;
                }
            }

            if (pass)
            {
                Session["idConvocatoriaEval"] = DDL_Convocatoria.SelectedValue;
                Session["SelectedIndex"] = DDL_Convocatoria.SelectedIndex;

                Response.Redirect("ListadoAcreditadores.aspx");
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe seleccionar un plan para asignar el acreditador.')", true);
                return;
            }
        }

        private void llenarGrilla(String str)
        {
            txtCondicion = "";

            if (DDL_Convocatoria.SelectedValue != "")
            {
                String sql = " SELECT DISTINCT P.ID_PROYECTO 'CODIGO', P.NOMPROYECTO 'NOMPROYECTO', " +
                             " CASE WHEN PC.CODCONTACTO IS NULL THEN 'No Asignado' ELSE 'Asignado' END 'Acreditador', " +
                             " (C.NOMBRES + ' ' + C.APELLIDOS) 'NOMACREDITADOR', " +
                             " PC.CODCONTACTO, MAX(PC.FECHAINICIO) 'FechaAsignacion' " +
                             " FROM PROYECTO P JOIN CONVOCATORIAPROYECTO CP " +
                             " ON (CP.CODCONVOCATORIA=" + DDL_Convocatoria.SelectedValue + " AND CP.CODPROYECTO=P.ID_PROYECTO) " +
                             " LEFT JOIN PROYECTOCONTACTO PC " +
                             " ON (PC.INACTIVO=0 AND PC.CODCONVOCATORIA = CP.CODCONVOCATORIA AND PC.ACREDITADOR=1 " +
                             " AND PC.CODPROYECTO=P.ID_PROYECTO) LEFT JOIN CONTACTO C " +
                             " ON (PC.CODCONTACTO = C.ID_CONTACTO AND PC.CODPROYECTO=P.ID_PROYECTO) " +
                             " where P.CodEstado in (3,10,11,12,15,16) ";

                if (str.Trim() != "")
                {
                    if (txtCondicion.Length > 0) { txtCondicion = txtCondicion + " AND "; }
                    else { txtCondicion = " AND "; }
                    txtCondicion = txtCondicion + " P.NOMPROYECTO LIKE '" + str + "%'";
                }

                if (!String.IsNullOrEmpty(txtBuscar))
                {
                    if (txtCondicion.Length > 0) { txtCondicion = txtCondicion + " AND "; }
                    else { txtCondicion = " AND "; }

                    Int32 number;
                    if (Int32.TryParse(txtBuscar, out number)) { txtCondicion = txtCondicion + " (P.NOMPROYECTO LIKE '%" + txtBuscar + "%'   OR P.ID_PROYECTO = " + txtBuscar + ")"; }
                    else { txtCondicion = txtCondicion + " P.NOMPROYECTO LIKE '%" + txtBuscar + "%'"; }
                }

                sql = sql + txtCondicion + " GROUP BY P.ID_PROYECTO,P.NOMPROYECTO,C.NOMBRES,C.APELLIDOS,PC.CODCONTACTO ";

                //Inicializar grilla.
                llenadoGrilla = new DataTable();

                //Asignar los resultados de la consulta a variable DataTable.
                llenadoGrilla = consultas.ObtenerDataTable(sql, "text");

                //Nueva columna para mostrar los valores formateados.
                llenadoGrilla.Columns.Add("a_FechaAsignacion", typeof(System.String));

                //Recorrer cada fila de la grilla para que en la columna generada se agregue la fecha de asignación formateada.
                foreach (DataRow row in llenadoGrilla.Rows)
                {
                    if (row["FechaAsignacion"].ToString().Trim() != "")
                    {
                        DateTime fecha = new DateTime();
                        String dat = "";
                        fecha = DateTime.Parse(row["FechaAsignacion"].ToString());
                        dat = fecha.ToString("dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                        try { row["a_FechaAsignacion"] = dat; }
                        catch { row["FechaAsignacion"] = row["FechaAsignacion"]; }
                        fecha = new DateTime();
                        dat = null;
                    }
                    else
                    {
                        try { row["a_FechaAsignacion"] = ""; }
                        catch { row["FechaAsignacion"] = ""; }
                    }
                }

                //Bindear la grilla.
                Session["dtEmpresas"] = llenadoGrilla;
                GV_COnvocatoria.DataSource = llenadoGrilla;
                GV_COnvocatoria.DataBind();
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe seleccionar una convocatoria.')", true);
                return;
            }
        }

        protected void LB_Codigo_Click(object sender, EventArgs e)
        {
            interno_redirect(sender, e);
        }

        protected void LB_NomProyecto_Click(object sender, EventArgs e)
        {
            interno_redirect(sender, e);
        }

        protected void LB_Acreditador_Click(object sender, EventArgs e)
        {
            interno_redirect(sender, e);
        }

        protected void LB_NomAcreditador_Click(object sender, EventArgs e)
        {
            interno_redirect(sender, e);
        }

        protected void LB_FechaAsignacion_Click(object sender, EventArgs e)
        {
            interno_redirect(sender, e);
        }

        /// <summary>
        /// SelectedIndexChanged de "DD_Convocatoria".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DDL_Convocatoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDL_Convocatoria.SelectedValue == "")
            {
                #region Ocultar ciertos campos.
                //P_Convocatoria.Visible = false;
                //P_Convocatoria.Enabled = false;

                tr_1.Attributes.Add("display", "none");
                tr_2.Attributes.Add("display", "none");
                tr_txt_buscar_plan.Attributes.Add("display", "none");
                tr_textbox_buscar_plan.Attributes.Add("display", "none");
                tr_3.Attributes.Add("display", "none");
                tr_btn_seleccionar.Attributes.Add("display", "none");
                tr_control_abc.Attributes.Add("display", "none");
                tr_grilla.Attributes.Add("display", "none");

                tr_1.Visible = false;
                tr_2.Visible = false;
                tr_txt_buscar_plan.Visible = false;
                tr_textbox_buscar_plan.Visible = false;
                tr_3.Visible = false;
                tr_btn_seleccionar.Visible = false;
                tr_control_abc.Visible = false;
                //tr_control_abc.Visible = false;
                tr_grilla.Visible = false;
                #endregion
            }
            else
            {
                #region Mostrar ciertos campos.
                //P_Convocatoria.Visible = true;
                //P_Convocatoria.Enabled = true;

                tr_1.Visible = true;
                tr_2.Visible = true;
                tr_txt_buscar_plan.Visible = true;
                tr_textbox_buscar_plan.Visible = true;
                tr_3.Visible = true;
                tr_btn_seleccionar.Visible = true;
                tr_control_abc.Visible = true;
                tr_grilla.Visible = true;
                #endregion
                llenarGrilla("");
            }
        }

        public void interno_redirect(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GV_FilaGrid = GV_COnvocatoria.Rows[indicefila];

            CheckBox checkbox = (CheckBox)GV_FilaGrid.FindControl("CB_Seleccionado");

            if (!checkbox.Checked)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe seleccionar un plan para asignar el acreditador')", true);
                return;
            }
            else
            {
                if (DDL_Convocatoria.SelectedValue != "")
                {
                    Int32 idCodigoPro = Int32.Parse(GV_COnvocatoria.DataKeys[GV_FilaGrid.RowIndex].Value.ToString());

                    Session["idCodigoPro"] = idCodigoPro;
                    Session["idConvocatoriaEval"] = DDL_Convocatoria.SelectedValue;
                    Session["SelectedIndex"] = DDL_Convocatoria.SelectedIndex;

                    Response.Redirect("ListadoAcreditadores.aspx");
                }
                else
                {
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe seleccionar una convocatoria.')", true);
                    return;
                }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 18/06/2014.
        /// Establecer el color del LinkButton "LB_Acreditador" si su texto es igual a "No Asignado".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GV_COnvocatoria_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lnk = e.Row.FindControl("LB_Acreditador") as LinkButton;

                if (lnk != null) { if (lnk.Text == "No Asignado") { lnk.ForeColor = System.Drawing.Color.Red; } }
            }
        }

        #region Métodos de selección del abecedario.

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "%" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_todos_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "%";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "A" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_A_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "A";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "B" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_B_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "B";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "C" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_C_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "C";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "D" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_D_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "D";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "E" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_E_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "E";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "F" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_F_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "F";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "G" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_G_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "G";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "H" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_H_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "H";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "I" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_I_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "I";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "J" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_J_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "J";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "K" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_K_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "K";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "L" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_L_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "L";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "M" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_M_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "M";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "N" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_N_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "N";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "O" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_O_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "O";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "P" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_P_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "P";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "Q" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_Q_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "Q";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "R" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_R_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "R";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "S" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_S_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "S";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "T" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_T_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "T";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "U" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_U_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "U";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "V" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_V_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "V";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "W" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_W_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "W";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "X" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_X_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "X";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "Y" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_Y_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "Y";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "Z" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_Z_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "Z";
            llenarGrilla(Session["upper_letter_selected"].ToString());
        }

        #endregion

        /// <summary>
        /// Se debe enviar la información de la tabla en uan variable se sesión
        /// para poder sortearlo.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            var sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;

                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 18/06/2014.
        /// Sortear la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GV_COnvocatoria_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GV_COnvocatoria.DataSource = Session["dtEmpresas"];
                GV_COnvocatoria.DataBind();
            }
        }

        protected void GV_COnvocatoria_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                GV_COnvocatoria.PageIndex = e.NewPageIndex;
                GV_COnvocatoria.DataSource = dt;
                GV_COnvocatoria.DataBind();
            }
        }
    }
}