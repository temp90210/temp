#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>06 - 03 - 2014</Fecha>
// <Archivo>CatalogoIndicador.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Web.UI;
using System.Linq;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoIndicador : Negocio.Base_Page
    {
        #region Propiedades

        public string Accion;
        public int Codproyecto
        {
            get { return Convert.ToInt32(ViewState["proyecto"].ToString()); }
            set { ViewState["proyecto"] = value; }
        }
        public int Codconvocatoria
        {
            get { return Convert.ToInt32(ViewState["convocatoria"].ToString()); }
            set { ViewState["convocatoria"] = value; }
        }
        public string CodIndicador;

        /// <summary>
        /// Contiene las consultas SQL.
        /// </summary>
        String txtSQL;

        /// <summary>
        /// Descripción del indicador seleccionado.
        /// </summary>
        String Descripcion_Loaded;

        /// <summary>
        /// Tab de esta pestaña.
        /// </summary>
        Int32 txtTab = Constantes.ConstSubIndicadoresFinancieros;

        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Accion = Session["AccionAporteEvaluacion"] != null && !string.IsNullOrEmpty(Session["AccionAporteEvaluacion"].ToString()) ? Session["AccionAporteEvaluacion"].ToString() : "Crear";
            Codproyecto = Convert.ToInt32(Session["Codproyecto"].ToString());
            Codconvocatoria = Convert.ToInt32(Session["Codconvocatoria"].ToString());
            Descripcion_Loaded = Session["Descripcion_Loaded"] != null && !string.IsNullOrEmpty(Session["Descripcion_Loaded"].ToString()) ? Session["Descripcion_Loaded"].ToString() : "";
            CodIndicador = Session["CodAporte"] != null && !string.IsNullOrEmpty(Session["CodAporte"].ToString()) ? Session["CodAporte"].ToString() : "0";

            if (!IsPostBack)
            {
                lbltitle0.Text = DateTime.Now.ToShortDateString();
                CargarInformacion();
            }
        }

        /// <summary>
        /// Cargar la información del indicador seleccionado.
        /// </summary>
        public void CargarInformacion()
        {
            try
            {
                if (Accion == "Crear")
                {
                    btn_crearaporte.Text = "Crear";
                    lbltitle.Text = "Crear Indicador";
                    LimpiarCampos();
                }
                else if (Accion == "Modificar")
                {
                    btn_crearaporte.Text = "Modificar";
                    lbltitle.Text = "Modificar Indicador";
                }

                if (Int32.Parse(CodIndicador) != 0)
                {
                    txtSQL = " SELECT * FROM EvaluacionProyectoIndicador WHERE codProyecto = " + Codproyecto +
                             " AND codConvocatoria = " + Codconvocatoria + " AND id_Indicador = " + CodIndicador;
                    var indicador = consultas.ObtenerDataTable(txtSQL, "text");

                    if (indicador.Rows.Count > 0)
                    {
                        txt_detalle.Text = indicador.Rows[0]["Descripcion"].ToString();
                        txtvalor.Text = indicador.Rows[0]["Valor"].ToString();
                        dpl_tipo.SelectedValue = indicador.Rows[0]["Tipo"].ToString();
                        dpl_tipo.Enabled = !Boolean.Parse(indicador.Rows[0]["Protegido"].ToString());
                    }

                    txtSQL = "";
                    indicador = null;
                }
            }
            catch { }
        }

        /// <summary>
        /// Guarda y actualiza....
        /// </summary>
        public void Guardar()
        {
            var eevaluacion = new EvaluacionProyectoIndicador();

            try
            {
                if (Accion == "Crear")
                {
                    #region Crear VERSIÓN COMENTADA.

                    //var indicador = consultas.Db.EvaluacionProyectoIndicadors.FirstOrDefault(
                    //                e => e.Descripcion == txt_detalle.Text
                    //                     && e.codProyecto == Codproyecto);

                    //if (indicador != null)
                    //{
                    //    RedirectPage(false, "Ya existe un Indicador con esa Descripción.");
                    //}
                    //else
                    //{
                    //    eevaluacion.codProyecto = Codproyecto;
                    //    eevaluacion.codConvocatoria = Codconvocatoria;
                    //    eevaluacion.Descripcion = txt_detalle.Text;
                    //    eevaluacion.Valor = Convert.ToDouble(txtvalor.Text.Trim());
                    //    eevaluacion.Tipo = Convert.ToChar(dpl_tipo.SelectedValue);
                    //    consultas.Db.EvaluacionProyectoIndicadors.InsertOnSubmit(eevaluacion);
                    //    consultas.Db.SubmitChanges();

                    //    //Actualizar fecha modificación del tab.
                    //    prActualizarTabEval(txtTab.ToString(), Codproyecto.ToString(), Codconvocatoria.ToString());

                    //    //Limpiar variables de sesión.
                    //    Session["CodAporte"] = null;
                    //    Session["AccionAporteEvaluacion"] = null;

                    //    //Limpiar campos.
                    //    LimpiarCampos();
                    //}

                    #endregion

                    #region Crear "versión mejorada".

                    //Inicializar variables.
                    DataTable RsIndicador = new DataTable();

                    txtSQL = "select *  from EvaluacionProyectoIndicador where descripcion = '" + txt_detalle.Text + "' and codproyecto=" + Codproyecto;
                    RsIndicador = consultas.ObtenerDataTable(txtSQL, "text");

                    if (RsIndicador.Rows.Count == 0)
                    {
                        txtSQL = " Insert into EvaluacionProyectoIndicador(CodProyecto, CodConvocatoria, descripcion, valor, Tipo, Protegido) " +
                                 " values (" + Codproyecto + ", " + Codconvocatoria + ", '" + txt_detalle.Text.Trim() + "'," + txtvalor.Text.Trim() + ",'" + dpl_tipo.SelectedValue + "',0)";

                        //Ejecutar sentencia.
                        ejecutaReader(txtSQL, 2);

                        //Actualizar fecha modificación del tab.
                        prActualizarTabEval(txtTab.ToString(), Codproyecto.ToString(), Codconvocatoria.ToString());

                        //Limpiar variables de sesión.
                        Session["CodAporte"] = null;
                        Session["AccionAporteEvaluacion"] = null;

                        //Limpiar campos.
                        LimpiarCampos();
                    }

                    #endregion
                }
                else if (Accion == "Modificar")
                {
                    #region Actualizar.

                    Actualizar();
                    //Limpiar variables de sesión.
                    Session["CodAporte"] = null;
                    Session["AccionAporteEvaluacion"] = null;

                    #endregion
                }
            }
            catch (Exception ex)
            { throw new Exception(ex.Message + " " + ex.StackTrace); }

        }

        /// <summary>
        /// Actualizar el indicador seleccionado.
        /// </summary>
        public void Actualizar()
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            DataTable RsIndicador = new DataTable();
            String bRepetido = "Ya existe un Indicador con esa Descripción";
            bool Protegido = false;

            #region Consultar Protegido.

            txtSQL = " SELECT Protegido FROM EvaluacionProyectoIndicador WHERE id_indicador = " + CodIndicador;
            var dt = consultas.ObtenerDataTable(txtSQL, "text");
            if (dt.Rows.Count > 0) { Protegido = Boolean.Parse(dt.Rows[0]["Protegido"].ToString()); }
            dt = null;

            #endregion

            try
            {
                txtSQL = " SELECT Id_Indicador, Protegido from EvaluacionProyectoIndicador " +
                         " WHERE descripcion = '" + txt_detalle.Text.Trim() + "' and CodProyecto = " + Codproyecto +
                         " AND CodConvocatoria = " + Codconvocatoria + " AND Id_Indicador <> " + CodIndicador;

                RsIndicador = consultas.ObtenerDataTable(txtSQL, "text");

                if (RsIndicador.Rows.Count == 0)
                {
                    txtSQL = "";

                    if (Protegido) { txtSQL = "Tipo = '" + dpl_tipo.SelectedValue + "', descripcion ='" + txt_detalle.Text.Trim() + "',"; }

                    txtSQL = " UPDATE  EvaluacionProyectoIndicador " +
                             " SET " + txtSQL + " Valor = " + txtvalor.Text.Trim() +
                             " WHERE Id_Indicador = " + CodIndicador;

                    //Ejecutar sentencia.
                    ejecutaReader(txtSQL, 2);

                    //Actualizar fecha modificación del tab.
                    prActualizarTabEval(txtTab.ToString(), Codproyecto.ToString(), Codconvocatoria.ToString());

                    #region COMENTARIOS NO BORRAR!.
                    //try
                    //{
                    //    //NEW RESULTS:
                    //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    //    cmd = new SqlCommand(txtSQL, con);

                    //    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    //    cmd.CommandType = CommandType.Text;
                    //    cmd.ExecuteNonQuery();
                    //    con.Close();
                    //    con.Dispose();
                    //    cmd.Dispose();
                    //    //Actualizar fecha modificación del tab.
                    //    prActualizarTabEval(txtTab.ToString(), Codproyecto.ToString(), Codconvocatoria.ToString());
                    //}
                    //catch { } 
                    #endregion
                }
                else
                {
                    //ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                    //return;
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + bRepetido + "')", true);
                    return;
                }
            }
            catch { }
        }

        /// <summary>
        /// Limpiar los campos.
        /// </summary>
        public void LimpiarCampos()
        {
            txt_detalle.Text = "";
            txtvalor.Text = "";
            lbltitle0.Text = "";
            Session["CodAporte"] = null;
            Session["AccionAporteEvaluacion"] = null;
        }

        /// <summary>
        /// Botón.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_crearaporte_Click(object sender, EventArgs e)
        {
            string validado = "";

            validado = ValidarCampos();

            if (validado == "")
            {
                Guardar();

                //Limpiar variables de sesión.
                Session["CodAporte"] = null;
                Session["AccionAporteEvaluacion"] = null;
                //BtnCerrar_Click(sender, e);
                //Redirigir.
                Response.Redirect("EvaluacionIndicadoresFinancieros.aspx");
                //ClientScriptManager cm = this.ClientScript;
                //cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                //cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location=window.opener.location;window.close();</script>");
                return;
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + validado + "')", true);
                return;
            }
        }

        /// <summary>
        /// Cerrar ventana...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCerrar_Click(object sender, EventArgs e)
        {
            //Limpiar variables de sesión.
            Session["CodAporte"] = null;
            Session["AccionAporteEvaluacion"] = null;
            RedirectPage(true);
        }

        /// <summary>
        /// Valida los campos.
        /// </summary>
        /// <returns>Vacío = Puede continuar. // Con datos = Error.</returns>
        private string ValidarCampos()
        {
            //Inicializar variables.
            string valor = "";
            decimal numero = 0; //int

            try
            {
                if (txt_detalle.Text.Trim() == "")
                {
                    valor = Texto("TXT_DESCRIP_REQ");
                }
                if (txt_detalle.Text.Trim().Length > 255)
                {
                    valor = "La descripción no debe tener mas de 255 caracteres";
                }
                try
                {
                    if (txtvalor.Text.Trim() != "")
                    { if (txtvalor.Text.Contains(',')) { txtvalor.Text = txtvalor.Text.Replace(',', '.'); } }
                    else { txtvalor.Text = "0"; }
                    numero = decimal.Parse(txtvalor.Text);
                }
                catch { valor = Texto("TXT_VALOR_NUM"); }

                return valor;
            }
            catch { return "Error inesperado"; }
        }
    }
}