#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>07 - 03 - 2014</Fecha>
// <Archivo>EvaluacionAspectos.aspx.cs</Archivo>

#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Negocio;
using System.Linq;
using System.Data;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class EvaluacionAspectos : Base_Page
    {
        #region Propiedades

        public int CodAspecto { get; set; }
        public int CodConvocatoria { get; set; }
        public int CodProyecto { get; set; }
        public int Puntaje { get; set; }
        public int total { get; set; }
        public int tab { get; set; }
        public string campo { get; set; }
        public Boolean esMiembro;
        /// <summary>
        /// Determina si "está" o "no" realizado...
        /// </summary>
        public Boolean bRealizado;
        /// <summary>
        /// Variable que debe cambiarse.
        /// </summary>
        String Accion2 = "CAMBIAR!";

        #endregion

        #region Metodos Evaluacion

        protected void Page_Load(object sender, EventArgs e)
        {
            CodAspecto = Convert.ToInt16(Request["codAspecto"]);
            string mensajeDeError = string.Empty;
            CodProyecto = !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) && Session["CodProyecto"] != null ? Convert.ToInt32(Session["CodProyecto"].ToString()) : 0;
            CodConvocatoria = !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) && Session["CodConvocatoria"] != null ? Convert.ToInt32(Session["CodConvocatoria"].ToString()) : 0;
            loadConstantes();

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, CodProyecto.ToString());

            //Consultar si está "realizado".
            bRealizado = esRealizado(tab.ToString(), CodProyecto.ToString(), "");

            if (esMiembro && !bRealizado) { this.div_Post_It.Visible = true; }

            if (esMiembro && !bRealizado && usuario.CodGrupo == Constantes.CONST_Evaluador && Accion2 != "Todos")
            { update.Visible = true; }

            if (!IsPostBack)
            {
                //chedk1.tabCod = tab;
                //chedk1.CodProyecto = CodProyecto.ToString();
                Post_It._txtCampo = campo;
                inicioEncabezado(CodProyecto.ToString(), CodConvocatoria.ToString(), 1);
                LoadDatosEvaluacionAspectos();
                ObtenerDatosUltimaActualizacion();
            }
        }

        public void loadConstantes()
        {
            // validamos el tipo de entrada si es por generales, medio ambiente etc
            switch (CodAspecto)
            {
                case 1:
                    tab = Constantes.ConstSubGenerales;
                    campo = "tablaGenerales";
                    break;
                case 2:
                    tab = Constantes.ConstSubComerciales;
                    campo = "tablaComerciales";
                    break;
                case 3:
                    tab = Constantes.ConstSubTecnicos;
                    campo = "tablaTecnicos";
                    break;
                case 4:
                    tab = Constantes.ConstSubOrganizacionales;
                    campo = "tablaOrganizacionales";
                    break;
                case 5:
                    tab = Constantes.ConstSubFinancieros;
                    campo = "tablaFinancieros";
                    break;
                case 6:
                    tab = Constantes.ConstSubMedioAmbiente;
                    campo = "tablaMedioAmbiente";
                    break;
                default:
                    tab = Constantes.ConstSubAspectosEvaluados;
                    campo = "AspectosEvaluados";
                    break;
            }
        }

        public void LoadDatosEvaluacionAspectos()
        {
            string mensajeDeError = string.Empty;

            if (CodAspecto != 0)
            {
                var result = consultas.ObtenerCamposEvaluacionObservaciones(CodProyecto, CodConvocatoria, CodAspecto, ref mensajeDeError);

                if (string.IsNullOrEmpty(mensajeDeError))
                {
                    DltEvaluacion.DataSource = result;
                    DltEvaluacion.DataBind();
                }
            }
        }

        public List<MD_ObtenerCamposEvaluacionObservacionesHijasResult> LoadDatosEvaluacionAspectos(string orden)
        {
            string mensajeDeError = string.Empty;
            List<MD_ObtenerCamposEvaluacionObservacionesHijasResult> result = null;

            if (CodAspecto != 0)
            {
                result = consultas.ObtenerCamposEvaluacionObservacionesHijas(orden, CodProyecto, CodConvocatoria, CodAspecto, ref mensajeDeError);

            }
            return result;
        }

        public void cargarcombo(ref DropDownList combo)
        {

        }

        public void DisabledControls()
        {
            if (CodAspecto != 6)
            {
                imagen1.Style.Add("display", "block");
                imagen2.Style.Add("display", "none");
            }
            else
            {
                imagen1.Style.Add("display", "none");
                imagen2.Style.Add("display", "block");


            }
        }

        public string GetObservation(int idcampo)
        {
            string observation = string.Empty;

            var query = consultas.Db.EvaluacionCampoJustificacions.FirstOrDefault(
                e => e.CodProyecto == CodProyecto
                     && e.CodConvocatoria == CodConvocatoria
                     && e.CodCampo == idcampo);
            if (query != null && !string.IsNullOrEmpty(query.Justificacion))
            {
                observation = query.Justificacion;
            }

            return observation;
        }

        #endregion

        protected void DltEvaluacion_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var campoid = e.Item.FindControl("campoid") as Label;

                var dtlHijos = e.Item.FindControl("DtlHijos") as DataList;

                if (CodAspecto != 6)
                {
                    if (dtlHijos != null)
                    {
                        if (campoid != null)
                            dtlHijos.DataSource = LoadDatosEvaluacionAspectos(campoid.Text);
                        dtlHijos.DataBind();
                    }
                }
                else
                {
                    DltEvaluacion.ShowHeader = false;
                    DltEvaluacion.ShowFooter = false;

                    if (dtlHijos != null)
                    {
                        if (campoid != null)
                            dtlHijos.DataSource = LoadDatosEvaluacionAspectos(campoid.Text);
                        dtlHijos.DataBind();
                    }
                }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var lpuntaje = e.Item.FindControl("lpuntajeObtenido") as Label;

                if (CodAspecto != 6)
                {
                    if (lpuntaje != null) lpuntaje.Text = Convert.ToString(Puntaje);
                }
                else
                {
                    if (lpuntaje != null) lpuntaje.Visible = false;
                }
            }
        }

        protected void DtlHijos_ItemDataBound(object sender, DataListItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var idcampo = e.Item.Parent.Parent.FindControl("campoid") as Label;
                var observaciones = e.Item.Parent.Parent.FindControl("txtobservaciones") as TextBox;
                var asignado = e.Item.FindControl("lAsignado") as Label;
                var maximo = e.Item.FindControl("lblmaximo") as Label;
                var ddl = e.Item.FindControl("Ddlpuntaje") as DropDownList;
                var ddl_1 = e.Item.FindControl("lbl_Ddlpuntaje") as Label;
                var ddlMedio = e.Item.FindControl("DdlpuntajeMedio") as DropDownList;
                var ddlMedio_1 = e.Item.FindControl("lbl_DdlpuntajeMedio") as Label;


                if (CodAspecto == 6)
                {
                    maximo.Visible = false;
                    ddl.Visible = false;
                    ddlMedio.Visible = true;

                    ddlMedio.Items.Insert(1, new ListItem("SI", maximo.Text));
                    if (asignado.Text != null && asignado.Text.Equals("0"))
                    {
                        ddlMedio.SelectedValue = asignado.Text;
                        ddlMedio_1.Text = asignado.Text;

                        if (usuario.CodGrupo == Constantes.CONST_GerenteEvaluador || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador)
                        {
                            ddlMedio.Visible = false;
                            ddlMedio_1.Visible = true;
                        }
                    }
                    if (asignado.Text != null && !asignado.Text.Equals("0"))
                    {
                        ddlMedio.SelectedValue = maximo.Text;
                        ddlMedio_1.Text = ddlMedio.SelectedItem.Text; //maximo.Text;

                        if (usuario.CodGrupo == Constantes.CONST_GerenteEvaluador || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador)
                        {
                            ddlMedio.Visible = false;
                            ddlMedio_1.Visible = true;
                        }
                    }
                    if (idcampo != null && !string.IsNullOrEmpty(idcampo.Text))
                    {
                        if (observaciones != null)
                        {
                            observaciones.Text = GetObservation(Convert.ToInt32(idcampo.Text)); observaciones.MaxLength = 1000;
                            if (!(Accion2 != "Todos" && esMiembro && !bRealizado && usuario.CodGrupo == Constantes.CONST_Evaluador))
                            { observaciones.Enabled = false; }
                        }
                    }
                }
                else
                {
                    if (idcampo != null && !string.IsNullOrEmpty(idcampo.Text))
                    {
                        if (observaciones != null)
                        {
                            observaciones.Text = GetObservation(Convert.ToInt32(idcampo.Text)); observaciones.MaxLength = 1000;
                            if (!(Accion2 != "Todos" && esMiembro && !bRealizado && usuario.CodGrupo == Constantes.CONST_Evaluador))
                            { observaciones.Enabled = false; }
                        }
                    }

                    ddl.Items.Insert(1, new ListItem(maximo.Text, maximo.Text));

                    if (asignado.Text != null && asignado.Text.Equals("0"))
                    {
                        ddl.SelectedValue = asignado.Text;
                        ddl_1.Text = asignado.Text;

                        if (usuario.CodGrupo == Constantes.CONST_GerenteEvaluador || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador)
                        {
                            ddl.Visible = false;
                            ddl_1.Visible = true;
                        }

                    }
                    if (asignado.Text != null && !asignado.Text.Equals("0"))
                    {
                        ddl.SelectedValue = maximo.Text;
                        ddl_1.Text = asignado.Text;

                        if (usuario.CodGrupo == Constantes.CONST_GerenteEvaluador || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador)
                        {
                            ddl.Visible = false;
                            ddl_1.Visible = true;
                        }
                    }
                }
                if (asignado.Text != null)
                {
                    Puntaje += !string.IsNullOrEmpty(asignado.Text) ? Convert.ToInt32(asignado.Text) : 0;
                }
            }
        }

        protected void update_Click(object sender, EventArgs e)
        {
            string resultado = "";

            resultado = Validar();

            if (resultado == "")
            {
                ActualizarEvaluacion();
                LoadDatosEvaluacionAspectos();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + resultado + "');", true);
                return;
            }
        }

        public void ActualizarEvaluacion()
        {
            foreach (DataListItem item in DltEvaluacion.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Footer)
                {
                    var observaciones = item.FindControl("txtobservaciones") as TextBox;
                    var campoid = item.FindControl("campoid") as Label;
                    var dtlHijos = item.FindControl("DtlHijos") as DataList;

                    if (dtlHijos != null)
                    {
                        foreach (DataListItem itemhijos in dtlHijos.Items)
                        {
                            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                            {
                                var evaluacionCampos = new EvaluacionCampo();

                                var idcampo = itemhijos.FindControl("idcampo") as Label;

                                var ddl = itemhijos.FindControl("Ddlpuntaje") as DropDownList;

                                if (ddl != null && ddl.Visible)
                                {
                                    if (idcampo != null)
                                    {
                                        evaluacionCampos.Puntaje = Convert.ToInt16(ddl.SelectedValue);
                                        evaluacionCampos.codCampo = Convert.ToInt16(idcampo.Text);
                                        consultarItems(evaluacionCampos);
                                    }
                                }

                                var ddlMedio = itemhijos.FindControl("DdlpuntajeMedio") as DropDownList;
                                if (ddlMedio != null && ddlMedio.Visible)
                                {
                                    evaluacionCampos.Puntaje = Convert.ToInt16(ddlMedio.SelectedValue);
                                    evaluacionCampos.codCampo = Convert.ToInt16(idcampo.Text);
                                    consultarItems(evaluacionCampos);
                                }
                            }
                        }
                    }
                    var evaluacion = new EvaluacionCampoJustificacion();
                    if (observaciones != null)
                    {
                        evaluacion.Justificacion = observaciones.Text;
                    }

                    if (campoid != null && !string.IsNullOrEmpty(campoid.Text))
                    {
                        evaluacion.CodCampo = Convert.ToInt16(campoid.Text);
                    }

                    crearcampoJustificacion(evaluacion);
                }
            }
        }

        /// <summary>
        /// Consultar ítems...
        /// Actualiza el puntaje "si la consulta es correcta" o ingresa la entidad "si ésta no existe".
        /// </summary>
        /// <param name="entidad">Entidad a consultar / consultar</param>
        public void consultarItems(EvaluacionCampo entidad)
        {
            try
            {
                //Obtener los valores de las variables de sesión.
                CodAspecto = Convert.ToInt16(Request["codAspecto"]);
                CodProyecto = !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) && Session["CodProyecto"] != null ? Convert.ToInt32(Session["CodProyecto"].ToString()) : 0;
                CodConvocatoria = !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) && Session["CodConvocatoria"] != null ? Convert.ToInt32(Session["CodConvocatoria"].ToString()) : 0;

                if (entidad.codCampo != 0)
                {
                    var query = consultas.Db.EvaluacionCampos.FirstOrDefault(e => e.codProyecto == CodProyecto &&
                                                                                  e.codConvocatoria == CodConvocatoria &&
                                                                                  e.codCampo == entidad.codCampo);
                    if (query != null && query.codCampo != 0)
                    {
                        query.Puntaje = entidad.Puntaje;
                        consultas.Db.SubmitChanges();
                        //Actualizar fecha de modificación
                        prActualizarTabEval(tab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());
                        ObtenerDatosUltimaActualizacion();
                    }
                    else
                    {
                        entidad.codProyecto = CodProyecto;
                        entidad.codConvocatoria = CodConvocatoria;
                        consultas.Db.EvaluacionCampos.InsertOnSubmit(entidad);
                        consultas.Db.SubmitChanges();
                        //Actualizar fecha de modificación
                        prActualizarTabEval(tab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());
                        ObtenerDatosUltimaActualizacion();
                    }
                }
            }
            catch (Exception) { throw new Exception("Error"); }
        }

        /// <summary>
        /// Crea la entidad o actualiza la justificación de la misma.
        /// </summary>
        /// <param name="entitdad">Entidad a consultar / procesar.</param>
        public void crearcampoJustificacion(EvaluacionCampoJustificacion entitdad)
        {
            //Obtener las variables de sesión.
            CodAspecto = Convert.ToInt16(Request["codAspecto"]);
            CodProyecto = !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) && Session["CodProyecto"] != null ? Convert.ToInt32(Session["CodProyecto"].ToString()) : 0;
            CodConvocatoria = !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) && Session["CodConvocatoria"] != null ? Convert.ToInt32(Session["CodConvocatoria"].ToString()) : 0;

            try
            {
                var query = consultas.Db.EvaluacionCampoJustificacions.FirstOrDefault(c => c.CodProyecto == CodProyecto &&
                                                                                   c.CodConvocatoria == CodConvocatoria &&
                                                                                   c.CodCampo == entitdad.CodCampo);

                if (query != null && query.CodCampo != 0)
                {
                    query.Justificacion = entitdad.Justificacion;
                    consultas.Db.SubmitChanges();
                    //Actualizar fecha de modificación
                    prActualizarTabEval(tab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());
                    ObtenerDatosUltimaActualizacion();
                }
                else
                {
                    entitdad.CodProyecto = CodProyecto;
                    entitdad.CodConvocatoria = CodConvocatoria;
                    consultas.Db.EvaluacionCampoJustificacions.InsertOnSubmit(entitdad);
                    consultas.Db.SubmitChanges();
                    //Actualizar fecha de modificación
                    prActualizarTabEval(tab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());
                    ObtenerDatosUltimaActualizacion();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
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
                bNuevo = es_bNuevo(CodProyecto.ToString());

                //Determinar si "está en acta".
                bEnActa = es_EnActa(CodProyecto.ToString(), CodConvocatoria.ToString());

                //Consultar si es "Miembro".
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, CodProyecto.ToString());

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(tab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + CodProyecto + " And CodContacto = " + usuario.IdContacto +
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
                         " where id_contacto = codcontacto and codtabEvaluacion = " + tab +
                         " and codproyecto = " + CodProyecto +
                         " and codconvocatoria = " + CodConvocatoria;

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
                //if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                { chk_realizado.Enabled = false; }

                //if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString() && lbl_nombre_user_ult_act.Text.Trim() != "" && CodigoEstado == Constantes.CONST_Evaluacion && (!bEnActa))
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
                        && tu.CodProyecto == CodProyecto
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
        { prActualizarTabEval(tab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString()); Marcar(tab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString(), chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

        /// <summary>
        /// Validar si el campo de "Observaciones" se encuentra con demasiados caracteres.
        /// </summary>
        /// <returns>String vacío = Puede continuar. // String con datos = Mostrar mensaje.</returns>
        private string Validar()
        {
            string resultado = "";

            foreach (DataListItem item in DltEvaluacion.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Footer)
                {
                    var observaciones = item.FindControl("txtobservaciones") as TextBox;
                    var label_obsv = item.FindControl("Label2") as Label; //Label que contiene el "Orden" de los campos de "Observaciones".

                    if (observaciones.Text.Trim().Length == 0)
                    { if (label_obsv != null) { resultado = "La observiación en la sección " + label_obsv.Text + " es requerida."; break; } else { resultado = Texto("TXT_OBSERVACION_REQ"); break; } }
                    if (observaciones.Text.Trim().Length > 1000)
                    { if (label_obsv != null) { resultado = "El texto de la observación en la sección " + label_obsv.Text + " es demasiado largo"; break; } else { resultado = "El texto es demasiado largo"; break; } }
                }
            }
            return resultado;
        }

        #endregion
    }
}