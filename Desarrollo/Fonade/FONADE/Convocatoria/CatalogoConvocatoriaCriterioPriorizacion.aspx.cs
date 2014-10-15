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

namespace Fonade.FONADE.Convocatoria
{
    public partial class CatalogoConvocatoriaCriterioPriorizacion : Negocio.Base_Page
    {

        public int IdConvocatoria;
        protected void Page_Load(object sender, EventArgs e)
        {
            IdConvocatoria = Convert.ToInt32(Session["Id_ConvocatCriterios"]);
            if (!IsPostBack)
            {

            }
        }

        protected void lds_criterioPriorizacion_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var query = from P in consultas.MostrarConvocatoriaCriterioPriorizacion(IdConvocatoria)
                            select P;
                e.Result = query;
            }
            catch (Exception)
            { }
        }

        protected void gvcriteriosPriorizacion_Load(object sender, EventArgs e)
        {
            int modificar = 1;
            bool esModificable = true;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_ValidarConvocatoriaCriterio", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodConvocatoria", IdConvocatoria);
            cmd.Parameters.AddWithValue("@CONST_AsignacionRecursos", Constantes.CONST_AsignacionRecursos);
            con.Open();
            SqlDataReader r = cmd.ExecuteReader();
            r.Read();
            modificar = Convert.ToInt16(r["Conteo"]);
            con.Close();
            con.Dispose();
            cmd.Dispose();

            if (modificar == 0)
            {
                esModificable = false;
            }

            btn_actualizar.Visible = esModificable;
            btn_actualizar.Enabled = esModificable;

            foreach (GridViewRow grd_Row in this.gvcriteriosPriorizacion.Rows)
            {
                ((TextBox)grd_Row.FindControl("txt_parametros")).Enabled = esModificable;
                ((TextBox)grd_Row.FindControl("txt_incidencia")).Enabled = esModificable;
                ((ImageButton)grd_Row.FindControl("btnEliminarcp")).Visible = esModificable;

            }
        }

        protected void btn_volver_Click(object sender, EventArgs e)
        {
            Session["IdConvocatoria"] = IdConvocatoria;
            Response.Redirect("Convocatoria.aspx");
        }

        protected void btn_actualizar_Click(object sender, EventArgs e)
        {
            bool validaVacio = false;
            if (gvcriteriosPriorizacion.Rows.Count != 0)
            {
                int sumatoria = 0;
                foreach (GridViewRow grd_Row in this.gvcriteriosPriorizacion.Rows)
                {
                    
                    if (((TextBox)grd_Row.FindControl("txt_incidencia")).Text == "")
                    {
                        validaVacio = true;
                    }
                    else
                    {
                        sumatoria += Convert.ToInt16(((TextBox)grd_Row.FindControl("txt_incidencia")).Text);
                    }
                }
                if (sumatoria == 100)
                {
                    if (validaVacio==false)
                    {
                        foreach (GridViewRow grd_Row in this.gvcriteriosPriorizacion.Rows)
                        {
                            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                            SqlCommand cmd = new SqlCommand("MD_convocatoria_criterios_priorizacion", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.AddWithValue("@Id_usuario", usuario.IdContacto.ToString());
                            cmd.Parameters.AddWithValue("@IdConvocatoria", IdConvocatoria);
                            cmd.Parameters.AddWithValue("@IdCriterioPriorizacion", Convert.ToInt32(((HiddenField)grd_Row.FindControl("hiddenID")).Value));
                            cmd.Parameters.AddWithValue("@parametro", ((TextBox)grd_Row.FindControl("txt_parametros")).Text);
                            cmd.Parameters.AddWithValue("@incidencias", Convert.ToInt32(((TextBox)grd_Row.FindControl("txt_incidencia")).Text));
                            cmd.Parameters.AddWithValue("@caso", "Update");
                            SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                            con.Open();
                            cmd2.ExecuteNonQuery();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            con.Dispose();
                            cmd2.Dispose();
                            cmd.Dispose();
                        }
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Se ha actualizado exitosamente!')", true);
                    }
                    
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La sumatoria de los porcentajes de incidencias debe ser igual a 100!')", true);
                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["Id_ConvocatCriterios"] = IdConvocatoria;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('AdicionarCriterioPriorizacion.aspx','_blank','width=623,height=570,toolbar=no, scrollbars=1, resizable=no');", true);
        }

        protected void gvcriteriosPriorizacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToString() == "EliminarCriterio")
            {
                string NumCriterio = e.CommandArgument.ToString();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_convocatoria_criterios_priorizacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@Id_usuario", usuario.IdContacto.ToString());
                cmd.Parameters.AddWithValue("@IdConvocatoria", IdConvocatoria);
                cmd.Parameters.AddWithValue("@IdCriterioPriorizacion", Convert.ToInt32(NumCriterio));
                cmd.Parameters.AddWithValue("@parametro", DBNull.Value);
                cmd.Parameters.AddWithValue("@incidencias", 0);
                cmd.Parameters.AddWithValue("@caso", "Delete");
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
                gvcriteriosPriorizacion.DataBind();
                gvcriteriosPriorizacion_Load(null, null);
            }
        }

        protected void lnkadicionar_Click(object sender, EventArgs e)
        {
            Session["Id_ConvocatCriterios"] = IdConvocatoria;

            Redirect(null, "AdicionarCriterioPriorizacion.aspx", "_Blank", "width=623,height=570,toolbar=no, scrollbars=1, resizable=no");
        }
    }
}