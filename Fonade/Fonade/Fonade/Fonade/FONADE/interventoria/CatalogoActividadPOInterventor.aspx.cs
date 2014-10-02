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
using System.Globalization;

namespace Fonade.FONADE.interventoria
{
    public partial class CatalogoActividadPOInterventor : Negocio.Base_Page
    {
        //variables 
        private string conexionstr = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        int CodProyecto;
        String CodActividad;
        int Mes;
        String NombreInterventor;
        string pagina;
        protected int CodContacto;
        String txtSQL;
        int txtTab = Constantes.CONST_PlanOperativoInter2;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string accion = Session["Accion"].ToString();
                    CodProyecto = (int)
                     (!string.IsNullOrEmpty(Session["CodProyecto"].ToString())
                          ? Convert.ToInt64(Session["CodProyecto"])
                          : 0);
                    CodActividad = Session["CodActividad"] != null && !string.IsNullOrEmpty(Session["CodActividad"].ToString()) ? Session["CodActividad"].ToString() : "0";
                    Mes = (int)
                    (!string.IsNullOrEmpty(Session["linkid"].ToString())
                         ? Convert.ToInt64(Session["linkid"])
                         : 0);
                    pagina = Session["pagina"] != null && !string.IsNullOrEmpty(Session["pagina"].ToString()) ? Session["pagina"].ToString() : "0";
                    lbl_enunciado.Text = void_establecerTitulo("Editar Avance");

                    ConsultarInformcionPlanOperativo();
                    MostrarInterventor();
                    //Deshabilitar campos:
                    if (usuario.CodGrupo == Constantes.CONST_Interventor || usuario.CodGrupo == Constantes.CONST_Asesor || usuario.CodGrupo == Constantes.CONST_JefeUnidad || usuario.CodGrupo == Constantes.CONST_CallCenter)
                    {
                        txt_cargo.Enabled = false;
                        txt_mes.Enabled = false;
                        txt_sueldo_obtenido.Enabled = false;
                        txt_prestaciones_obtenidas.Enabled = false;
                        txt_observaciones.Enabled = false;
                    }
                    if (accion == "actualizar" && usuario.CodGrupo == Constantes.CONST_Emprendedor)
                    {
                        txt_cargo.Enabled = false;
                        txt_mes.Enabled = false;
                        txt_sueldo_obtenido.Enabled = false;
                        txt_prestaciones_obtenidas.Enabled = false;
                        txt_observaciones.Enabled = false;
                        txt_observ_interventor.Enabled = false;
                        dd_aprobado.Enabled = false;
                        B_Acion.Visible = false;
                    }
                    if (accion == "Reportar" && usuario.CodGrupo == Constantes.CONST_Emprendedor)
                    {
                        lbl_enunciado.Text = void_establecerTitulo("Nuevo Avance");
                        tr_mes.Style.Add(HtmlTextWriterStyle.Display, "none");
                        tr_actividad.Style.Add(HtmlTextWriterStyle.Display, "none");
                        tr_observ_inter.Style.Add(HtmlTextWriterStyle.Display, "none");
                        txt_cargo.Visible = false;
                        txt_mes.Visible = false;
                        txt_observ_interventor.Visible = false;
                        dd_aprobado.Visible = false;
                        B_Acion.Text = "Crear";
                        img_btn_NuevoDocumento.Visible = true;
                    }
                    if (accion == "Borrar")
                    { Borrar_ValoresProyectados(); }
                }
            }
            catch //(NullReferenceException)
            { Response.Redirect("FramePlanOperativoInterventoria.aspx"); }
        }

        private void ConsultarInformcionPlanOperativo()
        {
            //Obtiene la conexión
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());

            //Consultar la información.
            //String sqlConsulta = "SELECT * FROM AvanceCargoPOMes WHERE CodCargo = " + CodProyecto + " AND Mes = " + MesDeLaNomina + " ";
            String sqlConsulta = "select a.Mes,p.nomactividad,a.observaciones,a.Valor,a.observacionesInterventor,a.aprobada from proyectoactividadPOInterventor p inner join avanceactividadPOMes a on p.Id_Actividad = a.CodActividad where codproyecto = " + CodProyecto + " and codActividad = " + CodActividad + " and Valor <>0  and a.Mes=" + Mes;


            //Asignar SqlCommand para su ejecución.
            SqlCommand cmd = new SqlCommand(sqlConsulta, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    //Aplicar los valores a los campos de texto.

                    txt_mes.Text = reader["Mes"].ToString();
                    txt_cargo.Text = reader["nomactividad"].ToString();
                    txt_observaciones.Text = reader["Observaciones"].ToString();
                    txt_observ_interventor.Text = reader["ObservacionesInterventor"].ToString();


                    //dd_aprobado.SelectedValue = reader["aprobada"].ToString();
                    if (reader["Aprobada"].ToString() == "True") { dd_aprobado.Items[0].Selected = true; }
                    else { dd_aprobado.Items[1].Selected = true; }
                    txt_sueldo_obtenido.Text = Convert.ToDouble(reader["Valor"].ToString()).ToString();
                    txt_prestaciones_obtenidas.Text = "0";
                    decimal valorTotal = Convert.ToDecimal(reader["Valor"].ToString());
                    lbl_Total.Text = "$" + valorTotal.ToString("0,0.00", CultureInfo.InvariantCulture);
                    lbl_tipoReq_Enunciado.Text = void_establecerTitulo("REQUERIMIENTOS DE RECURSOS POR MES") + Mes;

                }
            }

            catch (Exception ex)
            {
            }
        }

        private void MostrarInterventor()
        {
            String sql;


            sql = "SELECT Nombres + ' ' + Apellidos AS Nombre from Contacto where id_Contacto = " + usuario.IdContacto;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lbl_Interventor.Text = reader["Nombre"].ToString();
                    DateTime fecha = DateTime.Now;
                    string sMes = fecha.ToString("MMM", CultureInfo.CreateSpecificCulture("es-CO"));
                    lbl_tiempo.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year;
                }
                reader.Close();
                conn.Close();
            }
            catch (SqlException)
            {
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Establecer el primer valor en mayúscula, retornando un string con la primera en maýsucula.
        /// </summary>
        /// <param name="s">String a procesar</param>
        /// <returns>String procesado.</returns>
        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        protected void btn_crearaporte_Click(object sender, EventArgs e)
        {

        }

        protected void BtnCerrar_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Crear o actualizar...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void B_Acion_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();

            switch (B_Acion.Text)
            {
                case "Crear":
                    #region Crear.

                    #endregion
                    break;
                case "Actualizar":
                    #region Actualizar.

                    cmd = new SqlCommand("MD_Insertar_Actualizar_planOperativo", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlCommand cmd2 = new SqlCommand();
                    try
                    {

                        cmd.Parameters.AddWithValue("@Mes", txt_mes.Text);
                        cmd.Parameters.AddWithValue("@_CodActividad", Session["CodActividad"].ToString());
                        cmd.Parameters.AddWithValue("@ObsInterventor", txt_observ_interventor.Text);
                        cmd.Parameters.AddWithValue("@Aprobada", dd_aprobado.SelectedValue);
                        cmd2 = new SqlCommand(UsuarioActual(), con);
                        con.Open();
                        cmd2.ExecuteNonQuery();
                        int RegistrosGuardados = cmd.ExecuteNonQuery();
                        if (RegistrosGuardados > 0)
                        {
                            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Modificado exitosamente!'); window.opener.location.reload(); window.close();", true);
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información de Plan Operativo procesada correctamente.'); window.opener.location.reload();window.close();", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al Actualizar los Registros!'); window.opener.location.reload();", true);
                        }
                    }

                    catch// (Exception ex)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Información de Plan Operativo actualizada correctamente.'); window.close();", true);
                        return;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();

                        cmd2.Dispose();
                        cmd.Dispose();
                    }

                    #endregion
                    break;
                default:
                    break;
            }
        }

        protected void B_Cancelar_Click(object sender, EventArgs e)
        {
            //javascript:closeWindow()
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true);
        }

        protected void img_btn_enlazar_grilla_PDF_Click(object sender, ImageClickEventArgs e)
        {
            CodActividad = Session["CodActividad"] != null && !string.IsNullOrEmpty(Session["CodActividad"].ToString()) ? Session["CodActividad"].ToString() : "0";
            Mes = (int)
            (!string.IsNullOrEmpty(Session["linkid"].ToString())
                 ? Convert.ToInt64(Session["linkid"])
                 : 0);
            Redirect(null, "CatalogoDocumentoInterventoria.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }

        protected void img_btn_NuevoDocumento_Click(object sender, ImageClickEventArgs e)
        {
            CodActividad = Session["CodActividad"] != null && !string.IsNullOrEmpty(Session["CodActividad"].ToString()) ? Session["CodActividad"].ToString() : "0";
            Mes = (int)
            (!string.IsNullOrEmpty(Session["linkid"].ToString())
                 ? Convert.ToInt64(Session["linkid"])
                 : 0);
            Session["Accion_Docs"] = img_btn_NuevoDocumento.CommandName;
            Redirect(null, "CatalogoDocumentoInterventoria.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/09/2014.
        /// Eliminar el valor proyectado "según FONADE clásico, se activa cuando se selecciona (Eliminar Avance)".
        /// </summary>
        private void Borrar_ValoresProyectados()
        {
            try
            {
                //Borrar los valores proyectados por mes de la actividad
                txtSQL = "Delete from AvanceActividadPOMes where CodActividad=" + CodActividad + " and mes=" + Mes;
                ejecutaReader(txtSQL, 2);

                //Actualizar la fecha de modificación del tab
                prActualizarTabInter(txtTab.ToString(), CodProyecto.ToString());

                //MOstarr mensaje y cerrar.
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Avance eliminado correctamente.'); window.opener.location.reload();window.close();", true);
            }
            catch { }
        }
    }
}