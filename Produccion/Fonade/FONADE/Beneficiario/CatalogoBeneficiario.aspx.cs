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

namespace Fonade.FONADE.Beneficiario
{
    public partial class CatalogoBeneficiario : Negocio.Base_Page
    {
        #region Variables globales.

        /// <summary>
        /// Variable que contiene las consultas SQL.
        /// </summary>
        String txtSQL;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("BENEFICIARIOS");
                MostrarTabla();
            }
        }

        protected void lds_beneficiarios_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var consulta = (from Proy in consultas.Db.ProyectoContactos
                                where Proy.CodContacto == usuario.IdContacto
                                & Proy.CodRol == 3
                                select new
                                {
                                    codigo = Proy.CodProyecto
                                }).FirstOrDefault();
                int codigoProyecto = consulta.codigo;

                var query = from P in consultas.Db.MD_MostrarBeneficiarios(codigoProyecto)
                            select P;
                e.Result = query;
            }
            catch (Exception)
            { }
        }

        protected void gv_verBeneficiarios_DataBound(object sender, EventArgs e)
        {
            l_conteoBenef.Text = gv_verBeneficiarios.Rows.Count.ToString() + " beneficiarios registrados";
        }

        protected void Img_AgregarBenef_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("Beneficiario.aspx?LoadCode=0");
        }

        protected void Eliminar_benef(object sender, CommandEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_EliminarBeneficiario", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_pagobenef", Convert.ToInt32(e.CommandArgument));
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
                this.gv_verBeneficiarios.DataBind();
            }
            catch (Exception)
            { }
        }

        #region Métodos de Mauricio Arias Olave.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 25/06/2014.
        /// Mostrar la tabla para administración de beneficiaros o los controles según sea el caso.
        /// </summary>
        private void MostrarTabla()
        {
            //Inicializar variables.
            DataTable RS = new DataTable();
            Int32 CodEstadoProyecto = 0;
            String CodProyecto;

            try
            {
                txtSQL = " SELECT CodEstado, CodProyecto FROM Proyecto P, ProyectoContacto PC " +
                         " WHERE P.id_proyecto = PC.CodProyecto AND PC.Inactivo = 0 " +
                         " AND PC.CodContacto = " + usuario.IdContacto;

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0)
                {
                    CodEstadoProyecto = Int32.Parse(RS.Rows[0]["CodEstado"].ToString());
                    CodProyecto = RS.Rows[0]["CodProyecto"].ToString();
                }

                if (CodEstadoProyecto < Constantes.CONST_Ejecucion || CodEstadoProyecto >= Constantes.CONST_Asignado_para_acreditacion)
                { tabla_default.Visible = true; tabla_normal.Visible = false; }
                else { tabla_default.Visible = false; tabla_normal.Visible = true; }

            }
            catch { tabla_default.Visible = false; tabla_normal.Visible = true; }
        }

        #endregion
    }
}