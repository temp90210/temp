using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Negocio;
using System.Configuration;
using System.Web.UI;
using Fonade.Clases;
using System.Text.RegularExpressions;

namespace Fonade.FONADE.Administracion
{
    public partial class DesactivarUnidadEmprende : Base_Page
    {
        #region Variables globales.

        /// <summary>
        /// Código de la institución seleccionada para su desactivación.
        /// </summary>
        string Cod_Seleccionado;

        /// <summary>
        /// Nombre de la institución seleccionada.
        /// </summary>
        String Nombre_Seleccionado;

        /// <summary>
        /// Variable que contiene las consultas SQL.
        /// </summary>
        String txtSQL;

        /// <summary>
        /// Fecha actual.
        /// </summary>
        DateTime fecha_actual = DateTime.Today;

        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Cod_Seleccionado = Session["Cod_Institucion_Selected"].ToString();
                Nombre_Seleccionado = Session["NombreUnidad_Seleccionado"].ToString();

                txtSQL = " SELECT NomInstitucion, NomUnidad FROM Institucion WHERE Id_Institucion = " + Cod_Seleccionado;

                var dt = consultas.ObtenerDataTable(txtSQL, "text");

                lbl_titulo_data_desactivar.Text = "Desactivar " + dt.Rows[0]["NomInstitucion"].ToString() + " - " + dt.Rows[0]["NomUnidad"].ToString();

                dt = null;

                #region Cargar la fecha actual.
                if (!IsPostBack)
                {
                    //Cargar la fecha actual.
                    lbl_fechaInicio.Text = fecha_actual.ToString("M/dd/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                    DDL_Dia.SelectedValue = fecha_actual.Day.ToString();
                    DDL_Mes.SelectedValue = fecha_actual.Month.ToString();
                    DD_Anio.SelectedValue = fecha_actual.Year.ToString();
                } 
                #endregion
            }
            catch
            {
                ClientScriptManager cm = this.ClientScript;
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
            }
        }

        /// <summary>
        /// Desactivar institución seleccionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_desactivar_Click(object sender, EventArgs e)
        {
            if (DesactivarUnidad() == true)
            {
                Session["Cod_Institucion_Selected"] = null;
                Session["NombreUnidad_Seleccionado"] = null;
                ClientScriptManager cm = this.ClientScript;
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location.reload();window.close();</script>");
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 26/06/2014.
        /// Desactivar unidad de emprendimiento seleccionada / institución seleccionado.
        /// </summary>
        /// <returns>TRUE = Proceso correcto. // FALSE = Error.</returns>
        private bool DesactivarUnidad()
        {
            //Inicializar variables.
            bool final = false;
            DateTime fecInicio = new DateTime();
            DateTime fecFin = new DateTime();
            DataTable RS = new DataTable();
            String CodJefeUnidad = "";
            DataTable rstProyecto = new DataTable();
            string Caso = "";

            try
            {
                //Obtener las fechas de inicio y fin.
                fecInicio = fecha_actual; //Convert.ToDateTime(lbl_fechaInicio.Text);
                fecFin = Convert.ToDateTime(DDL_Dia.SelectedValue + "/" + DDL_Mes.SelectedValue + "/" + DD_Anio.SelectedValue);

                #region Actualizar datos de acuerdo a si ha chequeado el CheckBox "Desactivar indefinidamente".

                if (!txtIndefinido.Checked)
                {
                    //txtSQL = " UPDATE Institucion SET Inactivo = 1, FechaInicioInactivo = '" + fecInicio + "' WHERE Id_Institucion = " + Cod_Seleccionado;
                    Caso = "TRUE";
                }
                else
                {
                    //txtSQL = " UPDATE Institucion Set Inactivo = 1, FechaInicioInactivo = '" + fecInicio + "', FechaFinInactivo = '" + fecFin + "' WHERE Id_Institucion = " + Cod_Seleccionado;
                    Caso = "FALSE";
                }

                ////Ejecutar consulta SQL "con fecha".
                //ejecutaReader(txtSQL, 2);

                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    SqlCommand cmd = new SqlCommand("MD_DesactivarUnidadEmprendimiento", con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Caso", Caso);
                    cmd.Parameters.AddWithValue("@CodInstitucion ", Cod_Seleccionado);
                    cmd.Parameters.AddWithValue("@FechaInicioInactivo", fecInicio);
                    cmd.Parameters.AddWithValue("@FechaFinInactivo", fecFin);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
                catch { }

                final = true;

                #endregion

                #region Deja Inactivos los usuarios Jefes de unidad y asesores que pertenecen a esa institución.

                txtSQL = " UPDATE Contacto SET Inactivo = 1, CodInstitucion = NULL " +
                         " WHERE CodInstitucion = " + Cod_Seleccionado +
                         " AND Id_Contacto IN(SELECT CodContacto FROM GrupoContacto WHERE CodGrupo IN(" + Constantes.CONST_JefeUnidad + "," + Constantes.CONST_Asesor + ")) ";

                //Ejecutar consulta SQL.
                ejecutaReader(txtSQL, 1);

                final = true;

                #endregion

                #region Trae el jefe de la unidad.

                txtSQL = " SELECT CodContacto FROM InstitucionContacto WHERE FechaFin IS NULL AND CodInstitucion = " + Cod_Seleccionado;

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0)
                {
                    CodJefeUnidad = RS.Rows[0]["CodContacto"].ToString();

                    //Deja a los jefes de unidad sin Rol.
                    txtSQL = " DELETE FROM GrupoContacto WHERE CodContacto = " + CodJefeUnidad;

                    //Ejecutar consulta SQL.
                    ejecutaReader(txtSQL, 1);

                    final = true;
                }

                #endregion

                #region Cierra la relacion entre la Institución y el jefe de la unidad.

                txtSQL = " UPDATE InstitucionContacto SET FechaFin = GETDATE() WHERE FechaFin IS NULL " +
                         " AND CodInstitucion = " + Cod_Seleccionado;

                //Ejecutar consulta SQL.
                ejecutaReader(txtSQL, 1);

                final = true;

                #endregion

                #region Reasigna los proyectos de la unidad.

                txtSQL = " SELECT Id_Proyecto, NomProyecto, CodEstado FROM Proyecto WHERE CodInstitucion = " + Cod_Seleccionado;

                rstProyecto = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow row_rstProyecto in rstProyecto.Rows)
                {
                    //Trae los asesores del proyecto.
                    txtSQL = " SELECT PC.CodContacto FROM ProyectoContacto PC, Proyecto P " +
                             " WHERE PC.CodProyecto = P.Id_Proyecto" +
                             " AND PC.CodRol IN(" + Constantes.CONST_RolAsesorLider + "," + Constantes.CONST_RolAsesor + ")" +
                             " AND PC.FechaFin IS NULL" +
                             " AND PC.Inactivo = 0" +
                             " AND PC.CodProyecto = " + row_rstProyecto["Id_Proyecto"].ToString();

                    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    #region Deja a los asesores sin rol.

                    foreach (DataRow row_RS in RS.Rows)
                    {
                        txtSQL = " DELETE FROM GrupoContacto WHERE CodContacto = " + row_RS["CodContacto"].ToString();

                        //Ejecutar consulta SQL.
                        ejecutaReader(txtSQL, 1);

                        final = true;
                    }

                    #endregion

                    #region Deja el proyecto sin asesores.

                    txtSQL = " UPDATE ProyectoContacto SET FechaFin = GETDATE(), Inactivo = 1 " +
                             " WHERE CodProyecto = " + row_rstProyecto["Id_Proyecto"].ToString() +
                             " AND CodRol IN(" + Constantes.CONST_RolAsesorLider + "," + Constantes.CONST_RolAsesor + ") ";

                    //Ejecutar consulta SQL.
                    ejecutaReader(txtSQL, 1);

                    final = true;

                    #endregion

                    //Si al proyecto no le han asignado recursos pasara a REASIGNACION POR ASIGNACION:
                    if (Int32.Parse(row_rstProyecto["CodEstado"].ToString()) < Constantes.CONST_AsignacionRecursos)
                    {
                        #region Ejecutar bloque #1.

                        #region Actualización #1.

                        txtSQL = " UPDATE Proyecto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                                 " WHERE Id_Proyecto = " + row_rstProyecto["Id_Proyecto"].ToString();

                        //Ejecutar consulta SQL.
                        ejecutaReader(txtSQL, 1);

                        final = true;

                        #endregion

                        #region Los emprendedores pasan a la unidad temporal junto con sus proyectos (Actualización #2).

                        txtSQL = " UPDATE Contacto set CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                 " where id_contacto in (select codcontacto from proyectocontacto " +
                                 " where codproyecto = " + row_rstProyecto["Id_Proyecto"].ToString() + " and inactivo=0) ";

                        //Ejecutar consulta SQL.
                        ejecutaReader(txtSQL, 1);

                        final = true;

                        #endregion

                        #region CÓDIGO COMENTADO EN CLÁSICO.

                        //                                    'Asigna tarea a jefe de unidad REASIGNACION POR ASIGNACION, para asignacion de asesor
                        //'			txtSQL = "SELECT CodContacto FROM InstitucionContacto WHERE FechaFin IS NULL AND CodInstitucion ="& CONST_UnidadTemporal
                        //'			Set RS = Conn.Execute(txtSQL)
                        //'			
                        //'			If Not RS.EOF Then
                        //'				prTareaAsignarAsesor RS("CodContacto"),Session("CodUsuario"),rstProyecto("Id_Proyecto"),rstProyecto("NomProyecto")
                        //'			End If

                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region Ejecutar bloque #2.

                        #region Si al proyecto ya le han asignado recursos pasara a REASIGNACION POR SEGUIMIENTO.

                        txtSQL = " UPDATE Proyecto SET CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                 " WHERE Id_Proyecto = " + row_rstProyecto["Id_Proyecto"].ToString();

                        //Ejecutar consulta SQL.
                        ejecutaReader(txtSQL, 1);

                        final = true;

                        #endregion

                        #region Los emprendedores pasan a la unidad temporal junto con sus proyectos.

                        txtSQL = " UPDATE Contacto set CodInstitucion = " + Constantes.CONST_UnidadTemporal +
                                 " where id_contacto in (select codcontacto from proyectocontacto " +
                                 " where codproyecto = " + row_rstProyecto["Id_Proyecto"].ToString() +
                                 " and inactivo = 0) ";

                        //Ejecutar consulta SQL.
                        ejecutaReader(txtSQL, 1);

                        final = true;

                        #endregion

                        #region CÓDIGO COMENTADO EN FONADE CLÁSICO.

                        //                                    'Asigna tarea a jefe de unidad REASIGNACION POR ASIGNACION, para asignacion de asesor
                        //'			txtSQL = "SELECT CodContacto FROM InstitucionContacto WHERE FechaFin IS NULL AND CodInstitucion ="& CONST_UnidadTemporal
                        //'			Set RS = Conn.Execute(txtSQL)
                        //'			
                        //'			If Not RS.EOF Then
                        //'				prTareaAsignarAsesor RS("CodContacto"),Session("CodUsuario"),rstProyecto("Id_Proyecto"),rstProyecto("NomProyecto")
                        //'			End If

                        #endregion

                        #endregion
                    }
                }

                #endregion

                return final;
            }
            catch { return final; }
        }
    }
}