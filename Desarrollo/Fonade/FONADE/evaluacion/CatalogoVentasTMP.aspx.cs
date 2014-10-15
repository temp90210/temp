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
    public partial class CatalogoVentasTMP : Base_Page
    {
        #region Variables globales.
        public String CodProyecto;
        public int txtTab = Constantes.CONST_ProyeccionesVentas;
        public String codConvocatoria;
        String CodUsuario;
        String CodGrupo;
        string pagina;

        /// <summary>
        /// Código del producto (producción) seleccionado.
        /// </summary>
        public string codProduccion;

        String txtNomProyecto;

        String Detalles_CambiosPO_VO;

        String txtSQL;
        #endregion

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
                pagina = Session["pagina"] != null && !string.IsNullOrEmpty(Session["pagina"].ToString()) ? Session["pagina"].ToString() : "0";

                #region Consulta para traer el nombre del proyecto
                txtSQL = "select NomProyecto from Proyecto WHERE id_proyecto=" + CodProyecto;
                var rr = consultas.ObtenerDataTable(txtSQL, "text");
                if (rr.Rows.Count > 0) { txtNomProyecto = rr.Rows[0]["NomProyecto"].ToString(); rr = null; }
                #endregion

                #region Variables de sesión creadas en "CambiosPO.aspx".
                //Sesión inicial que indica que la información a procesar proviene de "CambiosPO.aspx".
                Detalles_CambiosPO_VO = Session["Detalles_CambiosPO_VO"] != null && !string.IsNullOrEmpty(Session["Detalles_CambiosPO_VO"].ToString()) ? Session["Detalles_CambiosPO_VO"].ToString() : "";
                #endregion

                if (Detalles_CambiosPO_VO != "")
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

                    //Llenar el panel.
                    llenarpanel();

                    //Buscar datos.
                    BuscarDatos_Ventas(true);
                    #endregion
                }
                else
                {
                    #region Cargar desde "FrameVentasInter.aspx".

                    //Aplicar para producción.
                    llenarpanel();

                    if (Session["Accion"].ToString().Equals("crear"))
                    {
                        B_Acion.Text = "Crear";
                        lbl_enunciado.Text = "Adicionar";
                        if (TB_Item.Text == "") { TB_Item.Text = ""; }
                        TB_Item.Visible = true;
                        L_Item.Visible = true;
                        TB_Item.Enabled = true;
                    }
                    if (Session["Accion"].ToString().Equals("actualizar") || Session["Accion"].ToString().Equals("Editar"))
                    {
                        B_Acion.Text = "Actualizar";
                        lbl_enunciado.Text = "Editar";

                        TB_Item.Text = "null";
                        TB_Item.Visible = false;
                        L_Item.Visible = false;
                        TB_Item.Enabled = false;

                        BuscarDatos_Ventas(false);
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
                    labelFondo.Text = "Ventas";//Cantidad
                    labelAportes.ID = "labelaportes";
                    labelAportes.Text = "Ingreso";//Costo
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
            DataTable RsActividad = new DataTable();
            String Valor = "";
            DataTable Rs = new DataTable();
            DataTable RsAux = new DataTable();
            String correcto = "";
            String NomActividad = "";

            if (CodProyecto != "0" || codProduccion != "0")
            {
                if (acion == 1)
                {
                    #region Guardar la información del producto en venta.

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

                                txtSQL = "select id_Ventas as id_Produccion from InterventorVentasTMP ORDER BY id_Ventas DESC";

                                reader = ejecutaReader(txtSQL, 1);

                                if (reader != null)
                                {
                                    if (reader.Read())
                                    {
                                        ActividadTmp = Convert.ToInt32(reader["id_Produccion"].ToString()) + 1;
                                    }
                                }

                                txtSQL = "Insert into InterventorVentasTMP (id_Ventas,CodProyecto,NomProducto) values (" + ActividadTmp + "," + CodProyecto + ",'" + TB_Item.Text + "')";

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

                                                txtSQL = "INSERT INTO InterventorVentasMesTMP(CodProducto,Mes,Valor,Tipo) " +
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
                        Session["CodProducto"] = null;
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

                            txtSQL = " select * from InterventorVentasTMP " +
                                     " where CodProyecto = " + CodProyecto + " and id_Ventas = " + codProduccion;
                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                            #endregion

                            #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA.

                            txtSQL = " Insert into InterventorVentas (CodProyecto, NomProducto) " +
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

                            txtSQL = " DELETE FROM InterventorVentasTMP " +
                                     " where CodProyecto = " + CodProyecto + " and id_Ventas = " + codProduccion;

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

                            txtSQL = " select id_Ventas from InterventorVentas ORDER BY id_Ventas DESC ";
                            RsActividad = consultas.ObtenerDataTable(txtSQL, "text");
                            Valor = RsActividad.Rows[0]["id_Ventas"].ToString();

                            #endregion

                            #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL POR MESES.

                            txtSQL = " select * from InterventorVentasMesTMP " +
                                     " where CodProducto = " + codProduccion;

                            //Asignar resultados de la consulta.
                            Rs = consultas.ObtenerDataTable(txtSQL, "text");

                            #endregion

                            #region INSERTA LOS NUEVOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.

                            foreach (DataRow row_Rs in Rs.Rows)
                            {
                                if (row_Rs["Tipo"].ToString() == "1")
                                {
                                    #region Tipo 1.
                                    //Modificacion para prevenir dobles registros - 12/10/2011
                                    txtSQL = " select * from InterventorVentasMes " +
                                             " where Mes = " + row_Rs["Mes"].ToString() +
                                             " and CodProducto = " + Valor + " and Tipo = 1 ";

                                    RsAux = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (RsAux.Rows.Count > 0)
                                    {
                                        #region Actualizar.

                                        txtSQL = " update InterventorVentasMes set Valor = " + row_Rs["Valor"].ToString() +
                                                 " where Mes = " + row_Rs["Mes"].ToString() +
                                                 " and CodProducto = " + Valor + " and Tipo = 1 ";

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Ingresar.

                                        txtSQL = " Insert into InterventorVentasMes (CodProducto, Mes, Valor, Tipo) " +
                                                 " values (" + Valor + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 1) ";

                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Tipo 2.

                                    //Modificacion para prevenir dobles registros - 12/10/2011
                                    txtSQL = " select * from InterventorVentasMes " +
                                             " where Mes = " + row_Rs["Mes"].ToString() +
                                             " and CodProducto = " + Valor + " and Tipo = 2 ";

                                    RsAux = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (RsAux.Rows.Count > 0)
                                    {
                                        #region Actualizar.

                                        txtSQL = " update InterventorVentasMes set Valor = " + row_Rs["Valor"].ToString() +
                                                 " where Mes = " + row_Rs["Mes"].ToString() +
                                                 " and CodProducto = " + Valor + " and Tipo = 2 ";

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Ingresar.

                                        txtSQL = " Insert into InterventorVentasMes (CodProducto, Mes, Valor, Tipo) " +
                                                 " values (" + Valor + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 2) ";

                                        #endregion
                                    }

                                    #endregion
                                }
                            }

                            #endregion

                            //Destruir variables.
                            CodProyecto = "0";
                            codProduccion = "0";
                            Session["CodProducto"] = null;
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

                            txtSQL = " DELETE FROM InterventorVentasTMP " +
                                     " where CodProyecto = " + CodProyecto + " and id_Ventas = " + codProduccion;

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

                            txtSQL = " DELETE FROM InterventorVentasMesTMP " +
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

                            #region Consultar y asignar resultados a "Rs" y "NomActividad".

                            txtSQL = " SELECT NomProducto FROM InterventorVentasTMP " +
                                     " WHERE CodProyecto = " + CodProyecto +
                                     " and id_Ventas = " + codProduccion;

                            Rs = consultas.ObtenerDataTable(txtSQL, "text");

                            if (Rs.Rows.Count > 0) { NomActividad = Rs.Rows[0]["NomProducto"].ToString(); }

                            #endregion

                            #region Generar tarea pendiente.

                            //Agendar tarea.
                            AgendarTarea agenda = new AgendarTarea
                            (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                            "Producto en ventas Rechazado por Gerente Interventor",
                            "Revisar productos en ventas " + txtNomProyecto + " - Actividad --> " + NomActividad + "<BR><BR>Observaciones:<BR>" + txt_inv_observaciones.Text.Trim(),
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
                            "Catálogo Ventas");

                            agenda.Agendar();

                            #endregion

                            //Destruir variables.
                            CodProyecto = "0";
                            codProduccion = "0";
                            Session["CodProducto"] = null;
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                            #endregion
                        }
                    }

                    #endregion

                    #endregion
                }
                if (acion == 2)
                {
                    #region Editar el producto en venta seleccionado.

                    //Comprobar si el usuario tiene el código grupo de "Coordinador Interventor" ó "Gerente Interventor".
                    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                    {
                        #region Si es Gerente Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1")//Si
                            {
                                #region Aprobado.

                                #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL.

                                txtSQL = " select * from InterventorVentasTMP " +
                                         " where CodProyecto = " + CodProyecto +
                                         " and id_Ventas = " + codProduccion;

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region ACTUALIZA LOS REGISTROS EN LA TABLA DEFINITIVA.

                                txtSQL = " Update InterventorVentas set CodProyecto = " + CodProyecto +
                                         " WHERE CodProyecto = " + CodProyecto +
                                         " and id_Ventas = " + codProduccion;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al actualizar registros de la tabla definitiva: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                #region BORRAR EL REGISTRO YA ACTUALIZADO DE LA TABLA TEMPORAL.

                                txtSQL = " DELETE FROM InterventorVentasTMP " +
                                         " where CodProyecto = " + CodProyecto + " and id_Ventas = " + codProduccion;

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

                                txtSQL = " select id_Ventas from InterventorVentas " +
                                         " WHERE CodProyecto = " + CodProyecto +
                                         " and id_Ventas = " + codProduccion;

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                Valor = RsActividad.Rows[0]["id_Ventas"].ToString();

                                #endregion

                                #region TRAE LOS REGISTROS DE LA TABLA TEMPORAL POR MESES.

                                txtSQL = " select * from InterventorVentasMesTMP " +
                                         " where CodProducto = " + codProduccion;

                                Rs = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region BORRAR TODOS LOS REGISTROS DE LA TABLA InterventorVentasMes CORRESPONDIENTES AL CODIGO DE ACTIVIDAD.

                                txtSQL = " DELETE FROM InterventorVentasMes " +
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
                                        #region Tipo 1.

                                        txtSQL = " Insert into InterventorVentasMes (CodProducto,Mes,Valor,Tipo) " +
                                                 " values (" + codProduccion + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 1) ";

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Tipo 2.

                                        txtSQL = " Insert into InterventorVentasMes (CodProducto,Mes,Valor,Tipo) " +
                                                 " values (" + codProduccion + ", " + row_Rs["Mes"].ToString() + ", " + row_Rs["Valor"].ToString() + ", 2) ";

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

                                txtSQL = " DELETE FROM InterventorVentasMesTMP " +
                                         " where CodProducto = " + codProduccion;

                                //Ejecutar consulta.
                                cmd = new SqlCommand(txtSQL, conn);
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar registros de la tabla tempora por mes: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                    return;
                                }

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                codProduccion = "0";
                                Session["CodProducto"] = null;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                        }

                        #endregion

                        #region Si es Coordinador Interventor.

                        if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                        {
                            if (dd_inv_aprobar.SelectedValue == "1") //Si
                            {
                                #region Aprobado.

                                #region Actualización.
                                txtSQL = " UPDATE InterventorVentasTMP SET ChequeoCoordinador = 1 " +
                                                             " where CodProyecto = " + CodProyecto + " and id_Ventas = " + codProduccion;

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

                                txtSQL = " select Id_grupo from Grupo " +
                                         " where NomGrupo = 'Gerente Interventor' ";

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region Consulta #2.

                                txtSQL = " select CodContacto from GrupoContacto " +
                                         " where CodGrupo = " + RsActividad.Rows[0]["Id_grupo"].ToString();

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region prTareaAsignarGerenteVentas: Está comentado en "DeclaraVariables.inc".

                                //Agendar tarea.
                                AgendarTarea agenda = new AgendarTarea
                                (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                                "Revisión Actividad al Plan Operativo",
                                "Revisión Adición, Modificación o Borrado de Actividad del interventor al Plan Operativo " + NomActividad,
                                CodProyecto,
                                19,
                                "0",
                                true,
                                1,
                                true,
                                false,
                                usuario.IdContacto,
                                "Accion=Editar&CodProyecto=" + CodProyecto + "&CodProducto=" + codProduccion,
                                "",
                                "Asignar Gerente Ventas");

                                agenda.Agendar();

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                codProduccion = "0";
                                Session["CodProducto"] = null;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);

                                #endregion
                            }
                            else
                            {
                                #region No Aprobado.

                                #region Consulta #1.

                                txtSQL = " SELECT EmpresaInterventor.CodContacto " +
                                         " FROM EmpresaInterventor " +
                                         " INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                         " WHERE EmpresaInterventor.Inactivo = 0 " +
                                         " AND EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider +
                                         " AND Empresa.codproyecto = " + CodProyecto;

                                RsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                                #endregion

                                #region Eliminación #1.

                                txtSQL = " DELETE FROM InterventorVentasTMP " +
                                         " where CodProyecto = " + CodProyecto +
                                         " and id_Ventas = " + codProduccion;

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

                                txtSQL = " DELETE FROM InterventorVentasMesTMP " +
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

                                #region Consultar y asignar resultados a "Rs" y "NomActividad".

                                txtSQL = " SELECT NomProducto FROM InterventorVentasTMP " +
                                         " WHERE CodProyecto = " + CodProyecto +
                                         " and id_Ventas = " + codProduccion;
                                Rs = consultas.ObtenerDataTable(txtSQL, "text");

                                if (Rs.Rows.Count > 0) { NomActividad = Rs.Rows[0]["NomProducto"].ToString(); }

                                #endregion

                                #region Generar tarea pendiente.

                                //Agendar tarea.
                                AgendarTarea agenda = new AgendarTarea
                                (Int32.Parse(RsActividad.Rows[0]["CodContacto"].ToString()),
                                "Producto en Ventas Rechazado por Coordinador de Interventoria",
                                "Revisar producto en ventas " + txtNomProyecto + " - Actividad --> " + NomActividad + "<BR><BR>Observaciones:<BR>" + txt_inv_observaciones.Text.Trim(),
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
                                "Catálogo Ventas");

                                agenda.Agendar();

                                #endregion

                                //Destruir variables.
                                CodProyecto = "0";
                                codProduccion = "0";
                                Session["CodProducto"] = null;
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

                                txtSQL = "select NomProducto from InterventorVentas where CodProyecto=" + CodProyecto + " and id_ventas=" + codProduccion;

                                reader = ejecutaReader(txtSQL, 1);

                                if (reader != null)
                                {
                                    if (reader.Read())
                                        NomProducto = reader["NomProducto"].ToString();
                                    else
                                        NomProducto = "";

                                    txtSQL = "Insert into InterventorVentasTMP (id_Ventas,CodProyecto,NomProducto,Tarea) values (" + codProduccion + "," + CodProyecto + ",'" + NomProducto + "','Modificar')";

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

                                                    txtSQL = "INSERT INTO InterventorVentasMesTMP(CodProducto,Mes,Valor,Tipo) " +
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
                        Session["CodProducto"] = null;
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
                            #region Aprobado

                            #region BORRA LOS REGISTROS EN LA TABLA DEFINITIVA.

                            txtSQL = " DELETE FROM InterventorVentas " +
                                     " where CodProyecto = " + CodProyecto + " and id_Ventas = " + codProduccion;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar registros de la tabla definitiva: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL.

                            txtSQL = " DELETE FROM InterventorVentasTMP " +
                                     " where CodProyecto = " + CodProyecto + " and id_Ventas = " + codProduccion;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar registros de la tabla temporal: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region BORRA LOS REGISTROS EN LA TABLA DEFINITIVA POR MESES.

                            txtSQL = " DELETE FROM InterventorVentasMes " +
                                     " where CodProducto = " + codProduccion;

                            //Ejecutar consulta.
                            cmd = new SqlCommand(txtSQL, conn);
                            correcto = String_EjecutarSQL(conn, cmd);

                            if (correcto != "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al eliminar registros de la tabla definitiva por mes: " + correcto + " \n LA CONSULTA ESTA ASÍ: " + txtSQL + "');", true);
                                return;
                            }

                            #endregion

                            #region BORRAR EL REGISTRO YA INSERTADO DE LA TABLA TEMPORAL POR MESES.

                            txtSQL = " DELETE FROM InterventorVentasMesTMP " +
                                     " where CodProducto = " + codProduccion;

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
                            codProduccion = "0";
                            Session["CodProducto"] = null;
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
                    Session["CodProducto"] = null;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
                }
            }
            else
            {
                return;
            }

            #region Comentarios anteriores al 23/04/2014.
            ////Inicializar variables.            
            //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            //SqlCommand cmd = new SqlCommand();

            //if (acion == 1)
            //{
            //    #region Guardar la información de Ventas. = SIN CONSULTAS.



            //    #endregion
            //}
            //if (acion == 2)
            //{
            //    #region Editar el Producto en Ventas seleccionado CÓDIGO ANTERIOR COMENTADO.

            //    ////Comprobar si el usuario tiene el código grupo de "Coordinador Interventor" ó "Gerente Interventor".
            //    //if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
            //    //{
            //    //    //Consulta los valores temporales.
            //    //    cmd = new SqlCommand("SELECT * FROM [Fonade].[dbo].[InterventorVentasTMP] WHERE [id_Produccion] = " + codProduccion, conn);

            //    //    //Consulta los valores reales.
            //    //    //cmd = new SqlCommand("SELECT * FROM [Fonade].[dbo].[InterventorVentas] WHERE [id_Produccion] = " + codProduccion, conn);

            //    //    //Consulta los "costos".
            //    //    //cmd = new SqlCommand("SELECT * FROM [Fonade].[dbo].[InterventorVentasMesTMP] WHERE [id_Produccion] = " + codProduccion + " ORDER BY [Mes] ", conn);

            //    //}
            //    //if (usuario.CodGrupo == Constantes.CONST_Interventor) //Si el usuario tiene el código grupo "Interventor".
            //    //{
            //    //    //Consulta los valores reales.
            //    //    cmd = new SqlCommand("SELECT * FROM [Fonade].[dbo].[InterventorVentas] WHERE [id_Produccion] = " + codProduccion, conn);

            //    //    //Consulta los "costos".
            //    //    //cmd = new SqlCommand("SELECT * FROM [Fonade].[dbo].[InterventorVentasMes] WHERE [id_Produccion] = " + codProduccion + " ORDER BY [Mes] ", conn);
            //    //}

            //    #endregion

            //    //Actualizar Producto seleccionado.
            //    ActualizarProductoEnVenta(s_valorAprobado);
            //}
            //if (acion == 3)
            //{
            //    //Modificar producto en venta.
            //    ModificarProductoEnVenta(s_valorAprobado);
            //}
            //else //No es un dato válido.
            //{ return; }

            //#region Código anterior comentado. NO BORRAR.

            ////String item = TB_Item.Text;

            ////String idGuardar = "";
            ////SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            ////if (usuario.CodGrupo == Constantes.CONST_Interventor)
            ////{
            ////    SqlCommand cmd = new SqlCommand("SELECT [CodCoordinador] FROM [Fonade].[dbo].[Interventor] WHERE [CodContacto] = " + CodUsuario, conn);

            ////    try
            ////    {
            ////        conn.Open();
            ////        SqlDataReader reader = cmd.ExecuteReader();
            ////        reader.Read();
            ////        String codCordinador = reader["CodCoordinador"].ToString();
            ////        reader.Close();
            ////        conn.Close();

            ////        if (String.IsNullOrEmpty(codCordinador))
            ////        {
            ////            System.Windows.Forms.MessageBox.Show("No tiene ningún coordinador asignado.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            ////            return;
            ////        }
            ////        else
            ////        {
            ////            conn.Open();
            ////            cmd = new SqlCommand("SELECT MAX([Id_Actividad]) Id_Actividad FROM [Fonade].[dbo].[ProyectoActividadPOInterventorTMP]", conn);
            ////            reader = cmd.ExecuteReader();
            ////            reader.Read();
            ////            if (reader.FieldCount != 0)
            ////            {
            ////                if (String.IsNullOrEmpty(reader["Id_Actividad"].ToString())) idGuardar = "" + 1;
            ////                else idGuardar = "" + (Int64.Parse(reader["Id_Actividad"].ToString()) + 1 + Int64.Parse(CodProyecto));
            ////            }

            ////        }
            ////    }
            ////    catch (SqlException se)
            ////    {
            ////        throw se;
            ////    }
            ////    finally
            ////    {
            ////        conn.Close();
            ////    }
            ////}

            ////string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            ////using (var con = new SqlConnection(conexionStr))
            ////{
            ////    using (var com = con.CreateCommand())
            ////    {
            ////        com.CommandText = "MD_Insertar_Actualizar_ProyectoActividadPO";
            ////        com.CommandType = System.Data.CommandType.StoredProcedure;

            ////        if (usuario.CodGrupo == Constantes.CONST_Interventor)
            ////        {
            ////            if (acion == 1) com.Parameters.AddWithValue("@_Id_Actividad", 0); //idGuardar);
            ////            if (acion == 2) com.Parameters.AddWithValue("@_Id_Actividad", CodActividad);
            ////        }
            ////        else
            ////        {
            ////            if (acion == 1) com.Parameters.AddWithValue("@_Id_Actividad", 0);
            ////            if (acion == 2) com.Parameters.AddWithValue("@_Id_Actividad", CodActividad);
            ////        }

            ////        //com.Parameters.AddWithValue("@_NomActividad", actividad);
            ////        com.Parameters.AddWithValue("@_CodProyecto", CodProyecto);
            ////        com.Parameters.AddWithValue("@_CodGrupo", usuario.CodGrupo);

            ////        if (acion == 1)
            ////        {
            ////            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            ////            {
            ////                com.Parameters.AddWithValue("@_caso", "CREATEINTERVENTOR");
            ////            }
            ////            else
            ////            {
            ////                com.Parameters.AddWithValue("@_caso", "CREATE");
            ////            }
            ////        }
            ////        if (acion == 2)
            ////        {
            ////            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            ////            {
            ////                com.Parameters.AddWithValue("@_caso", "UPDATEINTERVENTOR");
            ////            }
            ////            else
            ////            {
            ////                com.Parameters.AddWithValue("@_caso", "UPDATE");
            ////            }
            ////        }
            ////        try
            ////        {
            ////            con.Open();
            ////            com.ExecuteReader();
            ////        }
            ////        catch (Exception ex)
            ////        {
            ////            throw ex;
            ////        }
            ////        finally
            ////        {
            ////            con.Close();
            ////        }
            ////    }
            ////}

            ////String RsActividad = "";
            //////textbox
            ////if (usuario.CodGrupo == Constantes.CONST_Interventor)
            ////{
            ////    RsActividad = idGuardar;
            ////}
            ////else
            ////{
            ////    //SqlCommand cmd = new SqlCommand("SELECT [Id_Actividad] FROM [Fonade].[dbo].[ProyectoActividadPO] WHERE [NomActividad] = " + actividad + " AND [CodProyecto] = " + CodProyecto, conn);
            ////    SqlCommand cmd = new SqlCommand("SELECT [Id_Actividad] FROM [Fonade].[dbo].[ProyectoActividadPO] WHERE [CodProyecto] = " + CodProyecto + " ");

            ////    try
            ////    {
            ////        conn.Open();
            ////        SqlDataReader reader = cmd.ExecuteReader();
            ////        reader.Read();
            ////        RsActividad = reader["CodCoordinador"].ToString();
            ////        reader.Close();
            ////    }
            ////    catch (SqlException se)
            ////    {
            ////        throw se;
            ////    }
            ////    finally
            ////    {
            ////        conn.Close();
            ////    }
            ////}

            //#endregion

            //#region MD_Insertar_Actualizar_ProyectoActividadPOMes COMENTADO.

            ////using (var con = new SqlConnection(conexionStr))
            ////{
            ////    using (var com = con.CreateCommand())
            ////    {
            ////        com.CommandText = "MD_Insertar_Actualizar_ProyectoActividadPOMes";
            ////        com.CommandType = System.Data.CommandType.StoredProcedure;

            ////        for (int j = 1; j <= 2; j++)
            ////        {
            ////            for (int i = 1; i <= 12; i++)
            ////            {
            ////                Label controltext;
            ////                controltext = (Label)this.FindControl("TotalMes" + i);


            ////                if (acion == 1) com.Parameters.AddWithValue("@CodActividad", RsActividad);
            ////                if (acion == 2) com.Parameters.AddWithValue("@CodActividad", CodActividad);

            ////                com.Parameters.AddWithValue("@Mes", i);
            ////                com.Parameters.AddWithValue("@CodTipoFinanciacion", j);
            ////                com.Parameters.AddWithValue("@Valor", controltext.Text);

            ////                if (acion == 1)
            ////                {
            ////                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
            ////                    {
            ////                        com.Parameters.AddWithValue("@_caso", "CREATEINTERVENTOR");
            ////                    }
            ////                    else
            ////                    {
            ////                        com.Parameters.AddWithValue("@_caso", "CREATE");
            ////                    }
            ////                }
            ////                if (acion == 2)
            ////                {
            ////                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
            ////                    {
            ////                        com.Parameters.AddWithValue("@_caso", "UPDATEINTERVENTOR");
            ////                    }
            ////                    else
            ////                    {
            ////                        com.Parameters.AddWithValue("@_caso", "UPDATE");
            ////                    }
            ////                }

            ////                try
            ////                {
            ////                    con.Open();
            ////                    com.ExecuteReader();
            ////                }
            ////                catch (Exception ex)
            ////                {
            ////                    throw ex;
            ////                }
            ////                finally
            ////                {
            ////                    con.Close();
            ////                }
            ////            }
            ////        }
            ////    }
            ////}

            //#endregion 
            #endregion
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
            Session["CodProducto"] = null;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 09/04/2014.
        /// Modificar la información del valor seleccionado en la grilla de "Producción".
        /// </summary>
        /// <param name="VieneDeCambiosPO">TRUE = Viene de "CambiosPO.aspx". // FALSE = Viene de "FrameVentasInter.aspx".</param>
        private void BuscarDatos_Ventas(bool VieneDeCambiosPO)
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

                    sqlConsulta = "SELECT * FROM InterventorVentasTMP where id_Ventas = " + codProduccion;
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
                            else
                            { dd_inv_aprobar.SelectedValue = "0"; }
                            #endregion

                        }
                        catch (NullReferenceException) { }
                        catch (Exception) { }

                        TB_Item.Text = reader["Item"].ToString();
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
                        string valor_Obtenido = reader["Tipo"].ToString();//.Equals("1"); //CodTipoFinanciacion

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
                #region Viene de "FrameVentasInter.aspx".

                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    if (pagina.Equals("Produccion"))
                        sqlConsulta = "select * from InterventorProduccionMesTMP where CodProducto = " + codProduccion;
                    if (pagina.Equals("Ventas"))
                        sqlConsulta = "select * from InterventorVentasMesTMP where CodProducto = " + codProduccion;
                    if (pagina.Equals("Nomina"))
                        sqlConsulta = "select * from InterventorNominaMesTMP where CodCargo = " + codProduccion + " order by mes";
                }
                else
                {
                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        if (pagina.Equals("Produccion"))
                            sqlConsulta = "select * from InterventorProduccionMes where CodProducto = " + codProduccion + " ORDER BY tipo, mes";
                        if (pagina.Equals("Ventas"))
                            sqlConsulta = "select * from InterventorVentasMes where CodProducto = " + codProduccion + " ORDER BY tipo, mes";
                        if (pagina.Equals("Nomina"))
                            sqlConsulta = "select * from InterventorNominaMes where CodCargo = " + codProduccion + " order by mes";
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

        #region Comentarios.
        //private void sumar(TextBox textbox)
        //{
        //    //Inicializar variables.
        //    String textboxID = textbox.ID;
        //    TextBox textFondo;
        //    TextBox textAporte;
        //    Label controltext;

        //    //Se movieron las variables del try para la suma.
        //    Double suma1 = 0;
        //    Double suma2 = 0; //Según el FONADE clásico, el valor COSTO lo toma como ENTERO al SUMARLO.
        //    Int32 valor_suma2 = 0;

        //    var labelfondocosto = this.FindControl("labelfondocosto") as Label;
        //    var labelaportescosto = this.FindControl("labelaportescosto") as Label;
        //    var L_SumaTotalescosto = this.FindControl("L_SumaTotalescosto") as Label;

        //    //Details
        //    int limit = 0;
        //    if (textboxID.Length == 7)
        //        limit = 1;
        //    else
        //        limit = 2;

        //    //String objeto = "TotalMes" + (textboxID[textboxID.Length - 1]);
        //    String objeto = "TotalMes" + textboxID.Substring(6, limit);


        //    controltext = (Label)this.FindControl(objeto);
        //    textFondo = (TextBox)this.FindControl("Fondoo" + textboxID.Substring(6, limit)); //Sueldo
        //    textAporte = (TextBox)this.FindControl("Aporte" + textboxID.Substring(6, limit)); //Prestaciones

        //    if (textAporte.Text.Trim() == "")
        //    { suma2 = 0; textAporte.Text = suma2.ToString(); }
        //    else { suma2 = Double.Parse(textAporte.Text); valor_suma2 = Convert.ToInt32(suma2); textAporte.Text = valor_suma2.ToString(); }

        //    #region Comentarios anteriores NO BORRAR.

        //    //if (textboxID.Length == 7)
        //    //{
        //    //    objeto = "TotalMes" + (textboxID[textboxID.Length - 2]) + (textboxID[textboxID.Length - 1]);

        //    //    controltext = (Label)this.FindControl(objeto);
        //    //    string a = controltext.ID;

        //    //    textFondo = (TextBox)this.FindControl("Sueldo" + (textboxID[textboxID.Length - 2]) + (textboxID[textboxID.Length - 1]));
        //    //    textAporte = (TextBox)this.FindControl("Prestaciones" + (textboxID[textboxID.Length - 2]) + (textboxID[textboxID.Length - 1]));

        //    //    if (textAporte.Text.Trim() == "")
        //    //    { suma2 = 0; textAporte.Text = suma2.ToString(); }
        //    //    else { suma2 = Double.Parse(textAporte.Text); valor_suma2 = Convert.ToInt32(suma2); textAporte.Text = valor_suma2.ToString(); }
        //    //}
        //    //else
        //    //{
        //    //    controltext = (Label)this.FindControl(objeto);
        //    //    string a = controltext.ID;

        //    //    textFondo = (TextBox)this.FindControl("Sueldo" + (textboxID[textboxID.Length - 1]));
        //    //    textAporte = (TextBox)this.FindControl("Prestaciones" + (textboxID[textboxID.Length - 1]));

        //    //    if (textAporte.Text.Trim() == "")
        //    //    { suma2 = 0; textAporte.Text = suma2.ToString(); }
        //    //    else { suma2 = Double.Parse(textAporte.Text); valor_suma2 = Convert.ToInt32(suma2); textAporte.Text = valor_suma2.ToString(); }
        //    //} 

        //    #endregion


        //    try
        //    {
        //        if (String.IsNullOrEmpty(textFondo.Text))
        //        { suma1 = 0; textFondo.Text = suma1.ToString(); }
        //        else
        //        { suma1 = Double.Parse(textFondo.Text); textFondo.Text = suma1.ToString(); }

        //        if (String.IsNullOrEmpty(textAporte.Text))
        //        { suma2 = 0; textAporte.Text = suma2.ToString(); }
        //        else { suma2 = Double.Parse(textAporte.Text); valor_suma2 = Convert.ToInt32(suma2); textAporte.Text = valor_suma2.ToString(); }

        //        //Con formato
        //        //controltext.Text = "$" + (suma1 + suma2).ToString("0,0.00", CultureInfo.InvariantCulture);
        //        controltext.Text = "" + (suma1 + valor_suma2);

        //        labelfondocosto.Text = "0";


        //        foreach (Control miControl in T_Meses.Controls)
        //        {
        //            var tablerow = miControl.Controls;

        //            foreach (Control micontrolrows in tablerow)
        //            {
        //                var hijos = micontrolrows.Controls;

        //                foreach (Control chijos in hijos)
        //                {
        //                    if (chijos.GetType() == typeof(TextBox))
        //                    {
        //                        var text = chijos as TextBox;

        //                        if (text.ID.StartsWith("Fondoo")) //Sueldo
        //                        {
        //                            if (labelfondocosto != null)
        //                            { if (text.Text.Trim() == "") { text.Text = "0"; } labelfondocosto.Text = (Convert.ToDouble(labelfondocosto.Text) + Convert.ToDouble(text.Text)).ToString(); }
        //                            if (L_SumaTotalescosto != null)
        //                                L_SumaTotalescosto.Text = (Convert.ToDouble(labelfondocosto.Text)).ToString();
        //                        }
        //                    }
        //                }
        //            }
        //        }


        //    }
        //    catch (FormatException) { }
        //    catch (NullReferenceException) { }


        //}

        //private void sumarAporte(TextBox textbox, string param_opcional = null)
        //{
        //    String textboxID = textbox.ID;


        //    var labelfondocosto = this.FindControl("labelfondocosto") as Label;
        //    var labelaportescosto = this.FindControl("labelaportescosto") as Label;
        //    var L_SumaTotalescosto = this.FindControl("L_SumaTotalescosto") as Label;

        //    int limit = 0;
        //    if (textboxID.Length == 7)
        //        limit = 1;
        //    else
        //        limit = 2;

        //    String objeto = "TotalMes" + textboxID.Substring(6, limit);

        //    Label controltext;
        //    controltext = (Label)this.FindControl(objeto);

        //    TextBox textFondo;
        //    textFondo = (TextBox)this.FindControl("Fondoo" + textboxID.Substring(6, limit)); //Sueldo
        //    TextBox textAporte;
        //    textAporte = (TextBox)this.FindControl("Aporte" + textboxID.Substring(6, limit)); //Prestaciones
        //    try
        //    {
        //        Double suma1;
        //        Double suma2;

        //        if (String.IsNullOrEmpty(textFondo.Text))
        //            suma1 = 0;
        //        else
        //            suma1 = Double.Parse(textFondo.Text);

        //        if (String.IsNullOrEmpty(textAporte.Text))
        //            suma2 = 0;
        //        else
        //            suma2 = Double.Parse(textAporte.Text);



        //        if (!String.IsNullOrEmpty(param_opcional.Trim()))
        //        {
        //            //Tratamiento para los Productos en "Ventas".
        //            Int32 suma_convertida_1 = Convert.ToInt32(suma1);
        //            double suma_convertida_2 = Math.Floor(suma2);
        //            double valor = suma_convertida_1 + suma_convertida_2;
        //            controltext.Text = "" + valor;
        //        }
        //        else
        //        {
        //            controltext.Text = "" + (suma1 + suma2);
        //        }

        //        labelaportescosto.Text = "0";
        //        Int32 cantidades = 0;

        //        foreach (TableRow fila in T_Meses.Rows)
        //        {
        //            cantidades = 0;
        //            foreach (TableCell celda in fila.Cells)
        //            {
        //                foreach (Control control in celda.Controls)
        //                {
        //                    try
        //                    {

        //                    }
        //                    catch (Exception) { }
        //                }
        //            }
        //        }

        //        //foreach (Control miControl in T_Meses.Controls)
        //        //{
        //        //    var tablerow = miControl.Controls;

        //        //    foreach (Control micontrolrows in tablerow)
        //        //    {
        //        //        var hijos = micontrolrows.Controls;

        //        //        foreach (Control chijos in hijos)
        //        //        {
        //        //            if (chijos.GetType() == typeof(TextBox))
        //        //            {
        //        //                var text = chijos as TextBox;

        //        //                if (text.ID.StartsWith(Prestaciones))
        //        //                {
        //        //                    if (labelaportescosto != null)
        //        //                        labelaportescosto.Text = (Convert.ToDouble(labelaportescosto.Text) + Convert.ToDouble(text.Text)).ToString();

        //        //                    if (L_SumaTotalescosto != null)
        //        //                        L_SumaTotalescosto.Text = (Convert.ToDouble(L_SumaTotalescosto.Text) + (Convert.ToDouble(labelaportescosto.Text))).ToString();
        //        //                }
        //        //            }
        //        //        }
        //        //    }
        //        //}


        //    }
        //    catch (FormatException) { }
        //    catch (NullReferenceException) { }


        //}

        ///// <summary>
        ///// Textbox Changed para Fondo/Sueldo.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void TextBox_TextChanged(object sender, EventArgs e)
        //{
        //    TextBox textbox = (TextBox)sender;
        //    sumar(textbox);
        //}

        ///// <summary>
        ///// TextboxChanged para Aportes/Prestaciones.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void TextBoxAportes_TextChanged(object sender, EventArgs e)
        //{
        //    TextBox textbox = (TextBox)sender;
        //    sumarAporte(textbox, "0");
        //}

        ///// <summary>
        ///// Cerrar la ventana.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void B_Cancelar_Click(object sender, EventArgs e)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true);
        //}

        ///// <summary>
        ///// Mauricio Arias Olave.
        ///// 09/04/2014.
        ///// Modificar la información del valor seleccionado en la grilla de "Producción".
        ///// </summary>
        //private void BuscarDatos_Ventas()
        //{
        //    //Obtiene la conexión
        //    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

        //    //Inicializa la variable para generar la consulta.
        //    String sqlConsulta;

        //    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
        //    {
        //        sqlConsulta = "SELECT DISTINCT * FROM [Fonade].[dbo].[InterventorVentas] " +
        //                     " LEFT JOIN [Fonade].[dbo].[InterventorVentasMes] " +
        //                     " ON [Fonade].[dbo].[InterventorVentas].[id_ventas] = [Fonade].[dbo].[InterventorVentasMes].[CodProducto] " +
        //                     " WHERE [Fonade].[dbo].[InterventorVentas].[CodProyecto] = " + CodProyecto + " " +
        //                     " AND [Fonade].[dbo].[InterventorVentas].[id_ventas] = " + codProduccion + " ";
        //    }
        //    else
        //    {
        //        if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        {

        //            sqlConsulta = "SELECT DISTINCT * FROM [Fonade].[dbo].[InterventorVentas] " +
        //                         " LEFT JOIN [Fonade].[dbo].[InterventorVentasMes] " +
        //                         " ON [Fonade].[dbo].[InterventorVentas].[id_ventas] = [Fonade].[dbo].[InterventorVentasMes].[CodProducto] " +
        //                         " WHERE [Fonade].[dbo].[InterventorVentas].[CodProyecto] = " + CodProyecto + " " +
        //                         " AND [Fonade].[dbo].[InterventorVentas].[id_ventas] = " + codProduccion + " ";

        //            SqlCommand cmd = new SqlCommand(sqlConsulta, conn);
        //            try
        //            {
        //                conn.Open();
        //                SqlDataReader reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    TextBox controltext;
        //                    TextBox costoTotal;
        //                    string valor_Obtenido = reader["Tipo"].ToString();//.Equals("1");

        //                    if (valor_Obtenido.Trim() == "1")
        //                    {
        //                        controltext = (TextBox)this.FindControl("Fondoo" + reader["Mes"].ToString());
        //                        controltext.Text = reader["Valor"].ToString();
        //                        sumar(controltext);
        //                    }
        //                    else
        //                    {
        //                        controltext = (TextBox)this.FindControl("Fondoo" + reader["Mes"].ToString());//aaa

        //                        if (String.IsNullOrEmpty(reader["Valor"].ToString()))
        //                        {
        //                            controltext.Text = "0";
        //                            sumarAporte(controltext);//aaa
        //                        }
        //                        else
        //                        {
        //                            Double valor = Double.Parse(reader["Valor"].ToString());
        //                            controltext.Text = valor.ToString();
        //                            sumarAporte(controltext, codProduccion);
        //                        }
        //                    }
        //                }
        //            }
        //            //catch (SqlException se)
        //            catch (Exception se)
        //            {
        //                string h = se.Message;
        //                throw se;
        //            }
        //            finally
        //            {
        //                conn.Close();
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Mauricio Arias Olave.
        ///// 12/04/2014. Se crea este método para llamar por separado a las consultas.
        ///// </summary>
        ///// <param name="sqlConsulta">Consukta SQL.</param>
        ///// <param name="connection">Conexión.</param>
        //private void Ejecutar(string sqlConsulta, SqlConnection connection)
        //{
        //    //Obtiene la conexión
        //    connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

        //    SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

        //    try
        //    {
        //        connection.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            TextBox controltext;
        //            TextBox costoTotal;
        //            if (reader["Tipo"].ToString().Equals("1")) //CodTipoFinanciacion
        //            {
        //                controltext = (TextBox)this.FindControl("Sueldo" + reader["Mes"].ToString());

        //            }
        //            else
        //            {
        //                controltext = (TextBox)this.FindControl("Sueldo" + reader["Valor"].ToString());
        //            }
        //            controltext.Text = reader["Valor"].ToString();
        //            sumar(controltext);
        //        }
        //        connection.Close();
        //    }
        //    catch (SqlException se)
        //    {
        //        string h = se.Message;
        //        throw se;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}

        //#region Métodos creados el 16/04/2014.

        ///// <summary>
        ///// Crear el producto "en venta", falta que lea los valores de la tabla.
        ///// </summary>
        //private void CrearProductoEnVenta()
        //{
        //    //Inicializar variables.
        //    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //    SqlCommand cmd = new SqlCommand();
        //    String sqlConsulta;
        //    bool procesado = false;
        //    int codProduccion_Autoincr = 1;
        //    int i = 1;

        //    try
        //    {
        //        if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        {
        //            #region Procesar la información.

        //            //Consultar si tiene Coordinador asignado.
        //            var result = (from t in consultas.Db.Interventors
        //                          where t.CodContacto == usuario.IdContacto
        //                          select new { t.CodCoordinador }).FirstOrDefault();

        //            if (result.CodCoordinador == 0) //No existe
        //            {
        //                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
        //                return;
        //            }
        //            else
        //            {
        //                #region Ejecutar sentencias de inserción SQL.

        //                //Consultar el id_ventas para autoincrementarlo y usarlo en la inserción de datos.
        //                var result_ventas_autoincremental = (from t in consultas.Db.InterventorVentasTMPs
        //                                                     select new { t.id_ventas }).OrderByDescending(x => x.id_ventas);

        //                if (result_ventas_autoincremental.Count() == 0)
        //                {
        //                    ///No hay valores, por lo que se debe usar un 1 como valor autoincremental (Id_de la nueva venta.)
        //                    ///Es decir, se emplea la variable "codProduccion_Autoincr".
        //                }
        //                else
        //                {
        //                    //Incrementar valor.
        //                    codProduccion_Autoincr = result_ventas_autoincremental.First().id_ventas + 1;

        //                    //Ejecutar Insert #1.
        //                    sqlConsulta = "INSERT INTO InterventorVentasTMP (id_Ventas, CodProyecto, NomProducto) " +
        //                                  "VALUES (" + codProduccion_Autoincr + ", " + CodProyecto + ", 'NOMBREDELPRODUCTO') "; //Cambiar por el verdadero Nombre del producto.

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    if (procesado) //Si es TRUE, el proceso debe seguir normal, si no sale nada, toca revisar el código.
        //                    {
        //                        #region Recorrer la tabla para agregarle los valores, mientras se hace esta inserción.

        //                        if (i == 1)//Si en el ciclo, el valor es == 1 se hace esta inserción
        //                        {
        //                            //Ejecutar Insert de tipo 1.
        //                            sqlConsulta = "INSERT INTO InterventorVentasMesTMP(CodProducto, Mes, Valor, Tipo) " +
        //                                          "VALUES (" + codProduccion_Autoincr + ", " + 1 + ", " + 0 + ", 1) "; //Cambiar por el verdadero tipo que viene por sesión.

        //                            cmd = new SqlCommand(sqlConsulta, connection);
        //                            procesado = EjecutarSQL(connection, cmd);
        //                        }
        //                        else
        //                        {
        //                            //Ejecutar Insert de tipo 2.
        //                            sqlConsulta = "INSERT INTO InterventorVentasMesTMP(CodProducto, Mes, Valor, Tipo) " +
        //                                          "VALUES (" + codProduccion_Autoincr + ", " + 1 + ", " + 0 + ", 2) "; //Cambiar por el verdadero tipo que viene por sesión.

        //                            cmd = new SqlCommand(sqlConsulta, connection);
        //                            procesado = EjecutarSQL(connection, cmd);
        //                        }

        //                        #endregion

        //                        //prTareaAsignarCoordinadorVentas = En FONADE Clásico está comentado, por lo tanto no está implementado aquí.
        //                    }
        //                    else
        //                    {
        //                        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo crear el producto en venta en InterventorVentasTMP.')", true);
        //                        return;
        //                    }
        //                }

        //                #endregion
        //            }

        //            #endregion
        //        }
        //        else
        //        {
        //            //Según CatalogoVentasTMP.asp, sólo aplica para el grupo "Interventor" (cerrar ventana).
        //            //Cerrar la ventana.
        //            ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        //            return;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo crear el producto en venta.')", true);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// Mauricio Arias Olave.
        ///// 16/04/2014.
        ///// Adicionar el producto en venta seleccionado.
        ///// Adaptando exactamente el código fuente de CatalogoVentasTMP.asp.
        ///// </summary>
        //private void AdicionarProductoEnVenta(int Valor_AprobadoGerente)
        //{
        //    //Inicializar variables.
        //    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //    SqlCommand cmd = new SqlCommand();
        //    String sqlConsulta;
        //    bool procesado = false;
        //    int ProductoEnVentaSeleccionado = 0;
        //    int CodProyectoConvertido = 0;

        //    try { ProductoEnVentaSeleccionado = Convert.ToInt32(codProduccion.ToString()); }
        //    catch { return; }

        //    try { CodProyectoConvertido = Convert.ToInt32(CodProyecto.ToString()); }
        //    catch { return; }

        //    try
        //    {
        //        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
        //        {
        //            #region Procesar como Gerente Interventor.

        //            if (Valor_AprobadoGerente == 1) // Si está aprobado por el gerente
        //            {
        //                #region Sí fue aprobado por el gerente, por esto, continúa el flujo.

        //                #region Trae los registros de la tabla temporal. (Obtiene los datos NomProducto // id_ventas).

        //                var cod_cargo_linq = (from t in consultas.Db.InterventorVentasTMPs
        //                                      where t.CodProyecto == CodProyectoConvertido
        //                                      && t.id_ventas == ProductoEnVentaSeleccionado
        //                                      select new
        //                                      {
        //                                          t.NomProducto,
        //                                          t.id_ventas
        //                                      }).FirstOrDefault();

        //                #endregion

        //                if (cod_cargo_linq.id_ventas > 0)
        //                {
        //                    //Generación de datos.
        //                    string nom_producto_obtenido = cod_cargo_linq.NomProducto;
        //                    int Id_Ventas_Seleccionado = cod_cargo_linq.id_ventas;

        //                    #region Insertar los nuevos registros en la tabla definitiva.

        //                    sqlConsulta = "INSERT INTO InterventorVentas (CodProyecto, NomProducto) " +
        //                                  "VALUES (" + CodProyectoConvertido + ", '" + nom_producto_obtenido + "') ";

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion

        //                    #region Borrar el registro ingresado en la tabla temporal.

        //                    sqlConsulta = "DELETE FROM InterventorVentasTMP" +
        //                                  "WHERE CodProyecto = " + CodProyectoConvertido + " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion

        //                    #region Traer el código de la actividad para adicionarlo a la tabla definitiva por mes.

        //                    var cods_int_ventas = (from s in consultas.Db.InterventorVentas
        //                                           orderby s.id_ventas descending
        //                                           select new
        //                                           {
        //                                               s.id_ventas
        //                                           }).FirstOrDefault();

        //                    int cod_ventas_sql2 = cods_int_ventas.id_ventas;

        //                    #endregion

        //                    #region Trae los registros de la tabla temporal por meses.

        //                    var sql_3 = (from v in consultas.Db.InterventorVentasMesTMPs
        //                                 where v.CodProducto == ProductoEnVentaSeleccionado
        //                                 select new
        //                                 {
        //                                     v.Mes,
        //                                     v.Valor,
        //                                     v.Tipo
        //                                 }).FirstOrDefault();

        //                    int? Mes_sql_3 = sql_3.Mes;
        //                    decimal? Valor_sql_3 = sql_3.Valor;
        //                    int? Tipo_sql_3 = sql_3.Tipo;

        //                    #endregion

        //                    #region Inserta los nuevos registros en la tabla definitiva por meses

        //                    for (int i = 0; i < 14; i++)
        //                    {
        //                        if (Tipo_sql_3 == 1) // Si el tipo es 1, genera este flujo.
        //                        {
        //                            //Registra con tipo 1.
        //                            sqlConsulta = " INSERT INTO InterventorVentasMes (CodProducto, Mes, Valor, Tipo) " +
        //                                          " VALUES (" + ProductoEnVentaSeleccionado + ", " + Mes_sql_3 + ", " + Valor_sql_3 + ", 1)";

        //                            cmd = new SqlCommand(sqlConsulta, connection);
        //                            procesado = EjecutarSQL(connection, cmd);
        //                        }
        //                        else
        //                        {
        //                            //Registra con tipo 2.
        //                            sqlConsulta = " INSERT INTO InterventorVentasMes (CodProducto, Mes, Valor, Tipo) " +
        //                                          " VALUES (" + ProductoEnVentaSeleccionado + ", " + Mes_sql_3 + ", " + Valor_sql_3 + ", 2)";

        //                            cmd = new SqlCommand(sqlConsulta, connection);
        //                            procesado = EjecutarSQL(connection, cmd);
        //                        }
        //                    }

        //                    #endregion

        //                    #region Borrar el registro de la tabla temporal por meses.

        //                    sqlConsulta = "DELETE FROM InterventorVentasMesTMP" +
        //                                  "WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion
        //                }
        //                else
        //                {
        //                    //No se obtuvieron datos.
        //                    return;
        //                }

        //                #endregion
        //            }
        //            if (Valor_AprobadoGerente == 0) //No está aprobado.
        //            {
        //                #region Se procesa la información según el archivo "CatalogoVentasTMP.asp" de FONADE clasico.

        //                //Se devuelve al interventor, se le avisa al coordinador.
        //                var return_interventor = (from emp_int in consultas.Db.EmpresaInterventors
        //                                          join emp in consultas.Db.Empresas
        //                                          on emp_int.CodEmpresa equals emp.id_empresa
        //                                          where emp_int.Inactivo.Equals(0) && emp_int.Rol.Equals(Constantes.CONST_RolInterventorLider)
        //                                          && emp.CodProyecto.Equals(CodProyectoConvertido)
        //                                          select new { emp_int.CodContacto }).FirstOrDefault();

        //                int? codContacto_sql4 = return_interventor.CodContacto;

        //                #region Eliminación #1.

        //                sqlConsulta = "DELETE FROM InterventorVentasTMP" +
        //                              "WHERE CodProyecto = " + CodProyectoConvertido + " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Eliminación #2.

        //                sqlConsulta = "DELETE FROM InterventorVentasMesTMP" +
        //                              "WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Consultar cargo de la siguiente sentencia SQL y generar TareaPendiente.

        //                #region Obtener el nombre del proyecto "para ser usado en la creación de la tarea pendiente".

        //                var nmb_proyecto_linq = (from t in consultas.Db.Proyectos
        //                                         where t.Id_Proyecto.Equals(CodProyectoConvertido)
        //                                         select new { t.NomProyecto }).FirstOrDefault();

        //                string NmbProyecto = nmb_proyecto_linq.NomProyecto;

        //                #endregion

        //                #region Consultar nombre del producto de la siguiente sentencia SQL.

        //                var codcargo_sql5 = (from t in consultas.Db.InterventorVentasTMPs
        //                                     where t.CodProyecto == CodProyectoConvertido
        //                                     && t.id_ventas == ProductoEnVentaSeleccionado
        //                                     select new { t.NomProducto }).FirstOrDefault();

        //                string nmb_producto_obtenido_sql5 = codcargo_sql5.NomProducto;

        //                #endregion

        //                #region Generar tareas pendientes.

        //                TareaUsuario datoNuevo = new TareaUsuario();
        //                datoNuevo.CodContacto = (int)codContacto_sql4;
        //                datoNuevo.CodProyecto = CodProyectoConvertido;
        //                datoNuevo.NomTareaUsuario = "Producto en Ventas Rechazado por Gerente Interventor";
        //                datoNuevo.Descripcion = "Revisar Productos en Ventas " + NmbProyecto + " - Actividad --> " + nmb_producto_obtenido_sql5 + "<BR><BR>Observaciones:<BR>" + "OBSERVACIONES DEL INTERVENTOR"; //& fnRequest("ObservaInter")
        //                datoNuevo.CodTareaPrograma = 2;
        //                datoNuevo.Recurrente = "0"; //"false";
        //                datoNuevo.RecordatorioEmail = false;
        //                datoNuevo.NivelUrgencia = 1;
        //                datoNuevo.RecordatorioPantalla = true;
        //                datoNuevo.RequiereRespuesta = false;
        //                datoNuevo.CodContactoAgendo = usuario.IdContacto;
        //                datoNuevo.DocumentoRelacionado = "";

        //                try
        //                {
        //                    Consultas consulta = new Consultas();
        //                    consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
        //                }
        //                catch { string msg_err = "Error en generar tareas."; }

        //                #endregion

        //                #endregion

        //                #endregion
        //            }
        //            else
        //            {
        //                //Cerrar la ventana.
        //                ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        //                return;
        //            }

        //            #endregion
        //        }
        //        if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        {
        //            #region Procesar como Interventor.

        //            //Consultar si tiene Coordinador asignado.
        //            var result = (from t in consultas.Db.Interventors
        //                          where t.CodContacto == usuario.IdContacto
        //                          select new { t.CodCoordinador }).FirstOrDefault();

        //            if (result.CodCoordinador == 0) //No existe
        //            {
        //                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
        //                return;
        //            }
        //            else
        //            {
        //                #region Ejecutar sentencias SQL para asignar la tarea al coordinador.

        //                #region Obtener el nombre del producto para usarlo en la inserción siguiente.

        //                var NomProducto_linq = (from t in consultas.Db.InterventorVentas
        //                                        where t.CodProyecto == CodProyectoConvertido
        //                                        && t.id_ventas.Equals(ProductoEnVentaSeleccionado)
        //                                        select new { t.NomProducto }).FirstOrDefault();

        //                string Nombre_producto_Obtenido = NomProducto_linq.NomProducto;

        //                #endregion

        //                #region Inserción.

        //                sqlConsulta = "INSERT INTO InterventorVentasTMP (id_Ventas, CodProyecto, NomProducto, Tarea) " +
        //                              "VALUES (" + ProductoEnVentaSeleccionado + ", " + CodProyectoConvertido + ", '" + Nombre_producto_Obtenido + "', 'Modificar') ";

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Recorrer los campos de la tabla con 14 meses y crear registros según las condiciones señaladas en código.

        //                for (int i = 0; i < 15; i++)
        //                {
        //                    for (int j = 0; j < 2; j++)
        //                    {
        //                        //Si el valor de la caja de texto es != de vacío y es diferente de 0 hace la inserción.
        //                        //if ((i && j) && (i!= 0)) //A2
        //                        if (i == 0)
        //                        {
        //                            if (i == 1)
        //                            {
        //                                sqlConsulta = "INSERT INTO InterventorVentasMesTMP(CodProducto, Mes, Valor, Tipo) " +
        //                                              "VALUES (" + ProductoEnVentaSeleccionado + ", " + j + ", '" + j + 1 /*Valor de la caja de texto*/ + ", 1) ";

        //                                cmd = new SqlCommand(sqlConsulta, connection);
        //                                procesado = EjecutarSQL(connection, cmd);
        //                            }
        //                            else
        //                            {
        //                                sqlConsulta = "INSERT INTO InterventorVentasMesTMP(CodProducto, Mes, Valor, Tipo) " +
        //                                              "VALUES (" + ProductoEnVentaSeleccionado + ", " + j + ", '" + j + 1 /*Valor de la caja de texto*/ + ", 2) ";

        //                                cmd = new SqlCommand(sqlConsulta, connection);
        //                                procesado = EjecutarSQL(connection, cmd);
        //                            }
        //                        }
        //                    }
        //                }

        //                #endregion

        //                //prTareaAsignarCoordinadorVentas = No implementado en FONADE clásico, comentado.

        //                #endregion
        //            }

        //            #endregion
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo adicionar el producto en venta seleccionado.')", true);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// Actualizar el producto "en venta".
        ///// Recibe por Session el codProduccion seleccionado, pero se debe validar que si lo tenga, de lo
        ///// contrario lo retorna.
        ///// 16/04/2014: Se tiene que enviar un valor llamado "AprobadoGerente" (al parecer es sólo 1 = Si // 0 = No).
        ///// </summary>
        //private void ActualizarProductoEnVenta(int Valor_AprobadoGerente)
        //{
        //    //Inicializar variables.
        //    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //    SqlCommand cmd = new SqlCommand();
        //    String sqlConsulta;
        //    bool procesado = false;
        //    int ProductoEnVentaSeleccionado = 0;
        //    int CodProyectoConvertido = 0;

        //    try { ProductoEnVentaSeleccionado = Convert.ToInt32(codProduccion.ToString()); }
        //    catch { return; }

        //    try { CodProyectoConvertido = Convert.ToInt32(CodProyecto.ToString()); }
        //    catch { return; }

        //    try
        //    {
        //        ///Comprobar que si el usuario en sesión es un Gerente Interventor, si lo es puede
        //        ///ejecutar el flujo, de lo contrario no (no devuelve nada).

        //        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
        //        {
        //            #region Procesar la actualización como Gerente Interventor.

        //            if (Valor_AprobadoGerente == 1) // Si está aprobado por el gerente
        //            {
        //                #region Sí fue aprobado por el gerente, por esto, continúa el flujo.

        //                #region Trae los registros de la tabla temporal. (Obtiene los datos NomProducto // id_ventas).

        //                var cod_cargo_linq = (from t in consultas.Db.InterventorVentasTMPs
        //                                      where t.CodProyecto == CodProyectoConvertido
        //                                      && t.id_ventas == ProductoEnVentaSeleccionado
        //                                      select new
        //                                      {
        //                                          t.id_ventas,
        //                                          t.NomProducto
        //                                      }).FirstOrDefault();

        //                #endregion

        //                if (cod_cargo_linq.id_ventas > 0)
        //                {
        //                    //Generación de datos.
        //                    string NomProducto_Obtenido = cod_cargo_linq.NomProducto;
        //                    int Id_Producto_Venta_Seleccionado = cod_cargo_linq.id_ventas;

        //                    #region Insertar los nuevos registros en la tabla definitiva.

        //                    sqlConsulta = "INSERT INTO InterventorVentas (CodProyecto, NomProducto) " +
        //                                  "VALUES (" + CodProyectoConvertido + ", '" + NomProducto_Obtenido + "') ";

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion

        //                    #region Borrar el registro ingresado en la tabla temporal.

        //                    sqlConsulta = "DELETE FROM InterventorVentasTMP" +
        //                                  "WHERE CodProyecto = " + CodProyectoConvertido + " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion

        //                    #region Traer el código de la actividad para adicionarlo a la tabla definitiva por mes.

        //                    var cods_int_ventas = (from s in consultas.Db.InterventorVentas
        //                                           orderby s.id_ventas descending
        //                                           select new
        //                                           {
        //                                               s.id_ventas
        //                                           }).FirstOrDefault();

        //                    int cod_ventas_sql2 = cods_int_ventas.id_ventas;

        //                    #endregion

        //                    #region Trae los registros de la tabla temporal por meses.

        //                    var sql_3 = (from v in consultas.Db.InterventorVentasMesTMPs
        //                                 where v.CodProducto == ProductoEnVentaSeleccionado
        //                                 select new
        //                                 {
        //                                     v.Mes,
        //                                     v.Tipo,
        //                                     v.Valor
        //                                 }).FirstOrDefault();

        //                    int? Mes_sql_3 = sql_3.Mes;
        //                    decimal? Valor_sql_3 = sql_3.Valor;
        //                    int? Tipo_sql_3 = sql_3.Tipo;

        //                    #endregion

        //                    #region Inserta los nuevos registros en la tabla definitiva por meses

        //                    for (int i = 0; i < 14; i++)
        //                    {
        //                        if (Tipo_sql_3 == 1) // Si el tipo es 1, genera este flujo.
        //                        {
        //                            #region Consulta que verifica los datos para prevenir valores repetidos cuando es tipo 1.

        //                            // CodCargo, Mes, Valor, Tipo
        //                            sqlConsulta = " SELECT * FROM InterventorVentasMes " +
        //                                          " WHERE Mes = " + Mes_sql_3 + " AND CodProducto = " + Valor_sql_3 +
        //                                          " AND Tipo = 1 ";

        //                            DataTable tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

        //                            if (tabla.Rows.Count > 0) //Si existe, lo actualiza
        //                            {
        //                                sqlConsulta = " UPDATE InterventorVentasMes " +
        //                                              " SET Valor = " + Valor_sql_3 +
        //                                              " WHERE Mes = " + Mes_sql_3 +
        //                                              " AND CodProducto = " + ProductoEnVentaSeleccionado +
        //                                              " AND Tipo = 1";

        //                                cmd = new SqlCommand(sqlConsulta, connection);
        //                                procesado = EjecutarSQL(connection, cmd);
        //                            }
        //                            if (tabla.Rows.Count == 0) // Si NO existe, lo crea.
        //                            {
        //                                sqlConsulta = " INSERT INTO InterventorVentasMes (CodProducto, Mes, Valor, Tipo) " +
        //                                              " VALUES (" + ProductoEnVentaSeleccionado + ", " + Mes_sql_3 + ", " + Valor_sql_3 + ", 1)";

        //                                cmd = new SqlCommand(sqlConsulta, connection);
        //                                procesado = EjecutarSQL(connection, cmd);
        //                            }

        //                            #endregion
        //                        }
        //                        else
        //                        {
        //                            #region Consulta que verifica los datos para prevenir valores repetidos cuando es tipo 2.

        //                            // CodCargo, Mes, Valor, Tipo
        //                            sqlConsulta = " SELECT * FROM InterventorVentasMes " +
        //                                          " WHERE Mes = " + Mes_sql_3 + " AND CodProducto = " + Valor_sql_3 +
        //                                          " AND Tipo = 2 ";

        //                            DataTable tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

        //                            if (tabla.Rows.Count > 0) //Si existe, lo actualiza
        //                            {
        //                                sqlConsulta = " UPDATE InterventorVentasMes " +
        //                                              " SET Valor = " + Valor_sql_3 +
        //                                              " WHERE Mes = " + Mes_sql_3 +
        //                                              " AND CodProducto = " + ProductoEnVentaSeleccionado +
        //                                              " AND Tipo = 2";

        //                                cmd = new SqlCommand(sqlConsulta, connection);
        //                                procesado = EjecutarSQL(connection, cmd);
        //                            }
        //                            if (tabla.Rows.Count == 0) // Si NO existe, lo crea.
        //                            {
        //                                sqlConsulta = " INSERT INTO InterventorVentasMes (CodProducto, Mes, Valor, Tipo)" +
        //                                              " VALUES (" + ProductoEnVentaSeleccionado + ", " + Mes_sql_3 + ", " + Valor_sql_3 + ", 2)";

        //                                cmd = new SqlCommand(sqlConsulta, connection);
        //                                procesado = EjecutarSQL(connection, cmd);
        //                            }

        //                            #endregion
        //                        }
        //                    }

        //                    #endregion

        //                    #region Borrar el registro de la tabla temporal por meses.

        //                    sqlConsulta = "DELETE FROM InterventorVentasMesTMP" +
        //                                  "WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion
        //                }
        //                else
        //                {
        //                    //No se obtuvieron datos.
        //                    return;
        //                }

        //                #endregion
        //            }
        //            if (Valor_AprobadoGerente == 0) //No está aprobado.
        //            {
        //                #region Se procesa la información según el archivo "CatalogoVentasTMP.asp" de FONADE clasico.

        //                //Se devuelve al interventor, se le avisa al coordinador.
        //                var return_interventor = (from emp_int in consultas.Db.EmpresaInterventors
        //                                          join emp in consultas.Db.Empresas
        //                                          on emp_int.CodEmpresa equals emp.id_empresa
        //                                          where emp_int.Inactivo.Equals(0) && emp_int.Rol.Equals(Constantes.CONST_RolInterventorLider)
        //                                          && emp.CodProyecto.Equals(CodProyectoConvertido)
        //                                          select new { emp_int.CodContacto }).FirstOrDefault();

        //                int? codContacto_sql4 = return_interventor.CodContacto;

        //                #region Eliminación #1.

        //                sqlConsulta = "DELETE FROM InterventorVentasTMP" +
        //                              "WHERE CodProyecto = " + CodProyectoConvertido + " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Eliminación #2.

        //                sqlConsulta = "DELETE FROM InterventorVentasMesTMP" +
        //                              "WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Consultar cargo de la siguiente sentencia SQL y generar TareaPendiente.

        //                #region Obtener el nombre del proyecto "para ser usado en la creación de la tarea pendiente".

        //                var nmb_proyecto_linq = (from t in consultas.Db.Proyectos
        //                                         where t.Id_Proyecto.Equals(CodProyectoConvertido)
        //                                         select new { t.NomProyecto }).FirstOrDefault();

        //                string NmbProyecto = nmb_proyecto_linq.NomProyecto;

        //                #endregion

        //                #region Consultar nombre del producto de la siguiente sentencia SQL.

        //                var codcargo_sql5 = (from t in consultas.Db.InterventorVentasTMPs
        //                                     where t.CodProyecto == CodProyectoConvertido
        //                                     && t.id_ventas == ProductoEnVentaSeleccionado
        //                                     select new { t.NomProducto }).FirstOrDefault();

        //                string nmb_producto_obtenido_sql5 = codcargo_sql5.NomProducto;

        //                #endregion

        //                #region Generar tareas pendientes.

        //                TareaUsuario datoNuevo = new TareaUsuario();
        //                datoNuevo.CodContacto = (int)codContacto_sql4;
        //                datoNuevo.CodProyecto = CodProyectoConvertido;
        //                datoNuevo.NomTareaUsuario = "Producto en Ventas Rechazado por Gerente Interventor";
        //                datoNuevo.Descripcion = "Revisar Productos en Ventas " + NmbProyecto + " - Actividad --> " + nmb_producto_obtenido_sql5 + "<BR><BR>Observaciones:<BR>" + "OBSERVACIONES DEL INTERVENTOR"; //& fnRequest("ObservaInter")
        //                datoNuevo.CodTareaPrograma = 2;
        //                datoNuevo.Recurrente = "0"; //"false";
        //                datoNuevo.RecordatorioEmail = false;
        //                datoNuevo.NivelUrgencia = 1;
        //                datoNuevo.RecordatorioPantalla = true;
        //                datoNuevo.RequiereRespuesta = false;
        //                datoNuevo.CodContactoAgendo = usuario.IdContacto;
        //                datoNuevo.DocumentoRelacionado = "";

        //                try
        //                {
        //                    Consultas consulta = new Consultas();
        //                    consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
        //                }
        //                catch { string msg_err = "Error en generar tareas."; }

        //                #endregion

        //                #endregion

        //                #endregion
        //            }
        //            else
        //            {
        //                //Cerrar la ventana.
        //                ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        //                return;
        //            }

        //            #endregion
        //        }
        //        if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
        //        {
        //            #region Procesar la actualización como Coordinador Interventor.

        //            if (Valor_AprobadoGerente == 1) //Se usa la misma variable, pero para Coordinador se llama "Aprobado".
        //            {
        //                #region Update.

        //                sqlConsulta = " UPDATE InterventorVentasTMP " +
        //                              " SET ChequeoCoordinador = 1 " +
        //                              " WHERE CodProyecto = " + CodProyectoConvertido +
        //                              " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Obtener Id_Grupo de la consulta.

        //                var IdGrupo_sql1 = (from t in consultas.Db.Grupos
        //                                    where t.NomGrupo.Equals("Gerente Interventor")
        //                                    select new { t.Id_Grupo }).FirstOrDefault();

        //                int IdGrupo_Obtenido = IdGrupo_sql1.Id_Grupo;

        //                #endregion

        //                #region Con el Id_Grupo obtenido de la consulta anterior, se consulta el CodContacto de GrupoContacto.

        //                //Es una tabla relacional.
        //                var CodContacto_Rel_sql2 = (from t in consultas.Db.GrupoContactos
        //                                            where t.CodGrupo.Equals(IdGrupo_Obtenido)
        //                                            select new { t.CodContacto }).FirstOrDefault();

        //                int CodContacto_Obtenido = CodContacto_Rel_sql2.CodContacto;

        //                #endregion

        //                //Asignar tarea = NO IMPLEMENTADO = código comentado en el clásico. prTareaAsignarGerenteVentas
        //            }
        //            if (Valor_AprobadoGerente == 0) //No está aprobado.
        //            {
        //                #region Se procesa la información según el archivo "CatalogoVentasTMP.asp" de FONADE clasico.

        //                //Se devuelve al interventor, se le avisa al coordinador.
        //                var return_interventor = (from emp_int in consultas.Db.EmpresaInterventors
        //                                          join emp in consultas.Db.Empresas
        //                                          on emp_int.CodEmpresa equals emp.id_empresa
        //                                          where emp_int.Inactivo.Equals(0) && emp_int.Rol.Equals(Constantes.CONST_RolInterventorLider)
        //                                          && emp.CodProyecto.Equals(CodProyectoConvertido)
        //                                          select new { emp_int.CodContacto }).FirstOrDefault();

        //                int? codContacto_sql4 = return_interventor.CodContacto;

        //                #region Eliminación #1.

        //                sqlConsulta = "DELETE FROM InterventorVentasTMP" +
        //                              "WHERE CodProyecto = " + CodProyectoConvertido + " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Eliminación #2.

        //                sqlConsulta = "DELETE FROM InterventorVentasMesTMP" +
        //                              "WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Consultar cargo de la siguiente sentencia SQL y generar TareaPendiente.

        //                #region Obtener el nombre del proyecto "para ser usado en la creación de la tarea pendiente".

        //                var nmb_proyecto_linq = (from t in consultas.Db.Proyectos
        //                                         where t.Id_Proyecto.Equals(CodProyectoConvertido)
        //                                         select new { t.NomProyecto }).FirstOrDefault();

        //                string NmbProyecto = nmb_proyecto_linq.NomProyecto;

        //                #endregion

        //                #region Consultar nombre del producto de la siguiente sentencia SQL.

        //                var nmb_producto_sql5 = (from t in consultas.Db.InterventorVentasTMPs
        //                                         where t.CodProyecto == CodProyectoConvertido
        //                                         && t.id_ventas == ProductoEnVentaSeleccionado
        //                                         select new { t.NomProducto }).FirstOrDefault();

        //                string nmb_producto_obtenido_sql5 = nmb_producto_sql5.NomProducto;

        //                #endregion

        //                #region Generar tareas pendientes.

        //                TareaUsuario datoNuevo = new TareaUsuario();
        //                datoNuevo.CodContacto = (int)codContacto_sql4;
        //                datoNuevo.CodProyecto = CodProyectoConvertido;
        //                datoNuevo.NomTareaUsuario = "Producto en Ventas Rechazado por Coordinador de Interventoria";
        //                datoNuevo.Descripcion = "Revisar Productos en Ventas " + NmbProyecto + " - Actividad --> " + nmb_producto_obtenido_sql5 + "<BR><BR>Observaciones:<BR>" + "OBSERVACIONES DEL INTERVENTOR"; //& fnRequest("ObservaInter")
        //                datoNuevo.CodTareaPrograma = 2;
        //                datoNuevo.Recurrente = "0"; //"false";
        //                datoNuevo.RecordatorioEmail = false;
        //                datoNuevo.NivelUrgencia = 1;
        //                datoNuevo.RecordatorioPantalla = true;
        //                datoNuevo.RequiereRespuesta = false;
        //                datoNuevo.CodContactoAgendo = usuario.IdContacto;
        //                datoNuevo.DocumentoRelacionado = "";

        //                try
        //                {
        //                    Consultas consulta = new Consultas();
        //                    consulta.Db.TareaUsuarios.InsertOnSubmit(datoNuevo);
        //                }
        //                catch { string msg_err = "Error en generar tareas."; }

        //                #endregion

        //                #endregion

        //                #endregion
        //            }
        //            else
        //            {
        //                //Cerrar la ventana.
        //                ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        //                return;
        //            }

        //            #endregion
        //        }
        //        if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        {
        //            #region Procesar la actualización como Interventor.

        //            //Consultar si tiene Coordinador asignado.
        //            var result = (from t in consultas.Db.Interventors
        //                          where t.CodContacto == usuario.IdContacto
        //                          select new { t.CodCoordinador }).FirstOrDefault();

        //            if (result.CodCoordinador == 0) //No existe
        //            {
        //                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
        //                return;
        //            }
        //            else
        //            {
        //                #region Ejecuta sentencias de actualización SQL.

        //                #region Obtener el nombre del producto para usarlo en la inserción siguiente.

        //                var NomProducto_linq = (from t in consultas.Db.InterventorVentas
        //                                        where t.CodProyecto == CodProyectoConvertido
        //                                        && t.id_ventas.Equals(ProductoEnVentaSeleccionado)
        //                                        select new { t.NomProducto }).FirstOrDefault();

        //                string Nombre_producto_Obtenido = NomProducto_linq.NomProducto;

        //                #endregion

        //                #region Inserción.

        //                sqlConsulta = "INSERT INTO InterventorVentasTMP (id_Ventas, CodProyecto, NomProducto, Tarea) " +
        //                              "VALUES (" + ProductoEnVentaSeleccionado + ", " + CodProyectoConvertido + ", '" + Nombre_producto_Obtenido + "', 'Modificar') ";

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Recorrer los campos de la tabla con 14 meses y crear registros según las condiciones señaladas en código.

        //                for (int i = 0; i < 15; i++)
        //                {
        //                    for (int j = 0; j < 2; j++)
        //                    {
        //                        //Si el valor de la caja de texto es != de vacío y es diferente de 0 hace la inserción.
        //                        //if ((i && j) && (i!= 0)) //A2
        //                        if (i == 0)
        //                        {
        //                            if (i == 1)
        //                            {
        //                                sqlConsulta = "INSERT INTO InterventorVentasMesTMP(CodProducto, Mes, Valor, Tipo) " +
        //                                              "VALUES (" + ProductoEnVentaSeleccionado + ", " + j + ", '" + j + 1 /*Valor de la caja de texto*/ + ", 1) ";

        //                                cmd = new SqlCommand(sqlConsulta, connection);
        //                                procesado = EjecutarSQL(connection, cmd);
        //                            }
        //                            else
        //                            {
        //                                sqlConsulta = "INSERT INTO InterventorVentasMesTMP(CodProducto, Mes, Valor, Tipo) " +
        //                                              "VALUES (" + ProductoEnVentaSeleccionado + ", " + j + ", '" + j + 1 /*Valor de la caja de texto*/ + ", 2) ";

        //                                cmd = new SqlCommand(sqlConsulta, connection);
        //                                procesado = EjecutarSQL(connection, cmd);
        //                            }
        //                        }
        //                    }
        //                }

        //                #endregion

        //                //prTareaAsignarCoordinadorVentas = No implementado en FONADE clásico, comentado.

        //                #endregion
        //            }

        //            #endregion
        //        }
        //        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor || usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
        //        {
        //            //Cerrar la ventana.
        //            ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        //            return;
        //        }
        //        else
        //        {
        //            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene permisos para realizar esta acción.')", true);
        //            return;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo actualizar el producto en venta seleccionado.')", true);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// Mauricio Arias Olave.
        ///// 16/04/2014.
        ///// Modificar el producto seleccionado.
        ///// Adaptando exactamente el código fuente de CatalogoVentasTMP.asp.
        ///// </summary>
        ///// <param name="Valor_AprobadoGerente">Valor Aprobado, debe ser 1 para ejecutar el proceso, de lo contrario, se cerrará la ventana.</param>
        //private void ModificarProductoEnVenta(int Valor_AprobadoGerente)
        //{
        //    //Inicializar variables.
        //    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //    SqlCommand cmd = new SqlCommand();
        //    String sqlConsulta;
        //    bool procesado = false;
        //    int ProductoEnVentaSeleccionado = 0;
        //    int CodProyectoConvertido = 0;

        //    try
        //    {
        //        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
        //        {
        //            if (Valor_AprobadoGerente == 1) //Sí está aprobado.
        //            {
        //                #region Procesa la informacion, ya que está aprobada.

        //                #region Trae los registros de la tabla temporal. (Obtiene los datos "Cargo" = Nombre // Id_Nomina = Id).

        //                var cod_cargo_linq = (from t in consultas.Db.InterventorVentasTMPs
        //                                      where t.CodProyecto == CodProyectoConvertido
        //                                      && t.id_ventas == ProductoEnVentaSeleccionado
        //                                      select new
        //                                      {
        //                                          t.id_ventas
        //                                      }).FirstOrDefault();

        //                #endregion

        //                if (cod_cargo_linq.id_ventas > 0)
        //                {
        //                    //Generación de datos.
        //                    int Id_Ventas_Seleccionado = cod_cargo_linq.id_ventas;

        //                    #region Actualiza los registros en la tabla definitiva.

        //                    sqlConsulta = " UPDATE InterventorVentas " +
        //                                  " SET CodProyecto = " + CodProyectoConvertido +
        //                                  " WHERE CodProyecto = " + CodProyectoConvertido +
        //                                  " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion

        //                    #region Borrar el registro ya actualizado en la tabla temporal.

        //                    sqlConsulta = "DELETE FROM InterventorVentasTMP" +
        //                                  "WHERE id_Ventas = " + CodProyectoConvertido;

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion

        //                    #region Traer el código de la actividad para adicionarlo a la tabla definitiva por mes.

        //                    var cods_int_ventas = (from s in consultas.Db.InterventorVentas
        //                                           orderby s.id_ventas descending
        //                                           select new
        //                                           {
        //                                               s.id_ventas
        //                                           }).FirstOrDefault();

        //                    int cod_ventas_sql2 = cods_int_ventas.id_ventas;

        //                    #endregion

        //                    #region Trae los registros de la tabla temporal por meses.

        //                    //Consulta SQL. CodProducto, Mes, Tipo, valor
        //                    sqlConsulta = " SELECT CodProducto, Mes, Tipo, valor FROM InterventorVentasMesTMP " +
        //                                  " WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                    #region Borrar todos los registros de la tabla "InterventorVentasMes" correspondientes al código de la actividad...

        //                    sqlConsulta = "DELETE FROM InterventorVentasMes" +
        //                                  "WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion

        //                    //Asignar el resultado de la consulta en una tabla para luego recorrerla.
        //                    DataTable tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

        //                    #region Hace el recorrido de la tabla que contiene los resultados, evaluando si es de tipo 1 ó 2 y hace la inserción.

        //                    for (int i = 0; i < tabla.Rows.Count; i++)
        //                    {
        //                        if (tabla.Rows[i]["Tipo"].ToString() == "1")
        //                        {
        //                            #region Inserción de tipo 1.

        //                            sqlConsulta = " INSERT INTO InterventorVentasMes (CodProducto, Mes, Valor, Tipo) " +
        //                                          " VALUES (" + ProductoEnVentaSeleccionado + ", " + Convert.ToInt32(tabla.Rows[i]["Mes"].ToString()) + ", " + Convert.ToInt32(tabla.Rows[i]["Valor"].ToString()) + ", 1)";

        //                            cmd = new SqlCommand(sqlConsulta, connection);
        //                            procesado = EjecutarSQL(connection, cmd);

        //                            #endregion
        //                        }
        //                        else
        //                        {
        //                            #region Inserción de tipo 2.

        //                            sqlConsulta = " INSERT INTO InterventorVentasMes (CodProducto, Mes, Valor, Tipo) " +
        //                                          " VALUES (" + ProductoEnVentaSeleccionado + ", " + Convert.ToInt32(tabla.Rows[i]["Mes"].ToString()) + ", " + Convert.ToInt32(tabla.Rows[i]["Valor"].ToString()) + ", 2)";

        //                            cmd = new SqlCommand(sqlConsulta, connection);
        //                            procesado = EjecutarSQL(connection, cmd);

        //                            #endregion
        //                        }
        //                    }

        //                    #endregion

        //                    #region Borrar el registro ya ingresado en la tabla temporal por meses.

        //                    sqlConsulta = "DELETE FROM InterventorVentasMesTMP" +
        //                                  "WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                    cmd = new SqlCommand(sqlConsulta, connection);
        //                    procesado = EjecutarSQL(connection, cmd);

        //                    #endregion

        //                    #endregion
        //                }
        //                else
        //                {
        //                    //No se obtuvieron datos.
        //                    return;
        //                }

        //                #endregion
        //            }
        //            else
        //            {
        //                //Cerrar la ventana.
        //                ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            //No puede acceder a esta funcionalidad según "CatalogoVentasTMP.asp".
        //            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene permisos para realizar esta acción.')", true);
        //            return;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo modificar el producto seleccionado.')", true);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// Mauricio Arias Olave.
        ///// 16/04/2014.
        ///// Eliminar el producto en venta, de acuerdo a los permisos que tenga el usuario en sesión, es decir, 
        ///// de acuerdo a su grupo = rol.
        ///// <param name="Valor_Aprobado">Valor aprobado, debe ser 1 = SI // 0 = NO o saldrá y no hará nada.</param>
        ///// </summary>
        //private void EliminarProductoEnVenta(int Valor_Aprobado)
        //{
        //    //Inicializar variables.
        //    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //    SqlCommand cmd = new SqlCommand();
        //    String sqlConsulta;
        //    bool procesado = false;
        //    int ProductoEnVentaSeleccionado = 0;
        //    int CodProyectoConvertido = 0;

        //    try { ProductoEnVentaSeleccionado = Convert.ToInt32(codProduccion.ToString()); }
        //    catch { return; }

        //    try { CodProyectoConvertido = Convert.ToInt32(CodProyecto.ToString()); }
        //    catch { return; }

        //    try
        //    {
        //        if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        {
        //            #region Procesar el flujo como Interventor.

        //            //Consultar si tiene Coordinador asignado.
        //            var result = (from t in consultas.Db.Interventors
        //                          where t.CodContacto == usuario.IdContacto
        //                          select new { t.CodCoordinador }).FirstOrDefault();

        //            if (result.CodCoordinador == 0) //No existe
        //            {
        //                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
        //                return;
        //            }
        //            else
        //            {
        //                #region Asignar la tarea al coordinador.

        //                sqlConsulta = "INSERT INTO InterventorVentasTMP (id_Ventas, CodProyecto, Tarea) " +
        //                              "VALUES (" + ProductoEnVentaSeleccionado + ", " + CodProyectoConvertido + " , 'Borrar') ";

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Hace una segunda inserción a otra tabla...

        //                sqlConsulta = "INSERT INTO InterventorVentasMesTMP (CodProducto) " +
        //                              "VALUES ( " + ProductoEnVentaSeleccionado + ") ";

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                //Asignar tarea = NO IMPLEMENTADO = código comentado en el clásico. prTareaAsignarCoordinadorVentas
        //            }

        //            #endregion
        //        }
        //        if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
        //        {
        //            #region Procesar el flujo como Gerente Interventor.

        //            if (Valor_Aprobado == 1) // Si está aprobado
        //            {
        //                #region Borrar los registros de la tabla definitiva.

        //                sqlConsulta = " DELETE FROM InterventorVentas " +
        //                              " WHERE CodProyecto = " + CodProyectoConvertido + " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Borrar el registro ya ingresado en la tabla temporal.

        //                sqlConsulta = " DELETE FROM InterventorVentasTMP " +
        //                              " WHERE CodProyecto = " + CodProyectoConvertido + " AND id_Ventas = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Borra los registros de la tabla definitiva por meses.

        //                sqlConsulta = " DELETE FROM InterventorVentasMes " +
        //                              " WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion

        //                #region Borra el registro de la tabla temporal por meses.

        //                sqlConsulta = " DELETE FROM InterventorVentasMesTMP " +
        //                              " WHERE CodProducto = " + ProductoEnVentaSeleccionado;

        //                cmd = new SqlCommand(sqlConsulta, connection);
        //                procesado = EjecutarSQL(connection, cmd);

        //                #endregion
        //            }
        //            else
        //            {
        //                //No está aprobado.
        //                //Cerrar la ventana.
        //                ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        //                return;
        //            }

        //            #endregion
        //        }
        //        ///Inquietud: Según el FONADE clásico, valida PRIMERO si el grupo(rol) es Interventor, si es así, ejecuta el código, pero después 
        //        ///hace la misma validación de que si el grupo(rol) es Interventor, si es así, se cierra la ventana pero le envía otros datos a otra página...
        //        if (usuario.CodGrupo == Constantes.CONST_Interventor)
        //        {
        //            //Cerrar la ventana.
        //            ClientScriptManager cm = this.ClientScript; cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
        //            return;
        //        }
        //        else
        //        {
        //            //No puede acceder a esta funcionalidad según "CatalogoVentasTMP.asp".
        //            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene permisos para realizar esta acción.')", true);
        //            return;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo eliinar el producto en venta seleccionado.')", true);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// Mauricio Arias Olave.
        ///// Ejecutar SQL.
        ///// Método que recibe la conexión y la consulta SQL y la ejecuta.
        ///// </summary>
        ///// <param name="p_connection">Conexión.</param>
        ///// <param name="p_cmd">SqlCommand.</param>
        ///// <returns>TRUE = Consulta ejecutada correctamente. // FALSE = Error.</returns>
        //private bool EjecutarSQL(SqlConnection p_connection, SqlCommand p_cmd)
        //{
        //    //Ejecutar controladamente la consulta SQL.
        //    try
        //    {
        //        p_connection.Open();
        //        p_cmd.ExecuteReader();
        //        p_connection.Close();
        //        return true;
        //    }
        //    catch
        //    { return false; }
        //    finally
        //    { p_connection.Close(); }
        //}

        //#endregion 
        #endregion

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