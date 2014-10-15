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
    public partial class AdicionarCriterioPriorizacion : Negocio.Base_Page
    {
        public int idConvocatoria;

        protected void Page_Load(object sender, EventArgs e)
        {
             idConvocatoria = Convert.ToInt32(Session["Id_ConvocatCriterios"]);
             if (!IsPostBack)
             {
                 lbl_Titulo.Text = void_establecerTitulo("ADICIONAR CRITERIO DE PRIORIZACIÓN");
                 l_fechaActual.Text = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");
             }
        }

        protected void lds_criterioPriorizacion_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var query = from P in consultas.Mostrar_ListadoCriterios(idConvocatoria)
                            select P;
                e.Result = query;
            }
            catch (Exception)
            { }
        }

        protected void btn_adicionar_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow grd_Row in this.gvcriteriosPriorizacion.Rows)
            {
                if (((CheckBox)grd_Row.FindControl("ch_criterio")).Checked)
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    SqlCommand cmd = new SqlCommand("MD_convocatoria_criterios_priorizacion", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Id_usuario", usuario.IdContacto.ToString());
                    cmd.Parameters.AddWithValue("@IdConvocatoria", idConvocatoria);
                    cmd.Parameters.AddWithValue("@IdCriterioPriorizacion", Convert.ToInt32(((HiddenField)grd_Row.FindControl("hiddenID")).Value));
                    cmd.Parameters.AddWithValue("@parametro", "");
                    cmd.Parameters.AddWithValue("@incidencias", 0);
                    cmd.Parameters.AddWithValue("@caso", "Create");
                    SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                    con.Open();
                    cmd2.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd2.Dispose();
                    cmd.Dispose();
                } 
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "window.opener.location.reload(); window.close();", true);
        }

    }
}