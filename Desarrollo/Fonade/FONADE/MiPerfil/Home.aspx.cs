using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Account;
using System.Data;

namespace Fonade.FONADE.MiPerfil
{
    public partial class Home : Negocio.Base_Page
    {
        int TotalFilasActuales = 0;
        int TotalFilas = 0;
        public const int PAGE_SIZE = 10;
        String txtSQL;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { CargarGrillaTareas(""); }
        }

        protected void gw_tareas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int nivelurgencia;
            nivelurgencia = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "NivelUrgencia"));
            ImageButton imbtton = new ImageButton();
            imbtton = ((ImageButton)e.Row.Cells[0].FindControl("btn_Inactivar"));
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (nivelurgencia == Constantes.CONST_PostIt) { imbtton.ImageUrl = "/Images/IcoPost.gif"; }
                else { imbtton.ImageUrl = "/Images/Tareas/Urgencia" + nivelurgencia + ".gif"; }
            }
        }

        protected void gw_tareas_DataBound(object sender, EventArgs e)
        {
            if (usuario.CodGrupo != Constantes.CONST_GerenteAdministrador && usuario.CodGrupo != Constantes.CONST_AdministradorFonade && usuario.CodGrupo != Constantes.CONST_AdministradorSena)
            {
                if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor || usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
                { gw_Tareas.Columns[2].HeaderText = "Empresa"; }
                else
                { gw_Tareas.Columns[2].HeaderText = "Plan de negocio"; }
            }
            else { gw_Tareas.Columns[2].Visible = false; }
        }

        protected void gw_Tareas_RowCreated(object sender, GridViewRowEventArgs e)
        {
            #region Comentarios.
            //if (e.Row.RowType == DataControlRowType.Header)
            //{
            //    foreach (TableCell tc in e.Row.Cells)
            //    {
            //        if (tc.HasControls())
            //        {
            //            // buscar el link del header
            //            //LinkButton lnk = (LinkButton)tc.Controls[0];
            //            //ImageButton lnk = (ImageButton)tc.Controls[0];
            //            object lnk = new object();
            //            LinkButton lnk_2 = new LinkButton();

            //            if (tc.HasControls()) { lnk = tc.Controls[0]; }

            //            if (lnk.GetType().Equals("System.Web.UI.WebControls.DataControlLinkButton"))
            //            { lnk_2 = (LinkButton)tc.Controls[0]; }

            //            if (lnk != null && gw_Tareas.SortExpression == lnk_2.CommandArgument)
            //            {
            //                // inicializar nueva imagen
            //                System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
            //                // url de la imagen dinamicamente
            //                img.ImageUrl = "/Images/ImgFlechaOrden" + (gw_Tareas.SortDirection == SortDirection.Ascending ? "Up" : "Down") + ".gif";
            //                // a ñadir el espacio de la imagen
            //                tc.Controls.Add(new LiteralControl(" "));
            //                tc.Controls.Add(img);
            //            }
            //        }
            //    }
            //} 
            #endregion

            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.HasControls())
                    {
                        // buscar el link del header
                        LinkButton lnk = (LinkButton)tc.Controls[0];
                        if (lnk != null && gw_Tareas.SortExpression == lnk.CommandArgument)
                        {
                            // inicializar nueva imagen
                            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                            // url de la imagen dinamicamente
                            img.ImageUrl = "/Images/ImgFlechaOrden" + (gw_Tareas.SortDirection == SortDirection.Ascending ? "Up" : "Down") + ".gif";
                            // a ñadir el espacio de la imagen
                            tc.Controls.Add(new LiteralControl(" "));
                            tc.Controls.Add(img);

                        }
                    }
                }
            }
        }

        protected void lds_Tareas_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            DateTime FechaFiltro;
            DateTime FechaHoy = DateTime.Today;

            var query = (from t in consultas.Db.MD_Home(usuario.IdContacto, null, null, null)

                         select new
                         {
                             Id_TareaUsuario = t.Id_TareaUsuario,
                             NomTareaUsuario = t.NomTareaUsuario,
                             Id_Proyecto = t.Id_Proyecto,
                             NomProyecto = t.NomProyecto,
                             Fecha = t.Fecha,
                             Id_tareaRepeticion = t.Id_TareaUsuarioRepeticion,
                             usuarioAgendo = t.nombre,
                             PlanNegocio = t.NomProyecto,
                             NivelUrgencia = t.NivelUrgencia
                         });

            if (FiltrarFecha.SelectedValue == "Hoy")
            {
                FechaFiltro = FechaHoy;
                query = query.Where(t => t.Fecha == FechaFiltro).ToList();
            }
            if (FiltrarFecha.SelectedValue == "Dentro de 1 semana")
            {
                FechaFiltro = FechaHoy.AddDays(7);
                query = query.Where(t => t.Fecha >= DateTime.Today && t.Fecha <= FechaFiltro).ToList();
            }
            if (FiltrarFecha.SelectedValue == "Dentro de 2 semanas")
            {
                FechaFiltro = FechaHoy.AddDays(14);
                query = query.Where(t => t.Fecha >= DateTime.Today && t.Fecha <= FechaFiltro).ToList();
            }
            if (FiltrarFecha.SelectedValue == "Dentro de 1 mes")
            {
                FechaFiltro = FechaHoy.AddMonths(1);
                query = query.Where(t => t.Fecha >= DateTime.Today && t.Fecha <= FechaFiltro).ToList();
            }
            if (FiltrarFecha.SelectedValue == "Dentro de 6 meses")
            {
                FechaFiltro = FechaHoy.AddMonths(6);
                query = query.Where(t => t.Fecha >= DateTime.Today && t.Fecha <= FechaFiltro).ToList();
            }
            if (FiltrarFecha.SelectedValue == "Siempre")
            {
                FechaFiltro = FechaHoy;
                query = query.Where(t => t.Fecha <= FechaFiltro).ToList();
            }
            e.Arguments.TotalRowCount = query.Count();
            if (e.Arguments.TotalRowCount == 0)
            {
                gw_Tareas.EmptyDataTemplate.Equals("No tiene actividades para este rango de fechas");
            }
            //query = query.Skip(gw_Tareas.PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            e.Result = query;

            if (query.Count() == 0) { Lbl_Resultados.Text = query.Count() + " actividades."; }
            else { Lbl_Resultados.Text = gw_Tareas.PageSize + " de " + query.Count() + " actividades."; }
        }

        /// <summary>
        /// Paginación de la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gw_Tareas_PageIndexChanged(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["dt_tareas"] as DataTable;

            if (dt != null)
            {
                gw_Tareas.DataSource = dt;
                gw_Tareas.PageIndex = e.NewPageIndex;
                gw_Tareas.DataBind();
            }

            #region Comentarios anteriores.
            //gw_Tareas.PageSize = PAGE_SIZE;
            //gw_Tareas.PageIndex = e.NewPageIndex;
            //gw_Tareas.DataBind();
            //TotalFilasActuales = gw_Tareas.Rows.Count;
            //Lbl_Resultados.Text = TotalFilasActuales + "de" + TotalFilas + "Actividades"; 
            #endregion
        }

        /// <summary>
        /// Selección de filtro de la fecha.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FiltrarFecha_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ///Anterior a las modificaciones del 25/06/2014.
            //this.gw_Tareas.DataBind();

            //Nuevo código.
            if (FiltrarFecha.SelectedValue != "*") //Si es diferente de "Siempre"...
            {
                string addedSQL = " AND DATEDIFF(Day, Fecha, GetDate()) > - " + FiltrarFecha.SelectedValue;
                CargarGrillaTareas(addedSQL);
                addedSQL = null;
            }
        }

        #region Métodos generales.

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

        #endregion

        #region Métodos de Mauricio Arias Olave.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 25/06/2014.
        /// Cargar la grilla "gw_Tareas".
        /// </summary>
        /// <param name="addedSQL_query">SQL añadido (Dependiendo de la selección del DropDownList "FiltrarFecha").</param>
        private void CargarGrillaTareas(String addedSQL_query)
        {
            //Inicializar variables.
            DataTable dt = new DataTable();

            try
            {
                //Consulta SQL.
                txtSQL = " SELECT tu.Id_TareaUsuario, tur.Id_TareaUsuarioRepeticion AS [Id_tareaRepeticion], tu.NomTareaUsuario, tu.Descripcion, " +
                         " 	   tu.CodTareaPrograma, tu.RecordatorioEmail, tu.NivelUrgencia, Id_Proyecto, NomProyecto AS PlanNegocio,  " +
                         " 	   tu.RecordatorioPantalla, tu.RequiereRespuesta, tu.CodContactoAgendo, Fecha,  " +
                         " 	   convert(char(12),Fecha,107) +' '+ convert(char(8),Fecha,108) AS FechaHora,  " +
                         " 	   tur.Parametros, tp.NomTareaPrograma, tp.Ejecutable, tp.Icono,  " +
                         " 	   c.Nombres +' '+ c.Apellidos as [usuarioAgendo], c.Email   " +
                         " FROM TareaPrograma tp, TareaUsuarioRepeticion tur, Contacto c,  " +
                         " 	 Proyecto P RIGHT OUTER JOIN TareaUsuario tu  " +
                         " on p.Id_Proyecto=tu.CodProyecto  " +
                         " WHERE tur.FechaCierre IS NULL AND tu.CodContacto = " + usuario.IdContacto +
                         " AND tu.CodContactoAgendo = c.Id_Contacto AND tur.CodTareaUsuario = tu.Id_TareaUsuario  " +
                         " AND Id_TareaPrograma = CodTareaPrograma  " +
                         " AND DATEDIFF(Day, Fecha, GetDate()) > -1 ";

                //Si el parámetro NO está vacío, se le debe adicionar el SQL que carga por defecto.
                if (addedSQL_query.Trim() != "")
                { txtSQL = txtSQL + addedSQL_query; }

                //Adicionado el order by por defecto.
                txtSQL = txtSQL + " ORDER BY Fecha Asc";

                //Asignar resultados a variable DataTable.
                dt = consultas.ObtenerDataTable(txtSQL, "text");

                //Asignar DataTable a variable de sesión, bindear la grilla y destruir las variables.
                Session["dt_tareas"] = dt;
                gw_Tareas.DataSource = dt;
                gw_Tareas.DataBind();
                //Mostrar cuántas tareas hay.
                //Lbl_Resultados.Text = gw_Tareas.PageSize + " de " + dt.Rows.Count + " actividades.";
                if (dt.Rows.Count == 0) { Lbl_Resultados.Text = dt.Rows.Count + " actividades."; }
                else
                {
                    if (dt.Rows.Count == 1) { Lbl_Resultados.Text = dt.Rows.Count + " actividad."; }
                    else { Lbl_Resultados.Text = gw_Tareas.Rows.Count + " de " + dt.Rows.Count + " actividades."; }
                }
                dt = new DataTable();
            }
            catch { Lbl_Resultados.Text = "Hubo un problema al cargar la página, por favor recargue la página."; }
        }

        /// <summary>
        /// Sortear la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gw_Tareas_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dt_tareas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gw_Tareas.DataSource = Session["dt_tareas"];
                gw_Tareas.DataBind();
            }
        }

        #endregion

        protected void gw_Tareas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "mostrar_tarea")
            {
                Session["Id_tareaRepeticion"] = e.CommandArgument;
                Response.Redirect("~/Fonade/Tareas/TareasAgendar.aspx");
            }
            if (e.CommandName == "mostrar_proyecto")
            {
                Session["CodProyecto"] = e.CommandArgument.ToString();
                Response.Redirect("~/Fonade/Proyecto/ProyectoFrameSet.aspx");
            }
        }
    }
}
