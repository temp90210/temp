using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fonade.Negocio;
using System.Globalization;
using System.Data;

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoVentasPOInterventoria : Base_Page
    {
        //DIANA
        public String codProyecto;
        String NombreDeLaActividad; //Valor que debe enviarse cuando se crea una nuevo registro, se usa para la notificación en tareas.
        String CodUsuario;
        String CodGrupo;
        String CodProduccion;
        String MesDelProducto_Venta_Seleccionado;
        String NombreDelProductoSeleccionado; //Este valor se le debe pasar por RowCommand del GridView donde se esté invocando.
        /// <summary>
        /// Tipo de financiación que tiene el producto seleccionado al ser actualizado; su valor es retornado por
        /// el método "ConsultarInformacion".
        /// </summary>
        int v_CodTipoFinanciacion = 0;
        bool txtDeshabilitar;
        String txtSQL;
        Int32 txtTab = Constantes.CONST_SubPlanOperativo;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Modificado por Diana//
                //Obtener la información almacenada en las variables de sesión.
                CodUsuario = usuario.IdContacto.ToString();
                codProyecto = Session["proyecto"].ToString();
                CodGrupo = Session["CodGrupo"] != null && !string.IsNullOrEmpty(Session["CodGrupo"].ToString()) ? Session["CodGrupo"].ToString() : "0";
                CodProduccion = Session["CodProduccion"] != null && !string.IsNullOrEmpty(Session["CodProduccion"].ToString()) ? Session["CodProduccion"].ToString() : "0";
                MesDelProducto_Venta_Seleccionado = Session["MesDelProducto_Venta_Seleccionado"] != null && !string.IsNullOrEmpty(Session["MesDelProducto_Venta_Seleccionado"].ToString()) ? Session["MesDelProducto_Venta_Seleccionado"].ToString() : "0";
                NombreDelProductoSeleccionado = Session["NombreDelProductoSeleccionado"] != null && !string.IsNullOrEmpty(Session["NombreDelProductoSeleccionado"].ToString()) ? Session["NombreDelProductoSeleccionado"].ToString() : "0";
                NombreDeLaActividad = Session["NombreDeLaActividad"] != null && !string.IsNullOrEmpty(Session["NombreDeLaActividad"].ToString()) ? Session["CodGrupo"].ToString() : "NOMBRE DE LA ACTIVIDAD";

                #region Des/habilitar campos...

                if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_Asesor || usuario.CodGrupo == Constantes.CONST_JefeUnidad || usuario.CodGrupo == Constantes.CONST_CallCenter || dd_aprobado.Items[0].Selected)
                {
                    txtDeshabilitar = false;
                    if (dd_aprobado.Items[0].Selected) { txtDeshabilitar = false; }
                }

                txt_mes.Enabled = txtDeshabilitar;
                txt_producto_servicio.Enabled = txtDeshabilitar;
                txt_observ_interventor.Enabled = txtDeshabilitar;

                #endregion

                //Se ajusta el texto al valor correspondiente del mes seleccionado.
                lbl_tipoReq_Enunciado.Text = "REQUERIMIENTOS DE RECURSOS - MES " + MesDelProducto_Venta_Seleccionado;

                if (Session["Accion"].ToString().Equals("crear") || Session["Accion"].ToString().Equals("Nuevo"))
                {
                    B_Acion.Text = "Crear";
                    lbl_enunciado.Text = "NUEVO AVANCE";
                    img_btn_NuevoDocumento.Visible = true;
                    tr_mes.Visible = false;
                    tr_producto.Visible = false;
                    tr_obser_inter.Visible = false;
                    tr_act_aprobada.Visible = false;
                }
                if (Session["Accion"].ToString().Equals("actualizar"))
                {
                    B_Acion.Text = "Actualizar"; 
                    lbl_enunciado.Text = "EDITAR AVANCE";

                    if (!IsPostBack)
                    {
                        ConsultarInformcion();

                        //Mostrar el botón de "Nuevo Documento si está en edición y cumple estas condiciones.
                        if (usuario.CodGrupo != Constantes.CONST_Interventor && usuario.CodGrupo != Constantes.CONST_Asesor && usuario.CodGrupo != Constantes.CONST_JefeUnidad && usuario.CodGrupo != Constantes.CONST_CallCenter)
                        { img_btn_NuevoDocumento.Visible = true; dd_aprobado.Enabled = false; }

                        //Deshabilitar campos:
                        if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_Asesor || usuario.CodGrupo == Constantes.CONST_JefeUnidad || usuario.CodGrupo == Constantes.CONST_CallCenter)
                        {
                            txt_producto_servicio.Enabled = false;
                            txt_mes.Enabled = false;
                            txt_ingreso_obtenido.Enabled = false;
                            txt_ventas_obtenidas.Enabled = false;
                            txt_observaciones.Enabled = false;
                        }
                        MostrarInterventor();
                    }
                }
                if (Session["Accion"].ToString().Equals("borrar"))
                { B_Acion.Text = "Borrar"; Borrar_ValoresProyectados(); }
            }
            catch (Exception) { }
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
            String sqlConsulta = "SELECT * FROM AvanceVentasPOMes WHERE CodProducto = " + Convert.ToInt32(CodProduccion) + " AND Mes = " + MesDelProducto_Venta_Seleccionado + " ";

            //Asignar SqlCommand para su ejecución.
            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                #region Comentarios anteriores.
                //while (reader.Read())
                //{
                //    if (reader["Tipo"].ToString().Equals("1")) //CodTipoFinanciacion
                //    {
                //        //Aplicar los valores a los campos de texto.
                //        txt_mes.Text = reader["Mes"].ToString();
                //        txt_producto_servicio.Text = NombreDelProductoSeleccionado;
                //        txt_observaciones.Text = reader["Observaciones"].ToString();
                //        txt_observ_interventor.Text = reader["ObservacionesInterventor"].ToString();
                //        if (reader["Aprobada"].ToString() == "1") { dd_aprobado.Items[0].Selected = true; }
                //        else { dd_aprobado.Items[1].Selected = true; }
                //        txt_ingreso_obtenido.Text = reader["Valor"].ToString();
                //        txt_ventas_obtenidas.Text = reader["Valor"].ToString();
                //        lbl_Total.Text = reader["Valor"].ToString();
                //    }
                //    else
                //    {
                //        //Aplicar los valores a los campos de texto.
                //        txt_mes.Text = reader["Mes"].ToString();
                //        txt_producto_servicio.Text = NombreDelProductoSeleccionado;
                //        txt_observaciones.Text = reader["Observaciones"].ToString();
                //        txt_observ_interventor.Text = reader["ObservacionesInterventor"].ToString();
                //        if (reader["Aprobada"].ToString() == "1") { dd_aprobado.Items[0].Selected = true; }
                //        else { dd_aprobado.Items[1].Selected = true; }
                //        txt_ingreso_obtenido.Text = reader["Valor"].ToString();
                //        txt_ventas_obtenidas.Text = reader["Valor"].ToString();
                //        lbl_Total.Text = reader["Valor"].ToString();
                //    }
                //} 
                #endregion

                while (reader.Read())
                {
                    //Aplicar los valores a los campos del formulario.
                    txt_mes.Text = reader["Mes"].ToString();
                    txt_producto_servicio.Text = NombreDelProductoSeleccionado;
                    txt_observaciones.Text = reader["Observaciones"].ToString();
                    txt_observ_interventor.Text = reader["ObservacionesInterventor"].ToString();
                    v_CodTipoFinanciacion = Convert.ToInt32(reader["CodTipoFinanciacion"].ToString());

                    string valor = reader["Aprobada"].ToString();
                    try
                    {
                        if (reader["Aprobada"].ToString() == "True") { dd_aprobado.Items[0].Selected = true; }
                        else { dd_aprobado.Items[1].Selected = true; }
                    }
                    catch { dd_aprobado.Items[1].Selected = true; }

                    //Evaluar el valor "CodTipoFinanciacion" para determinar los valores a mostrar en los campos de texto.
                    if (reader["CodTipoFinanciacion"].ToString().Equals("1")) //CodTipoFinanciacion
                    {
                        txt_ingreso_obtenido.Text = Convert.ToDouble(reader["Valor"].ToString()).ToString();
                        decimal valorTotal = Convert.ToDecimal(reader["Valor"].ToString());
                        lbl_Total.Text = "$" + valorTotal.ToString("0,0.00", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        txt_ventas_obtenidas.Text = Convert.ToDouble(reader["Valor"].ToString()).ToString();
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
        /// Guardar la información en la tabla AvanceVentasPOMes.
        /// </summary>
        private void GuardarInformacion()
        {
            //Inicializar variables.
            int v_cod_ventas = 0;
            bool repetido = false;
            String sqlConsulta;
            DataTable tabla = new DataTable();
            int v_CodTipoFinanciacion = 0;
            int v_cod_proyecto = Convert.ToInt32(codProyecto);
            DataTable tabla_gerente_coord_inter = new DataTable();
            DataTable tabla_inter = new DataTable();

            //Obtiene la conexión
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            //Convertir.
            v_cod_ventas = int.Parse(CodProduccion);

            #region Consulta SQL comentada.
            //Según el clásico, hace una consulta para validar que el registro no sea repetido.
            //sqlConsulta = "SELECT * FROM InterventorNomina WHERE Cargo = " + txt_producto_servicio.Text.Trim() + 
            //              " AND CodProyecto = " + codProyecto + " AND Tipo <> '' "; 
            #endregion

            //Versión LINQ del SQL Anterior comentado.
            var sqldos = (from x in consultas.Db.InterventorVentas
                          where x.id_ventas == v_cod_ventas && x.CodProyecto == v_cod_ventas
                          select new { x.id_ventas, x.CodProyecto, x.NomProducto }).FirstOrDefault();

            if ((String.IsNullOrEmpty(sqldos.ToString())) || (sqldos.id_ventas == 0))
            {
                repetido = false;

                #region 1. Inserción de datos.

                if (v_CodTipoFinanciacion == 1)
                {
                    #region Inserción si el CodTipoFinanciacion = 1.

                    //Consulta.
                    sqlConsulta = "INSERT INTO AvanceVentasPOMes (CodProducto, Mes, CodTipoFinanciacion, Valor, Observaciones, CodContacto) " +
                                  "VALUES (" + sqldos.id_ventas + ", " + MesDelProducto_Venta_Seleccionado + ", " + 1 + ", " + txt_ingreso_obtenido.Text + ", '" + txt_observaciones.Text + "', " + usuario.IdContacto + ")";

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
                    sqlConsulta = "INSERT INTO AvanceVentasPOMes (CodProducto, Mes, CodTipoFinanciacion, Valor, Observaciones, CodContacto) " +
                                  "VALUES (" + sqldos.id_ventas + ", " + MesDelProducto_Venta_Seleccionado + ", " + 2 + ", " + txt_ingreso_obtenido.Text + ", '" + txt_observaciones.Text + "', " + usuario.IdContacto + ")";


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
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información de producción creada correctamente.');window.opener.location.reload();window.close();", true);
                return;
            }
            else
            {
                repetido = true;
                ///En el clásico NO hay un mensaje y/o validación de producto para su generación.
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El producto " + txt_producto_servicio.Text.Trim() + "  ya existe.')", true);
                return;
            }
        }

        /// <summary>
        /// Actualizar la información de la tabla AvanceVentasPOMes.
        /// </summary>
        private void ActualizarInformacion()
        {
            //Inicializar variables.
            String sqlConsulta;
            DataTable tabla = new DataTable();
            int v_cod_producto = Convert.ToInt32(CodProduccion);
            int v_cod_proyecto = Convert.ToInt32(codProyecto);
            DataTable tabla_gerente_coord_inter = new DataTable();
            DataTable tabla_inter = new DataTable();

            //Obtiene la conexión
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            #region Consulta SQL comentada.
            //Según el clásico, hace una consulta para validar que el registro no sea repetido.
            //sqlConsulta = "SELECT * FROM InterventorNomina WHERE Cargo = " + txt_producto_servicio.Text.Trim() + 
            //              " AND CodProyecto = " + codProyecto + " AND Tipo <> '' "; 
            #endregion

            //Versión LINQ del SQL Anterior comentado.
            var sqldos = (from x in consultas.Db.InterventorVentas
                          where x.id_ventas == v_cod_producto && x.CodProyecto == v_cod_proyecto
                          select new { x.id_ventas, x.CodProyecto, x.NomProducto }).FirstOrDefault();

            if (!(String.IsNullOrEmpty(sqldos.ToString()) || sqldos.id_ventas == 0))
            {
                #region 1. Actualización de datos.

                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    #region Trata la información para el grupo de Interventor.

                    //Consulta.
                    sqlConsulta = " UPDATE AvanceVentasPOMes " +
                                  " SET ObservacionesInterventor = '" + txt_observ_interventor.Text + "', " +
                                  " Aprobada = " + dd_aprobado.SelectedValue +
                                  " WHERE CodProducto = " + CodProduccion + " AND Mes = " + MesDelProducto_Venta_Seleccionado;

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
                        sqlConsulta = " UPDATE AvanceVentasPOMes " +
                                      " SET Mes = " + MesDelProducto_Venta_Seleccionado + " , " +
                                      " CodTipoFinanciacion = " + v_CodTipoFinanciacion + ", " +
                                      " Valor = " + txt_ingreso_obtenido.Text + ", " +
                                      " Observaciones = '" + txt_observaciones.Text + "', " +
                                      " CodContacto = " + usuario.IdContacto +
                                      " WHERE CodProducto = " + CodProduccion + " AND Mes = " + MesDelProducto_Venta_Seleccionado +
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
                        sqlConsulta = " UPDATE AvanceVentasPOMes " +
                                      " SET Mes = " + MesDelProducto_Venta_Seleccionado + " , " +
                                      " CodTipoFinanciacion = " + v_CodTipoFinanciacion + ", " +
                                      " Valor = " + txt_ventas_obtenidas.Text + ", " +
                                      " Observaciones = '" + txt_observaciones.Text + "', " +
                                      " CodContacto = " + usuario.IdContacto +
                                      " WHERE CodProducto = " + CodProduccion + " AND Mes = " + MesDelProducto_Venta_Seleccionado +
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
                                datoNuevo.Descripcion = "Revisar la actividad de Nómina <b>" + sqldos.NomProducto /*NomActividad*/ + "</b> del proyecto <b>" + tabla_inter.Rows[0]["NomProyecto"].ToString() + " (" + codProyecto + ")</b>";
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
                                datoNuevo.Descripcion = "Revisar la actividad de Nómina <b>" + sqldos.NomProducto /*NomActividad*/ + "</b> del proyecto <b>" + tabla_inter.Rows[0]["NomProyecto"].ToString() + " (" + codProyecto + ")</b>";
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
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información del producto actualizada correctamente.')", true);
                return;
            }
        }

        /// <summary>
        /// Redirigir al usuario para observar la información de la grilla con archivos adjuntos "PDF's".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_btn_enlazar_grilla_PDF_Click(object sender, ImageClickEventArgs e)
        {
            CodProduccion = Session["CodProduccion"] != null && !string.IsNullOrEmpty(Session["CodProduccion"].ToString()) ? Session["CodProduccion"].ToString() : "0";
            MesDelProducto_Venta_Seleccionado = Session["MesDelProducto_Venta_Seleccionado"] != null && !string.IsNullOrEmpty(Session["MesDelProducto_Venta_Seleccionado"].ToString()) ? Session["MesDelProducto_Venta_Seleccionado"].ToString() : "0";
            NombreDelProductoSeleccionado = Session["NombreDelProductoSeleccionado"] != null && !string.IsNullOrEmpty(Session["NombreDelProductoSeleccionado"].ToString()) ? Session["NombreDelProductoSeleccionado"].ToString() : "0";
            Redirect(null, "../interventoria/CatalogoVentasInter.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }

        /// <summary>
        /// Crear un nuevo documento...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_btn_NuevoDocumento_Click(object sender, ImageClickEventArgs e)
        {
            CodProduccion = Session["CodProduccion"] != null && !string.IsNullOrEmpty(Session["CodProduccion"].ToString()) ? Session["CodProduccion"].ToString() : "0";
            MesDelProducto_Venta_Seleccionado = Session["MesDelProducto_Venta_Seleccionado"] != null && !string.IsNullOrEmpty(Session["MesDelProducto_Venta_Seleccionado"].ToString()) ? Session["MesDelProducto_Venta_Seleccionado"].ToString() : "0";
            NombreDelProductoSeleccionado = Session["NombreDelProductoSeleccionado"] != null && !string.IsNullOrEmpty(Session["NombreDelProductoSeleccionado"].ToString()) ? Session["NombreDelProductoSeleccionado"].ToString() : "0";
            Session["Accion_Docs"] = img_btn_NuevoDocumento.CommandName;
            Redirect(null, "../interventoria/CatalogoVentasInter.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
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
                txtSQL = "Delete from AvanceVentasPOMes where CodProducto=" + CodProduccion + " and mes=" + MesDelProducto_Venta_Seleccionado;
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
         FROM AvanceVentasPOMes 
         WHERE CodProducto = CodProducto AND Mes = MesDelProducto_Venta_Seleccionado 
         ORDER BY CodTipoFinanciacion, Mes
         
         Si la Acción es = Borrar (que NO se vé el botón en el clásico):
         DELETE FROM AvanceVentasPOMes WHERE CodProducto = CodProducto AND Mes = MesDelProducto_Venta_Seleccionado
         
         
         */

#endregion