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
    public partial class ReporteEvaluadoresSector : System.Web.UI.Page
    {
        String idConvocatoriaEval;
        String nomConvocatoriaEval;

        DataTable datatable;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(Session["idConvocatoriaEval"].ToString()))
                {
                    idConvocatoriaEval = Session["idConvocatoriaEval"].ToString();
                    nomConvocatoriaEval = Session["idNombreConvocatoria"].ToString();
                }
            }
            catch (NullReferenceException)
            {
                Response.Redirect("ReportesEvaluacion");
            }

            L_ReportesEvaluacion.Text = "REPORTE #EVALUADORES POR SECTOR - " + nomConvocatoriaEval;
            //L_Fecha.Text = "" + DateTime.Now.Day + " Del Mes " + DateTime.Now.Month + " De " + DateTime.Now.Year;

            llenarGrilla();

            GV_Datos.DataSource = datatable;
            GV_Datos.DataBind();
        }

        private void llenarGrilla()
        {

            String nomRep = "";
            Int32 cuantos = 0;

            datatable = new DataTable();

            datatable.Columns.Add("Codigo");
            datatable.Columns.Add("NomSubSector");
            datatable.Columns.Add("Lugar");
            datatable.Columns.Add("Cuantos");

            String sql = "SELECT [Codigo], [NomSubSector], [NomCiudad] + ' (' + [NomDepartamento] + ')' AS Lugar, COUNT(PC.[CodContacto]) AS Cuantos FROM [Fonade].[dbo].[Proyecto] AS P INNER JOIN [Fonade].[dbo].[SubSector] ON [Id_SubSector] = [CodSubSector] INNER JOIN [Fonade].[dbo].[Ciudad] ON [Id_Ciudad] = [CodCiudad] INNER JOIN [Fonade].[dbo].[departamento] ON [Id_Departamento] = [CodDepartamento] INNER JOIN [Fonade].[dbo].[ConvocatoriaProyecto] AS CP ON [Id_Proyecto] = CP.[CodProyecto] AND [CodConvocatoria] = " + idConvocatoriaEval + " INNER JOIN [Fonade].[dbo].[ProyectoContacto] AS PC ON [Id_Proyecto] = PC.[CodProyecto] AND PC.[CodRol] = " + Constantes.CONST_RolEvaluador + " AND PC.[Inactivo] = 0 WHERE P.[Inactivo] = 0 GROUP BY [Codigo], [NomSubSector], [NomCiudad], [NomDepartamento] ORDER BY [Codigo], [NomCiudad]";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                do
                {
                    try
                    {
                        DataRow fila = datatable.NewRow();

                        if (!nomRep.Equals(reader["NomSubSector"].ToString()))
                        {
                            if (!String.IsNullOrEmpty(nomRep))
                            {
                                fila["Codigo"] = "";
                                fila["NomSubSector"] = "Total Evaluadores Sector";
                                fila["Lugar"] = "";
                                fila["Cuantos"] = cuantos;

                                datatable.Rows.Add(fila);
                                fila = datatable.NewRow();

                                cuantos = 0;
                            }
                            nomRep = reader["NomSubSector"].ToString();
                            fila["Codigo"] = reader["Codigo"].ToString();
                            fila["NomSubSector"] = reader["NomSubSector"].ToString();
                        }
                        else
                        {
                            fila["Codigo"] = "";
                            fila["NomSubSector"] = "";
                        }

                        fila["Lugar"] = reader["Lugar"].ToString();
                        fila["Cuantos"] = reader["Cuantos"].ToString();

                        cuantos += Int32.Parse(reader["Cuantos"].ToString());

                        datatable.Rows.Add(fila);
                    }
                    catch (InvalidOperationException) { }
                }
                while (reader.Read());

                DataRow fila1 = datatable.NewRow();

                if (!String.IsNullOrEmpty(nomRep))
                {
                    fila1["Codigo"] = "";
                    fila1["NomSubSector"] = "Total Evaluadores Sector";
                    fila1["Lugar"] = "";
                    fila1["Cuantos"] = cuantos;

                    datatable.Rows.Add(fila1);
                    cuantos = 0;
                }

                reader.Close();
            }
            catch (SqlException)
            {
            }
            finally
            {
                conn.Close();
            }
        }
    }
}