#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>05 - 03 - 2014</Fecha>
// <Archivo>EvaluacionObservaciones.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Linq;
using System.Web.Services;
using Datos;
using Fonade.Negocio;
using Newtonsoft.Json;
using System.Data;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class EvaluacionObservaciones : Base_Page
    {
        #region Propiedades

        public static string Codproyecto, Codconvocatoria;
        String txtSQL;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si "está" o "no" realizado...
        /// </summary>
        public Boolean bRealizado;

        #endregion

        #region Metodos

        /// <summary>
        /// Mauricio Arias Olave.
        /// 09/05/2014.
        /// Se declaran variables de sesión para establecer correctamente la información
        /// de la última actualización realizada y establecer check del CheckBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Session["CodProyecto"].ToString()))
                {
                    Codproyecto = Session["CodProyecto"].ToString();
                }
                if (!string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()))
                {
                    Codconvocatoria = Session["CodConvocatoria"].ToString();
                }

                //Consultar si es miembro.
                esMiembro = fnMiembroProyecto(usuario.IdContacto, Codproyecto);

                //Consultar si está "realizado".
                bRealizado = esRealizado(Constantes.ConstSubObservaciones.ToString(), Codproyecto, "");

                if (esMiembro && !bRealizado)
                {
                    this.div_Post_It.Visible = true;
                    this.div_Post_It0.Visible = true;
                    this.div_Post_It1.Visible = true;
                    this.div_Post_It2.Visible = true;
                    this.div_Post_It3.Visible = true;
                    this.div_Post_It4.Visible = true;
                    this.div_Post_It5.Visible = true;
                }

                if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !realizado && Codconvocatoria == "")
                { }
                else if (esMiembro && usuario.CodGrupo == Constantes.CONST_Evaluador && !bRealizado && Codconvocatoria != "")
                { }
                else
                {
                    Actividades.Enabled = false;
                    ProductosServicios.Enabled = false;
                    EstrategiaMercado.Enabled = false;
                    ProcesoProduccion.Enabled = false;
                    EstructuraOrganizacionalEval.Enabled = false;
                    TamanioLocalizacion.Enabled = false;
                    Generales.Enabled = false;
                }

                if (esMiembro && usuario.CodGrupo == Constantes.CONST_Evaluador && !bRealizado) { btnGuardar.Visible = true; }

                //Crear variable de sesión con el valor del tab para mostrar la información de la última actualización.
                Session["sTab"] = Datos.Constantes.ConstSubObservaciones;

                //Enviar el rol del usuario.
                Session["RolUsuario"] = usuario.CodGrupo;

                //Enviar el Id del usuario.
                Session["IdUsuario"] = usuario.IdContacto;

                inicioEncabezado(Codproyecto, Codconvocatoria, Constantes.ConstSubObservaciones);
                //Permisos();
                cargarobservacion();
                //CtrlCheckedProyecto1.CodProyecto = Codproyecto;
                //CtrlCheckedProyecto1.tabCod = Constantes.ConstSubObservaciones;

                ObtenerDatosUltimaActualizacion();
            }
        }

        public void Permisos()
        {
            if (miembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !realizado
                && string.IsNullOrEmpty(Codconvocatoria))
            {
                DisabledTextbox(string.Empty);

            }
            else if (miembro && usuario.CodGrupo == Constantes.CONST_Evaluador && !realizado &&
                     !string.IsNullOrEmpty(Codconvocatoria))
            {
                DisabledTextbox(string.Empty);
            }
            else
            {
                DisabledTextbox("readonly");
            }
        }

        private void DisabledTextbox(string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                Actividades.Enabled = false;
                ProductosServicios.Enabled = true;
                EstrategiaMercado.Enabled = true;
                ProcesoProduccion.Enabled = true;
                EstructuraOrganizacionalEval.Enabled = true;
                TamanioLocalizacion.Enabled = true;
                Generales.Enabled = true;
            }
            else
            {
                Actividades.Enabled = true;
                ProductosServicios.Enabled = false;
                EstrategiaMercado.Enabled = false;
                ProcesoProduccion.Enabled = false;
                EstructuraOrganizacionalEval.Enabled = false;
                TamanioLocalizacion.Enabled = false;
                Generales.Enabled = false;
            }
        }

        public void cargarobservacion()
        {
            txtSQL = " SELECT * FROM EvaluacionObservacion WHERE codproyecto = " + Codproyecto + " AND codConvocatoria = " + Codconvocatoria;

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            if (dt.Rows.Count > 0)
            {
                Actividades.Text = dt.Rows[0]["Actividades"].ToString();
                ProductosServicios.Text = dt.Rows[0]["ProductosServicios"].ToString();
                EstrategiaMercado.Text = dt.Rows[0]["EstrategiaMercado"].ToString();
                ProcesoProduccion.Text = dt.Rows[0]["ProcesoProduccion"].ToString();
                EstructuraOrganizacionalEval.Text = dt.Rows[0]["EstructuraOrganizacional"].ToString();
                TamanioLocalizacion.Text = dt.Rows[0]["TamanioLocalizacion"].ToString();
                Generales.Text = dt.Rows[0]["Generales"].ToString();
            }

            dt = null;
            txtSQL = "";
        }

        [WebMethod]
        public static string Guardar(EvaluacionObservacion entidad)
        {
            string mensajeDeError = string.Empty;

            try
            {
                var consulta = new Consultas();
                var evaluacionO = new EvaluacionObservacion();

                var count = consulta.Db.EvaluacionObservacions.FirstOrDefault(
                    e =>
                    e.CodProyecto == Convert.ToInt64(Codproyecto) &&
                    e.CodConvocatoria == Convert.ToInt64(Codconvocatoria));

                if (count != null && count.CodProyecto == 0)
                {
                    evaluacionO.CodProyecto = Convert.ToInt32(Codproyecto);
                    evaluacionO.CodConvocatoria = Convert.ToInt32(Codconvocatoria);
                    evaluacionO.Actividades = entidad.Actividades;
                    evaluacionO.ProductosServicios = entidad.ProductosServicios;
                    evaluacionO.EstrategiaMercado = entidad.EstrategiaMercado;
                    evaluacionO.ProcesoProduccion = entidad.ProcesoProduccion;
                    evaluacionO.EstructuraOrganizacional = entidad.EstructuraOrganizacional;
                    evaluacionO.TamanioLocalizacion = entidad.TamanioLocalizacion;
                    evaluacionO.Generales = entidad.Generales;
                    consulta.Db.EvaluacionObservacions.InsertOnSubmit(evaluacionO);
                    consulta.Db.SubmitChanges();
                }
                else
                {
                    if (count != null)
                    {
                        count.Actividades = entidad.Actividades;
                        count.ProductosServicios = entidad.ProductosServicios;
                        count.EstrategiaMercado = entidad.EstrategiaMercado;
                        count.ProcesoProduccion = entidad.ProcesoProduccion;
                        count.EstructuraOrganizacional = entidad.EstructuraOrganizacional;
                        count.TamanioLocalizacion = entidad.TamanioLocalizacion;
                        count.Generales = entidad.Generales;
                    }
                    consulta.Db.SubmitChanges();

                }
            }
            catch (Exception ex)
            {
                mensajeDeError = string.Format("Error de tipo: {0}", ex.Message);
            }
            return JsonConvert.SerializeObject(new
                                                   {
                                                       mensaje = mensajeDeError
                                                   });
        }

        //        If Request("Accion") = "Guardar" Then

        //    txtSQL = "SELECT Count(*) FROM EvaluacionObservacion WHERE codproyecto="&CodProyecto&" AND codConvocatoria="&codConvocatoria

        //    Set RS = Conn.execute(txtSQL)
        //    If rs(0) = 0 Then
        //        txtSQL=	"INSERT INTO EvaluacionObservacion (CodProyecto, codConvocatoria, "&_
        //                    "Actividades, ProductosServicios, EstrategiaMercado, ProcesoProduccion, EstructuraOrganizacional, TamanioLocalizacion, Generales)"&_
        //                "VALUES  ("&CodProyecto&","&codConvocatoria&" , '"& txtActividades &"','"& txtProductosServicios &"', '"& txtEstrategiaMercado &"', "&_
        //                    "'"& txtProcesoProduccion &"','"&  txtEstructuraOrganizacional &"','"& txtTamanioLocalizacion &"', '"&txtGenerales&"')"
        //    Else
        //        txtSQL=	"UPDATE EvaluacionObservacion "&_
        //                "SET Actividades = '"& txtActividades &"', "&_
        //                    "ProductosServicios = '"& txtProductosServicios &"', "&_
        //                    "EstrategiaMercado = '"& txtEstrategiaMercado &"', "&_
        //                    "ProcesoProduccion = '"& txtProcesoProduccion &"', "&_
        //                    "EstructuraOrganizacional = '"&  txtEstructuraOrganizacional &"', "&_
        //                    "TamanioLocalizacion = '"& txtTamanioLocalizacion &"', "&_
        //                    "Generales = '"& txtGenerales &"' "&_
        //                "WHERE codproyecto="&CodProyecto&" AND codConvocatoria="&codConvocatoria
        //    end If
        //    'response.write(txtSQL)	
        //    Conn.Execute(txtSQL)

        //    'Actualizar fecha modificación del tab
        //    prActualizarTabEval txtTab, CodProyecto, codConvocatoria
        //Else

        //    txtSQL = "SELECT * FROM EvaluacionObservacion WHERE codproyecto="&CodProyecto&" AND codConvocatoria="&codConvocatoria
        //    Set RS = Conn.execute(txtSQL)

        //    If not rs.eof Then
        //        txtActividades = RS("Actividades")
        //        txtProductosServicios = RS("ProductosServicios")
        //        txtEstrategiaMercado = RS("EstrategiaMercado")
        //        txtProcesoProduccion = RS("ProcesoProduccion")
        //        txtEstructuraOrganizacional = RS("EstructuraOrganizacional")
        //        txtTamanioLocalizacion = RS("TamanioLocalizacion")		
        //        txtGenerales = RS("Generales")

        //    End If	
        //End If

        //rs.close
        //Set rs = nothing

        /// <summary>
        /// Guardar o actualizar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            DataTable rs = new DataTable();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String correcto = "";

            #region Guardar.

            txtSQL = " SELECT Count(*) AS Conteo FROM EvaluacionObservacion WHERE codproyecto = " + Codproyecto +
                     " AND codConvocatoria =" + Codconvocatoria;

            rs = consultas.ObtenerDataTable(txtSQL, "text");

            if (rs.Rows.Count > 0)
            {
                if (Int32.Parse(rs.Rows[0]["Conteo"].ToString()) == 0)
                {
                    txtSQL = " INSERT INTO EvaluacionObservacion (CodProyecto, codConvocatoria, " +
                             " Actividades, ProductosServicios, EstrategiaMercado, ProcesoProduccion, " +
                             " EstructuraOrganizacional, TamanioLocalizacion, Generales) " +
                             " VALUES (" + Codproyecto + "," + Codconvocatoria + " , '" + Actividades.Text.Trim() + "','" + ProductosServicios.Text.Trim() + "', '" + EstrategiaMercado.Text.Trim() + "', " +
                             " '" + ProcesoProduccion.Text.Trim() + "','" + EstructuraOrganizacionalEval.Text.Trim() + "','" + TamanioLocalizacion.Text.Trim() + "', '" + Generales.Text.Trim() + "')";
                }
                else
                {
                    txtSQL = " UPDATE EvaluacionObservacion " +
                             " SET Actividades = '" + Actividades.Text.Trim() + "', " +
                             " ProductosServicios = '" + ProductosServicios.Text.Trim() + "', " +
                             " EstrategiaMercado = '" + EstrategiaMercado.Text.Trim() + "', " +
                             " ProcesoProduccion = '" + ProcesoProduccion.Text.Trim() + "', " +
                             " EstructuraOrganizacional = '" + EstructuraOrganizacionalEval.Text.Trim() + "', " +
                             " TamanioLocalizacion = '" + TamanioLocalizacion.Text.Trim() + "', " +
                             " Generales = '" + Generales.Text.Trim() + "' " +
                             " WHERE codproyecto=" + Codproyecto + " AND codConvocatoria = " + Codconvocatoria;
                }
            }

            if (txtSQL != "")
            {
                //Ejecutar consulta.
                cmd = new SqlCommand(txtSQL, conn);
                correcto = String_EjecutarSQL(conn, cmd);

                if (correcto != "") { }

                //Actualizar fecha modificación del tab
                prActualizarTabEval(Constantes.ConstSubObservaciones.ToString(), Codproyecto, Codconvocatoria);

                //Refrescar data.
                ObtenerDatosUltimaActualizacion();
            }

            //Actualizar.
            ObtenerDatosUltimaActualizacion();

            #endregion
        }

        #endregion

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
                bNuevo = es_bNuevo(Codproyecto.ToString());

                //Determinar si "está en acta".
                bEnActa = es_EnActa(Codproyecto.ToString(), Codconvocatoria.ToString());

                //Consultar si es "Miembro".
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, Codproyecto.ToString());

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(Constantes.ConstSubObservaciones.ToString(), Codproyecto, Codconvocatoria);

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + Codproyecto + " And CodContacto = " + usuario.IdContacto +
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
                         " where id_contacto = codcontacto and codtabEvaluacion = " + Constantes.ConstSubObservaciones +
                         " and codproyecto = " + Codproyecto +
                         " and codconvocatoria = " + Codconvocatoria;

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
                if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                { chk_realizado.Enabled = false; }

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
                        && tu.CodProyecto == Convert.ToInt32(Codproyecto)
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
            prActualizarTabEval(Constantes.ConstSubObservaciones.ToString(), Codproyecto, Codconvocatoria); Marcar(Constantes.ConstSubObservaciones.ToString(), Codproyecto, Codconvocatoria, chk_realizado.Checked); ObtenerDatosUltimaActualizacion();
        }

        #endregion
    }
}