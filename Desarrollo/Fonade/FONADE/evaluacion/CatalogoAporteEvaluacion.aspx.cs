using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoAporteEvaluacion : Negocio.Base_Page //System.Web.UI.Page
    {
        //Cadena de conexión
        private string conexionstr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        //variables 
        #region #variables

        string nombre;
        string detalle;
        float solicitud;
        double recomendado;
        int tipodeaporte;
        int txtTab = Datos.Constantes.CONST_subAportes;
        int CodAporte;
        int CodProyecto;
        int CodConvocatoria;
        Consultas oConsultas = new Consultas();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtsolicitado.Text = "0";
                txtsolicitado.Enabled = false;

                lblfecha.Text = DateTime.Now.ToShortDateString();
                Session["AccionAporteEvaluacion"] = Request["Accion"];
                if (!string.IsNullOrEmpty(Session["AccionAporteEvaluacion"].ToString()))
                {
                    switch (Session["AccionAporteEvaluacion"].ToString())
                    {
                        case "Nuevo":
                        case "Crear":
                            title.Text = "Crear Aporte";
                            break;
                        case "Actualizar":
                        case "Editar":
                            title.Text = "Modificar Aporte";
                            break;
                    }
                }
                Session["CodAporte"] = Request["Aporte"];

                if (int.Parse(Session["CodProyecto"].ToString()) > 0 && int.Parse(Session["CodConvocatoria"].ToString()) > 0)
                {
                    dpl_tipo.DataSource = oConsultas.Db.TipoIndicadorGestions.ToList();
                    dpl_tipo.DataTextField = "nomTipoIndicador";
                    dpl_tipo.DataValueField = "Id_TipoIndicador";
                    dpl_tipo.DataBind();

                    if (int.Parse(Session["CodAporte"].ToString()) > 0)
                    {
                        if (Session["AccionAporteEvaluacion"].ToString() != "eliminar")
                        {
                            CodAporte = int.Parse(Session["CodAporte"].ToString());
                            MD_EvaluacionProyectoAporte_SelectRow(CodAporte);

                        }
                        else
                        {
                            CodAporte = int.Parse(Session["CodAporte"].ToString());
                            MD_EvaluacionProyectoAporte_DeleteRow(CodAporte);
                            Redireccionar("Registro Eliminado Exitosamente");
                        }
                    }
                    else
                    {
                        btn_crearaporte.Text = "Crear";
                    }
                }
                else
                {
                    Redireccionar("No posee permisos para esta operación");
                }
            }
        }

        protected void btn_crearaporte_Click(object sender, EventArgs e)
        {
            CodProyecto = int.Parse(Session["CodProyecto"].ToString());
            CodConvocatoria = int.Parse(Session["CodConvocatoria"].ToString());

            nombre = TxtNombre.Text;
            recomendado =
                Convert.ToDouble(!string.IsNullOrEmpty(txtRecomendado.Text) ? Convert.ToDecimal(txtRecomendado.Text) : 0);
            tipodeaporte = int.Parse(dpl_tipo.SelectedValue);
            detalle = txt_detalle.Text;
            solicitud = float.Parse(txtsolicitado.Text);

            if (int.Parse(Session["CodAporte"].ToString()) > 0)
            {
                CodAporte = int.Parse(Session["CodAporte"].ToString());
                MD_EvaluacionProyectoAporte_Update(CodAporte, CodProyecto, CodConvocatoria, tipodeaporte, nombre, detalle, solicitud, recomendado, false);
                Redireccionar("Registro Actualizado Exitosamente!");
            }
            else
            {
                MD_EvaluacionProyectoAporte_Insert(CodProyecto, CodConvocatoria, tipodeaporte, nombre, detalle, solicitud, recomendado, true);
                Redireccionar("Registro Creado Exitosamente!");
            }
        }

        public void Redireccionar(string mensaje)
        {
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "Mensaje", "alert('" + mensaje + "');", true);
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "", "window.opener.location.reload();window.close();", true);
        }
        #region #aporte

        //Crear Indicador x id
        public int MD_EvaluacionProyectoAporte_Insert(int CodProyecto, int CodConvocatoria, int CodTipoIndicador, string Nombre, string Detalle, float Solicitado, double Recomendado, Boolean Protegido)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_EvaluacionProyectoAporte_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodProyecto", CodProyecto);
            cmd.Parameters.AddWithValue("@CodConvocatoria", CodConvocatoria);
            cmd.Parameters.AddWithValue("@CodTipoIndicador", CodTipoIndicador);
            cmd.Parameters.AddWithValue("@Nombre", Nombre);
            cmd.Parameters.AddWithValue("@Detalle", Detalle);
            cmd.Parameters.AddWithValue("@Solicitado", Solicitado);
            cmd.Parameters.AddWithValue("@Recomendado", Recomendado);
            cmd.Parameters.AddWithValue("@Protegido", Protegido);

            try
            {
                con.Open();
                int id = (int)cmd.ExecuteScalar();
                con.Close();
                con.Dispose();
                cmd.Dispose();
                prActualizarTabEval(txtTab.ToString(), Session["CodProyecto"].ToString(), Session["CodConvocatoria"].ToString());
                return id;
            }
            catch (Exception ex)
            {

                Response.Write(ex.Message);
                return 0;
            }
        }

        //Actualizar Indicador x id
        public int MD_EvaluacionProyectoAporte_Update(int Id_Aporte, int CodProyecto, int CodConvocatoria, int CodTipoIndicador, string Nombre, string Detalle, float Solicitado, double Recomendado, Boolean Protegido)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_EvaluacionProyectoAporte_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id_Aporte", Id_Aporte);
            cmd.Parameters.AddWithValue("@CodProyecto", CodProyecto);
            cmd.Parameters.AddWithValue("@CodConvocatoria", CodConvocatoria);
            cmd.Parameters.AddWithValue("@CodTipoIndicador", CodTipoIndicador);
            cmd.Parameters.AddWithValue("@Nombre", Nombre);
            cmd.Parameters.AddWithValue("@Detalle", Detalle);
            cmd.Parameters.AddWithValue("@Solicitado", Solicitado);
            cmd.Parameters.AddWithValue("@Recomendado", Recomendado);
            cmd.Parameters.AddWithValue("@Protegido", Protegido);

            try
            {
                con.Open();
                int id = (int)cmd.ExecuteScalar();
                con.Close();
                con.Dispose();
                cmd.Dispose();
                prActualizarTabEval(txtTab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());
                return id;
            }
            catch (Exception ex)
            {

                Response.Write(ex.Message);
                return 0;
            }
        }

        //Consultar  Indicador x id
        public List<EvaluacionProyectoAporte> MD_EvaluacionProyectoAporte_SelectRow(int Id_Aporte)
        {
            List<EvaluacionProyectoAporte> lst = null;
            if (Id_Aporte != 0)
            {
                lst = oConsultas.Db.EvaluacionProyectoAportes.Where(a => a.Id_Aporte == Id_Aporte).ToList();

                if (lst.Any())
                {
                    foreach (var dr_aporte in lst)
                    {
                        TxtNombre.Text = dr_aporte.Nombre;
                        txt_detalle.Text = dr_aporte.Detalle;
                        txtsolicitado.Text = dr_aporte.Solicitado.ToString();
                        txtRecomendado.Text = dr_aporte.Recomendado.ToString();
                        dpl_tipo.SelectedValue = dr_aporte.CodTipoIndicador.ToString();
                        btn_crearaporte.Text = "Actualizar";
                    }

                    TxtNombre.Enabled = false;
                    txtsolicitado.Enabled = false;
                    dpl_tipo.Enabled = false;
                }

            }
            return lst;
        }

        public void MD_EvaluacionProyectoAporte_DeleteRow(int Id_Aporte)
        {
            using (var con = new SqlConnection(conexionstr))
            {
                using (var com = con.CreateCommand())
                {
                    com.CommandText = "MD_EvaluacionProyectoAporte_DeleteRow";
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    // Validar que no guarde espacios en blanco
                    com.Parameters.AddWithValue("@Id_Aporte", Id_Aporte);
                    try
                    {
                        con.Open();
                        com.ExecuteNonQuery();
                        prActualizarTabEval(txtTab.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                } // using comando
            } // using conexion

        }


        #endregion

        #region #tipoindicador
        //Consultar tipo indicador x id
        public DataTable MD_TipoIndicadorGestion_SelectAll()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_TipoIndicadorGestion_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return new DataTable();

                }
                return null;
            }
        }

        #endregion

        protected void BtnCerrar_Click(object sender, EventArgs e)
        {

            ScriptManager.RegisterClientScriptBlock(this, GetType(), "",
                                                    "window.opener.location.reload();window.close();", true);

        }
    }
}