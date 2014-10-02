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
namespace Fonade.FONADE.interventoria
{
    public partial class AsignacionCoordinacionInterventores : Negocio.Base_Page
    {
        protected int CoordinadorIntervAsignado;

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Titulo.Text = void_establecerTitulo("ASIGNAR COORDINADORES A INTERVENTORES");
            if (!IsPostBack)
            {
                llenarInterventores();
            }
        }

        protected void llenarInterventores()
        {

            var query = from x in consultas.Db.MD_VerInterventoresActivos(Constantes.CONST_GerenteInterventor)
                        select new
                        {
                            nombre = x.nombre,
                            id_Contacto = x.Id_Contacto,
                        };
           
            ddl_evals.DataSource = query;
            ddl_evals.DataTextField = "nombre";
            ddl_evals.DataValueField = "Id_Contacto";
            ddl_evals.DataBind();
            ltitulo.Text = "Planes de negocio de " + ddl_evals.SelectedItem.Text;


        }

        protected void lds_listadoproy_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

            try
            {
                int CodInterventor = Convert.ToInt32(ddl_evals.SelectedValue);
                var query = from P in consultas.CargarProyectosInterv(CodInterventor, Constantes.CONST_RolInterventor, Constantes.CONST_RolInterventorLider)
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
                int CodInterventor = Convert.ToInt32(ddl_evals.SelectedValue);
                var query = from P in consultas.VerCoordinadorAsignado(CodInterventor)
                            select P;
                e.Result = query;

                var query2 = (from P in consultas.Db.MD_VerCoordinadorAsignado(CodInterventor)
                              select new
                              {
                                  P
                              }).FirstOrDefault();
                CoordinadorIntervAsignado = query2.P.Id_Contacto;
                llenarProgramas(CoordinadorIntervAsignado.ToString());
                PanellistadoCoordinadores.Visible = false;
            }
            catch (Exception exc)
            { }

        }

        protected void ddl_evals_SelectedIndexChanged(object sender, EventArgs e)
        {
            ltitulo.Text = "Planes de negocio de " + ddl_evals.SelectedItem.Text;
            DltEvaluacion.DataBind();
            gvcoorAsigando.DataBind();
        }

        protected void btn_asignar_Click(object sender, EventArgs e)
        {
            PanellistadoCoordinadores.Visible = true;

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
            SqlCommand cmd = new SqlCommand("MD_updateCoordIntervAsignado", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codinterv", Convert.ToInt32(ddl_evals.SelectedValue));
            cmd.Parameters.AddWithValue("@CodCoordInterv", Convert.ToInt32(rbl_coordinEval.SelectedValue));
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