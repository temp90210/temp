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

namespace Fonade.Controles.evaluacion
{
    public partial class CatalogoItem : System.Web.UI.UserControl
    {
        int codproyecto;
        int codconvocatoria;
        int coditem;
        int puntaje;
        int codaspecto;
        string textoescala;
        string textoitem;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (int.Parse(Session["CodProyecto"].ToString()) > 0 && int.Parse(Session["CodConvocatoria"].ToString()) > 0)
                {
                    if (int.Parse(Session["CodItem"].ToString()) > 0)
                    {
                        lt_item.Text = "Editar Item";
                        coditem = int.Parse(Session["CodItem"].ToString());
                        DataTable dt_item = new DataTable();
                        dt_item = MD_Item_SelectRow(coditem);
                        foreach (DataRow dr_item in dt_item.Rows)
                        {
                            txt_nombreitem.Value = dr_item["NomItem"].ToString();
                        }
                        dt_item.Dispose();

                        DataTable dt_itemescala = new DataTable();
                        dt_itemescala = MD_ItemEscala_SelectAll(coditem);
                        foreach (DataRow dr_itemescala in dt_item.Rows)
                        {
                            txt_texto.Value = dr_itemescala["NomItem"].ToString();
                            txt_puntaje.Value = dr_itemescala["Puntaje"].ToString();
                        }
                        dt_item.Dispose();



                    }
                    else
                    {
                        lt_item.Text = "Crear Item";

                    }
                }
                else
                {
                    Response.Write("no esta autorizado para seguir");
                }
            }
        }

        protected void btn_crear_Click(object sender, EventArgs e)
        {
            codproyecto = int.Parse(Session["CodProyecto"].ToString());
            codconvocatoria = int.Parse(Session["CodConvocatoria"].ToString());
            codaspecto = int.Parse(Session["CodAspecto"].ToString());
            puntaje = int.Parse(txt_puntaje.Value);
            textoescala = txt_texto.Value;
            textoitem = txt_nombreitem.Value;

            if (int.Parse(Session["CodItem"].ToString()) > 0)
            {

                coditem = int.Parse(Session["CodItem"].ToString());
                MD_ItemEscala_DeleteRow(coditem);
                MD_ItemEscala_Insert(coditem, textoescala, puntaje);

            }
            else
            {
                int id_item = (int)MD_Item_Insert(textoitem, codaspecto, true);
                MD_ItemEscala_Insert(id_item, textoescala, puntaje);
                MD_EvaluacionEvaluador_Insert(codproyecto, codconvocatoria, id_item, puntaje);

            }

        }

        #region #item

        //Crear Indicador x id
        public int MD_Item_Insert(string NomItem, int CodTabEvaluacion, Boolean Protegido)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_Item_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NomItem", NomItem);
            cmd.Parameters.AddWithValue("@CodTabEvaluacion", CodTabEvaluacion);
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
        protected void MD_Item_Update(int Id_Item, string NomItem)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_Item_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id_Item", Id_Item);
            cmd.Parameters.AddWithValue("@NomItem", NomItem);
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
        public DataTable MD_Item_SelectRow(int Id_Item)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_Item_SelectRow", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id_Item", Id_Item);
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

        #region #itemescala

        //Crear ItemEscala x id
        public int MD_ItemEscala_Insert(int CodItem, string Texto, int Puntaje)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_Item_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodItem", CodItem);
            cmd.Parameters.AddWithValue("@Texto", Texto);
            cmd.Parameters.AddWithValue("@Puntaje", Puntaje);
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

        //Actualizar ItemEscala x id
        public void MD_ItemEscala_Update(int CodItem, string Texto, int Puntaje)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_ItemEscala_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodItem", CodItem);
            cmd.Parameters.AddWithValue("@Texto", Texto);
            cmd.Parameters.AddWithValue("@Puntaje", Puntaje);
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
        public DataTable MD_ItemEscala_SelectAll(int CodItem)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_ItemEscala_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodItem", CodItem);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return new DataTable();

                }
                return null;
            }
        }

        //Eliminar Indicador x id
        public void MD_ItemEscala_DeleteRow(int CodItem)
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
                using (var com = con.CreateCommand())
                {
                    com.CommandText = "MD_ItemEscala_DeleteRow";
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    // Validar que no guarde espacios en blanco
                    com.Parameters.AddWithValue("@CodItem", CodItem);
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

        #region #evaluador
        //Crear Indicador x id
        protected void MD_EvaluacionEvaluador_Insert(int cod_proyecto, int cod_convocatoria, int CodItem, int Puntaje)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_EvaluacionEvaluador_Insert", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codProyecto", cod_proyecto);
            cmd.Parameters.AddWithValue("@codConvocatoria", cod_convocatoria);
            cmd.Parameters.AddWithValue("@CodItem", CodItem);
            cmd.Parameters.AddWithValue("@Puntaje", Puntaje);
            try
            {
                con.Open();
                int id = (int)cmd.ExecuteScalar();
                con.Close();
                con.Dispose();
                cmd.Dispose();
            }
            catch (Exception ex)
            {

                Response.Write(ex.Message);
            }
        }

        //Actualizar Indicador x id
        protected void MD_EvaluacionEvaluador_Update(int cod_proyecto, int cod_convocatoria, int CodItem, int Puntaje)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_EvaluacionEvaluador_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@codProyecto", cod_proyecto);
            cmd.Parameters.AddWithValue("@codConvocatoria", cod_convocatoria);
            cmd.Parameters.AddWithValue("@CodItem", CodItem);
            cmd.Parameters.AddWithValue("@Puntaje", Puntaje);
            try
            {
                con.Open();
                int id = (int)cmd.ExecuteScalar();
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
        public DataTable sp_EvaluacionProyectoIndicador_SelectRow(int id_Indicador)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("sp_EvaluacionProyectoIndicador_SelectRow", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_Indicador", id_Indicador);
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