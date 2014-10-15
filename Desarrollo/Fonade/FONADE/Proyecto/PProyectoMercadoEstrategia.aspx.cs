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
    public partial class PProyectoMercadoEstrategia : Negocio.Base_Page
    {
        public string codProyecto;
        public int txtTab = Constantes.CONST_EstrategiasMercado;
        public string codConvocatoria;
        public ProyectoMercadoEstrategia pme;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si está o no "realizado"...
        /// </summary>
        public Boolean bRealizado;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codProyecto, "");

            if (esMiembro && !bRealizado)
            {
                this.div_post_it_1.Visible = true;
                this.div_post_it_2.Visible = true;
                this.div_post_it_3.Visible = true;
                this.div_post_it_4.Visible = true;
                this.div_post_it_5.Visible = true;
                this.div_post_it_6.Visible = true;
                this.div_post_it_7.Visible = true;
                this.div_post_it_8.Visible = true;
            }

            codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            codConvocatoria = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["CodConvocatoria"].ToString() : "0";

            inicioEncabezado(codProyecto, codConvocatoria, txtTab);
            //Obtenemos el ProyectoMercadoProyeccionVenta
            pme = getProyectoMercadoEstrategia();

            if (pme == null)
            { /* Response.Redirect("~/Default.aspx"); O lanzar error */ return; }

            if (!IsPostBack)
            { definirCampos(); ObtenerDatosUltimaActualizacion(); }
        }

        private void definirCampos()
        {
            //si encuentra el proyecto mercado investigacion
            if (pme != null)
            {
                procesarCampo(ref txt_conceptoP, ref txt_conceptoP_HtmlEditorExtender, ref panel_conceptoP, pme.ConceptoProducto, esMiembro, bRealizado, "");
                procesarCampo(ref txt_estrategiaDist, ref txt_estrategiaDist_HtmlEditorExtender, ref panel_estrategiaDist, pme.EstrategiasDistribucion, esMiembro, bRealizado, "");
                procesarCampo(ref txt_estrategiaPrecio, ref txt_estrategiaPrecio_HtmlEditorExtender, ref panel_estrategiaPrecio, pme.EstrategiasPrecio, esMiembro, bRealizado, "");
                procesarCampo(ref txt_estrategiaPromo, ref txt_estrategiaPromo_HtmlEditorExtender, ref panel_estrategiaPromo, pme.EstrategiasPromocion, esMiembro, bRealizado, "");
                procesarCampo(ref txt_estrategiaCom, ref txt_estrategiaCom_HtmlEditorExtender, ref panel_estrategiaCom, pme.EstrategiasComunicacion, esMiembro, bRealizado, "");

                procesarCampo(ref txt_estrategiaServ, ref txt_estrategiaServ_HtmlEditorExtender, ref panel_estrategiaServ, pme.EstrategiasServicio, esMiembro, bRealizado, "");
                procesarCampo(ref txt_presupuestoM, ref txt_presupuestoM_HtmlEditorExtender, ref panel_presupuestoM, pme.PresupuestoMercado, esMiembro, bRealizado, "");
                procesarCampo(ref txt_estrategiaApr, ref txt_estrategiaApr_HtmlEditorExtender, ref panel_estrategiaApr, pme.EstrategiasAprovisionamiento, esMiembro, bRealizado, "");
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

        private ProyectoMercadoEstrategia getProyectoMercadoEstrategia()
        {
            var query = from pmi in consultas.Db.ProyectoMercadoEstrategias
                        where pmi.CodProyecto == Convert.ToInt32(codProyecto)
                        select pmi;

            return query.FirstOrDefault();

        }

        protected void btn_limpiarCampos_Click(object sender, EventArgs e)
        {

        }

        protected void btm_guardarCambios_Click(object sender, EventArgs e)
        {
            //Miramos si ya esxist el registro
            var query = from p in consultas.Db.ProyectoMercadoEstrategias
                        where p.CodProyecto == Convert.ToInt32(codProyecto)
                        select p;
            if (query.Count() > 0)
            {
                ProyectoMercadoEstrategia pmv = new ProyectoMercadoEstrategia()
                {
                    CodProyecto = Convert.ToInt32(codProyecto),
                    ConceptoProducto = txt_conceptoP.Text,
                    EstrategiasDistribucion = txt_estrategiaDist.Text,
                    EstrategiasPrecio = txt_estrategiaPrecio.Text,
                    EstrategiasPromocion = txt_estrategiaPromo.Text,
                    EstrategiasComunicacion = txt_estrategiaCom.Text,
                    EstrategiasServicio = txt_estrategiaServ.Text,
                    EstrategiasAprovisionamiento = txt_estrategiaApr.Text
                };
                consultas.Db.ProyectoMercadoEstrategias.InsertOnSubmit(pmv);
                //Actualizar fecha modificación del tab.
                prActualizarTab(txtTab.ToString(), codProyecto);
                ObtenerDatosUltimaActualizacion();
            }
            else
            {
                pme.ConceptoProducto = txt_conceptoP.Text;
                pme.EstrategiasDistribucion = txt_estrategiaDist.Text;
                pme.EstrategiasPrecio = txt_estrategiaPrecio.Text;
                pme.EstrategiasPromocion = txt_estrategiaPromo.Text;
                pme.EstrategiasComunicacion = txt_estrategiaCom.Text;
                pme.EstrategiasServicio = txt_estrategiaServ.Text;
                pme.EstrategiasAprovisionamiento = txt_estrategiaApr.Text;
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
                //if (!(EsMiembro && numPostIt == 0 && ((usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && usuario.CodGrupo == Constantes.CONST_RolEvaluador && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                if (!(EsMiembro && numPostIt == 0 && ((Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                { chk_realizado.Enabled = false; }

                //Mostrar el botón de guardar.
                //if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (usuario.CodGrupo == Constantes.CONST_RolEvaluador && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
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
        { prActualizarTab(txtTab.ToString(), codProyecto.ToString()); Marcar(txtTab.ToString(), codProyecto, "", chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

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