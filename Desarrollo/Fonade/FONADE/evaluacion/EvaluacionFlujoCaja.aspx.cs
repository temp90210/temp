using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.evaluacion
{
    public partial class EvaluacionFlujoCaja : Negocio.Base_Page
    {
        #region Variables globales.

        private String codProyecto;
        private String codConvocatoria;
        private Int32 numPeriodos;
        private String txtConclusionesFinancieras;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si "está" o "no" realizado...
        /// </summary>
        public Boolean bRealizado;
        Table tabla;
        /// <summary>
        /// Esta variable se debe cambiar para que se aplique correctamente los cambios.
        /// </summary>
        String Accion = "CAMBIAR!";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(Session["codProyecto"].ToString()))
                {
                    codProyecto = Session["CodProyecto"].ToString();
                    codConvocatoria = Session["CodConvocatoria"].ToString();
                }
            }
            catch (Exception) { }

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

            //Consultar si está "realizado".
            bRealizado = esRealizado(Constantes.ConstSubFlujoCaja.ToString(), codProyecto, "");

            //if (esMiembro && !bRealizado)
            if (!(esMiembro && !bRealizado))
            { this.div_Post_It1.Visible = true; }

            if (esMiembro && !bRealizado && usuario.CodGrupo == Constantes.CONST_Evaluador && Accion != "Todos")
            { TB_Conclusiones.Visible = true; B_Registar.Visible = true; }
            else
            { this.div_conclusiones.Visible = true; }

            //CtrlCheckedProyecto1.tabCod = Constantes.ConstSubFlujoCaja;
            //CtrlCheckedProyecto1.CodProyecto = codProyecto;
            DatosPron();
            if (!IsPostBack)
            {
                llenarPanelFlujo();
                TB_Conclusiones.Text = txtConclusionesFinancieras;
                this.div_conclusiones.InnerText = txtConclusionesFinancieras;

                Session["P_TablaValE"] = tabla;
                ObtenerDatosUltimaActualizacion();
            }

            try
            {
                tabla = new Table();
                tabla = (Table)Session["P_TablaValE"];
                P_FlujoCaja.Controls.Add(tabla);
            }
            catch (Exception) { }
        }

        private void DatosPron()
        {
            try
            {
                DataTable Anexos = new DataTable();

                Consultas consulta = new Consultas();

                String sql = "SELECT TiempoProyeccion FROM EvaluacionObservacion WHERE CodProyecto = " + codProyecto + " and codconvocatoria=" + codConvocatoria;

                Anexos = consulta.ObtenerDataTable(sql, "text");

                try
                {
                    if (Anexos.Rows.Count > 0)
                    { if (!String.IsNullOrEmpty(Anexos.Rows[0]["TiempoProyeccion"].ToString())) { numPeriodos = Int32.Parse(Anexos.Rows[0]["TiempoProyeccion"].ToString()); } else { numPeriodos = 0; } }
                    else { numPeriodos = 0; }
                }
                catch { numPeriodos = 0; }

                sql = "SELECT ConclusionesFinancieras FROM evaluacionobservacion WHERE CodProyecto = " + codProyecto + " AND CodConvocatoria = " + codConvocatoria;

                Anexos = consulta.ObtenerDataTable(sql, "text");

                txtConclusionesFinancieras = Anexos.Rows[0]["ConclusionesFinancieras"].ToString();
            }
            catch { }
        }

        private void llenarPanelFlujo()
        {
            DataTable Anexos = new DataTable();
            Consultas consulta = new Consultas();
            String sql = "SELECT Id_EvaluacionRubroProyecto, Descripcion FROM EvaluacionRubroProyecto WHERE codProyecto = " + codProyecto + " AND codConvocatoria = " + codConvocatoria;
            Anexos = consulta.ObtenerDataTable(sql, "text");

            tabla = new Table();
            tabla.CssClass = "Grilla";

            TableHeaderRow headerrow = new TableHeaderRow();

            headerrow.Cells.Add(crearceladtitulo("Rubro / Periodo", 1, 1, ""));

            for (int i = 0; i < numPeriodos; i++)
            {
                headerrow.Cells.Add(crearceladtitulo("" + (i + 1), 1, 1, ""));
                headerrow.Width = 78;
                headerrow.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
            }
            tabla.Rows.Add(headerrow);

            for (int i = 0; i < Anexos.Rows.Count; i++)
            {
                TableRow filaTabla = new TableRow();

                filaTabla.Cells.Add(celdaNormal(Anexos.Rows[i]["Descripcion"].ToString(), 1, 1, "1", "Des" + Anexos.Rows[i]["Id_EvaluacionRubroProyecto"].ToString()));

                for (int j = 0; j < numPeriodos; j++)
                {
                    sql = "SELECT Valor FROM EvaluacionRubroValor WHERE CodEvaluacionRubroProyecto = " + Anexos.Rows[i]["Id_EvaluacionRubroProyecto"].ToString() + " AND Periodo = " + (j + 1);
                    DataTable ValorData = new DataTable();
                    ValorData = consulta.ObtenerDataTable(sql, "text");

                    try
                    {
                        filaTabla.Cells.Add(celdaNormal(ValorData.Rows[0]["Valor"].ToString(), 1, 1, "", "val;" + (i + 1) + ";" + (j + 1)));
                    }
                    catch (IndexOutOfRangeException)
                    {
                        filaTabla.Cells.Add(celdaNormal("0", 1, 1, "", "val;" + (i + 1) + ";" + (j + 1)));
                    }
                }
                tabla.Rows.Add(filaTabla);
            }
        }

        private TableHeaderCell crearceladtitulo(String mensaje, Int32 colspan, Int32 rowspan, String cssestilo)
        {
            TableHeaderCell celda1 = new TableHeaderCell();
            celda1.ColumnSpan = colspan;
            celda1.RowSpan = rowspan;
            celda1.CssClass = cssestilo;

            Label titulo1 = new Label();
            titulo1.Text = mensaje;
            celda1.Controls.Add(titulo1);

            return celda1;
        }

        private TableCell celdaNormal(String mensaje, Int32 colspan, Int32 rowspan, String cssestilo, String id)
        {
            TableCell celda1 = new TableCell();
            celda1.ColumnSpan = colspan;
            celda1.RowSpan = rowspan;
            celda1.CssClass = cssestilo;

            if (cssestilo.Equals("1"))
            {
                Label titulo1 = new Label();
                titulo1.Text = mensaje;
                titulo1.Width = 50;
                titulo1.ID = id;
                celda1.Controls.Add(titulo1);
            }
            else
            {
                TextBox titulo1 = new TextBox();
                titulo1.Text = mensaje;
                titulo1.Width = 50;
                titulo1.ID = id;
                titulo1.TextChanged += TB_Conclusiones_TextChanged;
                if (esMiembro && !bRealizado && usuario.CodGrupo == Constantes.CONST_Evaluador && Accion != "Todos")
                { titulo1.Enabled = true; }
                else { titulo1.Enabled = false; }
                celda1.Controls.Add(titulo1);
            }

            return celda1;
        }

        protected void B_Registar_Click(object sender, EventArgs e)
        {
            #region Código de inserción/actualización.

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd;

            tabla = new Table();
            tabla = (Table)Session["P_TablaValE"];
            String sql = "SELECT Id_EvaluacionRubroProyecto, Descripcion FROM EvaluacionRubroProyecto WHERE codProyecto = " + codProyecto + " AND codConvocatoria = " + codConvocatoria;

            DataTable Anexos = new DataTable();

            Consultas consulta = new Consultas();

            Anexos = consulta.ObtenerDataTable(sql, "text");

            for (int i = 0; i < Anexos.Rows.Count; i++)
            {
                for (int j = 0; j < numPeriodos; j++)
                {
                    sql = "SELECT Valor FROM EvaluacionRubroValor WHERE CodEvaluacionRubroProyecto = " + Anexos.Rows[i]["Id_EvaluacionRubroProyecto"].ToString() + " AND Periodo = " + (j + 1);
                    DataTable ValorData = new DataTable();
                    ValorData = consulta.ObtenerDataTable(sql, "text");

                    TextBox valor = new TextBox();

                    foreach (Control celda in tabla.Rows[i + 1].Cells[j + 1].Controls)
                    {
                        if (celda is TextBox)
                        {
                            valor = (TextBox)celda;
                        }
                    }

                    String txtSQL;

                    if (ValorData.Rows.Count > 0)
                    {
                        txtSQL = "UPDATE EvaluacionRubroValor SET Valor = " + valor.Text + " WHERE CodEvaluacionRubroProyecto = " + Anexos.Rows[i]["Id_EvaluacionRubroProyecto"].ToString() + " AND Periodo = " + (j + 1);
                    }
                    else
                    {
                        txtSQL = "INSERT INTO EvaluacionRubroValor (CodEvaluacionRubroProyecto, Periodo, Valor) VALUES(" + Anexos.Rows[i]["Id_EvaluacionRubroProyecto"].ToString() + "," + (j + 1) + "," + valor.Text + ")";
                    }


                    cmd = new SqlCommand(txtSQL, conn);
                    try
                    {

                        conn.Open();
                        cmd.ExecuteReader();
                        conn.Close();
                    }
                    catch (SqlException se)
                    {
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            sql = "SELECT ConclusionesFinancieras FROM evaluacionobservacion WHERE CodProyecto = " + codProyecto + " AND CodConvocatoria = " + codConvocatoria;
            Anexos = consulta.ObtenerDataTable(sql, "text");

            if (Anexos.Rows.Count > 0)
            {
                sql = "UPDATE evaluacionobservacion SET ConclusionesFinancieras = '" + TB_Conclusiones.Text + "' WHERE CodProyecto = " + codProyecto + " AND CodConvocatoria = " + codConvocatoria;
            }
            else
            {
                sql = "INSERT INTO evaluacionobservacion (CodProyecto,CodConvocatoria,Actividades,ProductosServicios,EstrategiaMercado,ProcesoProduccion,EstructuraOrganizacional,TamanioLocalizacion,Generales,ConclusionesFinancieras) Values(" + codProyecto + "," + codConvocatoria + ",'','','','','','','','" + TB_Conclusiones.Text + "')";
            }

            cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }

            #endregion

            prActualizarTabEval(Constantes.ConstSubFlujoCaja.ToString(), codProyecto, codConvocatoria);
            ObtenerDatosUltimaActualizacion();
        }

        protected void TB_Conclusiones_TextChanged(object sender, EventArgs e)
        {
            try
            {
                tabla = new Table();
                tabla = (Table)Session["P_TablaValE"];
                //P_FlujoCaja.Controls.Add(tabla);
            }
            catch { }
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
                bNuevo = es_bNuevo(codProyecto.ToString());

                //Determinar si "está en acta".
                bEnActa = es_EnActa(codProyecto.ToString(), codConvocatoria.ToString());

                //Consultar si es "Miembro".
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto.ToString());

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(Constantes.ConstSubFlujoCaja.ToString(), codProyecto, codConvocatoria);

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + codProyecto + " And CodContacto = " + usuario.IdContacto +
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
                         " where id_contacto = codcontacto and codtabEvaluacion = " + Constantes.ConstSubFlujoCaja +
                         " and codproyecto = " + codProyecto +
                         " and codconvocatoria = " + codConvocatoria;

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
        { prActualizarTabEval(Constantes.ConstSubFlujoCaja.ToString(), codProyecto, codConvocatoria); Marcar(Constantes.ConstSubFlujoCaja.ToString(), codProyecto, codConvocatoria, chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

        #endregion
    }
}