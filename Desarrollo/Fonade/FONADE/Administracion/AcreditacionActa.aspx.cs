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

namespace Fonade.FONADE.Administracion
{
    public partial class AcreditacionActa : Base_Page
    {
        #region Variables globales.

        /// <summary>
        /// Código del proyecto "seleccionado de los detalles del acta principal seleccionado".
        /// TENER EN CUENTA: Se encuentra en la consulta de la grilla de detalles.
        /// </summary>
        private Int32 CodProyecto_Seleccionado;

        /// <summary>
        /// Código del acta seleccionado "se presume que viajará por Session".
        /// Cargado en el RowCommand de "gv_ResultadosActas".
        /// </summary>
        private Int32 CodActa_Seleccionado;

        /// <summary>
        /// CodActa que se obtiene al ejecutar el método "CrearActa"
        /// </summary>
        private Int32 CodActa_ConsultadoCreacion;

        /// <summary>
        /// Código de la convocatoria cargado al llamar al método "CargarDetallesCamposActaSeleccionado".
        /// Cargardo en el método "CargarDetallesCamposActaSeleccionado".
        /// </summary>
        private Int32 CodConvocatoria_Cargado;

        /// <summary>
        /// Indica "según el resultado de la primera consulta, si el valor existe o no.
        /// </summary>
        Boolean repetido;

        /// <summary>
        /// Boolean, Indica si el acta esta publicada o no.
        /// </summary>
        Boolean bPublicado;

        /// <summary>
        /// Valor que se concatena en el texto al seleccionar el acta de gv_ResultadosActas".
        /// </summary>
        String strtxtActaAcreditada;

        /// <summary>
        /// Determina si el acta está o no acreditada.
        /// </summary>
        Boolean bActaAcreditada;

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Page_Load.
        /// Si algo le falta el IsPostBack.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Establecer el título de la página actual.
                this.Page.Title = "FONADE - ";
                if (!IsPostBack)
                {
                    GenerarFecha_Year();
                    CargaGrillaActas_Inicial();
                    CargarListadoConvocatorias();
                    EvaluarEnunciado();
                    DateTime fecha = DateTime.Today;
                    dd_fecha_dias_Memorando.SelectedValue = fecha.Day.ToString();
                    dd_fecha_mes_Memorando.SelectedValue = fecha.Month.ToString();
                    dd_fecha_year_Memorando.SelectedValue = fecha.Year.ToString();
                }
            }
            catch { Response.Redirect("~/Account/Login.aspx"); }
        }

        #region Métodos generales.

        /// <summary>
        /// Se debe enviar la información de la tabla en uan variable se sesión
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
        /// 28/04/2014.
        /// Cargar inicialmente la grilla con los datos que se muestran
        /// al seleccionar la opción "CREAR ACTA PARCIAL DE ACREDITACIÓN" del menú.
        /// <param name="added_sql">Parámetro opcional "se usa para enviarle el tipo de actas a cargar."</param>
        /// </summary>
        private void CargaGrillaActas_Inicial(string added_sql = null)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String txtSQL = "";

            try
            {
                //Consulta.
                txtSQL = " SELECT Id_Acta, NumActa, NomActa, NomConvocatoria, FechaActa, e.publicado , ActaAcreditada " +
                              " FROM AcreditacionActa e, Convocatoria " +
                              " WHERE Id_Convocatoria = CodConvocatoria " + added_sql +
                              " ORDER BY NumActa"; //Inicialmente se debe establecer este ORDER BY.

                //Asignar resultados de la consulta a variable DataTable.
                var tabla_inicial = consultas.ObtenerDataTable(txtSQL, "text");

                //Si tiene datos, lo bindea.
                if (tabla_inicial.Rows.Count > 0)
                {
                    Session["dtEmpresas"] = tabla_inicial;
                    gv_resultadosActas.DataSource = tabla_inicial;
                    gv_resultadosActas.DataBind();
                }
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al cargar las actas.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/04/2014.
        /// Dependiendo de qué panel está visible, se establecen ciertos valores.
        /// </summary>
        private void EvaluarEnunciado()
        {
            if (pnlPrincipal.Visible == true)
            {
                pnl_detalles.Visible = false;
                lbl_enunciado.Text = "ACTAS PARCIALES DE ACREDITACIÓN";
            }
            if (pnl_detalles.Visible == true)
            {
                pnlPrincipal.Visible = false;
                lbl_enunciado.Text = "CREAR ACTA";
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/04/2014.
        /// Generar DropDownList con los valores de fecha "según el FONADE Clásico", tomando como base
        /// el año 2004 (en C#, se hace desde el 2003 para que de acuerdo a las pruebas en FONADE clásico, las
        /// fechas terminen en 2019) y agregándole 5 años.
        /// </summary>
        private void GenerarFecha_Year()
        {
            try
            {
                int currentYear = DateTime.Today.AddYears(-11).Year;
                int futureYear = DateTime.Today.AddYears(5).Year;

                for (int i = currentYear; i < futureYear; i++)
                {
                    ListItem item = new ListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    dd_fecha_year_Memorando.Items.Add(item);
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Cargar el listado de convocatorias al DropDownList "dd_convocatoriasSeleccionables".
        /// </summary>
        private void CargarListadoConvocatorias()
        {
            //Inicializa variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String txtSQL = "";

            try
            {
                //Consulta.
                txtSQL = " SELECT Id_Convocatoria, NomConvocatoria " +
                              " FROM Convocatoria WHERE Publicado = 1  ORDER BY NomConvocatoria ";

                //Asignar resultados de la consulta a variable de tipo DataTable.
                var Listado = consultas.ObtenerDataTable(txtSQL, "text");

                //Si tiene datos "y debe tener", se llena el DropDownList "dd_convocatoriasSeleccionables".
                if (Listado.Rows.Count > 0)
                {
                    //Recorrer resultados para generar DropDownList.
                    for (int i = 0; i < Listado.Rows.Count; i++)
                    {
                        ListItem item = new ListItem();
                        item.Value = Listado.Rows[i]["Id_Convocatoria"].ToString();
                        item.Text = Listado.Rows[i]["NomConvocatoria"].ToString();
                        dd_convocatoriasSeleccionables.Items.Add(item);
                    }
                }
            }
            catch
            {
                //Ocultar panel y re-evaluar los campos "porque hubo un error no controlado".
                pnl_detalles.Visible = false;
                pnlPrincipal.Visible = true;
                //CargarMemorandoLegalizaciones();
                EvaluarEnunciado();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al cargar el listado de convocatorias.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Habilitar o deshabilitar campos según el valor TRUE/FALSE del parámetro "habilitado".
        /// </summary>
        /// <param name="habilitado">Los campos estarán habilitados = "TRUE" o deshabilitados = "FALSE".</param>
        private void HabilitarCampos(bool habilitado)
        {
            txt_noActaSeleccionado.Enabled = habilitado;
            txt_NomActaSeleccionado.Enabled = habilitado;
            dd_fecha_dias_Memorando.Enabled = habilitado;
            dd_fecha_mes_Memorando.Enabled = habilitado;
            dd_fecha_year_Memorando.Enabled = habilitado;
            txt_observaciones.Enabled = habilitado;
            dd_convocatoriasSeleccionables.Enabled = habilitado;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Mostrar los campos para crear acta (acreditada o NO acreditada).
        /// </summary>
        /// <param name="TituloEnunciadoActa">Título que se mostrará segpun la elección (ACTA DE ACREDITACIÓN o ACTA DE NO ACREDITACIÓN).</param>
        private void MostrarCamposCrearActa(string TituloEnunciadoActa)
        {
            pnlPrincipal.Visible = false;
            pnl_detalles.Visible = true;
            lbl_enunciado_acta.Text = "CREAR ACTA";
            lbl_enunciado_acta.ForeColor = System.Drawing.Color.Red;
            lbl_enunciado_acta.Text = TituloEnunciadoActa;
            //Habilitar campos.
            HabilitarCampos(true);
            EvaluarEnunciado();
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Al seleccionarse un elemento del GridView "gv_resultadosActas", se 
        /// cargará la información de "solo-lectura", para posteriormente imprimirla.
        /// Actualización 28/04/2014: Se carga la información también en los campos de 
        /// impresión.
        /// </summary>
        private void CargarDetallesCamposActaSeleccionado(Int32 CodActa_Seleccionado)
        {
            //Inicializar variables.
            String txtSQL;
            DateTime fecha_Detalle = new DateTime();
            Int32 d;
            Int32 m;
            Int32 y;

            try
            {
                if (CodActa_Seleccionado != 0)
                {
                    txtSQL = " SELECT NumActa, NomActa, FechaActa, Observaciones, CodConvocatoria, Publicado, ActaAcreditada " +
                                  " FROM AcreditacionActa where Id_Acta = " + CodActa_Seleccionado;

                    var dtEmpresas = consultas.ObtenerDataTable(txtSQL, "text");

                    if (dtEmpresas.Rows.Count > 0)
                    {
                        //Deshabilitar campos.
                        HabilitarCampos(false);

                        Session["dtEmpresas"] = dtEmpresas;
                        //Asignar los resultados de la tabla a los campos de "solo-lectura".
                        txt_noActaSeleccionado.Text = dtEmpresas.Rows[0]["NumActa"].ToString();
                        sp_noActa.InnerText = dtEmpresas.Rows[0]["NumActa"].ToString();
                        txt_NomActaSeleccionado.Text = dtEmpresas.Rows[0]["NomActa"].ToString();
                        sp_Nombre.InnerText = dtEmpresas.Rows[0]["NomActa"].ToString();
                        txt_observaciones.Text = dtEmpresas.Rows[0]["Observaciones"].ToString();
                        sp_observaciones.InnerText = dtEmpresas.Rows[0]["Observaciones"].ToString();
                        bPublicado = Boolean.Parse(dtEmpresas.Rows[0]["Publicado"].ToString());
                        bActaAcreditada = Boolean.Parse(dtEmpresas.Rows[0]["ActaAcreditada"].ToString());
                        panel_AddPlanes.Visible = bActaAcreditada;

                        //Si está publicado, se bloquearán los campos.
                        if (bPublicado)
                        {
                            txt_noActaSeleccionado.Enabled = false;
                            txt_NomActaSeleccionado.Enabled = false;
                            dd_fecha_dias_Memorando.Enabled = false;
                            dd_fecha_mes_Memorando.Enabled = false;
                            dd_fecha_year_Memorando.Enabled = false;
                            txt_observaciones.Enabled = false;
                            dd_convocatoriasSeleccionables.Enabled = false;
                            //Mostrar botón de imprimir.
                            btn_imprimirMemorando.Visible = true;
                        }
                        else
                        {
                            Btn_crearActa.Text = "Actualizar";
                            Btn_crearActa.Visible = true;
                        }

                        if (!bActaAcreditada) { strtxtActaAcreditada = " NO "; Session["bActaAcreditada"] = "0"; } else { strtxtActaAcreditada = ""; Session["bActaAcreditada"] = "1"; }

                        //Establecer título.
                        lbl_enunciado_acta.Text = "ACTA DE " + strtxtActaAcreditada + "ACREDITACIÓN";

                        //Obtener la fecha en variable DateTime, obteniendo los valores (d/m/a) en variables separadas.
                        fecha_Detalle = Convert.ToDateTime(dtEmpresas.Rows[0]["FechaActa"].ToString());
                        d = fecha_Detalle.Day; m = fecha_Detalle.Month; y = fecha_Detalle.Year;

                        //Asginando el valor seleccionado de acuerdo con la información de las variables anteriores.
                        dd_fecha_dias_Memorando.SelectedValue = d.ToString();
                        dd_fecha_mes_Memorando.SelectedValue = m.ToString();
                        dd_fecha_year_Memorando.SelectedValue = y.ToString();

                        sp_FechaFormateada.InnerText = dd_fecha_mes_Memorando.SelectedItem.Text + " " + dd_fecha_dias_Memorando.SelectedValue + " de " + dd_fecha_year_Memorando.SelectedValue;

                        //Añadir el valor "CodConvocatoria" a la variable global interna "CodConvocatoria_Cargado" para ser
                        //usada en el método "CargarGrillaDetallesActaSeleccionado".
                        CodConvocatoria_Cargado = Convert.ToInt32(dtEmpresas.Rows[0]["CodConvocatoria"].ToString());
                        Session["CodConvocatoria_Acta"] = CodConvocatoria_Cargado;

                        //Asignar valor del código de la convocatoria obtenido.
                        Session["CODCONVOCATORIAAcreditar"] = CodConvocatoria_Cargado;//CodConvocatoria_Session

                        //Establecer la selección de DropDowList "dd_convocatoriasSeleccionables" según el valor obtenido.
                        dd_convocatoriasSeleccionables.SelectedValue = dtEmpresas.Rows[0]["CodConvocatoria"].ToString();

                        //Obtener el valor "CodConvocatoria".
                        sp_convocatoria.InnerText = "  " + dd_convocatoriasSeleccionables.SelectedItem.Text;
                        //dtEmpresas.Rows[0]["CodConvocatoria"].ToString();
                    }
                }
                else
                {
                    //Ocultar panel y re-evaluar los campos "porque NO se ha seleccionado el CodActa".
                    pnl_detalles.Visible = false;
                    pnlPrincipal.Visible = true;
                    //CargarMemorandoLegalizaciones();
                    EvaluarEnunciado();
                    return;
                }
            }
            catch
            {
                //Ocultar panel y re-evaluar los campos "porque hubo un error no controlado".
                pnl_detalles.Visible = false;
                pnlPrincipal.Visible = true;
                //CargarMemorandoLegalizaciones();
                EvaluarEnunciado();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en 'CargarDetallesCamposActaSeleccionado'.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Cargar la grilla de detalles del CodActa seleccionado.
        /// </summary>
        private void CargarGrillaDetallesActaSeleccionado(Int32 CodActa_Seleccionado)
        {
            //Parámetros:
            /*
             * @CodActa = INT "Código del acta seleccionado de la grilla principal".
             * @codConvocatoria = INT "Código de la convocatoria del acta seleccionado de la grilla principal".
             */

            try
            {
                consultas.Parameters = new[]
                                           {
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@CodActa", Value = CodActa_Seleccionado //CodActa
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@codConvocatoria", Value = CodConvocatoria_Cargado //codConvocatoria
                                                   }
                                           };

                DataTable dtEmpresas = consultas.ObtenerDataTable("pr_ProyectosAcreditados");

                Session["dtEmpresas"] = dtEmpresas;
                gv_imprimir_planesNegocio.DataSource = dtEmpresas;
                gv_imprimir_planesNegocio.DataBind();
                gv_DetallesActa.DataSource = dtEmpresas;
                gv_DetallesActa.DataBind();

                //Destruir variables al final de generar la consulta.
                consultas.Parameters = null;
            }
            catch
            {
                //Ocultar panel y re-evaluar los campos "porque hubo un error no controlado".
                pnl_detalles.Visible = false;
                pnlPrincipal.Visible = true;
                //CargarMemorandoLegalizaciones();
                EvaluarEnunciado();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en 'CargarGrillaDetallesActaSeleccionado'.')", true);
                return;
            }
        }

        #endregion

        #region Métodos de filtros ("Listar todas las Actas", "Actas Acreditadas" o "Actas NO Acreditadas").

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Dependiendo del valor seleccionado, ejecutará la consulta correspondiente de las actas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dd_tiposActa_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Inicializar variables.
            String txtSQL = "";

            if (dd_tiposActa.SelectedValue == "")
            {
                //Ejecuta la consulta "Listar todas las Actas".
                CargaGrillaActas_Inicial("");
            }
            if (dd_tiposActa.SelectedValue == "1")
            {
                //Ejecuta la consulta "Actas Acreditadas".
                CargaGrillaActas_Inicial(" AND ActaAcreditada = 1 ");
            }
            if (dd_tiposActa.SelectedValue == "0")
            {
                //Ejecuta la consulta "Actas NO Acreditadas".
                CargaGrillaActas_Inicial(" AND ActaAcreditada = 0 ");
            }
        }

        #endregion

        #region Actas de acreditación.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Mostrar campos para la creación del acta acreditada (ImageButton).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_btn_AddActaAcreditacion_Click(object sender, ImageClickEventArgs e)
        {
            MostrarCamposCrearActa("ACTA DE ACREDITACIÓN");
            Session["EsAcreditada_Valor"] = "1";
        }

        /// <summary>
        /// Mauricio Arias asOlave.
        /// 29/04/2014.
        /// Mostrar campos para la creación del acta acreditada (LinkButton).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnk_btn_AddActaAcreditacion_Click(object sender, EventArgs e)
        {
            MostrarCamposCrearActa("ACTA DE ACREDITACIÓN");
            Session["EsAcreditada_Valor"] = "1";
        }

        #endregion

        #region Actas de NO acreditación.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Mostrar campos para la creación del acta NO acreditada (ImageButton).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_btn_AddActa_NO_Acreditacion_Click(object sender, ImageClickEventArgs e)
        {
            MostrarCamposCrearActa("ACTA DE NO ACREDITACIÓN");
            Session["EsAcreditada_Valor"] = "0";
        }

        /// <summary>
        /// Mauricio Arias asOlave.
        /// 29/04/2014.
        /// Mostrar campos para la creación del acta NO acreditada (LinkButton).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnk_btn_AddActa_NO_Acreditacion_Click(object sender, EventArgs e)
        {
            MostrarCamposCrearActa("ACTA DE NO ACREDITACIÓN");
            Session["EsAcreditada_Valor"] = "0";
        }

        #endregion

        #region Inserción.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Crear Acta (if Request("Accion") = "Crear").
        /// COLOCAR CAMPOS Y DESCOMENTAR CONTENIDO DEL CÓDIGO!
        /// Actualización "29/04/2014": Se ha descomentado el código, pero se esperan los resultados de las pruebas para
        /// confirmar realmente la funcionalidad del código.
        /// <param name="EsAcreditada">Valor que indica si el acta está acreditada = "1", si NO es acreditada = "0".</param>
        /// </summary>
        private void CrearActa(Int32 EsAcreditada)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String txtSQL = "";
            DateTime strtxtFecha = new DateTime(); //Obtiene el valor depurado de la fecha seleccionada.
            DataTable RSActa = new DataTable();
            DataTable RS = new DataTable();

            try
            {
                try { strtxtFecha = Convert.ToDateTime(dd_fecha_dias_Memorando.SelectedValue + "/" + dd_fecha_mes_Memorando.SelectedValue + "/" + dd_fecha_year_Memorando.SelectedValue); }
                catch
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo depurar la fecha seleccionada.')", true);
                    return;
                }

                //Generar consulta.
                txtSQL = "SELECT Id_Acta FROM AcreditacionActa WHERE NomActa='" + txt_NomActaSeleccionado.Text + "'";

                //Asignar resultados de la consulta a variable DataTable.
                RSActa = consultas.ObtenerDataTable(txtSQL, "text");

                //Si NO hay datos, inserta.
                if (RSActa.Rows.Count == 0)
                {
                    //Insertar registro.
                    txtSQL = "INSERT INTO AcreditacionActa (NumActa, NomActa, FechaActa, Observaciones, CodConvocatoria, Publicado, ActaAcreditada)" +
                     "VALUES('" + txt_noActaSeleccionado.Text.Trim() + "','" + txt_NomActaSeleccionado.Text.Trim() + "','" + strtxtFecha + "','" + txt_NomActaSeleccionado.Text.Trim() + "'," + dd_convocatoriasSeleccionables.SelectedItem.Value + ",0,'" + EsAcreditada + "')";

                    try
                    {
                        //NEW RESULTS:
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                        cmd = new SqlCommand(txtSQL, con);

                        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                        cmd.Dispose();
                    }
                    catch { }

                    //Establecer booleano "según FONADE clásico."
                    repetido = false;

                    //Obtener el código del acta. - Mauricio Arias Olave "28/04/2014": Se agrega un 'Alias' al resultado de la consulta.
                    txtSQL = "SELECT IDENT_CURRENT('AcreditacionActa') AS ValorPK";

                    //Asignar el valor de la consulta a variable DataTable.
                    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    if (RS.Rows.Count > 0)
                    {
                        //Si obtuvo el código del acta.
                        CodActa_ConsultadoCreacion = Convert.ToInt32(RS.Rows[0]["ValorPK"].ToString());

                        //Deshabilitar campos.
                        HabilitarCampos(false);

                        if (!bPublicado && Btn_crearActa.Text != "Nuevo")
                        { panel_publicar.Visible = true; }
                    }

                    //Destruir variables.
                    RSActa = null;
                    RS = null;
                }
                else
                {
                    //No hace nada porque el dato ya existe, está repetido.
                    repetido = true;
                }
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo crear el acta.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Crear acta "Acreditada o NO acreditada" así como us actualización.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_crearActa_Click(object sender, EventArgs e)
        {
            if (Btn_crearActa.Text == "Nuevo")
            {
                #region Pasar por varias validaciones para finalmente poder crear el acta.

                //Inicializar variables.
                Int32 EsAcreditada_Valor = 0;

                try
                {
                    //Obtener el valor "EsAcreditada_Valor" de sesión, para ser usado en la creación del acta.
                    EsAcreditada_Valor = Convert.ToInt32(Session["EsAcreditada_Valor"].ToString());

                    //Sólo puede ser los valores "1 = Acreditada // 0 = NO Acreditada", de lo contraro, no debe hacer nada.
                    if (EsAcreditada_Valor == 1 || EsAcreditada_Valor == 0)
                    {
                        //Entra a la serie de validaciones.
                        #region Validaciones de los campos del formulario.

                        if (txt_noActaSeleccionado.Text.Trim() == "")
                        {
                            txt_noActaSeleccionado.Focus();
                            return;
                        }
                        if (txt_NomActaSeleccionado.Text.Trim() == "")
                        {
                            txt_NomActaSeleccionado.Focus();
                            return;
                        }
                        if (txt_observaciones.Text.Trim() == "")
                        {
                            txt_observaciones.Focus();
                            return;
                        }
                        else
                        {
                            CrearActa(EsAcreditada_Valor);
                        }

                        #endregion
                    }
                }
                catch
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al crear el acta (1).')", true);
                    return;
                }

                #endregion
            }
            if (Btn_crearActa.Text == "Actualizar")
            { ActualizarActa(); }
        }

        #endregion

        #region Actualización.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Actualizar el acta seleccionada (If Request("Accion")="Actualizar" Then).
        /// 29/04/2014: Cuando se llame a este método, se debe usar primero esta linea para desbloquear los
        /// campos.
        /// //Deshabilitar campos.
        /// HabilitarCampos(true); //Añadir botón de actualizar.
        /// QUITAR LA LÍNEA: "return; //Se emplea esta línea para evitar errores.".
        /// </summary>
        private void ActualizarActa()
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String txtSQL = "";
            bool correcto = false;
            String sqlAgregado = ", Publicado = 1 "; //Variable que almacena el valor indicado "sólo si el valor PUBLICADO = 1".
            DataTable RSActa = new DataTable();
            DataTable tabla_detalles = new DataTable();

            try
            {
                //return; //Se emplea esta línea para evitar errores.

                //Generar consulta.
                txtSQL = "SELECT Id_Acta FROM AcreditacionActa WHERE NomActa='" + "NOMBREDELACTASELECCIONADO!" + "' AND Id_Acta <> " + CodActa_ConsultadoCreacion;

                //Asignar resultado de la consulta a variable DataTable.
                RSActa = consultas.ObtenerDataTable(txtSQL, "text");

                #region Detalles.

                tabla_detalles = consultas.ObtenerDataTable("pr_ProyectosAcreditados");

                #endregion

                //Si contiene datos, realiza el siguiente proceso:
                if (RSActa.Rows.Count > 0)
                {
                    #region Actualizar detalles (viabilidad).

                    foreach (DataRow row in tabla_detalles.Rows)
                    {
                        #region Actualizar la viabilidad de los proyectos.

                        string viable = row["viableAcreditador"].ToString();
                        int viable_dato = 0;
                        if (viable == "SI") { viable_dato = 1; }

                        //Actualizar la viabilidad de los proyectos.
                        txtSQL = " UPDATE AcreditacionActaProyecto SET Acreditado = " + viable_dato +
                                 " WHERE CodActa = " + CodActa_ConsultadoCreacion +
                                 " AND CodProyecto = " + row["id_proyecto"].ToString();

                        //Asignar SqlCommand para su ejecución.
                        cmd = new SqlCommand(txtSQL, conn);

                        //Ejecutar SQL.
                        correcto = EjecutarSQL(conn, cmd);

                        if (correcto == false)
                        { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización del acta #(" + row["nomproyecto"].ToString() + ").')", true); /*return; //No debería haber errores.*/ }

                        #endregion

                        //Si el valor "Publicar" es igual a 1. "REVISAR DÓNDE SALE ESTE DATO!"
                        if (Publicar.Checked)
                        {
                            ///Si el proyecto es viable pasa al siguiente estado "Asignación de Recursos" sino
                            ///vuelve al estado de "Aprobación Técnica" para que pueda participar en una próxima convocatoria.
                            if (Boolean.Parse(row["Viable"].ToString()))
                            {
                                #region Actualización de viabilidad.

                                //Actualización:
                                txtSQL = " UPDATE Proyecto SET CodEstado = " + Constantes.CONST_Acreditado +
                                         " WHERE Id_Proyecto = " + row["id_proyecto"].ToString();

                                //Asignar SqlCommand para su ejecución.
                                cmd = new SqlCommand(txtSQL, conn);

                                //Ejecutar SQL.
                                correcto = EjecutarSQL(conn, cmd);

                                if (correcto == false)
                                { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización de viabilidad.).')", true);/*return; //No debería haber errores.*/ }

                                #endregion
                            }
                            else
                            {
                                #region Pasar el proyecto a estado de registro y asesoria manteniendo los emprendedores y asesores.

                                txtSQL = " UPDATE Proyecto SET CodEstado = " + Constantes.CONST_Registro_y_Asesoria +
                                         " WHERE Id_Proyecto = " + row["id_proyecto"].ToString();

                                //Asignar SqlCommand para su ejecución.
                                cmd = new SqlCommand(txtSQL, conn);

                                //Ejecutar SQL.
                                correcto = EjecutarSQL(conn, cmd);

                                if (correcto == false)
                                { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización de estado '(1)'.).')", true); /*return; //No debería haber errores.*/ }

                                #endregion
                            }

                            #region Desvincular el acreditador del proyecto.

                            txtSQL = " UPDATE ProyectoContacto SET Inactivo = 1, " +
                                     " FechaFin = GETDATE() " +
                                     " WHERE CodRol = 9 AND Inactivo = 0 AND CodProyecto = " + row["id_proyecto"].ToString();

                            //Asignar SqlCommand para su ejecución.
                            cmd = new SqlCommand(txtSQL, conn);

                            //Ejecutar SQL.
                            correcto = EjecutarSQL(conn, cmd);

                            if (correcto == false)
                            { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al desvincular el acreditador del priyecto.).')", true); /*return; //No debería haber errores.*/ }

                            #endregion
                        }
                    }

                    #endregion

                    #region Actualización siguiente.

                    //Actualización:
                    txtSQL = " UPDATE AcreditacionActa SET " +
                             " NumActa = " + Constantes.CONST_Acreditado + ", " +
                             " NomActa = " + txt_NomActaSeleccionado.Text + ", " +
                             " FechaActa = '" + DateTime.Today + "', " +
                             " Observaciones = '" + txt_observaciones.Text + "'";

                    //Si PUBLICADO = 1 se le añade la condición WHERE a la consulta.
                    if (Publicar.Checked)
                    {
                        txtSQL = txtSQL + " , Publicado = 1 ";

                        #region Generar acta en Excel!!!!

                        //Dim strNombreArchivo
                        //strNombreArchivo = "ArchivoActaParcial_[codActa].xls"
                        //strNombreArchivo = replace(strNombreArchivo,"[codActa]",codActa)
                        //CrearExcelActa strNombreArchivo,codActa,CodConvocatoria 

                        #endregion
                    }

                    txtSQL = txtSQL + sqlAgregado + " WHERE Id_Acta = " + CodActa_ConsultadoCreacion;

                    //Asignar SqlCommand para su ejecución.
                    cmd = new SqlCommand(txtSQL, conn);

                    //Ejecutar SQL.
                    correcto = EjecutarSQL(conn, cmd);

                    if (correcto == false)
                    { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización de viabilidad.).')", true); /*return; //No debería haber errores.*/ }

                    #endregion

                    //Termina el flujo asignando la variable "repetido" en TRUE.
                    repetido = false;
                }
                else
                    repetido = true;
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo actualizar el acta.')", true);
                return;
            }
        }

        #endregion

        #region Eliminación.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Eliminar el acta seleccionada.
        /// NOTA: El valor "CodActa_Seleccionado" DEBE estar cargado.
        /// </summary>
        private void EliminarActaSeleccionada(Int32 CodActa_Seleccionado)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String txtSQL = "";
            bool correcto = false;

            try
            {
                //Generar consulta.
                txtSQL = "SELECT Publicado FROM AcreditacionActa WHERE Id_Acta = " + CodActa_Seleccionado;

                //Asignar resultado de la consulta a variable DataTable.
                var valorPublicado = consultas.ObtenerDataTable(txtSQL, "text");

                //Si contiene datos, realiza el siguiente proceso:
                if (valorPublicado.Rows.Count > 0)
                {
                    #region Borrar todos los proyectos del acta.

                    //Consulta de eliminación.
                    txtSQL = "DELETE FROM AcreditacionActaproyecto WHERE CodActa = " + CodActa_Seleccionado;

                    //Asignar SqlCommand para su ejecución.
                    cmd = new SqlCommand(txtSQL, conn);

                    //Ejecutar SQL.
                    correcto = EjecutarSQL(conn, cmd);

                    if (correcto == false)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación (1).')", true);
                        return;
                    }

                    #endregion

                    #region Borrar el acta.

                    //Consulta de eliminación.
                    txtSQL = "DELETE FROM AcreditacionActa WHERE Id_Acta = " + CodActa_Seleccionado;

                    //Asignar SqlCommand para su ejecución.
                    cmd = new SqlCommand(txtSQL, conn);

                    //Ejecutar SQL.
                    correcto = EjecutarSQL(conn, cmd);

                    if (correcto == false)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación (2).')", true);
                        return;
                    }

                    #endregion
                }
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo eliminar el acta.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Eliminar el proyecto (If Request("Accion") = "BorrarProyecto" Then).
        /// </summary>
        private void BorrarProyecto(String id_proyecto)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String txtSQL;

            try
            {
                txtSQL = "DELETE FROM AcreditacionActaproyecto WHERE CodActa = " + CodActa_Seleccionado + " AND CodProyecto = " + id_proyecto;

                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    cmd = new SqlCommand(txtSQL, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                    //Recargar la grilla.
                    CargarGrillaDetallesActaSeleccionado(CodActa_Seleccionado);
                }
                catch
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo eliminar el proyecto seleccionado.')", true);
                    return;
                }
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo eliminar el proyecto seleccionado.')", true);
                return;
            }
        }

        #endregion

        #region Métodos de la grilla principal "gv_resultadosActas".

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Establecer valores "SI / NO" de acuerdo a lo que cargue la consulta, en su columna "Acta Acreditada"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_resultadosActas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Inicializar variables e instancias de controles.
                Label lbl = e.Row.FindControl("lbl_siNoActa") as Label;
                var labelActividadPo = e.Row.FindControl("lblactividaPOI") as Label;
                var lnk = e.Row.FindControl("lnkeliminar") as LinkButton;
                var imgEditar = e.Row.FindControl("lnkeliminar") as Image;

                try
                {
                    //Cambiar el valor a "SI / NO" según comportamiento de FONADE Clásico.
                    if (lbl != null)
                    {
                        if (lbl.Text == "True")
                            lbl.Text = "SI";
                        else
                            lbl.Text = "NO";
                    }

                    //Habilitar link de eliminación de acta NO publicada.
                    if (lnk != null && labelActividadPo != null)
                    {
                        if (labelActividadPo.Text == "False")
                        {
                            lnk.Visible = true;
                            imgEditar.Visible = true;
                        }
                        else
                        {
                            lnk.Visible = false;
                            imgEditar.Visible = false;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Sortear la grilla por "No Acta", "Nombre" o "Convocatoria".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_resultadosActas_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gv_resultadosActas.DataSource = Session["dtEmpresas"];
                gv_resultadosActas.DataBind();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Seleccionar el acta y habilitar la visualización de los detalles del 
        /// acta seleccionado.
        /// 29/04/2014: Añadidos los valores "CodActa_Seleccionado" y "Publicado_Valor".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_resultadosActas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "mostrar")
            {
                #region Código que separa el código del acta y su valor "publicado" para añadir los valores a variables globales públicas.

                //Separar los valores.
                var valores_command = new string[] { };
                valores_command = e.CommandArgument.ToString().Split(';');

                CodActa_Seleccionado = Convert.ToInt32(valores_command[0].ToString());
                if (valores_command[1].ToString() == "True") { Publicar.Checked = true; } else { Publicar.Checked = false; }

                #endregion

                //Mostrar la información en los campos correspondientes y evaluar.
                pnlPrincipal.Visible = false;
                pnl_detalles.Visible = true;
                EvaluarEnunciado();
                Session["CodActa_Seleccionado"] = CodActa_Seleccionado;
                CargarDetallesCamposActaSeleccionado(CodActa_Seleccionado);
                CargarGrillaDetallesActaSeleccionado(CodActa_Seleccionado);
                //Ocultar botón de creación de actas.
                Btn_crearActa.Visible = true;
                Btn_crearActa.Text = "Actualizar";
                //Mostrar botón de impresión.
            }
            if (e.CommandName == "eliminar")
            {
                EliminarActaSeleccionada(CodActa_Seleccionado);
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 05/05/2014
        /// 05/05/2014.
        /// Paginación de la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_resultadosActas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_resultadosActas.PageIndex = e.NewPageIndex;
            gv_resultadosActas.DataSource = Session["dtEmpresas"];
            gv_resultadosActas.DataBind();
        }

        #endregion

        #region Métodos de la grilla principal "gv_DetallesActa".

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Establecer funcionalidad RowCommand para las siguientes funciones.
        /// - Invocar ventana emergente con información del acreditador seleccionado".
        /// - Enlazar detalle seleccionado a otra página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_DetallesActa_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "mostrar_acreditador")
            {
                #region Código que separa el id del acreditado y su nombre para añadir los valores a variables de sesión.
                //Separar los valores.
                var valores_command = new string[] { };
                valores_command = e.CommandArgument.ToString().Split(';');

                //Nueva línea de código para almacenar nombre del producto seleccionado en sesión.
                Session["IdAcreditador_Session"] = valores_command[0];
                Session["NombreAcreditador_Session"] = valores_command[1];
                #endregion

                #region Cargar el Id_Grupo del contacto seleccionado y asignarlo a la variable de sesión "CodRol_ActaAcreditacion".

                String txtSQL = "";
                int valorGrupo = 0;

                //Consultar el CodGrupo del contacto seleccionado.
                txtSQL = "SELECT CodGrupo FROM GrupoContacto WHERE CodContacto = " + valores_command[0].ToString();

                //Asignar resultados de la consulta.
                var t = consultas.ObtenerDataTable(txtSQL, "text");

                //Obtener el CodGrupo
                Session["CodRol_ActaAcreditacion"] = t.Rows[0]["CodGrupo"].ToString();

                #endregion

                #region Invocar ventana emergente con información del acreditador seleccionado.

                Redirect(null, "../MiPerfil/VerPerfilContacto.aspx", "_blank",
                    "menubar=0,scrollbars=1,width=710,height=430,top=100");

                #endregion
            }
            if (e.CommandName == "mostrar_proyecto")
            {
                #region Mostrar la ventana con las detalles del detalle del acta seleccionado.

                #region Código que separa el id del proyecto y su nombre para añadir los valores a variables de sesión.
                //Separar los valores.
                var valores_command_proyecto = new string[] { };
                valores_command_proyecto = e.CommandArgument.ToString().Split(';');

                //Nueva línea de código para almacenar nombre del producto seleccionado en sesión.
                Session["ID_PROYECTOAcreditar"] = valores_command_proyecto[0];//IdProyecto_Session
                //Session["NombreProyecto_Session"] = valores_command_proyecto[1];
                //int a = Convert.ToInt32(Session["CodConvocatoria_Session"].ToString()); // Usado para comprobar si sí carga el código.
                //El código de la convocatoria se está enviando en el método "CargarDetallesCamposActaSeleccionado".

                #endregion

                //Redirigir al usuario a la página "ProyectoAcreditacion.aspx".
                Response.Redirect("ProyectoAcreditacion.aspx");

                #endregion
            }
            if (e.CommandName == "eliminar_detalle")
            {
                //Separar los valores.
                var valores_command = new string[] { };
                valores_command = e.CommandArgument.ToString().Split(';');
                try { int a = Int32.Parse(valores_command[1].ToString()); BorrarProyecto(valores_command[1]); }
                catch { }
            }
        }

        #endregion

        #region Métodos de la grilla principal "gv_imprimir_planesNegocio".

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Establecer valores "SI / NO" de acuerdo a lo que cargue la consulta, en su columna "Viable Acreditación".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_imprimir_planesNegocio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Inicializar variables e instancias de controles.
                Label lbl = e.Row.FindControl("lbl_ViableSiNo") as Label;

                try
                {
                    if (lbl != null)
                    {
                        if (lbl.Text == "True")
                            lbl.Text = "SI";
                        else
                            lbl.Text = "NO";
                    }
                }
                catch { }
            }
        }

        #endregion

        /// <summary>
        /// RowCommand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_DetallesActa_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lnk = e.Row.FindControl("lnkeliminar_detalle") as LinkButton;

                if (lnk != null)
                {
                    if (bPublicado)
                    { lnk.Visible = false; }
                    else
                    { lnk.Visible = true; }
                }
            }
        }

        /// <summary>
        /// Agregar planes de negocio al acta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void img_btn_addPlanes_Click(object sender, ImageClickEventArgs e)
        {
            string a = Session["CodActa_Seleccionado"].ToString();
            string b = Session["CodConvocatoria_Acta"].ToString();
            string c = Session["bActaAcreditada"].ToString();

            Redirect(null, "AdicionarProyectoAcreditacionActa.aspx", "_blank", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }

        /// <summary>
        /// Agregar planes de negocio al acta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnk_btn_addPlanes_Click(object sender, EventArgs e)
        {
            string a = Session["CodActa_Seleccionado"].ToString(); 
            string b = Session["CodConvocatoria_Acta"].ToString();
            string c = Session["bActaAcreditada"].ToString();
            Redirect(null, "AdicionarProyectoAcreditacionActa.aspx", "_blank", "menubar=0,scrollbars=1,width=710,height=290,top=100");
        }

        //lbl_siNoActaDetalle
        //591
    }
}