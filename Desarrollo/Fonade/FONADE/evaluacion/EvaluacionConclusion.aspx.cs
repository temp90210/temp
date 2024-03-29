﻿using Datos;
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
    public partial class EvaluacionConclusion : Negocio.Base_Page
    {
        DataTable datatable;

        public String codProyecto;
        public int txtTab = Constantes.ConstSubConclusion;//Constantes.CONST_ProyeccionesVentas;
        public String codConvocatoria;
        private ProyectoMercadoProyeccionVenta pm;
        Int32 bolValorRecomendado;
        public bool esMiembro;
        /// <summary>
        /// PAra saber si está o no "realizado".
        /// </summary>
        public Boolean bRealizado;

        protected void Page_Load(object sender, EventArgs e)
        {
            inicioEncabezado(codProyecto, codConvocatoria, Constantes.ConstSubFlujoCaja);
            if (miembro == false && realizado == true && usuario.CodGrupo == Constantes.CONST_Evaluador)
            {
                B_Guardar.Enabled = false;
            }
            datos();
            if (!IsPostBack)
            {
                llenarPlantilla();
            }

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto.ToString());

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codProyecto.ToString(), codConvocatoria); //Estaba vacío...

            if (esMiembro && usuario.CodGrupo == Constantes.CONST_Evaluador && !bRealizado) { B_Guardar.Visible = true; }

            if (!(esMiembro && !bRealizado)) { this.div_Post_It1.Visible = true; }

            if (usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador || usuario.CodGrupo == Constantes.CONST_GerenteEvaluador)
            {
                #region Ocultar otros campos...

                btn_guardar_ultima_actualizacion.Enabled = false;
                btn_guardar_ultima_actualizacion.Visible = false;
                DropDownList1.Enabled = false;
                DDL_Conceptos.Enabled = false;
                TB_Justificacion.Enabled = false;

                #endregion

                DDL_Conceptos.Enabled = false;
                DropDownList1.Enabled = false;
                TB_Justificacion.Enabled = false;
            }

            if (!IsPostBack) { ObtenerDatosUltimaActualizacion(); }
        }

        private void datos()
        {
            try
            {
                codProyecto = Session["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
                codConvocatoria = Session["codConvocatoria"].ToString();

                codProyecto = Request.QueryString["codProyecto"].ToString();
                Session["codProyectoval"] = codProyecto;
            }
            catch (Exception) { }

            //CtrlCheckedProyecto1.tabCod = Constantes.ConstSubConclusion;
            //CtrlCheckedProyecto1.CodProyecto = codProyecto;
        }

        private void llenarPlantilla()
        {
            vonvocatoria();
            String sql;
            sql = "SELECT [id_evaluacionconceptos], [nomevaluacionconceptos] FROM [Fonade].[dbo].[evaluacionconceptos] ORDER BY [id_evaluacionconceptos]";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DDL_Conceptos.Items.Clear();
                while (reader.Read())
                {
                    ListItem item = new ListItem();
                    item.Text = reader["nomevaluacionconceptos"].ToString();
                    item.Value = reader["id_evaluacionconceptos"].ToString();

                    if (!(datatable.Rows.Count < 1))
                        if (datatable.Rows[0]["codevaluacionconceptos"].ToString().Equals(reader["id_evaluacionconceptos"].ToString()))
                            item.Selected = true;

                    DDL_Conceptos.Items.Add(item);
                }
                reader.Close();

                if (!(datatable.Rows.Count < 1))
                {
                    TB_Justificacion.Text = datatable.Rows[0]["Justificacion"].ToString();
                    if (datatable.Rows[0]["Viable"].ToString().Equals("0") || datatable.Rows[0]["Viable"].ToString().Equals("False"))
                    {
                        DropDownList1.Items[0].Selected = true;
                        DropDownList1.Items[1].Selected = false;
                    }
                    else
                    {
                        DropDownList1.Items[0].Selected = false;
                        DropDownList1.Items[1].Selected = true;
                    }
                }
            }
            catch (SqlException)
            {
            }
            finally
            {
                conn.Close();
            }
        }

        private void vonvocatoria()
        {
            datos();
            datatable = new DataTable();

            datatable.Columns.Add("CodConvocatoria");
            datatable.Columns.Add("CodProyecto");
            datatable.Columns.Add("Fecha");
            datatable.Columns.Add("Justificacion");
            datatable.Columns.Add("Viable");
            datatable.Columns.Add("codevaluacionconceptos");

            String sql;
            sql = "SELECT * FROM [Fonade].[dbo].[ConvocatoriaProyecto] WHERE [CodProyecto] = " + codProyecto + " AND [CodConvocatoria] = " + codConvocatoria;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    DataRow fila = datatable.NewRow();
                    fila["CodConvocatoria"] = reader["CodConvocatoria"].ToString();
                    fila["CodProyecto"] = reader["CodProyecto"].ToString();
                    fila["Fecha"] = reader["Fecha"].ToString();
                    fila["Justificacion"] = reader["Justificacion"].ToString();
                    fila["Viable"] = reader["Viable"].ToString();
                    fila["codevaluacionconceptos"] = reader["codevaluacionconceptos"].ToString();
                    datatable.Rows.Add(fila);
                }
                reader.Close();
            }
            catch (SqlException se) { }
            finally
            { conn.Close(); }
        }

        /// <summary>
        /// Guadar los datos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_Guardar_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            string validar = "";
            ClientScriptManager cm = this.ClientScript;
            validar = fnValidar_Campos();

            if (validar == "")
            {
                if (DropDownList1.SelectedValue == "1")//1
                {
                    if (!fnValidoPuntaje(codProyecto, codConvocatoria))
                    {
                        DropDownList1.SelectedValue = "0";
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Todos los aspectos evaluados no obtuvieron el puntaje mínimo aprobatorio. No se puede dar viabilidad al proyecto.');</script>");
                        return;
                    }
                }
                else
                {
                    #region Actualizar!

                    string conexionStr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                    using (var con = new SqlConnection(conexionStr))
                    {
                        using (var com = con.CreateCommand())
                        {
                            com.CommandText = "MD_Actualizar_ConvocatoriaProyecto";
                            com.CommandType = System.Data.CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@_CodConvocatoria", codConvocatoria);
                            com.Parameters.AddWithValue("@_CodProyecto", codProyecto);
                            com.Parameters.AddWithValue("@_Justificacion", TB_Justificacion.Text);
                            com.Parameters.AddWithValue("@_Viable", DropDownList1.SelectedIndex);
                            com.Parameters.AddWithValue("@_codevaluacionconceptos", DDL_Conceptos.SelectedValue);

                            try
                            {
                                con.Open();
                                com.ExecuteReader();
                                prActualizarTabEval(txtTab.ToString(), codProyecto, codConvocatoria);
                                ObtenerDatosUltimaActualizacion();
                            }
                            catch (Exception ex)
                            { }
                            finally
                            {
                                con.Close();
                            }
                        }
                    }

                    cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Se han actualizado los datos.');</script>");
                    return;

                    #endregion
                }
            }
            else
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('" + validar + "');</script>");
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/07/2014.
        /// Función para validar el puntaje "según FONADE clásico".
        /// </summary>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <param name="CodConvocatoria">Código de la convocatoria.</param>
        /// <returns></returns>
        private bool fnValidoPuntaje(String CodProyecto, String CodConvocatoria)
        {
            //Inicializar variables.
            String txtSQL;
            bool bValido = true;
            DataTable RsAspecto = new DataTable();
            DataTable RS = new DataTable();

            try
            {
                //Obtener el puntaje mininmo aprobatorio para cada aspecto
                txtSQL = " select id_campo, puntaje " +
                         " from convocatoriacampo cc, campo c " +
                         " where id_campo=cc.codcampo and c.codcampo is null " +
                         " and codconvocatoria = " + CodConvocatoria;

                RsAspecto = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow r_RsAspecto in RsAspecto.Rows)
                {
                    //Puntaje obtenido en el aspecto
                    txtSQL = " select sum(ec.puntaje) puntaje " +
                             " from evaluacioncampo ec " +
                             " inner join campo c on c.id_campo = ec.codcampo " +
                             " inner join campo v on v.id_campo = c.codcampo " +
                             " inner join campo a on a.id_campo = v.codcampo " +
                             " where codproyecto=" + CodProyecto + " and codconvocatoria = " + CodConvocatoria +
                             " and a.id_campo = " + r_RsAspecto["id_campo"].ToString();

                    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    if (Int32.Parse(RS.Rows[0]["puntaje"].ToString()) < Int32.Parse(r_RsAspecto["puntaje"].ToString()))
                    { bValido = false; break; }
                }

                RsAspecto = null;
                RS = null;
                return bValido;
            }
            catch { return false; }
        }

        private Boolean validar()
        {
            datos();
            Boolean resul = true;
            DataTable comparacion = new DataTable();

            comparacion.Columns.Add("id_Campo");
            comparacion.Columns.Add("Puntaje");

            String sql;
            sql = @"SELECT [id_Campo], [Puntaje]
                    FROM [Fonade].[dbo].[ConvocatoriaCampo] AS CC, [Fonade].[dbo].[Campo] AS C
                    WHERE [id_Campo] = CC.[codCampo] AND C.[codCampo] IS NULL
                    AND [codConvocatoria] = " + codConvocatoria;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = comparacion.NewRow();

                    fila["id_Campo"] = reader["id_Campo"].ToString();
                    fila["Puntaje"] = reader["Puntaje"].ToString();

                    comparacion.Rows.Add(fila);
                }
                reader.Close();
            }
            catch (SqlException)
            {
            }
            finally
            {
                conn.Close();
            }


            for (int i = 0; i < comparacion.Rows.Count; i++)
            {
                sql = @"SELECT SUM(EC.[Puntaje]) AS PUNTAJE
                    FROM [Fonade].[dbo].[EvaluacionCampo] AS EC
                    INNER JOIN [Fonade].[dbo].[Campo] AS C ON C.[id_Campo] = EC.[codCampo]
                    INNER JOIN [Fonade].[dbo].[Campo] AS V ON V.[id_Campo] = C.[codCampo]
                    INNER JOIN [Fonade].[dbo].[Campo] AS A ON A.[id_Campo] = V.[codCampo]
                    WHERE [codProyecto] = " + codProyecto + @" AND [codConvocatoria] = " + codConvocatoria + @"
                    AND A.[id_Campo] = " + comparacion.Rows[i]["id_Campo"].ToString();

                conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                cmd = new SqlCommand(sql, conn);

                cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        if (Int32.Parse(reader["PUNTAJE"].ToString()) < Int32.Parse(comparacion.Rows[i]["Puntaje"].ToString()))
                        {
                            resul = false;
                            return resul;
                        }
                    }
                    reader.Close();
                }
                catch (SqlException)
                {
                }
                finally
                {
                    conn.Close();
                }
            }

            return resul;
        }

        protected void I_AyudaProVentas_Click(object sender, ImageClickEventArgs e)
        {
            Session["mensaje"] = 7;
            ClientScriptManager cm = this.ClientScript;
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>open('../Ayuda/Mensaje.aspx', 'Conceptos de Justificacion:', 'width=500,height=400');</script>");
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Session["mensaje"] = 8;
            ClientScriptManager cm = this.ClientScript;
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>open('../Ayuda/Mensaje.aspx', 'Justificacion', 'width=500,height=400');</script>");
        }

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

        private void ObtenerDatosUltimaActualizacion()
        {
            String txtSQL;
            DateTime fecha = new DateTime();
            DataTable tabla = new DataTable();
            bool bNuevo = true;
            bool bRealizado = false;
            bool bEnActa = false;
            bool EsMiembro = false;
            Int32 CodigoEstado = 0;

            try
            {

                bNuevo = es_bNuevo(codProyecto.ToString());
                bEnActa = es_EnActa(codProyecto.ToString(), codConvocatoria.ToString());
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto.ToString());

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

                txtSQL = " select nombres+' '+apellidos as nombre, fechamodificacion, realizado  " +
                         " from tabEvaluacionproyecto, contacto " +
                         " where id_contacto = codcontacto and codtabEvaluacion = " + txtTab +
                         " and codproyecto = " + codProyecto +
                         " and codconvocatoria = " + codConvocatoria;

                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                if (tabla.Rows.Count > 0)
                {
                    lbl_nombre_user_ult_act.Text = tabla.Rows[0]["nombre"].ToString().ToUpperInvariant();

                    try { fecha = Convert.ToDateTime(tabla.Rows[0]["FechaModificacion"].ToString()); }
                    catch { fecha = DateTime.Today; }

                    string sMes = fecha.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                    string hora = fecha.ToString("hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant();

                    if (hora.Contains("am")) { hora = hora.Replace("am", "a.m"); } if (hora.Contains("pm")) { hora = hora.Replace("pm", "p.m"); }
                    lbl_fecha_formateada.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year + " " + hora + ".";
                    bRealizado = Convert.ToBoolean(tabla.Rows[0]["Realizado"].ToString());
                }

                chk_realizado.Checked = bRealizado;

                //Evaluar "habilitación" del CheckBox.
                //if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                { chk_realizado.Enabled = false; }

                //if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString() && lbl_nombre_user_ult_act.Text.Trim() != "" && CodigoEstado == Constantes.CONST_Evaluacion && (!bEnActa))
                if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString() && lbl_nombre_user_ult_act.Text.Trim() != "" && CodigoEstado == Constantes.CONST_Evaluacion && (!bEnActa))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                tabla = null;
                txtSQL = null;
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
                tabla = null;
                txtSQL = null;
                return;
            }
        }

        private int Obtener_numPostIt()
        {
            Int32 numPosIt = 0;

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

        protected void btn_guardar_ultima_actualizacion_Click(object sender, EventArgs e)
        { prActualizarTabEval(txtTab.ToString(), codProyecto, codConvocatoria); Marcar(txtTab.ToString(), codProyecto, codConvocatoria, chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

        /// <summary>
        /// Validar de acuerdo a FONADE clásico.
        /// </summary>
        /// <returns>Vacío = Puede continuar. // Con datos = Error.</returns>
        private string fnValidar_Campos()
        {
            //Inicializar variables.
            String txtSQL = "";
            DataTable rs = new DataTable();
            string valor = "";

            txtSQL = " SELECT Sum(ValorRecomendado) AS Recomendado FROM EvaluacionObservacion WHERE CodProyecto = " + codProyecto +
                     " AND CodConvocatoria = " + codConvocatoria;
            rs = consultas.ObtenerDataTable(txtSQL, "text");

            if (rs.Rows.Count > 0)
            {
                if (!String.IsNullOrEmpty(rs.Rows[0]["Recomendado"].ToString()))
                {
                    if (Int32.Parse(rs.Rows[0]["Recomendado"].ToString()) > 0)
                    { bolValorRecomendado = 1; }
                }
            }

            try
            {
                if (DDL_Conceptos.SelectedValue == "")
                { valor = "Debe Seleccionar un concepto"; }
            }
            catch { valor = "Debe Seleccionar un concepto"; }

            if (TB_Justificacion.Text.Trim() == "")
            { valor = "La justificación es requerida"; }

            if (DropDownList1.SelectedValue == "1") //1
            {
                if (bolValorRecomendado == 0)
                { valor = "El valor recomendado no puede ser CERO. Por favor complete los datos en la subpestaña Aportes."; }
                else { valor = ""; }
            }

            return valor;
        }
    }
}