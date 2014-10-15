using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoIndicadorGestion : Negocio.Base_Page
    {

        public String codProyecto;
        //public int txtTab = Constantes.CONST_ProyeccionesVentas;
        public int txtTab = Constantes.ConstSubProductosIndicadores;
        public String codConvocatoria;
        /// <summary>
        /// Indica si se vá a crear o actualizar un indicador seleccionado.
        /// </summary>
        public String Accion;
        /// <summary>
        /// Código del indicador seleccionado para editar.
        /// </summary>
        public String CodIndicador;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                codProyecto = Session["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
                codConvocatoria = Session["codConvocatoria"].ToString();

                //codProyecto = Request.QueryString["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;

                Accion = Session["Accion"] != null && !string.IsNullOrEmpty(Session["Accion"].ToString()) ? Session["Accion"].ToString() : "Crear";
                CodIndicador = Session["CodIndicador"] != null && !string.IsNullOrEmpty(Session["CodIndicador"].ToString()) ? Session["CodIndicador"].ToString() : "0";
            }
            catch { Response.Redirect("EvaluacionProductos.aspx"); }

            if (!IsPostBack) { inicioPagina(); }
        }

        /// <summary>
        /// Configura el formulario de acuerdo a si es un indicador seleccionado o un
        /// indicador a crear.
        /// </summary>
        private void inicioPagina()
        {
            try
            {
                //Establecer el texto del botón "para activar su acción.
                B_Crear.Text = Accion;

                if (Accion == "Actualizar")
                {  CargarIndicadorSeleccionado(CodIndicador);  }
            }
            catch { LimpiarCampos(); }

            //codProyecto = "49472";
            //codConvocatoria = "151";
        }

        /// <summary>
        /// SelectedIndexChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DD_TipoIndicador_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DD_TipoIndicador.SelectedValue.Equals("Indicadores Cualitativos y de Cumplimiento"))
                TB_Denominador.Visible = false;
            else
                TB_Denominador.Visible = true;
        }

        /// <summary>
        /// Crear o actualizar el indicador.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_Crear_Click(object sender, EventArgs e)
        {
            String CodProyecto;
            String CodConvocatoria;
            String Aspecto;
            String FechaSeguimiento;
            String Numerador;
            String Denominador;
            String Descripcion;
            String RangoAceptable;

            if (!DD_TipoIndicador.SelectedValue.Equals("Indicadores Cualitativos y de Cumplimiento"))
            {
                if (String.IsNullOrEmpty(TB_Denominador.Text))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El Campo Denominador es requerido')", true);
                    return;
                }
                else
                { Denominador = TB_Denominador.Text; }
            }
            else
            { Denominador = ""; }

            CodProyecto = codProyecto;
            CodConvocatoria = codConvocatoria;
            Aspecto = TB_Aspecto.Text;
            FechaSeguimiento = TB_fechaSeguimiento.Text;
            Numerador = TB_Numerador.Text;

            Descripcion = TB_Descripcion.Text;
            RangoAceptable = TB_rango.Text;

            if (B_Crear.Text == "Crear")
            {
                #region Crear indicador.
                string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                using (var con = new SqlConnection(conexionStr))
                {
                    using (var com = con.CreateCommand())
                    {
                        com.CommandText = "MD_Insertar_Actualizar_EvaluacionIndicadorGestion";
                        com.CommandType = System.Data.CommandType.StoredProcedure;

                        com.Parameters.AddWithValue("@_Id_IndicadorGestion", 0);
                        com.Parameters.AddWithValue("@_CodProyecto", CodProyecto);
                        com.Parameters.AddWithValue("@_CodConvocatoria", CodConvocatoria);
                        com.Parameters.AddWithValue("@_Aspecto", Aspecto);
                        com.Parameters.AddWithValue("@_FechaSeguimiento", FechaSeguimiento);
                        com.Parameters.AddWithValue("@_Numerador", Numerador);
                        com.Parameters.AddWithValue("@_Denominador", Denominador);
                        com.Parameters.AddWithValue("@_Descripcion", Descripcion);
                        com.Parameters.AddWithValue("@_RangoAceptable", RangoAceptable);
                        com.Parameters.AddWithValue("@_caso", "CREATE");

                        try
                        {
                            con.Open();
                            com.ExecuteReader();
                            //Actualizar fecha modificación del tab.
                            prActualizarTabEval(txtTab.ToString(), codProyecto.ToString(), codConvocatoria.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
            }
            else if (B_Crear.Text == "Actualizar")
            {
                #region Actualizar indicador.

                //Inicializar variables.
                DataTable RsIndicador = new DataTable();
                String txtSQL = "";

                txtSQL = " SELECT Id_IndicadorGestion FROM EvaluacionIndicadorGestion " +
                         " WHERE Aspecto = '" + Aspecto + "' and CodProyecto=" + CodProyecto + " and " +
                         " CodConvocatoria = " + CodConvocatoria + " AND Id_IndicadorGestion<>" + CodIndicador;

                RsIndicador = consultas.ObtenerDataTable(txtSQL, "text");

                if (RsIndicador.Rows.Count == 0)
                {
                    txtSQL = " UPDATE EvaluacionIndicadorGestion " +
                             " SET Aspecto = '" + Aspecto + "', " +
                                 "FechaSeguimiento = '" + FechaSeguimiento + "', " +
                                 "Numerador = '" + Numerador + "', " +
                                 "Denominador = '" + Denominador + "', " +
                                 "Descripcion = '" + Descripcion + "', " +
                                 "RangoAceptable = " + RangoAceptable + " " +
                             " WHERE Id_IndicadorGestion = " + CodIndicador;

                    //Ejecutar consulta.
                    ejecutaReader(txtSQL, 2);
                    //Actualizar fecha modificación del tab.
                    prActualizarTabEval(txtTab.ToString(), codProyecto.ToString(), codConvocatoria.ToString());
                }

                #region COMENTARIOS NO BORRAR!.
                //if  RsIndicador.eof then			

                //    txtSQL=	"UPDATE EvaluacionIndicadorGestion "&_
                //            "SET Aspecto = '"&fnRequest("Aspecto")&"', "&_
                //                "FechaSeguimiento = '"&fnRequest("Fecha")&"', "&_ 
                //                "Numerador = '"&fnRequest("Numerador")&"', "&_
                //                "Denominador = '"&fnRequest("Denominador")&"', "&_  
                //                "Descripcion = '"&fnRequest("Descripcion")&"', "&_
                //                "RangoAceptable = "&fnRequest("RangoAceptable")&" "&_
                //            "WHERE Id_IndicadorGestion=" & CodIndicador

                //    Conn.Execute txtSQL

                //    'Actualizar fecha modificación del tab
                //    prActualizarTabEval txtTab, CodProyecto, codConvocatoria

                //    bRepetido = false
                //else
                //    bRepetido = true
                //End if

                //RsIndicador.close
                //Set RsIndicador=Nothing 
                #endregion

                #endregion
            }
            LimpiarCampos();
            Response.Redirect("EvaluacionProductos.aspx");
        }

        /// <summary>
        /// Limpiar los campos.
        /// </summary>
        private void LimpiarCampos()
        {
            L_NUEVOINDICADOR.Text = "NEUVO INDICADOR";
            TB_Aspecto.Text = "";
            TB_fechaSeguimiento.Text = "";
            TB_Numerador.Text = "";
            TB_Denominador.Text = "";
            TB_Descripcion.Text = "";
            TB_rango.Text = "";
            B_Crear.Text = "Crear";
        }

        #region Métodos de actualización.

        /// <summary>
        /// Cargar la información del indicador seleccionado.
        /// </summary>
        /// <param name="codIndicador">Indicador seleccionado.</param>
        private void CargarIndicadorSeleccionado(String codIndicador)
        {
            //Inicializar variables.
            DataTable rs = new DataTable();

            try
            {
                if (codIndicador != "0")
                {
                    rs = consultas.ObtenerDataTable(" SELECT * FROM EvaluacionIndicadorGestion WHERE id_indicadorGestion = " + codIndicador, "text");

                    if (rs.Rows.Count > 0)
                    {
                        L_NUEVOINDICADOR.Text = "MODIFICAR INDICADOR";
                        TB_Aspecto.Text = rs.Rows[0]["Aspecto"].ToString();
                        TB_fechaSeguimiento.Text = rs.Rows[0]["FechaSeguimiento"].ToString();
                        TB_Numerador.Text = rs.Rows[0]["Numerador"].ToString();
                        TB_Denominador.Text = rs.Rows[0]["Denominador"].ToString();
                        if (String.IsNullOrEmpty(rs.Rows[0]["Denominador"].ToString()))
                        {
                            DD_TipoIndicador.Items[0].Selected = false;
                            DD_TipoIndicador.Items[1].Selected = true;
                        }
                        TB_Descripcion.Text = rs.Rows[0]["Descripcion"].ToString();
                        TB_rango.Text = rs.Rows[0]["RangoAceptable"].ToString();
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}