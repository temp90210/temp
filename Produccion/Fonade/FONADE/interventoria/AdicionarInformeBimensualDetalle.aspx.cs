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
    public partial class AdicionarInformeBimensualDetalle : Negocio.Base_Page
    {
        public string CodVariable;
        public string CodInforme;
        public string Accion;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CodVariable = Session["CodVariable"] != null ? CodVariable = Session["CodVariable"].ToString() : "0";
            CodInforme = Session["CodInforme"] != null ? CodInforme = Session["CodInforme"].ToString() : "";
            Accion = Session["Accion"] != null ? Accion = Session["Accion"].ToString() : "";

            if (!IsPostBack)
            {
                l_usuariolog.Text = usuario.Nombres + " " + usuario.Apellidos;
                l_fechaActual.Text = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");

                //Establecer título.
                if (Accion != "")
                {
                    if (Accion == "Borrar2") { lbl_enunciado.Text = "Eliminar"; evaluar(Accion); }
                    else{lbl_enunciado.Text = Accion; btnActualizar.Text = Accion;}

                    if (Accion == "Adicionar")
                    { LimpiarCampos(); }
                }

                if (CodVariable == "0")
                { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); }
                else { if (Accion != "Adicionar") { CargarDatos(); } }
            }
        }

        /// <summary>
        /// Cancelar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session["CodVariable"] = null;
            Session["CodInforme"] = null;
            Session["Accion"] = null;
            LimpiarCampos();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnActualizar_Click1(object sender, EventArgs e)
        {
            //Inicializar variables.
            String sqlConsulta;
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();

            sqlConsulta = evaluar(Session["Accion"].ToString());

            try
            {
                //NEW RESULTS:
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                cmd = new SqlCommand(sqlConsulta, con);

                if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd.Dispose();
                Session["CodVariable"] = null;
                Session["CodInforme"] = null;
                Session["Accion"] = null;
                LimpiarCampos();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Detalle procesado correctamente.'); window.opener.location.reload(); window.close();", true);
                return;
            }
            catch { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al procesar el detalle seleccionado.');", true); return; }
        }

        /// <summary>
        /// De acuerdo al valor almacenado en sesión, se devolverá la consulta SQL a ejecutar.
        /// </summary>
        /// <param name="variableAccion">Variable Session["Accion"]</param>
        /// <returns>Consulta SQL a ejecutar.</returns>
        private String evaluar(String variableAccion)
        {
            String consulta = "";
            Int32 seguimiento = 0;
            if (cbHacerSeguimiento.Checked) { seguimiento = 1; }

            switch (variableAccion)
            {
                case "Adicionar":
                    consulta = " Insert into VariableDetalle (CodVariable,Cumplimiento,Observacion,Cumple,Seguimiento,IndicadorAsociado,CodInforme) " +
                               " values (" + CodVariable + ", '" + txtCumplimiento.Text.ToString() + "', '" + txtObservacionInterventor.Text.ToString() + "', "
                               + rbCumple.SelectedValue + ", " + seguimiento + ", '" + txtIndicador.Text.ToString() + "', " + CodInforme + ")";
                    break;

                case "Editar":
                    consulta = " UPDATE VariableDetalle SET Cumplimiento = '" + txtCumplimiento.Text.ToString() + "' , " +
                               " Observacion = '" + txtObservacionInterventor.Text.ToString() + "' , Cumple = " + rbCumple.SelectedValue + " , Seguimiento = " + seguimiento +
                               ", IndicadorAsociado = '" + txtIndicador.Text.ToString() + "'"
                               + " WHERE Id_VariableDetalle = " + CodVariable;
                    break;

                case "Borrar":
                case "Borrar2":
                    consulta = " Delete from VariableDetalle where Id_VariableDetalle = " + CodVariable;
                    break;

            }

            return consulta;
        }

        /// <summary>
        /// Mauricio Aris Olave.
        /// 18/07/2014.
        /// Si se selecciona el valor "Session["Accion"] = Editar", se cargarán los datos.
        /// </summary>
        private void CargarDatos()
        {
            //Inicializar variables.
            String txtSQL = "";
            DataTable RS = new DataTable();

            try
            {
                txtSQL = " SELECT * FROM VariableDetalle WHERE id_VariableDetalle = " + CodVariable;
                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0)
                {
                    txtCumplimiento.Text = RS.Rows[0]["Cumplimiento"].ToString();
                    txtObservacionInterventor.Text = RS.Rows[0]["Observacion"].ToString();
                    txtIndicador.Text = RS.Rows[0]["IndicadorAsociado"].ToString();

                    try
                    {
                        if (RS.Rows[0]["Cumple"].ToString() != "") //IndicadorAsociado
                        {
                            if (Boolean.Parse(RS.Rows[0]["Cumple"].ToString())) //IndicadorAsociado
                            { rbCumple.Items[0].Selected = true; }
                            else { rbCumple.Items[1].Selected = true; }
                        }
                        else { rbCumple.Items[1].Selected = true; }
                    }
                    catch
                    { rbCumple.Items[1].Selected = true; }

                    cbHacerSeguimiento.Checked = Boolean.Parse(RS.Rows[0]["Seguimiento"].ToString());
                }
            }
            catch { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al cargar el detalle seleccionado.'); window.opener.location.reload(); window.close();", true); return; }
        }

        /// <summary>
        /// Limpiar los campos.
        /// </summary>
        private void LimpiarCampos()
        {
            txtCumplimiento.Text = "";
            txtObservacionInterventor.Text = "";
            txtIndicador.Text = "";
            rbCumple.Items[0].Selected = false;
            rbCumple.Items[1].Selected = false;
            cbHacerSeguimiento.Checked = false;
        }
    }
}