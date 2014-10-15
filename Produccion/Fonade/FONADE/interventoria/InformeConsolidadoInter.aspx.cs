using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
namespace Fonade.FONADE.interventoria
{
    public partial class InformeConsolidadoInter : Negocio.Base_Page
    {
        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "FONDO EMPRENDER - Informe Consolidado de Interventoría";

            if (!IsPostBack)
            {
                if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_RolInterventorLider)
                { pnl_adicionar_informe_visita.Visible = true; CargarListaEmpresas(); }

                llenarData();
            }
        }

        private void llenarData(string filtro = null)
        {
            //Inicializar variables.
            if (!string.IsNullOrEmpty(dd_Empresas.SelectedValue))
                filtro = dd_Empresas.SelectedValue;

            String txtSQL = "";
            DataTable tabla_sql = new DataTable();

            if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
            {
                txtSQL = " SELECT InterventorInformeFinal.* FROM InterventorInformeFinal " +
                         " INNER JOIN Interventor ON InterventorInformeFinal.CodInterventor = Interventor.CodContacto " +
                         " WHERE (Interventor.CodCoordinador = " + usuario.IdContacto + ") ";
            }
            if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
            {
                txtSQL = " SELECT InterventorInformeFinal.* FROM InterventorInformeFinal " +
                         " INNER JOIN Interventor ON InterventorInformeFinal.CodInterventor = Interventor.CodContacto ";
            }
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
                txtSQL = " SELECT * FROM InterventorInformeFinal " +
                         " WHERE codinterventor= " + usuario.IdContacto;
            }
            if (!string.IsNullOrEmpty(txtSQL))
                txtSQL = txtSQL + " AND InterventorInformeFinal.NomInterventorInformeFinal LIKE '" + filtro + "%'";

            txtSQL = txtSQL + " ORDER BY InterventorInformeFinal.NomInterventorInformeFinal";
            //Asignar resultados de la consulta a variable DataTable.
            tabla_sql = consultas.ObtenerDataTable(txtSQL, "text");

            //Añadir columna.
            tabla_sql.Columns.Add("EstadoVAL", typeof(System.String));

            int inicial = 0;
            int valor = 0;

            //Recorrer las filas del DataTable y añadir el valor correspondiente.
            foreach (DataRow row in tabla_sql.Rows)
            {
                valor = Convert.ToInt32(tabla_sql.Rows[inicial]["Estado"].ToString());

                switch (valor)
                {
                    case 0:
                        row["EstadoVAL"] = "Edición";
                        break;
                    case 1:
                        row["EstadoVAL"] = "Enviado a Coordinador";
                        break;
                    case 2:
                        row["EstadoVAL"] = "Aprobado Coordinador";
                        break;
                    case 3:
                        row["EstadoVAL"] = "Aprobado Gerente Interventor";
                        break;
                    default:
                        break;
                }

                inicial++;
            }

            //Crear variable de sesión con la información obtenida.
            Session["dtEmpresas"] = tabla_sql;

            //Bindear la grilla.
            gv_informesinterventoria.DataSource = tabla_sql;
            gv_informesinterventoria.DataBind();

            #region Comentarios NO BORRAR!.
            ////Recorrer la grilla para establecer valores en los campos Label.
            //foreach (GridViewRow fila in gv_informesinterventoria.Rows)
            //{
            //    Label numero = (Label)fila.FindControl("lblestado");

            //    try
            //    {
            //        if (numero != null)
            //        {
            //            switch (Convert.ToInt32(numero.Text))
            //            {
            //                case 0:
            //                    numero.Text = "Edición";
            //                    break;
            //                case 1:
            //                    numero.Text = "Enviado a Coordinador";
            //                    break;
            //                case 2:
            //                    numero.Text = "Aprobado Coordinador";
            //                    break;
            //                case 3:
            //                    numero.Text = "Aprobado Gerente Interventor";
            //                    break;
            //            }
            //        }
            //    }
            //    catch (FormatException) { }
            //} 
            #endregion

            #region Comentado.
            //if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor || usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor || usuario.CodGrupo == Constantes.CONST_Interventor)
            //{
            //    var result = from r in consultas.VerInformeConsolidadoInterventoria(usuario.CodGrupo, usuario.IdContacto)

            //                 select new
            //                 {
            //                     Id_InterventorInformeFinal = r.Id_InterventorInformeFinal,
            //                     Estado = r.Estado,
            //                     FechaInforme = r.FechaInforme,
            //                     CodEmpresa = r.CodEmpresa,
            //                     NomInterventorInformeFinal = r.NomInterventorInformeFinal
            //                 };

            //    Session["dtEmpresas"] = result;
            //    gv_informesinterventoria.DataSource = result;
            //    gv_informesinterventoria.DataBind();
            //}
            //foreach (GridViewRow fila in gv_informesinterventoria.Rows)
            //{
            //    Label numero = (Label)fila.FindControl("lblestado");

            //    try
            //    {
            //        if (numero != null)
            //        {
            //            switch (Convert.ToInt32(numero.Text))
            //            {
            //                case 0:
            //                    numero.Text = "Edición";
            //                    break;
            //                case 1:
            //                    numero.Text = "Enviado a Coordinador";
            //                    break;
            //                case 2:
            //                    numero.Text = "Aprobado Coordinador";
            //                    break;
            //                case 3:
            //                    numero.Text = "Aprobado Gerente Interventor";
            //                    break;
            //            }
            //        }
            //    }
            //    catch (FormatException) { }
            //} 
            #endregion
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/05/2014.
        /// Cargar el listado de empresas.
        /// </summary>
        private void CargarListaEmpresas()
        {
            //Inicializar variables.
            String sqlConsulta = "";
            DataTable tabla = new DataTable();

            try
            {
                //Limpiar DropDownList "por si algo"...
                dd_Empresas.Items.Clear();

                //Consulta:
                sqlConsulta = " SELECT id_empresa, razonsocial FROM Empresa " +
                              " WHERE (id_empresa IN (SELECT CodEmpresa FROM EmpresaInterventor " +
                              " WHERE Inactivo=0 AND CodContacto = " + usuario.IdContacto + ")) ORDER BY razonsocial ";

                //Asignar resultados de la consulta a variable DataTable.
                tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Crear ítem por defecto.
                ListItem item_default = new ListItem();
                item_default.Value = "";
                item_default.Text = "Seleccione la Empresa para Adicionar el Informe Consolidado";
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

        /// <summary>
        /// Se debe enviar la información de la tabla en uan variable se sesión
        /// para poder sortearlo.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
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

        protected void gv_informesinterventoria_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                string valoresArgument = Convert.ToString(e.CommandArgument);

                string[] argument = valoresArgument.Split(';');

                Session["CodInforme"] = argument[0];
                Session["CodEmpresa"] = argument[1];

                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    Response.Redirect("AgregarInformeFinalInterventoria.aspx");
                else
                    Response.Redirect("AgregarInformeFinalInterventoriaInter.aspx");
            }
        }

        /// <summary>
        /// Sortear la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_informesinterventoria_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gv_informesinterventoria.DataSource = dt;
                gv_informesinterventoria.DataBind();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/05/2014.
        /// Redirige al usuario a la página "AdicionarInformeFinalInterventoriaInter.aspx" para adicionar un nuevo informe final.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_adicionar_informe_visita_Click(object sender, EventArgs e)
        {
            //Si ha seleccionado una empresa para generarle el informe final, redirige al usuario a la página
            //para crear el nuevo informe final.
            if (dd_Empresas.SelectedValue != "")
            {
                //Crear variable de sesión con valor de la empresa seleccionada para generarle informe final.
                Session["CodEmpresa"] = dd_Empresas.SelectedValue;
                //Según FONADE clásico, SIEMPRE, al seleccionar una empresa del listado para ingresarle el informe final, 
                //En la pantalla destino se crea el registro.
                Session["Accion"] = "Nuevo";

                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                    Response.Redirect("AgregarInformeFinalInterventoria.aspx");
                else
                    Response.Redirect("AgregarInformeFinalInterventoriaInter.aspx");
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Seleccione la Empresa para Adicionar el Informe')", true);
                return;
            }
        }

        protected void gv_informesinterventoria_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                gv_informesinterventoria.PageIndex = e.NewPageIndex;
                gv_informesinterventoria.DataSource = dt;
                gv_informesinterventoria.DataBind();
            }
        }

        protected void ddlbuscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarData(ddlbuscar.SelectedValue);
        }
    }
}