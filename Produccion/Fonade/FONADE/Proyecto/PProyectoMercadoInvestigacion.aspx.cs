using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Datos;

namespace Fonade.FONADE.Proyecto
{
    public partial class PProyectoMercadoInvestigacion : Negocio.Base_Page
    {
        public string codProyecto;
        public string codConvocatoria;
        public int txtTab = Constantes.CONST_InvestigacionMercados;
        protected ProyectoMercadoInvestigacion pmi;
        Boolean esMiembro;
        /// <summary>
        /// Consultar si el proyecto está o no realizado...
        /// </summary>
        Boolean bRealizado;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codProyecto, codConvocatoria);

            if (esMiembro && !bRealizado)
            {
                this.div_post_it_1.Visible = true;
                this.div_post_it_2.Visible = true;
                this.div_post_it_3.Visible = true;
                this.div_post_it_4.Visible = true;
                this.div_post_it_5.Visible = true;
            }

            codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            codConvocatoria = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["CodConvocatoria"].ToString() : "0";

            inicioEncabezado(codProyecto, codConvocatoria, txtTab);

            pmi = getProyectoMercadoInvestigacions();

            if (pmi == null) { /* Response.Redirect("~/Default.aspx"); O lanzar error  */ return; }

            if (!IsPostBack)
            { definirCampos(); ObtenerDatosUltimaActualizacion(); }
        }

        private ProyectoMercadoInvestigacion getProyectoMercadoInvestigacions()
        {
            var query = from pmi in consultas.Db.ProyectoMercadoInvestigacions
                        where pmi.CodProyecto == Convert.ToInt32(codProyecto)
                        select pmi;
            if (query.Count() > 0 && query != null)
            { return query.FirstOrDefault(); }
            else
            {
                Datos.ProyectoMercadoInvestigacion emptypmi = new Datos.ProyectoMercadoInvestigacion();
                return emptypmi;
            }
        }

        private void definirCampos()
        {
            //si encuentra el proyecto mercado investigacion
            if (pmi != null)
            {
                procesarCampo(ref txt_objetivos, ref txt_objetivos_HtmlEditorExtender, ref panel_objetivos, pmi.Objetivos, esMiembro, bRealizado, "");
                procesarCampo(ref txt_justificacion, ref txt_justificacion_HtmlEditorExtender, ref panel_justificacion, pmi.Justificacion, esMiembro, bRealizado, "");
                procesarCampo(ref txt_analisisS, ref txt_analisisS_HtmlEditorExtender, ref panel_analisiss, pmi.AnalisisSector, esMiembro, bRealizado, "");
                procesarCampo(ref txt_analisisM, ref txt_analisisM_HtmlEditorExtender, ref panel_analisism, pmi.AnalisisMercado, esMiembro, bRealizado, "");
                procesarCampo(ref txt_analisisC, ref txt_analisisC_HtmlEditorExtender, ref panel_analisisc, pmi.AnalisisCompetencia, esMiembro, bRealizado, "");
            }

            if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
            {
                btm_guardarCambios.Visible = true;
                btn_limpiarCampos.Visible = true;
            }
            else
            {
                btm_guardarCambios.Visible = false;
                btn_limpiarCampos.Visible = false;
            }
        }

        protected void btn_limpiarCampos_Click(object sender, EventArgs e)
        {
            //Add if it is necesary
        }

        protected void btm_guardarCambios_Click(object sender, EventArgs e)
        {
            //Miramos si ya esxist el registro
            var query = from p in consultas.Db.ProyectoMercadoInvestigacions
                        where p.CodProyecto == Convert.ToInt32(codProyecto)
                        select p;
            if (query.Count() > 0)
            {
                ProyectoMercadoInvestigacion pmv = new ProyectoMercadoInvestigacion()
                {
                    CodProyecto = Convert.ToInt32(codProyecto),
                    AnalisisSector = txt_analisisS.Text,
                    AnalisisMercado = txt_analisisM.Text,
                    AnalisisCompetencia = txt_analisisC.Text,
                    Objetivos = txt_objetivos.Text,
                    Justificacion = txt_justificacion.Text,
                };
                consultas.Db.ProyectoMercadoInvestigacions.InsertOnSubmit(pmv);
                //Actualizar fecha modificación del tab.
                prActualizarTab(txtTab.ToString(), codProyecto);
                ObtenerDatosUltimaActualizacion();
            }
            else
            {
                pmi.AnalisisSector = txt_analisisS.Text;
                pmi.AnalisisMercado = txt_analisisM.Text;
                pmi.AnalisisCompetencia = txt_analisisC.Text;
                pmi.Objetivos = txt_objetivos.Text;
                pmi.Justificacion = txt_justificacion.Text;
            }
            consultas.Db.SubmitChanges();
            //Actualizar fecha modificación del tab.
            prActualizarTab(txtTab.ToString(), codProyecto);
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
            bool bRealizado = false;
            bool EsMiembro = false;
            Int32 numPostIt = 0;
            Int32 CodigoEstado = 0;

            try
            {
                //Consultar si es miembro.
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

                //Obtener número "numPostIt".
                numPostIt = Obtener_numPostIt();

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(txtTab.ToString(), codProyecto, codConvocatoria);

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + codProyecto + " And CodContacto = " + usuario.IdContacto +
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
                         " AND CodProyecto = " + codProyecto;

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
                if (!(EsMiembro && numPostIt == 0 && ((Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                { chk_realizado.Enabled = false; }

                //Mostrar el botón de guardar.
                if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                //Mostrar los enlaces para adjuntar documentos.
                if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolEmprendedor.ToString() && !bRealizado)
                {
                    tabla_docs.Visible = true;
                }

                //Mostrar o no los botones...
                //if (If Session("Miembro")=1 and Session("CodGrupo") = CONST_Emprendedor and not bRealizado)
                if (EsMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
                {
                    btm_guardarCambios.Visible = true;
                    btn_limpiarCampos.Visible = true;
                }
                else
                {
                    btm_guardarCambios.Visible = false;
                    btn_limpiarCampos.Visible = false;
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
        {
            prActualizarTab(txtTab.ToString(), codProyecto.ToString());
            Marcar(txtTab.ToString(), codProyecto, "", chk_realizado.Checked);
            ObtenerDatosUltimaActualizacion();
            //Recargar la página entera.
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.top.location.reload();", true);
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codProyecto;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Nuevo";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codProyecto;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Vista";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        #endregion
    }
}