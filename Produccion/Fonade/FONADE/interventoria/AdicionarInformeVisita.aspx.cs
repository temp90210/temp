using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.interventoria
{
    public partial class AdicionarInformeVisita : Negocio.Base_Page
    {
        #region Variables globales.
        /// <summary>
        /// Id del informe seleccionado "para consultar la información del informe seleccionado.
        /// </summary>
        string idInforme;
        DataTable dt_informe;
        DataTable RSMedio;
        DataTable infoDeta;
        /// <summary>
        /// Código de la empresa seleccionada para generarle el informe.
        /// </summary>
        String CodEmpresa_NuevoInforme;
        /// <summary>
        /// Nombre de la empresa seleccionada para crearle el nuevo informe.
        /// </summary>
        String Nombre_Empresa;
        /// <summary>
        /// Creado y enviado desde "InformeVisitaInter.aspx" para disponer el formulario para crerar nuevos informes.
        /// </summary>
        String EsNuevo;
        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Versión de Mauricio Arias Olave.
            if (!IsPostBack)
            {
                #region Cargar la fecha actual en los campos correspondientes.
                DateTime fecha_actual = DateTime.Today;
                c_fecha_r.SelectedDate = fecha_actual;
                c_fecha_s.SelectedDate = fecha_actual;
                txtDate.Text = fecha_actual.ToString("dd/MM/yyyy");
                txtDate2.Text = fecha_actual.ToString("dd/MM/yyyy");
                #endregion

                //Consultar el Id y el nombre de la empresa seleccionada para generar un nuevo reporte.
                CodEmpresa_NuevoInforme = Session["InformeIdVisita"] != null && !string.IsNullOrEmpty(Session["InformeIdVisita"].ToString()) ? Session["InformeIdVisita"].ToString() : "0";
                Nombre_Empresa = Session["Nombre_Empresa"] != null && !string.IsNullOrEmpty(Session["Nombre_Empresa"].ToString()) ? Session["Nombre_Empresa"].ToString() : "";
                EsNuevo = Session["Nuevo"] != null && !string.IsNullOrEmpty(Session["Nuevo"].ToString()) ? Session["Nuevo"].ToString() : "False";

                //Determinar si se crea o se actualiza el informe.
                if (EsNuevo != "False")
                {
                    #region Crear nuevo informe.

                    //Cargar los DropDownLists.
                    lenarTabla(true);

                    //Asignar valores.
                    txtempresa.Text = Nombre_Empresa;

                    //Crear nuevo informe "Cargar el nombre de la empresa seleccionada y valores determinados.
                    btn_creaar.Text = "Ingresar Informe";
                    btn_eliminar.Visible = false;
                    #endregion
                }
                else
                {
                    #region Consultar informe seleccionado.
                    //Se ha seleccionado un informe para consultarle su información.
                    idInforme = Session["InformeIdVisita"] != null && !string.IsNullOrEmpty(Session["InformeIdVisita"].ToString()) ? Session["InformeIdVisita"].ToString() : "0";

                    //Si no tiene datos, tuvo que haber un error.
                    if (idInforme == "0")
                    { Response.Redirect("InformeVisitaInter.aspx"); } //return;
                    else
                    {
                        //Consultar la información:
                        DatosEntrada(Convert.ToInt32(idInforme));
                        infoPlan(Convert.ToInt32(idInforme));
                        lenarTabla(false);
                    }
                    #endregion
                }
            }

            //Atributos agregados a controles.
            medio1.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            medio2.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            medio3.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            medio4.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            #endregion

            #region Comentarios.
            //if (!IsPostBack)
            //{
            //    DatosEntrada();

            //    infoPlan();

            //    lenarTabla();
            //}

            //medio1.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            //medio2.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            //medio3.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
            //medio4.Attributes.Add("onkeypress", "javascript: return ValidNum(event);"); 
            #endregion
        }

        #region Métodos de consulta de informe seleccionado.

        /// <summary>
        /// Ejecutar consultas, asignando valores a variables "dt_informe" y "RSMedio".
        /// </summary>
        /// <param name="id_informe">Id del informe seleccionado.</param>
        private void DatosEntrada(Int32 id_informe)
        {
            string txtSQL = "Select * from InformeVisitaInterventoria i, Empresa e where i.CodEmpresa = e.Id_Empresa and id_informe = " + id_informe;
            dt_informe = consultas.ObtenerDataTable(txtSQL, "text");
            txtSQL = "Select Id_MedioDeTransporte, Valor from InformeMedioTransporte, MedioDeTransporte Where Id_MedioDeTransporte = CodMedioTransporte and CodInforme = " + id_informe;
            RSMedio = consultas.ObtenerDataTable(txtSQL, "text");
        }

        /// <summary>
        /// Cargar la información del detalle seleccionado, asignando el resultado de 
        /// la consulta a la variable "infoDeta".
        /// </summary>
        /// <param name="id_informe">Id del informe seleccionado.</param>
        private DataTable infoPlan(Int32 id_informe)
        {
            consultas.Parameters = new[] {
                new SqlParameter { 
                    ParameterName = "@CodGrupo",
                    Value = usuario.CodGrupo
                },
                new SqlParameter { 
                    ParameterName = "@CodUsuario",
                    Value = usuario.IdContacto
                },
                new SqlParameter { 
                    ParameterName = "@CodInforme",
                    Value = id_informe
                }
            };

            return infoDeta = consultas.ObtenerDataTable("MD_ReporteEmpresa");
        }

        /// <summary>
        /// Llenar tabla...
        /// </summary>
        /// <param name="esNuevo">TRUE = Sólo cargar los DropDownLists. // FALSE = Cargar todo, está consultando un informe.</param>
        private void lenarTabla(bool esNuevo)
        {
            if (esNuevo == true)
            {
                #region Cargar DropDownLists.

                var departamentos = from d in consultas.Db.departamentos

                                    orderby d.NomDepartamento ascending
                                    select new
                                    {
                                        Id_Departamento = d.Id_Departamento,
                                        NomDepartamento = d.NomDepartamento
                                    };

                ddl_dedorigen.DataSource = departamentos;
                ddl_dedorigen.DataTextField = "NomDepartamento";
                ddl_dedorigen.DataValueField = "Id_Departamento";
                ddl_dedorigen.DataBind();

                ddl_deddestino.DataSource = departamentos;
                ddl_deddestino.DataTextField = "NomDepartamento";
                ddl_deddestino.DataValueField = "Id_Departamento";
                ddl_deddestino.DataBind();

                #endregion
            }
            else
            {
                #region Consultar la información del informe seleccionado.
                Double valor = 0;
                infoDeta = infoPlan(Int32.Parse(idInforme)); //Consultar la información.
                txtinforme.Text = dt_informe.Rows[0]["NombreInforme"].ToString();
                txtempresa.Text = dt_informe.Rows[0]["razonsocial"].ToString();

                if (dt_informe.Rows[0]["Estado"].ToString().Equals("1"))
                    txtinforme.Enabled = false;

                if (infoDeta.Rows.Count > 0)               
                    lblnit.Text = infoDeta.Rows[0]["nit"].ToString();
                
                var departamentos = from d in consultas.Db.departamentos

                                    orderby d.NomDepartamento ascending
                                    select new
                                    {
                                        Id_Departamento = d.Id_Departamento,
                                        NomDepartamento = d.NomDepartamento
                                    };

                ddl_dedorigen.DataSource = departamentos;
                ddl_dedorigen.DataTextField = "NomDepartamento";
                ddl_dedorigen.DataValueField = "Id_Departamento";
                ddl_dedorigen.DataBind();

                ddl_deddestino.DataSource = departamentos;
                ddl_deddestino.DataTextField = "NomDepartamento";
                ddl_deddestino.DataValueField = "Id_Departamento";
                ddl_deddestino.DataBind();

                string txtSQL = "Select c1.CodDepartamento as dpto_o, c2.CodDepartamento as dpto_d from Ciudad c1, Ciudad c2 Where c1.Id_Ciudad = " + dt_informe.Rows[0]["CodCiudadOrigen"].ToString() + " and c2.Id_Ciudad = " + dt_informe.Rows[0]["CodCiudadDestino"].ToString();

                var dataDep = consultas.ObtenerDataTable(txtSQL, "text");

                try
                {
                    ddl_dedorigen.SelectedValue = dataDep.Rows[0]["dpto_o"].ToString();
                    ddl_dedorigenllenar(Int32.Parse(dataDep.Rows[0]["dpto_o"].ToString()));
                    ddl_ciuorigen.SelectedValue = dt_informe.Rows[0]["CodCiudadOrigen"].ToString();

                    ddl_deddestino.SelectedValue = dataDep.Rows[0]["dpto_d"].ToString();
                    ddl_deddestinollenar(Int32.Parse(dataDep.Rows[0]["dpto_d"].ToString()));
                    ddl_ciudestino.SelectedValue = dt_informe.Rows[0]["CodCiudadDestino"].ToString();
                }
                catch (FormatException) { }

                for (int i = 0; i < RSMedio.Rows.Count; i++)
                {
                    switch (Convert.ToInt32(RSMedio.Rows[i]["Id_MedioDeTransporte"].ToString()))
                    {
                        #region Formateados los valores por Mauricio Arias Olave.
                        case 1:
                            medio1.Text = RSMedio.Rows[i]["Valor"].ToString();
                            try { valor = Convert.ToDouble(medio1.Text); medio1.Text = valor.ToString(); }
                            catch { medio1.Text = RSMedio.Rows[i]["Valor"].ToString(); }
                            if (dt_informe.Rows[0]["Estado"].ToString().Equals("1")) medio1.Enabled = false;
                            break;
                        case 2:
                            medio2.Text = RSMedio.Rows[i]["Valor"].ToString();
                            try { valor = Convert.ToDouble(medio2.Text); medio2.Text = valor.ToString(); }
                            catch { medio2.Text = RSMedio.Rows[i]["Valor"].ToString(); }
                            if (dt_informe.Rows[0]["Estado"].ToString().Equals("1")) medio2.Enabled = false;
                            break;
                        case 3:
                            medio3.Text = RSMedio.Rows[i]["Valor"].ToString();
                            try { valor = Convert.ToDouble(medio3.Text); medio3.Text = valor.ToString(); }
                            catch { medio3.Text = RSMedio.Rows[i]["Valor"].ToString(); }
                            if (dt_informe.Rows[0]["Estado"].ToString().Equals("1")) medio3.Enabled = false;
                            break;
                        case 4:
                            medio4.Text = RSMedio.Rows[i]["Valor"].ToString();
                            try { valor = Convert.ToDouble(medio4.Text); medio4.Text = valor.ToString(); }
                            catch { medio4.Text = RSMedio.Rows[i]["Valor"].ToString(); }
                            if (dt_informe.Rows[0]["Estado"].ToString().Equals("1")) medio4.Enabled = false;
                            break;
                        #endregion
                    }
                }

                c_fecha_s.SelectedDate = DateTime.Parse(dt_informe.Rows[0]["FechaSalida"].ToString()); c_fecha_s.DataBind();
                c_fecha_r.SelectedDate = DateTime.Parse(dt_informe.Rows[0]["FechaRegreso"].ToString()); c_fecha_r.DataBind();

                tb_info_tecnica.Text = dt_informe.Rows[0]["InformacionTecnica"].ToString();
                tb_info_financiera.Text = dt_informe.Rows[0]["InformacionFinanciera"].ToString();

                if (!string.IsNullOrEmpty(idInforme))
                {
                    btn_creaar.Text = "Actualizar";
                    btn_eliminar.Text = "Borrar";
                }
                else
                {
                    btn_creaar.Text = "Ingresar Informe";
                }
                #endregion
            }
        }

        #endregion

        #region Métodos generales.

        protected void ddl_dedorigen_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_dedorigenllenar(Int32.Parse(ddl_dedorigen.SelectedValue));
        }

        private void ddl_dedorigenllenar(int id)
        {
            try
            {
                if (ddl_dedorigen.SelectedValue != "9999999999")
                {
                    var municipios = (from c in consultas.Db.Ciudads
                                      where c.CodDepartamento == id
                                      orderby c.NomCiudad ascending
                                      select new
                                      {
                                          NomCiudad = c.NomCiudad,
                                          Id_Ciudad = c.Id_Ciudad
                                      });
                    ddl_ciuorigen.DataSource = municipios;
                    ddl_ciuorigen.DataTextField = "NomCiudad";
                    ddl_ciuorigen.DataValueField = "ID_Ciudad";
                    ddl_ciuorigen.DataBind();
                }
            }
            catch (FormatException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (SqlException) { }
        }

        protected void ddl_deddestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_deddestinollenar(Int32.Parse(ddl_deddestino.SelectedValue));
        }

        private void ddl_deddestinollenar(int id)
        {
            try
            {
                if (ddl_deddestino.SelectedValue != "9999999999")
                {
                    var municipios = (from c in consultas.Db.Ciudads
                                      where c.CodDepartamento == id
                                      orderby c.NomCiudad ascending
                                      select new
                                      {
                                          NomCiudad = c.NomCiudad,
                                          Id_Ciudad = c.Id_Ciudad
                                      });
                    ddl_ciudestino.DataSource = municipios;
                    ddl_ciudestino.DataTextField = "NomCiudad";
                    ddl_ciudestino.DataValueField = "ID_Ciudad";
                    ddl_ciudestino.DataBind();
                }
            }
            catch (FormatException) { }
            catch (ArgumentOutOfRangeException) { }
        }

        protected void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (btn_eliminar.Text.Equals("Borrar"))
            {
                if (string.IsNullOrEmpty(idInforme))
                    idInforme = Session["InformeIdVisita"] != null && !string.IsNullOrEmpty(Session["InformeIdVisita"].ToString()) ? Session["InformeIdVisita"].ToString() : "0";

                string txtSQL = "Delete InformeMedioTransporte where CodInforme = " + idInforme;

                ejecutaReader(txtSQL, 2);

                txtSQL = "Delete InformeVisitaInterventoria where Id_Informe = " + idInforme;

                ejecutaReader(txtSQL, 2);

                Response.Redirect("InformeVisitaInter.aspx");
            }
        }

        private SqlDataReader ejecutaReader(String sql, int obj)
        {
            SqlDataReader reader = null;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                        reader.Close();
                }

                if (conn != null)
                    conn.Close();

                conn.Open();

                if (obj == 1)
                    reader = cmd.ExecuteReader();
                else
                    cmd.ExecuteReader();
            }
            catch (SqlException se)
            {
                if (conn != null)
                    conn.Close();
                return null;
            }

            return reader;
        }

        #endregion

        /// <summary>
        /// Si el texto del botón es "Ingresar Informe", creará un informe nuevo, de lo contrario
        /// actualizaciá o eliminará el informe seleccionado, al final del proceso, redirigirá al 
        /// usuario a la página de informes de visita.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_creaar_Click(object sender, EventArgs e)
        {
            if (btn_creaar.Text == "Ingresar Informe")
            {
                //Variable temporal que contendrá el resultado devuelto por el método "ValidarSeleccion()".
                string temp_string = ValidarSeleccion();

                //Si la variable contiene datos, significa que NO pasó las validaciones.
                if (temp_string != "")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + temp_string + "')", true);
                    temp_string = null;
                    return;
                }
                else
                {
                    //De lo contrario, creará el informe de visita.
                    #region Crear informe de visita.

                    try
                    {
                        //Inicializar variables.
                        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                        SqlCommand cmd = new SqlCommand();
                        String sqlConsulta = "";
                        DataTable tabla_sql = new DataTable();
                        Double intViatico = 0;
                        bool correcto = false;
                        Int32 intDiferencia = 0;
                        Int32 intInforme = 0;
                        Int32 intValorMedios = 0;
                        DateTime? FechaSalida = new DateTime();
                        DateTime? FechaRegreso = new DateTime();

                        CodEmpresa_NuevoInforme = Session["InformeIdVisita"] != null && !string.IsNullOrEmpty(Session["InformeIdVisita"].ToString()) ? Session["InformeIdVisita"].ToString() : "0";
                        if (CodEmpresa_NuevoInforme == "0")
                        { return; }

                        try
                        {
                            FechaSalida = Convert.ToDateTime(txtDate.Text);
                            FechaRegreso = Convert.ToDateTime(txtDate2.Text);
                        }
                        catch
                        {
                            FechaSalida = null;
                            FechaRegreso = null;
                        }

                        if (FechaSalida == null || FechaRegreso == null)
                        { return; }

                        //Consulta #1.
                        sqlConsulta = " SELECT Valor FROM Viatico " +
                                      " WHERE ((SELECT Salario FROM Interventor WHERE (CodContacto = " + usuario.IdContacto + ")) " +
                                      " BETWEEN LimiteInferior AND LimiteSuperior and LimiteSuperior != -1) ";

                        //Asignar resultados de la consulta #1 a variable DataTable.
                        tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Si tiene datos, consultar el campo "Valor".
                        if (tabla_sql.Rows.Count > 0)
                        {
                            #region Continuar con el flujo.

                            //Asignar el valor consultado en la tabla.
                            intViatico = Convert.ToDouble(tabla_sql.Rows[0]["Valor"].ToString());

                            //Ejecutar sp:
                            consultas.Parameters = new[]
                                           {
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@NombreInforme" ,Value = txtinforme.Text.Trim()
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@CodCiudadOrigen" ,Value = ddl_ciuorigen.SelectedValue
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@CodCiudadDestino" ,Value = ddl_ciudestino.SelectedValue
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@FechaSalida" ,Value = FechaSalida
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@FechaRegreso" ,Value = FechaRegreso
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@CodEmpresa" ,Value = CodEmpresa_NuevoInforme
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@CodInterventor" ,Value = usuario.IdContacto
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@InformacionTecnica" ,Value = tb_info_tecnica.Text.Trim()
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@InformacionFinanciera" ,Value = tb_info_financiera.Text.Trim()
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName = "@Estado" ,Value = false
                                                   }
                                           };

                            //Asignar resultados de la consulta a la variable DataTable.
                            var t_1 = consultas.ObtenerDataTable("MD_InsertarInformeVisita");

                            #region Si hizo la inserción, por lo tanto, puede seguir el flujo.

                            //Consulta #2
                            tabla_sql = null;
                            sqlConsulta = "";
                            sqlConsulta = " SELECT Id_Informe, CONVERT(int, FechaRegreso - FechaSalida) + 1 AS Diferencia " +
                                          " FROM InformeVisitaInterventoria" +
                                          " WHERE (Id_Informe = (SELECT MAX(Id_Informe) FROM InformeVisitaInterventoria)) ";

                            //Asignar resultados de la consulta anterior.
                            tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                            //Si la consulta anterior tiene datos...
                            if (tabla_sql.Rows.Count > 0)
                            {
                                intDiferencia = Convert.ToInt32(tabla_sql.Rows[0]["Diferencia"].ToString());
                                intInforme = Convert.ToInt32(tabla_sql.Rows[0]["Id_Informe"].ToString());

                                #region Actualizar medios...

                                //Consulta #3:
                                tabla_sql = null;
                                sqlConsulta = "";
                                sqlConsulta = " SELECT Id_MedioDeTransporte, NomMedioDeTransporte FROM MedioDeTransporte ";

                                //Asignar resultados de la consulta #3 a variable DataTable.
                                tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                                //Recorrer la consulta anterior para realizar inserciones.
                                for (int i = 0; i < tabla_sql.Rows.Count; i++)
                                {
                                    if (medio1.Text.Trim() != "" || medio1.Text.Trim() != "0")
                                    {
                                        #region Medio "Otro".

                                        //Obtener el valor del medio.
                                        intValorMedios = Convert.ToInt32(medio1.Text.Trim());

                                        //Inserción.
                                        sqlConsulta = " INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte, Valor) " +
                                                      " VALUES(" + intInforme + ", " + 1 + ", " + medio1.Text + ") ";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar SQL.
                                        correcto = EjecutarSQL(conn, cmd);
                                        break;
                                        #endregion
                                    }
                                    if (medio2.Text.Trim() != "" || medio2.Text.Trim() != "0")
                                    {
                                        #region Medio "Avión".

                                        //Obtener el valor del medio.
                                        intValorMedios = Convert.ToInt32(medio2.Text.Trim());

                                        //Inserción.
                                        sqlConsulta = " INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte, Valor) " +
                                                      " VALUES(" + intInforme + ", " + 2 + ", " + medio2.Text + ") ";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar SQL.
                                        correcto = EjecutarSQL(conn, cmd);
                                        break;
                                        #endregion
                                    }
                                    if (medio3.Text.Trim() != "" || medio3.Text.Trim() != "0")
                                    {
                                        #region Medio "Bus".

                                        //Obtener el valor del medio.
                                        intValorMedios = Convert.ToInt32(medio3.Text.Trim());

                                        //Inserción.
                                        sqlConsulta = " INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte, Valor) " +
                                                      " VALUES(" + intInforme + ", " + 3 + ", " + medio3.Text + ") ";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar SQL.
                                        correcto = EjecutarSQL(conn, cmd);
                                        break;
                                        #endregion
                                    }
                                    if (medio4.Text.Trim() != "" || medio4.Text.Trim() != "0")
                                    {
                                        #region Medio "Barco".

                                        //Obtener el valor del medio.
                                        intValorMedios = Convert.ToInt32(medio4.Text.Trim());

                                        //Inserción.
                                        sqlConsulta = " INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte, Valor) " +
                                                      " VALUES(" + intInforme + ", " + 4 + ", " + medio4.Text + ") ";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar SQL.
                                        correcto = EjecutarSQL(conn, cmd);
                                        break;
                                        #endregion
                                    }
                                }

                                #endregion

                                //Actualización final.
                                sqlConsulta = "";
                                sqlConsulta = " Update InformeVisitaInterventoria set CostoVisita = " + (intDiferencia * intViatico) + intValorMedios +
                                              " Where (Id_Informe = (SELECT MAX(Id_Informe) FROM InformeVisitaInterventoria)) ";

                                //Asignar SqlCommand para su ejecución.
                                cmd = new SqlCommand(sqlConsulta, conn);

                                //Ejecutar SQL.
                                correcto = EjecutarSQL(conn, cmd);

                                if (correcto == false)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo generar el informe de visita.')", true);
                                    return;
                                }
                                else
                                {
                                    //Si ha hecho todo el flujo bien, por seguridad se eliminan ciertas variables de sesión.
                                    Session["InformeIdVisita"] = "";
                                    Session["Nombre_Empresa"] = "";
                                }
                            }

                            #endregion

                            #endregion
                        }
                        else
                        {
                            #region Continuar con el flujo.

                            //Ejecutar consulta #2:
                            tabla_sql = null;
                            sqlConsulta = "";
                            sqlConsulta = " SELECT Valor FROM Viatico WHERE " +
                                          " ((SELECT Salario FROM Interventor " +
                                          " WHERE (CodContacto = " + usuario.IdContacto + ")) >= LimiteInferior and LimiteSuperior = -1) ";

                            //Asignar resultados de la consulta #2.
                            tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                            if (tabla_sql.Rows.Count > 0)
                            {
                                //Asignar valor a variable Int.
                                intViatico = Convert.ToDouble(tabla_sql.Rows[0]["Valor"].ToString());

                                sqlConsulta = "";
                                sqlConsulta = " INSERT INTO InformeVisitaInterventoria(NombreInforme, CodCiudadOrigen, CodCiudadDestino, FechaSalida," +
                                              " FechaRegreso, CodEmpresa, FechaInforme, CodInterventor, InformacionTecnica, InformacionFinanciera, Estado)" +
                                              " VALUES('" + txtinforme.Text.Trim() + "', " + ddl_ciuorigen.SelectedValue + ", " + ddl_ciudestino.SelectedValue + ", '" +
                                              c_fecha_s.SelectedDate + "', " + "'" + c_fecha_r.SelectedDate + "', " + CodEmpresa_NuevoInforme + ", GETDATE(), " +
                                              usuario.IdContacto + ", '" + tb_info_tecnica.Text.Trim() + "', '" + tb_info_financiera.Text.Trim() + "',0)";

                                //Asignar SqlCommand para su ejecución.
                                cmd = new SqlCommand(sqlConsulta, conn);

                                //Ejecutar SQL.
                                correcto = EjecutarSQL(conn, cmd);

                                if (correcto == false)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo generar el informe de visita.')", true);
                                    return;
                                }
                                else
                                {
                                    #region Si hizo la inserción, por lo tanto, puede seguir el flujo.

                                    //Consulta #2
                                    tabla_sql = null;
                                    sqlConsulta = "";
                                    sqlConsulta = " SELECT Id_Informe, CONVERT(int, FechaRegreso - FechaSalida) + 1 AS Diferencia " +
                                                  " FROM InformeVisitaInterventoria" +
                                                  " WHERE (Id_Informe = (SELECT MAX(Id_Informe) FROM InformeVisitaInterventoria)) ";

                                    //Asignar resultados de la consulta anterior.
                                    tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                                    if (tabla_sql.Rows.Count > 0)
                                    {
                                        intDiferencia = Convert.ToInt32(tabla_sql.Rows[0]["Diferencia"].ToString());
                                        intInforme = Convert.ToInt32(tabla_sql.Rows[0]["Id_Informe"].ToString());

                                        #region Actualizar medios...

                                        //Consulta #3:
                                        tabla_sql = null;
                                        sqlConsulta = "";
                                        sqlConsulta = " SELECT Id_MedioDeTransporte, NomMedioDeTransporte FROM MedioDeTransporte ";

                                        //Asignar resultados de la consulta #3 a variable DataTable.
                                        tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                                        //Recorrer la consulta anterior para realizar inserciones.
                                        for (int i = 0; i < tabla_sql.Rows.Count; i++)
                                        {
                                            if (medio1.Text.Trim() != "" || medio1.Text.Trim() != "0")
                                            {
                                                #region Medio "Otro".

                                                //Obtener el valor del medio.
                                                intValorMedios = Convert.ToInt32(medio1.Text.Trim());

                                                //Inserción.
                                                sqlConsulta = " INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte, Valor) " +
                                                              " VALUES(" + intInforme + ", " + 1 + ", " + medio1.Text + ") ";

                                                //Asignar SqlCommand para su ejecución.
                                                cmd = new SqlCommand(sqlConsulta, conn);

                                                //Ejecutar SQL.
                                                correcto = EjecutarSQL(conn, cmd);
                                                break;
                                                #endregion
                                            }
                                            if (medio2.Text.Trim() != "" || medio2.Text.Trim() != "0")
                                            {
                                                #region Medio "Avión".

                                                //Obtener el valor del medio.
                                                intValorMedios = Convert.ToInt32(medio2.Text.Trim());

                                                //Inserción.
                                                sqlConsulta = " INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte, Valor) " +
                                                              " VALUES(" + intInforme + ", " + 2 + ", " + medio2.Text + ") ";

                                                //Asignar SqlCommand para su ejecución.
                                                cmd = new SqlCommand(sqlConsulta, conn);

                                                //Ejecutar SQL.
                                                correcto = EjecutarSQL(conn, cmd);
                                                break;
                                                #endregion
                                            }
                                            if (medio3.Text.Trim() != "" || medio3.Text.Trim() != "0")
                                            {
                                                #region Medio "Bus".

                                                //Obtener el valor del medio.
                                                intValorMedios = Convert.ToInt32(medio3.Text.Trim());

                                                //Inserción.
                                                sqlConsulta = " INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte, Valor) " +
                                                              " VALUES(" + intInforme + ", " + 3 + ", " + medio3.Text + ") ";

                                                //Asignar SqlCommand para su ejecución.
                                                cmd = new SqlCommand(sqlConsulta, conn);

                                                //Ejecutar SQL.
                                                correcto = EjecutarSQL(conn, cmd);
                                                break;
                                                #endregion
                                            }
                                            if (medio4.Text.Trim() != "" || medio4.Text.Trim() != "0")
                                            {
                                                #region Medio "Barco".

                                                //Obtener el valor del medio.
                                                intValorMedios = Convert.ToInt32(medio4.Text.Trim());

                                                //Inserción.
                                                sqlConsulta = " INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte, Valor) " +
                                                              " VALUES(" + intInforme + ", " + 4 + ", " + medio4.Text + ") ";

                                                //Asignar SqlCommand para su ejecución.
                                                cmd = new SqlCommand(sqlConsulta, conn);

                                                //Ejecutar SQL.
                                                correcto = EjecutarSQL(conn, cmd);
                                                break;
                                                #endregion
                                            }
                                        }

                                        #endregion

                                        //Actualización final.
                                        sqlConsulta = "";
                                        sqlConsulta = " Update InformeVisitaInterventoria set CostoVisita = " + (intDiferencia * intViatico) + intValorMedios +
                                                      " Where (Id_Informe = (SELECT MAX(Id_Informe) FROM InformeVisitaInterventoria)) ";

                                        //Asignar SqlCommand para su ejecución.
                                        cmd = new SqlCommand(sqlConsulta, conn);

                                        //Ejecutar SQL.
                                        correcto = EjecutarSQL(conn, cmd);

                                        if (correcto == false)
                                        {
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo generar el informe de visita.')", true);
                                            return;
                                        }
                                        else
                                        {
                                            //Si ha hecho todo el flujo bien, por seguridad se eliminan ciertas variables de sesión.
                                            Session["InformeIdVisita"] = "";
                                            Session["Nombre_Empresa"] = "";
                                        }
                                    }

                                    #endregion
                                }
                            }

                            #endregion
                        }
                    }
                    catch
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo generar el informe de visita.')", true);
                        return;
                    }

                    #endregion
                }
            }
            else
            {
                #region Proceso para actualizar o eliminar informe...
                if (string.IsNullOrEmpty(idInforme))
                    idInforme = Session["InformeIdVisita"] != null && !string.IsNullOrEmpty(Session["InformeIdVisita"].ToString()) ? Session["InformeIdVisita"].ToString() : "0";

                #region validarDatos

                DateTime fecha = new DateTime();

                fecha = DateTime.Now;

                if (c_fecha_s.SelectedDate > fecha || c_fecha_r.SelectedDate > fecha)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Las Fechas de Salida y Regreso no pueden ser más recientes que la fecha actual')", true);
                    return;
                }

                if (c_fecha_r.SelectedDate < c_fecha_s.SelectedDate)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La fecha de Salida no puede ser más reciente que la Fecha de Regreso!!!')", true);
                    return;
                }

                string txtSQL = "Select NombreInforme from InformeVisitaInterventoria where id_Informe != " + idInforme + " and NombreInforme = '" + txtinforme.Text + "'";

                var resulRep = consultas.ObtenerDataTable(txtSQL, "text");

                if (resulRep.Rows.Count > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Imposible Modificar. Ya existe un informe con ese nombre.')", true);
                    return;
                }

                #endregion

                #region
                string informe = txtinforme.Text;
                string empresa = txtempresa.Text;
                string departamentoOrigen = ddl_dedorigen.SelectedValue;
                string departamentoDestino = ddl_deddestino.SelectedValue;
                string ciudadOrigen = ddl_ciuorigen.SelectedValue;
                string ciudadDestino = ddl_ciudestino.SelectedValue;
                string otro = medio1.Text;
                string avion = medio2.Text;
                string buss = medio3.Text;
                string barco = medio4.Text;
                string fechaSalida = c_fecha_s.SelectedDate.Value.Year + "-" + c_fecha_s.SelectedDate.Value.Month + "-" + c_fecha_s.SelectedDate.Value.Day + " " + c_fecha_s.SelectedDate.Value.Hour + ":" + c_fecha_s.SelectedDate.Value.Minute + ":" + c_fecha_s.SelectedDate.Value.Second;
                string fechaRegreso = c_fecha_r.SelectedDate.Value.Year + "-" + c_fecha_r.SelectedDate.Value.Month + "-" + c_fecha_r.SelectedDate.Value.Day + " " + c_fecha_r.SelectedDate.Value.Hour + ":" + c_fecha_r.SelectedDate.Value.Minute + ":" + c_fecha_r.SelectedDate.Value.Second;
                string info_tecnica = tb_info_tecnica.Text;
                string info_financiera = tb_info_financiera.Text;
                #endregion

                double intValorMedios = ingrearMedios(new string[] { otro, avion, buss, barco });

                txtSQL = @"Update InformeVisitaInterventoria Set NombreInforme = '" + informe + "', CodCiudadOrigen = " + ciudadOrigen + ", CodCiudadDestino = " + ciudadDestino +
                    ", FechaSalida = '" + fechaSalida + "', FechaRegreso = '" + fechaRegreso +
                    "', CostoVisita = " + intValorMedios + ", InformacionTecnica = '" + info_tecnica +
                    "', InformacionFinanciera = '" + info_financiera + "' Where Id_Informe = " + idInforme;

                ejecutaReader(txtSQL, 2);
                #endregion
            }

            //Finalmente redirige al usuario a la página de informes de visita.
            Response.Redirect("InformeVisitaInter.aspx");
        }

        private double ingrearMedios(string[] medios)
        {
            double intValorMedios = 0;
            for (int i = 1; i <= medios.Length; i++)
            {
                string txtSQL = "Select count(1) as Cuantos from InformeMedioTransporte Where CodInforme = " + idInforme + " and CodMedioTransporte = " + i;
                var medio1 = consultas.ObtenerDataTable(txtSQL, "text");

                if (String.IsNullOrEmpty(medios[i - 1].ToString()))
                {
                    txtSQL = "Delete InformeMedioTransporte Where CodInforme = " + idInforme + " And CodMedioTransporte = 1";
                }
                else
                {
                    intValorMedios += Convert.ToDouble(medios[i - 1].ToString());

                    if (Convert.ToInt32(medio1.Rows[0]["Cuantos"].ToString()) > 0)
                        txtSQL = "UPDATE InformeMedioTransporte SET Valor = " + medios[i - 1].ToString() + " WHERE Codinforme = " + idInforme + " AND CodMedioTransporte = 1";
                    else
                        txtSQL = "INSERT INTO InformeMedioTransporte(CodInforme, CodMedioTransporte,Valor) VALUES(" + idInforme + ", 1, " + medios[i - 1].ToString() + ")";
                }

                ejecutaReader(txtSQL, 2);
            }
            return intValorMedios;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 22/05/2014.
        /// Validar "nuevamente" la selección de departamento y ciudad origen.
        /// </summary>
        /// <returns>String con datos = NO debe continuar. // String vacío: Continuar.</returns>
        private string ValidarSeleccion()
        {
            try
            {
                if (ddl_ciuorigen.SelectedValue == "" || ddl_ciuorigen.SelectedValue == null)
                {
                    return "Debe seleccionar la ciudad de origen.";
                }
                if (ddl_dedorigen.SelectedValue == "" || ddl_dedorigen.SelectedValue == null)
                {
                    return "Debe seleccionar el departamento de origen.";
                }

                return "";
            }
            catch { return "ERROR"; }
        }
    }
}