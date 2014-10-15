// <Author>Diego Quiñonez</Author>
// <Fecha>14 - 03 - 2014</Fecha>
// <Archivo>EvaluacionActa.aspx.cs</Archivo>

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;
using Fonade.Negocio;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Datos;
using System.Configuration;
using Fonade.Account;
using LinqKit;
using AjaxControlToolkit;
using System.ComponentModel;

namespace Fonade.FONADE.evaluacion
{
    public partial class EvaluacionActa : Base_Page
    {
        Boolean estado;
        /// <summary>
        /// Variable que determina si puede editar o no.
        /// </summary>
        Boolean Permite_Editar;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (usuario.CodGrupo != Constantes.CONST_GerenteInterventor && usuario.CodGrupo != Constantes.CONST_CoordinadorInterventor && usuario.CodGrupo != Constantes.CONST_Interventor)
            {
                estado = true;
                Permite_Editar = true;
            }
            else
            {
                estado = false;
                Permite_Editar = false;
            }

            if (Permite_Editar)
            {
                p_apregar.Visible = true;
                p_apregar.Enabled = true;
            }
            else
            {
                p_apregar.Visible = false;
                p_apregar.Enabled = false;
            }

            if (!IsPostBack)
            { CargarActas(); }
        }

        #region Method Cargues

        private void CargarActas()
        {

            DataTable tabla_sql = new DataTable();
            String sqlConsulta = "";

            try
            {

                //Consulta.
                sqlConsulta = " SELECT E.Id_Acta ,E.NumActa ,E.NomActa ,c.NomConvocatoria ,e.publicado " +
                              " FROM evaluacionacta e JOIN convocatoria c ON  id_convocatoria = codconvocatoria " +
                              " ORDER BY NumActa ";

                //Asignar resultados a variable DataTable.
                tabla_sql = consultas.ObtenerDataTable(sqlConsulta, "text");
                Session["dtActas"] = tabla_sql;
                GrvActas.DataSource = tabla_sql;
                GrvActas.DataBind();
            }
            catch { }

            #region Comentado el código anterior a las modificaciones.
            //try
            //{
            //    if (usuario.CodGrupo == Constantes.CONST_GerenteInterventor || usuario.CodGrupo == Constantes.CONST_CoordinadorInterventor)
            //    {
            //        DataTable dtActas = consultas.ObtenerDataTable("MD_ObtenerActas");

            //        if (dtActas.Rows.Count != 0)
            //        {
            //            Session["dtActas"] = dtActas;
            //            GrvActas.DataSource = dtActas;
            //            GrvActas.DataBind();
            //        }
            //        else
            //        {

            //            DataTable dtActas1 = consultas.ObtenerDataTable("MD_ObtenerActas");

            //            if (dtActas1.Rows.Count != 0)
            //            {

            //                Session["dtActas"] = dtActas1;
            //                GrvActas.DataSource = dtActas1;
            //                GrvActas.DataBind();
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw new Exception(ex.Message);
            //} 
            #endregion
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

        #endregion

        #region NO BORRAR.

        //[WebMethod]
        //public static string Eliminar(int idacta)
        //{
        //    var consulta = new Datos.Consultas();

        //    string mensajeDeError;

        //    var entidad = consulta.Db.EvaluacionActas.FirstOrDefault(p => p.Id_Acta == idacta);

        //    if (entidad != null && (bool)(!entidad.publicado))
        //    {
        //        //Borrar todos los proyectos del acta

        //        var eproyecto = consulta.Db.EvaluacionActaProyectos.FirstOrDefault(p => p.CodActa == idacta);

        //        if (eproyecto != null)
        //        {
        //            consulta.Db.EvaluacionActaProyectos.DeleteOnSubmit(eproyecto);
        //            consulta.Db.SubmitChanges();
        //        }

        //        //Borrar el acta

        //        consulta.Db.EvaluacionActas.DeleteOnSubmit(entidad);
        //        consulta.Db.SubmitChanges();



        //        mensajeDeError = "ok";
        //    }
        //    else
        //    {
        //        mensajeDeError = "El registro no se puede eliminar";
        //    }


        //    return JsonConvert.SerializeObject(new
        //    {
        //        mensaje = mensajeDeError
        //    });
        //}

        #endregion

        #region Eventos Grid

        //private void metodoEliminar(String P_CodActividad)
        //{
        //    //Inicializar variables.
        //    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
        //    SqlCommand cmd = new SqlCommand();

        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {
        //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo eliminar la actividad seleccionada.')", true);
        //        return;
        //    }
        //}

        protected void GrvActasRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "NombreActa")
            {
                Session["idacta"] = e.CommandArgument.ToString().Split(';')[0];
                Session["publicado"] = e.CommandArgument.ToString().Split(';')[1];
                Response.Redirect("CrearActa.aspx");
            }
            if (e.CommandName == "eliminar")
            {
                EliminarActaSeleccionada(Convert.ToInt32(e.CommandArgument.ToString().Split(';')[0]));
                //BorrarProyecto();
            }
            //if (e.CommandName == "eliminar")
            //{
            //    metodoEliminar(e.CommandArgument.ToString());
            //}
        }

        protected void GrvActasPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["dtActas"] as DataTable;

            if (dt != null)
            {
                GrvActas.PageIndex = e.NewPageIndex;
                GrvActas.DataSource = dt;
                GrvActas.DataBind();
            }
        }

        protected void GrvActasSorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtActas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GrvActas.DataSource = Session["dtActas"];
                GrvActas.DataBind();
            }
        }

        protected void GrvActas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var link = e.Row.FindControl("imgborrar") as LinkButton;
                var publicado = e.Row.FindControl("publicado") as Label;
                var img = e.Row.FindControl("imgborrar") as LinkButton;

                if (link != null && img != null && publicado != null)
                {
                    if (publicado.Text == "1" || publicado.Text == "True")
                    {
                        //Ocultar el ImageButton.
                        link.Visible = false;
                        link.Enabled = false;
                        img.Visible = false;
                        img.Enabled = false;
                        img.Style.Add(HtmlTextWriterStyle.Cursor, "auto");
                    }
                    else
                    {
                        if (Permite_Editar)
                        {
                            link.Visible = true;
                            link.Attributes.Add("OnClick", "return alerta()");
                            link.Attributes.Add("OnClientClick", "return alerta()");
                            img.Visible = true;
                        }
                        else
                        {
                            //Visible pero NO estará habilitado para eliminar actas "como está en FONADE clásico".
                            link.Visible = true;
                            //Se eliminan los atributos que pueda tener...
                            link.Attributes.Clear();
                            link.Attributes.Add("OnClick", "return false;");
                            link.Enabled = false;
                            img.Visible = true;
                            img.Enabled = false;
                        }
                    }
                }
                Session["publicado"] = Convert.ToBoolean(publicado.Text);
            }
        }

        #endregion

        #region Eventos de los controles del formulario.

        protected void ImgBtn_Adicionar_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/FONADE/evaluacion/CrearActa.aspx?a=a");
        }

        protected void lnkBtn_Adicionar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/FONADE/evaluacion/CrearActa.aspx?a=a");
        }

        /// <summary>
        /// Eliminar el acta seleccionada.
        /// </summary>
        /// <param name="P_CodActa">Código del acta..</param>
        private void EliminarActaSeleccionada(Int32 P_CodActa)
        {
            //Inicializar variables.
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta;
            bool procesado = false;
            DataTable res = new DataTable();

            try
            {
                //Consultar si está publicado.
                sqlConsulta = "SELECT publicado FROM evaluacionacta WHERE Id_Acta = " + P_CodActa;

                res = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Si el conteo es cero, puede eliminar.
                if (res.Rows.Count > 0)
                {
                    procesado = Boolean.Parse(res.Rows[0]["publicado"].ToString());

                    if (!procesado)
                    {
                        #region Borrar todos los proyectos del acta.

                        sqlConsulta = "delete from evaluacionactaproyecto where codacta = " + P_CodActa;

                        try
                        {
                            //NEW RESULTS:
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                            cmd = new SqlCommand(sqlConsulta, con);

                            if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            con.Close();
                            con.Dispose();
                            cmd.Dispose();
                        }
                        catch { }
                        #endregion

                        #region Borrar el acta.

                        sqlConsulta = "delete from evaluacionacta where id_acta = " + P_CodActa;
                        try
                        {
                            //NEW RESULTS:
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                            cmd = new SqlCommand(sqlConsulta, con);

                            if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            con.Close();
                            con.Dispose();
                            cmd.Dispose();
                        }
                        catch { }

                        #endregion
                    }
                }

                //Recargar las actas.
                CargarActas();
            }
            catch
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: No se pudo eliminar el acta seleccionada.')", true);
                return;
            }
        }

        private bool EjecutarSQL(SqlConnection p_connection, SqlCommand p_cmd)
        {
            //Ejecutar controladamente la consulta SQL.
            try
            {
                p_connection.Open();
                p_cmd.ExecuteReader();
                p_connection.Close();
                return true;
            }
            catch
            { return false; }
            finally
            { p_connection.Close(); }
        }

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 15/07/2014.
        /// Borrar Proyecto seleccionado.
        /// ESTE MÉTODO DEBE SER EMPLEADO EN "CrearActa.aspx".
        /// </summary>
        /// <param name="CodActa">Código del acta.</param>
        /// <param name="CodProyecto">Código del proyecto.</param>
        private void BorrarProyecto(String CodActa, String CodProyecto)
        {
            //Inicializar variables.
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            bool procesado = false;
            String txtSQL = "";

            try
            {
                txtSQL = " delete from evaluacionactaproyecto where codacta = " + CodActa + " and codproyecto = " + CodProyecto;

                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    cmd = new SqlCommand(txtSQL, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Acta del proyecto eliminada.')", true);
                    return;
                }
                catch
                {
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo eliminar el proyecto del acta.')", true);
                    return;
                }
            }
            catch
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo eliminar el proyecto del acta.')", true);
                return;
            }
        }
    }
}