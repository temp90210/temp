﻿using System;
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


namespace Fonade.Controles.evaluacion
{
    public partial class CatalogoAporteEvaluacion : System.Web.UI.UserControl
    {

        //Cadena de conexión
        private string conexionstr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        //variables 
        #region #variables

        string nombre;
        string detalle;
        float solicitud;
        float solicitado;
        int tipodeaporte;
        int txtTab = Datos.Constantes.CONST_subAportes;
        int CodAporte;
        int CodProyecto;
        int CodConvocatoria;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["CodProyecto"] = 98;
            Session["Cod_convocatoria"] = 1;
            if (!IsPostBack)
            {
                if (int.Parse(Session["CodProyecto"].ToString()) > 0 && int.Parse(Session["CodConvocatoria"].ToString()) > 0)
                {
                    DataTable dt_indicador = MD_TipoIndicadorGestion_SelectAll();
                    dpl_tipo.DataSource = dt_indicador;
                    dpl_tipo.DataTextField = "nomTipoIndicador";
                    dpl_tipo.DataValueField = "Id_TipoIndicador";
                    dpl_tipo.DataBind();

                    if (int.Parse(Session["CodAporte"].ToString()) > 0)
                    {
                        if (Session["AccionAporteEvaluacion"].ToString() != "eliminar")
                        {
                            CodAporte = int.Parse(Session["CodAporte"].ToString());
                            DataTable dt_aporte = new DataTable();
                            dt_aporte = MD_EvaluacionProyectoAporte_SelectRow(CodAporte);
                            foreach (DataRow dr_aporte in dt_aporte.Rows)
                            {
                                txt_nombre.Value = dr_aporte["nombre"].ToString();
                                txt_detalle.Value = dr_aporte["detalle"].ToString();
                                txt_solicitado.Value = dr_aporte["Solicitado"].ToString();
                                txt_recomendado.Value = dr_aporte["Recomendado"].ToString();
                                dpl_tipo.SelectedValue = dr_aporte["CodTipoIndicador"].ToString();
                                btn_crearaporte.Text = "Actualizar";
                            }
                        }
                        else
                        {
                            CodAporte = int.Parse(Session["CodAporte"].ToString());
                            MD_EvaluacionProyectoAporte_DeleteRow(CodAporte);
                            Response.Redirect("EvaluacionAportes.aspx");
                        }

                    }
                    else
                    {
                        btn_crearaporte.Text = "Crear";
                    }
                }
                else
                {
                    Response.Write("no esta autorizado para seguir");
                    Response.Redirect("EvaluacionAportes.aspx");
                }
            }
        }

        protected void btn_crearaporte_Click(object sender, EventArgs e)
        {
            CodProyecto = int.Parse(Session["CodProyecto"].ToString());
            CodConvocatoria = int.Parse(Session["CodConvocatoria"].ToString());

            nombre = txt_nombre.Value;
            solicitud = float.Parse(txt_recomendado.Value);
            tipodeaporte = int.Parse(dpl_tipo.SelectedValue);
            detalle = txt_detalle.Value;
            solicitado = float.Parse(txt_solicitado.Value);

            if (int.Parse(Session["CodAporte"].ToString()) > 0)
            {
                CodAporte = int.Parse(Session["CodAporte"].ToString());
                MD_EvaluacionProyectoAporte_Update(CodAporte, CodProyecto, CodConvocatoria, tipodeaporte, nombre, detalle, solicitud, solicitado, false);
            }
            else
            {
                MD_EvaluacionProyectoAporte_Insert(CodProyecto, CodConvocatoria, tipodeaporte, nombre, detalle, solicitud, solicitado, true);
            }
            Response.Redirect("EvaluacionAportes.aspx");
        }

        #region #aporte

        //Crear Indicador x id
        public int MD_EvaluacionProyectoAporte_Insert(int CodProyecto, int CodConvocatoria, int CodTipoIndicador, string Nombre, string Detalle, float Solicitado, float Recomendado, Boolean Protegido)
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
                return id;
            }
            catch (Exception ex)
            {

                Response.Write(ex.Message);
                return 0;
            }
        }

        //Actualizar Indicador x id
        public int MD_EvaluacionProyectoAporte_Update(int Id_Aporte, int CodProyecto, int CodConvocatoria, int CodTipoIndicador, string Nombre, string Detalle, float Solicitado, float Recomendado, Boolean Protegido)
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
                return id;
            }
            catch (Exception ex)
            {

                Response.Write(ex.Message);
                return 0;
            }
        }

        //Consultar  Indicador x id
        public DataTable MD_EvaluacionProyectoAporte_SelectRow(int Id_Aporte)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_EvaluacionProyectoAporte_SelectRow", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id_Aporte", Id_Aporte);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return new DataTable();

                }
                return null;
            }
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

    }
}