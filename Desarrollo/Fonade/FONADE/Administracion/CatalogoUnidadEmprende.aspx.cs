using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Negocio;
using System.Configuration;
using System.Web.UI;
using Fonade.Clases;

namespace Fonade.FONADE.Administracion
{
    public partial class CatalogoUnidadEmprende : Base_Page
    {
        #region Variables globales.

        /// <summary>
        /// Valor que viene de "SeleccionarJefeUnidad.aspx" que indica ver el 
        /// panel de detalles.
        /// </summary>
        String OPEN;

        /// <summary>
        /// Valor que contiene el código de la institución seleccionado en el 
        /// RowCommand del GridView "gv_UnidadesEmprendimiento".
        /// </summary>
        String Institucion_Selected;

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    CargarUnidadesEmprendimiento("A"); //Por defecto, se deja la "A".
                    CargarTiposDeUnidad();
                    CargarDepartamentos();
                    CargarCiudades(0);
                    OPEN = Session["OPEN"] != null ? OPEN = Session["OPEN"].ToString() : "";
                    Institucion_Selected = Session["Cod_Institucion_Selected"] != null ? Institucion_Selected = Session["Cod_Institucion_Selected"].ToString() : "";
                    if (Institucion_Selected != "") { CargarCamposDetallesSeleccionado(Int32.Parse(Institucion_Selected)); }
                    else { pnlPrincipal.Visible = true; EvaluarEnunciado(""); }
                    EvaluarEnunciado(OPEN);
                }
            }
            catch (Exception ex)
            { 
                //NO es correcto simplemente "sacar" al usuario cuando
                //falla cualquier excepcion,
                //en vez de eso, buscamos excepciones especificas
                //Response.Redirect("~/Account/Login.aspx"); 
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Excepcion:"+ex.GetType().Name + ":" + ex.Message+"')", true);
               
            }
        }

        #region Métodos generales.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/04/2014.
        /// Dependiendo de qué panel está visible, se establecen ciertos valores.
        /// <param name="valor">Usado para mostrar el panel de actualización "ya que ha sido seleccionado el jefe de la unidad".</param>
        /// </summary>
        private void EvaluarEnunciado(string valor)
        {
            //Sólo se usa para cuando se vá a CREAR nuevas unidades, estableciendo campos y valores determinados.
            if (valor != "")
            {
                #region Comentarios.
                //pnl_detalles.Visible = true;
                //pnlPrincipal.Visible = false;
                //lbl_enunciado.Text = valor;
                //lbl_razonCambio.Visible = false;
                //txtCambioJefe.Visible = false;
                //btn_cambiarDatosJefe.Visible = false;
                //btn_crearUnidad.Visible = true;
                //btn_actualizarUnidad.Visible = false;
                //btn_modificarUnidad.Visible = false;
                //return; 
                #endregion

                pnlPrincipal.Visible = false;
                lbl_enunciado.Text = "MODIFICAR UNIDAD DE EMPRENDIMIENTO";
                btn_crearUnidad.Visible = false;
                btn_actualizarUnidad.Visible = false;
                //Campos de "Jefe Unidad".
                btn_cambiarDatosJefe.Visible = true;
                lbl_razonCambio.Visible = true;
                txtCambioJefe.Visible = true;
                btn_modificarUnidad.Visible = true;
                //CargarCamposDetallesSeleccionado(Int32.Parse(Session["Cod_Institucion_Selected"].ToString()));
            }
            else
            {
                if (pnlPrincipal.Visible == true)
                {
                    pnl_detalles.Visible = false;
                    lbl_enunciado.Text = "UNIDADES DE EMPRENDIMIENTO";
                }
                else
                {
                    pnlPrincipal.Visible = false;
                    lbl_enunciado.Text = "MODIFICAR UNIDAD DE EMPRENDIMIENTO";
                    btn_crearUnidad.Visible = false;
                    btn_actualizarUnidad.Visible = false;
                    //Campos de "Jefe Unidad".
                    btn_cambiarDatosJefe.Visible = true;
                    lbl_razonCambio.Visible = true;
                    txtCambioJefe.Visible = true;
                    btn_modificarUnidad.Visible = true;
                }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/04/2014.
        /// Cargar la grilla de "Unidades de emprendimiento".
        /// <param name="sLetraUpper">Letra seleccionada de las opciones de filtrado.</param>
        /// </summary>
        private void CargarUnidadesEmprendimiento(String sLetraUpper)
        {
            //Inicializar variables.
            String sqlConsulta;

            sqlConsulta = " SELECT Id_Institucion, NomUnidad, NomInstitucion, Inactivo, CodCiudad, NomTipoInstitucion " +
                          " , C.NomCiudad " + //Name of the city.
                          " FROM Institucion INNER JOIN TipoInstitucion  " +
                          " ON Id_TipoInstitucion = CodTipoInstitucion " +
                          " AND UPPER(NomUnidad) LIKE '" + sLetraUpper.ToUpperInvariant() + "'+ '%' " +
                          " LEFT JOIN Ciudad AS C ON CodCiudad = C.Id_Ciudad " + //get the name of the city.
                          " ORDER BY NomUnidad "; //NOTA: El sorting será programado en el evento "Sorting" de la grilla.

            var tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

            Session["dtEmpresas"] = tabla;
            gv_UnidadesEmprendimiento.DataSource = tabla;
            gv_UnidadesEmprendimiento.DataBind();
        }

        /// <summary>
        /// Se debe enviar la información de la tabla en una variable se sesión
        /// para poder sortearlo.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            var sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;

                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Cargar el listado de unidades al DropDownList "dd_CodTipoInstitucion".
        /// </summary>
        private void CargarTiposDeUnidad()
        {
            //Inicializar variables.
            String sqlConsulta;

            try
            {
                //Consulta 
                sqlConsulta = "SELECT Id_TipoInstitucion, NomTipoInstitucion From TipoInstitucion ORDER BY NomTipoInstitucion";

                var datos = consultas.ObtenerDataTable(sqlConsulta, "text");

                if (datos.Rows.Count > 0)
                {
                    for (int i = 0; i < datos.Rows.Count; i++)
                    {
                        ListItem item = new ListItem();
                        item.Value = datos.Rows[i]["Id_TipoInstitucion"].ToString();
                        item.Text = datos.Rows[i]["NomTipoInstitucion"].ToString();
                        dd_CodTipoInstitucion.Items.Add(item);
                    }
                }
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo cargar la lista de unidades.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Cargar el listado de departamentos al DropDownList "dd_SelDpto2".
        /// </summary>
        private void CargarDepartamentos()
        {
            //Inicializar variables.
            String sqlConsulta;

            try
            {
                //Consulta 
                sqlConsulta = "SELECT id_Departamento, NomDepartamento From Departamento ORDER BY NomDepartamento";

                var datos = consultas.ObtenerDataTable(sqlConsulta, "text");

                if (datos.Rows.Count > 0)
                {
                    for (int i = 0; i < datos.Rows.Count; i++)
                    {
                        ListItem item = new ListItem();
                        item.Value = datos.Rows[i]["id_Departamento"].ToString();
                        item.Text = datos.Rows[i]["NomDepartamento"].ToString();
                        dd_SelDpto2.Items.Add(item);
                    }
                }
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo cargar la lista de departamentos.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Cargar el listado de ciudades al DropDownList "dd_Ciudades".
        /// <param name="codDepto">Código del departamento seleccionado.</param>
        /// </summary>
        private void CargarCiudades(Int32 codDepto)
        {
            //Inicializar variables.
            String sqlConsulta;

            try
            {
                if (codDepto == 0)
                { return; }
                else
                {
                    //Vaciar los elementos que podría tener.
                    dd_Ciudades.Items.Clear();

                    //Consulta 
                    sqlConsulta = "SELECT Id_Ciudad, NomCiudad FROM Ciudad WHERE CodDepartamento = " + codDepto;

                    var datos = consultas.ObtenerDataTable(sqlConsulta, "text");

                    if (datos.Rows.Count > 0)
                    {
                        for (int i = 0; i < datos.Rows.Count; i++)
                        {
                            ListItem item = new ListItem();
                            item.Value = datos.Rows[i]["Id_Ciudad"].ToString();
                            item.Text = datos.Rows[i]["NomCiudad"].ToString();
                            dd_Ciudades.Items.Add(item);
                        }
                    }
                    else
                    {
                        //Se coloca valor por defecto porque no hay elementos.
                        ListItem item_default = new ListItem();
                        item_default.Value = "0";
                        item_default.Text = "No se hallaron ciudades";
                        item_default.Enabled = false;
                        dd_Ciudades.Items.Add(item_default);
                    }
                }
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo cargar la lista de departamentos.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Cargar la información completa de la unidad de emprendimiento seleccionada y mostrar
        /// dicha información en los campos de texto correspondientes.
        /// </summary>
        /// <param name="CodInstitucion">Código de la institución "Unidad de Emprendimiento" seleccionada de "gv_UnidadesEmprendimiento".</param>
        private void CargarCamposDetallesSeleccionado(Int32 CodInstitucion)
        {
            //Inicializar variables.
            String sqlConsulta = "";
            String consulta_jefeUnidad = "";

            try
            {
                #region Cargar la información detallada.

                //Generar consulta.
                sqlConsulta = " SELECT NomInstitucion, NomUnidad, Nit, Direccion, CodCiudad, CriteriosSeleccion, " +
                              " CodTipoInstitucion, MotivoCambioJefe, Telefono, Fax, WebSite " +
                              " FROM Institucion " +
                              " WHERE Id_Institucion = " + CodInstitucion;

                //Asignar los resultados de la consulta en variable DataTable.
                var sql_1 = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Si la consulta anterior tiene datos "y SI debe tener".
                if (sql_1.Rows.Count > 0)
                {
                    //Asigna los resultados de la consulta a los campos de texto.
                    txt_NombreCentroInstitucion.Text = sql_1.Rows[0]["NomInstitucion"].ToString();
                    txt_nombreUnidad.Text = sql_1.Rows[0]["NomUnidad"].ToString();
                    txt_NitCentroInstitucion.Text = sql_1.Rows[0]["Nit"].ToString();
                    txt_Direccion.Text = sql_1.Rows[0]["Direccion"].ToString();

                    #region Establecer la ciudad y departamentos seleccionados en las listas desplegables.

                    //Variables que uso internamente para procesar la selección cargada de ciudades y departamentos.
                    string consulta_depto = "";
                    consulta_depto = "SELECT CodDepartamento FROM Ciudad WHERE Id_Ciudad = " + sql_1.Rows[0]["CodCiudad"].ToString();
                    var var_consulta_depto = consultas.ObtenerDataTable(consulta_depto, "text");

                    ///Primero reviso si la lista de ciudades si tiene ítems, de lo contrario, toca cargarle las
                    ///ciudades y ahí sí establecer las selecciones en las listas desplegables.
                    if (dd_Ciudades.Items.Count == 0)
                    {
                        //Cargar las ciudades.
                        CargarCiudades(Convert.ToInt32((var_consulta_depto.Rows[0]["CodDepartamento"].ToString())));
                        dd_SelDpto2.SelectedValue = var_consulta_depto.Rows[0]["CodDepartamento"].ToString();
                        dd_Ciudades.SelectedValue = sql_1.Rows[0]["CodCiudad"].ToString();
                    }
                    else
                    {
                        //Establecer la ciudad y departamentos seleccionados en las listas desplegables.
                        CargarCiudades(Convert.ToInt32((var_consulta_depto.Rows[0]["CodDepartamento"].ToString())));
                        dd_SelDpto2.SelectedValue = var_consulta_depto.Rows[0]["CodDepartamento"].ToString();
                        dd_Ciudades.SelectedValue = sql_1.Rows[0]["CodCiudad"].ToString();
                    }

                    #endregion

                    //Cargar datos a campos de text que "FONADE clásico" no carga, pero en inserción son claves.
                    txtTelefonoUnidad.Text = sql_1.Rows[0]["Telefono"].ToString();
                    txtFaxUnidad.Text = sql_1.Rows[0]["Fax"].ToString();
                    txtWebsite.Text = sql_1.Rows[0]["WebSite"].ToString();

                    txtCriterios.Text = sql_1.Rows[0]["CriteriosSeleccion"].ToString();
                    dd_CodTipoInstitucion.SelectedValue = sql_1.Rows[0]["CodTipoInstitucion"].ToString();
                    txtCambioJefe.Text = sql_1.Rows[0]["MotivoCambioJefe"].ToString();

                    //Se envía esta variable de sesión para ser usada en "SeleccionarJefeUnidad.aspx".
                    Session["CodInstitucionNueva"] = CodInstitucion.ToString();

                    #region Cargar la información del jefe de unidad que tiene asignado esta unidad de emprendimiento seleccionada.

                    try
                    {
                        #region Asignar la variable de sesión del nuevo jefe de unidad seleccionado.

                        //Nueva variable de sesión para ser usada cuando se cambie el jefe de la unidad.
                        object codjefe = Session["CodNuevoJefe_Seleccionado"];
                        Session["codJefeUnidad"] = ((string[])codjefe)[0];
                        //Mostrar el panel detalles con la información seleccionada y seguir el flujo correspondiente.
                        pnlPrincipal.Visible = false;
                        pnl_detalles.Visible = true;

                        #region Consultar la información del nnuevo jefe de unidad seleccionado.

                        sqlConsulta = " SELECT CON.Id_Contacto, CON.Nombres, CON.Apellidos, CON.CodTipoIdentificacion, " +
                                      " CON.Identificacion, DP.Id_Departamento, CON.CodCiudad " +
                                      " FROM Contacto AS CON INNER JOIN Ciudad AS CD " +
                                      " ON CON.CodCiudad = CD.Id_Ciudad INNER JOIN Departamento DP " +
                                      " ON CD.CodDepartamento = DP.Id_Departamento " +
                                      " WHERE CON.Id_Contacto = " + ((string[])codjefe)[0];

                        DataTable RS = new DataTable();

                        RS = consultas.ObtenerDataTable(sqlConsulta, "text");

                        #region Datos específicos del jefe de unidad seleccionado.

                        if (RS.Rows.Count > 0)
                        {
                            txt_jefeUnidad.Text = RS.Rows[0]["Nombres"].ToString() + " " + RS.Rows[0]["Apellidos"].ToString();
                            Session["NmbJefeUnidad"] = txt_jefeUnidad.Text;
                            Session["codJefeUnidad"] = RS.Rows[0]["Id_Contacto"].ToString();
                            //Se cargan en variables de sesión el departamento y la ciudad (municipio) que tiene el usuario
                            //para ser usados en la página "SeleccionarJefeUnidad.aspx".
                            Session["CodCiudad_JefeUnidad"] = RS.Rows[0]["CodCiudad"].ToString();
                            Session["CodDepartamento_JefeUnidad"] = RS.Rows[0]["Id_Departamento"].ToString();
                        }

                        #endregion

                        #endregion

                        #endregion
                    }
                    catch
                    {
                        #region Cargar la información según el flujo normal, es decir, el jefe de unidad que tenía previamente la unidad seleccionada.

                        //Versión de Mauricio Arias Olave.
                        consulta_jefeUnidad = " SELECT CON.Id_Contacto, CON.Nombres, CON.Apellidos, CON.CodTipoIdentificacion, " +
                                              " CON.Identificacion, DP.Id_Departamento, CON.CodCiudad " +
                                              " FROM InstitucionContacto AS IC, Contacto AS CON INNER JOIN Ciudad AS CD " +
                                              " ON CON.CodCiudad = CD.Id_Ciudad INNER JOIN Departamento DP " +
                                              " ON CD.CodDepartamento = DP.Id_Departamento " +
                                              " WHERE IC.CodInstitucion = " + CodInstitucion +
                                              " AND Id_Contacto = CodContacto AND FechaFin IS NULL";


                        //Asignar resultados de la búsqueda a variables DataTable.
                        var var_consulta_jefeUnidad = consultas.ObtenerDataTable(consulta_jefeUnidad, "text");

                        //Si contiene datos...
                        if (var_consulta_jefeUnidad.Rows.Count > 0)
                        {
                            txt_jefeUnidad.Text = var_consulta_jefeUnidad.Rows[0]["Nombres"].ToString() + " " + var_consulta_jefeUnidad.Rows[0]["Apellidos"].ToString();
                            Session["NmbJefeUnidad"] = txt_jefeUnidad.Text;
                            //Id del jefe unidad que tenía:
                            Session["codJefeUnidad"] = var_consulta_jefeUnidad.Rows[0]["Id_Contacto"].ToString();

                            //Se cargan en variables de sesión el departamento y la ciudad (municipio) que tiene el usuario
                            //para ser usados en la página "SeleccionarJefeUnidad.aspx".
                            Session["CodCiudad_JefeUnidad"] = var_consulta_jefeUnidad.Rows[0]["CodCiudad"].ToString();
                            Session["CodDepartamento_JefeUnidad"] = var_consulta_jefeUnidad.Rows[0]["Id_Departamento"].ToString();
                        }

                        #endregion
                    }

                    #endregion
                }

                #endregion
            }
            catch (Exception)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo cargar los detalles de la unidad de emprendimiento seleccionado.')", true);
                return;
            }
        }

        #endregion

        #region Métodos especiales.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Método usado en "DeclaraVariables.inc" de FONADE Clásico.
        /// Usado para obtener el valor "Texto" de la tabla "Texto", este valor será usado en la creación
        /// de mensajes cuando el CheckBox "chk_actualizarInfo" esté chequeado; Si el resultado de la consulta
        /// NO trae datos, según FONADE Clásico, crea un registro con la información dada.
        /// </summary>
        /// <param name="NomTexto">Nombre del texto a consultar.</param>
        /// <returns>NomTexto consultado.</returns>
        private string Texto(String NomTexto)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String RSTexto;
            String txtSQL;
            bool correcto = false;

            //Consulta
            txtSQL = "SELECT Texto FROM Texto WHERE NomTexto='" + NomTexto + "'";

            var resultado = consultas.ObtenerDataTable(txtSQL, "text");

            if (resultado.Rows.Count > 0)
                return resultado.Rows[0]["Texto"].ToString();
            else
            {
                #region Si no existe la palabra "consultada", la crea.

                txtSQL = "INSERT INTO Texto (NomTexto, Texto) VALUES ('" + NomTexto + "','" + NomTexto + "')";

                //Asignar SqlCommand para su ejecución.
                cmd = new SqlCommand(txtSQL, conn);

                //Ejecutar SQL.
                correcto = EjecutarSQL(conn, cmd);

                if (correcto == false)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción de TEXTO.')", true);
                    return NomTexto; //""; //Debería retornar vacío y validar en el método donde se llame si esté validado.
                }
                else
                { return NomTexto; }

                #endregion
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Generar registros en tabla "LogEnvios".
        /// </summary>
        /// <param name="p_Asunto">Asunto.</param>
        /// <param name="p_EnviadoPor">Enviado Por.</param>
        /// <param name="p_EnviadoA">Enviado A:</param>
        /// <param name="p_Programa">Programa:</param>
        /// <param name="codProyectoActual">Código del proyecto</param>
        /// <param name="p_Exitoso">Exitoso "1/0".</param>
        private void prLogEnvios(String p_Asunto, String p_EnviadoPor, String p_EnviadoA, String p_Programa, Int32 codProyectoActual, Boolean p_Exitoso)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta = "";
            bool correcto = false;

            try
            {
                sqlConsulta = " INSERT INTO LogEnvios (Fecha, Asunto, EnviadoPor, EnviadoA, Programa, CodProyecto, Exitoso) " +
                              " VALUES (GETDATE(),'" + p_Asunto + "','" + p_EnviadoPor + "','" + p_EnviadoA + "','" + p_Programa + "'," + codProyectoActual + "," + p_Exitoso + ") ";

                //Asignar SqlCommand para su ejecución.
                cmd = new SqlCommand(sqlConsulta, conn);

                //Ejecutar SQL.
                correcto = EjecutarSQL(conn, cmd);

                if (correcto == false)
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción de log.')", true);
                    //return;
                }
            }
            catch { }
        }

        #endregion

        #region Inserción.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Método que hace consultas a la tabla "Texto" para validar los campos para la inserción de unidades de emprendimiento.
        /// </summary>
        /// <returns>Cadena de texto vacía = Todos los campos pasaron las validaciones. // variable llena = Error y/o no pasaron las validaciones.</returns>
        private string ValidarCamposInsercion()
        {
            //Inicializar variables.
            string validado = "";

            try
            {
                if (txt_nombreUnidad.Text.Trim() == "")
                { validado = Texto("TXT_NOMBREUNIDAD_REQ"); return validado; }
                if (txt_NombreCentroInstitucion.Text.Trim() == "")
                { validado = Texto("TXT_NOMBRECENTRO_REQ"); return validado; }
                if (txt_NitCentroInstitucion.Text.Trim() == "")
                { validado = Texto("TXT_NITUNIDAD_REQ"); return validado; }
                if (dd_SelDpto2.SelectedValue == "")
                { validado = Texto("TXT_DEPARTAMENTO_REQ"); return validado; }
                if (txt_Direccion.Text.Trim() == "")
                { validado = Texto("TXT_DIRECCIONUNIDAD_REQ"); return validado; }

                try
                {
                    if (String.IsNullOrEmpty(Session["codJefeUnidad"].ToString()))
                    { validado = Texto("TXT_CAMBIOJEFEUNIDAD_REQ"); return validado; }
                }
                catch { validado = "Error al obtener el valor ''codjefeUnidad''"; return validado; }

                if (txt_NitCentroInstitucion.Text.Trim() != "")
                {
                    try { int dato = Convert.ToInt32(txt_NitCentroInstitucion.Text.Trim()); }
                    catch { validado = Texto("TXT_NITUNIDAD_NOVALIDO"); return validado; }
                }
                else
                {
                    validado = Texto("TXT_NITUNIDAD_REQ");
                    return validado;
                }

                return validado;

            }
            catch { return validado; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Adicionar nueva unidad de emprendimiento.
        /// "Habilitar campos para creación". (LinkButton).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            pnlPrincipal.Visible = false;
            pnl_detalles.Visible = true;
            EvaluarEnunciado("NUEVA UNIDAD DE EMPRENDIMIENTO");
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Adicionar nueva unidad de emprendimiento.
        /// "Habilitar campos para creación". (ImageButton)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Adicionar_Click(object sender, ImageClickEventArgs e)
        {
            pnlPrincipal.Visible = false;
            pnl_detalles.Visible = true;
            EvaluarEnunciado("NUEVA UNIDAD DE EMPRENDIMIENTO");
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Crear unidad de emprendimiento.
        /// </summary>
        private void CrearUnidadEmprendimiento()
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            bool ejecucion_correcta = false; //Comprueba si la inserción y/o consultas SQL si se ejecutaron.
            String sqlConsulta = "";
            Int32 CodTipoInstitucion_Seleccionado = 0; //Almacenará el CodTipoInstitucion seleccionado de "dd_CodTipoInstitucion".
            Int32 CodCiudad_Seleccionado = 0; //Almacenará el CodCiudad seleccionado de "dd_Ciudades".
            Int32 bolContinua = 0; //Determina si el flujo puede continuar "según código fuente de FONADE clásico".
            Int32 numNit = 0; //Variable que almacena el nit digitado en la caja de texto "txt_NitCentroInstitucion".
            Int32 CodInstitucion_Cargado = 0; //Variable que almacena el valor de la consulta "MAX(Id_Institucion).
            Int32 codJefeUnidad = 0;//Almacena el valor "CodJefeUnidad" seleccionado para crear la unidad de emprendimiento.
            Int32 CodContacto_resultado_consulta_4 = 0; //Valor obtenido de la consulta "resultado_consulta_4".
            Int32 CodContacto_resultado_consulta_5 = 0; //Valor obtenido de la consulta "resultado_consulta_5".

            try
            {
                string validado = "";
                validado = ValidarCamposInsercion();

                if (validado.Trim() == "")
                {
                    #region Flujo de la información

                    #region Obtener valores seleccionados para usarlos desde variables internas.

                    //Convertir el valor seleccionado en "dd_CodTipoInstitucion" en un dato utilizable en el código.
                    CodTipoInstitucion_Seleccionado = Convert.ToInt32(dd_CodTipoInstitucion.SelectedValue);

                    //Convertir el valor seleccionado en "dd_Ciudades" en un dato utilizable en el código.
                    CodCiudad_Seleccionado = Convert.ToInt32(dd_Ciudades.SelectedValue);

                    //Obtener el NIT del centro a crear.
                    numNit = Convert.ToInt32(txt_NitCentroInstitucion.Text.Trim());

                    //Este valor se DEBE obtener de la ventana emergente que enviará por sesión el jefe unidad seleccionado.
                    try { codJefeUnidad = Convert.ToInt32(Session["codJefeUnidad"].ToString()); }
                    catch
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No ha seleccionado el jefe de la unidad.')", true);
                        return;
                    }

                    #endregion

                    #region Obtener el nombre de la institución para saber si puede continuar.

                    //Realizar consulta.
                    sqlConsulta = "SELECT NomTipoInstitucion FROM TipoInstitucion WHERE Id_TipoInstitucion = " + CodTipoInstitucion_Seleccionado;

                    //Asignar valor a variable DataTable.
                    var resultado_consulta_1 = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Si contiene datos, continuar con el flujo.
                    if (resultado_consulta_1.Rows.Count > 0)
                    {
                        //Si es "Externa" hace la consulta #2.
                        if (resultado_consulta_1.Rows[0]["NomTipoInstitucion"].ToString() == "Externa")
                        {
                            //Consulta #2.
                            sqlConsulta = " SELECT Nit, CodCiudad FROM Institucion WHERE " +
                                          " Nit = " + txt_NitCentroInstitucion.Text.Trim() +
                                          " AND CodCiudad = " + CodCiudad_Seleccionado; //ejecutaReader.

                            var resultado_consulta_2 = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (resultado_consulta_2.Rows.Count > 0)
                                bolContinua = 0;
                            else
                                bolContinua = 1;
                        }
                    }

                    #endregion

                    #region Si el valor "bolContinua es igual a 1, realiza la inserción en la tabla "Institucion".

                    if (bolContinua == 1)
                    {
                        #region Hacer inserción.

                        sqlConsulta = " INSERT INTO Institucion (NomInstitucion, NomUnidad, Nit, Direccion, Telefono, Fax, CodCiudad, Inactivo, CriteriosSeleccion, " +
                                      " CodTipoInstitucion, WebSite)" +
                                      " VALUES('" + txt_NombreCentroInstitucion.Text.Trim() + "'," + "'" + txt_nombreUnidad.Text.Trim() + "'," + numNit + "," +
                                      "'" + txt_Direccion.Text.Trim() + "','" + txtTelefonoUnidad.Text.Trim() + "','" + txtFaxUnidad.Text.Trim() + "'," + CodCiudad_Seleccionado +
                                      ",0," + "'" + txtCriterios.Text.Trim() + "'," + CodTipoInstitucion_Seleccionado + ",'" + txtWebsite.Text.Trim() + "')";

                        //Asignar SqlCommand para su ejecución.
                        cmd = new SqlCommand(sqlConsulta, conn);

                        //Ejecutar inserción.
                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                        if (ejecucion_correcta == false)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción (1).')", true);
                            return;
                        }

                        #endregion

                        #region Consultar el MAX(Id_Institucion) y si trae datos, genera aún mas flujos de ejecución.

                        //Consultar el MAX(Id_Institucion).
                        sqlConsulta = "SELECT MAX(Id_Institucion) AS CodInstitucion FROM Institucion";

                        //Asignar resultados de la consulta anterior a variable DataTable.
                        var resultado_consulta_2 = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Si la consulta trajo datos.
                        if (resultado_consulta_2.Rows.Count > 0)
                        {
                            //Convertir el valor obtenido y asignarlo a la variable dispuesta para ello.
                            CodInstitucion_Cargado = Convert.ToInt32(resultado_consulta_2.Rows[0]["CodInstitucion"].ToString());

                            #region Continuar con el flujo de registros.

                            #region Actualiza los datos del usuario jefe unidad y lo activa si es necesario.

                            sqlConsulta = " UPDATE Contacto SET Inactivo = 0, " +
                                          " CodInstitucion = " + CodInstitucion_Cargado + ", " +
                                          " CodCiudad = " + CodCiudad_Seleccionado +
                                          " WHERE Id_Contacto = " + codJefeUnidad;

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización al crear unidad de emprendimiento (1).')", true);
                                return;
                            }

                            #endregion

                            ///Actualiza la tabla institución contacto, por si el usuario es jefe de otra unidad 
                            ///le coloca la fecha fin de la actual unidad.
                            #region Actualización (2).

                            sqlConsulta = " UPDATE InstitucionContacto SET FechaFin = GETDATE() WHERE CodContacto = " + codJefeUnidad;

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización al crear unidad de emprendimiento (2).')", true);
                                return;
                            }

                            #endregion

                            #region Registra relación entre Institución Contacto.

                            sqlConsulta = " INSERT INTO InstitucionContacto (CodInstitucion, CodContacto, FechaInicio) " +
                                          " VALUES(" + CodInstitucion_Cargado + "," + codJefeUnidad + ", GETDATE()) ";

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción (2).')", true);
                                return;
                            }

                            #endregion

                            #region Borra la relación del usuario con cualquier otro grupo, solo puede pertenecer a un grupo.

                            sqlConsulta = "DELETE FROM GrupoContacto WHERE CodContacto = " + codJefeUnidad;

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación al crear unidad de emprendimiento (1).')", true);
                                return;
                            }

                            #endregion

                            #region Inserta la relación entre el contacto jefe de la unidad y el grupo de usuarios.

                            sqlConsulta = " INSERT INTO GrupoContacto (CodGrupo, CodContacto) " +
                                          " VALUES (" + Constantes.CONST_JefeUnidad + "," + codJefeUnidad + ")";

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción al crear unidad de emprendimiento (3).')", true);
                                return;
                            }

                            #endregion

                            #region Si tiene proyectos a su cargo, los deja pendientes de asignación de asesor.

                            //Generar consulta....
                            sqlConsulta = " SELECT CodProyecto, CodRol FROM ProyectoContacto " +
                                          " WHERE FechaFin IS NULL AND CodContacto = " + codJefeUnidad;

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en consulta (1).')", true);
                                return;
                            }

                            #endregion

                            #region Procesamiento siguiente.

                            //Consulta
                            sqlConsulta = " SELECT P.NomProyecto, PC.CodProyecto, PC.CodRol, E.Id_Estado " +
                                          " FROM ProyectoContacto PC,Proyecto P, Estado E " +
                                          " WHERE FechaFin is NULL " +
                                          " AND PC.CodProyecto = P.Id_Proyecto" +
                                          " AND P.CodEstado = E.Id_estado" +
                                          " AND PC.CodContacto = " + codJefeUnidad;

                            //Asignar resultados de la consulta anterior a variable DataTable.
                            var resultado_consulta_3 = consultas.ObtenerDataTable(sqlConsulta, "text");

                            //Si la consulta anterior obtuvo resultados...
                            if (resultado_consulta_3.Rows.Count > 0)
                            {
                                #region Realiza la siguente cadena de sucesos.

                                for (int i = 0; i < resultado_consulta_3.Rows.Count; i++)
                                {
                                    //Obtener el Id_Estado de la consulta anterior para procesarla en la condición.
                                    int dato_1 = Convert.ToInt32(resultado_consulta_3.Rows[0]["Id_Estado"].ToString());

                                    //Obtener el CodProyecto de la consulta anterior para usarlo en consultas SQL.
                                    int dato_2 = Convert.ToInt32(resultado_consulta_3.Rows[0]["CodProyecto"].ToString());

                                    //Obtener el NomProyecto de la consulta anterior para usarlo en la generación de tareas.
                                    string dato_3 = resultado_consulta_3.Rows[0]["NomProyecto"].ToString();

                                    //Si el Id_Estado es menor que la constante de asignación de recursos.
                                    if (dato_1 < Constantes.CONST_AsignacionRecursos)
                                    {
                                        #region Si al proyecto no le han asignado recursos pasará a REASIGNACION POR ASIGNACION.

                                        sqlConsulta = " UPDATE Proyecto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                      " WHERE Id_Proyecto = " + dato_2;

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar el proyecto sin recursos.')", true);
                                            return;
                                        }

                                        #endregion

                                        #region Actualiza la institución de los contactos activos del proyecto.

                                        sqlConsulta = " UPDATE Contacto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                      " WHERE Id_Contacto IN(SELECT CodContacto FROM ProyectoContacto WHERE FechaFin IS NULL AND Inactivo = 0 AND CodProyecto = " + dato_2 + ")";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la institución de los contactos activos del proyecto.')", true);
                                            return;
                                        }

                                        #endregion

                                        #region Asigna tarea a jefe de unidad REASIGNACION POR ASIGNACION, para asignación de asesor.

                                        //Consulta:
                                        sqlConsulta = " SELECT CodContacto FROM InstitucionContacto WHERE FechaFin IS NULL " +
                                                      " AND CodInstitucion = " + Constantes.CONST_UnidadTemporal;

                                        //Asignación de los resultados de la consulta anterior a variable DataTable.
                                        var resultado_consulta_4 = consultas.ObtenerDataTable(sqlConsulta, "text");

                                        //Si la tabla anterior contiene datos...
                                        if (resultado_consulta_4.Rows.Count > 0)
                                        {
                                            //Obtener el CodContacto de la consulta "resultado_consulta_4".
                                            CodContacto_resultado_consulta_4 = Convert.ToInt32(resultado_consulta_4.Rows[0]["CodContacto"].ToString());

                                            //Asignar tarea = NO IMPLEMENTADO = código comentado en el clásico. prTareaAsignarAsesor
                                            //Ver línea 1871 de "CatalogoInterventorTMP.aspx" y sucesivas.
                                            //prTareaAsignarAsesor CodContacto_resultado_consulta_4,usuario.Id_Usuario,dato_2,dato_3
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Si al proyecto ya le han asignado recursos pasará a REASIGNACION POR SEGUIMIENTO.

                                        sqlConsulta = " UPDATE Proyecto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                      " WHERE Id_Proyecto = " + dato_2;

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar el proyecto con recursos.')", true);
                                            return;
                                        }

                                        #endregion

                                        #region Actualiza la institución de los contactos activos del proyecto.

                                        sqlConsulta = " UPDATE Contacto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                      " WHERE Id_Contacto IN(SELECT CodContacto FROM ProyectoContacto WHERE FechaFin IS NULL AND Inactivo = 0 AND CodProyecto = " + dato_2 + ")";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar el proyecto con recursos.')", true);
                                            return;
                                        }

                                        #endregion

                                        #region Asigna tarea a jefe de unidad REASIGNACION POR ASIGNACION, para asignación de asesor.

                                        //Consulta:
                                        sqlConsulta = " SELECT CodContacto FROM InstitucionContacto WHERE FechaFin IS NULL AND CodInstitucion = " + Constantes.CONST_UnidadTemporal;

                                        //Asignación de los resultados de la consulta anterior a variable DataTable.
                                        var resultado_consulta_5 = consultas.ObtenerDataTable(sqlConsulta, "text");

                                        //Si la tabla anterior contiene datos...
                                        if (resultado_consulta_5.Rows.Count > 0)
                                        {
                                            //Obtener el CodContacto de la consulta "resultado_consulta_4".
                                            CodContacto_resultado_consulta_5 = Convert.ToInt32(resultado_consulta_5.Rows[0]["CodContacto"].ToString());

                                            //Asignar tarea = NO IMPLEMENTADO = código comentado en el clásico. prTareaAsignarAsesor
                                            //Ver línea 1871 de "CatalogoInterventorTMP.aspx" y sucesivas.
                                            //prTareaAsignarAsesor CodContacto_resultado_consulta_5,usuario.Id_Usuario,dato_2,dato_3
                                        }

                                        #endregion
                                    }
                                }

                                #endregion

                                #region Finaliza el ciclo anterior y Trae los datos del usuario para el envio del email.

                                //Consulta.
                                sqlConsulta = "SELECT Nombres, Apellidos, Email, Clave FROM Contacto WHERE Id_Contacto = " + codJefeUnidad;

                                //Asignación de resultado de la consulta anterior a variable DataTable.
                                var resultado_consulta_6 = consultas.ObtenerDataTable(sqlConsulta, "text");

                                //Si la consulta anterior contiene datos...
                                if (resultado_consulta_6.Rows.Count > 0)
                                {
                                    for (int j = 0; j < resultado_consulta_6.Rows.Count; j++)
                                    {
                                        #region Obtener valores de la consulta anterior para generar mensajes masivos.

                                        bool Enviado = false; //Establece si el mensaje fue o no enviado.
                                        String claveObtenida = ""; //Variable que almacena la clave obtenida de la consulta anterior.
                                        String Email_obtenido = ""; //Variable que almacena el email obtenido de la consulta anterior.
                                        String Nombre_obtenido = ""; //Variable que almacena el nombre obtenido de la consulta anterior.
                                        String Apellidos_obtenidos = ""; //Variable que almacena el nombre obtenido de la consulta anterior.

                                        //Obtener el valor "Clave" de "resultado_consulta_6".
                                        claveObtenida = resultado_consulta_6.Rows[j]["Clave"].ToString();

                                        //Obtener el valor "Email" de "resultado_consulta_6".
                                        Email_obtenido = resultado_consulta_6.Rows[j]["Email"].ToString();

                                        //Obtener el valor "Nombres" de "resultado_consulta_6".
                                        Nombre_obtenido = resultado_consulta_6.Rows[j]["Nombres"].ToString();

                                        //Obtener el valor "Nombres" de "resultado_consulta_6".
                                        Apellidos_obtenidos = resultado_consulta_6.Rows[j]["Apellidos"].ToString();

                                        #endregion

                                        #region Enviar el Email al usuario.

                                        //Variable que almacena el texto obtenido de la consulta a la tabla "Texto".
                                        String Texto_Obtenido = "";

                                        //Consultar el "TEXTO".
                                        Texto_Obtenido = Texto("TXT_EMAILENVIOCLAVE");

                                        //Sólo por si acaso, si el resultado de "Texto_Obtenido" NO devuelve los datos según el texto esperado,
                                        //se debe asignar el texto tal cual se vió en BD el "28/04/2014".
                                        if (Texto_Obtenido.Contains("Señor Usuario") || Texto_Obtenido.Trim() == null)
                                        {
                                            Texto_Obtenido = "Señor Usuario Con el usuario {{Email}} y contraseña {{Clave}},  podrá acceder al sistema de información por medio de la pagina www.fondoemprender.com,  allí encontrara en la parte superior del sistema específicamente en el botón con el signo de interrogación  (?) el manual de su perfil ''{{Rol}}''";
                                        }

                                        //Reemplazar determinados caracteres por caracteres definidos específicamente para esta acción.
                                        Texto_Obtenido = Texto_Obtenido.Replace("{{Rol}}", "Jefe Unidad");
                                        Texto_Obtenido = Texto_Obtenido.Replace("{{Email}}", Email_obtenido);
                                        Texto_Obtenido = Texto_Obtenido.Replace("{{Clave}}", claveObtenida);

                                        try
                                        {
                                            //Generar y enviar mensaje.
                                            Correo correo = new Correo(usuario.Email,
                                                                       "Fondo Emprender",
                                                                       Email_obtenido,
                                                                       Nombre_obtenido + " " + Apellidos_obtenidos,
                                                                       "Registro a Fondo Emprender",
                                                                       Texto_Obtenido);
                                            correo.Enviar();

                                            //El mensaje fue enviado.
                                            Enviado = true;

                                            //Inserción en tabla "LogEnvios".
                                            prLogEnvios("Registro a Fondo Emprender", usuario.Email, Email_obtenido, "Catálogo de Unidad", 0, Enviado);

                                            //Finalmente se redirige al usuario a "CatalogoUnidadEmprende.aspx".
                                            //Response.Redirect("CatalogoUnidadEmprende.aspx"); //Comentado el 02/05/2014. -- Reemplazado el 22/05/2014.
                                            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.location.reload(false); ", true);
                                        }
                                        catch
                                        {
                                            //El mensaje no pudo ser enviado.
                                            Enviado = false;

                                            //Inserción en tabla "LogEnvios".
                                            prLogEnvios("Registro a Fondo Emprender", usuario.Email, Email_obtenido, "Catálogo de Unidad", 0, Enviado);

                                            //Finalmente se redirige al usuario a "CatalogoUnidadEmprende.aspx".
                                            //Response.Redirect("CatalogoUnidadEmprende.aspx"); //Comentado el 02/05/2014. -- Reemplazado el 22/05/2014.
                                            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.location.reload(false); ", true);
                                        }

                                        #endregion
                                    }
                                }

                                #endregion
                            }

                            #endregion

                            #endregion
                        }

                        #endregion

                        //Finalizar el ciclo declarando la variable de sesión con el valor "false".
                        Session["bRepetido"] = "false";
                        //Response.Redirect("CatalogoUnidadEmprende.aspx"); //Comentado el 02/05/2014. -- Reemplazado el 22/05/2014.
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.location.reload(false); ", true);
                    }
                    else
                    { Session["bRepetido"] = "true"; }

                    #endregion

                    #endregion
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + validado + "')", true);
                    return;
                }
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo crear la unidad de emprendimiento.')", true);
                return;
            }
        }

        #endregion

        #region Actualización.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Método que hace consultas a la tabla "Texto" para validar los campos para la actualización de unidades de emprendimiento.
        /// </summary>
        /// <returns>Cadena de texto vacía = Todos los campos pasaron las validaciones. // variable llena = Error y/o no pasaron las validaciones.</returns>
        private string ValidarCamposActualizacion()
        {
            //Inicializar variables.
            string validado = "";

            try
            {
                if (String.IsNullOrEmpty(Session["codJefeUnidad"].ToString()))
                { validado = Texto("TXT_CAMBIOJEFEUNIDAD_REQ"); return validado; }

                return validado;
            }
            catch { validado = Texto("TXT_CAMBIOJEFEUNIDAD_REQ"); return validado; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Actualizar la unidad de emprendimiento seleccionada.
        /// </summary>
        private void ActualizarUnidadEmprendimiento()
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            bool ejecucion_correcta = false; //Comprueba si la inserción y/o consultas SQL si se ejecutaron.
            String sqlConsulta = "";
            Int32 CodTipoInstitucion_Seleccionado = 0; //Almacenará el CodTipoInstitucion seleccionado de "dd_CodTipoInstitucion".
            Int32 CodCiudad_Seleccionado = 0; //Almacenará el CodCiudad seleccionado de "dd_Ciudades".
            Int32 bolContinua = 0; //Determina si el flujo puede continuar "según código fuente de FONADE clásico".
            String numNit = "0"; //Variable que almacena el nit digitado en la caja de texto "txt_NitCentroInstitucion".
            Int32 CodInstitucion_Cargado = 0; //Variable que almacena el valor de la consulta "MAX(Id_Institucion).
            Int32 codJefeUnidad = 0;//Almacena el valor "CodJefeUnidad" seleccionado para crear la unidad de emprendimiento.
            Int32 codJefeUnidadAnterior = 0; //Almacena la información que tiene la variable de sesión "ID_JEFE_ANTERIOR".
            Int32 CodContacto_resultado_consulta_4 = 0; //Valor obtenido de la consulta "resultado_consulta_4".
            Int32 CodContacto_resultado_consulta_5 = 0; //Valor obtenido de la consulta "resultado_consulta_5".

            txtCargando.Visible = false;
            btn_modificarUnidad.Enabled = true;

            try
            {
                string validado = "";
                validado = ValidarCamposActualizacion();

                if (validado.Trim() == "")
                {
                    #region Flujo de la información

                    #region Obtener valores seleccionados para usarlos desde variables internas.

                    //Convertir el valor seleccionado en "dd_CodTipoInstitucion" en un dato utilizable en el código.
                    CodTipoInstitucion_Seleccionado = Convert.ToInt32(dd_CodTipoInstitucion.SelectedValue);

                    //Convertir el valor seleccionado en "dd_Ciudades" en un dato utilizable en el código.
                    CodCiudad_Seleccionado = Convert.ToInt32(dd_Ciudades.SelectedValue);

                    //Obtener el NIT del centro a crear.
                    numNit = txt_NitCentroInstitucion.Text.Trim();

                    //Este valor se DEBE obtener de la variable de sesión "codJefeUnidad". //Llamada antes como (ID_JEFE_ANTERIOR).
                    //cargada en el método "CargarCamposDetallesSeleccionado()".
                    try
                    {
                        codJefeUnidadAnterior = Convert.ToInt32(Session["codJefeUnidad"].ToString());
                    }
                    catch
                    {
                        Session["codJefeUnidad"] = "0";
                        codJefeUnidadAnterior = codJefeUnidad; //En este caso, debe ser el mismo para evitar errores.
                    }

                    //Este valor se DEBE obtener de la ventana emergente que enviará por sesión el jefe unidad seleccionado.
                    try { codJefeUnidad = Convert.ToInt32(Session["CodNuevoJefe_Seleccionado"].ToString());		/*Convert.ToInt32(Session["codJefeUnidad"].ToString());*/  }
                    catch
                    {
                        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No ha seleccionado el jefe de la unidad.')", true);
                        //return;
                        codJefeUnidad = Int32.Parse(Session["codJefeUnidad"].ToString());
                    }

                    ///Este valor es el Id de la unidad seleccionada y es obtenido de la variable 
                    ///de sesión "Session["Cod_Institucion_Selected"]".
                    try
                    {
                        CodInstitucion_Cargado = Convert.ToInt32(Session["Cod_Institucion_Selected"].ToString());
                    }
                    catch
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No ha seleccionado correctamente la unidad de emprendimiento.')", true);
                        return;
                    }


                    #endregion

                    #region Obtener el nombre de la institución para saber si puede continuar.

                    //Realizar consulta.
                    sqlConsulta = "SELECT NomTipoInstitucion FROM TipoInstitucion WHERE Id_TipoInstitucion = " + CodTipoInstitucion_Seleccionado;

                    //Asignar valor a variable DataTable.
                    var resultado_consulta_1 = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Asigna la siguiente variable con sul valor:
                    bolContinua = 1;

                    //Si contiene datos, continuar con el flujo.
                    if (resultado_consulta_1.Rows.Count > 0)
                    {
                        //Si es "Externa" hace la consulta #2.
                        if (resultado_consulta_1.Rows[0]["NomTipoInstitucion"].ToString() == "Externa")
                        {
                            //Consulta #2.
                            sqlConsulta = "SELECT CodCiudad FROM Institucion WHERE Id_Institucion = " + CodInstitucion_Cargado;

                            var sql_1 = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (sql_1.Rows.Count > 0)
                            {
                                if (sql_1.Rows[0]["CodCiudad"].ToString() != dd_Ciudades.SelectedValue)
                                {
                                    //Consulta #3.
                                    sqlConsulta = " SELECT Nit, CodCiudad FROM Institucion WHERE " +
                                                  " Nit = " + txt_NitCentroInstitucion.Text.Trim() +
                                                  " AND CodCiudad = " + CodCiudad_Seleccionado; //ejecutaReader.

                                    var resultado_consulta_2 = consultas.ObtenerDataTable(sqlConsulta, "text");

                                    if (resultado_consulta_2.Rows.Count > 0)
                                        bolContinua = 0;
                                    else
                                        bolContinua = 1;
                                }
                            }
                        }
                    }

                    #endregion

                    #region Si el valor "bolContinua es igual a 1, realiza la inserción en la tabla "Institucion".

                    if (bolContinua == 1)
                    {
                        #region Consultar el jefe de unidad que tiene la institución seleccionada antes de actualizar.

                        string consulta_jefeUnidad = " SELECT CON.Id_Contacto, CON.Nombres, CON.Apellidos, CON.CodTipoIdentificacion, " +
                                                     " CON.Identificacion, DP.Id_Departamento, CON.CodCiudad " +
                                                     " FROM InstitucionContacto AS IC, Contacto AS CON INNER JOIN Ciudad AS CD " +
                                                     " ON CON.CodCiudad = CD.Id_Ciudad INNER JOIN Departamento DP " +
                                                     " ON CD.CodDepartamento = DP.Id_Departamento " +
                                                     " WHERE IC.CodInstitucion = " + CodInstitucion_Cargado +
                                                     " AND Id_Contacto = CodContacto AND FechaFin IS NULL";

                        var rs = consultas.ObtenerDataTable(consulta_jefeUnidad, "text");

                        if (rs.Rows.Count > 0) { codJefeUnidadAnterior = Int32.Parse(rs.Rows[0]["Id_Contacto"].ToString()); }

                        #endregion

                        #region Hacer actualización.

                        sqlConsulta = " UPDATE Institucion SET " +
                                      " NomInstitucion = '" + txt_NombreCentroInstitucion.Text.Trim() + "', " +
                                      " NomUnidad = '" + txt_nombreUnidad.Text.Trim() + "', " +
                                      " Nit = " + numNit + ", " +
                                      " Direccion = '" + txt_Direccion.Text.Trim() + "', " +
                                      " CodCIudad = " + CodCiudad_Seleccionado + ", " +
                                      " CriteriosSeleccion = '" + txtCriterios.Text.Trim() + "', " +
                                      " CodTipoInstitucion = " + CodTipoInstitucion_Seleccionado + ", " +
                                      " MotivoCambioJefe = '" + txtCambioJefe.Text.Trim() + "', " +
                                      " Telefono = '" + txtTelefonoUnidad.Text.Trim() + "', " +
                                      " Fax = '" + txtFaxUnidad.Text.Trim() + "', " +
                                      " WebSite = '" + txtWebsite.Text.Trim() + "', " +
                                      " inactivo = 0 " +
                                      " WHERE Id_Institucion = " + CodInstitucion_Cargado;

                        //Asignar SqlCommand para su ejecución.
                        cmd = new SqlCommand(sqlConsulta, conn);

                        //Ejecutar inserción.
                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                        if (ejecucion_correcta == false)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (1).')", true);
                            return;
                        }

                        #endregion

                        //Si el JefeUnidad seleccionado es DIFERENTE al JefeUnidad que tenía...
                        if (codJefeUnidadAnterior != codJefeUnidad)
                        {
                            #region Flujo completo de la actualización restante.

                            #region Actualiza los datos del usuario jefe unidad y lo activa si es necesario.

                            sqlConsulta = " UPDATE Contacto SET Inactivo = 0, CodInstitucion = " + CodInstitucion_Cargado + " , " +
                                          " CodCiudad = " + CodCiudad_Seleccionado + " WHERE Id_Contacto = " + codJefeUnidad;

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la unidad de emprendimiento seleccionada (2).')", true);
                                return;
                            }

                            #endregion

                            ///Actualiza la tabla institución contacto, por si el usuario es jefe de otra unidad le coloca 
                            ///la fecha fin de la actual unidad.
                            #region Aplicar lo que dice el comentario de arriba.

                            sqlConsulta = " UPDATE InstitucionContacto SET FechaFin = GETDATE() " +
                                          " WHERE CodContacto = " + codJefeUnidad;

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la unidad de emprendimiento seleccionada (3).')", true);
                                return;
                            }

                            #endregion

                            #region Si la variable "codJefeUnidadAnterior" es diferente de cero, es decir, tiene algún valor...

                            if (codJefeUnidadAnterior != 0)
                            {
                                #region Actualiza la tabla institución contacto, le coloca fecha fin al jefe de la actual unidad.

                                sqlConsulta = " UPDATE InstitucionContacto " +
                                              " SET MotivoCambioJefe = '" + txtCambioJefe.Text.Trim() + "', " +
                                              " FechaFin = GETDATE() WHERE CodContacto = " + codJefeUnidadAnterior;

                                //Asignar SqlCommand para su ejecución.
                                cmd = new SqlCommand(sqlConsulta, conn);

                                //Ejecutar inserción.
                                ejecucion_correcta = EjecutarSQL(conn, cmd);

                                if (ejecucion_correcta == false)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la unidad de emprendimiento seleccionada (4).')", true);
                                    return;
                                }

                                #endregion

                                #region Borra la relación del jefe actual con el grupo jefes de unidad.

                                sqlConsulta = "DELETE FROM GrupoContacto WHERE CodContacto = " + codJefeUnidadAnterior;

                                //Asignar SqlCommand para su ejecución.
                                cmd = new SqlCommand(sqlConsulta, conn);

                                //Ejecutar inserción.
                                ejecucion_correcta = EjecutarSQL(conn, cmd);

                                if (ejecucion_correcta == false)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la unidad de emprendimiento seleccionada (5).')", true);
                                    return;
                                }

                                #endregion

                                #region Inactiva el usuario que va a ser cambiado.

                                sqlConsulta = "UPDATE Contacto SET Inactivo = 1 WHERE Id_Contacto = " + codJefeUnidadAnterior;

                                //Asignar SqlCommand para su ejecución.
                                cmd = new SqlCommand(sqlConsulta, conn);

                                //Ejecutar inserción.
                                ejecucion_correcta = EjecutarSQL(conn, cmd);

                                if (ejecucion_correcta == false)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la unidad de emprendimiento seleccionada (6).')", true);
                                    return;
                                }

                                #endregion
                            }

                            #endregion

                            #region Registra relacion entre Institución Contacto.

                            sqlConsulta = " INSERT INTO InstitucionContacto (CodInstitucion, CodContacto, FechaInicio)" +
                                          " VALUES(" + CodInstitucion_Cargado + ", " + codJefeUnidad + ", GETDATE())";

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (7).')", true);
                                return;
                            }

                            #endregion

                            #region Borra la relación del usuario con cualquier otro grupo, solo puede pertenecer a un grupo.

                            sqlConsulta = "DELETE FROM GrupoContacto WHERE CodContacto = " + codJefeUnidad;

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (8).')", true);
                                return;
                            }

                            #endregion

                            #region Inserta la relación entre el contacto jefe de la unidad y el grupo de usuarios.

                            sqlConsulta = " INSERT INTO GrupoContacto (CodGrupo, CodContacto) " +
                                          " VALUES (" + Constantes.CONST_JefeUnidad + ", " + codJefeUnidad + ")";

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(sqlConsulta, conn);

                            //Ejecutar inserción.
                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                            if (ejecucion_correcta == false)
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (9).')", true);
                                return;
                            }

                            #endregion

                            //Consulta....
                            sqlConsulta = " SELECT CodProyecto, CodRol FROM ProyectoContacto " +
                                          " WHERE FechaFin IS NULL AND CodContacto = " + codJefeUnidad;

                            #region La consulta que REALMENTE se usa...

                            sqlConsulta = " SELECT P.NomProyecto, PC.CodProyecto, PC.CodRol, E.Id_Estado " +
                                          " FROM ProyectoContacto PC,Proyecto P, Estado E " +
                                          " WHERE FechaFin IS NULL " +
                                          " AND PC.CodProyecto = P.Id_Proyecto" +
                                          " AND P.CodEstado = E.Id_estado " +
                                          " AND PC.CodContacto = " + codJefeUnidad;

                            var usedQuery = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (usedQuery.Rows.Count > 0)
                            {
                                #region Ciclo para continuar el flujo de datos.

                                for (int i = 0; i < usedQuery.Rows.Count; i++)
                                {
                                    //Obtener valores temporales.
                                    Int32 Estado_usedQuery = Convert.ToInt32(usedQuery.Rows[i]["Id_Estado"].ToString());
                                    Int32 CodProyecto_usedQuery = Convert.ToInt32(usedQuery.Rows[i]["CodProyecto"].ToString());
                                    String NomProyecto_usedQuery = usedQuery.Rows[i]["NomProyecto"].ToString();

                                    if (Estado_usedQuery < Constantes.CONST_AsignacionRecursos)
                                    {
                                        #region Si al proyecto no le han asignado recursos pasará a REASIGNACION POR ASIGNACION.

                                        sqlConsulta = " UPDATE Proyecto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                      " WHERE Id_Proyecto = " + CodProyecto_usedQuery;

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (10).')", true);
                                            return;
                                        }

                                        #endregion

                                        #region Actualiza la institución de los contactos activos del proyecto.

                                        sqlConsulta = " UPDATE Contacto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                      " WHERE Id_Contacto IN(SELECT CodContacto FROM ProyectoContacto WHERE FechaFin IS NULL " +
                                                      " AND Inactivo = 0 AND CodProyecto = " + CodProyecto_usedQuery + ")";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (11).')", true);
                                            return;
                                        }

                                        #endregion

                                        #region Asigna tarea a jefe de unidad REASIGNACION POR ASIGNACION, para asignación de asesor.

                                        //Consulta:
                                        sqlConsulta = " SELECT CodContacto FROM InstitucionContacto WHERE FechaFin IS NULL " +
                                                      " AND CodInstitucion = " + Constantes.CONST_UnidadTemporal;

                                        //Asignación de los resultados de la consulta anterior a variable DataTable.
                                        var resultado_consulta_4 = consultas.ObtenerDataTable(sqlConsulta, "text");

                                        //Si la tabla anterior contiene datos...
                                        if (resultado_consulta_4.Rows.Count > 0)
                                        {
                                            //Obtener el CodContacto de la consulta "resultado_consulta_4".
                                            CodContacto_resultado_consulta_4 = Convert.ToInt32(resultado_consulta_4.Rows[0]["CodContacto"].ToString());

                                            //Asignar tarea = NO IMPLEMENTADO = código comentado en el clásico. prTareaAsignarAsesor
                                            //Ver línea 1871 de "CatalogoInterventorTMP.aspx" y sucesivas.
                                            //prTareaAsignarAsesor CodContacto_resultado_consulta_4,usuario.Id_Usuario,CodProyecto_usedQuery,NomProyecto_usedQuery
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Si al proyecto ya le han asignado recursos pasara a REASIGNACION POR SEGUIMIENTO.

                                        sqlConsulta = " UPDATE Proyecto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                      " WHERE Id_Proyecto = " + CodProyecto_usedQuery;

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (12).')", true);
                                            return;
                                        }

                                        #endregion

                                        #region Actualiza la institución de los contactos activos del proyecto.

                                        sqlConsulta = " UPDATE Contacto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                      " WHERE Id_Contacto IN(SELECT CodContacto FROM ProyectoContacto " +
                                                      " WHERE FechaFin IS NULL AND Inactivo = 0 AND CodProyecto = " + CodProyecto_usedQuery + ")";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (13).')", true);
                                            return;
                                        }

                                        #endregion

                                        #region Asigna tarea a jefe de unidad REASIGNACION POR ASIGNACION, para asignación de asesor.

                                        //Consulta:
                                        sqlConsulta = " SELECT CodContacto FROM InstitucionContacto WHERE FechaFin IS NULL " +
                                                      " AND CodInstitucion = " + Constantes.CONST_UnidadTemporal;

                                        //Asignación de los resultados de la consulta anterior a variable DataTable.
                                        var resultado_consulta_4 = consultas.ObtenerDataTable(sqlConsulta, "text");

                                        //Si la tabla anterior contiene datos...
                                        if (resultado_consulta_4.Rows.Count > 0)
                                        {
                                            //Obtener el CodContacto de la consulta "resultado_consulta_4".
                                            CodContacto_resultado_consulta_4 = Convert.ToInt32(resultado_consulta_4.Rows[0]["CodContacto"].ToString());

                                            //Asignar tarea = NO IMPLEMENTADO = código comentado en el clásico. prTareaAsignarAsesor
                                            //Ver línea 1871 de "CatalogoInterventorTMP.aspx" y sucesivas.
                                            //prTareaAsignarAsesor CodContacto_resultado_consulta_4,usuario.Id_Usuario,CodProyecto_usedQuery,NomProyecto_usedQuery
                                        }


                                        #endregion
                                    }

                                    #region Cierra la relacion entre el usuario y el proyecto.

                                    sqlConsulta = " UPDATE ProyectoContacto SET FechaFin = GETDATE(), Inactivo = 1 " +
                                                  " WHERE CodProyecto = " + CodProyecto_usedQuery +
                                                  " AND CodContacto = " + codJefeUnidad + " AND Inactivo = 0";

                                    //Asignar SqlCommand para su ejecución.
                                    cmd = new SqlCommand(sqlConsulta, conn);

                                    //Ejecutar inserción.
                                    ejecucion_correcta = EjecutarSQL(conn, cmd);

                                    if (ejecucion_correcta == false)
                                    {
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (14).')", true);
                                        return;
                                    }

                                    #endregion
                                }


                                #endregion
                            }

                            #endregion

                            #region Si hay cambio de jefe, entonces las tareas pendientes pasan del anterior jefe al nuevo jefe.

                            if (codJefeUnidadAnterior != 0)
                            {
                                #region Reasignamos las tareas entre los usuarios (Tareas generadas para el usuario que se va a eliminar.

                                sqlConsulta = " SELECT Id_TareaUsuario FROM TareaUsuario TUC, TareaUsuarioRepeticion TUCR " +
                                              " WHERE TUC.Id_TareaUsuario = TUCR.CodTareaUsuario " +
                                              " AND TUCR.FechaCierre IS NULL AND TUC.CodContacto =" + codJefeUnidadAnterior; //cdbl(CodJefeUnidadAnterior);

                                var tareasAgendadasPara = consultas.ObtenerDataTable(sqlConsulta, "text");

                                if (tareasAgendadasPara.Rows.Count > 0)
                                {
                                    #region Actualización PENDIENTE!!!! LÍNEA 818. -- Corregida el 22/05/2014.

                                    for (int j = 0; j < tareasAgendadasPara.Rows.Count; j++)
                                    {
                                        sqlConsulta = " UPDATE TareaUsuario SET CodContacto=" + codJefeUnidad +
                                                      " WHERE Id_tareaUsuario = " + tareasAgendadasPara.Rows[j]["Id_TareaUsuario"].ToString();

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar inserción.
                                        ejecucion_correcta = EjecutarSQL(conn, cmd);

                                        if (ejecucion_correcta == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la unidad de emprendimiento seleccionada (#818).')", true);
                                            return;
                                        }
                                    }


                                    #endregion

                                    #region Reasignamos las tareas entre los usuarios (Tareas generadas por el usuario)

                                    sqlConsulta = " SELECT Id_TareaUsuario FROM TareaUsuario TUC, TareaUsuarioRepeticion TUCR " +
                                                  " WHERE TUC.Id_TareaUsuario = TUCR.CodTareaUsuario " +
                                                  " AND TUCR.FechaCierre IS NULL AND TUC.CodContactoAgendo = " + codJefeUnidadAnterior; //& cdbl(CodJefeUnidadAnterior)

                                    var tareasAgendadasPor = consultas.ObtenerDataTable(sqlConsulta, "text");

                                    if (tareasAgendadasPor.Rows.Count > 0)
                                    {
                                        #region Actualización PENDIENTE!!!! LÍNEA 837. -- Corregida el 22/05/2014.

                                        foreach (DataRow row_tareaPor in tareasAgendadasPor.Rows)
                                        {
                                            sqlConsulta = " UPDATE TareaUsuario SET CodContactoAgendo =" + codJefeUnidad +
                                                          " WHERE Id_tareaUsuario = " + row_tareaPor["Id_TareaUsuario"].ToString();

                                            //Asignar SqlCommand para su ejecución.
                                            cmd = new SqlCommand(sqlConsulta, conn);

                                            //Ejecutar inserción.
                                            ejecucion_correcta = EjecutarSQL(conn, cmd);

                                            if (ejecucion_correcta == false)
                                            {
                                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la unidad de emprendimiento seleccionada (#837).')", true);
                                                return;
                                            }
                                        }

                                        #endregion
                                    }

                                    #endregion
                                }

                                #endregion
                            }

                            #endregion

                            #region Trae los datos del usuario jefe para el envio del email

                            sqlConsulta = " SELECT Nombres, Apellidos, Email, Clave " +
                                          " FROM Contacto WHERE Id_Contacto = " + codJefeUnidad;

                            var tabla_final = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (tabla_final.Rows.Count > 0)
                            {
                                for (int j = 0; j < tabla_final.Rows.Count; j++)
                                {
                                    #region Obtener valores de la consulta anterior para generar mensajes masivos.

                                    bool Enviado = false; //Establece si el mensaje fue o no enviado.
                                    String claveObtenida = ""; //Variable que almacena la clave obtenida de la consulta anterior.
                                    String Email_obtenido = ""; //Variable que almacena el email obtenido de la consulta anterior.
                                    String Nombre_obtenido = ""; //Variable que almacena el nombre obtenido de la consulta anterior.
                                    String Apellidos_obtenidos = ""; //Variable que almacena el nombre obtenido de la consulta anterior.

                                    //Obtener el valor "Clave" de "tabla_final".
                                    claveObtenida = tabla_final.Rows[j]["Clave"].ToString();

                                    //Obtener el valor "Email" de "tabla_final".
                                    Email_obtenido = tabla_final.Rows[j]["Email"].ToString();

                                    //Obtener el valor "Nombres" de "tabla_final".
                                    Nombre_obtenido = tabla_final.Rows[j]["Nombres"].ToString();

                                    //Obtener el valor "Nombres" de "tabla_final".
                                    Apellidos_obtenidos = tabla_final.Rows[j]["Apellidos"].ToString();

                                    #endregion

                                    #region Enviar el Email al usuario.

                                    //Variable que almacena el texto obtenido de la consulta a la tabla "Texto".
                                    String Texto_Obtenido = "";

                                    //Consultar el "TEXTO".
                                    Texto_Obtenido = Texto("TXT_EMAILENVIOCLAVE");

                                    //Sólo por si acaso, si el resultado de "Texto_Obtenido" NO devuelve los datos según el texto esperado,
                                    //se debe asignar el texto tal cual se vió en BD el "28/04/2014".
                                    if (Texto_Obtenido.Contains("Señor Usuario") || Texto_Obtenido.Trim() == null)
                                    {
                                        Texto_Obtenido = "Señor Usuario Con el usuario {{Email}} y contraseña {{Clave}},  podrá acceder al sistema de información por medio de la pagina www.fondoemprender.com,  allí encontrara en la parte superior del sistema específicamente en el botón con el signo de interrogación  (?) el manual de su perfil ''{{Rol}}''";
                                    }

                                    //Reemplazar determinados caracteres por caracteres definidos específicamente para esta acción.
                                    Texto_Obtenido = Texto_Obtenido.Replace("{{Rol}}", "Jefe Unidad");
                                    Texto_Obtenido = Texto_Obtenido.Replace("{{Email}}", Email_obtenido);
                                    Texto_Obtenido = Texto_Obtenido.Replace("{{Clave}}", claveObtenida);

                                    try
                                    {
                                        //Generar y enviar mensaje.
                                        Correo correo = new Correo(usuario.Email,
                                                                   "Fondo Emprender",
                                                                   Email_obtenido,
                                                                   Nombre_obtenido + " " + Apellidos_obtenidos,
                                                                   "Registro a Fondo Emprender",
                                                                   Texto_Obtenido);
                                        correo.Enviar();

                                        //El mensaje fue enviado.
                                        Enviado = true;

                                        //Inserción en tabla "LogEnvios".
                                        prLogEnvios("Registro a Fondo Emprender", usuario.Email, Email_obtenido, "Catálogo de Unidad", 0, Enviado);

                                        //Finalmente se redirige al usuario a "FiltroAsesorInactivo.aspx".
                                        //Response.Redirect("CatalogoUnidadEmprende.aspx"); //Comentado el 02/05/2014. -- Reemplazado el 22/05/2014.
                                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.location.reload(false); ", true);
                                    }
                                    catch
                                    {
                                        //El mensaje no pudo ser enviado.
                                        Enviado = false;

                                        //Inserción en tabla "LogEnvios".
                                        prLogEnvios("Registro a Fondo Emprender", usuario.Email, Email_obtenido, "Catálogo de Unidad", 0, Enviado);

                                        //Finalmente se redirige al usuario a "FiltroAsesorInactivo.aspx".
                                        //Response.Redirect("CatalogoUnidadEmprende.aspx"); //Comentado el 02/05/2014. -- Reemplazado el 22/05/2014.
                                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.location.reload(false); ", true);
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            #endregion
                        }
                        //Si no, no hace nada.

                        //Finalizar el ciclo declarando la variable de sesión con el valor "false".
                        Session["bRepetido"] = "false";
                        //Response.Redirect("CatalogoUnidadEmprende.aspx");
                        //System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.location.reload(false); ", true);
                    }
                    else
                    {
                        Session["bRepetido"] = "true";
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + validado + "')", true);
                    return;
                }

                pnlPrincipal.Visible = true;
                EvaluarEnunciado("");
                Session["OPEN"] = null;
                Session["Cod_Institucion_Selected"] = null;
                return;
            }
            catch (Exception ex)
            {
                string h = ex.Message;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo actualizar la unidad de emprendimiento.')", true);
                pnlPrincipal.Visible = true;
                EvaluarEnunciado("");
                Session["OPEN"] = null;
                Session["Cod_Institucion_Selected"] = null;
                return;
            }
        }

        #endregion

        #region Eventos de la grilla "gv_UnidadesEmprendimiento".

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Establecer valor de visualización dependiendo del valor cargado en el label "lbl_estado".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_UnidadesEmprendimiento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Inicializar variables e instancias de controles.
                var lbl_inst = e.Row.FindControl("lbl_id_inst") as Label; //Id de la institución.
                var lbl_inact = e.Row.FindControl("lbl_estado") as Label; //Contiene el valor de la columan "inactivo".
                //var labelActividadPo = e.Row.FindControl("lblactividaPOI") as Label;//Controles de eliminación.
                //var lnk = e.Row.FindControl("lnkeliminar") as LinkButton; //Controles de eliminación.
                //var imgEditar = e.Row.FindControl("imgeditar") as Image;  //Controles de eliminación.
                var img = e.Row.FindControl("img_btn") as ImageButton;
                int valor_inactivo = 0;

                try
                {
                    if (lbl_inst != null && lbl_inact != null)
                    {
                        //Convertir el valor del texto del label en un dato operable para la siguiente condición.
                        valor_inactivo = Convert.ToInt32(lbl_inst.Text);

                        if (valor_inactivo != Constantes.CONST_UnidadTemporal)
                        {
                            if (lbl_inact.Text == "False") //0
                            {
                                //Mostrar información.
                                img.Visible = true;
                                img.Visible = true;
                                img.ImageUrl = "../../Images/icoBorrar.gif";
                                img.AlternateText = "Desactivar Unidad";
                                lbl_inact.Text = "Activa";
                            }
                            else
                            {
                                //Mostrar información según FONADE clásico.
                                img.Visible = true;
                                img.Visible = true;
                                img.ImageUrl = "../../Images/icoActivar.gif";
                                img.AlternateText = "Activar Unidad";
                                lbl_inact.Text = "Inactiva";
                            }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Evento RowCommand para establecer acción dependiendo del CommandName seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_UnidadesEmprendimiento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                #region Editar.
                try
                {
                    //Mostrar el panel detalles con la información seleccionada y seguir el flujo correspondiente.
                    pnlPrincipal.Visible = false;
                    pnl_detalles.Visible = true;
                    EvaluarEnunciado("");

                    #region Cargar la información completa de la unidad de emprendimiento seleccionada.

                    //Separar los valores "Id_institucion" y "NomInstitucion".
                    var valores_command = new string[] { };
                    valores_command = e.CommandArgument.ToString().Split(';');

                    Int32 Codigo_Procesado = Convert.ToInt32(valores_command[0].ToString());

                    //Ingresar valor en variable de sesión para obtenerlo mas adelante en el método "ActualizarUnidadEmprendimiento".
                    Session["Cod_Institucion_Selected"] = Codigo_Procesado.ToString();
                    CargarCamposDetallesSeleccionado(Codigo_Procesado);

                    #endregion
                }
                catch
                {
                    pnl_detalles.Visible = false;
                    pnlPrincipal.Visible = true;
                    EvaluarEnunciado("");
                    return;
                }
                #endregion
            }
            if (e.CommandName == "eliminar")
            {
                #region Eliminar.

                //Instanciar el control seleccionado.
                ImageButton img = e.CommandSource as ImageButton;

                if (img != null)
                {
                    //Separar los valores "Id_institucion" y "NomInstitucion".
                    var valores_command = new string[] { };
                    valores_command = e.CommandArgument.ToString().Split(';');

                    if (img.ImageUrl.Contains("icoBorrar.gif"))
                    {
                        #region Redirigir al usuario a "DesactivarUnidadEmprende.aspx".

                        Session["Cod_Institucion_Selected"] = valores_command[0];
                        Session["NombreUnidad_Seleccionado"] = valores_command[1];

                        Redirect(null, "DesactivarUnidadEmprende.aspx", "_blank",
                            "menubar=0,scrollbars=1,width=360,height=200,top=100");

                        #endregion
                    }
                    else
                    {
                        #region Se muestra la edición del valor seleccionado.

                        //Mostrar el panel detalles con la información seleccionada y seguir el flujo correspondiente.
                        pnlPrincipal.Visible = false;
                        pnl_detalles.Visible = true;
                        EvaluarEnunciado("");

                        #region Cargar la información completa de la unidad de emprendimiento seleccionada.

                        //Separar los valores "Id_institucion" y "NomInstitucion".
                        valores_command = new string[] { };
                        valores_command = e.CommandArgument.ToString().Split(';');

                        Int32 Codigo_Procesado = Convert.ToInt32(valores_command[0].ToString());

                        //Ingresar valor en variable de sesión para obtenerlo mas adelante en el método "ActualizarUnidadEmprendimiento".
                        Session["Cod_Institucion_Selected"] = Codigo_Procesado.ToString();
                        CargarCamposDetallesSeleccionado(Codigo_Procesado);

                        #endregion

                        #endregion
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Sortear la grilla por "Nombre Unidad" o "Tipo".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_UnidadesEmprendimiento_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gv_UnidadesEmprendimiento.DataSource = Session["dtEmpresas"];
                gv_UnidadesEmprendimiento.DataBind();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Paginación de la grilla "gv_UnidadesEmprendimiento", enviándole el filtro seleccionado previamente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_UnidadesEmprendimiento_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_UnidadesEmprendimiento.PageIndex = e.NewPageIndex;
            try { CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString()); }
            catch { CargarUnidadesEmprendimiento("A"); }
        }

        #endregion

        #region Métodos de selección del abecedario.

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "%" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_todos_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "%";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "A" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_A_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "A";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "B" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_B_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "B";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "C" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_C_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "C";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "D" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_D_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "D";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "E" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_E_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "E";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "F" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_F_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "F";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "G" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_G_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "G";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "H" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_H_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "H";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "I" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_I_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "I";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "J" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_J_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "J";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "K" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_K_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "K";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "L" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_L_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "L";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "M" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_M_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "M";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "N" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_N_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "N";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "O" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_O_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "O";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "P" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_P_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "P";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "Q" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_Q_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "Q";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "R" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_R_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "R";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "S" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_S_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "S";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "T" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_T_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "T";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "U" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_U_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "U";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "V" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_V_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "V";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "W" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_W_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "W";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "X" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_X_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "X";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "Y" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_Y_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "Y";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Al seleccionar este LinkButton, se establecerá el valor "Z" a
        /// la variable de sesión "upper_letter_selected".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_opcion_Z_Click(object sender, EventArgs e)
        {
            Session["upper_letter_selected"] = "Z";
            CargarUnidadesEmprendimiento(Session["upper_letter_selected"].ToString());
        }

        #endregion

        #region Eventos de los DropDownLists.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/04/2014.
        /// Al seleccionar el departamento, se generará el listado de ciudades correspondientes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dd_SelDpto2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            { int valor_depto = Convert.ToInt32(dd_SelDpto2.SelectedValue); CargarCiudades(valor_depto); }
            catch { CargarCiudades(0); }
        }

        #endregion

        #region Métodos SQL.

        /// <summary>
        /// Mauricio Arias Olave.
        /// Ejecutar SQL.
        /// Método que recibe la conexión y la consulta SQL y la ejecuta.
        /// </summary>
        /// <param name="p_connection">Conexión</param>
        /// <param name="p_cmd">Consulta SQL.</param>
        /// <returns>TRUE = Sentencia SQL ejecutada correctamente. // FALSE = Error.</returns>
        private bool EjecutarSQL(SqlConnection p_connection, SqlCommand p_cmd)
        {
            //Ejecutar controladamente la consulta SQL.
            try
            {
                p_connection.Open();
                p_cmd.ExecuteReader();
                p_connection.Close();
                return true;
            }
            catch (Exception) { return false; }
            //finally
            //{ p_connection.Close(); }
        }

        #endregion

        #region Métodos del formulario.

        /// <summary>
        /// Botón que llama al evento "CrearUnidadEmprendimiento()".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_crearUnidad_Click(object sender, EventArgs e)
        {
            CrearUnidadEmprendimiento();
        }

        /// <summary>
        /// Botón que llama al evento "ActualizarUnidadEmprendimiento()".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_modificarUnidad_Click(object sender, EventArgs e)
        {
            txtCargando.Visible = true;
            btn_modificarUnidad.Enabled = false;
            ActualizarUnidadEmprendimiento();

            pnlPrincipal.Visible = true;
            pnl_detalles.Visible = false;
            lbl_enunciado.Text = "UNIDADES DE EMPRENDIMIENTO";
            CargarUnidadesEmprendimiento("A");
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 02/05/2014.
        /// Actualizar la información del Jefe de Unidad de la unidad de emprendimiento seleccionada.
        /// Llama a la ventana emergente "CambiarDatosJefe.aspx" que contiene dicha funcionalidad.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_cambiarDatosJefe_Click(object sender, EventArgs e)
        {
            Redirect(null, "CambiarDatosJefe.aspx", "_blank",
                    "menubar=0,scrollbars=1,width=565,height=264,top=100");
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 02/05/2014.
        /// Buscar la información del jefe de unidad a asignar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_buscarJefeUnidad_Click(object sender, EventArgs e)
        {
            //Abrir ventana de "SeleccionarJefeUnidad.aspx".
            Redirect(null, "SeleccionarJefeUnidad.aspx", "_blank",
                    "menubar=0,scrollbars=1,width=420,height=270,top=100");
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 22/05/2014.
        /// Activar ventana "SeleccionarJefeUnidad".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnk_invocar_ventanaModal_Click(object sender, EventArgs e)
        {
            Redirect(null, "SeleccionarJefeUnidad.aspx", "_blank",
                    "menubar=0,scrollbars=1,width=420,height=270,top=100");
        }

        #endregion
    }
}