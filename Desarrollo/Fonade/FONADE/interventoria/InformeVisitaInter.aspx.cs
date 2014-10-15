using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.FONADE.interventoria
{
    public partial class InformeVisitaInter : Negocio.Base_Page
    {
        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                llenarData();

                if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_RolInterventorLider)
                { pnl_adicionar_informe_visita.Visible = true; CargarListaEmpresas(); }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/05/2014.
        /// Modificaciones al código fuente; añadida paginación a la grilla.
        /// Añadir columna encontrado en:
        /// http://stackoverflow.com/questions/2312966/add-new-column-and-data-to-datatable-that-already-contains-data-c-sharp
        /// </summary>
        private void llenarData(string filtro = null)
        {
            #region Versión SQL "para admitir paginación".
            try
            {
                //Inicializar variables.
                if (!string.IsNullOrEmpty(dd_Empresas.SelectedValue))
                    filtro = dd_Empresas.SelectedValue;

                String sqlConsulta = "";
                DataTable tabla_sql = new DataTable();

                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor)
                {
                    //Consulta en caso de que sea Gerente Interventor.
                    sqlConsulta = " SELECT * FROM InformeVisitaInterventoria WHERE 1=1";
                }
                if (usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                {
                    //Consulta en caso de que sea Coordinador Interventor.
                    sqlConsulta = "";
                    sqlConsulta = " SELECT InformeVisitaInterventoria.* FROM InformeVisitaInterventoria " +
                                  " INNER JOIN Interventor " +
                                  " ON InformeVisitaInterventoria.CodInterventor = Interventor.CodContacto " +
                                  " WHERE (Interventor.CodCoordinador = " + usuario.IdContacto + ") ";
                }
                if (usuario.CodGrupo != Constantes.CONST_GerenteInterventor && usuario.CodGrupo != Constantes.CONST_CoordinadorInterventor)
                {
                    //Consulta en caso de que NO sea ni Gerente ni Coordinador Interventor.
                    sqlConsulta = "";
                    sqlConsulta = " SELECT * FROM InformeVisitaInterventoria " +
                                  " WHERE CodInterventor = " + usuario.IdContacto;
                }
                if (!string.IsNullOrEmpty(sqlConsulta))
                    sqlConsulta = sqlConsulta + " AND InformeVisitaInterventoria.NombreInforme LIKE '" + filtro + "%'";

                sqlConsulta = sqlConsulta + " ORDER BY InformeVisitaInterventoria.NombreInforme";
                //Asignar resultados de la consulta a variable DataTable.
                tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Inicializar variable que indica el inicio del incremento que se evalúa mas adelante.
                int inicial = 1;

                //Añadir columna adicional "para mostrar números".
                tabla_sql.Columns.Add("NUM", typeof(System.Int32));

                //Recorrer las filas del DataTable y añadir el valor incremental.
                foreach (DataRow row in tabla_sql.Rows)
                {
                    row["NUM"] = inicial;
                    inicial++;
                }

                //Crear variable de sesión con la información obtenida.
                Session["dtEmpresas"] = tabla_sql;

                //Asignar DataTable anterior a GridView "gv_informesinterventoria".
                gv_informesinterventoria.DataSource = tabla_sql;
                gv_informesinterventoria.DataBind();
            }
            catch
            { }

            #endregion

            #region Versión LINQ COMENTADA.
            #region Código comentado anterior a las modificaciones.
            //DataTable datatable = new DataTable();

            //consultas.Parameters = new[]
            //{
            //    new SqlParameter{
            //        ParameterName = "@CodGrupo",
            //        Value = usuario.CodGrupo
            //    },
            //    new SqlParameter{
            //        ParameterName = "@CodUsuario",
            //        Value = usuario.IdContacto
            //    }
            //}; 
            #endregion

            ////datatable = consultas.ObtenerDataTable("MD_InformeVistaInterventoria");
            //if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor || usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
            //{
            //    var result = from r in consultas.VerInformeVisitasInterventoria(usuario.CodGrupo, usuario.IdContacto)
            //                 orderby r.NombreInforme ascending
            //                 select new
            //                 {
            //                     Id_Informe = r.Id_Informe,
            //                     NombreInforme = r.NombreInforme
            //                 };

            //    gv_informesinterventoria.DataSource = result;
            //    gv_informesinterventoria.DataBind();
            //}
            //foreach (GridViewRow fila in gv_informesinterventoria.Rows)
            //{
            //    Label numero = (Label)fila.FindControl("lbl_numero");

            //    if (numero != null)
            //    {
            //        numero.Text = Convert.ToString(indexer + 1);
            //        indexer += 1;
            //    }
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
                sqlConsulta = " SELECT id_empresa, razonsocial " +
                              " FROM Empresa WHERE (id_empresa IN (SELECT CodEmpresa FROM EmpresaInterventor " +
                              " WHERE CodContacto = " + usuario.IdContacto + ")) ORDER BY razonsocial ";

                //Asignar resultados de la consulta a variable DataTable.
                tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Crear ítem por defecto.
                ListItem item_default = new ListItem();
                item_default.Value = "";
                item_default.Text = "Seleccione la Empresa para Adicionar el Informe de Visita";
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
        /// Se debe enviar la información de la tabla en una variable se sesión
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

        /// <summary>
        /// RowCommand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_informesinterventoria_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MostrarInforme")
            {
                //Separar los valores.
                var valores_command = new string[] { };
                valores_command = e.CommandArgument.ToString().Split(';');
                Session["InformeIdVisita"] = valores_command[0]; //Convert.ToInt32(e.CommandArgument);
                Session["Nuevo"] = "False";
                Response.Redirect("AdicionarInformeVisita.aspx?Accion=Consultar");
            }
            Session["InformeIdVisita"] = "";
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/05/2014.
        /// Paginación de la grilla "gv_informesinterventoria".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Redirige al usuario a la página "AdicionarInformeVisita.aspx" para adicionar un nuevo informe de visita.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_adicionar_informe_visita_Click(object sender, EventArgs e)
        {
            //Si ha seleccionado una empresa para generarle el informe de visita, redirige al usuario a la página
            //para crear el nuevo informe de visita.
            if (dd_Empresas.SelectedValue != "")
            {
                Session["InformeIdVisita"] = dd_Empresas.SelectedValue;
                Session["Nombre_Empresa"] = dd_Empresas.SelectedItem.Text;
                Session["Nuevo"] = "Nuevo";
                Response.Redirect("AdicionarInformeVisita.aspx");
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Seleccione una Empresa para generar el informe de Visita')", true);
                return;
            }
        }

        protected void ddlbuscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarData(ddlbuscar.SelectedValue);
        }
    }
}