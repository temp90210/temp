using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.evaluacion
{
    public partial class AnexoEmprendedor : System.Web.UI.Page
    {
        String CodProyecto;
        String CodContacto;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(Session["commadArgumentAnexoEmprendedor"].ToString()))
                {
                    Response.Redirect("ReporteFinalAcreditacion.aspx");
                }
                else
                {
                    String[] valores = Session["commadArgumentAnexoEmprendedor"].ToString().Split(';');
                    CodProyecto = valores[0];
                    CodContacto = valores[1];
                }
            }
            catch (Exception) { }

            String sql = "SELECT NOMBRES  + ' ' + APELLIDOS FROM CONTACTO WHERE ID_CONTACTO=" + CodContacto;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    L_Conctat.Text = "Anexos del emprendedor: " + reader[0].ToString();
                }

                conn.Close();
            }
            catch (SqlException) { }
            finally
            {
                conn.Close();
            }

            buscarAnexos();
        }

        private void buscarAnexos()
        {

            String strTipoArchivo;
  		    String strDescripcion;

            DataTable puDatatable = new DataTable();

            puDatatable.Columns.Add("Archivo");
            puDatatable.Columns.Add("Tipo");
            puDatatable.Columns.Add("Descripcion");

            DataTable Anexos = new DataTable();

            Consultas consulta = new Consultas();

            String sql = "SELECT * FROM ContactoArchivosAnexos WHERE CODPROYECTO= '" + CodProyecto + "' and TipoArchivo ='Anexo1'";

            Anexos = consulta.ObtenerDataTable(sql, "text");

            if (Anexos != null)
            {
                if (Anexos.Rows.Count > 0)
                {
                    DataRow fila = puDatatable.NewRow();

                    strTipoArchivo = Anexos.Rows[0]["TipoArchivo"].ToString();
                    strDescripcion = strTipoArchivo + " - " + Anexos.Rows[0]["TipoArchivo"].ToString();

                    fila["Archivo"] = Anexos.Rows[0]["ruta"].ToString();
                    fila["Tipo"] = strTipoArchivo;
                    fila["Descripcion"] = strDescripcion;
                    puDatatable.Rows.Add(fila);
                }
            }

            sql = @"SELECT     a.*, e.TituloObtenido, es.NomNivelEstudio, a.CodProyecto, c.Nombres, c.Apellidos, a.TipoArchivo
                ,case when a.CodContactoEstudio is null then '' else  e.TituloObtenido + ' (' +  es.NomNivelEstudio + ')' end as Descripcion
                ,isnull(e.anotitulo,datepart(year,getdate())) as ano_titulo
                FROM
                ContactoArchivosAnexos AS a
                LEFT OUTER JOIN Contacto c ON c.Id_Contacto = a.CodContacto
                LEFT OUTER JOIN ContactoEstudio AS e  on e.Id_ContactoEstudio = a.CodContactoEstudio
                LEFT OUTER JOIN NivelEstudio AS es ON e.CodNivelEstudio = es.Id_NivelEstudio
                WHERE a.CodProyecto = " + CodProyecto + " AND a.codContacto= " + CodContacto + @"
                ORDER BY a.TipoArchivo, c.Id_Contacto, ano_titulo Desc";

            Anexos = consulta.ObtenerDataTable(sql, "text");

            if (Anexos != null)
            {
                if (Anexos.Rows.Count > 0)
                {
                    for (int i = 0; i < Anexos.Rows.Count; i++)
                    {
                        DataRow fila = puDatatable.NewRow();

                        strTipoArchivo = Anexos.Rows[i]["TipoArchivo"].ToString();
                        strDescripcion = strTipoArchivo + " - " + Anexos.Rows[i]["TipoArchivo"].ToString();

                        fila["Archivo"] = Anexos.Rows[i]["ruta"].ToString();
                        fila["Tipo"] = strTipoArchivo;
                        fila["Descripcion"] = strDescripcion;
                        puDatatable.Rows.Add(fila);
                    }
                }
            }

            GV_Anexos.DataSource = puDatatable;
            GV_Anexos.DataBind();
        }
    }
}