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
    public partial class CatalogoActividadPOTMP : Base_Page
    {
        public String codProyecto;
        public int txtTab = Constantes.CONST_ProyeccionesVentas;
        Int32 CodActividad;
        /// <summary>
        /// Variable para ejecutar consultas SQL.
        /// </summary>
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
            try
            {
                //Código del proyecto:
                codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";

                //Asignar evento JavaScript a TextBox.
                TB_item.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");

                #region Procesar la información que proviene de "FramePlanOperativoInterventoria.aspx".

                //Código de la actividad seleccionada:
                CodActividad = Session["CodActividad"] != null && !string.IsNullOrEmpty(Session["CodActividad"].ToString()) ? Convert.ToInt32(Session["CodActividad"].ToString()) : 0;

                //En "FramePlanOperativoInterventoria.aspx" se creó otra variable que contiene el Id de la actividad.
                if (CodActividad == 0)
                {
                    //Aquí se obtiene el valor generado en "FramePlanOperativoInterventoria.aspx"
                    CodActividad = Session["NomActividad"] != null && !string.IsNullOrEmpty(Session["NomActividad"].ToString()) ? Convert.ToInt32(Session["NomActividad"].ToString()) : 0;

                    //Si ya aquí NO contiene datos, es un error, se recarga la página padre y se cierra esta ventana emergente.
                    if (CodActividad == 0)
                    { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.opener.location.reload();window.close();", true); }
                }

                //Evaluar la acción a tomar.
                if (Session["Accion"].ToString().Equals("crear"))
                { B_Acion.Width = 100; }
                if (Session["Accion"].ToString().Equals("actualizar") || Session["Accion"].ToString().Equals("Editar"))
                { B_Acion.Text = "Actualizar"; }
                if (Session["Accion"].ToString().Equals("borrar"))
                { B_Acion.Text = "Borrar"; }

                //Llenar el panel.
                llenarpanel();

                //Establecer título.
                CargarTitulo(Session["Accion"].ToString());

                if (CodActividad != 0)
                {
                    #region Bloquea los campos dependiendo de la consulta en el campo "ChequeoCoordinador".
                    txtSQL = " SELECT ChequeoCoordinador FROM proyectoactividadPOInterventorTMP " +
                             " where CodProyecto = " + codProyecto + " AND id_Actividad = " + CodActividad;

                    var dt = consultas.ObtenerDataTable(txtSQL, "text");

                    if (dt.Rows.Count > 0)
                    {
                        switch (dt.Rows[0]["ChequeoCoordinador"].ToString())
                        {
                            case "0":
                                break;
                            default:
                                //Inhabilitar campos:
                                TB_item.Enabled = false;
                                TB_Actividad.Enabled = false;
                                TB_metas.Enabled = false;
                                //Inhabilitar panel que contiene la tabla dinámica.
                                P_Meses.Enabled = false;
                                break;
                        }
                        dt = null;
                    }
                    #endregion

                    //Buscar los datos.
                    buscarDatos(CodActividad);
                }

                #endregion
            }
            catch (Exception) { }
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
            prorroga = ObtenerProrroga(codProyecto);
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
        private void buscarDatos(Int32 actividad)
        {
            #region Información de la grilla "Actividades en Aprobación".

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            String sqlConsulta;
            DataTable tabla = new DataTable();

            if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
            {
                #region Consultar la información si es Coordinador o Gerente Interventor.
                sqlConsulta = "SELECT * FROM [Fonade].[dbo].[ProyectoActividadPOInterventorTMP] WHERE [CodProyecto] = " + codProyecto + "AND [Id_Actividad] = " + actividad;

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

        protected void B_Acion_Click(object sender, EventArgs e)
        {
            if (B_Acion.Text.Equals("Crear")) alamcenar(1);
            if (B_Acion.Text.Equals("Actualizar")) alamcenar(2);
        }

        private void alamcenar(int acion)
        {
            //Inicializar variables.            
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();

            if (acion == 1)
            {
                #region Guardar la información de plan operativo "guarda en tablas temporales".

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
                                "values (" + ActividadTmp + "," + codProyecto + "," + TB_item.Text + ",'" + TB_Actividad.Text + "','" + TB_metas.Text + "')";

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
                    codProyecto = "0";
                    //Session["CodProyecto"] = null;
                    CodActividad = 0;
                    Session["CodActividad"] = null;
                    Session["NomActividad"] = null;
                    RedirectPage(false, string.Empty, "cerrar");
                }

                #endregion
            }
            if (acion == 2)
            {
                #region Editar el plan operativo seleccionado.

                //Comprobar si el usuario tiene el código grupo de "Coordinador Interventor" ó "Gerente Interventor".
                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                { }
                if (usuario.CodGrupo == Constantes.CONST_Interventor) //Si el usuario tiene el código grupo "Interventor".
                {
                    string txtSQL = "SELECT CodCoordinador FROM interventor WHERE codcontacto=" + usuario.IdContacto;
                    SqlDataReader reader = ejecutaReader(txtSQL, 1);

                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            txtSQL = "INSERT INTO proyectoactividadPOInterventorTMP (id_actividad,CodProyecto,Item,NomActividad,Metas,Tarea) " +
                            "values (" + CodActividad + "," + codProyecto + "," + TB_item.Text + ",'" + TB_Actividad.Text + "','" + TB_metas.Text + "','Modificar')";

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
                    codProyecto = "0";
                    //Session["CodProyecto"] = null;
                    CodActividad = 0;
                    Session["CodActividad"] = null;
                    Session["NomActividad"] = null;
                    RedirectPage(false, string.Empty, "cerrar");
                }

                #endregion
            }
            else //No es un dato válido.
            { return; }
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
            codProyecto = "0";
            //Session["CodProyecto"] = null;
            CodActividad = 0;
            Session["CodActividad"] = null;
            Session["NomActividad"] = null;
            RedirectPage(false, string.Empty, "cerrar");
        }

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
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    lbl_titulo_PO.Text = "Consultar";
                    //Ocultar botón de "actualización".
                    B_Acion.Visible = false;
                    //Inhabilitar campos.
                    TB_item.Enabled = false;
                    TB_Actividad.Enabled = false;
                    TB_metas.Enabled = false;
                    //Inhabilitar panel que contiene la tabla dinámica.
                    P_Meses.Enabled = false;
                }
            }
            catch { lbl_titulo_PO.Text = "ADICIONAR ACTIVIDAD"; }
        }
    }
}