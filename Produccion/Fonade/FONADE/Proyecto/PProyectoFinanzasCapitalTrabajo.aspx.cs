using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Globalization;

namespace Fonade.FONADE.Proyecto
{
    public partial class PProyectoFinanzasCapitalTrabajo : Negocio.Base_Page
    {
        public string codProyecto;
        public int txtTab = Constantes.CONST_CapitalTrabajo;
        public string idCapitalEdita;
        public string accion;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si está o no "realizado"...
        /// </summary>
        public Boolean bRealizado;

        protected void Page_Load(object sender, EventArgs e)
        {
            codProyecto = Session["codProyecto"] != null && !string.IsNullOrEmpty(Session["codProyecto"].ToString()) ? Session["codProyecto"].ToString() : "";

            if (Request.QueryString["Accion"] == "Editar")
            {
                accion = Request.QueryString["Accion"].ToString();
                idCapitalEdita = Request.QueryString["idCapital"].ToString();
            }

            inicioEncabezado(codProyecto, "", txtTab);

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codProyecto, "");

            //if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_AdministradorFonade)
            if (esMiembro && !bRealizado)//!chk_realizado.Checked)
            { this.div_Post_It_2.Visible = true; }

            if (miembro == true && usuario.CodGrupo == Constantes.CONST_Emprendedor && realizado == false)
                pnlAdicionar.Visible = true;

            if (!IsPostBack)
            {
                ObtenerDatosUltimaActualizacion();

                if (accion == "Editar")
                {
                    CargarControlesEditarCapital(codProyecto, idCapitalEdita);
                }
                CargarGridCapitalTrabajo();
            }
        }

        protected void CargarGridCapitalTrabajo()
        {

            var query = (from p in consultas.Db.ProyectoCapitals
                         where p.CodProyecto == Convert.ToInt32(codProyecto)
                         orderby p.Componente
                         select new { p.Id_Capital, p.Componente, p.Valor, p.Observacion }
                         );

            DataTable datos = new DataTable();
            datos.Columns.Add("CodProyecto");
            datos.Columns.Add("id_Capital");
            datos.Columns.Add("componente");
            datos.Columns.Add("valor");
            datos.Columns.Add("observacion");

            double total = 0;
            foreach (var item in query)
            {

                DataRow dr = datos.NewRow();
                dr["CodProyecto"] = codProyecto;
                dr["id_Capital"] = item.Id_Capital;
                dr["componente"] = item.Componente;
                dr["valor"] = item.Valor.ToString("0,0.00", CultureInfo.InvariantCulture); ;
                dr["observacion"] = item.Observacion;
                total += Convert.ToDouble(item.Valor);
                datos.Rows.Add(dr);

            }

            DataRow drTotal = datos.NewRow();
            drTotal["componente"] = "Total";
            drTotal["valor"] = total.ToString("0,0.00", CultureInfo.InvariantCulture);
            datos.Rows.Add(drTotal);

            gw_CapitalTrabajo.DataSource = datos;
            gw_CapitalTrabajo.DataBind();

            for (int i = 0; i < gw_CapitalTrabajo.Rows.Count; i++)
            {
                if (gw_CapitalTrabajo.Rows[i].Cells[3].Text != "&nbsp;" && gw_CapitalTrabajo.Rows[i].Cells[1].Text != "Total")
                {
                    if (miembro == true && usuario.CodGrupo == Constantes.CONST_Emprendedor && realizado == false)
                    {
                        string id_Capital = gw_CapitalTrabajo.DataKeys[i].Value.ToString();
                        gw_CapitalTrabajo.Rows[i].Cells[1].Text = "<a href='PProyectoFinanzasCapitalTrabajo.aspx?CodProyecto=" + codProyecto + "&idCapital=" + id_Capital + "&Accion=Editar'>" + gw_CapitalTrabajo.Rows[i].Cells[1].Text + "</a>";
                    }
                    else
                    {
                        gw_CapitalTrabajo.Rows[i].Cells[0].Text = "";
                    }
                }
                else
                {
                    gw_CapitalTrabajo.Rows[i].Cells[0].Text = "";
                    gw_CapitalTrabajo.Rows[i].Cells[1].Text = "<span class='TitulosRegistrosGrilla'>" + gw_CapitalTrabajo.Rows[i].Cells[1].Text + "</span>";
                }
            }
        }

        protected void gw_CapitalTrabajo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Borrar")
            {
                string idCapital = e.CommandArgument.ToString();
                consultas.Db.ExecuteCommand("Delete from ProyectoCapital where Id_Capital={0}", Convert.ToInt32(idCapital));
                ObtenerDatosUltimaActualizacion();
                CargarGridCapitalTrabajo();
            }
        }

        protected void btnAdicionarCapitalTrabajo_Click(object sender, EventArgs e)
        {
            pnlCapitalTrabajo.Visible = false;
            pnlCrearCapitalTrabajo.Visible = true;
            btnCrearCapitalTrabajo.Text = "Crear";
        }

        protected void btnCancelarNuevoCapital_Click(object sender, EventArgs e)
        {

            if (accion == "Editar")
            {
                Response.Redirect("PProyectoFinanzasCapitalTrabajo.aspx?CodProyecto=" + codProyecto + "");
            }
            else
            {
                CargarGridCapitalTrabajo();
                pnlCapitalTrabajo.Visible = true;
                pnlCrearCapitalTrabajo.Visible = false;
            }

        }

        protected void btnCrearCapitalTrabajo_Click(object sender, EventArgs e)
        {

            if (accion == "Editar")
            {

                var query = (from p in consultas.Db.ProyectoCapitals
                             where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                             p.Id_Capital == Convert.ToInt32(idCapitalEdita)
                             select p
                      ).First();

                query.Componente = txtComponente.Text;
                query.Valor = Convert.ToDecimal(txtValor.Text);
                query.Observacion = txtObservacioin.Text;
                consultas.Db.SubmitChanges();
                ObtenerDatosUltimaActualizacion();
                Response.Redirect("PProyectoFinanzasCapitalTrabajo.aspx?CodProyecto=" + codProyecto + "");

            }
            else
            {

                Datos.ProyectoCapital datosNuevos = new ProyectoCapital()
                {
                    CodProyecto = Convert.ToInt32(codProyecto),
                    Componente = txtComponente.Text,
                    Valor = Convert.ToDecimal(txtValor.Text),
                    Observacion = txtObservacioin.Text
                };
                consultas.Db.ProyectoCapitals.InsertOnSubmit(datosNuevos);

                consultas.Db.SubmitChanges();
                ObtenerDatosUltimaActualizacion();
                Response.Redirect("PProyectoFinanzasCapitalTrabajo.aspx?CodProyecto=" + codProyecto + "");
            }
        }

        private void CargarControlesEditarCapital(string codProyecto, string idInversionEdita)
        {

            pnlCapitalTrabajo.Visible = false;
            pnlCrearCapitalTrabajo.Visible = true;

            var query = (from p in consultas.Db.ProyectoCapitals
                         where p.CodProyecto == Convert.ToInt32(codProyecto) &&
                         p.Id_Capital == Convert.ToInt32(idCapitalEdita)
                         select p
                             ).First();

            txtComponente.Text = query.Componente;
            txtValor.Text = ((int)query.Valor).ToString();
            txtObservacioin.Text = query.Observacion.ToString();
            btnCrearCapitalTrabajo.Text = "Actualizar";
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
                CodigoEstado = CodEstado_Proyecto(txtTab.ToString(), codProyecto, "");//CodConvocatoria

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