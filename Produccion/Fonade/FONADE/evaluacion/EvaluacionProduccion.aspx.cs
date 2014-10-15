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

namespace Fonade.FONADE.evaluacion
{
    public partial class EvaluacionProduccion : Negocio.Base_Page
    {
        public String codProyecto;
        public int txtTab = Constantes.CONST_subProduccion;
        public String codConvocatoria;
        private ProyectoMercadoProyeccionVenta pm;
        public bool esMiembro;
        /// <summary>
        /// Indica si está o no "realizado".
        /// </summary>
        public Boolean bRealizado;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                codProyecto = Session["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;

                //codProyecto = Request.QueryString["codProyecto"].ToString();
                //Session["codProyectoval"] = codProyecto;

                //codProyecto = Session["CodProyecto"].ToString();
                codConvocatoria = Session["CodConvocatoria"].ToString();

                //codProyecto = "49472";//"45298"

                //Consultar si es miembro.
                esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto.ToString());

                //Consultar si está "realizado".
                bRealizado = esRealizado(txtTab.ToString(), codProyecto.ToString(), "");

                if (esMiembro && !bRealizado) { this.div_Post_It1.Visible = true; }

                if (!IsPostBack)
                {
                    if (usuario.CodGrupo == Constantes.CONST_GerenteEvaluador || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador)
                    {
                        this.div_Post_It1.Visible = false;
                        btn_guardar_ultima_actualizacion.Enabled = false;
                        btn_guardar_ultima_actualizacion.Visible = false;
                        chk_realizado.Enabled = false;
                    }

                    llenarGrid();
                    frameDerecho();
                    ObtenerDatosUltimaActualizacion();
                }
            }
            catch (Exception) { }
        }

        protected void I_AyudaProVentas_Click(object sender, ImageClickEventArgs e)
        {
            Session["mensaje"] = "5";
            ClientScriptManager cm = this.ClientScript;
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>open('../Ayuda/Mensaje.aspx', 'Produccion', 'width=500,height=400');</script>");
        }

        private void llenarGrid()
        {
            DataTable datatable = new DataTable();

            datatable.Columns.Add("Id_Producto");
            datatable.Columns.Add("NomProducto");

            String sql;
            sql = "SELECT [Id_Producto], [NomProducto] FROM [Fonade].[dbo].[ProyectoProducto] WHERE [CodProyecto] = " + codProyecto + " ORDER BY [Id_Producto]";
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["Id_Producto"] = reader["Id_Producto"].ToString();
                    fila["NomProducto"] = reader["NomProducto"].ToString();
                    datatable.Rows.Add(fila);
                }
                GV_ProyectoProducto.DataSource = datatable;
                GV_ProyectoProducto.DataBind();
                reader.Close();
            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }
        }

        //protected void LB_Id_Insumo_Click(object sender, EventArgs e)
        //{
        //    var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
        //    GridViewRow GVInventario = GV_ProyectoProducto.Rows[indicefila];

        //    String id_poroducto = GV_ProyectoProducto.DataKeys[GVInventario.RowIndex].Value.ToString();


        //    Session["id_productoEvaluacionProducto"] = id_poroducto;
        //    string parametros =  "'InsumoProducto.aspx', 'Proyección de ventas', 'width=400,height=200'";

        //    ScriptManager.RegisterClientScriptBlock(this, GetType(), "", "window.open("+ parametros +");",true);
        //}

        private void frameDerecho()
        {
            DataTable[] datatable = new DataTable[12];
            DataTable costoTotal = new DataTable();

            for (int i = 0; i < 12; i++)
            {
                datatable[i] = new DataTable();

                datatable[i].Columns.Add("Id_Producto");
                datatable[i].Columns.Add("Unidades");
                datatable[i].Columns.Add("PRECIO");
            }

            costoTotal.Columns.Add("Id_Producto");
            costoTotal.Columns.Add("Unidades");
            costoTotal.Columns.Add("PRECIO");

            String sql;
            sql = @"SELECT [Id_Producto], [Unidades], [Mes], ISNULL(SUM([Cantidad] * [Precio]), 0) AS PRECIO
                    FROM [Fonade].[dbo].[ProyectoProductoInsumo] AS I
                    RIGHT OUTER JOIN [Fonade].[dbo].[ProyectoInsumoPrecio] AS IP ON I.[CodInsumo] = IP.[CodInsumo]
                    LEFT OUTER JOIN [Fonade].[dbo].[ProyectoProducto] AS P ON I.[CodProducto] = P.[Id_Producto]
                    LEFT OUTER JOIN [Fonade].[dbo].[ProyectoProductoUnidadesVentas] AS U ON U.[CodProducto] = P.[Id_Producto]
                    WHERE [Periodo] = 1 AND [Ano] = 1 AND [CodProyecto] = " + codProyecto + @"
                    GROUP BY [Id_Producto], [Unidades], [Mes]
                    ORDER BY [Id_Producto], [Mes]";


            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int unidades = 0;
                Double precio = 0;

                while (reader.Read())
                {
                    DataRow fila = datatable[Int32.Parse(reader["Mes"].ToString()) - 1].NewRow();

                    fila["Id_Producto"] = reader["Id_Producto"].ToString();
                    fila["Unidades"] = reader["Unidades"].ToString();
                    fila["PRECIO"] = reader["PRECIO"].ToString();

                    datatable[Int32.Parse(reader["Mes"].ToString()) - 1].Rows.Add(fila);

                    unidades += Int32.Parse(reader["Unidades"].ToString());
                    precio += Double.Parse(reader["PRECIO"].ToString());

                    if (Int32.Parse(reader["Mes"].ToString()) == 12)
                    {
                        DataRow filatotal = costoTotal.NewRow();
                        filatotal["Id_Producto"] = reader["Id_Producto"].ToString();
                        filatotal["Unidades"] = unidades;
                        filatotal["PRECIO"] = precio;
                        costoTotal.Rows.Add(filatotal);

                        unidades = 0;
                        precio = 0;
                    }
                }

                for (int i = 0; i < 12; i++)
                {
                    String objeto = "GV_Mes" + (i + 1);
                    GridView gridview = (GridView)this.FindControl(objeto);
                    gridview.DataSource = datatable[i];
                    gridview.DataBind();
                }

                GV_costoTotal.DataSource = costoTotal;
                GV_costoTotal.DataBind();

                //GV_ProyectoProducto.DataSource = datatable;
                //GV_ProyectoProducto.DataBind();
                reader.Close();
            }
            catch (SqlException) { }
            catch (NullReferenceException) { }
            finally
            {
                conn.Close();
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
            bool bEnActa = false;
            Int32 numPostIt = 0;
            Int32 CodigoEstado = 0;

            try
            {
                //Consultar si está en acta...
                bEnActa = es_EnActa(codProyecto.ToString(), codConvocatoria.ToString());

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

                if (rs.Rows.Count > 0)
                {
                    //Crear la variable de sesión.
                    Session["CodRol"] = rs.Rows[0]["CodRol"].ToString();
                }

                //Destruir la variable.
                rs = null;

                #endregion

                //Consultar los datos a mostrar en los campos correspondientes a la actualización.
                txtSQL = " SELECT Nombres + ' ' + Apellidos AS nombre, FechaModificacion, Realizado " +
                         " FROM TabEvaluacionProyecto, Contacto " +
                         " WHERE Id_Contacto = CodContacto AND CodTabEvaluacion = " + txtTab +
                         " AND CodProyecto = " + codProyecto + " AND CodConvocatoria = " + codConvocatoria;

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

                //Evaluar "habilitación" del CheckBox.
                if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                { chk_realizado.Enabled = false; }

                if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString() && lbl_nombre_user_ult_act.Text.Trim() != "" && CodigoEstado == Constantes.CONST_Evaluacion && (!bEnActa))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                //Destruir variables.
                tabla = null;
                txtSQL = null;
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
                //Destruir variables.
                tabla = null;
                txtSQL = null;
                return;
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
        /// 24/06/2014.
        /// Guardar la información "Ultima Actualización".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_guardar_ultima_actualizacion_Click(object sender, EventArgs e)
        { prActualizarTabEval(txtTab.ToString(), codProyecto, codConvocatoria); }

        #endregion
    }
}