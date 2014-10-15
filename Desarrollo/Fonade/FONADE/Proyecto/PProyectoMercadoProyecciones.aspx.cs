using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.FONADE.Proyecto
{
    public partial class PProyectoMercadoProyecciones : Negocio.Base_Page
    {
        public String codProyecto;
        public int txtTab = Constantes.CONST_ProyeccionesVentas;
        public String codConvocatoria;
        private ProyectoMercadoProyeccionVenta pm;

        String txtCodPeriodo;
        /// <summary>
        /// Tiempo de proyección "numAnios".
        /// </summary>
        Int32 txtTiempoProyeccion;
        String txtMetodoProyeccion;
        String txtCostoVenta;

        /// <summary>
        /// Variable que contiene las consultas SQL.
        /// </summary>
        String txtSQL;
        Boolean esMiembro;
        Boolean bTiempo;
        /// <summary>
        /// Consultar si el proyecto está o no realizado...
        /// </summary>
        Boolean bRealizado;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            codConvocatoria = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["CodConvocatoria"].ToString() : "0";

            pm = getProyectoMercadoProyeccionVenta();

            if (pm == null) { return; }

            if (!IsPostBack)
            {
                TB_CostoVenta.MaxLength = 100;

                //Consultar datos.
                CargarPeriodos();
                definirCampos();
                CargarProyeccionesDeVentas();
                ObtenerDatosUltimaActualizacion();

                //Consultar si es miembro.
                esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

                //Consultar si está "realizado".
                bRealizado = esRealizado(txtTab.ToString(), codProyecto, codConvocatoria);

                if (esMiembro && !bRealizado)
                {
                    this.div_post_it_1.Visible = true;
                    this.div_post_it_2.Visible = true;
                    this.div_post_it_3.Visible = true;
                }

                if (!(esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor) || bRealizado)
                {
                    DDL_Dia.Enabled = false;
                    DDL_Mes.Enabled = false;
                    DD_Anio.Enabled = false;
                    DD_MetProy.Enabled = false;
                    DD_Periodo.Enabled = false;
                    DropDownList1.Enabled = false;
                    TB_CostoVenta.Enabled = false;
                    B_Guardar.Visible = false;
                }

                if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor)
                {
                    if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !chk_realizado.Checked && bTiempo)
                    {
                        IB_AgregarProductoServicio.Visible = true;
                        B_AgregarProductoServicio.Visible = true;
                    }
                }

                try
                {
                    llenarGridView();
                    #region Llamada a métodos COMENTADOS.
                    //llenarGridView2();
                    //llenarGridView3(); 
                    #endregion

                    //Nuevos métodos.
                    Tabla_VentasUnidades();
                    Tabla_IngresosVenta();

                    if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado && bTiempo)
                        GV_productoServicio.Columns[9].Visible = true;
                    else
                        GV_productoServicio.Columns[9].Visible = false;
                }
                catch { }
            }
        }

        private Boolean Validar()
        {
            Boolean resultado = true;


            if (txtTiempoProyeccion.ToString() != DropDownList1.SelectedValue || txtCodPeriodo != DD_Periodo.SelectedValue)
            {
                var resul = System.Windows.Forms.MessageBox.Show("Si cambia el periodo de proyección y/o el tamaño del periodo se borraran las proyecciones de ventas para los productos actuales.  Esta seguro de realizar este cambio?", "Advrtencia", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (resul == System.Windows.Forms.DialogResult.No)
                {
                    resultado = true;
                }
                else
                {
                    resultado = false;
                }
            }

            return resultado;
        }

        protected void B_Guardar_Click(object sender, EventArgs e)
        {

            if (bTiempo == true)
            {
                if (Validar() == true)
                { return; }
            }

            String FechaInicioDia = DDL_Dia.SelectedValue;
            String FechainicioMes = DDL_Mes.SelectedValue;
            String FechaInicioAnio = DD_Anio.SelectedValue;

            String periodo = DD_Periodo.SelectedValue;
            String tiempo = DropDownList1.SelectedValue;

            String metodo = DD_MetProy.SelectedValue;

            String costoVenta = TB_CostoVenta.Text;

            String JusProVentas = TB_JusProVen.Text;

            String PoliCarte = TB_PoliCarte.Text;

            Int32 valor;

            String sql;
            sql = "SELECT COUNT(*) as resul FROM [Fonade].[dbo].[ProyectoMercadoProyeccionVentas] WHERE [codproyecto] = " + codProyecto;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                valor = Int32.Parse(reader["resul"].ToString());
                conn.Close();


                string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                using (var con = new SqlConnection(conexionStr))
                {
                    using (var com = con.CreateCommand())
                    {
                        com.CommandText = "MD_Insertar_Actualizar_ProyectoMercadoProyeccionVentas";
                        com.CommandType = System.Data.CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@_CodProyecto", codProyecto);
                        com.Parameters.AddWithValue("@_FechaArranque", FechaInicioAnio + "-" + FechainicioMes + "-" + FechaInicioDia);
                        com.Parameters.AddWithValue("@_CodPeriodo", periodo);
                        com.Parameters.AddWithValue("@_TiempoProyeccion", tiempo);
                        com.Parameters.AddWithValue("@_MetodoProyeccion", metodo);
                        com.Parameters.AddWithValue("@_PoliticaCartera", PoliCarte);
                        com.Parameters.AddWithValue("@_CostoVenta", costoVenta);
                        com.Parameters.AddWithValue("@_justificacion", JusProVentas);

                        if (valor > 0) com.Parameters.AddWithValue("@_caso", "UPDATE");
                        else com.Parameters.AddWithValue("@_caso", "CREATE");
                        // Validar que no guarde espacios en blanco
                        try
                        {
                            con.Open();
                            com.ExecuteReader();
                            //Actualizar fecha modificación del tab.
                            prActualizarTab(txtTab.ToString(), codProyecto);
                            ObtenerDatosUltimaActualizacion();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }

                reader.Close();
            }
            catch (SqlException se)
            {
                throw se;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// LinkButton "Agregar Producto o Servicio".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_AgregarProductoServicio_Click(object sender, EventArgs e)
        {
            Session["OpcionMercadoProyecciones"] = "agregar";
            Response.Redirect("~/FONADE/evaluacion/CatalogoProducto.aspx");
        }

        /// <summary>
        /// Llenar grilla "GV_productoServicio".
        /// </summary>
        private void llenarGridView()
        {
            //Inicializar variables.
            DataTable tabla = new System.Data.DataTable();

            try
            {
                txtSQL = "SELECT * FROM ProyectoProducto WHERE CodProyecto = " + codProyecto;
                tabla = consultas.ObtenerDataTable(txtSQL, "text");
                GV_productoServicio.DataSource = tabla;
                GV_productoServicio.DataBind();
                tabla = null;
                txtSQL = null;
            }
            catch { tabla = null; txtSQL = null; }
        }

        #region MÉTODOS COMENTADOS
        ///// <summary>
        ///// Crear DataTable "para ser asignada a tabla_interna".
        ///// </summary>
        ///// <returns>tabla_interna</returns>
        //private DataTable crearDatatable()
        //{
        //    #region Comentarios.
        //    //DataTable mini_tabla = new DataTable();

        //    //mini_tabla.Columns.Add("NomProducto");
        //    //mini_tabla.Columns.Add("anio1");
        //    //mini_tabla.Columns.Add("anio2");
        //    //mini_tabla.Columns.Add("anio3");

        //    //return mini_tabla; 
        //    #endregion

        //    DataTable tabla_interna = new System.Data.DataTable();
        //    Int32 int_txtTiempoProyeccion = 0;

        //    try
        //    {
        //        int_txtTiempoProyeccion = Int32.Parse(txtTiempoProyeccion);
        //        tabla_interna.Columns.Add("NomProducto", typeof(System.String));

        //        for (int i = 1; i < int_txtTiempoProyeccion + 1; i++)
        //        { tabla_interna.Columns.Add("Año " + i.ToString(), typeof(System.Double)); }

        //        //tabla_interna.Columns.Add("Unidades", typeof(System.String));

        //        return tabla_interna;
        //    }
        //    catch { return new System.Data.DataTable(); }
        //}

        ///// <summary>
        ///// Consultar los productos.
        ///// </summary>
        ///// <returns>dt</returns>
        //private DataTable resultadoProyeccionVentas()
        //{
        //    //Inicializar variables.
        //    DataTable dt = new DataTable();

        //    try
        //    {
        //        txtSQL = "SELECT * FROM ProyectoProducto WHERE CodProyecto = " + codProyecto;

        //        dt = consultas.ObtenerDataTable(txtSQL, "text");

        //        if (dt.Rows.Count > 0)
        //        {
        //            L_ProyeccinVentas.Visible = true;
        //            Label2.Visible = true;
        //            return dt;
        //        }
        //        else
        //        {
        //            L_ProyeccinVentas.Visible = false;
        //            Label2.Visible = false;
        //            return dt;
        //        }
        //    }
        //    catch { return dt; }
        //}

        ///// <summary>
        ///// Consultar la información basada en proyecciones de ingresos.
        ///// </summary>
        ///// <returns>dt</returns>
        //private DataTable resultadoProyeccionIngresos()
        //{
        //    //Inicializar variables.
        //    DataTable dt = new DataTable();

        //    try
        //    {
        //        txtSQL = " select id_producto, nomproducto, porcentajeiva " +
        //                 " from proyectoproducto where codproyecto = " + codProyecto;

        //        dt = consultas.ObtenerDataTable(txtSQL, "text");

        //        return dt;
        //    }
        //    catch { return new System.Data.DataTable(); }
        //}

        ///// <summary>
        ///// Mauricio Arias Olave.
        ///// 06/06/2014.
        ///// Llenar GridView "GV_ProyeccionVentas1" = "Proyección de Ventas (Unidades)".
        ///// </summary>
        //private void llenarGridView2()
        //{
        //    DataTable tabla_interna = crearDatatable();
        //    DataTable mini = new System.Data.DataTable();
        //    DataTable tabla_final = new System.Data.DataTable();

        //    try
        //    {
        //        //Traer los resultados de las proyecciones.
        //        DataTable dt = resultadoProyeccionVentas();

        //        //Cambiar el nombre de la fila a mostrar.
        //        tabla_interna.Columns[0].ColumnName = "Producto o Servicio";

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            //Fila que contiene las columnas "NomProducto", "Año 1" - "Año ...N".
        //            DataRow fila = tabla_interna.NewRow();

        //            //Agregar en la variable "fila" el valor cargado en "dr".
        //            fila["Producto o Servicio"] = dr["NomProducto"].ToString();

        //            //Consultar valores anuales.
        //            txtSQL = " select sum(unidades) as unidades, Ano from proyectoproductounidadesventas " +
        //                     " where codproducto = " + dr["Id_Producto"].ToString() + " group by ano order by ano";

        //            //Asignar resultados de la consulta interna.
        //            //Contiene las columnas
        //            //"unidades" y "Ano".
        //            mini = consultas.ObtenerDataTable(txtSQL, "text");

        //            //Recorrer las filas de la tabla "mini" para generar los datos.
        //            foreach (DataRow row in mini.Rows)
        //            {
        //                //Añadir el valor "unidades" al año correspondiente.
        //                fila["Año " + row["Ano"].ToString()] = row["unidades"].ToString();
        //            }

        //            //Adicionar la fila generada con los valores.
        //            tabla_interna.Rows.Add(fila);
        //        }

        //        //Bindear la grilla.
        //        GV_ProyeccionVentas1.DataSource = tabla_interna; //tabla_final;
        //        GV_ProyeccionVentas1.DataBind();

        //        //Destruir variables.
        //        tabla_final = null;
        //        tabla_interna = null;
        //        mini = null;
        //    }
        //    catch
        //    {
        //        //Destruir variables.
        //        tabla_final = null;
        //        tabla_interna = null;
        //        mini = null;
        //    }
        //}

        ///// <summary>
        ///// Mauricio Arias Olave.
        ///// 06/06/2014.
        ///// Llenar GridView "GV_ProyeccionVentas2" = "Proyección de Ingresos por Ventas".
        ///// </summary>
        //private void llenarGridView3()
        //{
        //    DataTable tabla_interna = crearDatatable();
        //    DataTable mini = new System.Data.DataTable();
        //    DataTable tabla_final = new System.Data.DataTable();

        //    try
        //    {
        //        //Traer los resultados de las proyecciones.
        //        DataTable dt = resultadoProyeccionIngresos();

        //        //Cambiar el nombre de la fila a mostrar.
        //        tabla_interna.Columns[0].ColumnName = "Producto o Servicio";

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            //Fila que contiene las columnas "NomProducto", "Año 1" - "Año ...N".
        //            DataRow fila = tabla_interna.NewRow();

        //            //Agregar en la variable "fila" el valor cargado en "dr".
        //            fila["Producto o Servicio"] = dr["NomProducto"].ToString();

        //            //Consultar valores anuales.
        //            txtSQL = " select sum(unidades) as unidades, precio, sum(unidades) * precio as total, ano " +
        //                     " from proyectoproductounidadesventas u, proyectoproductoprecio p " +
        //                     " where p.codproducto=u.codproducto and periodo = ano " +
        //                     " and p.codproducto = " + dr["Id_Producto"].ToString() +
        //                     " group by ano,precio order by ano";

        //            //Asignar resultados de la consulta interna.
        //            //Contiene las columnas
        //            //"total" y "Ano".
        //            mini = consultas.ObtenerDataTable(txtSQL, "text");

        //            //Recorrer las filas de la tabla "mini" para generar los datos.
        //            foreach (DataRow row in mini.Rows)
        //            {
        //                //Añadir el valor "total" al año correspondiente.
        //                fila["Año " + row["Ano"].ToString()] = row["total"].ToString();
        //            }

        //            //Adicionar la fila generada con los valores.
        //            tabla_interna.Rows.Add(fila);
        //        }

        //        //Bindear la grilla.
        //        GV_ProyeccionVentas2.DataSource = tabla_interna; //tabla_final;
        //        GV_ProyeccionVentas2.DataBind();

        //        //Destruir variables.
        //        tabla_final = null;
        //        tabla_interna = null;
        //        mini = null;
        //    }
        //    catch
        //    {
        //        //Destruir variables.
        //        tabla_final = null;
        //        tabla_interna = null;
        //        mini = null;
        //    }
        //}

        //-- 
        #endregion

        protected void LB_Insumo_Click(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GVInventario = GV_productoServicio.Rows[indicefila];

            Session["Id_Producto"] = GV_productoServicio.DataKeys[GVInventario.RowIndex].Value.ToString();
            Session["Insumo"] = 0;
            Session["CodProyecto"] = codProyecto;

            Response.Redirect("~/FONADE/Convocatoria/CatalogoInsumo.aspx");
        }

        /// <summary>
        /// ImageButton "Agregar Producto o Servicio".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void IB_AgregarProductoServicio_Click(object sender, ImageClickEventArgs e)
        {
            Session["OpcionMercadoProyecciones"] = "agregar";
            Response.Redirect("~/FONADE/evaluacion/CatalogoProducto.aspx");
        }

        /// <summary>
        /// Evento que detecta cual fue el LinButton activado para luego obtener la información que será
        /// creada en variables de sesión y redirigiá finalmente a la página "~/FONADE/evaluacion/CatalogoProducto.aspx".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LB_ProductoServicio_Click(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GVInventario = GV_productoServicio.Rows[indicefila];

            String Id_Producto = GV_productoServicio.DataKeys[GVInventario.RowIndex].Value.ToString();

            Session["OpcionMercadoProyecciones"] = "actualizar";
            Session["valordeId_Producto"] = Id_Producto;
            Response.Redirect("~/FONADE/evaluacion/CatalogoProducto.aspx");
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            try
            {
                //var resul = System.Windows.Forms.MessageBox.Show("Esta seguro que desea borrar el producto seleccionado?", "Advrtencia", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                //if (resul == System.Windows.Forms.DialogResult.Yes)
                //{
                var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
                GridViewRow GVInventario = GV_productoServicio.Rows[indicefila];

                String Id_Producto = GV_productoServicio.DataKeys[GVInventario.RowIndex].Value.ToString();

                SqlCommand cmd;
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

                cmd = new SqlCommand("DELETE FROM [Fonade].[dbo].[ProyectoProducto] WHERE [Id_Producto] = " + Id_Producto, conn);

                try
                {

                    conn.Open();
                    cmd.ExecuteReader();
                    conn.Close();
                    ObtenerDatosUltimaActualizacion();
                }
                catch (SqlException se)
                {
                    throw se;
                }
                finally
                {
                    conn.Close();
                }
                //}
            }
            catch (Exception exs) { throw exs; }
        }

        private void enviar()
        {
            ClientScriptManager cm = this.ClientScript;
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>open('../Ayuda/Mensaje.aspx', 'Proyección de ventas', 'width=500,height=400');</script>");
        }

        protected void I_AyudaProVentas_Click(object sender, ImageClickEventArgs e)
        {
            Session["mensaje"] = "1"; enviar();
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Session["mensaje"] = "2"; enviar();
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Session["mensaje"] = "3"; enviar();
        }

        /// <summary>
        /// RowDataBound para generar "o no" los controles para adminsitrar la información que
        /// se visualiza en el GridView "GV_productoServicio".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GV_productoServicio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lnk = e.Row.FindControl("LinkButton1") as LinkButton;
                var img = e.Row.FindControl("I_imagen") as Image;
                var lnk_nmb = e.Row.FindControl("LB_ProductoServicio") as LinkButton;
                var lnk_insumo = e.Row.FindControl("LB_Insumo") as LinkButton;

                if (lnk != null && img != null && lnk_nmb != null && lnk_insumo != null)
                {
                    if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
                    {
                        lnk.Visible = true;
                        img.Visible = true;
                        lnk_nmb.Visible = true;
                        lnk_insumo.Visible = true;
                    }
                    else
                    {
                        lnk.Visible = false;
                        img.Visible = false;
                        lnk_nmb.Enabled = false;
                        lnk_nmb.ForeColor = System.Drawing.Color.Black;
                        lnk_nmb.Style.Add("text-decoration", "none");
                        lnk_insumo.Visible = false;
                    }
                }
            }
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
        /// Obtener la información acerca de la última actualización realizada, ási como la habilitación del 
        /// CheckBox para el usuario dependiendo de su grupo / rol.
        /// </summary>
        private void ObtenerDatosUltimaActualizacion()
        {
            //Inicializar variables.
            String txtSQL;
            DateTime fecha = new DateTime();
            DataTable tabla = new DataTable();
            bool bRealizado = false;
            bool EsMiembro = false;
            Int32 numPostIt = 0;
            Int32 CodigoEstado = 0;

            try
            {
                //Consultar si es miembro.
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

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
                if (!(EsMiembro && numPostIt == 0 && ((Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (CodigoEstado == Constantes.CONST_Evaluacion && Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && es_bNuevo(codProyecto)))) || lbl_nombre_user_ult_act.Text.Trim() == "")
                { chk_realizado.Enabled = false; }

                //Mostrar el botón de guardar.
                //if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (usuario.CodGrupo == Constantes.CONST_RolAsesorLider && CodigoEstado == Constantes.CONST_Inscripcion) || (usuario.CodGrupo == Constantes.CONST_RolEvaluador && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
                if (EsMiembro && numPostIt == 0 && lbl_nombre_user_ult_act.Text != "" && (Session["CodRol"].ToString() == Constantes.CONST_RolAsesorLider.ToString() && CodigoEstado == Constantes.CONST_Inscripcion) || (Session["CodRol"].ToString() == Constantes.CONST_RolEvaluador.ToString() && CodigoEstado == Constantes.CONST_Evaluacion && es_bNuevo(codProyecto)))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                //Mostrar los enlaces para adjuntar documentos.
                if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolEmprendedor.ToString() && !bRealizado)
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
                //return;
            }
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
        /// Cargar los periodos "DD_Periodo" y  "DropDownList1".
        /// </summary>
        private void CargarPeriodos()
        {
            //Inicializar variables.
            DataTable tabla = new System.Data.DataTable();

            try
            {
                DD_Periodo.Items.Clear();
                DropDownList1.Items.Clear();

                txtSQL = " SELECT Id_Periodo, NomPeriodo FROM Periodo";
                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                //DD_Periodo.
                foreach (DataRow row in tabla.Rows)
                {
                    ListItem item = new ListItem();
                    item.Value = row["Id_Periodo"].ToString();
                    item.Text = row["NomPeriodo"].ToString();
                    DD_Periodo.Items.Add(item);
                }

                txtSQL = null;
                tabla = null;

                txtSQL = " SELECT TiempoProyeccion FROM ProyectoMercadoProyeccionVentas WHERE CodProyecto = " + codProyecto;
                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                //DropDownList1.
                for (int i = 0; i < Convert.ToInt32(tabla.Rows[0]["TiempoProyeccion"].ToString()) + 1; i++)
                {
                    ListItem item = new ListItem();
                    item.Value = i.ToString();
                    item.Text = i.ToString();
                    DropDownList1.Items.Add(item);
                }
            }
            catch
            {
                DD_Periodo.Items.Clear();
                List<string> valores = new List<string>();
                valores.Add("Mes");
                valores.Add("Bimestre");
                valores.Add("Trimestre");
                valores.Add("Semestre");
                for (int i = 1; i < 6; i++)
                {
                    ListItem item = new ListItem();
                    item.Value = i.ToString();
                    item.Text = valores[i];
                    DD_Periodo.Items.Add(item);

                    ListItem item_s = new ListItem();
                    item_s.Value = i.ToString();
                    item_s.Text = i.ToString();
                    DropDownList1.Items.Add(item_s);
                }
                valores = null;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 06/06/2014.
        /// Cargar la información de la ventana "Proyecciones de Ventas".
        /// </summary>
        private void CargarProyeccionesDeVentas()
        {
            //Inicializar variables.
            DataTable tabla = new System.Data.DataTable();
            try
            {
                txtSQL = "SELECT * FROM ProyectoMercadoProyeccionVentas WHERE CodProyecto = " + codProyecto;
                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                if (tabla.Rows.Count > 0)
                {
                    //Inicializar variables.
                    DateTime fecha_arranque = new DateTime();
                    String tiempo_proyeccion = "";

                    DD_Periodo.SelectedValue = tabla.Rows[0]["CodPeriodo"].ToString();

                    foreach (ListItem item in DropDownList1.Items)
                    {
                        tiempo_proyeccion = tabla.Rows[0]["TiempoProyeccion"].ToString();
                        if (item.Value == tiempo_proyeccion)
                        { item.Selected = true; break; }
                    }
                    DD_MetProy.SelectedValue = tabla.Rows[0]["MetodoProyeccion"].ToString();

                    //Consultar la fecha.
                    try { fecha_arranque = Convert.ToDateTime(tabla.Rows[0]["FechaArranque"].ToString()); }
                    catch { fecha_arranque = DateTime.Today; }

                    //Establecer selección de fecha de arranque.
                    DDL_Dia.SelectedValue = fecha_arranque.Day.ToString();
                    DDL_Mes.SelectedValue = fecha_arranque.Month.ToString();
                    DD_Anio.SelectedValue = fecha_arranque.Year.ToString();

                    TB_CostoVenta.Text = tabla.Rows[0]["CostoVenta"].ToString();

                    txtCodPeriodo = tabla.Rows[0]["CodPeriodo"].ToString();
                    txtTiempoProyeccion = Convert.ToInt32(tabla.Rows[0]["TiempoProyeccion"].ToString());
                    txtMetodoProyeccion = tabla.Rows[0]["MetodoProyeccion"].ToString();
                    txtCostoVenta = tabla.Rows[0]["CostoVenta"].ToString();
                    bTiempo = true;
                }
                else
                { bTiempo = false; }
            }
            catch { bTiempo = false; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 09/06/2014.
        /// Cargar el valor "numAnios" en la variable global y de sesión llamadas "int_txtTiempoProyeccion".
        /// </summary>
        /// <returns>int_txtTiempoProyeccion / numAnios</returns>
        private Int32 Cargar_numAnios()
        {
            //Inicializar variables.
            Int32 int_txtTiempoProyeccion = 0;
            try
            {
                txtSQL = " SELECT TiempoProyeccion FROM ProyectoMercadoProyeccionVentas WHERE codProyecto = " + codProyecto;

                var dt = consultas.ObtenerDataTable(txtSQL, "text");

                if (dt.Rows.Count > 0)
                {
                    int_txtTiempoProyeccion = Int32.Parse(dt.Rows[0]["TiempoProyeccion"].ToString());
                    dt = null;
                    txtSQL = null;
                    Session["int_txtTiempoProyeccion"] = int_txtTiempoProyeccion;
                    txtTiempoProyeccion = int_txtTiempoProyeccion;
                    return int_txtTiempoProyeccion;
                }
                else
                {
                    dt = null;
                    txtSQL = null;
                    Session["int_txtTiempoProyeccion"] = int_txtTiempoProyeccion;
                    txtTiempoProyeccion = int_txtTiempoProyeccion;
                    return int_txtTiempoProyeccion;
                }
            }
            catch { Session["int_txtTiempoProyeccion"] = int_txtTiempoProyeccion; txtTiempoProyeccion = int_txtTiempoProyeccion; return int_txtTiempoProyeccion; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/06/2014.
        /// Cargar tabla "Proyección de Ventas (Unidades)".
        /// </summary>
        private void Tabla_VentasUnidades()
        {
            //Inicializar variables.
            Table Tabla_Unidades = new Table();
            Tabla_Unidades.CssClass = "Grilla";
            Tabla_Unidades.Attributes.Add("width", "100%");
            Tabla_Unidades.Attributes.Add("cellspacing", "1");
            TableCell celdaEncabezado = new TableCell();
            TableCell celdaDatos = new TableCell();
            TableCell celdaDatosda = new TableCell();
            TableHeaderRow fila1 = new TableHeaderRow();
            TableRow fila = new TableRow();
            DataTable rsProducto = new System.Data.DataTable();
            DataTable rsUnidades = new System.Data.DataTable();

            try
            {
                #region Generar fila "Proyección de Ventas (Unidades)".

                fila1 = new TableHeaderRow();
                fila1.Attributes.Add("bgcolor", "#3D5A87");
                fila1.Attributes.Add("align", "center");
                celdaEncabezado = new TableHeaderCell();
                celdaEncabezado.Attributes.Add("colspan", "6");
                celdaEncabezado.Attributes.Add("color", "white");
                celdaEncabezado.Style.Add("text-align", "center");
                celdaEncabezado.Text = "Proyección de Ventas (Unidades)";
                fila1.Cells.Add(celdaEncabezado);
                Tabla_Unidades.Rows.Add(fila1);

                #endregion

                #region Generar encabezados de "Producto o Servicio" y las celdas de Años.

                fila1 = new TableHeaderRow();
                celdaEncabezado = new TableHeaderCell();
                celdaEncabezado.Attributes.Add("text-align", "left");
                celdaEncabezado.Text = "Producto o Servicio";
                fila1.Cells.Add(celdaEncabezado);

                for (int i = 1; i < Cargar_numAnios() + 1; i++)
                {
                    celdaEncabezado = new TableHeaderCell();
                    celdaEncabezado.Attributes.Add("text-align", "left");
                    celdaEncabezado.Text = "Año " + i.ToString();
                    fila1.Cells.Add(celdaEncabezado);
                }
                Tabla_Unidades.Rows.Add(fila1);

                #endregion

                //Consultar productos.
                txtSQL = " select id_producto, nomproducto from proyectoproducto where codproyecto = " + codProyecto;

                //Asignar resultados a variable DataTable.
                rsProducto = consultas.ObtenerDataTable(txtSQL, "text");

                //Generar filas.
                foreach (DataRow row_rsProducto in rsProducto.Rows)
                {
                    #region Inicializar la fila con la primera celda "Nombre del producto".
                    //Inicializar fila.
                    fila = new TableHeaderRow();
                    fila.Attributes.Add("bgcolor", "#3D5A87");
                    //Inicializar la celda del encabezado.
                    celdaDatos = new TableCell();
                    celdaDatos.Attributes.Add("text-align", "left");
                    //Continuar con la generación de la celda.
                    celdaDatos.Text = row_rsProducto["nomproducto"].ToString();
                    //Agregar la celda a la fila.
                    fila.Cells.Add(celdaDatos);
                    #endregion

                    #region Consultar y asignar los resultados a la variable "rsUnidades".
                    txtSQL = " select sum(unidades) as unidades,ano from proyectoproductounidadesventas " +
                                         " where codproducto = " + row_rsProducto["id_Producto"].ToString() +
                                         " group by ano order by ano ";
                    rsUnidades = consultas.ObtenerDataTable(txtSQL, "text");
                    #endregion

                    #region Generar las celdas con las unidades.
                    foreach (DataRow row_rsUnidades in rsUnidades.Rows)
                    {
                        //<td align='right'>"&rsUnidades("unidades")&"</td>"&VbCrLf
                        //Inicializar la celda.
                        celdaDatosda = new TableCell();
                        celdaDatosda.Attributes.Add("align", "right");
                        celdaDatosda.Style.Add("text-align", "right");
                        //Continuar con la generación de la celda.
                        celdaDatosda.Text = row_rsUnidades["unidades"].ToString();
                        //Añadir la celda a la fila.
                        fila.Cells.Add(celdaDatosda);
                    }
                    #endregion

                    //Agregar la fila a la tabla.
                    Tabla_Unidades.Rows.Add(fila);
                }

                //Agregar la tabla.
                pnl_Datos.Controls.Add(Tabla_Unidades);

                #region Destruir variables.

                Tabla_Unidades = null;
                celdaEncabezado = null;
                celdaDatos = null;
                celdaDatosda = null;
                fila1 = null;
                fila = null;
                rsProducto = null;
                rsUnidades = null;

                #endregion
            }
            catch
            {
                #region Destruir variables.

                Tabla_Unidades = null;
                celdaEncabezado = null;
                celdaDatos = null;
                celdaDatosda = null;
                fila1 = null;
                fila = null;
                rsProducto = null;
                rsUnidades = null;

                #endregion
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/06/2014.
        /// Cargar tabla "Proyección de Ingresos por Ventas".
        /// </summary>
        private void Tabla_IngresosVenta()
        {
            //Inicializar variables.
            Table Tabla_Unidades = new Table();
            Tabla_Unidades.CssClass = "Grilla";
            Tabla_Unidades.Attributes.Add("width", "100%");
            Tabla_Unidades.Attributes.Add("cellspacing", "1");
            TableCell celdaEncabezado = new TableCell();
            TableCell celdaDatos = new TableCell();
            TableCell celdaDatosda = new TableCell();
            TableCell celdaEspecial = new TableCell();
            TableHeaderRow fila1 = new TableHeaderRow();
            TableRow fila = new TableRow();
            DataTable rsProducto = new System.Data.DataTable();
            DataTable rsUnidades = new System.Data.DataTable();
            Double[] TotalPt = new Double[10];
            Double[] TotalIvaPt = new Double[10];
            Label lbl = new Label();
            lbl.ID = "lbl_b";
            lbl.Text = "<br/><br/>";
            pnl_Datos.Controls.Add(lbl);
            lbl = null;
            String[] arr_totales = { "&nbsp;", "Total", "IVA", "Total mas IVA" };

            try
            {
                #region Generar fila "Proyección de Ingresos por Ventas".

                fila1 = new TableHeaderRow();
                fila1.Attributes.Add("bgcolor", "#3D5A87");
                fila1.Attributes.Add("align", "center");
                celdaEncabezado = new TableHeaderCell();
                celdaEncabezado.Attributes.Add("colspan", "6");
                celdaEncabezado.Attributes.Add("color", "white");
                celdaEncabezado.Style.Add("text-align", "center");
                celdaEncabezado.Text = "Proyección de Ingresos por Ventas";
                fila1.Cells.Add(celdaEncabezado);
                Tabla_Unidades.Rows.Add(fila1);

                #endregion

                #region Generar encabezados de "Producto o Servicio" y las celdas de Años.

                fila1 = new TableHeaderRow();
                celdaEncabezado = new TableHeaderCell();
                celdaEncabezado.Attributes.Add("text-align", "left");
                celdaEncabezado.Text = "Producto o Servicio";
                fila1.Cells.Add(celdaEncabezado);

                for (int i = 1; i < Cargar_numAnios() + 1; i++)
                {
                    celdaEncabezado = new TableHeaderCell();
                    celdaEncabezado.Attributes.Add("text-align", "left");
                    celdaEncabezado.Text = "Año " + i.ToString();
                    fila1.Cells.Add(celdaEncabezado);
                }
                Tabla_Unidades.Rows.Add(fila1);

                #endregion

                //Consultar productos.
                txtSQL = " select id_producto, nomproducto, porcentajeiva from proyectoproducto where codproyecto = " + codProyecto;

                //Asignar resultados a variable DataTable.
                rsProducto = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow row_rsProducto in rsProducto.Rows)
                {
                    #region Consultar y asignar resultados a la variable "rsUnidades".
                    txtSQL = " select sum(unidades) as unidades, precio, sum(unidades)*precio as total, ano " +
                                         " from proyectoproductounidadesventas u, proyectoproductoprecio p " +
                                         " where p.codproducto=u.codproducto and periodo=ano " +
                                         " and p.codproducto = " + row_rsProducto["id_Producto"].ToString() +
                                         " group by ano,precio order by ano";

                    rsUnidades = consultas.ObtenerDataTable(txtSQL, "text");
                    #endregion

                    foreach (DataRow rw in rsUnidades.Rows)
                    {
                        #region Generar la primera celda con el valor "Nombre del Producto".
                        //Inicializar fila.
                        fila = new TableHeaderRow();
                        fila.Attributes.Add("bgcolor", "#3D5A87");
                        //Inicializar la celda del encabezado.
                        celdaDatos = new TableCell();
                        celdaDatos.Attributes.Add("text-align", "left");
                        //Continuar con la generación de la celda.
                        celdaDatos.Text = row_rsProducto["nomproducto"].ToString();
                        //Agregar la celda a la fila.
                        fila.Cells.Add(celdaDatos);
                        #endregion

                        Int32 yyyy = Int32.Parse(rw["ano"].ToString());
                        TotalPt[yyyy] = TotalPt[yyyy] + Double.Parse(rw["total"].ToString());

                        Int32 ivas = Int32.Parse(rw["ano"].ToString());
                        TotalIvaPt[yyyy] = TotalIvaPt[yyyy] + (Double.Parse(rw["total"].ToString()) * Double.Parse(row_rsProducto["porcentajeiva"].ToString()) / 100);
                    }

                    #region Generar las celdas acerca de la multiplicación de las unidades por los precios.
                    foreach (DataRow row_rsUnidades in rsUnidades.Rows)
                    {
                        //Inicializar la celda.
                        celdaDatosda = new TableCell();
                        celdaDatosda.Attributes.Add("align", "right");
                        celdaDatosda.Style.Add("text-align", "right");
                        //Continuar con la generación de la celda.
                        celdaDatosda.Text = (Double.Parse(row_rsUnidades["unidades"].ToString()) * Double.Parse(row_rsUnidades["precio"].ToString())).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                        fila.Cells.Add(celdaDatosda);
                    }
                    #endregion

                    //Agregar la fila a la tabla.
                    Tabla_Unidades.Rows.Add(fila);
                }

                #region Generar filas de totales.

                //Recorrer la variable "arr_totates" que contiene los títulos pre-definidos.
                for (int i = 0; i < arr_totales.Count(); i++)
                {
                    #region Generar la fila única de acuerdo al valor almacenado en el arreglo "arr_totales".

                    #region Generar la primera celda de la fila.
                    fila = new TableRow();
                    celdaEspecial = new TableCell();
                    celdaEspecial.Text = "<strong>" + arr_totales[i] + "</strong>";
                    fila.Cells.Add(celdaEspecial);
                    #endregion

                    #region Establecer iteración de datos.

                    if (i == 1)
                    {
                        #region Iterar la variable "TotalPt" = Años.
                        for (int j = 1; j < txtTiempoProyeccion + 1; j++)
                        {
                            celdaEspecial = new TableCell();
                            celdaEspecial.Attributes.Add("align", "right");
                            celdaEspecial.Text = "<strong>" + TotalPt[j].ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</strong>";
                            fila.Cells.Add(celdaEspecial);
                        }
                        #endregion
                    }
                    if (i == 2)
                    {
                        #region Iterar la variable "numIVA" = IVA's.
                        for (int j = 1; j < txtTiempoProyeccion + 1; j++)
                        {
                            celdaEspecial = new TableCell();
                            celdaEspecial.Attributes.Add("align", "right");
                            celdaEspecial.Text = "<strong>" + TotalIvaPt[j].ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</strong>";
                            fila.Cells.Add(celdaEspecial);
                        }
                        #endregion
                    }
                    if (i == 3)
                    {
                        #region Iterar para sumar los valores de las variables "TotalPt" y "numIVA".
                        for (int j = 1; j < txtTiempoProyeccion + 1; j++)
                        {
                            celdaEspecial = new TableCell();
                            celdaEspecial.Attributes.Add("align", "right");
                            celdaEspecial.Text = "<strong>" + (TotalIvaPt[j] + TotalPt[j]).ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</strong>";
                            fila.Cells.Add(celdaEspecial);
                        }
                        #endregion
                    }

                    #endregion

                    //Añadir la celda generada a la fila.
                    fila.Cells.Add(celdaEspecial);

                    //Agregar fila a la tabla.
                    Tabla_Unidades.Rows.Add(fila);

                    #endregion
                }

                #endregion

                //Agregar la tabla.
                pnl_Datos.Controls.Add(Tabla_Unidades);

                #region Destruir variables.

                Tabla_Unidades = null;
                celdaEncabezado = null;
                celdaDatos = null;
                celdaDatosda = null;
                fila1 = null;
                fila = null;
                rsProducto = null;
                rsUnidades = null;

                #endregion
            }
            catch
            {
                #region Destruir variables.

                Tabla_Unidades = null;
                celdaEncabezado = null;
                celdaDatos = null;
                celdaDatosda = null;
                fila1 = null;
                fila = null;
                rsProducto = null;
                rsUnidades = null;

                #endregion
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
        { prActualizarTab(txtTab.ToString(), codProyecto.ToString()); Marcar(txtTab.ToString(), codProyecto, "", chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

        protected void ImageButton11_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codProyecto;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Nuevo";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        protected void ImageButton22_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodProyecto"] = codProyecto;
            Session["txtTab"] = txtTab.ToString();
            Session["Accion"] = "Vista";
            Redirect(null, "CatalogoDocumento.aspx", "_blank", "menubar=0,scrollbars=1,width=663,height=547,top=100");
        }

        #endregion

        /// <summary>
        /// Mostrar el valor "otro" cuando se selecciona el método de proyección.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DD_MetProy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DD_MetProy.SelectedValue == "Otro")
                td_otroMedio.Visible = true;
            else
                td_otroMedio.Visible = false;
        }

        private ProyectoMercadoProyeccionVenta getProyectoMercadoProyeccionVenta()
        {
            var query = from pmi in consultas.Db.ProyectoMercadoProyeccionVentas
                        where pmi.CodProyecto == Convert.ToInt32(codProyecto)
                        select pmi;

            return query.FirstOrDefault();
        }

        private void definirCampos()
        {
            //si encuentra el proyecto mercado investigacion
            if (pm != null)
            {
                procesarCampo(ref TB_JusProVen, ref HEE_JusProVen, ref panel_JusProVen, pm.justificacion, esMiembro, bRealizado, "");
                procesarCampo(ref TB_PoliCarte, ref HEE_PoliCarte, ref panel_PoliCarte, pm.PoliticaCartera, esMiembro, bRealizado, "");
            }

            if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado)
                B_Guardar.Visible = true;
            else
                B_Guardar.Visible = false;
        }
    }
}