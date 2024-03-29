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
    public partial class ListadoAcreditadores : Negocio.Base_Page
    {
        String lCodContacto = "";
        String lCodProyectos = "";
        String[] lArregloProyectos;
        String lCodProyecto = "";
        String lCodConvocatoria = "";
        String txtSQL;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //L_Fecha.Text = "" + DateTime.Now.Day + " Del Mes " + DateTime.Now.Month + " De " + DateTime.Now.Year;
            if (!IsPostBack)
            {
                CargarAcreditadores();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 18/06/2014.
        /// Cargar la grilla "GridView1".
        /// </summary>
        private void CargarAcreditadores()
        {
            try
            {
                txtSQL = " SELECT ID_CONTACTO, (NOMBRES + ' ' + APELLIDOS) 'NOMBRE'," +
                         " EMAIL, (SELECT COUNT(*)FROM PROYECTOCONTACTO WHERE ACREDITADOR=1 AND INACTIVO =0 " +
                         " AND CODCONTACTO = ID_CONTACTO) 'CANTIDAD' FROM CONTACTO WHERE FLAGACREDITADOR = 1 " +
                         " AND INACTIVO = 0 " +
                         " ORDER BY 'NOMBRE' ASC ";

                var dt = consultas.ObtenerDataTable(txtSQL, "text");

                Session["dtEmpresas_1"] = dt;
                GridView1.DataSource = dt;
                GridView1.DataBind();
                dt = null;
            }
            catch (Exception ex)
            {
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al cargar la tabla de acreditadores. Error: '" + ex.Message + ".)", true);
                return;
            }
        }

        protected void LB_Nombre_Click(object sender, EventArgs e)
        {
            Revisar(sender, e);
        }

        protected void LB_Emaoil_Click(object sender, EventArgs e)
        {
            Revisar(sender, e);
        }

        private void Revisar(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(Session["idConvocatoriaEval"].ToString()))
                {
                    Response.Redirect("AsignarProyectosAcreditadores");
                    return;
                }
            }
            catch (Exception)
            {
                Response.Redirect("AsignarProyectosAcreditadores");
                return;
            }

            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GV_FilaGrid = GridView1.Rows[indicefila];

            lCodContacto = "" + GridView1.DataKeys[GV_FilaGrid.RowIndex].Value.ToString();
            lCodProyectos = "" + Session["idCodigoPro"].ToString();
            lArregloProyectos = lCodProyectos.Split(' ');
            lCodConvocatoria = "" + Session["idConvocatoriaEval"].ToString();

            for (int i = 0; i < lArregloProyectos.Length; i++)
            {
                asignarAcreditador(lCodContacto, lArregloProyectos[i], lCodConvocatoria);
            }
            Response.Redirect("AsignarProyectosAcreditadores.aspx");
        }

        private void asignarAcreditador(String pCodContacto, String pCodProyecto, String pCodConvocatoria)
        {
            String sql;

            sql = "SELECT ID_PROYECTOCONTACTO FROM PROYECTOCONTACTO WHERE CODPROYECTO=" + pCodProyecto + " AND INACTIVO=0 AND ACREDITADOR=1 AND CODCONVOCATORIA=" + pCodConvocatoria;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    sql = "UPDATE PROYECTOCONTACTO SET CODCONTACTO = " + pCodContacto + ", FECHAINICIO=getdate() WHERE CodProyecto=" + pCodProyecto + " AND INACTIVO=0 AND CODCONVOCATORIA=" + pCodConvocatoria;
                }
                else
                {
                    sql = "INSERT INTO PROYECTOCONTACTO (CODPROYECTO,CODCONTACTO,CODROL,FECHAINICIO,INACTIVO,CODCONVOCATORIA,ACREDITADOR) VALUES (" + pCodProyecto + "," + pCodContacto + "," + Constantes.CONST_RolAcreditador + ",GETDATE(),0," + pCodConvocatoria + ",1)";
                }
                reader.Close();
                conn.Close();
                cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    cmd.ExecuteReader();
                    conn.Close();
                }
                catch (SqlException se)
                {
                }
                finally
                {
                    conn.Close();
                }

            }
            catch (SqlException)
            {
            }
            finally
            {
                conn.Close();
            }

            sql = "UPDATE PROYECTO SET CODESTADO=10 WHERE ID_PROYECTO=" + pCodProyecto;

            cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
            }
            catch (SqlException se)
            {
            }
            finally
            {
                conn.Close();
            }
        }

        protected void LB_Cantidad_Click(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GV_FilaGrid = GridView1.Rows[indicefila];

            Int32 idCodigoPro = Int32.Parse(GridView1.DataKeys[GV_FilaGrid.RowIndex].Value.ToString());

            Session["idCodigoProUser"] = idCodigoPro;

            Response.Redirect("PlanesDeNegocio.aspx");
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

        /// <summary>
        /// Paginación de la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["dtEmpresas_1"] as DataTable;

            if (dt != null)
            {
                GridView1.PageIndex = e.NewPageIndex;
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 18/06/2014.
        /// Quitar línea cuando los planes de negocio sean cero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var link = e.Row.FindControl("LB_Cantidad") as LinkButton;

                if (link != null) { if (link.Text == "0") { link.Style.Add("text-decoration", "none"); } }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 18/06/2014.
        /// Sortear la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtEmpresas_1"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GridView1.DataSource = Session["dtEmpresas_1"];
                GridView1.DataBind();
            }
        }
    }
}