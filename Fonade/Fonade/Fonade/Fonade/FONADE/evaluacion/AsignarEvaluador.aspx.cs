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
    public partial class AsignarEvaluador : Negocio.Base_Page
    {
        protected int EValAsignado;

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Titulo.Text = void_establecerTitulo("ASIGNAR EVALUADOR A PROYECTO");
            if (!IsPostBack)
            {
                llenarSectores();
                llenarProyectos();
                ddl_evals.Items.Add(new ListItem("TODOS", "-1", true));
            }
        }

        protected void llenarSectores()
        {
            DataTable eval = consultas.ObtenerDataTable("SELECT id_Sector, nomSector FROM Sector ORDER BY nomSector", "text");

            if (eval.Rows.Count != 0)
            {
                ddl_evals.DataSource = eval;
                ddl_evals.DataTextField = "nomSector";
                ddl_evals.DataValueField = "id_Sector";
                ddl_evals.DataBind();
                //ltitulo.Text = "Planes de negocio de " + ddl_evals.SelectedItem.Text;
            }
        }

        protected void lds_listadoproy_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

            try
            {

                var query = from P in consultas.cargarProyectoSumarioActual(Convert.ToInt32(ddl_proyectos.SelectedValue))
                            select P;
                if (query.Count() == 0)
                {
                    Panelproyectos.Visible = false;
                }
                else
                {
                    Panelproyectos.Visible = true;
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
                int Codeval = Convert.ToInt32(ddl_evals.SelectedValue);
                var query = from P in consultas.Db.MD_VerEvalDelProyecto(Convert.ToInt32(ddl_proyectos.SelectedValue))
                            select P;
                e.Result = query;

                var query2 = (from P in consultas.Db.MD_VerEvalDelProyecto(Convert.ToInt32(ddl_proyectos.SelectedValue))
                              select new
                              {
                                  P
                              }).FirstOrDefault();
                EValAsignado = query2.P.codcontacto; 
                llenarProgramas(EValAsignado.ToString());
                PanellistadoCoordinadores.Visible = false;
            }
            catch (Exception exc)
            { }

        }

        protected void llenarProgramas(string coordinador)
        {

            rbl_coordinEval.ClearSelection();
            rbl_coordinEval.Items.Clear();
            rbl_coordinEval.Dispose();

            var query = from x in consultas.Db.MD_VerEvalSector(Convert.ToInt32(ddl_evals.SelectedValue))
                        orderby x.nombre
                        select new
                        {
                            nombreCoord = x.nombre,
                            id_coord = x.Id_Contacto,
                        };
            rbl_coordinEval.DataSource = query.ToList();
            rbl_coordinEval.DataTextField = "nombreCoord";
            rbl_coordinEval.DataValueField = "id_coord";
            rbl_coordinEval.DataBind();
            rbl_coordinEval.SelectedValue = coordinador;

        }

        protected void ddl_evals_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarProyectos();
            DltEvaluacion.DataBind();
            gvcoorAsigando.DataBind();

        }

        protected void llenarProyectos()
        {
            //Inicializar variables.
            String txtSQL = "";
            DataTable RSProyecto = new DataTable();
            ddl_proyectos.Items.Clear();

            #region Comentarios (1).
            //consultas.Parameters = new[]{
            //new SqlParameter{

            //    ParameterName="@sector", Value= Convert.ToInt32(ddl_evals.SelectedValue)

            //}};

            //DataTable eval = consultas.ObtenerDataTable("MD_cargarProyectosSector"); 
            #endregion

            #region Comentarios (2).
            //if (eval.Rows.Count != 0)
            //{
            //    //ddl_proyectos.DataSource = eval;
            //    //ddl_proyectos.DataTextField = "proyecto";
            //    //ddl_proyectos.DataValueField = "Id_Proyecto";
            //    //ddl_proyectos.DataBind();                

            //    //ltitulo.Text = "Planes de negocio de " + ddl_evals.SelectedItem.Text;
            //} 
            #endregion

            if (ddl_evals.SelectedItem.Text == "TODOS")
            {
                txtSQL = " SELECT Id_Proyecto, NomProyecto, pc.CodContacto from PROYECTO p " +
                         " left join Proyectocontacto pc on id_proyecto=codproyecto " +
                         " and pc.inactivo=0 and codrol=" + Constantes.CONST_RolEvaluador +
                         " where CodEstado = " + Constantes.CONST_Acreditado + " or codestado =" + Constantes.CONST_Evaluacion;
            }
            else
            {
                txtSQL = " SELECT Id_Proyecto, NomProyecto, pc.CodContacto " +
                         " FROM Proyecto P " +
                         " INNER JOIN Subsector S ON P.codsubSector=S.id_subsector and codSector = " + ddl_evals.SelectedValue +
                         " LEFT JOIN Proyectocontacto pc on id_proyecto=codproyecto " +
                         " and pc.inactivo=0 and codrol = " + Constantes.CONST_RolEvaluador +
                         " WHERE CodEstado = " + Constantes.CONST_Acreditado + " or codestado =" + Constantes.CONST_Evaluacion;
            }

            RSProyecto = consultas.ObtenerDataTable(txtSQL, "text");

            ListItem item = new ListItem();
            foreach (DataRow row in RSProyecto.Rows)
            {
                item = new ListItem();
                item.Value = row["Id_Proyecto"].ToString();
                if (String.IsNullOrEmpty(row["CodContacto"].ToString()))
                {
                    //Clase "Menu" en FONADE clásico.
                    item.Attributes.Add("title", "Plan de Negocio sin evaluador");
                    item.Attributes.Add("style", "color:#CC0000; font-weight:bold;");
                    item.Text = row["Id_Proyecto"].ToString() + " " + row["NomProyecto"].ToString() + " !! Plan de Negocio sin evaluador";
                }
                else
                    item.Text = row["Id_Proyecto"].ToString() + " " + row["NomProyecto"].ToString();
                ddl_proyectos.Items.Add(item);
            }
            //consultas.Parameters = null;
        }

        protected void btn_asignar_Click(object sender, EventArgs e)
        {
            PanellistadoCoordinadores.Visible = true;
        }

        protected void btn_actualizar_Click(object sender, EventArgs e)
        {
            var query = (from x in consultas.Db.ConvocatoriaProyectos
                         orderby x.Fecha descending
                         where x.CodProyecto == Convert.ToInt32(ddl_proyectos.SelectedValue)
                         select new
                         {
                             x.CodConvocatoria
                         }).FirstOrDefault();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_updateEvalAsignado", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodProyecto", Convert.ToInt32(ddl_proyectos.SelectedValue));
            cmd.Parameters.AddWithValue("@CodConvocatoria", query.CodConvocatoria);
            cmd.Parameters.AddWithValue("@CodEvalNuevo", Convert.ToInt32(rbl_coordinEval.SelectedValue));
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
            llenarProyectos();
        }

        protected void ddl_proyectos_SelectedIndexChanged(object sender, EventArgs e)
        {
            DltEvaluacion.DataBind();
            gvcoorAsigando.DataBind();
        }
    }
}