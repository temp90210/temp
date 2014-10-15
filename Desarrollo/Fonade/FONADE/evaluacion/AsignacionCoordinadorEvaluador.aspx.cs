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
    public partial class AsignacionCoordinadorEvaluador : Negocio.Base_Page
    {

        protected int CoordinadorEValAsignado;

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Titulo.Text = void_establecerTitulo("ASIGNAR COORDINADOR A EVALUADORES");
            if (!IsPostBack)
            {
                llenarEvaluadores();
            }
        }

        protected void llenarEvaluadores()
        {
            DataTable eval = consultas.ObtenerDataTable("MD_VerEvaluadoresActivos");

            if (eval.Rows.Count != 0)
            {
                ddl_evals.DataSource = eval;
                ddl_evals.DataTextField = "nombre";
                ddl_evals.DataValueField = "Id_Contacto";
                ddl_evals.DataBind();
                ddl_evals.Items.Insert(0, new ListItem("","",true));
                ltitulo.Text = "Planes de negocio de " + ddl_evals.SelectedItem.Text;
            }
        }

        protected void lds_listadoproy_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

            try
            {
                int Codeval = -1;
                if (!string.IsNullOrEmpty(ddl_evals.SelectedValue))
                {
                    Codeval = Convert.ToInt32(ddl_evals.SelectedValue);
                }
                    var query = from P in consultas.CargarProyectosEval(Codeval, Constantes.CONST_RolEvaluador)
                                select P;
                    if (query.Count() == 0)
                    {
                        Panelproyectos.Visible = false;
                        lmensajeproy.Visible = true;
                    }
                    else
                    {
                        Panelproyectos.Visible = true;
                        lmensajeproy.Visible = false;
                    }
                    e.Result = query;
            }
            catch (Exception exc)
            { }

        }

        protected void lds_CoordAignado_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

            try
            {
                int Codeval = -1;
                if (!string.IsNullOrEmpty(ddl_evals.SelectedValue))
                {
                    Codeval = Convert.ToInt32(ddl_evals.SelectedValue);
                }
                var query = from P in consultas.VerCoordinadorAsignado(Codeval)
                            select P;
                e.Result = query;

                var query2 = (from P in consultas.Db.MD_VerCoordinadorAsignado(Codeval)
                              select new
                              {
                                  P
                              }).FirstOrDefault();
                CoordinadorEValAsignado = query2.P.Id_Contacto;
                llenarProgramas(CoordinadorEValAsignado.ToString());
                PanellistadoCoordinadores.Visible = false;
            }
            catch (Exception exc)
            { }

        }

        protected void ddl_evals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_evals.SelectedValue))
            {
                ltitulo.Text = "Planes de negocio de " + ddl_evals.SelectedItem.Text;
                DltEvaluacion.DataBind();
                gvcoorAsigando.DataBind();
            }
        }

        protected void btn_asignar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_evals.SelectedValue))
            {
                PanellistadoCoordinadores.Visible = true;
            }
        }

        protected void llenarProgramas(string coordinador)
        {

            rbl_coordinEval.ClearSelection();
            rbl_coordinEval.Items.Clear();
            rbl_coordinEval.Dispose();

            var query = from x in consultas.Db.MD_VerCoordinadoresDEEvaluacion()
                        select new
                        {
                            nombreCoord = x.nombre,
                            id_coord = x.id_contacto,
                        };
            rbl_coordinEval.DataSource = query.ToList();
            rbl_coordinEval.DataTextField = "nombreCoord";
            rbl_coordinEval.DataValueField = "id_coord";
            rbl_coordinEval.DataBind();
            rbl_coordinEval.SelectedValue = coordinador;

        }

        protected void btn_actualizar_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_updateCoordEvalAsignado", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codeval", Convert.ToInt32(ddl_evals.SelectedValue));
            cmd.Parameters.AddWithValue("@CodCoordEval", Convert.ToInt32(rbl_coordinEval.SelectedValue));
            SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
            con.Open();
            cmd2.ExecuteNonQuery();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
            cmd2.Dispose();
            cmd.Dispose();
            gvcoorAsigando.DataBind();
            PanellistadoCoordinadores.Visible = false;
        }
    }
}