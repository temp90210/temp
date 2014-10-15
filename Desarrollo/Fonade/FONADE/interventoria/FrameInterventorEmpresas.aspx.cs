using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Fonade.Account;
using LinqKit;
using AjaxControlToolkit;
using System.ComponentModel;
using System.Globalization;
using Fonade.Negocio;

namespace Fonade.FONADE.interventoria
{
    public partial class FrameInterventorEmpresas : Base_Page
    {
        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDropDownListSectores();
                GetSelectedRecord();
            }
        }

        #region Métodos generales.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Crear tabla base para bindear los datos en la grilla "gv_sectores_encontrados".
        /// </summary>
        /// <returns>DataTable con las columas predefinidas.</returns>
        private DataTable CrearDataTable()
        {
            //Inicializar variable DataTable.
            DataTable tabla_base = new DataTable();

            //Id_Sector.
            DataColumn Column1 = new DataColumn();
            Column1.AllowDBNull = true;
            Column1.DataType = System.Type.GetType("System.String");
            Column1.ColumnName = "Id_Sector";

            //CodProyecto.
            DataColumn Column2 = new DataColumn();
            Column2.AllowDBNull = true;
            Column2.DataType = System.Type.GetType("System.String");
            Column2.ColumnName = "CodProyecto";

            //RazonSocial.
            DataColumn Column3 = new DataColumn();
            Column3.AllowDBNull = true;
            Column3.DataType = System.Type.GetType("System.String");
            Column3.ColumnName = "RazonSocial";

            //Añadir las columnas generadas a la tabla.
            tabla_base.Columns.Add(Column1);
            tabla_base.Columns.Add(Column2);
            tabla_base.Columns.Add(Column3);

            //Retornar la tabla base.
            return tabla_base;
        }

        /// <summary>
        /// Obtener el registro seleccionado.
        /// </summary>
        private void GetSelectedRecord()
        {
            for (int i = 0; i < gv_SubDetalles_interventores.Rows.Count; i++)
            {
                RadioButton rb = (RadioButton)gv_SubDetalles_interventores.Rows[i]
                                .Cells[0].FindControl("rb_interv_lider");
                if (rb != null)
                {
                    if (rb.Checked)
                    {
                        HiddenField hf = (HiddenField)gv_SubDetalles_interventores.Rows[i]
                                        .Cells[0].FindControl("HiddenField1");
                        if (hf != null)
                        {
                            ViewState["SelectedContact"] = hf.Value;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// nuevo registro
        /// Establecer el registro seleccionado.
        /// </summary>
        private void SetSelectedRecord()
        {
            for (int i = 0; i < gv_SubDetalles_interventores.Rows.Count; i++)
            {
                RadioButton rb = (RadioButton)gv_SubDetalles_interventores.Rows[i].Cells[0]
                                                .FindControl("rb_interv_lider");
                if (rb != null)
                {
                    HiddenField hf = (HiddenField)gv_SubDetalles_interventores.Rows[i]
                                        .Cells[0].FindControl("HiddenField1");
                    if (hf != null && ViewState["SelectedContact"] != null)
                    {
                        if (hf.Value.Equals(ViewState["SelectedContact"].ToString()))
                        {
                            rb.Checked = true;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Métodos de la izquierda.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Cargar el DropDownList "dd_Sectores" con la información cargada de la consulta SQL.
        /// </summary>
        private void CargarDropDownListSectores()
        {
            //Inicializar variables.
            String sqlConsulta = "";
            ListItem item_defecto = new ListItem();

            try
            {
                if (dd_Sectores.Items.Count > 0)
                { dd_Sectores.Items.Clear(); /*Limpiar "en caso de que tenga ítems" para evitar que se repitan.*/}

                //Agregar ítem por defecto.
                item_defecto = new ListItem();
                item_defecto.Value = "[ALL]"; //revisar cómo hace la consulta para cargar TODOS los sectores...
                item_defecto.Text = "[Todos]";
                dd_Sectores.Items.Add(item_defecto);

                //Consulta:
                sqlConsulta = "SELECT Id_Sector, NomSector FROM Sector ORDER BY NomSector";

                //Asignar resultados de la consulta a la variable DataTable.
                var llenarListado = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Recorrer la tabla con datos para crear por cada elemento, un ListItem
                //y luego adicionarlo al DropDownList.
                for (int i = 0; i < llenarListado.Rows.Count; i++)
                {
                    //Instancia de ListItem
                    ListItem item = new ListItem();
                    item.Value = llenarListado.Rows[i]["Id_Sector"].ToString();
                    item.Text = llenarListado.Rows[i]["NomSector"].ToString();
                    dd_Sectores.Items.Add(item);
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Método que bindea la grilla de acuerdo con la selección del valor en el 
        /// DropDownList "dd_Sectores".
        /// </summary>
        private void ObtenerCodSubSector()
        {
            //Inicializar variables.
            String sqlConsulta = "";
            DataTable tabla_final = new DataTable();
            tabla_final = CrearDataTable();

            try
            {
                if (dd_Sectores.SelectedValue == "[ALL]")
                {
                    #region Procesar la información cuando se requieren ver TODOS los sectores.

                    //Primera consulta: //I use the exact field which i want to use in the query instead "*".
                    sqlConsulta = "SELECT Id_Proyecto FROM Proyecto WHERE CodEstado >= " + Constantes.CONST_Ejecucion;

                    //Asignar resultados de la primera consulta.
                    var sql_1 = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Recorrer los resultados de la primera consulta para ejecutar la segunda consulta.
                    for (int i = 0; i < sql_1.Rows.Count; i++)
                    {
                        //Segunda consulta: //I use the exact fields which i want to use in the query instead "*".
                        sqlConsulta = "SELECT Id_Empresa, CodProyecto, RazonSocial FROM Empresa WHERE CodProyecto = " + sql_1.Rows[i]["Id_Proyecto"].ToString();

                        //Asignar resultados de la segunda consulta:
                        var sql_2 = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Recorro los resultados de la segunda consulta para añadir los elementos a la tabla.
                        for (int j = 0; j < sql_2.Rows.Count; j++)
                        {
                            try { tabla_final.Rows.Add(dd_Sectores.SelectedValue, sql_2.Rows[j]["CodProyecto"].ToString(), sql_2.Rows[j]["RazonSocial"].ToString()); }
                            catch { /*Al parecer, como no hay algunos datos en las consultas, sale error, se coloca try/catch para evitar problemas.*/ }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region Procesar la información cuando el valor seleccionado NO sea [Todos].

                    //Primera consulta.
                    sqlConsulta = "SELECT * FROM SubSector WHERE CodSector = " + dd_Sectores.SelectedValue;

                    //Asignar resultados de la primera consulta.
                    var sql_1 = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Recorrer los datos de la primera consulta para ejecutar la segunda consulta.
                    for (int i = 0; i < sql_1.Rows.Count; i++)
                    {
                        //Segunda consulta:
                        sqlConsulta = " SELECT * FROM Proyecto " +
                                      " WHERE CodSubSector = " + sql_1.Rows[i]["Id_SubSector"].ToString() +
                                      " AND CodEstado >= " + Constantes.CONST_Ejecucion;

                        //Asignar resultados de la segunda consulta.
                        var sql_2 = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Si la segunda consulta tiene datos...
                        if (sql_2.Rows.Count > 0)
                        {
                            //Recorrer los datos de la segunda consulta para ejecutar la tercera consulta.
                            for (int j = 0; j < sql_2.Rows.Count; j++)
                            {
                                //Tercera consulta:
                                sqlConsulta = "SELECT * FROM Empresa WHERE CodProyecto = " + sql_2.Rows[j]["Id_Proyecto"].ToString();

                                //Asignar resultados de la tercera consulta.
                                var sql_3 = consultas.ObtenerDataTable(sqlConsulta, "text");

                                //Si la tercera consulta tiene datos...
                                if (sql_3.Rows.Count > 0)
                                {
                                    for (int k = 0; k < sql_3.Rows.Count; k++)
                                    {
                                        //Añadir determinados elementos a la tabla.

                                        //Código del Sector padre (primero) seleccionado
                                        //Código del proyecto de la TERCERA consulta.
                                        //Razón Social de la TERCERA consulta.

                                        try { tabla_final.Rows.Add(dd_Sectores.SelectedValue, sql_3.Rows[k]["CodProyecto"].ToString(), sql_3.Rows[k]["RazonSocial"].ToString()); }
                                        catch { /*Al parecer, como no hay algunos datos en las consultas, sale error, se coloca try/catch para evitar problemas.*/ }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }

                //Bindear finalmente la grilla.
                gv_sectores_encontrados.DataSource = tabla_final;
                gv_sectores_encontrados.DataBind();
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Evento que al seleccionar un ítem del DropDownList, oculta el
        /// posiblemente "visible" label que indica que debe seleccionar un sector de la lista.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dd_Sectores_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnl_seleccione_sector.Visible = false;
            ObtenerCodSubSector();
            //Ocultar los demás campos que podrían estar visibles.
            gv_detalles_interventor.Visible = false;
            gv_interventoresCreados.Visible = false; //Grilla de interventores creados.
        }

        #region Métodos de la grilla "gv_sectores_encontrados".

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Establecer texto en el campo correspondiente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_sectores_encontrados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Inicializar variables internas.
                var lnk = e.Row.FindControl("lnk_btn_sector_seleccionar") as LinkButton;
                var hdf = e.Row.FindControl("hdf_nmb") as HiddenField;

                if (lnk != null && hdf != null)
                { lnk.Text = hdf.Value; }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Mostrar los campos de la izquierda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_sectores_encontrados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "mostrar")
            {
                //Separar los datos.
                string palabras = e.CommandArgument.ToString();
                string[] arr_palabras = palabras.Split(';');

                //Mostrar los páneles de la izquierda con sus respectivos datos.
                pnl_info.Visible = false;
                lbl_info_detalles_sector.Visible = true;
                gv_detalles_interventor.Visible = true;
                Session["Cod_Emp_TMP"] = arr_palabras[1];
                CargarInfoEmpresa(arr_palabras[1]);
                //Sumar esta variable en sesión para usarla
                Session["codproyecto_rowcommand"] = arr_palabras[1];

                //Ocultar los controles del botón "Actualizar".
                pnl_btn_actualizar.Visible = false;
                btn_Actualizar_Interventores.Visible = false;

                //Mostrar los demás datos...
                if (lnkbtn_asignarinterventor.Visible == false)
                { lnkbtn_asignarinterventor.Visible = true; }

                //Ocultar sub-grilla de detalles.
                if (gv_SubDetalles_interventores.Visible == true)
                { gv_SubDetalles_interventores.Visible = false; }

                ////Ocultar la grilla de interventores creados.
                //if (gv_interventoresCreados.Visible == true)
                //{ gv_interventoresCreados.Visible = false; }
            }
        }

        #endregion

        #endregion

        #region Métodos de la derecha.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Cargar la información mas detallada de la empresa/sector seleccionado.
        /// 08/05/2014: Se bindea también la grilla "gv_interventoresCreados" al llamar al método "CargarInterventoresCreados".
        /// </summary>
        /// <param name="CodProyecto_SectorSeleccionado">Código del proyecto del sector seleccionado de la izquierda de la pantalla.</param>
        private void CargarInfoEmpresa(String CodProyecto_SectorSeleccionado)
        {
            //Inicializar variables.
            String sqlConsulta = "";

            try
            {
                #region Obtener la información de la empresa del sector seleccionado.
                //Primera consulta.
                sqlConsulta = " SELECT id_empresa, razonsocial, Nit, ObjetoSocial " +
                                  " FROM Empresa WHERE codproyecto = " + CodProyecto_SectorSeleccionado;

                //Asignar resultados a variable DataTable.
                var sql_1 = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Obtener lo siguientes valores y asignarlos a los HiddenFields:
                hdf_IdEmpresa.Value = sql_1.Rows[0]["id_empresa"].ToString();
                hdf_RazonSocial.Value = sql_1.Rows[0]["razonsocial"].ToString();
                hdf_Nit.Value = sql_1.Rows[0]["Nit"].ToString();
                hdf_ObjSocial.Value = sql_1.Rows[0]["ObjetoSocial"].ToString();

                lbl_info_detalles_sector.Text = "<strong>Empresa para el Plan de Negocio: </strong><br />" +
                                                "<span class='antetitulo'>Razón Social - " + hdf_RazonSocial.Value + "</span><br />" +
                                                "<span class='antetitulo'>Nit - " + hdf_Nit.Value + "</span><br />" +
                                                "<span class='antetitulo'>Objeto Social - " + hdf_ObjSocial.Value + "</span><br/>" +
                                                "<span class='antetitulo'>CIUU - " + dd_Sectores.SelectedItem.Text + "<br/><br/>";
                #endregion

                #region Generar la grilla.

                //Reiniciar la variable.
                sqlConsulta = "";
                //Segunda consulta.
                sqlConsulta = " SELECT Contacto.Id_Contacto, Contacto.Nombres+ ' ' +  Contacto.Apellidos AS Nombres, Contacto.Email " +
                              " FROM dbo.Contacto INNER JOIN EmpresaInterventor " +
                              " ON Contacto.Id_Contacto = EmpresaInterventor.CodContacto INNER JOIN Empresa " +
                              " ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa" +
                              " WHERE (Empresa.codproyecto = " + CodProyecto_SectorSeleccionado + ")" +
                              " AND (EmpresaInterventor.Rol = 8)" +
                              " AND (EmpresaInterventor.Inactivo = 0)";

                //Asignar resultados a variable DataTable.
                var sql_2 = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Bindear la grilla.
                gv_detalles_interventor.DataSource = sql_2;
                gv_detalles_interventor.DataBind();

                #endregion

                //Llamada al evento para bindear la grilla "este método muestra o no la grilla dependiendo de si tiene datos".
                CargarInterventoresCreados(CodProyecto_SectorSeleccionado);
            }
            catch { string err = ""; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Cargar la sub-grilla de contactos para "asignarles interventores"...
        /// </summary>
        private void CargarContactos_AsignarInterventor()
        {
            //Inicializar variables.
            String sqlConsulta = "";

            try
            {
                //Consulta.
                sqlConsulta = " SELECT DISTINCT C.Id_Contacto, C.Nombres, C.Apellidos, C.Inactivo " +
                              " FROM Contacto C INNER JOIN InterventorSector PC " +
                              " ON C.Id_Contacto = PC.CodContacto " +
                              " AND PC.CodSector = " + dd_Sectores.SelectedValue + "INNER JOIN Interventor " +
                              " ON C.Id_Contacto = Interventor.CodContacto " +
                              " WHERE (Interventor.CodCoordinador = " + usuario.IdContacto + ") " +
                              " ORDER BY C.Nombres, C.Apellidos ";

                //Asignar valores a variable DataTable.
                var sql_subGrilla = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Bindear la grilla.
                gv_SubDetalles_interventores.DataSource = sql_subGrilla;
                gv_SubDetalles_interventores.DataBind();

            }
            catch { string err = ""; }
        }

        #region Métodos de la grilla "gv_interventoresCreados".

        /// <summary>
        /// Mauricio Arias Olave.
        /// 08/05/2014.
        /// Cargar los interventores creados, se asignan a la grilla "gv_interventoresCreados" y la vuelve
        /// invisible en caso de que no tenga datos.
        /// </summary>
        /// <param name="CodProyecto_SectorSeleccionado"></param>
        private void CargarInterventoresCreados(String CodProyecto_SectorSeleccionado)
        {
            //Inicializar variables.
            String sqlConsulta = "";

            try
            {
                //Consulta.
                sqlConsulta = " SELECT EmpresaInterventor.* FROM EmpresaInterventor " +
                              " INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                              " WHERE " +
                              "     ( " +
                              "         EmpresaInterventor.CodContacto IN " +
                              "         ( " +
                              "         SELECT id_contacto FROM Contacto C " +
                              "         INNER JOIN InterventorSector PC ON C.Id_Contacto = PC.CodContacto " +
                              "         ) " +
                              "     ) " +
                              " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventor + ") " +
                              " AND (EmpresaInterventor.Inactivo = 0) " +
                              " AND (Empresa.codproyecto = " + CodProyecto_SectorSeleccionado + ")";

                var tabla_conteo = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Si tiene datos, debe mostrar la SEGUNDA GRILLA.
                if (tabla_conteo.Rows.Count > 0)
                {
                    //Generar consulta para la segunda grilla.
                    sqlConsulta = ""; //Se limpia la variable "por si algo"...
                    sqlConsulta = " SELECT Contacto.Id_Contacto, Contacto.Nombres + ' ' + Contacto.Apellidos AS DT_FULLNAME, Contacto.Email " +
                                  " FROM dbo.Contacto INNER JOIN EmpresaInterventor " +
                                  " ON Contacto.Id_Contacto = EmpresaInterventor.CodContacto INNER JOIN Empresa " +
                                  " ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                  " WHERE (Empresa.codproyecto = " + CodProyecto_SectorSeleccionado + ") " +
                                  " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventor + ") " +
                                  " AND (EmpresaInterventor.Inactivo = 0)";

                    //Asignar los resultados de la consulta anterior a variable DataTable que será 
                    //usada para bindear la grilla.
                    var tabla_result = consultas.ObtenerDataTable(sqlConsulta, "text");

                    //Bindear la grilla.
                    gv_interventoresCreados.Visible = true;
                    gv_interventoresCreados.DataSource = tabla_result;
                    gv_interventoresCreados.DataBind();
                }
                else
                {
                    //De lo contrario la grilla PERMANECE invisible.
                    gv_interventoresCreados.Visible = false;
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 08/07/2014.
        /// Se usa para realizar uan consulta que determina si tiene determinado valor y establecer el
        /// check en el CheckBox, y en el RadioButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SubDetalles_interventores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ///Para ejecutar esto, la grilla "gv_SubDetalles_interventores" DEBE TENER DATOS, 
            ///si tiene, debe tener el valor "Inactivo = FALSE / 0", se chequea el CheckBox ...
            ///Ver si también para el RadioButton...

            //Establecer primero el Check... = LISTO!
            //Establecer para el RadioButton.... = LISTO!

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Inicializar variables.
                String consulta_string = "";
                DataTable result_consulta_string = new DataTable();
                var check = e.Row.FindControl("chk_objeto") as CheckBox;
                var radio = e.Row.FindControl("rb_interv_lider") as RadioButton;
                var cntct = e.Row.FindControl("hdf_contacto") as HiddenField;
                var inact = e.Row.FindControl("hdf_inactivo_inter") as HiddenField;
                string cod_proyecto_session = Session["codproyecto_rowcommand"].ToString();

                if (check != null && inact != null && cntct != null && radio != null)
                {
                    if (inact.Value == "False" || inact.Value == "0")
                    {
                        #region Establecer Check del CheckBox.
                        consulta_string = "";
                        consulta_string = " SELECT EmpresaInterventor.CodContacto FROM EmpresaInterventor" +
                                          " INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa" +
                                          " WHERE (EmpresaInterventor.CodContacto = " + cntct.Value + ")" +
                                          " AND (EmpresaInterventor.Rol IN (" + Constantes.CONST_RolInterventor + "," + Constantes.CONST_RolInterventorLider + "))" +
                                          " AND (EmpresaInterventor.Inactivo = 0)" +
                                          " AND (Empresa.codproyecto = " + cod_proyecto_session + ")";

                        result_consulta_string = consultas.ObtenerDataTable(consulta_string, "text");

                        if (result_consulta_string.Rows.Count > 0)
                        { check.Checked = true; }

                        result_consulta_string = null;
                        consulta_string = null;
                        #endregion

                        #region Establecer Check del RadioButton.

                        consulta_string = "";
                        consulta_string = " SELECT EmpresaInterventor.CodContacto " +
                                          " FROM EmpresaInterventor INNER JOIN Empresa " +
                                          " ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                          " WHERE (EmpresaInterventor.CodContacto = " + cntct.Value + ") " +
                                          " AND (EmpresaInterventor.Rol IN (" + Constantes.CONST_RolInterventorLider + ")) " +
                                          " AND (EmpresaInterventor.Inactivo = 0) " +
                                          " AND (Empresa.codproyecto = " + cod_proyecto_session + ") ";

                        result_consulta_string = consultas.ObtenerDataTable(consulta_string, "text");

                        if (result_consulta_string.Rows.Count > 0)
                        { radio.Checked = true; }

                        result_consulta_string = null;
                        consulta_string = null;

                        #endregion
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/05/2014.
        /// Mostrar sub-grilla para asignar interventores.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkbtn_asignarinterventor_Click(object sender, EventArgs e)
        {
            CargarContactos_AsignarInterventor();
            gv_detalles_interventor.Visible = false;
            lnkbtn_asignarinterventor.Visible = false;
            gv_SubDetalles_interventores.Visible = true;
            gv_interventoresCreados.Visible = false; //Grilla de interventores creados.
            pnl_btn_actualizar.Visible = true;
            btn_Actualizar_Interventores.Visible = true;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 29/04/2014.
        /// Establecer funcionalidad RowCommand para las siguientes funciones.
        /// - Invocar ventana emergente con información del usuario seleccionado".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_SubDetalles_interventores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "mostrar_acreditador")
            {
                #region Código que separa el id del acreditado y su nombre para añadir los valores a variables de sesión.
                //Separar los valores.
                var valores_command = new string[] { };
                valores_command = e.CommandArgument.ToString().Split(';');

                //Nueva línea de código para almacenar nombre del producto seleccionado en sesión.
                Session["IdAcreditador_Session"] = valores_command[0];
                Session["NombreAcreditador_Session"] = valores_command[1];
                #endregion

                #region Cargar el Id_Grupo del contacto seleccionado y asignarlo a la variable de sesión "CodRol_ActaAcreditacion".

                String sqlConsulta = "";

                //Consultar el CodGrupo del contacto seleccionado.
                sqlConsulta = "SELECT CodGrupo FROM GrupoContacto WHERE CodContacto = " + valores_command[0].ToString();

                //Asignar resultados de la consulta.
                var t = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Obtener el CodGrupo
                Session["CodRol_ActaAcreditacion"] = t.Rows[0]["CodGrupo"].ToString();

                #endregion

                #region Invocar ventana emergente con información del acreditador seleccionado.

                Redirect(null, "../MiPerfil/VerPerfilContacto.aspx", "_blank",
                    "menubar=0,scrollbars=1,width=710,height=430,top=100");

                #endregion
            }
        }

        /// <summary>
        /// Agregar Interventores...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Actualizar_Interventores_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta = "";
            string correcto;
            DateTime Fechainicio = DateTime.Today;

            try
            {
                //Obtener lo siguientes valores y asignarlos a los HiddenFields:
                if (hdf_IdEmpresa.Value.Trim() == "")
                { return; }

                #region Consulta de actualización. NOTA: Según FONADE clásico, esta consulta NO se ejecuta. (línea 84 de ListaInterventor.asp)

                #region Comentarios.
                //NOTA: Se modifica lo anterior.
                //sqlConsulta = " UPDATE EmpresaInterventor SET Fechafin = '" + DateTime.Today + "', Inactivo = 1 " +
                //              " WHERE CodEmpresa = " + hdf_IdEmpresa.Value +
                //              " AND Rol IN (" + Constantes.CONST_RolInterventor + "," + Constantes.CONST_RolInterventorLider + ")";

                ////Asignar SqlCommand para su ejecución.
                //cmd = new SqlCommand(sqlConsulta, conn);

                ////Ejecutar SQL.
                //correcto = String_EjecutarSQL(conn, cmd);

                //if (correcto != "")
                //{
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización (1).')", true);
                //    return;
                //} 
                #endregion

                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    cmd = new SqlCommand("MD_Update_EmpresaInterventor", con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Fechafin", DateTime.Today);
                    cmd.Parameters.AddWithValue("@CodEmpresa", hdf_IdEmpresa.Value);
                    cmd.Parameters.AddWithValue("@CONST_RolInterventor", Constantes.CONST_RolInterventor);
                    cmd.Parameters.AddWithValue("@CONST_RolInterventorLider", Constantes.CONST_RolInterventorLider);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
                catch { }
                #endregion

                //Recorrer la grilla para revisar por cada fila los valores a procesar.
                foreach (GridViewRow fila in gv_SubDetalles_interventores.Rows)
                {
                    CheckBox check = (CheckBox)fila.FindControl("chk_objeto");
                    RadioButton radio = (RadioButton)fila.FindControl("rb_interv_lider");
                    int idContacto_obtenido = Convert.ToInt32(gv_SubDetalles_interventores.DataKeys[fila.RowIndex].Value.ToString());
                    bool FueChequeado = false;

                    if (check.Checked == true)
                    {
                        #region Inserta los interventores, incluido el lider, aunque primero lo inserta como Interventor.

                        #region COMENTARIOS!
                        //sqlConsulta = " INSERT INTO EmpresaInterventor (codempresa, codcontacto, rol, Fechainicio) " +
                        //              " VALUES (" + hdf_IdEmpresa.Value + ", " + idContacto_obtenido + ", " + Constantes.CONST_RolInterventor + ",'" + Fechainicio + "')";

                        ////Asignar SqlCommand para su ejecución.
                        //cmd = new SqlCommand(sqlConsulta, conn);

                        ////Ejecutar SQL.
                        //correcto = String_EjecutarSQL(conn, cmd);

                        //if (correcto != "")
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción de interventor (1).')", true);
                        //    return;
                        //} 
                        #endregion

                        #region Ajustado.
                        try
                        {
                            //NEW RESULTS:
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                            cmd = new SqlCommand("MD_Update_EmpresaInterventor", con);

                            if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Fechafin", DateTime.Today);
                            cmd.Parameters.AddWithValue("@CodEmpresa", hdf_IdEmpresa.Value);
                            cmd.Parameters.AddWithValue("@CONST_RolInterventor", Constantes.CONST_RolInterventor);
                            cmd.Parameters.AddWithValue("@CONST_RolInterventorLider", Constantes.CONST_RolInterventorLider);
                            cmd.ExecuteNonQuery();
                            con.Close();
                            con.Dispose();
                            cmd.Dispose();
                        }
                        catch { }
                        #endregion

                        #endregion
                    }
                    if (radio.Checked == true && FueChequeado == false)
                    {
                        #region Flujo para procesar los interventores líderes seleccionados "RadioButtons".

                        //Consulta. "En lugar del asterisco (*), se usa el campo que se requiere".
                        sqlConsulta = " SELECT CodEmpresa FROM EmpresaInterventor WHERE Inactivo = 0 " +
                                      " AND CodContacto = " + idContacto_obtenido +
                                      " AND Rol = " + Constantes.CONST_RolInterventor +
                                      " AND CodEmpresa = " + hdf_IdEmpresa.Value;

                        //Asignar resultados de la consulta.
                        var sql_CodEmpresa = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Si tiene datos, actualiza, si no, actualiza otro dato e inserta.
                        if (sql_CodEmpresa.Rows.Count > 0)
                        {
                            #region Actualiza el código del Interventor Líder.

                            //Hago recorrido de datos "paar evitar que de pronto se ignoren datos.
                            for (int i = 0; i < sql_CodEmpresa.Rows.Count; i++)
                            {
                                #region Actualización del interventor líder (en "EmpresaInterventor").
                                sqlConsulta = " UPDATE EmpresaInterventor SET CodContacto = " + idContacto_obtenido + ", " +
                                                                      " Rol = " + Constantes.CONST_RolInterventorLider +
                                                                      " WHERE CodEmpresa = " + sql_CodEmpresa.Rows[i]["CodEmpresa"].ToString() +
                                                                      " AND CodContacto=" + idContacto_obtenido +
                                                                      " AND Rol= " + Constantes.CONST_RolInterventor;

                                //Asignar SqlCommand para su ejecución.
                                cmd = new SqlCommand(sqlConsulta, conn);

                                //Ejecutar SQL.
                                correcto = String_EjecutarSQL(conn, cmd);

                                if (correcto != "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en actualización de interventor (" + i + ").')", true);
                                    return;
                                }
                                #endregion
                            }

                            #endregion
                        }
                        else
                        {
                            #region Inserta un nuevo Interventor Líder a EmpresaInterventor. (Consulta UPDATE)

                            #region COMENTARIOS!.
                            ////Actualización.
                            //sqlConsulta = " UPDATE EmpresaInterventor SET Fechafin = '" + Fechainicio + "', " +
                            //              " Inactivo = 1 WHERE CodEmpresa=" + hdf_IdEmpresa.Value +
                            //              " AND Rol = " + Constantes.CONST_RolInterventorLider;

                            ////Asignar SqlCommand para su ejecución.
                            //cmd = new SqlCommand(sqlConsulta, conn);

                            ////Ejecutar SQL.
                            //correcto = String_EjecutarSQL(conn, cmd);

                            //if (correcto != "")
                            //{
                            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al insertar un nuevo interventor líder (1).')", true);
                            //    return;
                            //} 
                            #endregion

                            #region Ajustado.
                            try
                            {
                                //NEW RESULTS:
                                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                                cmd = new SqlCommand("MD_Update_EmpresaInterventor", con);

                                if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Fechafin", DateTime.Today);
                                cmd.Parameters.AddWithValue("@CodEmpresa", hdf_IdEmpresa.Value);
                                cmd.Parameters.AddWithValue("@CONST_RolInterventor", Constantes.CONST_RolInterventor);
                                cmd.Parameters.AddWithValue("@CONST_RolInterventorLider", Constantes.CONST_RolInterventorLider);
                                cmd.ExecuteNonQuery();
                                con.Close();
                                con.Dispose();
                                cmd.Dispose();
                            }
                            catch { }
                            #endregion

                            #endregion

                            #region Inserción en EmpresaInterventor. (Consulta INSERT)

                            #region COMENTARIOS!
                            ////Actualización.
                            //sqlConsulta = " INSERT INTO EmpresaInterventor (codempresa,codcontacto,rol,Fechainicio) " +
                            //              " VALUES (" + hdf_IdEmpresa.Value + ", " + idContacto_obtenido + ", " + Constantes.CONST_RolInterventorLider + ",'" + Fechainicio + "')";

                            ////Asignar SqlCommand para su ejecución.
                            //cmd = new SqlCommand(sqlConsulta, conn);

                            ////Ejecutar SQL.
                            //correcto = String_EjecutarSQL(conn, cmd);

                            //if (correcto != "")
                            //{
                            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al insertar un nuevo interventor líder (2).')", true);
                            //    return;
                            //} 
                            #endregion

                            #region Ajustado.
                            try
                            {
                                //NEW RESULTS:
                                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                                cmd = new SqlCommand("MD_Insertar_New_EmpresaInterventor", con);

                                if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@CodEmpresa", hdf_IdEmpresa.Value);
                                cmd.Parameters.AddWithValue("@CodContacto", idContacto_obtenido);
                                cmd.Parameters.AddWithValue("@Rol", Constantes.CONST_RolInterventorLider);
                                cmd.Parameters.AddWithValue("@FechaInicio", Fechainicio);
                                cmd.ExecuteNonQuery();
                                con.Close();
                                con.Dispose();
                                cmd.Dispose();
                            }
                            catch { }
                            #endregion

                            #endregion
                        }

                        #endregion

                        //Determina que sólo un registro puede ser interventor líder.
                        FueChequeado = true;
                    }
                }

                ///Al terminar el flujo, según el FONADE clásico, la página se "recarga".
                ///Response.Write "window.parent.location = 'FrameinterventorEmpresas.asp?CIUU=" & fnrequest("CIUU") & "&CodProyecto=" & CodProyecto & "';"&VbCrLf
                ///PERO en este caos, no es necesario, porque con el código anterior si busca que la información seleccionada
                ///sea la misma...
                ObtenerCodSubSector();
                //Mostrar el título por defecto.
                pnl_info.Visible = true;
                lbl_info.Visible = true;
                //Se deja la pantalla como al inicio, pero con el DropDownList y resultados de la izquierda visibles.
                lbl_info_detalles_sector.Visible = true;
                CargarInfoEmpresa(Session["Cod_Emp_TMP"].ToString());
                gv_detalles_interventor.Visible = true;
                lnkbtn_asignarinterventor.Visible = true;
                gv_interventoresCreados.Visible = false; //Grilla de interventores creados.
                gv_SubDetalles_interventores.Visible = false;
                btn_Actualizar_Interventores.Visible = false; //Botón oprimido.
            }
            catch { }
        }

        #endregion
    }
}