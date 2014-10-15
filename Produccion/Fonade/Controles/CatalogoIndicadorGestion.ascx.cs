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

namespace Fonade.Controles
{
    public partial class CatalogoIndicadorGestion : System.Web.UI.UserControl
    {
        int Id_IndicadorGestion;
        int CodProyecto;
        int CodConvocatoria;
        string Aspecto;
        string FechaSeguimiento;
        string Numerador;
        string Denominador;
        string Descripcion;
        int RangoAceptable;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (int.Parse(Session["CodProyecto"].ToString()) > 0 && int.Parse(Session["CodConvocatoria"].ToString()) > 0)
                {
                    if (int.Parse(Session["CodProyecto"].ToString()) > 0)
                    {
                        btn_accioncatalogo.Text = "Actualizar";
                        Id_IndicadorGestion = int.Parse(Session["CodProyecto"].ToString());
                        DataTable dt_indicador = new DataTable();
                        dt_indicador = MD_EvaluacionIndicadorGestion_SelectRow(Id_IndicadorGestion);
                        foreach (DataRow dr_indicador in dt_indicador.Rows)
                        {
                            txt_aspecto.Value = dr_indicador["Aspecto"].ToString();
                            txt_fecha.Value = dr_indicador["FechaSeguimiento"].ToString();
                            dpl_tipoindicador.SelectedValue = dr_indicador["Id_IndicadorGestion"].ToString();
                            txt_numerador.Value = dr_indicador["Numerador"].ToString();
                            txt_denominador.Value = dr_indicador["Denominador"].ToString();
                            txt_descripcion.Value = dr_indicador["Descripcion"].ToString();
                            txt_rangoaceptable.Value = dr_indicador["RangoAceptable"].ToString();
                        }

                        dt_indicador.Dispose();
                    }
                    else
                    {
                        btn_accioncatalogo.Text = "Crear";
                    }
                }
                else
                {
                    Response.Redirect("EvaluacionProductos.aspx");
                }
            }

        }

        protected void btn_accioncatalogo_Click(object sender, EventArgs e)
        {
            Aspecto = txt_aspecto.Value;
            FechaSeguimiento = txt_fecha.Value;
            Id_IndicadorGestion = int.Parse(dpl_tipoindicador.SelectedValue);
            Numerador = txt_numerador.Value;
            Denominador = txt_denominador.Value;
            Descripcion = txt_descripcion.Value;
            RangoAceptable = int.Parse(txt_rangoaceptable.Value);
            CodProyecto = int.Parse(Session["CodProyecto"].ToString());
            CodConvocatoria = int.Parse(Session["CodConvocatoria"].ToString());

            if (int.Parse(Session["codIndicador"].ToString()) > 0)
            {
                Id_IndicadorGestion = int.Parse(Session["Id_IndicadorGestion"].ToString());
                MD_EvaluacionIndicadorGestion_Update(Id_IndicadorGestion, CodProyecto, CodConvocatoria, Aspecto, FechaSeguimiento, Numerador, Denominador, Descripcion, RangoAceptable);

                Response.Redirect("EvaluacionProductos.aspx");
            }
            else
            {
                MD_EvaluacionIndicadorGestion_Insert(CodProyecto, CodConvocatoria, Aspecto, FechaSeguimiento, Numerador, Denominador, Descripcion, RangoAceptable);
                Response.Redirect("EvaluacionProductos.aspx");
            }

        }



        #region #Indicadorgestion

        //Crear Indicador x id
        public int MD_EvaluacionIndicadorGestion_Insert(int CodProyecto, int CodConvocatoria, string Aspecto, string FechaSeguimiento, string Numerador, string Denominador, string Descripcion, int RangoAceptable)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_EvaluacionIndicadorGestion_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodProyecto", CodProyecto);
            cmd.Parameters.AddWithValue("@CodConvocatoria", CodConvocatoria);
            cmd.Parameters.AddWithValue("@Aspecto", Aspecto);
            cmd.Parameters.AddWithValue("@FechaSeguimiento", FechaSeguimiento);
            cmd.Parameters.AddWithValue("@Numerador", Numerador);
            cmd.Parameters.AddWithValue("@Denominador", Denominador);
            cmd.Parameters.AddWithValue("@Denominador", Descripcion);
            cmd.Parameters.AddWithValue("@RangoAceptable", RangoAceptable);

            try
            {
                con.Open();
                int id = (int)cmd.ExecuteScalar();
                con.Close();
                con.Dispose();
                cmd.Dispose();
                return id;
            }
            catch (Exception ex)
            {

                Response.Write(ex.Message);
                return 0;
            }
        }

        //Actualizar Indicador x id
        public void MD_EvaluacionIndicadorGestion_Update(int Id_IndicadorGestion, int CodProyecto, int CodConvocatoria, string Aspecto, string FechaSeguimiento, string Numerador, string Denominador, string Descripcion, int RangoAceptable)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_EvaluacionIndicadorGestion_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_IndicadorGestion", Id_IndicadorGestion);
            cmd.Parameters.AddWithValue("@CodProyecto", CodProyecto);
            cmd.Parameters.AddWithValue("@CodConvocatoria", CodConvocatoria);
            cmd.Parameters.AddWithValue("@Aspecto", Aspecto);
            cmd.Parameters.AddWithValue("@FechaSeguimiento", FechaSeguimiento);
            cmd.Parameters.AddWithValue("@Numerador", Numerador);
            cmd.Parameters.AddWithValue("@Denominador", Denominador);
            cmd.Parameters.AddWithValue("@Denominador", Descripcion);
            cmd.Parameters.AddWithValue("@RangoAceptable", RangoAceptable);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd.Dispose();

            }
            catch (Exception ex)
            {

                Response.Write(ex.Message);
            }
        }

        //Consultar  Indicador x id
        public DataTable MD_EvaluacionIndicadorGestion_SelectRow(int Id_IndicadorGestion)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_EvaluacionIndicadorGestion_SelectRow", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id_IndicadorGestion)", Id_IndicadorGestion);
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


    }
}