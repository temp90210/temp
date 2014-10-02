﻿using Datos;
using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fonade.Negocio;
using System.Globalization;
using Fonade.Clases;


namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoInterventorTMP : Base_Page
    {
        #region Variables globales internas.
        public String CodProyecto;
        public int txtTab = Constantes.CONST_ProyeccionesVentas;

        /// <summary>
        /// Código de la nómina seleccionada.
        /// </summary>
        public Int32 CodCargo;

        /// <summary>
        /// Usado para determinar si hace la búsqueda en la tabla TMP o normal.
        /// Ej: si este valor es != null, hace la consulta en "InterventorNominaMesTMP", de lo contrario
        /// consultará en "InterventorNominaMes".
        /// </summary>
        public string ValorTMP;

        /// <summary>
        /// Variable que DEBE pasarse por sesión para revisar si el valor está o no aprobado.
        /// Por defecto, en el Page_Load, se dejará 0 si NO ENCUENTRA NADA.
        /// </summary>
        public int s_valorAprobado;

        /// <summary>
        /// Valor que es recibido por sesión, proveniente de "FrameNominaInter.aspx" y contiene los valores "Cargo" ó "Insumo".
        /// </summary>
        public string v_Tipo;

        /// <summary>
        /// Mauricio Arias Olave. "12/05/2014": Indica que si el valor contiene alguna información, significa que proviene
        /// probablemente de un valor seleccionado en "CambiosPO.aspx", por lo tanto, DEBE mostrar ciertos campos que se
        /// mantienen invisibles.
        /// </summary>
        String Detalles_CambiosPO_NO;

        /// <summary>
        /// Valor que indica cuál es la página a ejecutar, ya que, se había acoplado todas las páginas de Plan Operativo, 
        /// Nómina, Producción y Ventas en una sola, este método diferenciaba los diferentes tipos de métodos dependiendo
        /// de la página a aplicarse.
        /// </summary>
        String pagina;

        /// <summary>
        /// Nombre del proyecto.
        /// </summary>
        String txtNomProyecto;

        /// <summary>
        /// Variable para consultas SQL.
        /// </summary>
        String txtSQL;
        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Código del proyecto:
            CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            //Código de la nómina seleccionada:
            CodCargo = Session["CodNomina"] != null && !string.IsNullOrEmpty(Session["CodNomina"].ToString()) ? Convert.ToInt32(Session["CodNomina"].ToString()) : 0;

            #region Establecer en variable de sesión "txtObjeto" el valor correspondiente de acuerdo a su rol.
            //Si es coordinador o gerente, se crea la sesión "txtObjeto" con el valor "Agendar Tarea";
            if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
            { Session["txtObjeto"] = "Agendar Tarea"; }
            //Si es Interventor, lo hace con otro dato.
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            { Session["txtObjeto"] = "Mis Proyectos para Seguimiento de Interventoría"; }
            #endregion

            #region Consulta para traer el nombre del proyecto
            txtSQL = "select NomProyecto from Proyecto WHERE id_proyecto=" + CodProyecto;
            var rr = consultas.ObtenerDataTable(txtSQL, "text");
            if (rr.Rows.Count > 0) { txtNomProyecto = rr.Rows[0]["NomProyecto"].ToString(); rr = null; }
            #endregion

            try
            {
                if (!IsPostBack)
                {
                    ////Asignar evento JavaScript a TextBox.
                    //TB_Item.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");

                    #region Variables de sesión creadas en "CambiosPO.aspx".
                    //Sesión inicial que indica que la información a procesar proviene de "CambiosPO.aspx".
                    Detalles_CambiosPO_NO = Session["Detalles_CambiosPO_NO"] != null && !string.IsNullOrEmpty(Session["Detalles_CambiosPO_NO"].ToString()) ? Session["Detalles_CambiosPO_NO"].ToString() : "";
                    #endregion

                    //Revisar si la variable contiene datos "debe ser así para volver visibles ciertos campos".
                    if (Detalles_CambiosPO_NO.Trim() != "")
                    {
                        #region Procesar la información que proviene de "CambiosPO.aspx".
                        lbl_inv_aprobar.Visible = true;
                        dd_inv_aprobar.Visible = true;
                        lbl_inv_obvservaciones.Visible = true;
                        txt_inv_observaciones.Visible = true;

                        //Los otros campos se ocultan:
                        L_Item.Visible = false;
                        TB_Item.Visible = false;

                        //Inhabilitar panel que contiene la tabla dinámica.
                        P_Meses.Enabled = false;

                        //Evaluar la acción a tomar.
                        if (Session["Accion"].ToString().Equals("actualizar") || Session["Accion"].ToString().Equals("Editar"))
                        {
                            B_Acion.Text = "Actualizar";
                            lbl_enunciado.Text = "MODIFICAR";
                            L_Item.Visible = false;
                            TB_Item.Visible = false;
                            TB_Item.Text = "Default";
                        }

                        //Llenar el panel.
                        llenarpanel();

                        //Buscar los datos.
                        BuscarDatos_Nomina(CodCargo, true);
                        #endregion
                    }
                    else
                    {
                        #region Procesar la información que proviene de "FrameNominaInter.aspx".
                        //Valor aprobado de la nómina seleccionada. = UNKNOWN.
                        s_valorAprobado = Session["s_valorAprobado"] != null && !string.IsNullOrEmpty(Session["s_valorAprobado"].ToString()) ? Convert.ToInt32(Session["s_valorAprobado"].ToString()) : 0;
                        //Variable desconocida, se supone que la había puesto en "FrameNominaInter.aspx".
                        //ValorTMP = Session["ValorTMP"] != null && !string.IsNullOrEmpty(Session["ValorTMP"].ToString()) ? Session["ValorTMP"].ToString() : "0";
                        //Tipo "Cargo" o "Insumo".
                        v_Tipo = Session["v_Tipo"] != null && !string.IsNullOrEmpty(Session["v_Tipo"].ToString()) ? Session["v_Tipo"].ToString() : "";
                        pagina = Session["pagina"] != null && !string.IsNullOrEmpty(Session["pagina"].ToString()) ? Session["pagina"].ToString() : "";

                        #region NO BORRAR.
                        //if (pagina == "")
                        //{
                        //    //Destruir variables.
                        //    CodProyecto = "0";
                        //    CodNomina = 0;
                        //    ValorTMP = "";
                        //    s_valorAprobado = 0;
                        //    v_Tipo = "";
                        //    Session["CodNomina"] = null;
                        //    Session["s_valorAprobado"] = null;
                        //    Session["v_Tipo"] = null;
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                        //} 
                        #endregion

                        //Evaluar la acción a tomar.
                        if (Session["Accion"].ToString().Equals("crear"))
                        {
                            B_Acion.Text = "Crear";
                            lbl_enunciado.Text = "ADICIONAR";

                            //Limpiar campos "si estaban llenos"...
                            TB_Item.Text = "";
                            txt_inv_observaciones.Text = "";

                        }
                        if (Session["Accion"].ToString().Equals("actualizar") || Session["Accion"].ToString().Equals("Editar"))
                        {
                            B_Acion.Text = "Actualizar";
                            lbl_enunciado.Text = "EDITAR";
                            L_Item.Visible = false;
                            TB_Item.Visible = false;
                            TB_Item.Text = "Default";
                        }
                        if (Session["Accion"].ToString().Equals("borrar")) { B_Acion.Text = "Borrar"; }
                        if (Session["Accion"].ToString().Equals("Modificar")) { B_Acion.Text = "Modificar"; }
                        if (Session["Accion"].ToString().Equals("consultar"))
                        {
                            B_Acion.Visible = false;
                            lbl_enunciado.Text = "CONSULTAR";
                            L_Item.Visible = false;
                            TB_Item.Visible = false;
                        }

                        //Llenar el panel.
                        llenarpanel();

                        //Para que NO cargue la información seleccionada
                        if (!Session["Accion"].ToString().Equals("crear"))
                        {
                            //Buscar los datos.
                            BuscarDatos_Nomina(CodCargo, false);
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
        /// Generar tabla.
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
                    labelMes.Style.Add(HtmlTextWriterStyle.TextAlign, "left");
                    labelFondo.ID = "labelfondo";
                    labelFondo.Text = "Sueldo";
                    labelFondo.Style.Add(HtmlTextWriterStyle.TextAlign, "left");
                    labelAportes.ID = "labelaportes";
                    labelAportes.Text = "Prestaciones";
                    labelAportes.Style.Add(HtmlTextWriterStyle.TextAlign, "left");
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
                    textboxAporte.TextChanged += new EventHandler(TextBoxAportes_TextChanged);
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
                if (i == prorrogaTotal)//15
                {

                    ///Se comenta porque se deben ver los totales.
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
        /// Dependiendo del valor, si es 1, creará registros, si es 2, los actualizará.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_Acion_Click(object sender, EventArgs e)
        {
            if (B_Acion.Text.Equals("Crear")) alamcenar(1);
            if (B_Acion.Text.Equals("Actualizar")) alamcenar(2);
            if (B_Acion.Text.Equals("Modificar")) alamcenar(3);
        }

        #region NO BORRAR!
        ///// <summary>
        ///// Guardar y/o actualizar la información.
        ///// </summary>
        ///// <param name="acion">Si el valor es 1, guardará la información, si es 2, actualizará dicha información.</param>
        //private void alamcenar(int acion)
        //{
        //    //Inicializar variables.            
        //    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //    SqlCommand cmd = new SqlCommand();

        //    if (acion == 1)
        //    {
        //        #region Llamada al procedimiento almacenado "MD_Insertar_Actualizar_InterventorTMP" COMENTADA.

        //        //string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        //        //using (var con = new SqlConnection(conexionStr))
        //        //{
        //        //    using (var com = con.CreateCommand())
        //        //    {
        //        //        com.CommandText = "MD_Insertar_Actualizar_InterventorTMP";
        //        //        com.CommandType = System.Data.CommandType.StoredProcedure;

        //        //        for (int j = 1; j <= 2; j++)
        //        //        {
        //        //            for (int i = 1; i <= 12; i++)
        //        //            {
        //        //                Label controltext;
        //        //                controltext = (Label)this.FindControl("TotalMes" + i);

        //        //                if (acion == 1)
        //        //                {
        //        //                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        //                    {
        //        //                        com.Parameters.AddWithValue("@_caso", "create");
        //        //                        if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        //                        {
        //        //                            if (acion == 1) com.Parameters.AddWithValue("@id_Nomina", 0); //idGuardar);
        //        //                            if (acion == 2) com.Parameters.AddWithValue("@id_Nomina", CodNomina);
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            if (acion == 1) com.Parameters.AddWithValue("@id_Nomina", 0);
        //        //                            if (acion == 2) com.Parameters.AddWithValue("@id_Nomina", CodNomina);
        //        //                        }

        //        //                        com.Parameters.AddWithValue("@_Cargo", TB_Item.Text);
        //        //                        com.Parameters.AddWithValue("@_CodProyecto", CodProyecto);

        //        //                        com.Parameters.AddWithValue("@_tipo", "");

        //        //                        com.Parameters.AddWithValue("@mes", i);

        //        //                        com.Parameters.AddWithValue("@valor", j);
        //        //                    }

        //        //                }
        //        //                else
        //        //                {
        //        //                    com.Parameters.AddWithValue("@_caso", "create");
        //        //                }
        //        //            }
        //        //        }

        //        //        if (acion == 2)
        //        //        {
        //        //            if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        //            {
        //        //                com.Parameters.AddWithValue("@_caso", "update");
        //        //            }
        //        //            else
        //        //            {
        //        //                com.Parameters.AddWithValue("@_caso", "update");
        //        //            }
        //        //        }
        //        //        try
        //        //        {
        //        //            con.Open();
        //        //            com.ExecuteReader();
        //        //        }
        //        //        catch (Exception ex)
        //        //        {
        //        //            throw ex;
        //        //        }
        //        //        finally
        //        //        {
        //        //            con.Close();
        //        //        }
        //        //    }
        //        //}

        //        #endregion

        //        //Crear nómina.
        //        CrearNomina();
        //    }
        //    if (acion == 2)
        //    {
        //        //Actualizar Nómina.
        //        ActualizarNomina(s_valorAprobado);
        //    }
        //    if (acion == 3)
        //    {
        //        //Modificar Nómina.
        //        ModificarNomina(s_valorAprobado);
        //    }
        //    else //No es un dato válido.
        //    {
        //        //Destruir variables.
        //        CodProyecto = "0";
        //        CodNomina = 0;
        //        ValorTMP = "";
        //        s_valorAprobado = 0;
        //        v_Tipo = "";
        //        Session["CodNomina"] = null;
        //        Session["s_valorAprobado"] = null;
        //        Session["v_Tipo"] = null;
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
        //    }
        //} 
        #endregion

        /// <summary>
        /// Guardar y/o actualizar la información.
        /// </summary>
        /// <param name="acion">Si el valor es 1, guardará la información, si es 2, actualizará dicha información.</param>
        private void alamcenar(int acion)
        {
            //Inicializar variables.            
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String NomActividad = "";
            DataTable RsActividad = new DataTable();
            String Valor = "";
            String sTipoInsumo = "";
            DataTable Rs = new DataTable();
            DataTable RsRevisa = new DataTable();
            String correcto = "";
            DataTable RsTempAux = new DataTable();


            if (acion == 1)
            {
                #region Guardar la información de la nómina.

                #region Si es Interventor.
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    string txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;
                    SqlDataReader reader = ejecutaReader(txtSQL, 1);

                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            int ActividadTmp = 1;
                            txtSQL = "select Id_Nomina as id_Produccion from InterventorNominaTMP ORDER BY Id_Nomina DESC";


                            reader = ejecutaReader(txtSQL, 1);

                            if (reader != null)
                            {
                                if (reader.Read())
                                {
                                    ActividadTmp = Convert.ToInt32(reader["id_Produccion"].ToString()) + 1;
                                }
                            }

                            txtSQL = "Insert into InterventorNominaTMP (Id_Nomina,CodProyecto,Cargo,Tipo) values (" + ActividadTmp + "," + CodProyecto + ",'" + TB_Item.Text + "','" + Session["v_Tipo"].ToString() + "')";


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

                                            txtSQL = "INSERT INTO InterventorNominaMesTMP(CodCargo,Mes,Valor,Tipo) " +
                                            "VALUES(" + ActividadTmp + "," + mes + "," + valor + "," + tipo + ")";

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
                    CodCargo = 0;
                    ValorTMP = "";
                    s_valorAprobado = 0;
                    v_Tipo = "";
                    Session["CodNomina"] = null;
                    Session["s_valorAprobado"] = null;
                    Session["v_Tipo"] = null;
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

                        txtSQL = " select * from InterventorNominaTMP " +
                                 " where CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                        //Asignar resultados a variable DataTable.
                        RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                        #endregion

                        #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA.

                        txtSQL = " Insert into InterventorNomina (CodProyecto, Cargo, Tipo) " +
                                 " values (" + CodProyecto + ", '" + RsActividad.Rows[0]["Cargo"].ToString() + "', '" + RsActividad.Rows[0]["Tipo"].ToString() + "')";

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

                        txtSQL = " DELETE FROM InterventorNominaTMP " +
                                 " where CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                        //Ejecutar consulta.
                        cmd = new SqlCommand(txtSQL, conn);
                        correcto = String_EjecutarSQL(conn, cmd);

                        if (correcto != "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #1: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                            return;
                        }

                        #endregion

                        #region TRAE EL CODIGO DE ACTIVIDAD PARA ADICIONARLO A LA TABLA DEFINITIVA POR MES.

                        txtSQL = " select Id_Nomina, Tipo from InterventorNomina ORDER BY Id_Nomina DESC ";

                        //Asignar resultados a variable DataTable.
                        RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                        if (RsActividad.Rows.Count > 0)
                        {
                            Valor = RsActividad.Rows[0]["Id_Nomina"].ToString();
                            sTipoInsumo = RsActividad.Rows[0]["Tipo"].ToString();
                        }

                        #endregion

                        #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL POR MESES.

                        txtSQL = " select * from InterventorNominaMesTMP " +
                                 " where CodCargo = " + CodCargo;

                        Rs = consultas.ObtenerDataTable(txtSQL, "text");

                        #endregion

                        #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.
                        //INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.
                        foreach (DataRow row_Rs in Rs.Rows)
                        {
                            if (row_Rs["Tipo"].ToString() == "1")
                            {
                                #region Tipo 1.
                                //Modificacion para prevenir dobles registros Vladimir Delgado - oct 30 2007
                                txtSQL = " select * from InterventorNominaMes where mes = " + row_Rs["Mes"].ToString() +
                                         " and codcargo = " + Valor + " and Tipo = 1 ";
                                RsTempAux = consultas.ObtenerDataTable(txtSQL, "text");

                                if (RsTempAux.Rows.Count > 0)
                                {
                                    #region Actualizar.

                                    txtSQL = " update InterventorNominaMes set Valor = " + row_Rs["Valor"].ToString() +
                                             " where mes = " + row_Rs["Mes"].ToString() +
                                             " and codcargo = " + Valor + " and Tipo = 1 ";

                                    #endregion
                                }
                                else
                                {
                                    #region Ingresar.

                                    txtSQL = " Insert into InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                             " values (" + Valor + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 1) ";

                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region Tipo 2.
                                //Modificacion para prevenir dobles registros Vladimir Delgado - oct 30 2007
                                txtSQL = " select * from InterventorNominaMes where mes = " + row_Rs["Mes"].ToString() +
                                         " and codcargo = " + Valor + " and Tipo = 2 ";
                                RsTempAux = consultas.ObtenerDataTable(txtSQL, "text");

                                if (RsTempAux.Rows.Count > 0)
                                {
                                    #region Actualizar.

                                    txtSQL = " update InterventorNominaMes set Valor = " + row_Rs["Valor"].ToString() +
                                             " where mes = " + row_Rs["Mes"].ToString() +
                                             " and codcargo = " + Valor + " and Tipo = 2 ";

                                    #endregion
                                }
                                else
                                {
                                    #region Ingresar.

                                    txtSQL = " Insert into InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                             " values (" + Valor + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 2) ";

                                    #endregion
                                }
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
                        txtSQL = " DELETE InterventorNominaMesTMP " +
                                 " where CodCargo = " + CodCargo;

                        //Ejecutar consulta.
                        cmd = new SqlCommand(txtSQL, conn);
                        correcto = String_EjecutarSQL(conn, cmd);

                        if (correcto != "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar el registro ya insertado en la tabla temporal por mes: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                            return;
                        }
                        #endregion

                        //Destruir variables.
                        CodProyecto = "0";
                        CodCargo = 0;
                        ValorTMP = "";
                        s_valorAprobado = 0;
                        v_Tipo = "";
                        Session["CodNomina"] = null;
                        Session["s_valorAprobado"] = null;
                        Session["v_Tipo"] = null;
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

                        #region Actualización.

                        txtSQL = " DELETE FROM InterventorNominaTMP " +
                                 " where CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                        //Ejecutar consulta.
                        cmd = new SqlCommand(txtSQL, conn);
                        correcto = String_EjecutarSQL(conn, cmd);

                        if (correcto != "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar en tabla temporal " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                            return;
                        }

                        #endregion

                        #region Eliminación.

                        txtSQL = " DELETE FROM InterventorNominaMesTMP " +
                                 " where CodCargo = " + CodCargo;

                        //Ejecutar consulta.
                        cmd = new SqlCommand(txtSQL, conn);
                        correcto = String_EjecutarSQL(conn, cmd);

                        if (correcto != "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar en tabla de meses temporal " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                            return;
                        }

                        #endregion

                        #region Cargar "Cargo" y asignar dicho valor a la variable NomActividad.

                        txtSQL = " SELECT Cargo FROM InterventorNominaTMP WHERE CodProyecto = " + CodProyecto +
                                 " and Id_Nomina = " + CodCargo;

                        //Asignar resultados de la consulta a variable "rs".
                        Rs = consultas.ObtenerDataTable(txtSQL, "text");

                        //Asignar "Cargo" a variable "NomActividad".
                        if (Rs.Rows.Count > 0) { NomActividad = Rs.Rows[0]["Cargo"].ToString(); }

                        #endregion

                        #region Generar tarea pendiente.

                        //Agendar tarea.
                        AgendarTarea agenda = new AgendarTarea
                        (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                        "Cargo de Nómina Rechazado por Gerente Interventor",
                        "Revisar cargo de nómina " + txtNomProyecto + " - Actividad --> " + NomActividad + "<BR><BR>Observaciones:<BR>" + txt_inv_observaciones.Text.Trim(),
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
                        "Catálogo Actividad Nómina");

                        agenda.Agendar();

                        #endregion

                        //Destruir variables.
                        CodProyecto = "0";
                        CodCargo = 0;
                        ValorTMP = "";
                        s_valorAprobado = 0;
                        v_Tipo = "";
                        Session["CodNomina"] = null;
                        Session["s_valorAprobado"] = null;
                        Session["v_Tipo"] = null;
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
                #region Editar la información de la nómina seleccionada.

                if (CodProyecto == "0" || CodCargo == 0)
                {
                    return;
                }
                else
                {
                    //Comprobar si el usuario tiene el código grupo de "Coordinador Interventor" ó "Gerente Interventor".
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Si es Coordinador Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1") //Si
                            {
                                #region Aprobado.

                                #region Actualización.
                                txtSQL = " UPDATE InterventorNominaTMP SET ChequeoCoordinador=1" +
                                         " where CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);
                                #endregion

                                #region Comentarios.
                                //try
                                //{
                                //    //NEW RESULTS:
                                //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                                //    cmd = new SqlCommand(txtSQL, con);

                                //    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                                //    cmd.CommandType = CommandType.Text;
                                //    cmd.ExecuteNonQuery();
                                //    con.Close();
                                //    con.Dispose();
                                //    cmd.Dispose();
                                //}
                                //catch { } 
                                #endregion

                                #region Consulta y asignar resultados a "RsActividad".

                                txtSQL = " select Id_grupo from Grupo " +
                                         " where NomGrupo = 'Gerente Interventor' ";

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region Consulta y asignar resultados a "RsActividad" luego de haber consultado el Id_Grupo.
                                txtSQL = " select CodContacto from GrupoContacto " +
                                         " where CodGrupo =" + RsActividad.Rows[0]["Id_grupo"].ToString();

                                //Asignar resultados.
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region prTareaAsignarGerenteNomina. ESTÁ COMENTADO EN FONADE CLÁSICO.

                                //prTareaAsignarGerenteNomina RsActividad("CodContacto"),Session("CodUsuario"),CodProyecto,txtNomProyecto,CodCargo

                                //Agendar tarea.
                                AgendarTarea agenda = new AgendarTarea
                                (usuario.IdContacto,
                                "Revisión Actividad al Plan Operativo",
                                "Revisión Adición, Modificación o Borrado de Actividad del interventor al Plan Operativo" + txtNomProyecto,
                                CodProyecto,
                                15,
                                "0",
                                false,
                                1,
                                true,
                                false,
                                usuario.IdContacto,
                                "",
                                "",
                                "Asignar Gerente Nómina");

                                agenda.Agendar();

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                CodCargo = 0;
                                ValorTMP = "";
                                s_valorAprobado = 0;
                                v_Tipo = "";
                                Session["CodNomina"] = null;
                                Session["s_valorAprobado"] = null;
                                Session["v_Tipo"] = null;
                                //RedirectPage(false, string.Empty, "cerrar");
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                            else
                            {
                                #region No Aprobado.

                                #region Consultar y asignar resultados a "RsActividad".

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
                                         " where CodProyecto = " + CodProyecto + " and Id_Actividad = " + CodCargo;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                #endregion

                                #region Eliminación #2.

                                txtSQL = " DELETE FROM InterventorNominaMesTMP " +
                                         " where CodCargo = " + CodCargo;

                                #region Comentarios NO BORRAR!.
                                ////Ejecutar consulta.
                                //cmd = new SqlCommand(txtSQL, conn);
                                //correcto = String_EjecutarSQL(conn, cmd); 
                                #endregion

                                //Ejecutar setencia.
                                ejecutaReader(txtSQL, 2);

                                #endregion

                                #region Consultar, asignar resultados a variable "Rs" y obtener el nombre del cargo.

                                txtSQL = " SELECT Cargo FROM InterventorNominaTMP " +
                                         " WHERE CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                                //Asignar resultados a la variable "Rs".
                                Rs = consultas.ObtenerDataTable(txtSQL, "text");

                                //Obtener el nombre del cargo.
                                if (Rs.Rows.Count > 0) { NomActividad = Rs.Rows[0]["Cargo"].ToString(); Rs = null; }

                                #endregion

                                #region Generar nueva tarea pendiente.
                                //Agendar tarea.
                                AgendarTarea agenda = new AgendarTarea
                                (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                                "Cargo de Nómina Rechazado por Coordinador de Interventoria",
                                "Revisar cargo de nómina " + txtNomProyecto + " - Actividad --> " + NomActividad + "<BR><BR>Observaciones:<BR>" + txt_inv_observaciones.Text.Trim(),
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
                                "Catálogo Nómina");

                                agenda.Agendar();

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                CodCargo = 0;
                                ValorTMP = "";
                                s_valorAprobado = 0;
                                v_Tipo = "";
                                Session["CodNomina"] = null;
                                Session["s_valorAprobado"] = null;
                                Session["v_Tipo"] = null;
                                //RedirectPage(false, string.Empty, "cerrar");
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                        }

                        #endregion

                        #region Si es Gerente Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1") //Si
                            {
                                #region Aprobado.

                                #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL.

                                txtSQL = " select * from InterventorNominaTMP " +
                                         " where CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                                //Asignar resultados a variable DataTable.
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region ACTUALIZA LOS REGISTROS EN LA TABLA DEFINITIVA.

                                txtSQL = " Update InterventorNomina set CodProyecto = " + CodProyecto + "," +
                                         " Tipo = '" + RsActividad.Rows[0]["Tipo"].ToString() + "' " +
                                         " WHERE CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region BORRAR EL REGISTRO YA ACTUALIZADO DE LA TABLA TEMPORAL.

                                txtSQL = " DELETE FROM InterventorNominaTMP " +
                                         " where CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #1: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region TRAE EL CODIGO DE ACTIVIDAD PARA ADICIONARLO A LA TABLA DEFINITIVA POR MES.

                                txtSQL = " select * from InterventorNominaMesTMP " +
                                         " where CodCargo = " + CodCargo;

                                //Asignar resultados a variable DataTable.
                                Rs = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region BORRAR TODOS LOS REGISTROS DE LA TABLA InterventorNominaMes CORRESPONDIENTES AL CODIGO DE ACTIVIDAD.

                                txtSQL = " DELETE FROM InterventorNominaMes " +
                                         " where CodCargo = " + CodCargo;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #2: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.
                                //INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.
                                foreach (DataRow row_Rs in Rs.Rows)
                                {
                                    if (row_Rs["Tipo"].ToString() == "1")
                                    {
                                        #region Inserción de tipo 1.
                                        txtSQL = " Insert into InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                                 " values (" + CodCargo + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 1)";
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Inserta de tipo 2.
                                        txtSQL = " Insert into InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                                 " values (" + CodCargo + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 2)";
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
                                txtSQL = " DELETE InterventorNominaMesTMP " +
                                         " where CodCargo = " + CodCargo;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar el registro ya insertado en la tabla temporal por mes: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }
                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                CodCargo = 0;
                                ValorTMP = "";
                                s_valorAprobado = 0;
                                v_Tipo = "";
                                Session["CodNomina"] = null;
                                Session["s_valorAprobado"] = null;
                                Session["v_Tipo"] = null;
                                //RedirectPage(false, string.Empty, "cerrar");
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                        }

                        #endregion
                    }
                    if (usuario.CodGrupo == Constantes.CONST_Interventor) //Si el usuario tiene el código grupo "Interventor".
                    {
                        #region Si es Interventor.
                        string txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;
                        SqlDataReader reader = ejecutaReader(txtSQL, 1);

                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                string NomProducto;

                                txtSQL = "select Cargo as NomProducto from InterventorNomina where CodProyecto=" + CodProyecto + " and Id_Nomina=" + CodCargo;

                                reader = ejecutaReader(txtSQL, 1);

                                if (reader != null)
                                {
                                    if (reader.Read())
                                        NomProducto = reader["NomProducto"].ToString();
                                    else
                                        NomProducto = "";

                                    txtSQL = "Insert into InterventorNominaTMP (Id_Nomina,CodProyecto,cargo,Tipo,Tarea) values (" + CodCargo + "," + CodProyecto + ",'" + NomProducto + "','" + Session["v_Tipo"].ToString() + "','Modificar')";

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

                                                    txtSQL = "INSERT INTO InterventorNominaMesTMP(CodCargo,Mes,Valor,Tipo) " +
                                                    "VALUES(" + CodCargo + "," + mes + "," + valor + "," + tipo + ")";

                                                    ejecutaReader(txtSQL, 2);
                                                }
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
                        CodCargo = 0;
                        ValorTMP = "";
                        s_valorAprobado = 0;
                        v_Tipo = "";
                        Session["CodNomina"] = null;
                        Session["s_valorAprobado"] = null;
                        Session["v_Tipo"] = null;
                        //RedirectPage(false, string.Empty, "cerrar");
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                        #endregion
                    }
                }

                #endregion
            }
            if (acion == 3) /*Eliminar*/
            {
                #region Eliminar según Gerente Interventor.
                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    #region BORRA LOS REGISTROS EN LA TABLA DEFINITIVA.

                    txtSQL = " DELETE FROM InterventorNomina " +
                             "where CodProyecto = " + CodProyecto + " and Id_Nomina = " + CodCargo;

                    //Ejecutar consulta.
                    cmd = new SqlCommand(txtSQL, conn);
                    correcto = String_EjecutarSQL(conn, cmd);

                    if (correcto != "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #1: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                        return;
                    }

                    #endregion

                    #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL.

                    txtSQL = " DELETE FROM InterventorNominaTMP " +
                             " where CodProyecto=" + CodProyecto + " and Id_Nomina = " + CodCargo;

                    //Ejecutar consulta.
                    cmd = new SqlCommand(txtSQL, conn);
                    correcto = String_EjecutarSQL(conn, cmd);

                    if (correcto != "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #2: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                        return;
                    }

                    #endregion

                    #region BORRA LOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.

                    txtSQL = " DELETE FROM InterventorNominaMes " +
                             " where CodCargo = " + CodCargo;

                    //Ejecutar consulta.
                    cmd = new SqlCommand(txtSQL, conn);
                    correcto = String_EjecutarSQL(conn, cmd);

                    if (correcto != "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #3: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                        return;
                    }

                    #endregion

                    #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL POR MESES.

                    txtSQL = " DELETE FROM InterventorNominaMesTMP " +
                             " where CodCargo = " + CodCargo;

                    //Ejecutar consulta.
                    cmd = new SqlCommand(txtSQL, conn);
                    correcto = String_EjecutarSQL(conn, cmd);

                    if (correcto != "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #4: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                        return;
                    }

                    #endregion

                    //Destruir variables.
                    CodProyecto = "0";
                    CodCargo = 0;
                    ValorTMP = "";
                    s_valorAprobado = 0;
                    v_Tipo = "";
                    Session["CodNomina"] = null;
                    Session["s_valorAprobado"] = null;
                    Session["v_Tipo"] = null;
                    //RedirectPage(false, string.Empty, "cerrar");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                }

                #endregion
            }
            else //No es un dato válido.
            {
                //Destruir variables.
                CodProyecto = "0";
                CodCargo = 0;
                ValorTMP = "";
                s_valorAprobado = 0;
                v_Tipo = "";
                Session["CodNomina"] = null;
                Session["s_valorAprobado"] = null;
                Session["v_Tipo"] = null;
                //RedirectPage(false, string.Empty, "cerrar");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
            }
        }

        protected void Crear()
        {
            try
            {

                var dt = new DataTable();
                consultas.Parameters = new[]
                                           {
                                              
                                               new SqlParameter

                                                {
                                                       ParameterName = "@_CodProyecto" ,Value = CodProyecto 
                                                },
                                                 new SqlParameter

                                                {
                                                       ParameterName = "@id_Nomina" ,Value = CodCargo 
                                                },
                                               new SqlParameter
                                                {
                                                       ParameterName = "@_cargo" ,Value = TB_Item.Text.ToString() 
                                                },

                                                  new SqlParameter
                                                {
                                                       ParameterName = "@_tipo" ,Value = 1
                                                },
                                                    new SqlParameter
                                                {
                                                       ParameterName = "@_caso" ,Value = "create"
                                                },
                                           };

                dt = consultas.ObtenerDataTable("MD_Insertar_Actualizar_InterventorTMP");

                if (dt.Rows.Count != 0)
                {

                    Response.Redirect("../Interventoria/SeguimientoFrameSet.aspx");
                }
            }
            catch (Exception ex)
            {

            }
        }

        #region Código anterior comentado. NO BORRAR.

        //String item = TB_Item.Text;

        //String idGuardar = "";
        //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

        //if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //{
        //    SqlCommand cmd = new SqlCommand("SELECT [CodCoordinador] FROM [Fonade].[dbo].[Interventor] WHERE [CodContacto] = " + CodUsuario, conn);

        //    try
        //    {
        //        conn.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        reader.Read();
        //        String codCordinador = reader["CodCoordinador"].ToString();
        //        reader.Close();
        //        conn.Close();

        //        if (String.IsNullOrEmpty(codCordinador))
        //        {
        //            System.Windows.Forms.MessageBox.Show("No tiene ningún coordinador asignado.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        //            return;
        //        }
        //        else
        //        {
        //            conn.Open();
        //            cmd = new SqlCommand("SELECT MAX([Id_Actividad]) Id_Actividad FROM [Fonade].[dbo].[ProyectoActividadPOInterventorTMP]", conn);
        //            reader = cmd.ExecuteReader();
        //            reader.Read();
        //            if (reader.FieldCount != 0)
        //            {
        //                if (String.IsNullOrEmpty(reader["Id_Actividad"].ToString())) idGuardar = "" + 1;
        //                else idGuardar = "" + (Int64.Parse(reader["Id_Actividad"].ToString()) + 1 + Int64.Parse(CodProyecto));
        //            }

        //        }
        //    }
        //    catch (SqlException se)
        //    {
        //        throw se;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        //string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        //using (var con = new SqlConnection(conexionStr))
        //{
        //    using (var com = con.CreateCommand())
        //    {
        //        com.CommandText = "MD_Insertar_Actualizar_ProyectoActividadPO";
        //        com.CommandType = System.Data.CommandType.StoredProcedure;

        //        if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        {
        //            if (acion == 1) com.Parameters.AddWithValue("@_Id_Actividad", 0); //idGuardar);
        //            if (acion == 2) com.Parameters.AddWithValue("@_Id_Actividad", CodActividad);
        //        }
        //        else
        //        {
        //            if (acion == 1) com.Parameters.AddWithValue("@_Id_Actividad", 0);
        //            if (acion == 2) com.Parameters.AddWithValue("@_Id_Actividad", CodActividad);
        //        }

        //        //com.Parameters.AddWithValue("@_NomActividad", actividad);
        //        com.Parameters.AddWithValue("@_CodProyecto", CodProyecto);
        //        com.Parameters.AddWithValue("@_CodGrupo", usuario.CodGrupo);

        //        if (acion == 1)
        //        {
        //            if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //            {
        //                com.Parameters.AddWithValue("@_caso", "CREATEINTERVENTOR");
        //            }
        //            else
        //            {
        //                com.Parameters.AddWithValue("@_caso", "CREATE");
        //            }
        //        }
        //        if (acion == 2)
        //        {
        //            if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //            {
        //                com.Parameters.AddWithValue("@_caso", "UPDATEINTERVENTOR");
        //            }
        //            else
        //            {
        //                com.Parameters.AddWithValue("@_caso", "UPDATE");
        //            }
        //        }
        //        try
        //        {
        //            con.Open();
        //            com.ExecuteReader();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            con.Close();
        //        }
        //    }
        //}

        //String RsActividad = "";
        ////textbox
        //if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //{
        //    RsActividad = idGuardar;
        //}
        //else
        //{
        //    //SqlCommand cmd = new SqlCommand("SELECT [Id_Actividad] FROM [Fonade].[dbo].[ProyectoActividadPO] WHERE [NomActividad] = " + actividad + " AND [CodProyecto] = " + CodProyecto, conn);
        //    SqlCommand cmd = new SqlCommand("SELECT [Id_Actividad] FROM [Fonade].[dbo].[ProyectoActividadPO] WHERE [CodProyecto] = " + CodProyecto + " ");

        //    try
        //    {
        //        conn.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        reader.Read();
        //        RsActividad = reader["CodCoordinador"].ToString();
        //        reader.Close();
        //    }
        //    catch (SqlException se)
        //    {
        //        throw se;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        #endregion

        #region MD_Insertar_Actualizar_ProyectoActividadPOMes COMENTADO.

        //using (var con = new SqlConnection(conexionStr))
        //{
        //    using (var com = con.CreateCommand())
        //    {
        //        com.CommandText = "MD_Insertar_Actualizar_ProyectoActividadPOMes";
        //        com.CommandType = System.Data.CommandType.StoredProcedure;

        //        for (int j = 1; j <= 2; j++)
        //        {
        //            for (int i = 1; i <= 12; i++)
        //            {
        //                Label controltext;
        //                controltext = (Label)this.FindControl("TotalMes" + i);


        //                if (acion == 1) com.Parameters.AddWithValue("@CodActividad", RsActividad);
        //                if (acion == 2) com.Parameters.AddWithValue("@CodActividad", CodActividad);

        //                com.Parameters.AddWithValue("@Mes", i);
        //                com.Parameters.AddWithValue("@CodTipoFinanciacion", j);
        //                com.Parameters.AddWithValue("@Valor", controltext.Text);

        //                if (acion == 1)
        //                {
        //                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //                    {
        //                        com.Parameters.AddWithValue("@_caso", "CREATEINTERVENTOR");
        //                    }
        //                    else
        //                    {
        //                        com.Parameters.AddWithValue("@_caso", "CREATE");
        //                    }
        //                }
        //                if (acion == 2)
        //                {
        //                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //                    {
        //                        com.Parameters.AddWithValue("@_caso", "UPDATEINTERVENTOR");
        //                    }
        //                    else
        //                    {
        //                        com.Parameters.AddWithValue("@_caso", "UPDATE");
        //                    }
        //                }

        //                try
        //                {
        //                    con.Open();
        //                    com.ExecuteReader();
        //                }
        //                catch (Exception ex)
        //                {
        //                    throw ex;
        //                }
        //                finally
        //                {
        //                    con.Close();
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion

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

            if (textAporte.Text.Trim() == "")
            { suma2 = 0; textAporte.Text = suma2.ToString(); }
            else { suma2 = Double.Parse(textAporte.Text); valor_suma2 = Convert.ToInt32(suma2); textAporte.Text = valor_suma2.ToString(); }

            #region Comentarios anteriores NO BORRAR.

            //if (textboxID.Length == 7)
            //{
            //    objeto = "TotalMes" + (textboxID[textboxID.Length - 2]) + (textboxID[textboxID.Length - 1]);

            //    controltext = (Label)this.FindControl(objeto);
            //    string a = controltext.ID;

            //    textFondo = (TextBox)this.FindControl("Sueldo" + (textboxID[textboxID.Length - 2]) + (textboxID[textboxID.Length - 1]));
            //    textAporte = (TextBox)this.FindControl("Prestaciones" + (textboxID[textboxID.Length - 2]) + (textboxID[textboxID.Length - 1]));

            //    if (textAporte.Text.Trim() == "")
            //    { suma2 = 0; textAporte.Text = suma2.ToString(); }
            //    else { suma2 = Double.Parse(textAporte.Text); valor_suma2 = Convert.ToInt32(suma2); textAporte.Text = valor_suma2.ToString(); }
            //}
            //else
            //{
            //    controltext = (Label)this.FindControl(objeto);
            //    string a = controltext.ID;

            //    textFondo = (TextBox)this.FindControl("Sueldo" + (textboxID[textboxID.Length - 1]));
            //    textAporte = (TextBox)this.FindControl("Prestaciones" + (textboxID[textboxID.Length - 1]));

            //    if (textAporte.Text.Trim() == "")
            //    { suma2 = 0; textAporte.Text = suma2.ToString(); }
            //    else { suma2 = Double.Parse(textAporte.Text); valor_suma2 = Convert.ToInt32(suma2); textAporte.Text = valor_suma2.ToString(); }
            //} 

            #endregion


            try
            {
                if (String.IsNullOrEmpty(textFondo.Text))
                { suma1 = 0; textFondo.Text = suma1.ToString(); }
                else
                { suma1 = Double.Parse(textFondo.Text); textFondo.Text = suma1.ToString(); }

                if (String.IsNullOrEmpty(textAporte.Text))
                { suma2 = 0; textAporte.Text = suma2.ToString(); }
                else { suma2 = Double.Parse(textAporte.Text); valor_suma2 = Convert.ToInt32(suma2); textAporte.Text = valor_suma2.ToString(); }

                //Con formato
                //controltext.Text = "$" + (suma1 + suma2).ToString("0,0.00", CultureInfo.InvariantCulture);
                controltext.Text = "" + (suma1 + valor_suma2);

                labelfondocosto.Text = "0";


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
                                    { if (text.Text.Trim() == "") { text.Text = "0"; } labelfondocosto.Text = (Convert.ToDouble(labelfondocosto.Text) + Convert.ToDouble(text.Text)).ToString(); }
                                    if (L_SumaTotalescosto != null)
                                        L_SumaTotalescosto.Text = (Convert.ToDouble(labelfondocosto.Text)).ToString();
                                }
                            }
                        }
                    }
                }


            }
            catch (FormatException) { }
            catch (NullReferenceException) { }


        }

        private void sumarAporte(TextBox textbox, string param_opcional = null)
        {
            String textboxID = textbox.ID;


            var labelfondocosto = this.FindControl("labelfondocosto") as Label;
            var labelaportescosto = this.FindControl("labelaportescosto") as Label;
            var L_SumaTotalescosto = this.FindControl("L_SumaTotalescosto") as Label;

            int limit = 0;
            if (textboxID.Length == 7)
                limit = 1;
            else
                limit = 2;

            String objeto = "TotalMes" + textboxID.Substring(6, limit);

            Label controltext;
            controltext = (Label)this.FindControl(objeto);

            TextBox textFondo;
            textFondo = (TextBox)this.FindControl("Fondoo" + textboxID.Substring(6, limit)); //Sueldo
            TextBox textAporte;
            textAporte = (TextBox)this.FindControl("Aporte" + textboxID.Substring(6, limit)); //Prestaciones
            try
            {
                Double suma1;
                Double suma2;

                if (String.IsNullOrEmpty(textFondo.Text))
                    suma1 = 0;
                else
                    suma1 = Double.Parse(textFondo.Text);

                if (String.IsNullOrEmpty(textAporte.Text))
                    suma2 = 0;
                else
                    suma2 = Double.Parse(textAporte.Text);



                if (!String.IsNullOrEmpty(param_opcional.Trim()))
                {
                    //Tratamiento para los Productos en "Ventas".
                    Int32 suma_convertida_1 = Convert.ToInt32(suma1);
                    double suma_convertida_2 = Math.Floor(suma2);
                    double valor = suma_convertida_1 + suma_convertida_2;
                    controltext.Text = "" + valor;
                }
                else
                {
                    controltext.Text = "" + (suma1 + suma2);
                }

                labelaportescosto.Text = "0";
                Int32 cantidades = 0;

                foreach (TableRow fila in T_Meses.Rows)
                {
                    cantidades = 0;
                    foreach (TableCell celda in fila.Cells)
                    {
                        foreach (Control control in celda.Controls)
                        {
                            try
                            {

                            }
                            catch (Exception) { }
                        }
                    }
                }

                //foreach (Control miControl in T_Meses.Controls)
                //{
                //    var tablerow = miControl.Controls;

                //    foreach (Control micontrolrows in tablerow)
                //    {
                //        var hijos = micontrolrows.Controls;

                //        foreach (Control chijos in hijos)
                //        {
                //            if (chijos.GetType() == typeof(TextBox))
                //            {
                //                var text = chijos as TextBox;

                //                if (text.ID.StartsWith(Prestaciones))
                //                {
                //                    if (labelaportescosto != null)
                //                        labelaportescosto.Text = (Convert.ToDouble(labelaportescosto.Text) + Convert.ToDouble(text.Text)).ToString();

                //                    if (L_SumaTotalescosto != null)
                //                        L_SumaTotalescosto.Text = (Convert.ToDouble(L_SumaTotalescosto.Text) + (Convert.ToDouble(labelaportescosto.Text))).ToString();
                //                }
                //            }
                //        }
                //    }
                //}


            }
            catch (FormatException) { }
            catch (NullReferenceException) { }


        }

        /// <summary>
        /// Textbox Changed para Fondo/Sueldo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            sumar(textbox);
        }

        /// <summary>
        /// TextboxChanged para Aportes/Prestaciones.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TextBoxAportes_TextChanged(object sender, EventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            sumarAporte(textbox, "0");
        }

        /// <summary>
        /// Cerrar la ventana.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_Cancelar_Click(object sender, EventArgs e)
        {
            //Destruir variables.
            CodProyecto = "0";
            CodCargo = 0;
            ValorTMP = "";
            s_valorAprobado = 0;
            v_Tipo = "";
            Session["CodNomina"] = null;
            Session["s_valorAprobado"] = null;
            Session["v_Tipo"] = null;
            //RedirectPage(false, string.Empty, "cerrar");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 09/04/2014.
        /// Modificar la información del valor seleccionado en la grilla de "Personal Calificado" en "Nómina".
        /// Modificado por Mauricio Arias Olave el 12/04/2014.
        /// Modificado por Mauricio Arias Olave el 15/05/2014.
        /// </summary>
        /// <param name="nomina">Nómina a consultar.</param>
        /// <param name="VieneDeCambiosPO">TRUE = Cargar la información de "CambiosPO.aspx". // FALSE = Cargar la información de "FrameNominaInter.aspx".</param>
        private void BuscarDatos_Nomina(Int32 nomina, bool VieneDeCambiosPO)
        {
            #region Versión unificada "se había puesto TODO en la página CatalogoProduccionTMP.aspx".

            if (VieneDeCambiosPO == true)
            {
                #region Cargar la información según la página "CambiosPO.aspx".

                //Inicializar variables.
                String sqlConsulta = "";
                String _tarea = "";
                String _ChequeoCoordinador = "";
                String _ChequeoGerente = "";
                DataTable tabla_sql = new DataTable();
                SqlCommand cmd = new SqlCommand();

                try
                {
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Procesar la información como Coordinador o Gerente Interventor.

                        #region Consulta #1.
                        //Ejecutar consulta para obtener determinados valores.
                        sqlConsulta = " SELECT * FROM InterventorNominaTMP " +
                                      " WHERE id_Nomina = " + CodCargo;

                        //Asignar resultados de la consulta anterior a variable DataTable.
                        tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                        if (tabla_sql.Rows.Count > 0)
                        {
                            #region Aprobación de cambios del coordinador.
                            if (!String.IsNullOrEmpty(_ChequeoCoordinador))
                            {
                                if (_ChequeoCoordinador == "True" || _ChequeoCoordinador == "1")
                                { dd_inv_aprobar.SelectedValue = "1"; }
                            }
                            else
                            { dd_inv_aprobar.SelectedValue = "0"; }
                            #endregion

                            #region Aprobación de cambios del gerente.
                            if (!String.IsNullOrEmpty(_ChequeoCoordinador))
                            {
                                if (_ChequeoCoordinador == "True" || _ChequeoCoordinador == "1")
                                { dd_inv_aprobar.SelectedValue = "1"; }
                            }
                            else
                            { dd_inv_aprobar.SelectedValue = "0"; }
                            #endregion

                            //Cargar la información en los controles del formulario.
                            TB_Item.Text = tabla_sql.Rows[0]["Cargo"].ToString();
                        }
                        else
                        {
                            //Tarea ya aprobada (tendría que salir un botón para cerrar la ventana actual...)...
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Tarea ya aprobada.');this.window.close();", true);
                            return;
                        }
                        #endregion

                        #region Consulta #2.

                        sqlConsulta = " select * from InterventorNominaMesTMP where CodCargo = " + nomina + " order by mes";

                        #endregion

                        #endregion
                    }
                    else
                    {
                        if (usuario.CodGrupo == Constantes.CONST_Interventor)
                        {
                            #region Procesar la información como Interventor.

                            #region Consulta #1.
                            //Consulta.
                            sqlConsulta = " SELECT *  FROM InterventorNomina " +
                                          " WHERE id_Nomina =" + CodCargo;

                            //Inicializar la variable "en caso de que contenga algo".
                            tabla_sql = new DataTable();

                            //Asignar resultados de la consulta en variable DataTable.
                            tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                            //Si la consulta anterior contiene datos...
                            if (tabla_sql.Rows.Count > 0)
                            {
                                //Cargar la información en los controles del formulario.
                                TB_Item.Text = tabla_sql.Rows[0]["Cargo"].ToString();
                            }
                            #endregion

                            #region Consulta #2.

                            sqlConsulta = " select * from InterventorNominaMes where CodCargo = " + nomina + " order by mes";

                            #endregion

                            #endregion
                        }
                    }

                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                    cmd = new SqlCommand(sqlConsulta, conn);
                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            TextBox controltext;
                            string valor_Obtenido = reader["Tipo"].ToString();//.Equals("1");

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
                        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error " + se.Message + "')", true);
                    }
                    finally
                    {
                        if (conn != null)
                            conn.Close();
                    }
                }
                catch { }

                #endregion
            }
            else
            {
                #region Cargar la información según la página "FrameNominaInter.aspx".

                //Obtiene la conexión
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

                //Inicializa la variable para generar la consulta.
                String sqlConsulta = "";

                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    sqlConsulta = "select * from InterventorNominaMesTMP where CodCargo = " + nomina + " order by mes";
                }
                else
                {
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        sqlConsulta = "select * from InterventorNominaMes where CodCargo = " + nomina + " order by mes";
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
                        string valor_Obtenido = reader["Tipo"].ToString();//.Equals("1");

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

            #region Versión 2.0 COMENTADA.

            ////Inicializar variables.
            //String sqlConsulta = "";
            //String _tarea = "";
            //String _ChequeoCoordinador = "";
            //String _ChequeoGerente = "";
            //DataTable tabla_sql = new DataTable();
            //SqlCommand cmd = new SqlCommand();

            //try
            //{
            //    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
            //    {
            //        #region Procesar la información como Coordinador o Gerente Interventor.

            //        #region Consulta #1.
            //        //Ejecutar consulta para obtener determinados valores.
            //        sqlConsulta = " SELECT * FROM InterventorNominaTMP " +
            //                      " WHERE id_Nomina = " + CodNomina;

            //        //Asignar resultados de la consulta anterior a variable DataTable.
            //        tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

            //        if (tabla_sql.Rows.Count > 0)
            //        {
            //            #region Aprobación de cambios del coordinador.
            //            if (!String.IsNullOrEmpty(_ChequeoCoordinador))
            //            {
            //                if (_ChequeoCoordinador == "True" || _ChequeoCoordinador == "1")
            //                { dd_inv_aprobar.SelectedValue = "1"; }
            //            }
            //            else
            //            { dd_inv_aprobar.SelectedValue = "0"; }
            //            #endregion

            //            #region Aprobación de cambios del gerente.
            //            if (!String.IsNullOrEmpty(_ChequeoCoordinador))
            //            {
            //                if (_ChequeoCoordinador == "True" || _ChequeoCoordinador == "1")
            //                { dd_inv_aprobar.SelectedValue = "1"; }
            //            }
            //            else
            //            { dd_inv_aprobar.SelectedValue = "0"; }
            //            #endregion

            //            //Cargar la información en los controles del formulario.
            //            TB_Item.Text = tabla_sql.Rows[0]["Cargo"].ToString();
            //        }
            //        else
            //        {
            //            //Tarea ya aprobada (tendría que salir un botón para cerrar la ventana actual...)...
            //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Tarea ya aprobada.');this.window.close();", true);
            //            return;
            //        }
            //        #endregion

            //        #region Consulta #2.

            //        sqlConsulta = " select * from InterventorNominaMesTMP where CodCargo = " + nomina +" order by mes";

            //        #endregion

            //        #endregion
            //    }
            //    else
            //    {
            //        if (usuario.CodGrupo == Constantes.CONST_Interventor)
            //        {
            //            #region Procesar la información como Interventor.

            //            #region Consulta #1.
            //            //Consulta.
            //            sqlConsulta = " SELECT *  FROM InterventorNomina " +
            //                          " WHERE id_Nomina =" + CodNomina;

            //            //Inicializar la variable "en caso de que contenga algo".
            //            tabla_sql = new DataTable();

            //            //Asignar resultados de la consulta en variable DataTable.
            //            tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

            //            //Si la consulta anterior contiene datos...
            //            if (tabla_sql.Rows.Count > 0)
            //            {
            //                //Cargar la información en los controles del formulario.
            //                TB_Item.Text = tabla_sql.Rows[0]["Cargo"].ToString();
            //            }
            //            #endregion

            //            #region Consulta #2.

            //            sqlConsulta = " select * from InterventorNominaMes where CodCargo = " + nomina +" order by mes";

            //            #endregion

            //            #endregion
            //        }
            //    }

            //    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            //    cmd = new SqlCommand(sqlConsulta, conn);
            //    try
            //    {
            //        conn.Open();
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            TextBox controltext;
            //            string valor_Obtenido = reader["Tipo"].ToString();//.Equals("1");

            //            if (valor_Obtenido.Equals("1"))
            //                controltext = (TextBox)this.FindControl("Fondoo" + reader["Mes"].ToString());
            //            else
            //                controltext = (TextBox)this.FindControl("Aporte" + reader["Mes"].ToString());

            //            if (String.IsNullOrEmpty(reader["Valor"].ToString()))
            //                controltext.Text = "0";
            //            else
            //            {
            //                Double valor = Double.Parse(reader["Valor"].ToString());
            //                controltext.Text = valor.ToString();
            //            }

            //            sumar(controltext);
            //        }
            //    }
            //    //catch (SqlException se)
            //    catch (Exception se)
            //    {
            //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error " + se.Message + "')", true);
            //    }
            //    finally
            //    {
            //        if (conn != null)
            //            conn.Close();
            //    }
            //}
            //catch { }

            #endregion

            #region No borrar.
            //SqlCommand cmd = new SqlCommand(sqlConsulta, conn);
            //try
            //{
            //    conn.Open();
            //    SqlDataReader reader = cmd.ExecuteReader();
            //    if (reader.Read())
            //    {
            //        #region Validar si es gerente o coordinador COMENTADO.
            //        //try
            //        //{
            //        //    //Session["Tarea"] = reader["Tarea"].ToString();
            //        //    //Chequeo_CoordinadorInterventor = reader["ChequeoCoordinador"].ToString();
            //        //    //Chequeo_GerenteInterventor = reader["ChequeoGerente"].ToString();
            //        //}
            //        //catch (NullReferenceException) { }
            //        //catch (Exception) { } 
            //        #endregion

            //        #region (Según el clásico "FONADE clásico") sólo se edita la información de los campos de texto.
            //        //TB_item.Text = reader["Item"].ToString();
            //        //TB_Actividad.Text = reader["NomActividad"].ToString();
            //        //TB_metas.Text = reader["Metas"].ToString(); 
            //        #endregion

            //        //Ocultar campos que NO se usan en este formulario.
            //        L_Actividad.Visible = false;
            //        L_Item.Visible = false;
            //        L_Metas.Visible = false;
            //        TB_Actividad.Visible = false;
            //        TB_item.Visible = false;
            //        TB_metas.Visible = false;
            //    }
            //}
            //catch (SqlException se)
            //{
            //    throw se;
            //}
            //finally
            //{
            //    conn.Close();
            //} 
            #endregion
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/04/2014. Se crea este método para llamar por separado a las consultas.
        /// APLICABLE PARA EL MÉTODO BuscarDatos_Nomina().
        /// </summary>
        /// <param name="sqlConsulta">Consulta SQL.</param>
        /// <param name="connection">Conexión.</param>
        private void Ejecutar(string sqlConsulta, SqlConnection connection)
        {
            //Obtiene la conexión
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TextBox controltext;
                    TextBox costoTotal;
                    if (reader["Tipo"].ToString().Equals("1")) //CodTipoFinanciacion
                    {
                        controltext = (TextBox)this.FindControl("Fondoo" + reader["Mes"].ToString());
                    }
                    else
                    {
                        controltext = (TextBox)this.FindControl("Fondoo" + reader["Valor"].ToString());
                    }
                    controltext.Text = reader["Valor"].ToString();
                    sumar(controltext);
                }
                connection.Close();
            }
            catch (SqlException se)
            {
                string h = se.Message;
                throw se;
            }
            finally
            {
                connection.Close();
            }
        }

        #region Métodos creados el 16/04/2014.

        /// <summary>
        /// Crear la nómina, falta que lea los valores de la tabla.
        /// </summary>
        private void CrearNomina()
        {
            //Inicializar variables.
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta;
            bool procesado = false;
            int CodNomina_Autoincr = 1;
            int i = 1;

            try
            {
                if (v_Tipo.Trim() == null || v_Tipo.Trim() == "")
                {
                    //No le ha enviado el valor "Tipo".
                    //Cerrar la ventana.
                    ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                    return;
                }
                else
                {
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        #region Procesar la información.

                        //Consultar si tiene Coordinador asignado.
                        var result = (from t in consultas.Db.Interventors
                                      where t.CodContacto == usuario.IdContacto
                                      select new { t.CodCoordinador }).FirstOrDefault();

                        if (result.CodCoordinador == 0) //No existe
                        {
                            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
                            return;
                        }
                        else
                        {
                            #region Ejecutar sentencias de inserción SQL.

                            //Consultar el Id_Nomina para autoincrementarlo y usarlo en la inserción de datos.
                            var result_nomina_autoincremental = (from t in consultas.Db.InterventorNominaTMPs
                                                                 select new { t.Id_Nomina }).OrderByDescending(x => x.Id_Nomina);

                            if (result_nomina_autoincremental.Count() == 0)
                            {
                                ///No hay valores, por lo que se debe usar un 1 como valor autoincremental (Id_de la nueva nómina.)
                                ///Es decir, se emplea la variable "CodNomina_Autoincr".
                            }
                            else
                            {
                                //Incrementar valor.
                                CodNomina_Autoincr = result_nomina_autoincremental.First().Id_Nomina + 1;

                                //Ejecutar Insert #1.
                                sqlConsulta = " INSERT INTO InterventorNominaTMP (Id_Nomina,CodProyecto,Cargo,Tipo) " +
                                              " VALUES (" + CodNomina_Autoincr + ", " + CodProyecto + ", '" + TB_Item.Text.Trim() + "', '" + v_Tipo + "') ";
                                //ejecutaReader(sqlConsulta, 2); //1 = retornar // 2 = ejecutar.

                                cmd = new SqlCommand(sqlConsulta, connection);
                                procesado = EjecutarSQL(connection, cmd);

                                if (procesado) //Si es TRUE, el proceso debe seguir normal, si no sale nada, toca revisar el código.
                                {
                                    #region Recorrer la tabla para agregarle los valores, mientras se hace esta inserción.

                                    if (i == 1)//Si en el ciclo, el valor es == 1 se hace esta inserción
                                    {
                                        //Ejecutar Insert de tipo 1.
                                        sqlConsulta = " INSERT INTO InterventorNominaMesTMP (CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + CodNomina_Autoincr + ", " + 1 + ", " + 0 + ", 1) ";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                    else
                                    {
                                        //Ejecutar Insert de tipo 2.
                                        sqlConsulta = " INSERT INTO InterventorNominaMesTMP (CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + CodNomina_Autoincr + ", " + 1 + ", " + 0 + ", 2) ";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }

                                    #endregion

                                    //prTareaAsignarCoordinadorNomina = En FONADE Clásico está comentado, por lo tanto no está implementado aquí.
                                }
                                else
                                {
                                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo crear la nómina en InterventorNominaTMP.')", true);
                                    return;
                                }
                            }

                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        //Según CatalogoInterventorTMP.asp, sólo aplica para el grupo "Interventor" (cerrar ventana).
                        //Cerrar la ventana.
                        ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo crear la nómina.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 16/04/2014.
        /// Adicionar la nómina seleccionada.
        /// Adaptando exactamente el código fuente de CatalogoInterventorTMP.asp.
        /// </summary>
        private void AdicionarNomina(int Valor_AprobadoGerente)
        {
            //Inicializar variables.
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta;
            bool procesado = false;
            int NominaSeleccionada = 0;
            int CodProyectoConvertido = 0;

            try { NominaSeleccionada = Convert.ToInt32(CodCargo.ToString()); }
            catch { return; }

            try { CodProyectoConvertido = Convert.ToInt32(CodProyecto.ToString()); }
            catch { return; }

            try
            {
                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    #region Procesar como Gerente Interventor.

                    if (Valor_AprobadoGerente == 1) // Si está aprobado por el gerente
                    {
                        #region Sí fue aprobado por el gerente, por esto, continúa el flujo.

                        #region Trae los registros de la tabla temporal. (Obtiene los datos "Cargo" = Nombre // Id_Nomina = Id).

                        var cod_cargo_linq = (from t in consultas.Db.InterventorNominaTMPs
                                              where t.CodProyecto == CodProyectoConvertido
                                              && t.Id_Nomina == NominaSeleccionada
                                              select new
                                              {
                                                  t.Id_Nomina,
                                                  t.Cargo,
                                                  t.Tipo
                                              }).FirstOrDefault();

                        #endregion

                        if (cod_cargo_linq.Id_Nomina > 0)
                        {
                            //Generación de datos.
                            string NomCargo_Obtenido = cod_cargo_linq.Cargo;
                            int Id_Nomina_Seleccionado = cod_cargo_linq.Id_Nomina;
                            int Tipo_Obtenido = Convert.ToInt32(cod_cargo_linq.Tipo);

                            #region Insertar los nuevos registros en la tabla definitiva.

                            sqlConsulta = " INSERT INTO InterventorNomina (CodProyecto, Cargo, Tipo) " +
                                          " VALUES (" + CodProyectoConvertido + ", '" + NomCargo_Obtenido + "', " + Tipo_Obtenido + " ) ";

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion

                            #region Borrar el registro ingresado en la tabla temporal.

                            sqlConsulta = " DELETE FROM InterventorNominaTMP " +
                                          " WHERE CodProyecto = " + CodProyectoConvertido + " AND Id_Nomina = " + NominaSeleccionada;

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion

                            #region Traer el código de la actividad para adicionarlo a la tabla definitiva por mes.

                            var cods_int_nomina = (from s in consultas.Db.InterventorNominas
                                                   orderby s.Id_Nomina descending
                                                   select new
                                                   {
                                                       s.Id_Nomina,
                                                       s.Tipo
                                                   }).FirstOrDefault();

                            int cod_Nomina_sql2 = cods_int_nomina.Id_Nomina;
                            string tipo_sql2 = cods_int_nomina.Tipo;

                            #endregion

                            #region Trae los registros de la tabla temporal por meses.

                            var sql_3 = (from v in consultas.Db.InterventorNominaMesTMPs
                                         where v.CodCargo == NominaSeleccionada
                                         select new
                                         {
                                             v.Mes,
                                             v.Valor,
                                             v.Tipo
                                         }).FirstOrDefault();

                            int? Mes_sql_3 = sql_3.Mes;
                            decimal? Valor_sql_3 = sql_3.Valor;
                            int? Tipo_sql_3 = sql_3.Tipo;

                            #endregion

                            #region Inserta los nuevos registros en la tabla definitiva por meses

                            for (int i = 0; i < 14; i++)
                            {
                                if (Tipo_sql_3 == 1) // Si el tipo es 1, genera este flujo.
                                {
                                    #region Consulta que verifica los datos para prevenir valores repetidos cuando es tipo 1.

                                    // CodCargo, Mes, Valor, Tipo
                                    sqlConsulta = " SELECT * FROM InterventorNominaMes " +
                                                  " WHERE Mes = " + Mes_sql_3 + " AND CodCargo = " + Valor_sql_3 +
                                                  " AND Tipo = 1 ";

                                    DataTable tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                                    if (tabla.Rows.Count > 0) //Si existe, lo actualiza
                                    {
                                        sqlConsulta = " UPDATE InterventorNominaMes " +
                                                      " SET Valor = " + Valor_sql_3 +
                                                      " WHERE Mes = " + Mes_sql_3 +
                                                      " AND CodCargo = " + NominaSeleccionada +
                                                      " AND Tipo = 1";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                    if (tabla.Rows.Count == 0) // Si NO existe, lo crea.
                                    {
                                        sqlConsulta = " INSERT INTO InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + NominaSeleccionada + ", " + Mes_sql_3 + ", " + Valor_sql_3 + ", 1)";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region Consulta que verifica los datos para prevenir valores repetidos cuando es tipo 2.

                                    // CodCargo, Mes, Valor, Tipo
                                    sqlConsulta = " SELECT * FROM InterventorNominaMes " +
                                                  " WHERE Mes = " + Mes_sql_3 + " AND CodCargo = " + Valor_sql_3 +
                                                  " AND Tipo = 2 ";

                                    DataTable tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                                    if (tabla.Rows.Count > 0) //Si existe, lo actualiza
                                    {
                                        sqlConsulta = " UPDATE InterventorNominaMes " +
                                                      " SET Valor = " + Valor_sql_3 +
                                                      " WHERE Mes = " + Mes_sql_3 +
                                                      " AND CodCargo = " + NominaSeleccionada +
                                                      " AND Tipo = 2";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                    if (tabla.Rows.Count == 0) // Si NO existe, lo crea.
                                    {
                                        sqlConsulta = " INSERT INTO InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + NominaSeleccionada + ", " + Mes_sql_3 + ", " + Valor_sql_3 + ", 2)";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            #region Borrar el registro de la tabla temporal por meses.

                            sqlConsulta = " DELETE FROM InterventorNominaMesTMP " +
                                          " WHERE CodCargo = " + NominaSeleccionada;

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion
                        }
                        else
                        {
                            //No se obtuvieron datos.
                            return;
                        }

                        #endregion
                    }
                    if (Valor_AprobadoGerente == 0) //No está aprobado.
                    {
                        #region Se procesa la información según el archivo "CatalogoInterventorTMP.asp" de FONADE clasico.

                        //Se devuelve al interventor, se le avisa al coordinador.
                        var return_interventor = (from emp_int in consultas.Db.EmpresaInterventors
                                                  join emp in consultas.Db.Empresas
                                                  on emp_int.CodEmpresa equals emp.id_empresa
                                                  where emp_int.Inactivo.Equals(0) && emp_int.Rol.Equals(Constantes.CONST_RolInterventorLider)
                                                  && emp.codproyecto.Equals(CodProyectoConvertido)
                                                  select new { emp_int.CodContacto }).FirstOrDefault();

                        int? codContacto_sql4 = return_interventor.CodContacto;

                        #region Eliminación #1.

                        sqlConsulta = " DELETE FROM InterventorNominaTMP " +
                                      " WHERE CodProyecto = " + CodProyectoConvertido + " AND Id_Nomina = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Eliminación #2.

                        sqlConsulta = " DELETE FROM InterventorNominaMesTMP " +
                                      " WHERE CodCargo = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Consultar cargo de la siguiente sentencia SQL y generar TareaPendiente.

                        #region Obtener el nombre del proyecto "para ser usado en la creación de la tarea pendiente".

                        var nmb_proyecto_linq = (from t in consultas.Db.Proyectos
                                                 where t.Id_Proyecto.Equals(CodProyectoConvertido)
                                                 select new { t.NomProyecto }).FirstOrDefault();

                        string NmbProyecto = nmb_proyecto_linq.NomProyecto;

                        #endregion

                        #region Consultar cargo de la siguiente sentencia SQL.

                        var codcargo_sql5 = (from t in consultas.Db.InterventorNominaTMPs
                                             where t.CodProyecto == CodProyectoConvertido
                                             && t.Id_Nomina == NominaSeleccionada
                                             select new { t.Cargo }).FirstOrDefault();

                        string cargo_obtenido_sql5 = codcargo_sql5.Cargo;

                        #endregion

                        #region Generar tareas pendientes.

                        TareaUsuario datoNuevo = new TareaUsuario();
                        datoNuevo.CodContacto = (int)codContacto_sql4;
                        datoNuevo.CodProyecto = CodProyectoConvertido;
                        datoNuevo.NomTareaUsuario = "Cargo de Nómina Rechazado por Gerente Interventor";
                        datoNuevo.Descripcion = "Revisar cargo de nómina " + NmbProyecto + " - Actividad --> " + cargo_obtenido_sql5 + "<BR><BR>Observaciones:<BR>" + "OBSERVACIONES DEL INTERVENTOR"; //& fnRequest("ObservaInter")
                        datoNuevo.CodTareaPrograma = 2;
                        datoNuevo.Recurrente = "0"; //"false";
                        datoNuevo.RecordatorioEmail = false;
                        datoNuevo.NivelUrgencia = 1;
                        datoNuevo.RecordatorioPantalla = true;
                        datoNuevo.RequiereRespuesta = false;
                        datoNuevo.CodContactoAgendo = usuario.IdContacto;
                        datoNuevo.DocumentoRelacionado = "";

                        try
                        {
                            Consultas consulta = new Consultas();
                            consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
                        }
                        catch { string msg_err = "Error en generar tareas."; }

                        #endregion

                        #endregion

                        #endregion
                    }
                    else
                    {
                        //Cerrar la ventana.
                        ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                        return;
                    }

                    #endregion>
                }
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    #region Procesar como Interventor.

                    //Consultar si tiene Coordinador asignado.
                    var result = (from t in consultas.Db.Interventors
                                  where t.CodContacto == usuario.IdContacto
                                  select new { t.CodCoordinador }).FirstOrDefault();

                    if (result.CodCoordinador == 0) //No existe
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
                        return;
                    }
                    else
                    {
                        #region Ejecutar sentencias SQL para asignar la tarea al coordinador.

                        #region Obtener el nombre del producto para usarlo en la inserción siguiente.

                        var NomProducto_linq = (from t in consultas.Db.InterventorNominas
                                                where t.CodProyecto == CodProyectoConvertido
                                                && t.Id_Nomina.Equals(NominaSeleccionada)
                                                select new { t.Cargo }).FirstOrDefault();

                        string NombreCargo_producto_Obtenido = NomProducto_linq.Cargo;

                        #endregion

                        #region Inserción.

                        sqlConsulta = " INSERT INTO InterventorNominaTMP (Id_Nomina, CodProyecto, cargo, Tipo, Tarea) " +
                                      " VALUES (" + NominaSeleccionada + ", " + CodProyectoConvertido + ", '" + NombreCargo_producto_Obtenido + "', '" + v_Tipo + "', 'Modificar') ";

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Recorrer los campos de la tabla con 14 meses y crear registros según las condiciones señaladas en código.

                        for (int i = 0; i < 15; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                //Si el valor de la caja de texto es != de vacío y es diferente de 0 hace la inserción.
                                //if ((i && j) && (i!= 0)) //A2
                                if (i == 0)
                                {
                                    if (i == 1)
                                    {
                                        sqlConsulta = " INSERT INTO InterventorNominaMesTMP(CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + NominaSeleccionada + ", " + j + ", '" + j + 1 /*Valor de la caja de texto*/ + ", 1) ";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                    else
                                    {
                                        sqlConsulta = " INSERT INTO InterventorNominaMesTMP(CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + NominaSeleccionada + ", " + j + ", '" + j + 1 /*Valor de la caja de texto*/ + ", 2) ";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                }
                            }
                        }

                        #endregion

                        //prTareaAsignarCoordinadorNomina = No implementado en FONADE clásico, comentado.

                        #endregion
                    }

                    #endregion
                }
            }
            catch (Exception)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo adicionar la nómina seleccionada.')", true);
                return;
            }
        }

        /// <summary>
        /// Actualizar nómina.
        /// Recibe por Session el CodNomina seleccionado, pero se debe validar que si lo tenga, de lo
        /// contrario lo retorna.
        /// 16/04/2014: Se tiene que enviar un valor llamado "AprobadoGerente" (al parecer es sólo 1 = Si // 0 = No).
        /// </summary>
        private void ActualizarNomina(int Valor_AprobadoGerente)
        {
            //Inicializar variables.
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta;
            bool procesado = false;
            int NominaSeleccionada = 0;
            int CodProyectoConvertido = 0;

            try { NominaSeleccionada = Convert.ToInt32(CodCargo.ToString()); }
            catch { return; }

            try { CodProyectoConvertido = Convert.ToInt32(CodProyecto.ToString()); }
            catch { return; }

            try
            {
                ///Comprobar que si el usuario en sesión es un Gerente Interventor, si lo es puede
                ///ejecutar el flujo, de lo contrario no (no devuelve nada).

                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    #region Procesar la actualización como Gerente Interventor.

                    if (Valor_AprobadoGerente == 1) // Si está aprobado por el gerente
                    {
                        #region Sí fue aprobado por el gerente, por esto, continúa el flujo.

                        #region Trae los registros de la tabla temporal. (Obtiene los datos "Cargo" = Nombre // Id_Nomina = Id).

                        var cod_cargo_linq = (from t in consultas.Db.InterventorNominaTMPs
                                              where t.CodProyecto == CodProyectoConvertido
                                              && t.Id_Nomina == NominaSeleccionada
                                              select new
                                              {
                                                  t.Id_Nomina,
                                                  t.Cargo,
                                                  t.Tipo
                                              }).FirstOrDefault();

                        #endregion

                        if (cod_cargo_linq.Id_Nomina > 0)
                        {
                            //Generación de datos.
                            string NomCargo_Obtenido = cod_cargo_linq.Cargo;
                            int Id_Nomina_Seleccionado = cod_cargo_linq.Id_Nomina;
                            int Tipo_Obtenido = Convert.ToInt32(cod_cargo_linq.Tipo);

                            #region Insertar los nuevos registros en la tabla definitiva.

                            sqlConsulta = " INSERT INTO InterventorNomina (CodProyecto, Cargo, Tipo) " +
                                          " VALUES (" + CodProyectoConvertido + ", '" + NomCargo_Obtenido + "', " + Tipo_Obtenido + " ) ";

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion

                            #region Borrar el registro ingresado en la tabla temporal.

                            sqlConsulta = " DELETE FROM InterventorNominaTMP " +
                                          " WHERE CodProyecto = " + CodProyectoConvertido + " AND Id_Nomina = " + NominaSeleccionada;

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion

                            #region Traer el código de la actividad para adicionarlo a la tabla definitiva por mes.

                            var cods_int_nomina = (from s in consultas.Db.InterventorNominas
                                                   orderby s.Id_Nomina descending
                                                   select new
                                                   {
                                                       s.Id_Nomina,
                                                       s.Tipo
                                                   }).FirstOrDefault();

                            int cod_Nomina_sql2 = cods_int_nomina.Id_Nomina;
                            string tipo_sql2 = cods_int_nomina.Tipo;

                            #endregion

                            #region Trae los registros de la tabla temporal por meses.

                            var sql_3 = (from v in consultas.Db.InterventorNominaMesTMPs
                                         where v.CodCargo == NominaSeleccionada
                                         select new
                                         {
                                             v.Mes,
                                             v.Valor,
                                             v.Tipo
                                         }).FirstOrDefault();

                            int? Mes_sql_3 = sql_3.Mes;
                            decimal? Valor_sql_3 = sql_3.Valor;
                            int? Tipo_sql_3 = sql_3.Tipo;

                            #endregion

                            #region Inserta los nuevos registros en la tabla definitiva por meses

                            for (int i = 0; i < 14; i++)
                            {
                                if (Tipo_sql_3 == 1) // Si el tipo es 1, genera este flujo.
                                {
                                    #region Consulta que verifica los datos para prevenir valores repetidos cuando es tipo 1.

                                    // CodCargo, Mes, Valor, Tipo
                                    sqlConsulta = " SELECT * FROM InterventorNominaMes " +
                                                  " WHERE Mes = " + Mes_sql_3 + " AND CodCargo = " + Valor_sql_3 +
                                                  " AND Tipo = 1 ";

                                    DataTable tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                                    if (tabla.Rows.Count > 0) //Si existe, lo actualiza
                                    {
                                        sqlConsulta = " UPDATE InterventorNominaMes " +
                                                      " SET Valor = " + Valor_sql_3 +
                                                      " WHERE Mes = " + Mes_sql_3 +
                                                      " AND CodCargo = " + NominaSeleccionada +
                                                      " AND Tipo = 1";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                    if (tabla.Rows.Count == 0) // Si NO existe, lo crea.
                                    {
                                        sqlConsulta = " INSERT INTO InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + NominaSeleccionada + ", " + Mes_sql_3 + ", " + Valor_sql_3 + ", 1)";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region Consulta que verifica los datos para prevenir valores repetidos cuando es tipo 2.

                                    // CodCargo, Mes, Valor, Tipo
                                    sqlConsulta = " SELECT * FROM InterventorNominaMes " +
                                                  " WHERE Mes = " + Mes_sql_3 + " AND CodCargo = " + Valor_sql_3 +
                                                  " AND Tipo = 2 ";

                                    DataTable tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                                    if (tabla.Rows.Count > 0) //Si existe, lo actualiza
                                    {
                                        sqlConsulta = " UPDATE InterventorNominaMes " +
                                                      " SET Valor = " + Valor_sql_3 +
                                                      " WHERE Mes = " + Mes_sql_3 +
                                                      " AND CodCargo = " + NominaSeleccionada +
                                                      " AND Tipo = 2";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                    if (tabla.Rows.Count == 0) // Si NO existe, lo crea.
                                    {
                                        sqlConsulta = " INSERT INTO InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + NominaSeleccionada + ", " + Mes_sql_3 + ", " + Valor_sql_3 + ", 2)";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            #region Borrar el registro de la tabla temporal por meses.

                            sqlConsulta = " DELETE FROM InterventorNominaMesTMP " +
                                          " WHERE CodCargo = " + NominaSeleccionada;

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion
                        }
                        else
                        {
                            //No se obtuvieron datos.
                            return;
                        }

                        #endregion
                    }
                    if (Valor_AprobadoGerente == 0) //No está aprobado.
                    {
                        #region Se procesa la información según el archivo "CatalogoInterventorTMP.asp" de FONADE clasico.

                        //Se devuelve al interventor, se le avisa al coordinador.
                        var return_interventor = (from emp_int in consultas.Db.EmpresaInterventors
                                                  join emp in consultas.Db.Empresas
                                                  on emp_int.CodEmpresa equals emp.id_empresa
                                                  where emp_int.Inactivo.Equals(0) && emp_int.Rol.Equals(Constantes.CONST_RolInterventorLider)
                                                  && emp.codproyecto.Equals(CodProyectoConvertido)
                                                  select new { emp_int.CodContacto }).FirstOrDefault();

                        int? codContacto_sql4 = return_interventor.CodContacto;

                        #region Eliminación #1.

                        sqlConsulta = " DELETE FROM InterventorNominaTMP " +
                                      " WHERE CodProyecto = " + CodProyectoConvertido + " AND Id_Nomina = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Eliminación #2.

                        sqlConsulta = " DELETE FROM InterventorNominaMesTMP " +
                                      " WHERE CodCargo = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Consultar cargo de la siguiente sentencia SQL y generar TareaPendiente.

                        #region Obtener el nombre del proyecto "para ser usado en la creación de la tarea pendiente".

                        var nmb_proyecto_linq = (from t in consultas.Db.Proyectos
                                                 where t.Id_Proyecto.Equals(CodProyectoConvertido)
                                                 select new { t.NomProyecto }).FirstOrDefault();

                        string NmbProyecto = nmb_proyecto_linq.NomProyecto;

                        #endregion

                        #region Consultar cargo de la siguiente sentencia SQL.

                        var codcargo_sql5 = (from t in consultas.Db.InterventorNominaTMPs
                                             where t.CodProyecto == CodProyectoConvertido
                                             && t.Id_Nomina == NominaSeleccionada
                                             select new { t.Cargo }).FirstOrDefault();

                        string cargo_obtenido_sql5 = codcargo_sql5.Cargo;

                        #endregion

                        #region Generar tareas pendientes.

                        if (cargo_obtenido_sql5.Trim() != "")
                        {
                            TareaUsuario datoNuevo = new TareaUsuario();
                            datoNuevo.CodContacto = (int)codContacto_sql4;
                            datoNuevo.CodProyecto = CodProyectoConvertido;
                            datoNuevo.NomTareaUsuario = "Cargo de Nómina Rechazado por Gerente Interventor";
                            datoNuevo.Descripcion = "Revisar cargo de nómina " + NmbProyecto + " - Actividad --> " + cargo_obtenido_sql5 + "<BR><BR>Observaciones:<BR>" + "OBSERVACIONES DEL INTERVENTOR"; //& fnRequest("ObservaInter")
                            datoNuevo.CodTareaPrograma = 2;
                            datoNuevo.Recurrente = "0"; //"false";
                            datoNuevo.RecordatorioEmail = false;
                            datoNuevo.NivelUrgencia = 1;
                            datoNuevo.RecordatorioPantalla = true;
                            datoNuevo.RequiereRespuesta = false;
                            datoNuevo.CodContactoAgendo = usuario.IdContacto;
                            datoNuevo.DocumentoRelacionado = "";

                            try
                            {
                                Consultas consulta = new Consultas();
                                consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
                            }
                            catch { string msg_err = "Error en generar tareas."; }
                        }

                        #endregion

                        #endregion

                        #endregion
                    }
                    else
                    {
                        //Cerrar la ventana.
                        ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                        return;
                    }

                    #endregion
                }
                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                {
                    #region Procesar la actualización como Coordinador Interventor.

                    if (Valor_AprobadoGerente == 1) //Se usa la misma variable, pero para Coordinador se llama "Aprobado".
                    {
                        #region Update.

                        sqlConsulta = " UPDATE InterventorNominaTMP " +
                                      " SET ChequeoCoordinador = 1 " +
                                      " WHERE CodProyecto = " + CodProyectoConvertido +
                                      " AND Id_Nomina = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Obtener Id_Grupo de la consulta.

                        var IdGrupo_sql1 = (from t in consultas.Db.Grupos
                                            where t.NomGrupo.Equals("Gerente Interventor")
                                            select new { t.Id_Grupo }).FirstOrDefault();

                        int IdGrupo_Obtenido = IdGrupo_sql1.Id_Grupo;

                        #endregion

                        #region Con el Id_Grupo obtenido de la consulta anterior, se consulta el CodContacto de GrupoContacto.

                        //Es una tabla relacional.
                        var CodContacto_Rel_sql2 = (from t in consultas.Db.GrupoContactos
                                                    where t.CodGrupo.Equals(IdGrupo_Obtenido)
                                                    select new { t.CodContacto }).FirstOrDefault();

                        int CodContacto_Obtenido = CodContacto_Rel_sql2.CodContacto;

                        #endregion

                        //Asignar tarea = NO IMPLEMENTADO = código comentado en el clásico. prTareaAsignarGerenteNomina
                    }
                    if (Valor_AprobadoGerente == 0) //No está aprobado.
                    {
                        #region Se procesa la información según el archivo "CatalogoInterventorTMP.asp" de FONADE clasico.

                        //Se devuelve al interventor, se le avisa al coordinador.
                        var return_interventor = (from emp_int in consultas.Db.EmpresaInterventors
                                                  join emp in consultas.Db.Empresas
                                                  on emp_int.CodEmpresa equals emp.id_empresa
                                                  where emp_int.Inactivo.Equals(0) && emp_int.Rol.Equals(Constantes.CONST_RolInterventorLider)
                                                  && emp.codproyecto.Equals(CodProyectoConvertido)
                                                  select new { emp_int.CodContacto }).FirstOrDefault();

                        int? codContacto_sql4 = return_interventor.CodContacto;

                        #region Eliminación #1.

                        sqlConsulta = " DELETE FROM InterventorNominaTMP " +
                                      " WHERE CodProyecto = " + CodProyectoConvertido + " AND Id_Nomina = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Eliminación #2.

                        sqlConsulta = " DELETE FROM InterventorNominaMesTMP " +
                                      " WHERE CodCargo = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Consultar cargo de la siguiente sentencia SQL y generar TareaPendiente.

                        #region Obtener el nombre del proyecto "para ser usado en la creación de la tarea pendiente".

                        var nmb_proyecto_linq = (from t in consultas.Db.Proyectos
                                                 where t.Id_Proyecto.Equals(CodProyectoConvertido)
                                                 select new { t.NomProyecto }).FirstOrDefault();

                        string NmbProyecto = nmb_proyecto_linq.NomProyecto;

                        #endregion

                        #region Consultar cargo de la siguiente sentencia SQL.

                        var codcargo_sql5 = (from t in consultas.Db.InterventorNominaTMPs
                                             where t.CodProyecto == CodProyectoConvertido
                                             && t.Id_Nomina == NominaSeleccionada
                                             select new { t.Cargo }).FirstOrDefault();

                        string cargo_obtenido_sql5 = codcargo_sql5.Cargo;

                        if (cargo_obtenido_sql5.Trim() != "")
                        {
                            #region Generar tareas pendientes.

                            TareaUsuario datoNuevo = new TareaUsuario();
                            datoNuevo.CodContacto = (int)codContacto_sql4;
                            datoNuevo.CodProyecto = CodProyectoConvertido;
                            datoNuevo.NomTareaUsuario = "Cargo de Nómina Rechazado por Coordinador de Interventoria";
                            datoNuevo.Descripcion = "Revisar cargo de nómina " + NmbProyecto + " - Actividad --> " + cargo_obtenido_sql5 + "<BR><BR>Observaciones:<BR>" + "OBSERVACIONES DEL INTERVENTOR"; //& fnRequest("ObservaInter")
                            datoNuevo.CodTareaPrograma = 2;
                            datoNuevo.Recurrente = "0"; //"false";
                            datoNuevo.RecordatorioEmail = false;
                            datoNuevo.NivelUrgencia = 1;
                            datoNuevo.RecordatorioPantalla = true;
                            datoNuevo.RequiereRespuesta = false;
                            datoNuevo.CodContactoAgendo = usuario.IdContacto;
                            datoNuevo.DocumentoRelacionado = "";

                            try
                            {
                                Consultas consulta = new Consultas();
                                consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
                            }
                            catch { string msg_err = "Error en generar tareas."; }

                            #endregion
                        }

                        #endregion

                        #endregion

                        #endregion
                    }
                    else
                    {
                        //Cerrar la ventana.
                        ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                        return;
                    }

                    #endregion
                }
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    #region Procesar la actualización como Interventor.

                    //Consultar si tiene Coordinador asignado.
                    var result = (from t in consultas.Db.Interventors
                                  where t.CodContacto == usuario.IdContacto
                                  select new { t.CodCoordinador }).FirstOrDefault();

                    if (result.CodCoordinador == 0) //No existe
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
                        return;
                    }
                    else
                    {
                        #region Ejecuta sentencias de actualización SQL.

                        #region Obtener el nombre del producto para usarlo en la inserción siguiente.

                        var NomProducto_linq = (from t in consultas.Db.InterventorNominas
                                                where t.CodProyecto == CodProyectoConvertido
                                                && t.Id_Nomina.Equals(NominaSeleccionada)
                                                select new { t.Cargo }).FirstOrDefault();

                        string NombreCargo_producto_Obtenido = NomProducto_linq.Cargo;

                        #endregion

                        #region Inserción.

                        sqlConsulta = " INSERT INTO InterventorNominaTMP (Id_Nomina, CodProyecto, cargo, Tipo, Tarea) " +
                                      " VALUES (" + NominaSeleccionada + ", " + CodProyectoConvertido + ", '" + NombreCargo_producto_Obtenido + "', '" + v_Tipo + "', 'Modificar') ";

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Recorrer los campos de la tabla con 14 meses y crear registros según las condiciones señaladas en código.

                        for (int i = 0; i < 15; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                //Si el valor de la caja de texto es != de vacío y es diferente de 0 hace la inserción.
                                //if ((i && j) && (i!= 0)) //A2
                                if (i == 0)
                                {
                                    if (i == 1)
                                    {
                                        sqlConsulta = " INSERT INTO InterventorNominaMesTMP(CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + NominaSeleccionada + ", " + j + ", '" + j + 1 /*Valor de la caja de texto*/ + ", 1) ";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                    else
                                    {
                                        sqlConsulta = " INSERT INTO InterventorNominaMesTMP(CodCargo, Mes, Valor, Tipo) " +
                                                      " VALUES (" + NominaSeleccionada + ", " + j + ", '" + j + 1 /*Valor de la caja de texto*/ + ", 2) ";

                                        cmd = new SqlCommand(sqlConsulta, connection);
                                        procesado = EjecutarSQL(connection, cmd);
                                    }
                                }
                            }
                        }

                        #endregion

                        //prTareaAsignarCoordinadorNomina = No implementado en FONADE clásico, comentado.

                        #endregion
                    }

                    #endregion
                }
                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor || usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                {
                    //Cerrar la ventana.
                    ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                    return;
                }
                else
                {
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene permisos para realizar esta acción.')", true);
                    return;
                }
            }
            catch (Exception)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo actualizar la nómina seleccionada.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 16/04/2014.
        /// Modificar la nómina seleccionada.
        /// Adaptando exactamente el código fuente de CatalogoInterventorTMP.asp.
        /// </summary>
        /// <param name="Valor_AprobadoGerente">Valor Aprobado, debe ser 1 para ejecutar el proceso, de lo contrario, se cerrará la ventana.</param>
        private void ModificarNomina(int Valor_AprobadoGerente)
        {
            //Inicializar variables.
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta;
            bool procesado = false;
            int NominaSeleccionada = 0;
            int CodProyectoConvertido = 0;

            try
            {
                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    if (Valor_AprobadoGerente == 1) //Sí está aprobado.
                    {
                        #region Procesa la informacion, ya que está aprobada.

                        #region Trae los registros de la tabla temporal. (Obtiene los datos "Cargo" = Nombre // Id_Nomina = Id).

                        var cod_cargo_linq = (from t in consultas.Db.InterventorNominaTMPs
                                              where t.CodProyecto == CodProyectoConvertido
                                              && t.Id_Nomina == NominaSeleccionada
                                              select new
                                              {
                                                  t.Id_Nomina,
                                                  t.Cargo,
                                                  t.Tipo
                                              }).FirstOrDefault();

                        #endregion

                        if (cod_cargo_linq.Id_Nomina > 0)
                        {
                            //Generación de datos.
                            string NomCargo_Obtenido = cod_cargo_linq.Cargo;
                            int Id_Nomina_Seleccionado = cod_cargo_linq.Id_Nomina;
                            int Tipo_Obtenido = Convert.ToInt32(cod_cargo_linq.Tipo);

                            #region Actualiza los registros en la tabla definitiva.

                            sqlConsulta = " UPDATE InterventorNomina " +
                                          " SET CodProyecto = " + CodProyectoConvertido + ", " +
                                          " Tipo = " + Tipo_Obtenido +
                                          " WHERE CodProyecto = " + CodProyectoConvertido +
                                          " AND Id_Nomina = " + NominaSeleccionada;

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion

                            #region Borrar el registro ya actualizado en la tabla temporal.

                            sqlConsulta = " DELETE FROM InterventorNominaTMP " +
                                          " WHERE CodProyecto = " + CodProyectoConvertido;

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion

                            #region Traer el código de la actividad para adicionarlo a la tabla definitiva por mes.

                            var cods_int_nomina = (from s in consultas.Db.InterventorNominas
                                                   orderby s.Id_Nomina descending
                                                   select new
                                                   {
                                                       s.Id_Nomina,
                                                       s.Tipo
                                                   }).FirstOrDefault();

                            int cod_Nomina_sql2 = cods_int_nomina.Id_Nomina;
                            string tipo_sql2 = cods_int_nomina.Tipo;

                            #endregion

                            #region Trae los registros de la tabla temporal por meses.

                            //Consulta SQL. CodCargo, Mes, Valor, Tipo
                            sqlConsulta = " SELECT CodCargo, Mes, Valor, Tipo FROM InterventorNominaMesTMP " +
                                          " WHERE CodCargo = " + NominaSeleccionada;

                            #region Borrar todos los registros de la tabla "InterventorNominaMes" correspondientes al código de la actividad...

                            sqlConsulta = " DELETE FROM InterventorNominaMes " +
                                          " WHERE CodCargo = " + NominaSeleccionada;

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion

                            //Asignar el resultado de la consulta en una tabla para luego recorrerla.
                            DataTable tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                            #region Hace el recorrido de la tabla que contiene los resultados, evaluando si es de tipo 1 ó 2 y hace la inserción.

                            for (int i = 0; i < tabla.Rows.Count; i++)
                            {
                                if (tabla.Rows[i]["Tipo"].ToString() == "1")
                                {
                                    #region Inserción de tipo 1.

                                    sqlConsulta = " INSERT INTO InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                                  " VALUES (" + NominaSeleccionada + ", " + Convert.ToInt32(tabla.Rows[i]["Mes"].ToString()) + ", " + Convert.ToInt32(tabla.Rows[i]["Valor"].ToString()) + ", 1)";

                                    cmd = new SqlCommand(sqlConsulta, connection);
                                    procesado = EjecutarSQL(connection, cmd);

                                    #endregion
                                }
                                else
                                {
                                    #region Inserción de tipo 2.

                                    sqlConsulta = " INSERT INTO InterventorNominaMes (CodCargo, Mes, Valor, Tipo) " +
                                                  " VALUES (" + NominaSeleccionada + ", " + Convert.ToInt32(tabla.Rows[i]["Mes"].ToString()) + ", " + Convert.ToInt32(tabla.Rows[i]["Valor"].ToString()) + ", 2)";

                                    cmd = new SqlCommand(sqlConsulta, connection);
                                    procesado = EjecutarSQL(connection, cmd);

                                    #endregion
                                }
                            }

                            #endregion

                            #region Borrar el registro ya ingresado en la tabla temporal por meses.

                            sqlConsulta = " DELETE FROM InterventorNominaMesTMP " +
                                          " WHERE CodCargo = " + NominaSeleccionada;

                            cmd = new SqlCommand(sqlConsulta, connection);
                            procesado = EjecutarSQL(connection, cmd);

                            #endregion

                            #endregion
                        }
                        else
                        {
                            //No se obtuvieron datos.
                            return;
                        }

                        #endregion
                    }
                    else
                    {
                        //Cerrar la ventana.
                        ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                        return;
                    }
                }
                else
                {
                    //No puede acceder a esta funcionalidad según "CatalogoInterventorTMP.asp".
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene permisos para realizar esta acción.')", true);
                    return;
                }
            }
            catch (Exception)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo modificar la nómina seleccionada.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 16/04/2014.
        /// Eliminar nómina de acuerdo a los permisos que tenga el usuario en sesión, es decir, 
        /// de acuerdo a su grupo = rol.
        /// <param name="Valor_Aprobado">Valor aprobado, debe ser 1 = SI // 0 = NO o saldrá y no hará nada.</param>
        /// </summary>
        private void EliminarNomina(int Valor_Aprobado)
        {
            //Inicializar variables.
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta;
            bool procesado = false;
            int NominaSeleccionada = 0;
            int CodProyectoConvertido = 0;

            try { NominaSeleccionada = Convert.ToInt32(CodCargo.ToString()); }
            catch { return; }

            try { CodProyectoConvertido = Convert.ToInt32(CodProyecto.ToString()); }
            catch { return; }

            try
            {
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    #region Procesar el flujo como Interventor.

                    //Consultar si tiene Coordinador asignado.
                    var result = (from t in consultas.Db.Interventors
                                  where t.CodContacto == usuario.IdContacto
                                  select new { t.CodCoordinador }).FirstOrDefault();

                    if (result.CodCoordinador == 0) //No existe
                    {
                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
                        return;
                    }
                    else
                    {
                        #region Asignar la tarea al coordinador.

                        sqlConsulta = " INSERT INTO InterventorNominaTMP (id_nomina, CodProyecto, Tipo, Tarea) " +
                                      " VALUES (" + NominaSeleccionada + ", " + CodProyectoConvertido + ", '" + v_Tipo + "' , 'Borrar') ";

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Hace una segunda inserción a otra tabla...

                        sqlConsulta = " INSERT INTO InterventorNominaMesTMP (CodCargo) " +
                                      " VALUES ( " + NominaSeleccionada + ") ";

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        //Asignar tarea = NO IMPLEMENTADO = código comentado en el clásico. prTareaAsignarCoordinadorNomina
                    }

                    #endregion
                }
                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    #region Procesar el flujo como Gerente Interventor.

                    if (Valor_Aprobado == 1) // Si está aprobado
                    {
                        #region Borrar los registros de la tabla definitiva.

                        sqlConsulta = " DELETE FROM InterventorNomina " +
                                      " WHERE CodProyecto = " + CodProyectoConvertido + " AND Id_Nomina = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Borrar el registro ya ingresado en la tabla temporal.

                        sqlConsulta = " DELETE FROM InterventorNominaTMP " +
                                      " WHERE CodProyecto = " + CodProyectoConvertido + " AND Id_Nomina = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Borra los registros de la tabla definitiva por meses.

                        sqlConsulta = " DELETE FROM InterventorNominaMes " +
                                      " WHERE CodCargo = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion

                        #region Borra el registro de la tabla temporal por meses.

                        sqlConsulta = " DELETE FROM InterventorNominaMesTMP " +
                                      " WHERE CodCargo = " + NominaSeleccionada;

                        cmd = new SqlCommand(sqlConsulta, connection);
                        procesado = EjecutarSQL(connection, cmd);

                        #endregion
                    }
                    else
                    {
                        //No está aprobado.
                        //Cerrar la ventana.
                        ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                        return;
                    }

                    #endregion
                }
                ///Inquietud: Según el FONADE clásico, valida PRIMERO si el grupo(rol) es Interventor, si es así, ejecuta el código, pero después 
                ///hace la misma validación de que si el grupo(rol) es Interventor, si es así, se cierra la ventana pero le envía otros datos a otra página...
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    //window.opener.parent.parent.location.href=""FrameNominaInter.asp?CodAspecto=2&CodProyecto="&CodProyecto&""";" & vbCrLf & "window.close();
                    //Cerrar la ventana.
                    ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
                    return;
                }
                else
                {
                    //No puede acceder a esta funcionalidad según "CatalogoInterventorTMP.asp".
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene permisos para realizar esta acción.')", true);
                    return;
                }
            }
            catch (Exception)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo eliinar la nómina seleccionada.')", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Ejecutar SQL.
        /// Método que recibe la conexión y la consulta SQL y la ejecuta.
        /// </summary>
        /// <param name="p_connection">Conexión.</param>
        /// <param name="p_cmd">SqlCommand.</param>
        /// <returns>TRUE = Consulta ejecutada correctamente. // FALSE = Error.</returns>
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
            catch
            { return false; }
            finally
            { p_connection.Close(); }
        }

        #endregion

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
                    sqlConsulta = " select * from InterventorNominaMes where CodCargo =  " + CodCargo +
                                  " ORDER BY mes ";
                }

                //Si ha pasado por la variable y tiene consulta.
                if (sqlConsulta != "")
                {
                    //Cargar la información de los meses...
                }
            }
            catch { }
        }
    }
}