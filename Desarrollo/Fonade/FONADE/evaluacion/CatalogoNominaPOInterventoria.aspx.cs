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
using Fonade.Negocio;
using System.Globalization;
using Fonade.Clases;

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoNominaPOInterventoria : Base_Page
    {
        String codProyecto;
        String codActividad;
        String NombreDeLaActividad; //Valor que debe enviarse cuando se crea una nuevo registro, se usa para la notificación en tareas.
        String CodUsuario;
        String CodGrupo;
        int CodCargo;
        String CodNomina;
        String MesDeLaNomina;
        String NombreDelCargo; //Este valor se le debe pasar por RowCommand del GridView donde se esté invocando.
        /// <summary>
        /// Tipo de financiación que tiene el producto seleccionado al ser actualizado; su valor es retornado por
        /// el método "ConsultarInformacion".
        /// </summary>
        int v_CodTipoFinanciacion = 0;
        Int32 txtTab = Constantes.CONST_SubPlanOperativo;
        String txtSQL;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Obtener la información almacenada en las variables de sesión.
                CodUsuario = usuario.IdContacto.ToString();
                codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
                codActividad = Session["CodActividad"] != null && !string.IsNullOrEmpty(Session["CodActividad"].ToString()) ? Session["CodActividad"].ToString() : "0";
                CodGrupo = Session["CodGrupo"] != null && !string.IsNullOrEmpty(Session["CodGrupo"].ToString()) ? Session["CodGrupo"].ToString() : "0";
                CodNomina = Session["CodNomina"] != null && !string.IsNullOrEmpty(Session["CodNomina"].ToString()) ? Session["CodNomina"].ToString() : "0";
                MesDeLaNomina = Session["MesDeLaNomina"] != null && !string.IsNullOrEmpty(Session["MesDeLaNomina"].ToString()) ? Session["MesDeLaNomina"].ToString() : "0";
                NombreDelCargo = Session["NombreDelCargo"] != null && !string.IsNullOrEmpty(Session["NombreDelCargo"].ToString()) ? Session["NombreDelCargo"].ToString() : "0";
                NombreDeLaActividad = Session["NombreDeLaActividad"] != null && !string.IsNullOrEmpty(Session["NombreDeLaActividad"].ToString()) ? Session["CodGrupo"].ToString() : "NOMBRE DE LA ACTIVIDAD";

                //Se ajusta el texto al valor correspondiente del mes seleccionado.
                lbl_tipoReq_Enunciado.Text = "REQUERIMIENTOS DE RECURSOS - MES " + MesDeLaNomina;

                if (Session["Accion"].ToString().Equals("crear") || Session["Accion"].ToString().Equals("Reportar"))
                {
                    B_Acion.Text = "Crear";
                    lbl_enunciado.Text = "NUEVO AVANCE";

                    if (Session["Accion"].ToString().Equals("Reportar"))
                    {
                        tr_mes.Style.Add(HtmlTextWriterStyle.Display, "none");
                        tr_cargo.Style.Add(HtmlTextWriterStyle.Display, "none");
                        tr_observ_inter.Style.Add(HtmlTextWriterStyle.Display, "none");
                        tr_act.Style.Add(HtmlTextWriterStyle.Display, "none");
                        img_btn_NuevoDocumento.Visible = true;
                    }
                }
                if (Session["Accion"].ToString().Equals("actualizar"))
                {
                    B_Acion.Text = "Actualizar";
                    lbl_enunciado.Text = "EDITAR AVANCE";

                    if (!IsPostBack)
                    {
                        ConsultarInformcion();
                        //Deshabilitar campos:
                        if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_Asesor || usuario.CodGrupo == Constantes.CONST_JefeUnidad || usuario.CodGrupo == Constantes.CONST_CallCenter)
                        {
                            txt_cargo.Enabled = false;
                            txt_mes.Enabled = false;
                            txt_sueldo_obtenido.Enabled = false;
                            txt_prestaciones_obtenidas.Enabled = false;
                            txt_observaciones.Enabled = false;
                        }
                        MostrarInterventor();
                    }
                }
                if (Session["Accion"].ToString().Equals("borrar")) { B_Acion.Text = "Borrar"; Borrar_ValoresProyectados(); }
                //ConsultarInformcionPlanOperativo();
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Mostrar interventor.
        /// </summary>
        private void MostrarInterventor()
        {
            String sql;

            sql = "SELECT Nombres + ' ' + Apellidos AS Nombre from Contacto where id_Contacto = " + usuario.IdContacto;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lbl_Interventor.Text = reader["Nombre"].ToString();
                    DateTime fecha = DateTime.Now;
                    string sMes = fecha.ToString("MMM", CultureInfo.CreateSpecificCulture("es-CO"));
                    lbl_tiempo.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year;
                }
                reader.Close();
                conn.Close();
            }
            catch (SqlException)
            { }
            finally
            { conn.Close(); }
        }

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
        /// Redirigir al usuario para observar la información de la grilla con archivos adjuntos "PDF's".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_btn_enlazar_grilla_PDF_Click(object sender, ImageClickEventArgs e)
        {
            CodNomina = Session["CodNomina"] != null && !string.IsNullOrEmpty(Session["CodNomina"].ToString()) ? Session["CodNomina"].ToString() : "0";
            MesDeLaNomina = Session["MesDeLaNomina"] != null && !string.IsNullOrEmpty(Session["MesDeLaNomina"].ToString()) ? Session["MesDeLaNomina"].ToString() : "0";
            Redirect(null, "../interventoria/CatalogoNominaInter.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }

        /// <summary>
        /// Redirigir al usuario para crear un nuevo documento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_btn_NuevoDocumento_Click(object sender, ImageClickEventArgs e)
        {
            CodNomina = Session["CodNomina"] != null && !string.IsNullOrEmpty(Session["CodNomina"].ToString()) ? Session["CodNomina"].ToString() : "0";
            MesDeLaNomina = Session["MesDeLaNomina"] != null && !string.IsNullOrEmpty(Session["MesDeLaNomina"].ToString()) ? Session["MesDeLaNomina"].ToString() : "0";
            Session["Accion_Docs"] = img_btn_NuevoDocumento.CommandName;
            Redirect(null, "../interventoria/CatalogoNominaInter.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }

        /// <summary>
        /// Determinar el tipo de acción a efectuar.
        /// Actualizar ó Crear.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_Acion_Click(object sender, EventArgs e)
        {
            if (B_Acion.Text.Equals("Crear")) almacenar(1);
            if (B_Acion.Text.Equals("Actualizar")) almacenar(2);
        }

        /// <summary>
        /// Almacenar la información.
        /// </summary>
        /// <param name="accion">Acción a efectuar, sólo puede tener el valor "1" = Guardar ó "2" = Actualizar.</param>
        private void almacenar(int accion)
        {
            if (accion == 1)
            {
                #region Guardar la información.

                GuardarInformacion();

                #endregion
            }
            if (accion == 2)
            {
                #region Actualizar la información.

                ActualizarInformacion();

                #endregion
            }
            else
            {
                return; //Valor no válido.
            }
        }

        /// <summary>
        /// Cerrar ventana emergente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_Cancelar_Click(object sender, EventArgs e)
        {
            RedirectPage(false, string.Empty, "cerrar");
        }

        /// <summary>
        /// Método que carga la información de los detalles dependiendo de la nómina seleccionada.
        /// La nómina está en sesión, por esto, este método no tiene parámetros en la firma.
        /// </summary>
        private void ConsultarInformcion()
        {
            //Obtiene la conexión
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            //Consultar la información.
            //String sqlConsulta = "SELECT * FROM AvanceCargoPOMes WHERE CodCargo = " + CodProyecto + " AND Mes = " + MesDeLaNomina + " ";
            String sqlConsulta = "SELECT * FROM AvanceCargoPOMes WHERE CodCargo = " + CodNomina + " AND Mes = " + MesDeLaNomina + " ";
            //Asignar SqlCommand para su ejecución.
            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //Aplicar los valores a los campos del formulario.
                    txt_mes.Text = reader["Mes"].ToString();
                    txt_cargo.Text = NombreDelCargo;
                    txt_observaciones.Text = reader["Observaciones"].ToString();
                    txt_observ_interventor.Text = reader["ObservacionesInterventor"].ToString();
                    string valor = reader["Aprobada"].ToString();
                    if (reader["Aprobada"].ToString() == "True") { dd_aprobado.Items[0].Selected = true; }
                    else { dd_aprobado.Items[1].Selected = true; }

                    //Evaluar el valor "CodTipoFinanciacion" para determinar los valores a mostrar en los campos de texto.
                    if (reader["CodTipoFinanciacion"].ToString().Equals("1")) //CodTipoFinanciacion
                    {
                        txt_sueldo_obtenido.Text = Convert.ToDouble(reader["Valor"].ToString()).ToString();
                        decimal valorTotal = Convert.ToDecimal(reader["Valor"].ToString());
                        lbl_Total.Text = "$" + valorTotal.ToString("0,0.00", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        txt_prestaciones_obtenidas.Text = Convert.ToDouble(reader["Valor"].ToString()).ToString();
                    }
                }
            }
            catch (Exception se) //SqlException
            {
                string h = se.Message;
                throw se;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Consultar la información del valor seleccionado.
        /// Este método SOLO debe aplicarse cuando sea para PLAN OPERATIVO.
        /// </summary>
        private void ConsultarInformcionPlanOperativo()
        {
            //Obtiene la conexión
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            //Consultar la información.
            //String sqlConsulta = "SELECT * FROM AvanceCargoPOMes WHERE CodCargo = " + CodProyecto + " AND Mes = " + MesDeLaNomina + " ";
            String sqlConsulta = "SELECT * FROM AvanceCargoPOMes WHERE CodCargo = " + CodNomina + " AND Mes = " + MesDeLaNomina + " ";
            //Asignar SqlCommand para su ejecución.
            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    v_CodTipoFinanciacion = Convert.ToInt32(reader["CodTipoFinanciacion"].ToString());

                    if (reader["Tipo"].ToString().Equals("1")) //CodTipoFinanciacion
                    {
                        //Aplicar los valores a los campos de texto.
                        txt_mes.Text = reader["Mes"].ToString();
                        //txt_cargo.Text = reader[""];
                        txt_observaciones.Text = reader["Observaciones"].ToString();
                        txt_observ_interventor.Text = reader["ObservacionesInterventor"].ToString();
                        if (reader["Aprobada"].ToString() == "1") { dd_aprobado.Items[0].Selected = true; }
                        else { dd_aprobado.Items[1].Selected = true; }
                        txt_sueldo_obtenido.Text = reader["Valor"].ToString();
                        txt_prestaciones_obtenidas.Text = reader["Valor"].ToString();
                        lbl_Total.Text = reader["Valor"].ToString();
                    }
                    else
                    {
                        //Aplicar los valores a los campos de texto.
                        txt_mes.Text = reader["Mes"].ToString();
                        txt_cargo.Text = NombreDelCargo;
                        txt_observaciones.Text = reader["Observaciones"].ToString();
                        txt_observ_interventor.Text = reader["ObservacionesInterventor"].ToString();
                        if (reader["Aprobada"].ToString() == "1") { dd_aprobado.Items[0].Selected = true; }
                        else { dd_aprobado.Items[1].Selected = true; }
                        txt_sueldo_obtenido.Text = reader["Valor"].ToString();
                        txt_prestaciones_obtenidas.Text = reader["Valor"].ToString();
                        lbl_Total.Text = reader["Valor"].ToString();
                    }
                }
            }
            catch (Exception se) //SqlException
            {
                string h = se.Message;
                throw se;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Guardar la información en la tabla AvanceCargoPOMes.
        /// </summary>
        private void GuardarInformacion()
        {
            //Inicializar variables.
            bool repetido = false;
            String sqlConsulta;
            DataTable tabla = new DataTable();
            int v_CodTipoFinanciacion = 0;
            int v_cod_nomina = Convert.ToInt32(codProyecto);
            DataTable tabla_gerente_coord_inter = new DataTable();
            DataTable tabla_inter = new DataTable();

            //Obtiene la conexión
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            #region Consulta SQL comentada.
            //Según el clásico, hace una consulta para validar que el registro no sea repetido.
            //sqlConsulta = "SELECT * FROM InterventorNomina WHERE Cargo = " + txt_cargo.Text.Trim() + 
            //              " AND CodProyecto = " + codProyecto + " AND Tipo <> '' "; 
            #endregion

            //Versión LINQ del SQL Anterior comentado.
            var sqldos = (from x in consultas.Db.InterventorNominas
                          where x.Cargo == txt_cargo.Text && x.CodProyecto == v_cod_nomina && x.Tipo != ""
                          select new { x.Id_Nomina, x.CodProyecto, x.Cargo, x.Tipo }).FirstOrDefault();

            if ((String.IsNullOrEmpty(sqldos.ToString())) || (sqldos.Id_Nomina == 0))
            {
                repetido = false;

                #region 1. Inserción de datos.

                if (v_CodTipoFinanciacion == 1)
                {
                    #region Inserción si el CodTipoFinanciacion = 1.

                    //Consulta.
                    sqlConsulta = "INSERT INTO AvanceCargoPOMes (CodCargo, Mes, CodTipoFinanciacion, Valor, Observaciones, CodContacto) " +
                                  "VALUES (" + sqldos.Id_Nomina + ", " + MesDeLaNomina + ", " + 1 + ", " + txt_sueldo_obtenido.Text + ", '" + txt_observ_interventor.Text + "', " + usuario.IdContacto + ")";

                    //Asignar SqlCommand para su ejecución.
                    SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

                    //Ejecutar SQL.
                    EjecutarSQL(connection, cmd);

                    #endregion
                }
                else
                {
                    #region Inserción si el CodTipoFinanciacion = 2.

                    //Consulta.
                    sqlConsulta = "INSERT INTO AvanceCargoPOMes (CodCargo, Mes, CodTipoFinanciacion, Valor, Observaciones, CodContacto) " +
                                  "VALUES (" + sqldos.Id_Nomina + ", " + MesDeLaNomina + ", " + 2 + ", " + txt_sueldo_obtenido.Text + ", '" + txt_observ_interventor.Text + "', " + usuario.IdContacto + ")";

                    //Asignar SqlCommand para su ejecución.
                    SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

                    //Ejecutar SQL.
                    EjecutarSQL(connection, cmd);

                    #endregion
                }

                #endregion

                #region 2. Notificación a los interventores del proyecto.

                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    #region Aplicar flujo a los grupos Interventores.

                    //Ejecutar consulta. "Aplicable si es para Interventor".
                    sqlConsulta = "SELECT DISTINCT CodContacto FROM ProyectoContacto WHERE CodProyecto = " + codProyecto + " AND CodRol in (1,2,3) AND Inactivo = 0 ";

                    //Llamado a evento que devuelve un DataTable.
                    tabla_inter = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Si el DataTable contiene datos, ejecutará el código que contiene.
                    if (tabla_inter.Rows.Count > 0)
                    {
                        //Inicializar listado para almacenar los códigos de los contactos obtenidos de la consulta.
                        List<int> ListaCodContactos = new List<int>();

                        //Añadir los contactos obtenidos.
                        for (int i = 0; i < tabla_inter.Rows.Count; i++)
                        {
                            int CodContacto_obtenido = int.Parse(tabla_inter.Rows[i]["CodContacto"].ToString());
                            ListaCodContactos.Add(CodContacto_obtenido);
                        }

                        #region Generar tareas a cada contacto obtenido del listado de contactos obtenidos.

                        try
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);
                            SqlDataReader reader = cmd.ExecuteReader();
                            //Inicializador "este valor se usa para incrementar cada vez que el while se ejecuta, obteniendo los valores del listado de contactos."
                            int incr = 0;

                            while (reader.Read())
                            {
                                //Crear tarea pendiente.
                                #region Versión de Diego.
                                //AgendarTarea agenda = new AgendarTarea
                                //    (reader["CodContacto"].ToString(),
                                //    "Revisar Actividad de Nómina. Se ha modificado una actividad.",
                                //    "Revisar la actividad de Nómina <b>" + sqldos.Cargo + "</b> del proyecto <b>" + codProyecto + "</b>",
                                //    codProyecto, 
                                //    2, 
                                //    "0", 
                                //    false, 
                                //    1, 
                                //    true, 
                                //    false, 
                                //    usuario.IdContacto,
                                //    ("CodProyecto=" + codProyecto), 
                                //    "", 
                                //    "Catálogo Nómina Plan Operativo Interventoría.");
                                //agenda.Agendar(); 
                                #endregion
                                TareaUsuario datoNuevo = new TareaUsuario();
                                datoNuevo.CodContacto = ListaCodContactos[incr];
                                datoNuevo.CodProyecto = Convert.ToInt32(codProyecto);
                                datoNuevo.NomTareaUsuario = "Revisar Actividad de Nómina. Se ha creado una actividad.";
                                datoNuevo.Descripcion = "Revisar la actividad de Nómina <b>" + NombreDeLaActividad /*NomActividad*/ + "</b> del proyecto <b>" + tabla_inter.Rows[0]["NomProyecto"].ToString() + " (" + codProyecto + ")</b>";
                                datoNuevo.CodTareaPrograma = 2;
                                datoNuevo.Recurrente = "0";
                                datoNuevo.RecordatorioEmail = true; //1 = true // 0 = false.
                                datoNuevo.NivelUrgencia = 1;
                                datoNuevo.RecordatorioPantalla = true;
                                datoNuevo.RequiereRespuesta = false;
                                datoNuevo.CodContactoAgendo = usuario.IdContacto;
                                datoNuevo.DocumentoRelacionado = "";
                                try
                                {
                                    Consultas consulta = new Consultas();
                                    consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
                                    incr++; //Incremento.
                                }
                                catch (Exception) { }
                            }
                        }
                        catch (Exception) { }
                        finally
                        {
                            connection.Close();
                        }

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    #region Aplicar flujo a los grupos de Gerente y Coordinador Interventor.

                    //Ejecutar consulta. "Aplicable si es para Gerente o Coordinador Interventor.
                    sqlConsulta = "SELECT EmpresaInterventor.CodContacto, Proyecto.NomProyecto" +
                                  "FROM EmpresaInterventor INNER JOIN Empresa " +
                                  "ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa INNER JOIN Proyecto " +
                                  "ON Empresa.codproyecto = Proyecto.Id_Proyecto" +
                                  "WHERE Empresa.codproyecto = " + codProyecto + " AND EmpresaInterventor.Inactivo = 0 ";

                    //Llamado a evento que devuelve un DataTable.
                    tabla_gerente_coord_inter = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Si el DataTable contiene datos, ejecutará el código que contiene.
                    if (tabla_gerente_coord_inter.Rows.Count > 0)
                    {
                        //Inicializar listado para almacenar los códigos de los contactos obtenidos de la consulta.
                        List<int> ListaCodContactos = new List<int>();

                        //Añadir los contactos obtenidos.
                        for (int i = 0; i < tabla_gerente_coord_inter.Rows.Count; i++)
                        {
                            int CodContacto_obtenido = int.Parse(tabla_inter.Rows[i]["CodContacto"].ToString());
                            ListaCodContactos.Add(CodContacto_obtenido);
                        }

                        #region Generar tareas a cada contacto obtenido del listado de contactos obtenidos.

                        try
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);
                            SqlDataReader reader = cmd.ExecuteReader();
                            //Inicializador "este valor se usa para incrementar cada vez que el while se ejecuta, obteniendo los valores del listado de contactos."
                            int incr = 0;

                            while (reader.Read())
                            {
                                //Crear tarea pendiente.
                                #region Versión de Diego.
                                //AgendarTarea agenda = new AgendarTarea
                                //    (reader["CodContacto"].ToString(),
                                //    "Revisar Actividad de Nómina. Se ha modificado una actividad.",
                                //    "Revisar la actividad de Nómina <b>" + sqldos.Cargo + "</b> del proyecto <b>" + codProyecto + "</b>",
                                //    codProyecto, 
                                //    2, 
                                //    "0", 
                                //    false, 
                                //    1, 
                                //    true, 
                                //    false, 
                                //    usuario.IdContacto,
                                //    ("CodProyecto=" + codProyecto), 
                                //    "", 
                                //    "Catálogo Nómina Plan Operativo Interventoría.");
                                //agenda.Agendar(); 
                                #endregion
                                TareaUsuario datoNuevo = new TareaUsuario();
                                datoNuevo.CodContacto = ListaCodContactos[incr];
                                datoNuevo.CodProyecto = Convert.ToInt32(codProyecto);
                                datoNuevo.NomTareaUsuario = "Revisar Actividad de Nómina. Se ha creado una actividad.";
                                datoNuevo.Descripcion = "Revisar la actividad de Nómina <b>" + NombreDeLaActividad /*NomActividad*/ + "</b> del proyecto <b>" + tabla_inter.Rows[0]["NomProyecto"].ToString() + " (" + codProyecto + ")</b>";
                                datoNuevo.CodTareaPrograma = 2;
                                datoNuevo.Recurrente = "0"; //"false";
                                datoNuevo.RecordatorioEmail = true;
                                datoNuevo.NivelUrgencia = 1;
                                datoNuevo.RecordatorioPantalla = true;
                                datoNuevo.RequiereRespuesta = false;
                                datoNuevo.CodContactoAgendo = usuario.IdContacto;
                                datoNuevo.DocumentoRelacionado = "";
                                try
                                {
                                    Consultas consulta = new Consultas();
                                    consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
                                    incr++; //Incremento.
                                }
                                catch (Exception) { }
                            }
                        }
                        catch (Exception) { }
                        finally
                        {
                            connection.Close();
                        }

                        #endregion
                    }

                    #endregion
                }

                #endregion

                //Si todo sale bien, vá a mostrar este mensaje.
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información de nómina creada correctamente.');window.opener.location.reload();window.close();", true);
                return;
            }
            else
            {
                repetido = true;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La nómina " + txt_cargo.Text.Trim() + "  ya existe.')", true);
                return;
            }
        }

        /// <summary>
        /// Actualizar la información de la tabla AvanceCargoPOMes.
        /// </summary>
        private void ActualizarInformacion()
        {
            //Inicializar variables.
            String sqlConsulta;
            DataTable tabla = new DataTable();
            int v_cod_nomina = Convert.ToInt32(CodNomina);
            int v_cod_proyecto = Convert.ToInt32(codProyecto);
            DataTable tabla_gerente_coord_inter = new DataTable();
            DataTable tabla_inter = new DataTable();

            //Obtiene la conexión
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            //Versión LINQ del SQL de FONADE Clásico para obtener el nombre del cargo.
            var sqldos = (from x in consultas.Db.InterventorNominas
                          where x.Id_Nomina == v_cod_nomina && x.CodProyecto == v_cod_proyecto
                          select new { x.Id_Nomina, x.CodProyecto, x.Cargo, x.Tipo }).FirstOrDefault();

            if (!(String.IsNullOrEmpty(sqldos.ToString()) || sqldos.Id_Nomina == 0))
            {
                //El nombre del cargo consulado está siendo procesado como "sqldos.Cargo".

                #region 1. Actualización de datos.

                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    //"Según el clásico, la tabla AvanceCargoPOMes" el CONST_Interventor, solo puede actualizar ciertos datos.

                    #region Trata la información para el Interventor.

                    //Consulta.
                    sqlConsulta = " UPDATE AvanceCargoPOMes " +
                                  " SET ObservacionesInterventor = '" + txt_observ_interventor.Text + "', " +
                                  " Aprobada = " + dd_aprobado.SelectedValue +
                                  " WHERE CodCargo = " + CodNomina + " AND Mes = " + MesDeLaNomina;

                    //Asignar SqlCommand para su ejecución.
                    SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

                    //Ejecutar SQL.
                    EjecutarSQL(connection, cmd);

                    #endregion
                }
                else
                {
                    #region Trata la información para los grupos de Gerente y Coordinador Interventor.

                    if (v_CodTipoFinanciacion == 1)
                    {
                        #region Actualización si el CodTipoFinanciacion = 1.

                        //Consulta.
                        sqlConsulta = " UPDATE AvanceCargoPOMes " +
                                      " SET Mes = " + MesDeLaNomina + " , " +
                                      " CodTipoFinanciacion = " + v_CodTipoFinanciacion + ", " +
                                      " Valor = " + txt_sueldo_obtenido.Text + ", " +
                                      " Observaciones = '" + txt_observaciones.Text + "', " +
                                      " CodContacto = " + usuario.IdContacto +
                                      " WHERE CodCargo = " + CodNomina + " AND Mes = " + MesDeLaNomina +
                                      " AND CodTipoFinanciacion = 1";

                        //Asignar SqlCommand para su ejecución.
                        SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

                        //Ejecutar SQL.
                        EjecutarSQL(connection, cmd);

                        #endregion
                    }
                    else
                    {
                        #region Actualización si el CodTipoFinanciacion = 2.

                        //Consulta.
                        sqlConsulta = " UPDATE AvanceCargoPOMes " +
                                      " SET Mes = " + MesDeLaNomina + " , " +
                                      " CodTipoFinanciacion = " + v_CodTipoFinanciacion + ", " +
                                      " Valor = " + txt_prestaciones_obtenidas.Text + ", " +
                                      " Observaciones = '" + txt_observaciones.Text + "', " +
                                      " CodContacto = " + usuario.IdContacto +
                                      " WHERE CodCargo = " + CodNomina + " AND Mes = " + MesDeLaNomina +
                                      " AND CodTipoFinanciacion = 2";

                        //Asignar SqlCommand para su ejecución.
                        SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

                        //Ejecutar SQL.
                        EjecutarSQL(connection, cmd);

                        #endregion
                    }

                    #endregion
                }

                #endregion

                #region 2. Notificación a los interventores del proyecto.

                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    #region Aplicar flujo a los grupos Interventores.

                    //Ejecutar consulta. "Aplicable si es para Interventor".
                    //sqlConsulta = "SELECT DISTINCT CodContacto FROM ProyectoContacto WHERE CodProyecto = " + codProyecto + " AND CodRol in (1,2,3) AND Inactivo = 0 ";
                    sqlConsulta = " SELECT EmpresaInterventor.CodContacto, Proyecto.NomProyecto " +
                                  " FROM EmpresaInterventor INNER JOIN Empresa " +
                                  " ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa INNER JOIN Proyecto " +
                                  " ON Empresa.codproyecto = Proyecto.Id_Proyecto " +
                                  " WHERE Empresa.codproyecto = " + codProyecto + " AND EmpresaInterventor.Inactivo = 0 ";

                    //Llamado a evento que devuelve un DataTable.
                    tabla_inter = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Si el DataTable contiene datos, ejecutará el código que contiene.
                    if (tabla_inter.Rows.Count > 0)
                    {
                        //Inicializar listado para almacenar los códigos de los contactos obtenidos de la consulta.
                        List<int> ListaCodContactos = new List<int>();

                        //Añadir los contactos obtenidos.
                        for (int i = 0; i < tabla_inter.Rows.Count; i++)
                        {
                            int CodContacto_obtenido = int.Parse(tabla_inter.Rows[i]["CodContacto"].ToString());
                            ListaCodContactos.Add(CodContacto_obtenido);
                        }

                        #region Generar tareas a cada contacto obtenido del listado de contactos obtenidos.

                        try
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);
                            SqlDataReader reader = cmd.ExecuteReader();
                            //Inicializador "este valor se usa para incrementar cada vez que el while se ejecuta, obteniendo los valores del listado de contactos."
                            int incr = 0;

                            while (reader.Read())
                            {
                                //Crear tarea pendiente.
                                #region Versión de Diego.
                                //AgendarTarea agenda = new AgendarTarea
                                //    (reader["CodContacto"].ToString(),
                                //    "Revisar Actividad de Nómina. Se ha modificado una actividad.",
                                //    "Revisar la actividad de Nómina <b>" + sqldos.Cargo + "</b> del proyecto <b>" + codProyecto + "</b>",
                                //    codProyecto, 
                                //    2, 
                                //    "0", 
                                //    false, 
                                //    1, 
                                //    true, 
                                //    false, 
                                //    usuario.IdContacto,
                                //    ("CodProyecto=" + codProyecto), 
                                //    "", 
                                //    "Catálogo Nómina Plan Operativo Interventoría.");
                                //agenda.Agendar(); 
                                #endregion
                                TareaUsuario datoNuevo = new TareaUsuario();
                                datoNuevo.CodContacto = ListaCodContactos[incr];
                                datoNuevo.CodProyecto = Convert.ToInt32(codProyecto);
                                datoNuevo.NomTareaUsuario = "Revisar Actividad de Nómina. Se ha modificado una actividad.";
                                datoNuevo.Descripcion = "Revisar la actividad de Nómina <b>" + sqldos.Cargo /*NomActividad*/ + "</b> del proyecto <b>" + tabla_inter.Rows[0]["NomProyecto"].ToString() + " (" + codProyecto + ")</b>";
                                datoNuevo.CodTareaPrograma = 2;
                                datoNuevo.Recurrente = "0";
                                datoNuevo.RecordatorioEmail = true; //1 = true // 0 = false.
                                datoNuevo.NivelUrgencia = 1;
                                datoNuevo.RecordatorioPantalla = true;
                                datoNuevo.RequiereRespuesta = false;
                                datoNuevo.CodContactoAgendo = usuario.IdContacto;
                                datoNuevo.DocumentoRelacionado = "";
                                try
                                {
                                    Consultas consulta = new Consultas();
                                    consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
                                    incr++; //Incremento.
                                }
                                catch (Exception) { }
                            }
                        }
                        catch (Exception) { }
                        finally
                        {
                            connection.Close();
                        }

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    #region Aplicar flujo a los grupos de Gerente y Coordinador Interventor.

                    //Ejecutar consulta. "Aplicable si es para Gerente o Coordinador Interventor.
                    sqlConsulta = " SELECT EmpresaInterventor.CodContacto, Proyecto.NomProyecto " +
                                  " FROM EmpresaInterventor INNER JOIN Empresa " +
                                  " ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa INNER JOIN Proyecto " +
                                  " ON Empresa.codproyecto = Proyecto.Id_Proyecto " +
                                  " WHERE Empresa.codproyecto = " + codProyecto + " AND EmpresaInterventor.Inactivo = 0 ";

                    //Llamado a evento que devuelve un DataTable.
                    tabla_gerente_coord_inter = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Si el DataTable contiene datos, ejecutará el código que contiene.
                    if (tabla_gerente_coord_inter.Rows.Count > 0)
                    {
                        //Inicializar listado para almacenar los códigos de los contactos obtenidos de la consulta.
                        List<int> ListaCodContactos = new List<int>();

                        //Añadir los contactos obtenidos.
                        for (int i = 0; i < tabla_gerente_coord_inter.Rows.Count; i++)
                        {
                            int CodContacto_obtenido = int.Parse(tabla_inter.Rows[i]["CodContacto"].ToString());
                            ListaCodContactos.Add(CodContacto_obtenido);
                        }

                        #region Generar tareas a cada contacto obtenido del listado de contactos obtenidos.

                        try
                        {
                            connection.Open();
                            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);
                            SqlDataReader reader = cmd.ExecuteReader();
                            //Inicializador "este valor se usa para incrementar cada vez que el while se ejecuta, obteniendo los valores del listado de contactos."
                            int incr = 0;

                            while (reader.Read())
                            {
                                //Crear tarea pendiente.
                                #region Versión de Diego.
                                //AgendarTarea agenda = new AgendarTarea
                                //    (reader["CodContacto"].ToString(),
                                //    "Revisar Actividad de Nómina. Se ha modificado una actividad.",
                                //    "Revisar la actividad de Nómina <b>" + sqldos.Cargo + "</b> del proyecto <b>" + codProyecto + "</b>",
                                //    codProyecto, 
                                //    2, 
                                //    "0", 
                                //    false, 
                                //    1, 
                                //    true, 
                                //    false, 
                                //    usuario.IdContacto,
                                //    ("CodProyecto=" + codProyecto), 
                                //    "", 
                                //    "Catálogo Nómina Plan Operativo Interventoría.");
                                //agenda.Agendar(); 
                                #endregion
                                TareaUsuario datoNuevo = new TareaUsuario();
                                datoNuevo.CodContacto = ListaCodContactos[incr];
                                datoNuevo.CodProyecto = Convert.ToInt32(codProyecto);
                                datoNuevo.NomTareaUsuario = "Revisar Actividad de Nómina. Se ha modificado una actividad.";
                                datoNuevo.Descripcion = "Revisar la actividad de Nómina <b>" + sqldos.Cargo /*NomActividad*/ + "</b> del proyecto <b>" + tabla_inter.Rows[0]["NomProyecto"].ToString() + " (" + codProyecto + ")</b>";
                                datoNuevo.CodTareaPrograma = 2;
                                datoNuevo.Recurrente = "0"; //"false";
                                datoNuevo.RecordatorioEmail = true;
                                datoNuevo.NivelUrgencia = 1;
                                datoNuevo.RecordatorioPantalla = true;
                                datoNuevo.RequiereRespuesta = false;
                                datoNuevo.CodContactoAgendo = usuario.IdContacto;
                                datoNuevo.DocumentoRelacionado = "";
                                try
                                {
                                    Consultas consulta = new Consultas();
                                    consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
                                    incr++; //Incremento.
                                }
                                catch (Exception) { }
                            }
                        }
                        catch (Exception) { }
                        finally
                        {
                            connection.Close();
                        }

                        #endregion
                    }

                    #endregion
                }

                #endregion

                //Si todo sale bien, vá a mostrar este mensaje.
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información de nómina actualizada correctamente.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/09/2014.
        /// Eliminar el valor proyectado "según FONADE clásico, se activa cuando se selecciona (Eliminar Avance)".
        /// </summary>
        private void Borrar_ValoresProyectados()
        {
            try
            {
                //Borrar los valores proyectados por mes de la actividad
                txtSQL = "Delete from AvanceCargoPOMes where CodCargo=" + CodCargo + " and mes=" + MesDeLaNomina;
                ejecutaReader(txtSQL, 2);

                //Actualizar la fecha de modificación del tab
                prActualizarTab(txtTab.ToString(), codProyecto.ToString());

                //MOstarr mensaje y cerrar.
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Avance eliminado correctamente.'); window.opener.location.reload();window.close();", true);
            }
            catch { }
        }
    }
}

#region Consultas.

/*
         
         Costos:
         SELECT CodTipoFinanciacion, mes, valor 
         FROM AvanceCargoPOMes 
         WHERE CodCargo = CodNomina AND Mes = MesDeLaNomina 
         ORDER BY CodTipoFinanciacion, Mes"
         
         Si la Acción es = Borrar (que NO se vé el botón en el clásico):
         DELETE FROM AvanceCargoPOMes WHERE CodCargo = CodNomina AND Mes = MesDeLaNomina
         
         
         */

#endregion