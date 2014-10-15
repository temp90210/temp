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
    public partial class ReporteFechas : System.Web.UI.Page
    {
        String idConvocatoriaEval;
        String nomConvocatoriaEval;

        DataTable informacionGeneral;

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
            ////L_Fecha.Text = "" + DateTime.Now.Day + " Del Mes " + DateTime.Now.Month + " De " + DateTime.Now.Year;

            informacionInicial();

            llenarGrilla();

            GV_Datos.DataSource = datatable;
            GV_Datos.DataBind();
        }

        private void informacionInicial()
        {
            informacionGeneral = new DataTable();

            informacionGeneral.Columns.Add("FechaInicio");
            informacionGeneral.Columns.Add("DIAS");
            informacionGeneral.Columns.Add("Evaluador");
            informacionGeneral.Columns.Add("Id_Proyecto");
            informacionGeneral.Columns.Add("NomProyecto");
            informacionGeneral.Columns.Add("Codigo");
            informacionGeneral.Columns.Add("NomSubSector");

            String sql;

            sql = "SELECT [FechaInicio], DATEDIFF(DAY , [FechaInicio], GETDATE()) AS DIAS, [Nombres] + ' ' + [Apellidos] AS Evaluador, [Id_Proyecto], [NomProyecto], [Codigo], [NomSubSector] FROM [Fonade].[dbo].[Proyecto] INNER JOIN [Fonade].[dbo].[ProyectoContacto] AS PC ON [Id_Proyecto] = PC.[CodProyecto] AND [CodRol] = " + Constantes.CONST_RolEvaluador + " AND PC.[Inactivo] = 0 INNER JOIN [Fonade].[dbo].[ConvocatoriaProyecto] AS CP ON PC.[CodConvocatoria] = CP.[CodConvocatoria] AND [Id_Proyecto] = CP.[CodProyecto] AND CP.[CodConvocatoria] = " + idConvocatoriaEval + " INNER JOIN [Fonade].[dbo].[Contacto] ON [Id_Contacto] = PC.[CodContacto] INNER JOIN [Fonade].[dbo].[SubSector] ON [Id_SubSector] = [CodSubSector] ORDER BY [Id_Contacto]";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DataRow fila = informacionGeneral.NewRow();
                    fila["FechaInicio"] = reader["FechaInicio"].ToString();
                    fila["DIAS"] = reader["DIAS"].ToString();
                    fila["Evaluador"] = reader["Evaluador"].ToString();
                    fila["Id_Proyecto"] = reader["Id_Proyecto"].ToString();
                    fila["NomProyecto"] = reader["NomProyecto"].ToString();
                    fila["Codigo"] = reader["Codigo"].ToString();
                    fila["NomSubSector"] = reader["NomSubSector"].ToString();
                    informacionGeneral.Rows.Add(fila);
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

        private void llenarGrilla()
        {
            datatable = new DataTable();

            datatable.Columns.Add("FechaInicio");
            datatable.Columns.Add("DIAS");
            datatable.Columns.Add("Evaluador");
            datatable.Columns.Add("NomProyecto");
            datatable.Columns.Add("CIIU");
            datatable.Columns.Add("Tarea1");
            datatable.Columns.Add("Tarea2");
            datatable.Columns.Add("Tarea3");
            datatable.Columns.Add("Agendo");
            datatable.Columns.Add("Responsable");

            for (int i = 0; i < informacionGeneral.Rows.Count; i++)
            {
                //DataRow fila = datatable.NewRow();

                //fila["FechaInicio"] = informacionGeneral.Rows[i]["FechaInicio"].ToString();
                //fila["DIAS"] = informacionGeneral.Rows[i]["DIAS"].ToString();
                //fila["Evaluador"] = informacionGeneral.Rows[i]["Evaluador"].ToString();
                //fila["NomProyecto"] = informacionGeneral.Rows[i]["NomProyecto"].ToString();
                //fila["CIIU"] = informacionGeneral.Rows[i]["Codigo"].ToString() + informacionGeneral.Rows[i]["NomSubSector"].ToString();

                String sql = "SELECT [NomTareaUsuario], [Fecha], DATEDIFF(HOUR, [Fecha], GETDATE()) AS DIAS, A.[Nombres] + ' ' + A.[Apellidos] + ' (' + AG.[NomGrupo]  + ')' AS GrupoAgendo, C.[Nombres] + ' ' + C.[Apellidos] + ' (' + CG.[NomGrupo]  + ')' AS GrupoResponsable FROM [Fonade].[dbo].[TareaUsuario] AS TU INNER JOIN [Fonade].[dbo].[TareaUsuarioRepeticion] ON [Id_TareaUsuario] = [CodTareaUsuario] AND [FechaCierre] IS NULL INNER JOIN [Fonade].[dbo].[Contacto] AS A ON A.[Id_Contacto] = TU.[CodContactoAgendo] AND A.[Inactivo] = 0 INNER JOIN [Fonade].[dbo].[Contacto] AS C ON C.[Id_Contacto] = TU.[CodContacto] AND C.[Inactivo] = 0 INNER JOIN [Fonade].[dbo].[GrupoContacto] AS GC ON C.[Id_Contacto] = GC.[CodContacto] INNER JOIN [Fonade].[dbo].[GrupoContacto] AS GA ON A.[Id_Contacto] = GA.[CodContacto] INNER JOIN [Fonade].[dbo].[Grupo] AS CG ON CG.[Id_Grupo] = GC.[CodGrupo] INNER JOIN [Fonade].[dbo].[Grupo] AS AG ON AG.[Id_Grupo] = GA.[CodGrupo] WHERE [CodProyecto] = " + informacionGeneral.Rows[i]["Id_Proyecto"].ToString() + " AND [CodTareaPrograma] = " + Constantes.CONST_PostIt + " ORDER BY DIAS";

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        DataRow fila = datatable.NewRow();

                        fila["FechaInicio"] = informacionGeneral.Rows[i]["FechaInicio"].ToString();
                        fila["DIAS"] = informacionGeneral.Rows[i]["DIAS"].ToString();
                        fila["Evaluador"] = informacionGeneral.Rows[i]["Evaluador"].ToString();
                        fila["NomProyecto"] = informacionGeneral.Rows[i]["NomProyecto"].ToString();
                        fila["CIIU"] = informacionGeneral.Rows[i]["Codigo"].ToString() + informacionGeneral.Rows[i]["NomSubSector"].ToString();

                        fila["Tarea1"] = "";
                        fila["Tarea2"] = "";
                        fila["Tarea3"] = "";
                        if (Int32.Parse(reader["DIAS"].ToString()) >= 24 && Int32.Parse(reader["DIAS"].ToString()) < 48)
                        {
                            fila["Tarea1"] = reader["NomTareaUsuario"].ToString();
                        }
                        else
                        {
                            if (Int32.Parse(reader["DIAS"].ToString()) >= 48 && Int32.Parse(reader["DIAS"].ToString()) < 72)
                            {
                                fila["Tarea2"] = reader["NomTareaUsuario"].ToString();
                            }
                            else
                            {
                                fila["Tarea3"] = reader["NomTareaUsuario"].ToString();
                            }
                        }
                        
                        fila["Agendo"] = reader["GrupoAgendo"].ToString();
                        fila["Responsable"] = reader["GrupoResponsable"].ToString();

                        datatable.Rows.Add(fila);
                        reader.Read();

                        try
                        {
                            do
                            {
                                fila = datatable.NewRow();
                                fila["FechaInicio"] = "";
                                fila["DIAS"] = "";
                                fila["Evaluador"] = "";
                                fila["NomProyecto"] = "";
                                fila["CIIU"] = "";

                                fila["Tarea1"] = "";
                                fila["Tarea2"] = "";
                                fila["Tarea3"] = "";
                                if (Int32.Parse(reader["DIAS"].ToString()) >= 24 && Int32.Parse(reader["DIAS"].ToString()) < 48)
                                {
                                    fila["Tarea1"] = reader["NomTareaUsuario"].ToString();
                                }
                                else
                                {
                                    if (Int32.Parse(reader["DIAS"].ToString()) >= 48 && Int32.Parse(reader["DIAS"].ToString()) < 72)
                                    {
                                        fila["Tarea2"] = reader["NomTareaUsuario"].ToString();
                                    }
                                    else
                                    {
                                        fila["Tarea3"] = reader["NomTareaUsuario"].ToString();
                                    }
                                }

                                fila["Agendo"] = reader["GrupoAgendo"].ToString();
                                fila["Responsable"] = reader["GrupoResponsable"].ToString();

                                datatable.Rows.Add(fila);
                            } while (reader.Read());
                        }
                        catch (InvalidOperationException) { }
                    }
                    else
                    {
                        DataRow fila = datatable.NewRow();

                        fila["FechaInicio"] = informacionGeneral.Rows[i]["FechaInicio"].ToString();
                        fila["DIAS"] = informacionGeneral.Rows[i]["DIAS"].ToString();
                        fila["Evaluador"] = informacionGeneral.Rows[i]["Evaluador"].ToString();
                        fila["NomProyecto"] = informacionGeneral.Rows[i]["NomProyecto"].ToString();
                        fila["CIIU"] = informacionGeneral.Rows[i]["Codigo"].ToString() + informacionGeneral.Rows[i]["NomSubSector"].ToString();

                        fila["Tarea1"] = "";
                        fila["Tarea2"] = "";
                        fila["Tarea3"] = "";
                        fila["Agendo"] = "";
                        fila["Responsable"] = "";

                        datatable.Rows.Add(fila);
                    }
                    
                    //reader.Close();
                }
                catch (SqlException)
                {
                }
                finally
                {
                    conn.Close();
                }

                //datatable.Rows.Add(fila);
            }
        }
    }
}