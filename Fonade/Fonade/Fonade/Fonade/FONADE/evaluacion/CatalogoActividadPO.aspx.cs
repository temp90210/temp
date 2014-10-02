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
using Fonade.Clases;

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoActividadPO : Base_Page
    {
        public String CodProyecto;
        public int txtTab = Constantes.CONST_ProyeccionesVentas;

        Int32 CodActividad;
        /// <summary>
        /// Mauricio Arias Olave. "12/05/2014": Indica que si el valor contiene alguna información, significa que proviene
        /// probablemente de un valor seleccionado en "CambiosPO.aspx", por lo tanto, DEBE mostrar ciertos campos que se
        /// mantienen invisibles.
        /// </summary>
        String Detalles_CambiosPO_PO;
        /// <summary>
        /// Mauricio Arias Olave.
        /// "27/05/2014": Valor creado en "FramePlanOperativoInterventoria.aspx" que dicta que el valor seleccionado
        /// es de "Actividades en Aprobación", por lo tanto consulta las tablas temporales.
        /// </summary>
        String dato_TMP;
        /// <summary>
        /// Nombre del proyecto.
        /// </summary>
        String txtNomProyecto;
        String txtSQL;

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/05/2014.
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Establecer en variable de sesión "txtObjeto" el valor correspondiente de acuerdo a su rol.
            //Si es coordinador o gerente, se crea la sesión "txtObjeto" con el valor "Agendar Tarea";
            if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
            { Session["txtObjeto"] = "Agendar Tarea"; }
            //Si es Interventor, lo hace con otro dato.
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            { Session["txtObjeto"] = "Mis Proyectos para Seguimiento de Interventoría"; }
            //Si es Evaluador...
            if (usuario.CodGrupo == Constantes.CONST_Evaluador)
            { Session["txtObjeto"] = "Mis planes de negocio a evaluar"; }
            #endregion

            //Código del proyecto:
            CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            //Código de la actividad seleccionada.
            CodActividad = Session["CodActividad"] != null && !string.IsNullOrEmpty(Session["CodActividad"].ToString()) ? Convert.ToInt32(Session["CodActividad"].ToString()) : 0;

            #region Consulta para traer el nombre del proyecto
            txtSQL = "select NomProyecto from Proyecto WHERE id_proyecto=" + CodProyecto;
            var rr = consultas.ObtenerDataTable(txtSQL, "text");
            if (rr.Rows.Count > 0) { txtNomProyecto = rr.Rows[0]["NomProyecto"].ToString(); rr = null; }
            #endregion

            try
            {
                if (!IsPostBack)
                {
                    //Asignar evento JavaScript a TextBox.
                    TB_item.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");

                    #region Variables de sesión creadas en "CambiosPO.aspx".
                    //Sesión inicial que indica que la información a procesar proviene de "CambiosPO.aspx".
                    Detalles_CambiosPO_PO = Session["Detalles_CambiosPO_PO"] != null && !string.IsNullOrEmpty(Session["Detalles_CambiosPO_PO"].ToString()) ? Session["Detalles_CambiosPO_PO"].ToString() : "";
                    #endregion

                    //Revisar si la variable contiene datos "debe ser así para volver visibles ciertos campos".
                    if (Detalles_CambiosPO_PO.Trim() != "" && CodActividad != 0)
                    {
                        #region Procesar la información que proviene de "CambiosPO.aspx".

                        //Mostrar campos.
                        lbl_inv_aprobar.Visible = true;
                        dd_inv_aprobar.Visible = true;
                        lbl_inv_obvservaciones.Visible = true;
                        txt_inv_observaciones.Visible = true;

                        //Inhabilitar panel que contiene la tabla dinámica.
                        P_Meses.Enabled = false;

                        //Evaluar la acción a tomar.
                        if (Session["Accion"].ToString().Equals("actualizar") || Session["Accion"].ToString().Equals("Editar"))
                        {
                            B_Acion.Text = "Actualizar";
                            CargarTitulo("Modificar");
                        }

                        //Llenar el panel.
                        llenarpanel();

                        if (CodActividad != 0)
                        {
                            //Buscar los datos.
                            buscarDatos(CodActividad, true);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Procesar la información que proviene de "FramePlanOperativoInterventoria.aspx".

                        //Si ya aquí NO contiene datos, es porque es una NUEVA ACTIVIDAD!
                        if (CodActividad == 0) { lbl_titulo_PO.Text = "NUEVA ACTIVIDAD"; }

                        //Evaluar la acción a tomar.
                        if (Session["Accion"].ToString().Equals("crear"))
                        {
                            B_Acion.Width = 100;
                            CargarTitulo("Adicionar");

                        }
                        if (Session["Accion"].ToString().Equals("actualizar") || Session["Accion"].ToString().Equals("Editar"))
                        {
                            B_Acion.Text = "Actualizar";
                            lbl_titulo_PO.Text = "Editar";
                        }
                        if (Session["Accion"].ToString().Equals("borrar"))
                        {
                            B_Acion.Text = "Borrar";
                            CargarTitulo("Eliminar");
                        }

                        //Llenar el panel.
                        llenarpanel();

                        if (CodActividad != 0)
                        {
                            //Buscar los datos.
                            buscarDatos(CodActividad, false);
                        }

                        #endregion
                    }
                }
                else
                {
                    //Llenar el panel.
                    llenarpanel();
                }
            }
            catch { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true); }
        }

        /// <summary>
        /// Mauricio Arias Olave "15/04/2014": Se cambia el código fuente para generar 14 meses.
        /// </summary>
        private void llenarpanel()
        {
            TableRow filaTablaMeses = new TableRow();
            T_Meses.Rows.Add(filaTablaMeses);
            TableRow filaTablaFondo = new TableRow();
            T_Meses.Rows.Add(filaTablaFondo);
            TableRow filaTablaAporte = new TableRow();
            T_Meses.Rows.Add(filaTablaAporte);
            TableRow filaTotal = new TableRow();
            T_Meses.Rows.Add(filaTotal);

            #region Nuevas líneas, con el valor obtenido de la prórrgoa se suma a la constante de meses y se genera la tabla.
            int prorroga = 0;
            prorroga = ObtenerProrroga(CodProyecto);
            int prorrogaTotal = prorroga + Constantes.CONST_Meses + 1; //El +1 es paar evitar modificar aún mas el for...
            #endregion

            for (int i = 1; i <= prorrogaTotal; i++) //for (int i = 1; i <= 13; i++) = Son 14 meses según el FONADE clásico.
            {
                TableCell celdaMeses;
                TableCell celdaFondo;
                TableCell celdaAporte;
                TableCell celdaTotal;


                if (i == 1)
                {
                    celdaMeses = new TableCell();
                    celdaFondo = new TableCell();
                    celdaAporte = new TableCell();
                    celdaTotal = new TableCell();


                    Label labelMes = new Label();
                    Label labelFondo = new Label();
                    Label labelAportes = new Label();
                    Label labelTotal = new Label();

                    labelMes.ID = "labelMes";
                    labelFondo.ID = "labelfondo";
                    labelFondo.Text = "Fondo Emprender"; //Cantidad
                    labelAportes.ID = "labelaportes";
                    labelAportes.Text = "Aporte Emprendedor"; //Costo
                    labelTotal.ID = "L_SumaTotales";
                    labelTotal.Text = "Total";


                    celdaMeses.Controls.Add(labelMes);
                    celdaFondo.Controls.Add(labelFondo);
                    celdaAporte.Controls.Add(labelAportes);
                    celdaTotal.Controls.Add(labelTotal);


                    filaTablaMeses.Cells.Add(celdaMeses);
                    filaTablaFondo.Cells.Add(celdaFondo);
                    filaTablaAporte.Cells.Add(celdaAporte);
                    filaTotal.Cells.Add(celdaTotal);
                }


                if (i < prorrogaTotal) //15
                {
                    celdaMeses = new TableCell();
                    celdaFondo = new TableCell();
                    celdaAporte = new TableCell();
                    celdaTotal = new TableCell();


                    celdaMeses.Width = 50;
                    celdaFondo.Width = 50;
                    celdaAporte.Width = 50;
                    celdaTotal.Width = 50;

                    //String variable = "Mes" + i;
                    Label labelMeses = new Label();
                    labelMeses.ID = "Mes" + i;
                    labelMeses.Text = "Mes " + i;
                    labelMeses.Width = 50;
                    TextBox textboxFondo = new TextBox();
                    textboxFondo.ID = "Fondoo" + i;
                    textboxFondo.Width = 50;
                    textboxFondo.TextChanged += new System.EventHandler(TextBox_TextChanged);
                    textboxFondo.AutoPostBack = true;
                    textboxFondo.MaxLength = 10;
                    textboxFondo.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
                    TextBox textboxAporte = new TextBox();
                    textboxAporte.ID = "Aporte" + i;
                    textboxAporte.Width = 50;
                    textboxAporte.TextChanged += new EventHandler(TextBox_TextChanged);
                    textboxAporte.AutoPostBack = true;
                    textboxAporte.MaxLength = 10;
                    textboxAporte.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
                    Label totalMeses = new Label();
                    totalMeses.ID = "TotalMes" + i;
                    totalMeses.Text = "0.0";
                    totalMeses.Width = 50;

                    //P_Meses.Controls.Add(label);
                    celdaMeses.Controls.Add(labelMeses);


                    celdaFondo.Controls.Add(textboxFondo);
                    celdaAporte.Controls.Add(textboxAporte);
                    celdaTotal.Controls.Add(totalMeses);

                    filaTablaMeses.Cells.Add(celdaMeses);
                    filaTablaFondo.Cells.Add(celdaFondo);
                    filaTablaAporte.Cells.Add(celdaAporte);
                    filaTotal.Cells.Add(celdaTotal);
                }
                if (i == prorrogaTotal) //15
                {

                    //if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    //{

                    celdaMeses = new TableCell();
                    celdaFondo = new TableCell();
                    celdaAporte = new TableCell();
                    celdaTotal = new TableCell();

                    Label labelMes = new Label();
                    Label labelFondo = new Label();
                    Label labelAportes = new Label();
                    Label labelTotal = new Label();

                    labelMes.ID = "labelMescosto";
                    labelMes.Text = "Costo Total";
                    labelFondo.ID = "labelfondocosto";
                    labelFondo.Text = "0";
                    labelAportes.ID = "labelaportescosto";
                    labelAportes.Text = "0";
                    labelTotal.ID = "L_SumaTotalescosto";
                    labelTotal.Text = "0";

                    celdaMeses.Controls.Add(labelMes);
                    celdaFondo.Controls.Add(labelFondo);
                    celdaAporte.Controls.Add(labelAportes);
                    celdaTotal.Controls.Add(labelTotal);

                    filaTablaMeses.Cells.Add(celdaMeses);
                    filaTablaFondo.Cells.Add(celdaFondo);
                    filaTablaAporte.Cells.Add(celdaAporte);
                    filaTotal.Cells.Add(celdaTotal);

                    //}
                }
            }
        }

        /// <summary>
        /// Buscar los datos de la actividad seleccionada.
        /// </summary>
        /// <param name="actividad">Actividad a consultar</param>
        /// <param name="VieneDeCambiosPO">TRUE = Cargar la información de "CambiosPO.aspx". // FALSE = Cargar la información de "FramePlanOperativoInterventoria.aspx".</param>
        private void buscarDatos(Int32 actividad, bool VieneDeCambiosPO)
        {
            dato_TMP = Session["dato_TMP"] != null && !string.IsNullOrEmpty(Session["dato_TMP"].ToString()) ? Session["dato_TMP"].ToString() : "";

            if (VieneDeCambiosPO == true)
            {
                #region Cargar la información según la página "CambiosPO.aspx".

                String ChequeoCoordinador;
                String ChequeoGerente;

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                String sqlConsulta;
                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    #region Los campos del formulario se bloquean.

                    TB_item.Enabled = false;
                    TB_Actividad.Enabled = false;
                    TB_metas.Enabled = false;
                    P_Meses.Enabled = false;

                    #endregion

                    sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOInterventorTMP] WHERE [CodProyecto] = " + CodProyecto + "AND [Id_Actividad] = " + actividad;
                }
                else
                {
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOInterventor] WHERE [Id_Actividad] = " + actividad;
                    }
                    else
                    {
                        sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPO] WHERE [Id_Actividad] = " + actividad;
                    }
                }

                SqlCommand cmd = new SqlCommand(sqlConsulta, conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        try
                        {
                            Session["Tarea"] = reader["Tarea"].ToString();
                            ChequeoCoordinador = reader["ChequeoCoordinador"].ToString();
                            ChequeoGerente = reader["ChequeoGerente"].ToString();

                            #region Aprobación de cambios del coordinador.
                            if (!String.IsNullOrEmpty(ChequeoCoordinador))
                            {
                                if (ChequeoCoordinador == "True" || ChequeoCoordinador == "1")
                                { dd_inv_aprobar.SelectedValue = "1"; }
                            }
                            else
                            { dd_inv_aprobar.SelectedValue = "0"; }
                            #endregion
                            #region Aprobación de cambios del gerente.
                            if (!String.IsNullOrEmpty(ChequeoGerente))
                            {
                                if (ChequeoGerente == "True" || ChequeoGerente == "1")
                                { dd_inv_aprobar.SelectedValue = "1"; }
                            }
                            else
                            { dd_inv_aprobar.SelectedValue = "0"; }
                            #endregion

                        }
                        catch (NullReferenceException) { }
                        catch (Exception) { }

                        TB_item.Text = reader["Item"].ToString();
                        TB_Actividad.Text = reader["NomActividad"].ToString();
                        TB_metas.Text = reader["Metas"].ToString();
                    }
                }
                catch (SqlException se)
                {
                    throw se;
                }
                finally
                {
                    conn.Close();
                }

                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOMesInterventorTMP] WHERE [CodActividad] = " + actividad;
                }
                else
                {
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOMesInterventor] WHERE [CodActividad] = " + actividad;
                    }
                    else
                    {
                        sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOMes] WHERE [CodActividad] = " + actividad;
                    }
                }

                cmd = new SqlCommand(sqlConsulta, conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        TextBox controltext;
                        string valor_Obtenido = reader["CodTipoFinanciacion"].ToString();//.Equals("1");

                        if (valor_Obtenido.Equals("1"))
                            controltext = (TextBox)this.FindControl("Fondoo" + reader["Mes"].ToString());
                        else
                            controltext = (TextBox)this.FindControl("Aporte" + reader["Mes"].ToString());

                        if (String.IsNullOrEmpty(reader["Valor"].ToString()))
                            controltext.Text = "0";
                        else
                        {
                            Double valor = Double.Parse(reader["Valor"].ToString());
                            controltext.Text = valor.ToString();
                        }

                        sumar(controltext);
                    }
                }
                //catch (SqlException se)
                catch (Exception se)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error " + se.Message + "')", true);
                }
                finally
                {
                    if (conn != null)
                        conn.Close();
                }


                #endregion
            }
            else
            {
                #region Cargar la información según la página "FramePlanOperativoInterventoria.aspx".

                if (dato_TMP == "")
                {
                    #region Información de la grilla "Actividades".
                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                    String sqlConsulta;
                    DataTable tabla = new DataTable();

                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Consultar la información si es Coordinador o Gerente Interventor.
                        sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOInterventorTMP] WHERE [CodProyecto] = " + CodProyecto + "AND [Id_Actividad] = " + actividad;

                        tabla = null;
                        tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                        if (tabla.Rows.Count > 0)
                        {
                            TB_item.Text = tabla.Rows[0]["Item"].ToString();
                            TB_Actividad.Text = tabla.Rows[0]["NomActividad"].ToString();
                            TB_metas.Text = tabla.Rows[0]["Metas"].ToString();
                        }
                        #endregion
                    }
                    else
                    {
                        if (usuario.CodGrupo == Constantes.CONST_Interventor)
                        {
                            #region Consultar la información si es Interventor.
                            sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOInterventor] WHERE [Id_Actividad] = " + actividad;

                            tabla = null;
                            tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (tabla.Rows.Count > 0)
                            {
                                TB_item.Text = tabla.Rows[0]["Item"].ToString();
                                TB_Actividad.Text = tabla.Rows[0]["NomActividad"].ToString();
                                TB_metas.Text = tabla.Rows[0]["Metas"].ToString();
                            }
                            #endregion
                        }
                        else
                        {
                            #region Consultar "si es otro tipo de usuario - grupo"...
                            sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPO] WHERE [Id_Actividad] = " + actividad;

                            tabla = null;
                            tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (tabla.Rows.Count > 0)
                            {
                                TB_item.Text = tabla.Rows[0]["Item"].ToString();
                                TB_Actividad.Text = tabla.Rows[0]["NomActividad"].ToString();
                                TB_metas.Text = tabla.Rows[0]["Metas"].ToString();
                            }
                            #endregion
                        }
                    }
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOMesInterventorTMP] WHERE [CodActividad] = " + actividad;
                    }
                    else
                    {
                        if (usuario.CodGrupo == Constantes.CONST_Interventor)
                        {
                            sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOMesInterventor] WHERE [CodActividad] = " + actividad;
                        }
                        else
                        {
                            sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOMes] WHERE [CodActividad] = " + actividad;
                        }
                    }

                    SqlCommand cmd = new SqlCommand(sqlConsulta, conn);
                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            TextBox controltext;
                            string valor_Obtenido = reader["CodTipoFinanciacion"].ToString();//.Equals("1");

                            if (valor_Obtenido.Equals("1"))
                                controltext = (TextBox)this.FindControl("Fondoo" + reader["Mes"].ToString());
                            else
                                controltext = (TextBox)this.FindControl("Aporte" + reader["Mes"].ToString());

                            if (String.IsNullOrEmpty(reader["Valor"].ToString()))
                                controltext.Text = "0";
                            else
                            {
                                Double valor = Double.Parse(reader["Valor"].ToString());
                                controltext.Text = valor.ToString();
                            }

                            sumar(controltext);
                        }
                    }
                    //catch (SqlException se)
                    catch (Exception se)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error " + se.Message + "')", true);
                    }
                    finally
                    {
                        if (conn != null)
                            conn.Close();
                    }
                    #endregion
                }
                else
                {
                    #region Información de la grilla "Actividades en Aprobación".

                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                    String sqlConsulta;
                    DataTable tabla = new DataTable();

                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Consultar la información si es Coordinador o Gerente Interventor.
                        sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOInterventorTMP] WHERE [CodProyecto] = " + CodProyecto + "AND [Id_Actividad] = " + actividad;

                        tabla = null;
                        tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                        if (tabla.Rows.Count > 0)
                        {
                            TB_item.Text = tabla.Rows[0]["Item"].ToString();
                            TB_Actividad.Text = tabla.Rows[0]["NomActividad"].ToString();
                            TB_metas.Text = tabla.Rows[0]["Metas"].ToString();
                        }
                        #endregion
                    }
                    else
                    {
                        if (usuario.CodGrupo == Constantes.CONST_Interventor)
                        {
                            #region Consultar la información si es Interventor.
                            sqlConsulta = "SELECT * FROM ProyectoActividadPOInterventorTMP WHERE Id_Actividad = " + actividad;

                            tabla = null;
                            tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (tabla.Rows.Count > 0)
                            {
                                TB_item.Text = tabla.Rows[0]["Item"].ToString();
                                TB_Actividad.Text = tabla.Rows[0]["NomActividad"].ToString();
                                TB_metas.Text = tabla.Rows[0]["Metas"].ToString();
                            }
                            #endregion
                        }
                        else
                        {
                            #region Consultar "si es otro tipo de usuario - grupo"...
                            sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPO] WHERE [Id_Actividad] = " + actividad;

                            tabla = null;
                            tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (tabla.Rows.Count > 0)
                            {
                                TB_item.Text = tabla.Rows[0]["Item"].ToString();
                                TB_Actividad.Text = tabla.Rows[0]["NomActividad"].ToString();
                                TB_metas.Text = tabla.Rows[0]["Metas"].ToString();
                            }
                            #endregion
                        }
                    }
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOMesInterventorTMP] WHERE [CodActividad] = " + actividad;
                    }
                    else
                    {
                        if (usuario.CodGrupo == Constantes.CONST_Interventor)
                        {
                            sqlConsulta = "SELECT * FROM ProyectoActividadPOMesInterventorTMP WHERE CodActividad = " + actividad;
                        }
                        else
                        {
                            sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOMes] WHERE [CodActividad] = " + actividad;
                        }
                    }

                    SqlCommand cmd = new SqlCommand(sqlConsulta, conn);
                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            TextBox controltext;
                            string valor_Obtenido = reader["CodTipoFinanciacion"].ToString();//.Equals("1");

                            if (valor_Obtenido.Equals("1"))
                                controltext = (TextBox)this.FindControl("Fondoo" + reader["Mes"].ToString());
                            else
                                controltext = (TextBox)this.FindControl("Aporte" + reader["Mes"].ToString());

                            if (String.IsNullOrEmpty(reader["Valor"].ToString()))
                                controltext.Text = "0";
                            else
                            {
                                Double valor = Double.Parse(reader["Valor"].ToString());
                                controltext.Text = valor.ToString();
                            }

                            sumar(controltext);
                        }
                    }
                    //catch (SqlException se)
                    catch (Exception se)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error " + se.Message + "')", true);
                    }
                    finally
                    {
                        if (conn != null)
                            conn.Close();
                    }

                    #endregion
                }

                #endregion
            }
        }

        protected void B_Acion_Click(object sender, EventArgs e)
        {
            if (B_Acion.Text.Equals("Crear") || B_Acion.Text.Equals("Adicionar")) { alamcenar(1); }
            if (B_Acion.Text.Equals("Actualizar") || B_Acion.Text.Equals("Modificar")) { alamcenar(2); }
        }

        private void alamcenar(int acion)
        {
            //Inicializar variables.            
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String NomActividad = "";
            DataTable RsActividad = new DataTable();
            String Valor = "";
            DataTable Rs = new DataTable();
            DataTable RsRevisa = new DataTable();
            String correcto = "";

            if (CodProyecto != "0" || CodActividad != 0)
            {
                if (acion == 1)
                {
                    #region Guardar la información de plan operativo "guarda en tablas temporales".

                    #region Si es Interventor.
                    //Si es interventor inserta los registros en tablas temporales para la aprobación del coordinador y el gerente.
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        //Asigna la tarea al coordinador.
                        string txtSQL = "select CodCoordinador  from interventor where codcontacto=" + usuario.IdContacto;
                        SqlDataReader reader = ejecutaReader(txtSQL, 1);

                        //Verifica si el interventor tiene un coordinador asignado.
                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                //Variable para capturar el número temporal de la actividad.
                                int ActividadTmp = 1;
                                txtSQL = "select Id_Actividad  from proyectoactividadPOInterventorTMP ORDER BY Id_Actividad DESC";

                                reader = ejecutaReader(txtSQL, 1);

                                if (reader != null)
                                {
                                    if (reader.Read())
                                    {
                                        ActividadTmp = Convert.ToInt32(reader["Id_Actividad"].ToString()) + 1;
                                    }
                                }
                                txtSQL = "Insert into proyectoactividadPOInterventorTMP (id_actividad,CodProyecto,Item,NomActividad,Metas) " +
                                    "values (" + ActividadTmp + "," + CodProyecto + "," + TB_item.Text + ",'" + TB_Actividad.Text + "','" + TB_metas.Text + "')";

                                ejecutaReader(txtSQL, 2);

                                string mes = "0";
                                string valor = "0";
                                string tipo = "0";

                                #region Leer los valores de los TextBox para insertarlos en la tabla temporal.
                                foreach (Control miControl in T_Meses.Controls)
                                {
                                    var tablerow = miControl.Controls;

                                    foreach (Control micontrolrows in tablerow)
                                    {
                                        var hijos = micontrolrows.Controls;

                                        foreach (Control chijos in hijos)
                                        {
                                            if (chijos.GetType() == typeof(TextBox))
                                            {
                                                var text = chijos as TextBox;

                                                if (text.ID.StartsWith("Fondoo")) //Sueldo
                                                {
                                                    tipo = "1";
                                                }
                                                else
                                                {
                                                    if (text.ID.StartsWith("Aporte")) //Sueldo
                                                    {
                                                        tipo = "2";
                                                    }
                                                }
                                                if (string.IsNullOrEmpty(text.Text))
                                                    valor = "0";
                                                else
                                                    valor = text.Text;

                                                int limit = 0;
                                                if (text.ID.Length == 7)
                                                    limit = 1;
                                                else
                                                    limit = 2;

                                                mes = text.ID.Substring(6, limit);

                                                //Insertar los costos por mes y tipo de financiacion.
                                                txtSQL = "INSERT INTO ProyectoactividadPOMesInterventorTMP(CodActividad,Mes,CodTipoFinanciacion,Valor) " +
                                                "VALUES(" + ActividadTmp + "," + mes + "," + tipo + "," + valor + ")";

                                                ejecutaReader(txtSQL, 2);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.');window.opener.location.reload();window.close();", true);
                                return;
                            }
                        }

                        //Destruir variables.
                        CodProyecto = "0";
                        //Session["CodProyecto"] = null;
                        CodActividad = 0;
                        Session["CodActividad"] = null;
                        Session["NomActividad"] = null;
                        //RedirectPage(false, string.Empty, "cerrar");
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                    }
                    #endregion

                    #region Si es Gerente Interventor.

                    if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        if (dd_inv_aprobar.SelectedValue == "1")//Si
                        {
                            #region Aprobado.

                            #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL.

                            txtSQL = " select * from proyectoactividadPOInterventorTMP " +
                                     " where CodProyecto = " + CodProyecto +
                                     " and Id_Actividad = " + CodActividad;

                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                            #endregion

                            #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA.

                            txtSQL = " Insert into proyectoactividadPOInterventor (CodProyecto, Item, NomActividad, Metas) " +
                                     " values (" + CodProyecto + ", " + RsActividad.Rows[0]["Item"].ToString() + ", '" + RsActividad.Rows[0]["NomActividad"].ToString() + "', '" + RsActividad.Rows[0]["Metas"].ToString() + "')";

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL.

                            txtSQL = " DELETE proyectoactividadPOInterventorTMP " +
                                     " where CodProyecto = " + CodProyecto +
                                     " and Id_Actividad = " + CodActividad;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar registros de la tabla temporal: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region TRAE EL CODIGO DE ACTIVIDAD PARA ADICIONARLO A LA TABLA DEFINITIVA POR MES.

                            txtSQL = " select Id_Actividad from proyectoactividadPOInterventor ORDER BY Id_Actividad DESC ";
                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");
                            Valor = RsActividad.Rows[0]["Id_Actividad"].ToString();

                            #endregion

                            #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL POR MESES.

                            txtSQL = " select distinct * from ProyectoActividadPOMesInterventorTMP " +
                                     " where CodActividad = " + CodActividad;
                            Rs = consultas.ObtenerDataTable(txtSQL, "text");

                            #endregion

                            #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.

                            foreach (DataRow row_Rs in Rs.Rows)
                            {
                                //Modificacion Jul 16 de 2007 - Vladimir Delgado
                                txtSQL = " SELECT * FROM ProyectoActividadPOMesInterventor " +
                                         " where codactividad = " + CodActividad + " and mes = " + row_Rs["Mes"].ToString();

                                RsRevisa = consultas.ObtenerDataTable(txtSQL, "text");

                                if (RsRevisa.Rows.Count > 0)
                                {
                                    #region Actualizar.

                                    txtSQL = " update ProyectoActividadPOMesInterventor set CodTipoFinanciacion = " + row_Rs["CodTipoFinanciacion"].ToString() + ", Valor = " + row_Rs["Valor"].ToString() +
                                             " where codactividad = " + CodActividad + " and mes = " + row_Rs["Mes"].ToString();

                                    #endregion
                                }
                                else
                                {
                                    #region Ingresar.

                                    txtSQL = " Insert into ProyectoActividadPOMesInterventor (CodActividad,Mes,CodTipoFinanciacion,Valor) " +
                                             " values (" + Valor + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["CodTipoFinanciacion"].ToString() + ", " + row_Rs["Valor"].ToString() + ") ";

                                    #endregion
                                }

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción de registros: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                            }

                            #endregion

                            #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL POR MESES.

                            txtSQL = " DELETE ProyectoActividadPOMesInterventorTMP " +
                                     " where CodActividad = " + CodActividad;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar registros de la tabla temporal por mes: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            //Destruir variables.
                            CodProyecto = "0";
                            //Session["CodProyecto"] = null;
                            CodActividad = 0;
                            Session["CodActividad"] = null;
                            Session["NomActividad"] = null;
                            //RedirectPage(false, string.Empty, "cerrar");
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                            #endregion
                        }
                        else
                        {
                            #region No Aprobado.

                            #region Se devuelve al interventor, se le avisa al coordinador.

                            txtSQL = " SELECT EmpresaInterventor.CodContacto " +
                                     " FROM EmpresaInterventor " +
                                     " INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                     " WHERE EmpresaInterventor.Inactivo = 0 " +
                                     " AND EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider +
                                     " AND Empresa.codproyecto = " + CodProyecto;

                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                            #endregion

                            #region Eliminación #1.

                            txtSQL = " DELETE FROM proyectoactividadPOInterventorTMP " +
                                     " where CodProyecto = " + CodProyecto +
                                     " and Id_Actividad = " + CodActividad;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #1: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region Eliminación #2.

                            txtSQL = " DELETE FROM ProyectoactividadPOMesInterventorTMP " +
                                     " where CodActividad = " + CodActividad;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #2: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region Consultar y asignar resultados a variables "Rs" y "NomActividad".

                            txtSQL = " SELECT NomActividad FROM proyectoactividadPOInterventorTMP " +
                                     " WHERE CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodActividad;

                            Rs = consultas.ObtenerDataTable(txtSQL, "text");

                            if (Rs.Rows.Count > 0) { NomActividad = Rs.Rows[0]["NomActividad"].ToString(); }

                            #endregion

                            #region Generar tarea pendiente.

                            //Agendar tarea.
                            AgendarTarea agenda = new AgendarTarea
                            (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                            "Actividad del Plan Operativo Rechazada por Gerente Interventor",
                            "Revisar actividad del plan operativo " + txtNomProyecto + " - Actividad --> " + NomActividad + "<BR><BR>Observaciones:<BR>" + txt_inv_observaciones.Text.Trim(),
                            CodProyecto,
                            2,
                            "0",
                            true,
                            1,
                            true,
                            false,
                            usuario.IdContacto,
                            "CodProyecto=" + CodProyecto,
                            "",
                            "Catálogo Actividad Plan Operativo");

                            agenda.Agendar();

                            #endregion

                            //Destruir variables.
                            CodProyecto = "0";
                            //Session["CodProyecto"] = null;
                            CodActividad = 0;
                            Session["CodActividad"] = null;
                            Session["NomActividad"] = null;
                            //RedirectPage(false, string.Empty, "cerrar");
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                            #endregion
                        }
                    }

                    #endregion

                    #endregion
                }
                if (acion == 2)
                {
                    #region Editar el plan operativo seleccionado.

                    //Comprobar si el usuario tiene el código grupo de "Coordinador Interventor" ó "Gerente Interventor".
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Ejecutar como Coordinador Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1") //Si
                            {
                                #region Aprobado.

                                #region Actualización.
                                //Actualización.
                                txtSQL = " UPDATE proyectoactividadPOInterventorTMP SET ChequeoCoordinador = 1 " +
                                         " where CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodActividad;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar la actividad aprobada: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                                #endregion

                                #region COMENTADO EN FONADE CLÁSICO "prTareaAsignarGerente".
                                ////Consulta.
                                //txtSQL = " select Id_grupo from Grupo " +
                                //         " where NomGrupo = 'Gerente Interventor' ";

                                ////Asignar resultados de la consulta.
                                //var dt = consultas.ObtenerDataTable(txtSQL, "text");
                                //dt = null;
                                ////COMENTADO "DeclaraVariables.inc  - línea 727.
                                ////prTareaAsignarGerente RsActividad("CodContacto"),Session("CodUsuario"),CodProyecto,txtNomProyecto,CodActividad 
                                #endregion

                                //Finalmente, destruye las variables.
                                correcto = "";
                                CodProyecto = "0";
                                CodActividad = 0;
                                Session["CodActividad"] = null;
                                Session["NomActividad"] = null;
                                Session["CodProyecto"] = null;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                            else
                            {
                                #region No Aprobado.

                                #region Consultar y asignar valores a "RsActividad".
                                //Consulta.
                                txtSQL = " SELECT EmpresaInterventor.CodContacto " +
                                         " FROM EmpresaInterventor " +
                                         " INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                         " WHERE EmpresaInterventor.Inactivo = 0 " +
                                         " AND EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider +
                                         " AND Empresa.CodProyecto = " + CodProyecto;

                                //Asignar resultados de la consulta.
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");
                                #endregion

                                #region Eliminación #1.
                                //Eliminación #1.
                                txtSQL = " DELETE FROM proyectoactividadPOInterventorTMP " +
                                         " where CodProyecto=" + CodProyecto + " and Id_Actividad = " + CodActividad;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en Eliminación #1: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                                #endregion

                                #region Eliminación #2.
                                //Eliminación #2.
                                txtSQL = " DELETE FROM ProyectoactividadPOMesInterventorTMP " +
                                         " where CodActividad=" + CodActividad;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en Eliminación #2: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                                #endregion

                                #region Consultar y asignar valores a "rs".
                                //Generar consulta.
                                txtSQL = " SELECT NomActividad FROM proyectoactividadPOInterventorTMP " +
                                         " WHERE CodProyecto = " + CodProyecto +
                                         " AND Id_Actividad = " + CodActividad;

                                //Asignar resultados de la consulta.
                                var rs = consultas.ObtenerDataTable(txtSQL, "text");
                                #endregion

                                #region Recorrer la consulta anterior para enviar la agenda.
                                for (int i = 0; i < rs.Rows.Count; i++)
                                {
                                    NomActividad = rs.Rows[i]["NomActividad"].ToString();

                                    //Agendar tarea.
                                    AgendarTarea agenda = new AgendarTarea
                                    (usuario.IdContacto,
                                    "Actividad del Plan Operativo Rechazada por Coordinador de Interventoria",
                                    "Revisar actividad del plan operativo " + txtNomProyecto + " - Actividad --> " + NomActividad + "<BR><BR>Observaciones:<BR>" + txt_inv_observaciones.Text.Trim(),
                                    CodProyecto,
                                    2,
                                    "0",
                                    true,
                                    1,
                                    true,
                                    false,
                                    usuario.IdContacto,
                                    "",
                                    "",
                                    "Traslado Planes");

                                    agenda.Agendar();
                                }
                                #endregion

                                //Finalmente, destruye las variables.
                                correcto = "";
                                CodProyecto = "0";
                                CodActividad = 0;
                                Session["CodActividad"] = null;
                                Session["NomActividad"] = null;
                                Session["CodProyecto"] = null;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                        }

                        #endregion

                        #region Ejecutar como Gerente Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1") //Si
                            {
                                #region Aprobado.

                                #region TRAE LOS REGISTROS DE LA TABA TEMPORAL.
                                //TRAE LOS REGISTROS DE LA TABLA TEMPORAL
                                txtSQL = " select * from proyectoactividadPOInterventorTMP " +
                                         " where CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodActividad;

                                //Asignar resultados de la consulta anterior.
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");
                                #endregion

                                #region ACTUALIZA LOS REGISTROS EN LA TABLA DEFINITIVA
                                txtSQL = " Update proyectoactividadPOInterventor set CodProyecto = " + CodProyecto + "," +
                                         " Item = " + RsActividad.Rows[0]["Item"].ToString() + ", " +
                                         " NomActividad = '" + RsActividad.Rows[0]["NomActividad"].ToString() + "', " +
                                         " Metas = '" + RsActividad.Rows[0]["Metas"].ToString() + "'" +
                                         " WHERE CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodActividad;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar en tabla definitiva " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                                #endregion

                                #region BORRAR EL REGISTRO YA ACTUALIZADO DE LA TABLA TEMPORAL
                                txtSQL = " DELETE FROM proyectoactividadPOInterventorTMP " +
                                         " where CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodActividad;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al borrar registro actualizado en tabla temporal: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                                #endregion

                                #region TRAE EL CODIGO DE ACTIVIDAD PARA ADICIONARLO A LA TABLA DEFINITIVA POR MES
                                txtSQL = " select Id_Actividad from proyectoactividadPOInterventor " +
                                         " WHERE CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodActividad;

                                //Asignar resultados de la consulta.
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                //Obtener el valor del campo "Id_Actividad".
                                Valor = RsActividad.Rows[0]["Id_Actividad"].ToString();
                                #endregion

                                #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL POR MESES
                                txtSQL = " select distinct * from ProyectoActividadPOMesInterventorTMP " +
                                         " where CodActividad = " + CodActividad + " and valor is not null ";

                                //Asignar los resultados de la consulta.
                                Rs = consultas.ObtenerDataTable(txtSQL, "text");
                                #endregion

                                #region BORRAR TODOS LOS REGISTROS DE LA TABLA ProyectoActividadPOMesInterventor CORRESPONDIENTES AL CODIGO DE ACTIVIDAD
                                txtSQL = " DELETE ProyectoActividadPOMesInterventor " +
                                         " where CodActividad = " + CodActividad;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar todos los registros de POMesInterventor de la actividad: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                                #endregion

                                #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.
                                //INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.
                                foreach (DataRow row_Rs in Rs.Rows)
                                {
                                    //Modificacion Jul 16 de 2007 - Vladimir Delgado
                                    txtSQL = " SELECT * FROM ProyectoActividadPOMesInterventor " +
                                             " where codactividad = " + CodActividad +
                                             " and mes = " + row_Rs["Mes"].ToString() +
                                             " and CodTipoFinanciacion = " + row_Rs["CodTipoFinanciacion"].ToString();

                                    //Asignar resultados de la consulta anterior.
                                    RsRevisa = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (RsRevisa.Rows.Count > 0)
                                    {
                                        #region Actualizar.
                                        txtSQL = " update ProyectoActividadPOMesInterventor " +
                                                 " set CodTipoFinanciacion = " + row_Rs["CodTipoFinanciacion"].ToString() + "," +
                                                 " Valor = " + row_Rs["Valor"].ToString() + " " +
                                                 " where codactividad = " + CodActividad + " and mes = " + row_Rs["Mes"].ToString();
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Inserta datos.
                                        txtSQL = " Insert into ProyectoActividadPOMesInterventor (CodActividad, Mes, CodTipoFinanciacion, Valor) " +
                                                 " values (" + CodActividad + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["CodTipoFinanciacion"].ToString() + ", " + row_Rs["Valor"].ToString() + ")";
                                        #endregion
                                    }

                                    //Ejecutar consulta.
                                    cmd = new SqlCommand(txtSQL, conn);
                                    correcto = String_EjecutarSQL(conn, cmd);

                                    if (correcto != "")
                                    {
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al insertar en la tabla definitiva por mes: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                        return;
                                    }
                                }
                                #endregion

                                #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL POR MESES
                                txtSQL = " DELETE ProyectoActividadPOMesInterventorTMP " +
                                         " where CodActividad = " + CodActividad;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al elimunar el registro ya insertado en la tabla temporal por mes: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                                #endregion

                                //Finalmente, destruye las variables.
                                correcto = "";
                                CodProyecto = "0";
                                CodActividad = 0;
                                Session["CodActividad"] = null;
                                Session["NomActividad"] = null;
                                Session["CodProyecto"] = null;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                        }

                        #endregion
                    }
                    if (usuario.CodGrupo == Constantes.CONST_Interventor) //Si el usuario tiene el código grupo "Interventor".
                    {
                        #region Ejecutar como Interventor.
                        string txtSQL = "SELECT CodCoordinador FROM interventor WHERE codcontacto=" + usuario.IdContacto;
                        SqlDataReader reader = ejecutaReader(txtSQL, 1);

                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                txtSQL = "INSERT INTO proyectoactividadPOInterventorTMP (id_actividad,CodProyecto,Item,NomActividad,Metas,Tarea) " +
                                "values (" + CodActividad + "," + CodProyecto + "," + TB_item.Text + ",'" + TB_Actividad.Text + "','" + TB_metas.Text + "','Modificar')";

                                ejecutaReader(txtSQL, 2);

                                //Faltaba el "FROM"...
                                txtSQL = "Delete FROM ProyectoactividadPOMesInterventorTMP Where Codactividad =" + CodActividad + " ";

                                ejecutaReader(txtSQL, 2);

                                string mes = "0";
                                string valor = "0";
                                string tipo = "0";

                                foreach (Control miControl in T_Meses.Controls)
                                {
                                    var tablerow = miControl.Controls;

                                    foreach (Control micontrolrows in tablerow)
                                    {
                                        var hijos = micontrolrows.Controls;

                                        foreach (Control chijos in hijos)
                                        {
                                            if (chijos.GetType() == typeof(TextBox))
                                            {
                                                var text = chijos as TextBox;

                                                if (text.ID.StartsWith("Fondoo")) //Sueldo
                                                {
                                                    tipo = "1";
                                                }
                                                else
                                                {
                                                    if (text.ID.StartsWith("Aporte")) //Sueldo
                                                    {
                                                        tipo = "2";
                                                    }
                                                }
                                                if (string.IsNullOrEmpty(text.Text))
                                                    valor = "0";
                                                else
                                                    valor = text.Text;

                                                int limit = 0;
                                                if (text.ID.Length == 7)
                                                    limit = 1;
                                                else
                                                    limit = 2;

                                                mes = text.ID.Substring(6, limit);

                                                txtSQL = "INSERT INTO ProyectoactividadPOMesInterventorTMP(CodActividad,Mes,CodTipoFinanciacion,Valor) VALUES(" + CodActividad + "," + mes + "," + tipo + "," + valor + ")";

                                                ejecutaReader(txtSQL, 2);
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.');window.opener.location.reload();window.close();", true);
                                return;
                            }
                        }

                        //Destruir variables.
                        CodProyecto = "0";
                        //Session["CodProyecto"] = null;
                        CodActividad = 0;
                        Session["CodActividad"] = null;
                        Session["NomActividad"] = null;
                        //RedirectPage(false, string.Empty, "cerrar");
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                        #endregion
                    }

                    #endregion
                }
                if (acion == 3) /*Eliminar*/
                {
                    #region Si es Gerente Interventor.
                    if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        //Si está aprobado...
                        if (dd_inv_aprobar.SelectedValue == "1") //Si
                        {
                            #region Aprobado.

                            #region BORRA LOS REGISTROS EN LA TABLA DEFINITIVA.
                            //BORRA LOS REGISTROS EN LA TABLA DEFINITIVA
                            txtSQL = " DELETE proyectoactividadPOInterventor " +
                                     " where CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodActividad;

                            //Ejecutar consulta.
                            String_EjecutarSQL(conn, cmd);
                            #endregion

                            #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL.
                            //BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL
                            txtSQL = " DELETE proyectoactividadPOInterventorTMP " +
                                     " where CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodActividad;

                            //Ejecutar consulta.
                            String_EjecutarSQL(conn, cmd);
                            #endregion

                            #region BORRA LOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.
                            //BORRA LOS REGISTROS EN LA TABLA DEFINITIVA POR MESES
                            txtSQL = " DELETE ProyectoActividadPOMesInterventor " +
                                     " where CodActividad = " + CodActividad;

                            //Ejecutar consulta.
                            String_EjecutarSQL(conn, cmd);
                            #endregion

                            #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL POR MESES.
                            //BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL POR MESES
                            txtSQL = " DELETE ProyectoActividadPOMesInterventorTMP " +
                                     " where CodActividad = " + CodActividad;

                            //Ejecutar consulta.
                            String_EjecutarSQL(conn, cmd);
                            #endregion

                            //Destruir variables.
                            CodProyecto = "0";
                            //Session["CodProyecto"] = null;
                            CodActividad = 0;
                            Session["CodActividad"] = null;
                            Session["NomActividad"] = null;

                            //Actualizar la fecha de modificación del tab.
                            prActualizarTab(txtTab.ToString(), CodProyecto);

                            //RedirectPage(false, string.Empty, "cerrar");
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                            #endregion
                        }
                    }
                    #endregion
                }
                else //No es un dato válido.
                {
                    //Destruir variables.
                    CodProyecto = "0";
                    //Session["CodProyecto"] = null;
                    CodActividad = 0;
                    Session["CodActividad"] = null;
                    Session["NomActividad"] = null;
                    //RedirectPage(false, string.Empty, "cerrar");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                }
            }
            else
            {
                return;
            }
        }

        private void sumar(TextBox textbox)
        {
            //Inicializar variables.
            String textboxID = textbox.ID;
            TextBox textFondo;
            TextBox textAporte;
            Label controltext;

            //Se movieron las variables del try para la suma.
            Double suma1 = 0;
            Double suma2 = 0; //Según el FONADE clásico, el valor COSTO lo toma como ENTERO al SUMARLO.
            Int32 valor_suma2 = 0;

            var labelfondocosto = this.FindControl("labelfondocosto") as Label;
            var labelaportescosto = this.FindControl("labelaportescosto") as Label;
            var L_SumaTotalescosto = this.FindControl("L_SumaTotalescosto") as Label;

            //Details
            int limit = 0;
            if (textboxID.Length == 7)
                limit = 1;
            else
                limit = 2;

            //String objeto = "TotalMes" + (textboxID[textboxID.Length - 1]);
            String objeto = "TotalMes" + textboxID.Substring(6, limit);


            controltext = (Label)this.FindControl(objeto);
            textFondo = (TextBox)this.FindControl("Fondoo" + textboxID.Substring(6, limit)); //Sueldo
            textAporte = (TextBox)this.FindControl("Aporte" + textboxID.Substring(6, limit)); //Prestaciones

            try
            {
                if (String.IsNullOrEmpty(textFondo.Text))
                {
                    suma1 = 0;
                    textFondo.Text = suma1.ToString();
                }
                else
                {
                    suma1 = Double.Parse(textFondo.Text);
                    textFondo.Text = suma1.ToString();
                }

                if (String.IsNullOrEmpty(textAporte.Text))
                {
                    suma2 = 0;
                    textAporte.Text = suma2.ToString();
                }
                else
                {
                    suma2 = Double.Parse(textAporte.Text);
                    //valor_suma2 = Convert.ToInt32(suma2);
                    textAporte.Text = suma2.ToString();
                }

                //Con formato
                //controltext.Text = "$" + (suma1 + suma2).ToString("0,0.00", CultureInfo.InvariantCulture);
                controltext.Text = "" + (suma1 + suma2);

                labelfondocosto.Text = "0";
                labelaportescosto.Text = "0";


                foreach (Control miControl in T_Meses.Controls)
                {
                    var tablerow = miControl.Controls;

                    foreach (Control micontrolrows in tablerow)
                    {
                        var hijos = micontrolrows.Controls;

                        foreach (Control chijos in hijos)
                        {
                            if (chijos.GetType() == typeof(TextBox))
                            {
                                var text = chijos as TextBox;

                                if (text.ID.StartsWith("Fondoo")) //Sueldo
                                {
                                    if (labelfondocosto != null)
                                    {
                                        if (string.IsNullOrEmpty(text.Text.Trim()))
                                        {
                                            text.Text = "0";
                                        }
                                        labelfondocosto.Text = (Convert.ToDouble(labelfondocosto.Text) + Convert.ToDouble(text.Text)).ToString();
                                    }
                                    //if (L_SumaTotalescosto != null)
                                    //    L_SumaTotalescosto.Text = (Convert.ToDouble(labelfondocosto.Text)).ToString();
                                }
                                else
                                {
                                    if (text.ID.StartsWith("Aporte")) //Sueldo
                                    {
                                        if (labelaportescosto != null)
                                        {
                                            if (string.IsNullOrEmpty(text.Text.Trim()))
                                            {
                                                text.Text = "0";
                                            }
                                            labelaportescosto.Text = (Convert.ToDouble(labelaportescosto.Text) + Convert.ToDouble(text.Text)).ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (L_SumaTotalescosto != null)
                {
                    L_SumaTotalescosto.Text = (Convert.ToDouble(labelaportescosto.Text)).ToString() + (Convert.ToDouble(labelfondocosto.Text)).ToString();
                }
            }
            catch (FormatException) { }
            catch (NullReferenceException) { }
        }

        protected void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            sumar(textbox);
        }

        protected void B_Cancelar_Click(object sender, EventArgs e)
        {
            //Destruir variables.
            CodProyecto = "0";
            //Session["CodProyecto"] = null;
            CodActividad = 0;
            Session["CodActividad"] = null;
            Session["NomActividad"] = null;
            //RedirectPage(false, string.Empty, "cerrar");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
        }

        #region Métodos activados por la selección de un plan operativo en "cambiosPO.aspx".

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/05/2014.
        /// Cargar el título, dependiendo de la acción a realizar.
        /// </summary>
        /// <param name="accion">La acción DEBE SER "Adicionar", "Modificar" o "Eliminar".</param>
        private void CargarTitulo(String accion)
        {
            try
            {
                if (accion == "Adicionar")
                {
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        lbl_titulo_PO.Text = "ADICIONAR ACTIVIDAD";
                    }
                }
                if (accion == "Modificar")
                {
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        lbl_titulo_PO.Text = "MODIFICAR ACTIVIDAD";
                    }
                }
                if (accion == "Eliminar")
                {
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        lbl_titulo_PO.Text = "BORRAR ACTIVIDAD";
                    }
                }
            }
            catch { lbl_titulo_PO.Text = "ADICIONAR ACTIVIDAD"; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/05/2014.
        /// Eliminar (FONADE clásico como tal no muestra la ejecución de sentencia DELETE)... revisar.
        /// </summary>
        private void Eliminar()
        {
            //Inicializar variables.
            String sqlConsulta = "";

            try
            {
                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    sqlConsulta = " SELECT DISTINCT CodTipoFinanciacion, Mes, Valor " +
                                  " FROM proyectoactividadPOmesInterventor WHERE CodActividad = " + CodActividad +
                                  " ORDER BY codtipofinanciacion, mes ";
                }
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    sqlConsulta = " SELECT DISTINCT CodTipoFinanciacion, Mes, Valor " +
                                  " FROM proyectoactividadPOmesInterventor WHERE CodActividad = " + CodActividad +
                                  " ORDER BY codtipofinanciacion, mes";
                }

                //Si ha pasado por la variable y tiene consulta.
                if (sqlConsulta != "")
                {
                    //Cargar la información de los meses...
                }
            }
            catch { }
        }

        #endregion
    }
}