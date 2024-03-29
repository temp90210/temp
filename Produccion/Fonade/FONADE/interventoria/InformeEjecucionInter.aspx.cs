﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
namespace Fonade.FONADE.interventoria
{
    public partial class InformeEjecucionInter : Negocio.Base_Page
     {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Establecer el título de la página.
            this.Page.Title = "FONDO EMPRENDER - Mis Proyectos para Seguimiento de INTERVENTORÍA";

            if (!IsPostBack)
            {
                llenarData();

                if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_RolInterventorLider)
                { pnl_adicionar_informe_visita.Visible = true; CargarListaEmpresas(); }
            }
        }

        private void llenarData(string filtro = null)
        {
            //Inicializar variables.
            if (!string.IsNullOrEmpty(dd_Empresas.SelectedValue))
                filtro = dd_Empresas.SelectedValue;

            String txtSQL = "";
            DataTable tabla = new DataTable();

            try
            {
                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    txtSQL = "";
                    txtSQL = " SELECT InformePresupuestal.* FROM InformePresupuestal " +
                             " INNER JOIN Interventor ON InformePresupuestal.codinterventor = Interventor.CodContacto " +
                             " WHERE (UPPER(InformePresupuestal.NomInformePresupuestal) LIKE '%%') ";
                }
                else if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    {
                        txtSQL = "";
                        txtSQL = " SELECT * FROM InformePresupuestal WHERE codinterventor = " + usuario.IdContacto +
                                 " AND (UPPER (NomInformePresupuestal) LIKE '%%') ORDER BY NomInformePresupuestal ";
                    }
                    else
                    {
                        txtSQL = " SELECT InformePresupuestal.* FROM InformePresupuestal " +
                                 " INNER JOIN Interventor ON InformePresupuestal.codinterventor = Interventor.CodContacto " +
                                 " WHERE (UPPER(InformePresupuestal.NomInformePresupuestal) LIKE '%%') " +
                                 " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ") ";
                    }
                if (!string.IsNullOrEmpty(txtSQL))
                    txtSQL = txtSQL + " AND InformePresupuestal.NomInformePresupuestal LIKE '" + filtro + "%'";

                txtSQL = txtSQL + " ORDER BY dbo.InformePresupuestal.NomInformePresupuestal";

                if (txtSQL.Trim() != "")
                {
                    //Asignar resultados de la consulta a variable DataTable.
                    tabla = consultas.ObtenerDataTable(txtSQL, "text");

                    Session["dtEmpresas"] = tabla;
                    gv_informesejecucion.DataSource = tabla;
                    gv_informesejecucion.DataBind();
                }
            }
            catch { }
        }

        private void CargarListaEmpresas()
        {
            //Inicializar variables.
            String txtSQL = "";
            DataTable tabla = new DataTable();

            try
            {
                //Limpiar DropDownList "por si algo"...
                dd_Empresas.Items.Clear();

                //Consulta:
                txtSQL = " SELECT id_empresa, razonsocial FROM Empresa " +
                              " WHERE (id_empresa IN (SELECT CodEmpresa FROM EmpresaInterventor " +
                              " WHERE Inactivo=0 AND CodContacto = " + usuario.IdContacto + ")) ORDER BY razonsocial ";

                //Asignar resultados de la consulta a variable DataTable.
                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                //Crear ítem por defecto.
                ListItem item_default = new ListItem();
                item_default.Value = "";
                item_default.Text = "Seleccione la Empresa para Adicionar el Informe Presupuestal";
                dd_Empresas.Items.Add(item_default);

                //Recorrer la lista y generar ListItems.
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    ListItem item = new ListItem();
                    item.Value = tabla.Rows[i]["id_empresa"].ToString();
                    item.Text = tabla.Rows[i]["razonsocial"].ToString();
                    dd_Empresas.Items.Add(item);
                }
            }
            catch
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo cargar el listado de empresas.')", true);
                return;
            }
        }

        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            var sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;

                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        protected void gv_informesejecucion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "mostrar")
            {
                try
                {
                    string valoresArgument = Convert.ToString(e.CommandArgument);

                    string[] argument = valoresArgument.Split(';');

                    Session["CodInforme"] = argument[0];
                    Session["Periodo"] = argument[1];
                    Session["CodEmpresa"] = argument[2];

                    if (usuario.CodGrupo == Constantes.CONST_Interventor)
                        Response.Redirect("AdicionarInformePresupuestal.aspx");
                    else
                        Response.Redirect("AdicionarInformePresupuestalInter.aspx");
                }
                catch (IndexOutOfRangeException) { }
            }
        }

        protected void gv_informesejecucion_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gv_informesejecucion.DataSource = dt;
                gv_informesejecucion.DataBind();
            }
        }

        #region Anterior código en LINQ que "intentaba" bindear la información.
        //protected void ldsejecucioninter_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        //{
        //    if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_GerenteInterventor || usuario.CodGrupo == Constantes.CONST_Interventor)
        //    {
        //        var result = from r in consultas.VerInformeEjecucionInterventoria(usuario.CodGrupo, usuario.IdContacto)

        //                     select new
        //                     {
        //                         id_informepresupuestal = r.id_informepresupuestal,
        //                         Periodo = r.Periodo,
        //                         codempresa = r.codempresa,
        //                         NomInformePresupuestal = r.NomInformePresupuestal
        //                     };

        //        e.Result = result.ToList();
        //    }
        //} 
        #endregion

        protected void gv_informesejecucion_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                gv_informesejecucion.PageIndex = e.NewPageIndex;
                gv_informesejecucion.DataSource = dt;
                gv_informesejecucion.DataBind();
            }
        }

        protected void btn_adicionar_informe_visita_Click(object sender, EventArgs e)
        {
            //Si ha seleccionado una empresa para generarle el informe de presupuesto, redirige al usuario a la página
            //para crear el nuevo informe presupuestal.
            if (dd_Empresas.SelectedValue != "")
            {
                //Crear variable de sesión con valor de la empresa seleccionada para generarle informe presupuestal.
                Session["CodEmpresa"] = dd_Empresas.SelectedValue;
                Session["NUEVO"] = "NUEVO";

                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    Response.Redirect("AdicionarInformePresupuestal.aspx");
                else
                    Response.Redirect("AdicionarInformePresupuestalInter.aspx");
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Seleccione la Empresa para Adicionar el Informe Presupuestal')", true);
                return;
            }
        }

        protected void ddlbuscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarData(ddlbuscar.SelectedValue);
        }
    }
}