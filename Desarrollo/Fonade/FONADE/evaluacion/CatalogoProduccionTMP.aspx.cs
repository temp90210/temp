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
    public partial class CatalogoProduccionTMP : Base_Page
    {
        public String CodProyecto;
        public int txtTab = Constantes.CONST_ProyeccionesVentas;
        public String codConvocatoria;
        string pagina;

        /// <summary>
        /// Código del producto (producción) seleccionado.
        /// </summary>
        public string codProduccion;

        /// <summary>
        /// Usado para determinar si hace la búsqueda en la tabla TMP o normal.
        /// Ej: si este valor es != null, hace la consulta en "InterventorProduccionMesTMP", de lo contrario
        /// consultará en "InterventorProduccionMes".
        /// </summary>
        public string ValorTMP;

        /// <summary>
        /// Valor que es recibido por sesión y contiene los valores "Cargo" ó "Insumo".
        /// </summary>
        public string v_Tipo;

        /// <summary>
        /// Nombre del proyecto.
        /// </summary>
        String txtNomProyecto;

        /// <summary>
        /// Indica que el valor seleccionado viene de "CambiosPO.aspx".
        /// </summary>
        String Detalles_CambiosPO_PO;

        /// <summary>
        /// Variable que contiene las consultas SQL.
        /// </summary>
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

                //Obtener la información almacenada en las variables de sesión.
                CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
                codProduccion = Session["CodProducto"] != null && !string.IsNullOrEmpty(Session["CodProducto"].ToString()) ? Session["CodProducto"].ToString() : "0";
                ValorTMP = Session["ValorTMP"] != null && !string.IsNullOrEmpty(Session["ValorTMP"].ToString()) ? Session["ValorTMP"].ToString() : "0";
                pagina = Session["pagina"] != null && !string.IsNullOrEmpty(Session["pagina"].ToString()) ? Session["pagina"].ToString() : "0";
                v_Tipo = Session["v_Tipo"] != null && !string.IsNullOrEmpty(Session["v_Tipo"].ToString()) ? Session["v_Tipo"].ToString() : "";

                #region Consulta para traer el nombre del proyecto
                txtSQL = "select NomProyecto from Proyecto WHERE id_proyecto = " + CodProyecto;
                var rr = consultas.ObtenerDataTable(txtSQL, "text");
                if (rr.Rows.Count > 0) { txtNomProyecto = rr.Rows[0]["NomProyecto"].ToString(); rr = null; }
                #endregion

                #region Variables de sesión creadas en "CambiosPO.aspx".
                //Sesión inicial que indica que la información a procesar proviene de "CambiosPO.aspx".
                Detalles_CambiosPO_PO = Session["Detalles_CambiosPO_PO"] != null && !string.IsNullOrEmpty(Session["Detalles_CambiosPO_PO"].ToString()) ? Session["Detalles_CambiosPO_PO"].ToString() : "";
                #endregion

                //Revisar si la variable contiene datos "debe ser así para volver visibles ciertos campos".
                if (Detalles_CambiosPO_PO.Trim() != "" && codProduccion != "0")
                {
                    #region CambiosPO.aspx.
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
                    else
                    {
                        L_Item.Visible = true;
                        TB_Item.Enabled = true;
                    }

                    //Llenar el panel.
                    llenarpanel();

                    //Buscar datos.
                    BuscarDatos_Produccion(true);
                    #endregion
                }
                else
                {
                    #region FrameProduccionInter.aspx.

                    //Aplicar para producción.
                    llenarpanel();

                    if (Session["Accion"].ToString().Equals("crear"))
                    {
                        B_Acion.Text = "Crear";
                        lbl_enunciado.Text = "Adicionar";
                        TB_Item.Visible = true;
                        L_Item.Visible = true;
                        TB_Item.Text = "";
                        txt_inv_observaciones.Text = "";
                    }
                    if (Session["Accion"].ToString().Equals("actualizar") || Session["Accion"].ToString().Equals("Editar"))
                    {
                        B_Acion.Text = "Actualizar";
                        lbl_enunciado.Text = "Editar";

                        TB_Item.Text = "null";
                        TB_Item.Visible = false;
                        L_Item.Visible = false;
                        TB_Item.Enabled = false;
                        TB_Item.Text = "A";

                        BuscarDatos_Produccion(false);
                    }
                    if (Session["Accion"].ToString().Equals("borrar"))
                    {
                        B_Acion.Text = "Borrar";
                    }

                    #endregion
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
                    labelFondo.ID = "labelfondo";
                    labelFondo.Text = "Cantidad";
                    labelAportes.ID = "labelaportes";
                    labelAportes.Text = "Costo";
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


                if (i < prorrogaTotal)//15
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
                    TextBox textboxAporte = new TextBox();
                    textboxAporte.ID = "Aporte" + i;
                    textboxAporte.Width = 50;
                    textboxAporte.TextChanged += new EventHandler(TextBox_TextChanged);
                    textboxAporte.AutoPostBack = true;
                    textboxAporte.MaxLength = 10;
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

                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {

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

                    }
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
        }

        /// <summary>
        /// Guardar y/o actualizar la información.
        /// </summary>
        /// <param name="acion">Si el valor es 1, guardará la información, si es 2, actualizará dicha información.</param>
        private void alamcenar(int acion)
        {
            //Inicializar variables.            
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String correcto = "";
            String Valor = "";
            DataTable RsActividad = new DataTable();
            DataTable Rs = new DataTable();
            String NomActividad = "";

            if (CodProyecto != "0" || codProduccion != "0")
            {
                if (acion == 1)
                {
                    #region Guardar la información.

                    #region Si es Interventor.
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        #region Guardar como Interventor.
                        string txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;
                        SqlDataReader reader = ejecutaReader(txtSQL, 1);

                        if (reader != null)
                        {
                            if (reader.Read())
                            {
                                int ActividadTmp = 1;
                                txtSQL = "select id_Produccion from InterventorProduccionTMP ORDER BY id_Produccion DESC";

                                reader = ejecutaReader(txtSQL, 1);

                                if (reader != null)
                                {
                                    if (reader.Read())
                                    {
                                        ActividadTmp = Convert.ToInt32(reader["id_Produccion"].ToString()) + 1;
                                    }
                                }

                                txtSQL = "Insert into InterventorProduccionTMP (id_Produccion,CodProyecto,NomProducto) values (" + ActividadTmp + "," + CodProyecto + ",'" + TB_Item.Text + "')";

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

                                                txtSQL = "INSERT INTO InterventorProduccionMesTMP(CodProducto,Mes,Valor,Tipo) " +
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
                        codProduccion = "0";
                        ValorTMP = "";
                        v_Tipo = "";
                        Session["CodProduccion"] = null;
                        Session["CodNomina"] = null;
                        Session["v_Tipo"] = null;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                        #endregion
                    }
                    #endregion

                    #region Si es Gerente Interventor.
                    if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Si es Gerente Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1") //Si
                            {
                                #region Aprobado.

                                #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL.

                                txtSQL = " select * from InterventorProduccionTMP " +
                                         " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA.

                                txtSQL = " Insert into InterventorProduccion (CodProyecto, NomProducto) " +
                                         " values (" + CodProyecto + ", '" + RsActividad.Rows[0]["NomProducto"].ToString() + "')";

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al insertar registros en la tabla definitiva: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL.

                                txtSQL = " DELETE FROM InterventorProduccionTMP " +
                                         " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

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

                                txtSQL = "select id_Produccion from InterventorProduccion ORDER BY id_Produccion DESC";
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");
                                Valor = RsActividad.Rows[0]["id_Produccion"].ToString();

                                #endregion

                                #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL POR MESES.

                                txtSQL = " select * from InterventorProduccionMesTMP " +
                                         " where CodProducto = " + codProduccion;
                                Rs = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.

                                foreach (DataRow row_Rs in Rs.Rows)
                                {
                                    if (row_Rs["Tipo"].ToString() == "1")
                                    {
                                        #region Ingresar Tipo 1.

                                        txtSQL = " Insert into InterventorProduccionMes (CodProducto,Mes,Valor,Tipo) " +
                                                 " values (" + Valor + "," + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 1) ";

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Ingresar Tipo 2.

                                        txtSQL = " Insert into InterventorProduccionMes (CodProducto,Mes,Valor,Tipo) " +
                                                 " values (" + Valor + "," + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 2) ";

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

                                #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL POR MESES.

                                txtSQL = " DELETE FROM InterventorProduccionMesTMP " +
                                         " where CodProducto = " + codProduccion;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar registros de la tabla temporal por meses: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                codProduccion = "0";
                                ValorTMP = "";
                                v_Tipo = "";
                                Session["CodProduccion"] = null;
                                Session["CodNomina"] = null;
                                Session["v_Tipo"] = null;
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

                                txtSQL = " DELETE FROM InterventorProduccionTMP " +
                                         " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

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

                                txtSQL = " DELETE FROM InterventorProduccionMesTMP " +
                                         " where CodProducto = " + codProduccion;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #2: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region Consultar y cargar resultados a variable "Rs" y "NomActividad".

                                txtSQL = " SELECT NomProducto FROM InterventorProduccionTMP " +
                                         " WHERE CodProyecto = " + CodProyecto + " and id_Produccion= " + codProduccion;
                                Rs = consultas.ObtenerDataTable(txtSQL, "text");

                                if (Rs.Rows.Count > 0) { NomActividad = Rs.Rows[0]["NomProducto"].ToString(); }

                                #endregion

                                #region Generar tarea pendiente.

                                //Agendar tarea.
                                AgendarTarea agenda = new AgendarTarea
                                (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                                "Producto en producción Rechazado por Gerente Interventor",
                                "Revisar productos en producción " + txtNomProyecto + " - Actividad --> " + NomActividad + "<BR><BR>Observaciones:<BR>" + txt_inv_observaciones.Text.Trim(),
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
                                "Catálogo Producción");

                                agenda.Agendar();

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                codProduccion = "0";
                                ValorTMP = "";
                                v_Tipo = "";
                                Session["CodProduccion"] = null;
                                Session["CodNomina"] = null;
                                Session["v_Tipo"] = null;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                        }

                        #endregion
                    }
                    #endregion

                    #endregion
                }
                if (acion == 2)
                {
                    #region Editar la información previamente seleccionada.

                    //Comprobar si el usuario tiene el código grupo de "Coordinador Interventor" ó "Gerente Interventor".
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Si es Gerente Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1") //Si
                            {
                                #region Aprobado.

                                #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL.

                                txtSQL = " select * from InterventorProduccionTMP " +
                                         " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region ACTUALIZA LOS REGISTROS EN LA TABLA DEFINITIVA.

                                txtSQL = " Update InterventorProduccion set CodProyecto = " + CodProyecto +
                                         " WHERE CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar registros en la tabla definitiva: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region BORRAR EL REGISTRO YA ACTUALIZADO DE LA TABLA TEMPORAL.

                                txtSQL = " DELETE FROM InterventorProduccionTMP " +
                                         " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

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

                                txtSQL = "select id_Produccion from InterventorProduccion ORDER BY id_Produccion DESC";
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");
                                Valor = RsActividad.Rows[0]["id_Produccion"].ToString();

                                #endregion

                                #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL POR MESES.

                                txtSQL = " select * from InterventorProduccionMesTMP " +
                                         " where CodProducto = " + codProduccion;
                                Rs = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region BORRAR TODOS LOS REGISTROS DE LA TABLA InterventorProduccionMes CORRESPONDIENTES AL CODIGO DE ACTIVIDAD.

                                txtSQL = " DELETE FROM InterventorProduccionMes " +
                                         " where CodProducto = " + codProduccion;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar registros: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.

                                foreach (DataRow row_Rs in Rs.Rows)
                                {
                                    if (row_Rs["Tipo"].ToString() == "1")
                                    {
                                        txtSQL = " Insert into InterventorProduccionMes (CodProducto,Mes,Valor,Tipo) " +
                                                 " values (" + codProduccion + "," + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + " , 1) ";
                                    }
                                    else
                                    {
                                        txtSQL = " Insert into InterventorProduccionMes (CodProducto,Mes,Valor,Tipo) " +
                                                 " values (" + codProduccion + "," + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + " , 2) ";
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
                                txtSQL = " DELETE InterventorProduccionMesTMP " +
                                         " where CodProducto = " + codProduccion;

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
                                codProduccion = "0";
                                ValorTMP = "";
                                v_Tipo = "";
                                Session["CodProduccion"] = null;
                                Session["CodNomina"] = null;
                                Session["v_Tipo"] = null;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                        }

                        #endregion

                        #region Si es Coordinador Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1")//Si
                            {
                                #region Aprobado.

                                #region Actualización.

                                txtSQL = " UPDATE InterventorProduccionTMP SET ChequeoCoordinador = 1 " +
                                         " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización #1: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region Consulta #1.

                                txtSQL = " select Id_grupo from Grupo where NomGrupo = 'Gerente Interventor' ";
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region Consulta #2.

                                txtSQL = " select CodContacto from GrupoContacto " +
                                         " where CodGrupo = " + RsActividad.Rows[0]["Id_grupo"].ToString();
                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region prTareaAsignarGerenteProduccion: El método está comentado en "DeclaraVariables.inc".

                                //Agendar tarea.
                                AgendarTarea agenda = new AgendarTarea
                                (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                                "Revisión Actividad al Plan Operativo",
                                "Revisión Adición, Modificación o Borrado de Actividad del interventor al Plan Operativo " + txtNomProyecto,
                                CodProyecto,
                                17,
                                "0",
                                true,
                                1,
                                true,
                                false,
                                usuario.IdContacto,
                                "Accion=Editar&CodProyecto=" + CodProyecto + "&CodProducto=" + codProduccion,
                                "",
                                "Asignar Gerente Producción");

                                agenda.Agendar();

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                codProduccion = "0";
                                ValorTMP = "";
                                v_Tipo = "";
                                Session["CodProduccion"] = null;
                                Session["CodNomina"] = null;
                                Session["v_Tipo"] = null;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                            else
                            {
                                #region No Aprobado.

                                #region Consulta.

                                txtSQL = " SELECT EmpresaInterventor.CodContacto " +
                                         " FROM EmpresaInterventor " +
                                         " INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                         " WHERE EmpresaInterventor.Inactivo = 0 " +
                                         " AND EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider +
                                         " AND Empresa.codproyecto = " + CodProyecto;

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region Eliminación #1.

                                txtSQL = " DELETE FROM InterventorProduccionTMP " +
                                         " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

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

                                txtSQL = " DELETE FROM InterventorProduccionMesTMP " +
                                         " where CodProducto = " + codProduccion;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en eliminación #2: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region Consultar y asignar valores a variables "Rs" y "NomActividad".

                                txtSQL = " SELECT NomProducto FROM InterventorProduccionTMP " +
                                         " WHERE CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;
                                Rs = consultas.ObtenerDataTable(txtSQL, "text");

                                if (Rs.Rows.Count > 0) { NomActividad = Rs.Rows[0]["NomProducto"].ToString(); }

                                #endregion

                                #region Generar tarea.

                                //Agendar tarea.
                                AgendarTarea agenda = new AgendarTarea
                                (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                                "Producto en Producción Rechazado por Coordinador de Interventoria",
                                "Revisar producto en producción " + txtNomProyecto + " - Actividad --> " + NomActividad + "<BR><BR>Observaciones:<BR>" + txt_inv_observaciones.Text.Trim(),
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
                                "Catálogo Producción");

                                agenda.Agendar();

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                codProduccion = "0";
                                ValorTMP = "";
                                v_Tipo = "";
                                Session["CodProduccion"] = null;
                                Session["CodNomina"] = null;
                                Session["v_Tipo"] = null;
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

                                if (pagina.Equals("Produccion"))
                                    txtSQL = "select NomProducto from InterventorProduccion where CodProyecto=" + CodProyecto + " and id_Produccion=" + codProduccion;
                                if (pagina.Equals("Ventas"))
                                    txtSQL = "select NomProducto from InterventorVentas where CodProyecto=" + CodProyecto + " and id_ventas=" + codProduccion;
                                if (pagina.Equals("Nomina"))
                                    txtSQL = "select Cargo as NomProducto from InterventorNomina where CodProyecto=" + CodProyecto + " and Id_Nomina=" + codProduccion;

                                reader = ejecutaReader(txtSQL, 1);

                                if (reader != null)
                                {
                                    if (reader.Read())
                                        NomProducto = reader["NomProducto"].ToString();
                                    else
                                        NomProducto = "";

                                    if (pagina.Equals("Produccion"))
                                        txtSQL = "Insert into InterventorProduccionTMP (id_Produccion,CodProyecto,NomProducto,Tarea) values (" + codProduccion + "," + CodProyecto + ",'" + NomProducto + "','Modificar')";
                                    if (pagina.Equals("Ventas"))
                                        txtSQL = "Insert into InterventorVentasTMP (id_Ventas,CodProyecto,NomProducto,Tarea) values (" + codProduccion + "," + CodProyecto + ",'" + NomProducto + "','Modificar')";
                                    if (pagina.Equals("Nomina"))
                                        txtSQL = "Insert into InterventorNominaTMP (Id_Nomina,CodProyecto,cargo,Tipo,Tarea) values (" + codProduccion + "," + CodProyecto + ",'" + NomProducto + "','" + Session["v_Tipo"].ToString() + "','Modificar')";

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


                                                    if (pagina.Equals("Produccion"))
                                                        txtSQL = "INSERT INTO InterventorProduccionMesTMP(CodProducto,Mes,Valor,Tipo) " +
                                                        "VALUES(" + codProduccion + "," + mes + "," + valor + "," + tipo + ")";
                                                    if (pagina.Equals("Ventas"))
                                                        txtSQL = "INSERT INTO InterventorVentasMesTMP(CodProducto,Mes,Valor,Tipo) " +
                                                        "VALUES(" + codProduccion + "," + mes + "," + valor + "," + tipo + ")";
                                                    if (pagina.Equals("Nomina"))
                                                        txtSQL = "INSERT INTO InterventorNominaMesTMP(CodCargo,Mes,Valor,Tipo) " +
                                                        "VALUES(" + codProduccion + "," + mes + "," + valor + "," + tipo + ")";

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
                        codProduccion = "0";
                        ValorTMP = "";
                        v_Tipo = "";
                        Session["CodProduccion"] = null;
                        Session["CodNomina"] = null;
                        Session["v_Tipo"] = null;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                        #endregion
                    }

                    #endregion
                }
                if (acion == 3) //Eliminar
                {
                    #region Si es Gerente Interventor.
                    if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {

                        if (dd_inv_aprobar.SelectedValue == "1") //Si
                        {
                            #region Aprobado.

                            #region BORRA LOS REGISTROS EN LA TABLA DEFINITIVA.

                            txtSQL = " DELETE FROM InterventorProduccion " +
                                     " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar los registros en la tabla definitiva: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL.

                            txtSQL = " DELETE FROM InterventorProduccionTMP " +
                                     " where CodProyecto = " + CodProyecto + " and id_Produccion = " + codProduccion;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar los registros en la tabla definitiva: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region BORRA LOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.

                            txtSQL = " DELETE FROM InterventorProduccionMes " +
                                     " where CodProducto = " + codProduccion;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar los registros en la tabla definitiva por meses: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL POR MESES.

                            txtSQL = " DELETE FROM InterventorProduccionMesTMP " +
                                     " where CodProducto = " + codProduccion;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar los registros en la tabla temporal por meses: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            //Destruir variables.
                            CodProyecto = "0";
                            codProduccion = "0";
                            ValorTMP = "";
                            v_Tipo = "";
                            Session["CodProduccion"] = null;
                            Session["CodNomina"] = null;
                            Session["v_Tipo"] = null;
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
                    codProduccion = "0";
                    ValorTMP = "";
                    v_Tipo = "";
                    Session["CodProduccion"] = null;
                    Session["CodNomina"] = null;
                    Session["v_Tipo"] = null;
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
        /// Cerrar la ventana.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_Cancelar_Click(object sender, EventArgs e)
        {
            //Destruir variables.
            CodProyecto = "0";
            codProduccion = "0";
            ValorTMP = "";
            v_Tipo = "";
            Session["CodProduccion"] = null;
            Session["CodNomina"] = null;
            Session["v_Tipo"] = null;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 09/04/2014.
        /// Modificar la información del valor seleccionado en la grilla de "Producción".
        /// </summary>
        /// <param name="VieneDeCambiosPO">TRUE = Viene de "CambiosPO.aspx". // FALSE = Viene de "FrameProduccionInter.aspx".</param>
        private void BuscarDatos_Produccion(bool VieneDeCambiosPO)
        {
            //Obtiene la conexión
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            //Inicializa la variable para generar la consulta.
            String sqlConsulta = "";

            if (VieneDeCambiosPO == true)
            {
                #region Cargar la información de "CambiosPO.aspx".

                String ChequeoCoordinador;
                String ChequeoGerente;

                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    #region Los campos del formulario se bloquean.

                    TB_Item.Enabled = false;
                    P_Meses.Enabled = false;

                    #endregion

                    sqlConsulta = "SELECT * FROM InterventorProduccionTMP where id_Produccion = " + codProduccion;
                }
                else
                {
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        sqlConsulta = "SELECT * FROM InterventorProduccion where id_Produccion =" + codProduccion;
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
                            //else
                            //{ dd_inv_aprobar.SelectedValue = "0"; }
                            #endregion

                            TB_Item.Text = reader["Item"].ToString();
                        }
                        catch (NullReferenceException) { }
                        catch (Exception) { }
                    }
                    else
                    {
                        //Destruir variables.
                        CodProyecto = "0";
                        codProduccion = "0";
                        ValorTMP = "";
                        v_Tipo = "";
                        Session["CodProduccion"] = null;
                        Session["CodNomina"] = null;
                        Session["v_Tipo"] = null;
                        //Tarea ya aprobada (tendría que salir un botón para cerrar la ventana actual...)...
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Tarea ya aprobada.');window.opener.location.reload();window.close();", true);
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
                    if (Session["Tarea"].ToString() == "Borrar")
                    {
                        sqlConsulta = " select * from InterventorProduccionMes where CodProducto = " + codProduccion;
                    }
                    else
                    {
                        sqlConsulta = "select * from InterventorProduccionMesTMP where CodProducto = " + codProduccion;
                    }
                }
                else
                {
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        sqlConsulta = " select * from InterventorProduccionMes where CodProducto = " + codProduccion + " ORDER BY tipo, mes";
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
            else
            {
                #region Carga la información de "FrameProduccionInter.aspx".

                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    sqlConsulta = "select * from InterventorProduccionMesTMP where CodProducto = " + codProduccion;
                }
                else
                {
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        sqlConsulta = "select * from InterventorProduccionMes where CodProducto = " + codProduccion + " ORDER BY tipo, mes";
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
                        lbl_enunciado.Text = "NUEVO";
                    }
                }
                if (accion == "Modificar")
                {
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        lbl_enunciado.Text = "MODIFICAR";
                    }
                }
                if (accion == "Eliminar")
                {
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        lbl_enunciado.Text = "ELIMINAR";
                    }
                }
            }
            catch { lbl_enunciado.Text = "ADICIONAR ACTIVIDAD"; }
        }

        #endregion
    }
}