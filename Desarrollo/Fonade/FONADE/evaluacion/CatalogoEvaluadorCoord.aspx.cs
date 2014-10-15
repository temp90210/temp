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

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoEvaluadorCoord : Negocio.Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Titulo.Text = void_establecerTitulo("COORDINADORES DE EVALUACIÓN");
            if (usuario.CodGrupo != Constantes.CONST_GerenteEvaluador)
            {
                Response.Redirect(@"\FONADE\MiPerfil\Home.aspx");
            }
        }

        protected void lds_listaCoordEval_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

            try
            {
                var query = from P in consultas.VerCoordinadoresDeEvaluacion()
                            select P;



                switch (gvcCoordinadoresEval.SortExpression)
                {
                    case "nombre":
                        if (gvcCoordinadoresEval.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.nombre);
                        else
                            query = query.OrderByDescending(t => t.nombre);
                        break;
                    case "email":
                        if (gvcCoordinadoresEval.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.email);
                        else
                            query = query.OrderByDescending(t => t.email);
                        break;
                    case "Cuantos":
                        if (gvcCoordinadoresEval.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.Cuantos);
                        else
                            query = query.OrderByDescending(t => t.Cuantos);
                        break;
                    case "inactividad":
                        if (gvcCoordinadoresEval.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(t => t.inactividad);
                        else
                            query = query.OrderByDescending(t => t.inactividad);
                        break;
                    default:
                        query = query.OrderBy(t => t.nombre);
                        break;
                }


                e.Arguments.TotalRowCount = query.Count();

                e.Result = query;
            }
            catch (Exception exc)
            { }

        }

        protected void gvcCoordinadoresEval_Load(object sender, EventArgs e)
        {
            foreach (GridViewRow grd_Row in this.gvcCoordinadoresEval.Rows)
            {
                int inactivo= Convert.ToInt32(((HiddenField)grd_Row.FindControl("Hiddeninactivo")).Value);

                int NumEvaluadores = Convert.ToInt32(((HiddenField)grd_Row.FindControl("HiddenNumevals")).Value);
                if (NumEvaluadores==0)
                {
                    ((Button)grd_Row.FindControl("hlcuantos")).Enabled = false;

                    if (inactivo==0)
                    {
                        ((ImageButton)grd_Row.FindControl("ibtninactivar")).Visible = true;
                    }
                    else
                    {
                        ((ImageButton)grd_Row.FindControl("ibtnreactivar")).Visible = true;
                    }
                }
            }
        }

        protected void gvcCoordinadoresEval_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName.ToString())
            {
                case "vercuantos":
                    Session["ContactoEvaluador"] = e.CommandArgument.ToString();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('VerEvaluadoresCoordinador.aspx','_blank','width=602,height=390,toolbar=no, scrollbars=no, resizable=no');", true);
                break;

                case "reactivarcoorEval":
                    reactivarperfilEvaluador(Convert.ToInt32(e.CommandArgument));
                break;

                case "editacontacto":
                    Session["ContactoEvaluador"] = e.CommandArgument.ToString();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('CoordinadorEvaluador.aspx','_blank','width=562,height=420,toolbar=no, scrollbars=1, resizable=no');", true);
                    break;

                case "verestador":
                    string enviarsesion = e.CommandArgument.ToString() + "," + Constantes.CONST_CoordinadorEvaluador.ToString() + ",Ver";
                    Session["ContactoEvaluador"] = enviarsesion;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('DesactivarEvaluador.aspx','_blank','width=502,height=200,toolbar=no, scrollbars=no, resizable=no');", true);
                    break;

                case "inactivarcoorEval":
                    string enviarsesion2 = e.CommandArgument.ToString() + "," + Constantes.CONST_CoordinadorEvaluador.ToString() + ",Desactivar";
                    Session["ContactoEvaluador"] = enviarsesion2;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('DesactivarEvaluador.aspx','_blank','width=502,height=350,toolbar=no, scrollbars=no, resizable=no');", true);
                    break;
            }
        }

        protected void reactivarperfilEvaluador(int evaluador)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_ActivarEvaluadores", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodContacto", evaluador);
            SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
            con.Open();
            cmd2.ExecuteNonQuery();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
            cmd2.Dispose();
            cmd.Dispose();
            Response.Redirect(Request.RawUrl);
        }

        protected void lbtn_crearCoorE_Click(object sender, EventArgs e)
        {
            Session["ContactoEvaluador"] = "0";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('CoordinadorEvaluador.aspx','_blank','width=562,height=420,toolbar=no, scrollbars=1, resizable=no');", true);
        }

        protected void btn_asignar_Click(object sender, EventArgs e)
        {
            Response.Redirect("AsignacionCoordinadorEvaluador.aspx");
        }
    }
}