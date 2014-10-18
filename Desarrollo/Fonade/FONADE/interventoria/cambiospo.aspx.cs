using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace Fonade.FONADE.interventoria
{
    public partial class cambiospo : Negocio.Base_Page
    {
        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "FONDO EMPRENDER - CAMBIOS A PLANES OPERATIVOS";

            if (!IsPostBack)
            {
                //llenar("");
                CargarCambios_PO("");
            }
        }

        /// <summary>
        /// Establecer qué datos serán cargados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlfiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCambios_PO(ddlfiltro.SelectedValue);
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/05/2014.
        /// Cargar las grillas acerca de los cambios hechos.
        /// Se usa como reemplazo del método "llenar".
        /// </summary>
        /// <param name="opcion">Valor que determina si carga la información Modificada, Eliminada o Adicionada.</param>
        private void CargarCambios_PO(string opcion)
        {
            //Inicializar variables.
            String sqlConsulta = "";
            DataTable tabla_PO = new DataTable();
            DataTable tabla_Nomina = new DataTable();
            DataTable tabla_Produccion = new DataTable();
            DataTable tabla_Ventas = new DataTable();

            try
            {
                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                {
                    #region Plan Operativo.

                    if (opcion.Trim() != "")
                    {
                        //Cargar la información de acuerdo a la selección del DropDownList.
                        sqlConsulta = " SELECT ProyectoActividadPOInterventorTMP.Id_Actividad, ProyectoActividadPOInterventorTMP.NomActividad, " +
                                  " ProyectoActividadPOInterventorTMP.CodProyecto, ProyectoActividadPOInterventorTMP.Item,  " +
                                  " ProyectoActividadPOInterventorTMP.Tarea, " +
                                  " Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                  " FROM EmpresaInterventor  INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                  " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                  " INNER JOIN ProyectoActividadPOInterventorTMP ON Empresa.codproyecto = ProyectoActividadPOInterventorTMP.CodProyecto " +
                                  " INNER JOIN Contacto ON Interventor.CodContacto = Contacto.Id_Contacto " +
                                  " WHERE (ProyectoActividadPOInterventorTMP.ChequeoGerente IS NULL) " +
                                  " AND (ProyectoActividadPOInterventorTMP.ChequeoCoordinador IS NULL) " +
                                  " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                  " AND (EmpresaInterventor.Inactivo = 0) " +
                                  " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ") " +
                                  " AND (Tarea = '" + opcion + "')" +
                                  " ORDER BY ProyectoActividadPOInterventorTMP.NomActividad ASC ";

                        //Asignar resultados a variable DataTable
                        tabla_PO = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvactividadesplanoperativo.DataSource = tabla_PO;
                        gvactividadesplanoperativo.DataBind();
                    }
                    else
                    {
                        //Cargar la grilla sin filtros adicionales.
                        sqlConsulta = " SELECT ProyectoActividadPOInterventorTMP.Id_Actividad, ProyectoActividadPOInterventorTMP.NomActividad, " +
                                  " ProyectoActividadPOInterventorTMP.CodProyecto, ProyectoActividadPOInterventorTMP.Item,  " +
                                  " ProyectoActividadPOInterventorTMP.Tarea, " +
                                  " Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                  " FROM EmpresaInterventor  INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                  " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                  " INNER JOIN ProyectoActividadPOInterventorTMP ON Empresa.codproyecto = ProyectoActividadPOInterventorTMP.CodProyecto " +
                                  " INNER JOIN Contacto ON Interventor.CodContacto = Contacto.Id_Contacto " +
                                  " WHERE (ProyectoActividadPOInterventorTMP.ChequeoGerente IS NULL) " +
                                  " AND (ProyectoActividadPOInterventorTMP.ChequeoCoordinador IS NULL) " +
                                  " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                  " AND (EmpresaInterventor.Inactivo = 0) " +
                                  " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ") " +
                                  " ORDER BY ProyectoActividadPOInterventorTMP.NomActividad ASC ";

                        //Asignar resultados a variable DataTable
                        tabla_PO = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvactividadesplanoperativo.DataSource = tabla_PO;
                        gvactividadesplanoperativo.DataBind();
                    }

                    #endregion

                    #region Nómina.

                    if (opcion.Trim() != "")
                    {
                        //Cargar la información de acuerdo a la selección del DropDownList.
                        sqlConsulta = " SELECT InterventorNominaTMP.Id_Nomina, InterventorNominaTMP.Cargo, " +
                                      " InterventorNominaTMP.CodProyecto, InterventorNominaTMP.Tipo, InterventorNominaTMP.Tarea, " +
                                      " Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                      " FROM EmpresaInterventor  INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                      " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                      " INNER JOIN InterventorNominaTMP ON Empresa.codproyecto = InterventorNominaTMP.CodProyecto " +
                                      " INNER JOIN Contacto ON Interventor.CodContacto = Contacto.Id_Contacto " +
                                      " WHERE (InterventorNominaTMP.ChequeoGerente IS NULL) " +
                                      " AND (InterventorNominaTMP.ChequeoCoordinador IS NULL) " +
                                      " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                      " AND (EmpresaInterventor.Inactivo = 0) " +
                                      " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ")" +
                                      " AND (Tarea = '" + opcion + "')" +
                                      " ORDER BY InterventorNominaTMP.Tarea DESC ";

                        //Asignar resultados a variable DataTable
                        tabla_Nomina = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvcargosnomina.DataSource = tabla_Nomina;
                        gvcargosnomina.DataBind();
                    }
                    else
                    {
                        //Cargar la grilla sin filtros adicionales.
                        sqlConsulta = " SELECT InterventorNominaTMP.Id_Nomina, InterventorNominaTMP.Cargo, " +
                                      " InterventorNominaTMP.CodProyecto, InterventorNominaTMP.Tipo, InterventorNominaTMP.Tarea, " +
                                      " Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                      " FROM EmpresaInterventor  INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                      " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                      " INNER JOIN InterventorNominaTMP ON Empresa.codproyecto = InterventorNominaTMP.CodProyecto " +
                                      " INNER JOIN Contacto ON Interventor.CodContacto = Contacto.Id_Contacto " +
                                      " WHERE (InterventorNominaTMP.ChequeoGerente IS NULL) " +
                                      " AND (InterventorNominaTMP.ChequeoCoordinador IS NULL) " +
                                      " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                      " AND (EmpresaInterventor.Inactivo = 0) " +
                                      " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ")" +
                                      " ORDER BY InterventorNominaTMP.Tarea DESC ";

                        //Asignar resultados a variable DataTable
                        tabla_Nomina = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvcargosnomina.DataSource = tabla_Nomina;
                        gvcargosnomina.DataBind();
                    }

                    #endregion

                    #region Producción.

                    if (opcion.Trim() != "")
                    {
                        //Cargar la información de acuerdo a la selección del DropDownList.
                        sqlConsulta = " SELECT InterventorProduccionTMP.Id_Produccion, InterventorProduccionTMP.NomProducto, " +
                                      " InterventorProduccionTMP.CodProyecto, InterventorProduccionTMP.Tarea, " +
                                      " Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                      " FROM EmpresaInterventor INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                      " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                      " INNER JOIN InterventorProduccionTMP ON Empresa.codproyecto = InterventorProduccionTMP.CodProyecto " +
                                      " INNER JOIN Contacto ON Interventor.CodContacto = Contacto.Id_Contacto " +
                                      " WHERE (InterventorProduccionTMP.ChequeoGerente IS NULL) " +
                                      " AND (InterventorProduccionTMP.ChequeoCoordinador IS NULL) " +
                                      " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                      " AND (EmpresaInterventor.Inactivo = 0) " +
                                      " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ")" +
                                      " AND (Tarea = '" + opcion + "')" +
                                      " ORDER BY InterventorProduccionTMP.Tarea DESC ";

                        //Asignar resultados a variable DataTable
                        tabla_Produccion = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvproductosproduccion.DataSource = tabla_Produccion;
                        gvproductosproduccion.DataBind();
                    }
                    else
                    {
                        //Cargar la información sin filtros adicionales.
                        sqlConsulta = " SELECT InterventorProduccionTMP.Id_Produccion, InterventorProduccionTMP.NomProducto, " +
                                      " InterventorProduccionTMP.CodProyecto, InterventorProduccionTMP.Tarea, " +
                                      " Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                      " FROM EmpresaInterventor INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                      " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                      " INNER JOIN InterventorProduccionTMP ON Empresa.codproyecto = InterventorProduccionTMP.CodProyecto " +
                                      " INNER JOIN Contacto ON Interventor.CodContacto = Contacto.Id_Contacto " +
                                      " WHERE (InterventorProduccionTMP.ChequeoGerente IS NULL) " +
                                      " AND (InterventorProduccionTMP.ChequeoCoordinador IS NULL) " +
                                      " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                      " AND (EmpresaInterventor.Inactivo = 0) " +
                                      " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ")" +
                                      " ORDER BY InterventorProduccionTMP.Tarea DESC ";

                        //Asignar resultados a variable DataTable
                        tabla_Produccion = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvproductosproduccion.DataSource = tabla_Produccion;
                        gvproductosproduccion.DataBind();
                    }

                    #endregion

                    #region Ventas.

                    if (opcion.Trim() != "")
                    {
                        //Cargar la información de acuerdo a la selección del DropDownList.
                        sqlConsulta = " SELECT InterventorVentasTMP.Id_Ventas, InterventorVentasTMP.NomProducto, " +
                                      " InterventorVentasTMP.CodProyecto, InterventorVentasTMP.Tarea, " +
                                      " Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                      " FROM EmpresaInterventor INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                      " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                      " INNER JOIN InterventorVentasTMP ON Empresa.codproyecto = InterventorVentasTMP.CodProyecto " +
                                      " INNER JOIN Contacto ON Interventor.CodContacto = Contacto.Id_Contacto " +
                                      " WHERE (InterventorVentasTMP.ChequeoGerente IS NULL) " +
                                      " AND (InterventorVentasTMP.ChequeoCoordinador IS NULL) " +
                                      " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                      " AND (EmpresaInterventor.Inactivo = 0) " +
                                      " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ")" +
                                      " AND (Tarea = '" + opcion + "')" +
                                      " ORDER BY InterventorVentasTMP.Tarea DESC ";

                        //Asignar resultados a variable DataTable
                        tabla_Ventas = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvproductosventas.DataSource = tabla_Ventas;
                        gvproductosventas.DataBind();
                    }
                    else
                    {
                        //Cargar la información sin filtros adicionales.
                        sqlConsulta = " SELECT InterventorVentasTMP.Id_Ventas, InterventorVentasTMP.NomProducto, " +
                                      " InterventorVentasTMP.CodProyecto, InterventorVentasTMP.Tarea, " +
                                      " Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                      " FROM EmpresaInterventor INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                                      " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                      " INNER JOIN InterventorVentasTMP ON Empresa.codproyecto = InterventorVentasTMP.CodProyecto " +
                                      " INNER JOIN Contacto ON Interventor.CodContacto = Contacto.Id_Contacto " +
                                      " WHERE (InterventorVentasTMP.ChequeoGerente IS NULL) " +
                                      " AND (InterventorVentasTMP.ChequeoCoordinador IS NULL) " +
                                      " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                      " AND (EmpresaInterventor.Inactivo = 0) " +
                                      " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ")" +
                                      " ORDER BY InterventorVentasTMP.Tarea DESC ";

                        //Asignar resultados a variable DataTable
                        tabla_Ventas = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvproductosventas.DataSource = tabla_Ventas;
                        gvproductosventas.DataBind();
                    }

                    #endregion

                    #region Bloquear columnas de chequeo.

                    foreach (GridViewRow fila in gvactividadesplanoperativo.Rows)
                    {
                        if (!gvactividadesplanoperativo.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                        {
                            CheckBox box = (CheckBox)fila.FindControl("chckplanopera");
                            box.Enabled = false;
                        }
                    }
                    foreach (GridViewRow fila in gvcargosnomina.Rows)
                    {
                        if (!gvcargosnomina.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                        {
                            CheckBox box = (CheckBox)fila.FindControl("chcknomina");
                            box.Enabled = false;
                        }
                    }

                    foreach (GridViewRow fila in gvproductosproduccion.Rows)
                    {
                        if (!gvproductosproduccion.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                        {
                            CheckBox box = (CheckBox)fila.FindControl("chckproduccion");
                            box.Enabled = false;
                        }
                    }

                    foreach (GridViewRow fila in gvproductosventas.Rows)
                    {
                        if (!gvproductosventas.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                        {
                            CheckBox box = (CheckBox)fila.FindControl("chckventas");
                            box.Enabled = false;
                        }
                    }

                    #endregion
                }
                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    #region Plan Operativo.
                    if (opcion.Trim() != "")
                    {
                        //Cargar la información de acuerdo a la selección del DropDownList.
                        sqlConsulta = " SELECT ProyectoActividadPOInterventorTMP.Id_Actividad, ProyectoActividadPOInterventorTMP.NomActividad, " +
                                      " ProyectoActividadPOInterventorTMP.CodProyecto, ProyectoActividadPOInterventorTMP.Item, " +
                                      " ProyectoActividadPOInterventorTMP.Tarea, Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                      " FROM ProyectoActividadPOInterventorTMP " +
                                      " INNER JOIN Empresa ON ProyectoActividadPOInterventorTMP.CodProyecto = Empresa.codproyecto " +
                                      " INNER JOIN EmpresaInterventor ON Empresa.id_empresa = EmpresaInterventor.CodEmpresa " +
                                      " INNER JOIN Contacto ON EmpresaInterventor.CodContacto = Contacto.Id_Contacto " +
                                      " WHERE (ProyectoActividadPOInterventorTMP.ChequeoGerente IS NULL) " +
                                      " AND (ProyectoActividadPOInterventorTMP.ChequeoCoordinador = 1) " +
                                      " AND (EmpresaInterventor.Inactivo = 0) " +
                                      " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ")" +
                                      " AND (Tarea = '" + opcion + "')" +
                                      " ORDER BY ProyectoActividadPOInterventorTMP.NomActividad ASC ";

                        //Asignar resultados a variable DataTable
                        tabla_PO = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvactividadesplanoperativo.DataSource = tabla_PO;
                        gvactividadesplanoperativo.DataBind();
                    }
                    else
                    {
                        //Cargar la información sin filtros adicionales.
                        sqlConsulta = " SELECT ProyectoActividadPOInterventorTMP.Id_Actividad, ProyectoActividadPOInterventorTMP.NomActividad, " +
                                      " ProyectoActividadPOInterventorTMP.CodProyecto, ProyectoActividadPOInterventorTMP.Item, " +
                                      " ProyectoActividadPOInterventorTMP.Tarea, Empresa.razonsocial, Contacto.Nombres, Contacto.Apellidos " +
                                      " FROM ProyectoActividadPOInterventorTMP " +
                                      " INNER JOIN Empresa ON ProyectoActividadPOInterventorTMP.CodProyecto = Empresa.codproyecto " +
                                      " INNER JOIN EmpresaInterventor ON Empresa.id_empresa = EmpresaInterventor.CodEmpresa " +
                                      " INNER JOIN Contacto ON EmpresaInterventor.CodContacto = Contacto.Id_Contacto " +
                                      " WHERE (ProyectoActividadPOInterventorTMP.ChequeoGerente IS NULL) " +
                                      " AND (ProyectoActividadPOInterventorTMP.ChequeoCoordinador = 1) " +
                                      " AND (EmpresaInterventor.Inactivo = 0) " +
                                      " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ")" +
                                      " ORDER BY ProyectoActividadPOInterventorTMP.NomActividad ASC ";

                        //Asignar resultados a variable DataTable
                        tabla_PO = consultas.ObtenerDataTable(sqlConsulta, "text");

                        //Bindear la grilla.
                        gvactividadesplanoperativo.DataSource = tabla_PO;
                        gvactividadesplanoperativo.DataBind();
                    }
                    #endregion
                }
            }
            catch
            { }
        }

        private void llenar(string opc)
        {
            //var gvoperativo = consultas.Db.MD_CambiosPlanOperativo(usuario.IdContacto, usuario.CodGrupo, "OPERATIVO").OrderBy(po => po.NomActividad);

            //var gvnomina = consultas.Db.MD_CambiosPlanOperativo(usuario.IdContacto, usuario.CodGrupo, "NOMINA").OrderBy(no => no.Tarea);

            //var gvproduccion = consultas.Db.MD_CambiosPlanOperativo(usuario.IdContacto, usuario.CodGrupo, "PRODUCCION").OrderBy(pr => pr.Tarea);

            //var gvventas = consultas.Db.MD_CambiosPlanOperativo(usuario.IdContacto, usuario.CodGrupo, "VENTAS").OrderBy(vn => vn.Tarea);

            consultas.Parameters = new[]{
                new SqlParameter{
                    ParameterName = "@CodUsuario",
                    Value = usuario.IdContacto
                },
                new SqlParameter{
                    ParameterName = "@CodGrupo",
                    Value = usuario.CodGrupo
                },
                new SqlParameter{
                    ParameterName = "@opcion",
                    Value = "OPERATIVO"
                }
            };

            var gvoperativo = consultas.ObtenerDataTable("MD_CambiosPlanOperativo");

            consultas.Parameters = new[]{
                new SqlParameter{
                    ParameterName = "@CodUsuario",
                    Value = usuario.IdContacto
                },
                new SqlParameter{
                    ParameterName = "@CodGrupo",
                    Value = usuario.CodGrupo
                },
                new SqlParameter{
                    ParameterName = "@opcion",
                    Value = "NOMINA"
                }
            };

            var gvnomina = consultas.ObtenerDataTable("MD_CambiosPlanOperativo");

            consultas.Parameters = new[]{
                new SqlParameter{
                    ParameterName = "@CodUsuario",
                    Value = usuario.IdContacto
                },
                new SqlParameter{
                    ParameterName = "@CodGrupo",
                    Value = usuario.CodGrupo
                },
                new SqlParameter{
                    ParameterName = "@opcion",
                    Value = "PRODUCCION"
                }
            };

            var gvproduccion = consultas.ObtenerDataTable("MD_CambiosPlanOperativo");

            consultas.Parameters = new[]{
                new SqlParameter{
                    ParameterName = "@CodUsuario",
                    Value = usuario.IdContacto
                },
                new SqlParameter{
                    ParameterName = "@CodGrupo",
                    Value = usuario.CodGrupo
                },
                new SqlParameter{
                    ParameterName = "@opcion",
                    Value = "VENTAS"
                }
            };

            var gvventas = consultas.ObtenerDataTable("MD_CambiosPlanOperativo");


            if (!string.IsNullOrEmpty(opc))
            {
                string filtro = "Tipo_Solicitud = '" + opc + "'";

                var gvoperativo1 = gvoperativo.Select(filtro);
                var gvnomina1 = gvnomina.Select(filtro);
                var gvproduccion1 = gvproduccion.Select(filtro);
                var gvventas1 = gvventas.Select(filtro);

                gvactividadesplanoperativo.DataSource = gvoperativo1;
                gvactividadesplanoperativo.DataBind();
                gvcargosnomina.DataSource = gvnomina1;
                gvcargosnomina.DataBind();
                gvproductosproduccion.DataSource = gvproduccion1;
                gvproductosproduccion.DataBind();
                gvproductosventas.DataSource = gvventas1;
                gvproductosventas.DataBind();
            }
            else
            {
                var gvoperativo1 = gvoperativo;
                var gvnomina1 = gvnomina;
                var gvproduccion1 = gvproduccion;
                var gvventas1 = gvventas;

                gvactividadesplanoperativo.DataSource = gvoperativo1;
                gvactividadesplanoperativo.DataBind();
                gvcargosnomina.DataSource = gvnomina1;
                gvcargosnomina.DataBind();
                gvproductosproduccion.DataSource = gvproduccion1;
                gvproductosproduccion.DataBind();
                gvproductosventas.DataSource = gvventas1;
                gvproductosventas.DataBind();
            }

            desCkec();
        }

        private void desCkec()
        {
            foreach (GridViewRow fila in gvactividadesplanoperativo.Rows)
            {
                if (!gvactividadesplanoperativo.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                {
                    CheckBox box = (CheckBox)fila.FindControl("chckplanopera");
                    box.Enabled = false;
                }
            }
            foreach (GridViewRow fila in gvcargosnomina.Rows)
            {
                if (!gvcargosnomina.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                {
                    CheckBox box = (CheckBox)fila.FindControl("chcknomina");
                    box.Enabled = false;
                }
            }

            foreach (GridViewRow fila in gvproductosproduccion.Rows)
            {
                if (!gvproductosproduccion.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                {
                    CheckBox box = (CheckBox)fila.FindControl("chckproduccion");
                    box.Enabled = false;
                }
            }

            foreach (GridViewRow fila in gvproductosventas.Rows)
            {
                if (!gvproductosventas.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                {
                    CheckBox box = (CheckBox)fila.FindControl("chckventas");
                    box.Enabled = false;
                }
            }
        }

        protected void chectodos_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow fila in gvactividadesplanoperativo.Rows)
            {
                if (!gvactividadesplanoperativo.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                {
                    CheckBox box = (CheckBox)fila.FindControl("chckplanopera");
                    if (box.Enabled && ddlfiltro.SelectedItem.Text == "Borrar")
                        box.Checked = chectodos.Checked;
                }
            }
            foreach (GridViewRow fila in gvcargosnomina.Rows)
            {
                if (!gvcargosnomina.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                {
                    CheckBox box = (CheckBox)fila.FindControl("chcknomina");
                    if (box.Enabled && ddlfiltro.SelectedItem.Text == "Borrar")
                        box.Checked = chectodos.Checked;
                }
            }

            foreach (GridViewRow fila in gvproductosproduccion.Rows)
            {
                if (!gvproductosproduccion.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                {
                    CheckBox box = (CheckBox)fila.FindControl("chckproduccion");
                    if (box.Enabled && ddlfiltro.SelectedItem.Text == "Borrar")
                        box.Checked = chectodos.Checked;
                }
            }

            foreach (GridViewRow fila in gvproductosventas.Rows)
            {
                if (!gvproductosventas.DataKeys[fila.RowIndex].Value.ToString().Equals("Borrar"))
                {
                    CheckBox box = (CheckBox)fila.FindControl("chckventas");
                    if (box.Enabled && ddlfiltro.SelectedItem.Text == "Borrar")
                        box.Checked = chectodos.Checked;
                }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/05/2014.
        /// RowCommand de Plan Operativo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvactividadesplanoperativo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] param = e.CommandArgument.ToString().Split(';');

            Session["Accion"] = param[0];
            Session["CodProyecto"] = param[1];
            Session["CodActividad"] = param[2];

            //NEW Session. (Indica que al cargar este valor, ciertos valores en "CatalogoActividadPO" se volverán visibles).
            Session["Detalles_CambiosPO_PO"] = "PO";

            Redirect(null, "../Evaluacion/CatalogoActividadPO.aspx", "_Blank", "width=900,height=600");
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/05/2014.
        /// RowCommand de Nómina.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvcargosnomina_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] param = e.CommandArgument.ToString().Split(';');

            Session["Accion"] = param[0];
            Session["CodProyecto"] = param[1];
            Session["CodNomina"] = param[2];

            //NEW Session. (Indica que al cargar este valor, ciertos valores en "CatalogoInterventorTMP" se volverán visibles).
            Session["Detalles_CambiosPO_NO"] = "NO";

            Redirect(null, "../Evaluacion/CatalogoInterventorTMP.aspx", "_Blank", "width=730,height=585");
            //Redirect(null, "CatalogoInterventorTMP.aspx", "_Blank", "width=730,height=585");
        }

        protected void gvproductosproduccion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] param = e.CommandArgument.ToString().Split(';');

            Session["Accion"] = param[0];
            Session["CodProyecto"] = param[1];
            Session["CodProducto"] = param[2];

            //NEW Session. (Indica que al cargar este valor, ciertos valores en "CatalogoProduccionTMP" se volverán visibles).
            Session["Detalles_CambiosPO_PO"] = "NO";

            Redirect(null, "../Evaluacion/CatalogoProduccionTMP.aspx", "_Blank", "width=730,height=585");
        }

        protected void gvproductosventas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] param = e.CommandArgument.ToString().Split(';');

            Session["Accion"] = param[0];
            Session["CodProyecto"] = param[1];
            Session["CodProducto"] = param[2];

            //NEW Session. (Indica que al cargar este valor, ciertos valores en "CatalogoVentasTMP" se volverán visibles).
            Session["Detalles_CambiosPO_VO"] = "NO";

            Redirect(null, "CatalogoVentasTMP.aspx", "_Blank", "width=730,height=585");
        }
    }
}