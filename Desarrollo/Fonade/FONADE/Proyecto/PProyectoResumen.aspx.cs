using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Fonade.Account;
using LinqKit;
using AjaxControlToolkit;
using System.ComponentModel;

namespace Fonade.FONADE.Proyecto
{
    public partial class PProyectoResumen : Negocio.Base_Page
    {
        public string codigo;
        public string codConvocatoria;
        public int txtTab = Constantes.CONST_SubResumenEjecutivo;
        Boolean esMiembro;
        Boolean bRealizado;

        protected void Page_Load(object sender, EventArgs e)
        {
            //codigo = Convert.ToInt32(Request.QueryString["codProyecto"]);
            codigo = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            codConvocatoria = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["codConvocatoria"].ToString() : "0";

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codigo);

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codigo, codConvocatoria);

            //Pintar post it.
            if (esMiembro && !bRealizado)
            {
                td_Post_It1.Visible = true;
                td_Post_It2.Visible = true;
                td_Post_It3.Visible = true;
                td_Post_It4.Visible = true;
                td_Post_It5.Visible = true;
                td_Post_It6.Visible = true;
            }

            if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
            {
                //Mostrar la información editable.
                PanelGuardar.Visible = true;
                PanelGuardar.Enabled = false;
                btn_guardar.Visible = false;
                btn_guardar.Enabled = false;
                tr_concepto.Visible = true;
                tr_potencial.Visible = true;
                tr_ventajas.Visible = true;
                tr_resumen.Visible = true;
                tr_proyecciones.Visible = true;
                tr_conclusiones.Visible = true;
            }
            else
            {
                //Mostrar los div's SOLO LECTURA
                tr_concepto.Controls.Clear();
                tr_concepto_vi.Visible = true;
                div_concepto.Visible = true;

                tr_potencial.Controls.Clear();
                tr_potencial_vi.Visible = true;
                div_potencial.Visible = true;

                tr_ventajas.Controls.Clear();
                tr_ventajas_vi.Visible = true;
                div_ventajas.Visible = true;

                tr_resumen.Controls.Clear();
                tr_resumen_vi.Visible = true;
                div_resumen.Visible = true;

                tr_proyecciones.Controls.Clear();
                tr_proyecciones_vi.Visible = true;
                div_proyecciones.Visible = true;

                tr_conclusiones.Controls.Clear();
                tr_conclusiones_vi.Visible = true;
                div_conclusiones.Visible = true;
            }

            if (!IsPostBack)
            {
                ObtenerDatosUltimaActualizacion();
                llenarcampos(Int32.Parse(codigo));
            }
        }

        private void llenarcampos(int codigo)
        {
            var sqlQuery = (from resumen in consultas.Db.ProyectoResumenEjecutivos
                            where resumen.CodProyecto == codigo
                            select new
                            {
                                resumen,
                            }).FirstOrDefault();
            if (sqlQuery != null)
            {
                #region Habilitar visualziación de Justificación y Políticas de acuerdo al rol del usuario. COMENTADO.
                //if (usuario.CodGrupo != Constantes.CONST_Emprendedor)
                //{
                //    #region Campos de solo-lectura visibles.
                //    tr_concepto_vi.Visible = true;
                //    div_concepto.Visible = true;

                //    tr_potencial_vi.Visible = true;
                //    div_potencial.Visible = true;

                //    tr_ventajas_vi.Visible = true;
                //    div_ventajas.Visible = true;

                //    tr_resumen_vi.Visible = true;
                //    div_resumen.Visible = true;

                //    tr_proyecciones_vi.Visible = true;
                //    div_proyecciones.Visible = true;

                //    tr_conclusiones_vi.Visible = true;
                //    div_conclusiones.Visible = true;
                //    #endregion

                //    //Filas invisibles.
                //    tr_concepto.Visible = false;
                //    tr_potencial.Visible = false;
                //    tr_ventajas.Visible = false;
                //    tr_resumen.Visible = false;
                //    tr_proyecciones.Visible = false;
                //    tr_conclusiones.Visible = false;

                //    div_concepto.InnerHtml = sqlQuery.resumen.ConceptoNegocio;
                //    div_conclusiones.InnerHtml = sqlQuery.resumen.ConclusionesFinancieras;
                //    div_potencial.InnerHtml = sqlQuery.resumen.PotencialMercados;
                //    div_proyecciones.InnerHtml = sqlQuery.resumen.Proyecciones;
                //    div_resumen.InnerHtml = sqlQuery.resumen.ResumenInversiones;
                //    div_ventajas.InnerHtml = sqlQuery.resumen.VentajasCompetitivas;

                //    //Otros controles ocultos.
                //    Post_It1.Visible = false;
                //    Post_It2.Visible = false;
                //    Post_It3.Visible = false;
                //    Post_It4.Visible = false;
                //    Post_It5.Visible = false;
                //    Post_It6.Visible = false;
                //    btn_guardar.Visible = false;
                //}
                //else
                //{
                //    #region Campos de solo-lectura invisibles.
                //    tr_concepto_vi.Visible = false;
                //    div_concepto.Visible = false;

                //    tr_potencial_vi.Visible = false;
                //    div_potencial.Visible = false;

                //    tr_ventajas_vi.Visible = false;
                //    div_ventajas.Visible = false;

                //    tr_resumen_vi.Visible = false;
                //    div_resumen.Visible = false;

                //    tr_proyecciones_vi.Visible = false;
                //    div_proyecciones.Visible = false;

                //    tr_conclusiones_vi.Visible = false;
                //    div_conclusiones.Visible = false;
                //    #endregion

                //    //Filas visibles.
                //    tr_concepto.Visible = true;
                //    tr_potencial.Visible = true;
                //    tr_ventajas.Visible = true;
                //    tr_resumen.Visible = true;
                //    tr_proyecciones.Visible = true;
                //    tr_conclusiones.Visible = true;

                //    txt_concepto.Text = sqlQuery.resumen.ConceptoNegocio;
                //    txt_Conclusiones.Text = sqlQuery.resumen.ConclusionesFinancieras;
                //    txt_potencial.Text = sqlQuery.resumen.PotencialMercados;
                //    txt_Proyecciones.Text = sqlQuery.resumen.Proyecciones;
                //    txt_ResumenInversiones.Text = sqlQuery.resumen.ResumenInversiones;
                //    txt_Ventajas.Text = sqlQuery.resumen.VentajasCompetitivas;
                //}
                #endregion

                if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
                {
                    txt_concepto.Text = sqlQuery.resumen.ConceptoNegocio;
                    txt_Conclusiones.Text = sqlQuery.resumen.ConclusionesFinancieras;
                    txt_potencial.Text = sqlQuery.resumen.PotencialMercados;
                    txt_Proyecciones.Text = sqlQuery.resumen.Proyecciones;
                    txt_ResumenInversiones.Text = sqlQuery.resumen.ResumenInversiones;
                    txt_Ventajas.Text = sqlQuery.resumen.VentajasCompetitivas;
                }
                else
                {
                    div_concepto.InnerHtml = sqlQuery.resumen.ConceptoNegocio;
                    div_conclusiones.InnerHtml = sqlQuery.resumen.ConclusionesFinancieras;
                    div_potencial.InnerHtml = sqlQuery.resumen.PotencialMercados;
                    div_proyecciones.InnerHtml = sqlQuery.resumen.Proyecciones;
                    div_resumen.InnerHtml = sqlQuery.resumen.ResumenInversiones;
                    div_ventajas.InnerHtml = sqlQuery.resumen.VentajasCompetitivas;
                }
            }
        }

        protected void btn_guardar_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_InsertUpdateproyectoresumenejecutivo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codigoProyecto", codigo);
            cmd.Parameters.AddWithValue("@txtConcepto", txt_concepto.Text);
            cmd.Parameters.AddWithValue("@txtPotencial", txt_potencial.Text);
            cmd.Parameters.AddWithValue("@txtVentajas", txt_Ventajas.Text);
            cmd.Parameters.AddWithValue("@txtInversiones", txt_ResumenInversiones.Text);
            cmd.Parameters.AddWithValue("@txtProyecciones", txt_Proyecciones.Text);
            cmd.Parameters.AddWithValue("@txtConclusiones", txt_Conclusiones.Text);
            SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
            con.Open();
            cmd2.ExecuteNonQuery();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
            cmd2.Dispose();
            cmd.Dispose();
            //Actualizar fecha modificación del tab.
            prActualizarTab(txtTab.ToString(), codigo);
            ObtenerDatosUltimaActualizacion();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Se ha guardado Correctamente!')", true);
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
            bool bRealizado = false;
            bool EsMiembro = false;
            Int32 numPostIt = 0;
            Int32 CodigoEstado = 0;

            try
            {
                //Consultar si es miembro.
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, codigo.ToString());

                //Obtener número "numPostIt".
                numPostIt = Obtener_numPostIt();

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(txtTab.ToString(), codigo, codConvocatoria);

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + codigo + " And CodContacto = " + usuario.IdContacto +
                         " and inactivo=0 and FechaInicio<=getdate() and FechaFin is null ";

                //Asignar variables a DataTable.
                var rs = consultas.ObtenerDataTable(txtSQL, "text");

                //Crear la variable de sesión.
                if (rs.Rows.Count > 0) { Session["CodRol"] = rs.Rows[0]["CodRol"].ToString(); }
                else { Session["CodRol"] = ""; }

                //Destruir la variable.
                rs = null;

                #endregion

                //Consultar los datos a mostrar en los campos correspondientes a la actualización.
                txtSQL = " SELECT Nombres + ' ' + Apellidos AS nombre, FechaModificacion, Realizado " +
                         " FROM TabProyecto, Contacto " +
                         " where Id_Contacto = CodContacto AND CodTab = " + txtTab +
                         " AND CodProyecto = " + codigo;

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

                    //Valor "bRealziado".
                    bRealizado = Convert.ToBoolean(tabla.Rows[0]["Realizado"].ToString());
                }

                //Asignar check de acuerdo al valor obtenido en "bRealizado".
                chk_realizado.Checked = bRealizado;

                //Determinar si el usuario actual puede o no "chequear" la actualización.
                //if (!(EsMiembro && numPostIt == 0 && ((usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && usuario.CodGrupo == Constantes.CONST_RolEvaluador && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                if (!(EsMiembro && numPostIt == 0 && ((Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && es_bNuevo(codigo)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                { chk_realizado.Enabled = false; }

                //Mostrar el botón de guardar.
                //if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (usuario.CodGrupo == Constantes.CONST_RolEvaluador && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
                if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codigo)))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                //Mostrar los enlaces para adjuntar documentos.
                if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolEmprendedor.ToString() && !bRealizado)
                {
                    tabla_docs.Visible = true;
                }

                //Destruir variables.
                tabla = null;
                txtSQL = null;
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
                //Destruir variables.
                //tabla = null;
                //txtSQL = null;
                //return;
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
                        && tu.CodProyecto == Int32.Parse(codigo)
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
        { prActualizarTab(txtTab.ToString(), codigo.ToString()); Marcar(txtTab.ToString(), codigo, "", chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codigo;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Nuevo";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codigo;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Vista";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        #endregion
    }
}