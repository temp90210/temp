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

namespace Fonade.FONADE.Proyecto
{
    public partial class ProyectoOperacionCompras : Negocio.Base_Page
    {
        #region Variables globales.

        public String codProyecto;
        public int txtTab = Constantes.CONST_Compras;
        public String codConvocatoria;
        String txtSQL;
        Boolean esMiembro;
        Boolean bRealizado;

        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //codProyecto = Session["CodProyecto"] != null ? codProyecto = Session["CodProyecto"].ToString() : "0";
            codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            codConvocatoria = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["CodConvocatoria"].ToString() : "0";

            if (!IsPostBack)
            {
                //Consultar si es miembro.
                esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

                //Consultar si está "realizado".
                bRealizado = esRealizado(txtTab.ToString(), codProyecto, ""); //);

                if (codProyecto != "0")
                {
                    //if (usuario.CodGrupo != Constantes.CONST_Emprendedor)
                    if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
                    {
                        Post_It1.Visible = true;
                        tr_buttons.Visible = true;
                        tr_1.Visible = true;
                    }
                    else
                    {
                        Post_It1.Visible = false;
                        tr_buttons.Visible = false;
                        tr_1.Visible = false;
                    }

                    ObtenerDatosUltimaActualizacion();
                    GenerarTablaInsumos();
                }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 09/06/2014.
        /// Generar la tabla "Insumos"...
        /// </summary>
        private void GenerarTablaInsumos()
        {
            //Inicializar variables.
            String[] encabezados = { "&nbsp;", "Materia Prima, Insumo o Requerimiento", 
                                     "Unidad", "Cantidad", "Presentación", "Margen de Desperdicio (%)" };
            String[] Tamanos = { "5%", "32%", "12%", "13%", "16%", "22%" };
            TableCell celdaEncabezado = new TableCell();
            TableCell celdaDatos = new TableCell();
            TableHeaderRow fila1 = new TableHeaderRow();
            TableRow fila = new TableRow();
            DataTable rsProducto = new System.Data.DataTable();
            DataTable rsInsumo = new System.Data.DataTable();
            String txtTipoInsumo;
            Table DynamicTable = new Table();
            int aumt = 0;
            int incr = 0;

            try
            {
                //Consulta.
                txtSQL = " select id_producto, nomproducto " +
                         " from proyectoproducto " +
                         " where codproyecto = " + codProyecto +
                         " order by nomproducto ";

                //Asignar resultados de la consulta.
                rsProducto = consultas.ObtenerDataTable(txtSQL, "text");

                //Recorrer DataTable "rsProducto".
                foreach (DataRow row_RsProducto in rsProducto.Rows)
                {
                    #region Crear nueva tabla.

                    //Datos iniciales de la tabla.
                    DynamicTable = new Table();
                    DynamicTable.CssClass = "Grilla";
                    DynamicTable.Attributes.Add("width", "98%");
                    DynamicTable.Attributes.Add("border", "0");
                    DynamicTable.Attributes.Add("cellspacing", "1");
                    DynamicTable.Attributes.Add("cellpadding", "4");
                    DynamicTable.ID = "NewTable_" + incr.ToString();

                    #region Generar nueva fila con el nombre del producto.

                    fila = new TableRow();
                    fila.Style.Add("background-color", "White");
                    celdaDatos = new TableCell();
                    celdaDatos.Attributes.Add("colspan", "4");
                    celdaDatos.Text = "<b>" + row_RsProducto["nomProducto"].ToString() + "</b>";
                    fila.Cells.Add(celdaDatos);
                    DynamicTable.Rows.Add(fila);

                    #endregion

                    #region Generar encabezados para la tabla dinámica.

                    fila1 = new TableHeaderRow();
                    foreach (string item_string in encabezados)
                    {
                        celdaEncabezado = new TableHeaderCell();
                        celdaEncabezado.Text = item_string;
                        celdaEncabezado.Attributes.Add("align", "left");
                        celdaEncabezado.Attributes.Add("width", Tamanos[aumt]);
                        fila1.Cells.Add(celdaEncabezado);
                        aumt++;
                    }
                    DynamicTable.Rows.Add(fila1);
                    aumt = 0;

                    #endregion

                    #region Consulta SQL para cargar insumos y materias primas.

                    txtSQL = " SELECT p.*, i.Unidad, i.id_insumo, i.nomInsumo, T.* " +
                             " FROM proyectoproductoinsumo p, ProyectoInsumo i, TipoInsumo T " +
                             " WHERE codTipoInsumo = Id_TipoInsumo AND codinsumo = id_insumo  " +
                             " AND codproducto = " + row_RsProducto["id_producto"].ToString() +
                             " ORDER BY NomTipoInsumo, NomInsumo";

                    rsInsumo = consultas.ObtenerDataTable(txtSQL, "text");

                    txtTipoInsumo = "";

                    foreach (DataRow row_RsInsumo in rsInsumo.Rows)
                    {
                        if (txtTipoInsumo != row_RsInsumo["id_TipoInsumo"].ToString())
                        {
                            //Asignar el valor del tipo de insumo
                            txtTipoInsumo = row_RsInsumo["id_TipoInsumo"].ToString();

                            //Generar nueva fila "Insumo" o "Materia Prima".
                            fila = new TableRow();
                            fila.Attributes.Add("height", "30");
                            celdaDatos = new TableCell();
                            celdaDatos.Text = "&nbsp;";
                            fila.Cells.Add(celdaDatos);
                            celdaDatos = new TableCell();
                            celdaDatos.Text = "<strong style=\"color:#174680;\">" + row_RsInsumo["nomTipoInsumo"].ToString() + "</strong>";
                            celdaDatos.Attributes.Add("align", "left");
                            celdaDatos.Attributes.Add("colspan", "5");
                            fila.Cells.Add(celdaDatos);
                            DynamicTable.Rows.Add(fila);
                        }

                        #region Generar fila con demás datos.

                        //Fila de espacio.
                        fila = new TableRow();
                        celdaDatos = new TableCell();
                        celdaDatos.Text = "&nbsp;";
                        celdaDatos.Attributes.Add("text-align", "center");
                        fila.Cells.Add(celdaDatos);

                        //Nombre del insumo:
                        celdaDatos = new TableCell();
                        celdaDatos.Text = row_RsInsumo["nomInsumo"].ToString();
                        celdaDatos.Attributes.Add("align", "left");
                        celdaDatos.Attributes.Add("valign", "middle");
                        fila.Cells.Add(celdaDatos);

                        //Unidad:
                        celdaDatos = new TableCell();
                        celdaDatos.Text = row_RsInsumo["Unidad"].ToString();
                        celdaDatos.Attributes.Add("valign", "middle");
                        fila.Cells.Add(celdaDatos);

                        //Cantidad:
                        celdaDatos = new TableCell();
                        celdaDatos.Text = row_RsInsumo["Cantidad"].ToString();
                        celdaDatos.Attributes.Add("align", "right");
                        fila.Cells.Add(celdaDatos);

                        //Presentación:
                        celdaDatos = new TableCell();
                        celdaDatos.Text = row_RsInsumo["Presentacion"].ToString();
                        celdaDatos.Attributes.Add("align", "right");
                        fila.Cells.Add(celdaDatos);

                        //Desperdicio:
                        celdaDatos = new TableCell();
                        celdaDatos.Text = row_RsInsumo["Desperdicio"].ToString();
                        celdaDatos.Attributes.Add("align", "right");
                        fila.Cells.Add(celdaDatos);
                        DynamicTable.Rows.Add(fila);

                        #endregion
                    }

                    #endregion

                    #region Generar fila separadora.

                    //fila = new TableRow();
                    //fila.Attributes.Add("colspan", "4");
                    //celdaDatos = new TableCell();
                    //celdaDatos.Text = "&nbsp;";
                    //fila.Cells.Add(celdaDatos);
                    //DynamicTable.Rows.Add(fila);

                    Label lbl = new Label();
                    lbl.ID = "lbl_" + incr.ToString();
                    lbl.Text = "&nbsp;";
                    pnl_tablas.Controls.Add(lbl);

                    #endregion

                    //Añadir la tabla dinámica al panel.
                    pnl_tablas.Controls.Add(DynamicTable);
                    DynamicTable.DataBind();

                    #endregion

                    //Al terminar este foreach, debe incrementar...
                    incr++;
                }
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
        /// 06/06/2014.
        /// Obtener la información acerca de la última actualización realizada, ási como la habilitación del 
        /// CheckBox para el usuario dependiendo de su grupo / rol.
        /// </summary>
        private void ObtenerDatosUltimaActualizacion()
        {
            //Inicializar variables.
            DateTime fecha = new DateTime();
            DataTable tabla = new DataTable();
            bool bRealizado = false;
            Int32 numPostIt = 0;
            Int32 CodigoEstado = 0;

            try
            {
                //Consultar si es miembro.
                esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

                //Obtener número "numPostIt".
                numPostIt = Obtener_numPostIt();

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(txtTab.ToString(), codProyecto, codConvocatoria);

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + codProyecto + " And CodContacto = " + usuario.IdContacto +
                         " and inactivo=0 and FechaInicio<=getdate() and FechaFin is null ";

                //Asignar variables a DataTable.
                var rs = consultas.ObtenerDataTable(txtSQL, "text");

                //Crear la variable de sesión.
                if (rs.Rows.Count > 0) { Session["CodRol"] = rs.Rows[0]["CodRol"].ToString(); }
                else { Session["CodRol"] = ""; }

                //Destruir la variable.
                rs = null;

                #endregion

                //Consultar los datos a mostrar en los campos correspondientes a la actualización.
                txtSQL = " SELECT Nombres + ' ' + Apellidos AS nombre, FechaModificacion, Realizado " +
                         " FROM TabProyecto, Contacto " +
                         " where Id_Contacto = CodContacto AND CodTab = " + txtTab +
                         " AND CodProyecto = " + codProyecto;

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

                    //Valor "bRealziado".
                    bRealizado = Convert.ToBoolean(tabla.Rows[0]["Realizado"].ToString());
                }

                //Asignar check de acuerdo al valor obtenido en "bRealizado".
                chk_realizado.Checked = bRealizado;

                //Determinar si el usuario actual puede o no "chequear" la actualización.
                //if (!(EsMiembro && numPostIt == 0 && ((usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && usuario.CodGrupo == Constantes.CONST_RolEvaluador && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                if (!(esMiembro && numPostIt == 0 && ((Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                { chk_realizado.Enabled = false; }

                //Mostrar el botón de guardar.
                //if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (usuario.CodGrupo == Constantes.CONST_RolEvaluador && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
                if (esMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                //Mostrar los enlaces para adjuntar documentos.
                if (esMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolEmprendedor.ToString() && !bRealizado)
                {
                    tabla_docs.Visible = true;
                }

                //Destruir variables.
                tabla = null;
                txtSQL = null;
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
                //Destruir variables.
                //tabla = null;
                //txtSQL = null;
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/06/2014.
        /// Guardar la información "Ultima Actualización".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_guardar_ultima_actualizacion_Click(object sender, EventArgs e)
        { prActualizarTab(txtTab.ToString(), codProyecto.ToString()); Marcar(txtTab.ToString(), codProyecto, "", chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); GenerarTablaInsumos(); }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codProyecto;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Nuevo";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codProyecto;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Vista";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        #endregion
    }
}