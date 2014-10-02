using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Fonade.FONADE.Administracion
{
    public partial class AdicionarProyectoAcreditacionActa : Negocio.Base_Page
    {
        #region Variables globales.

        /// <summary>
        /// Código del acta.
        /// </summary>
        String CodActa;

        /// <summary>
        /// Convocatoria del acta seleccionada.
        /// </summary>
        String CodConvocatoria;

        /// <summary>
        /// Entero (procesado de un booleano). Determina si el acta está o no acreditada.
        /// </summary>
        Int32 strEstadoAcreditacion;

        /// <summary>
        /// Contiene las consultas de SQL.
        /// </summary>
        String txtSQL;

        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CodActa = Session["CodActa_Seleccionado"] != null ? CodActa = Session["CodActa_Seleccionado"].ToString() : "0";
            CodConvocatoria = Session["CodConvocatoria_Acta"] != null ? CodConvocatoria = Session["CodConvocatoria_Acta"].ToString() : "0";
            strEstadoAcreditacion = Session["bActaAcreditada"] != null ? strEstadoAcreditacion = Convert.ToInt32(Session["bActaAcreditada"].ToString()) : 0;

            if (!IsPostBack)
            {
                if (CodActa == "0" && CodConvocatoria == "0")
                { System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); return; }

                //Cargar la grilla de planes seleccionables.
                CargarPlanesSeleccionables("", "", "");
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 21/07/2014.
        /// Método que carga los planes de negocio para seleccionar.
        /// </summary>
        /// <param name="filtro">Del DropDownList.</param>
        /// <param name="strBuscarNombre">Nombre digitado.</param>
        /// <param name="strBuscarId">Id digitado.</param>
        private void CargarPlanesSeleccionables(String filtro, String strBuscarNombre, String strBuscarId)
        {
            //Inicializar variables.
            DataTable rs = new DataTable();
            strBuscarNombre = " AND UPPER(nomproyecto) like '%" + strBuscarNombre + "%' ";
            strBuscarId = " AND Id_proyecto = '" + strBuscarId + "' ";

            try
            {
                txtSQL = " select id_proyecto, nomproyecto, CodEstado from proyecto inner join ConvocatoriaProyecto on  Id_Proyecto=Codproyecto " +
                         " where id_Proyecto not in " +
                         " (select CodProyecto from AcreditacionActaProyecto AAP inner join AcreditacionActa AA on AA.Id_Acta = AAP.CodActa where AA.codConvocatoria = " + CodConvocatoria + ")" +
                         " and codestado in ('" + strEstadoAcreditacion + "') and codconvocatoria = " + CodConvocatoria;

                //Verifica si tiene el filtro.
                if (filtro != "") { txtSQL = txtSQL + " AND UPPER(nomproyecto) like '" + filtro + "%'"; }
                //Verifica si se ha escrito el nombre del proyecto.
                if (strBuscarNombre != "") { txtSQL = txtSQL + strBuscarNombre; }
                //Verifica si se ha escrito el id del proyecto.
                if (strBuscarId != "") { txtSQL = txtSQL + strBuscarId; }

                txtSQL = txtSQL + " group by id_proyecto,nomproyecto, CodEstado order by nomproyecto ASC ";

                rs = consultas.ObtenerDataTable(txtSQL, "text");

                gvPlanesNegocio.DataSource = rs;
                gvPlanesNegocio.DataBind();
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al cargar los planes de negocio seleccionables.')", true);
                return;
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
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
            CargarPlanesSeleccionables(Session["upper_letter_selected"].ToString(), "", "");
        }

        #endregion

        /// <summary>
        /// Buscar planes de negocio seleccionables.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_buscar_Click(object sender, EventArgs e)
        { CargarPlanesSeleccionables("", txtBuscar.Text.Trim(), txtBuscarId.Text.Trim()); }

        /// <summary>
        /// Chequear todos los planes de negocio seleccionables.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chectodos_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow fila in gvPlanesNegocio.Rows)
            {
                CheckBox box = (CheckBox)fila.FindControl("chckplanopera");
                if (box.Enabled)
                    box.Checked = chectodos.Checked;
            }
        }

        /// <summary>
        /// Adicionar el(los) plan(es) de negocio seleccionado(s) al acta seleccionada en "AcreditacionActa.aspx".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Adicionar_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            ClientScriptManager cm = this.ClientScript;
            DataTable rs = new DataTable();
            int Estado = 0;

            foreach (GridViewRow fila in gvPlanesNegocio.Rows)
            {
                CheckBox box = (CheckBox)fila.FindControl("chckplanopera");
                HiddenField hdf = (HiddenField)fila.FindControl("hdf_proyecto");

                if (box != null && hdf != null)
                {
                    if (box.Checked)
                    {
                        txtSQL = "select CodEstado from proyecto where Id_proyecto = '" + hdf.Value + "'";
                        rs = consultas.ObtenerDataTable(txtSQL, "text");

                        if (rs.Rows.Count > 0)
                        {
                            if (!String.IsNullOrEmpty(rs.Rows[0]["Estado"].ToString()))
                            {
                                if (Int32.Parse(rs.Rows[0]["Estado"].ToString()) == Constantes.CONST_Aprobacion_Acreditacion)
                                { Estado = 1; }
                            }
                        }
                        rs = null;

                        txtSQL = " insert into AcreditacionActaproyecto values (" + CodActa + ", " + hdf.Value + ", " + Estado + ")";

                        try
                        {
                            //NEW RESULTS:
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                            SqlCommand cmd = new SqlCommand(txtSQL, con);

                            if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            con.Close();
                            con.Dispose();
                            cmd.Dispose();
                        }
                        catch { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al agregar los planes de negocio al acta.')", true); }
                    }
                }
            }

            //Recargar la página padre y cerrar la ventana actual.
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location=window.opener.location;window.close();</script>");
        }
    }
}