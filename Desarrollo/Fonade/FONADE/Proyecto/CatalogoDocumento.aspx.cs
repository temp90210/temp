using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Datos;
using System.IO;

namespace Fonade.FONADE.Proyecto
{
    public partial class CatalogoDocumento : Negocio.Base_Page
    {
        #region Variables globales.

        String txtTab;
        String CodProyecto;
        String txtSQL;
        Boolean bRepetido;
        String Accion;
        Boolean Miembro;
        String CodDocumento;

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
                txtTab = Session["txtTab"] != null && !string.IsNullOrEmpty(Session["txtTab"].ToString()) ? Session["txtTab"].ToString() : "0";
                Accion = Session["Accion"] != null && !string.IsNullOrEmpty(Session["Accion"].ToString()) ? Session["Accion"].ToString() : "0";
            }
            catch { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); }

            //Se valida que si tenga datos "válidos".
            if (CodProyecto == "0" || txtTab == "0") { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); }

            if (!IsPostBack)
            {
                //Consultar si es miembro.
                Miembro = fnMiembroProyecto(usuario.IdContacto, CodProyecto);

                if (Accion == "Nuevo") { btn_Accion.Text = "Crear"; LimpiarCampos(); }
                else
                {
                    CargarGrillaDocumentos();
                }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// Eliminar documento.
        /// Se debe ajustar la funcionalidad para "Eliminar el documento físicamente".
        /// </summary>
        /// <param name="CodDocumento">Código del documento.</param>
        private void Eliminar(String CodDocumento)
        {
            //Inicializar las variables.
            SqlCommand cmd = new SqlCommand();

            try
            {
                //Borrar la inversión
                txtSQL = "update Documento set borrado=1 where Id_Documento = " + CodDocumento;

                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    cmd = new SqlCommand(txtSQL, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                    CargarGrillaDocumentos();
                }
                catch { }

                #region Modificacion para Eliminar el archivo Físicamente COMENTADO!
                ////Vladimir Delgado Barbosa 15 Mayo de 2007
                //Dim RSB
                //txtSQL = "select distinct URL from Documento where borrado=1 and Id_Documento ="&CodDocumento
                //set RSB= Conn.execute(txtSQL)
                //if Not RSB.EOF then BorrarArchivo(RSB("URL"))
                ////Fin de modificacion 
                #endregion
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// Actualizar documento.
        /// </summary>
        private void Actualizar()
        {
            //Inicializar las variables.
            SqlCommand cmd = new SqlCommand();

            try
            {
                //Borrar la inversión
                txtSQL = " Update Documento set NomDocumento ='" + NomDocumento.Text + "'," +
                         " Comentario = '" + Comentario.Text + "'," +
                         " codTab = " + txtTab;

                if (Link.Text != "") { txtSQL = txtSQL + ", Url='" + Link.Text + "'"; }

                txtSQL = txtSQL + " WHERE Id_Documento = " + CodDocumento;

                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    cmd = new SqlCommand(txtSQL, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// Validar el "documento" antes de regitrarlo en BD.
        /// </summary>
        /// <returns>string vacío = puede continuar. // string con datos: Ver mensaje.</returns>
        private string Validar()
        {
            //Inicializar variables.
            String msg = "";

            try
            {
                if (bRepetido) { msg = "Ya existe un Documento con ese Nombre."; }

                if (NomDocumento.Text.Trim() == "") { msg = Texto("TXT_NOMBRE_REQ"); }

                if (Accion == "Nuevo")
                { if (!Archivo.HasFile || Link.Text == "") { msg = "El archivo o link del documento es requerido"; } }

                return msg;
            }
            catch (Exception ex) { msg = "Error: " + ex.Message; return msg; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// Cargar la información "detalles" del documento seleccionado.
        /// </summary>
        private void CargarInfo_Documento()
        {
            //Inicializar variables.
            DataTable RsDocumento = new DataTable();

            try
            {
                //Mostrar panel.
                pnl_datos.Visible = true;
                pnl_Grilla.Visible = false;

                txtSQL = " SELECT nomdocumento, url, comentario, extension " +
                         " FROM Documento, documentoformato " +
                         " WHERE Id_DocumentoFormato = CodDocumentoFormato and Id_Documento=" + CodDocumento;

                RsDocumento = consultas.ObtenerDataTable(txtSQL, "text");

                lblTitulo.Text = "EDITAR DOCUMENTO";

                if (RsDocumento.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(RsDocumento.Rows[0]["nomdocumento"].ToString()))
                    { NomDocumento.Text = RsDocumento.Rows[0]["nomdocumento"].ToString(); }

                    if (!String.IsNullOrEmpty(RsDocumento.Rows[0]["Extension"].ToString()) && RsDocumento.Rows[0]["Extension"].ToString() == "Link")
                    { Link.Visible = true; Link.Text = RsDocumento.Rows[0]["Extension"].ToString(); }

                    if (!String.IsNullOrEmpty(RsDocumento.Rows[0]["comentario"].ToString()))
                    { Comentario.Text = RsDocumento.Rows[0]["comentario"].ToString(); }
                }

                btn_Accion.Text = "Actualizar";
            }
            catch
            {
                pnl_datos.Visible = false;
                pnl_Grilla.Visible = true;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// Cargar la grilla con los documentos.
        /// </summary>
        private void CargarGrillaDocumentos()
        {
            //Inicializar variables.
            DataTable RsDocumento = new DataTable();

            try
            {
                txtSQL = " select id_documento, nomdocumento, fecha, url, nomdocumentoformato, icono, CodDocumentoFormato " +
                         " from documento, documentoformato " +
                         " where id_documentoformato = coddocumentoformato and borrado = 0 " +
                         " and codproyecto = " + CodProyecto + " and codTab = " + txtTab + "  order by nomdocumento";

                RsDocumento = consultas.ObtenerDataTable(txtSQL, "text");

                Session["docs"] = RsDocumento;
                gv_Documentos.DataSource = RsDocumento;
                gv_Documentos.DataBind();

                pnl_datos.Visible = false;
                pnl_Grilla.Visible = true;
                btn_Accion.Text = "Actualizar";
            }
            catch { }
        }

        /// <summary>
        /// Determinar la acción a tomar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Accion_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            String validado = "";

            validado = Validar();

            if (validado == "")
            {
                if (btn_Accion.Text == "Crear")
                {

                }
                else
                {
                    if (btn_Accion.Text == "Actualizar")
                    { Actualizar(); }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + validado + "');", true);
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// RowDataBound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_Documentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lnk = e.Row.FindControl("lnk_eliminar") as LinkButton;
                var img = e.Row.FindControl("imgDoc") as ImageButton;
                var hdf = e.Row.FindControl("hdf_icono") as HiddenField;
                var lnk2 = e.Row.FindControl("lnk_NomDoc") as LinkButton;
                var lbl = e.Row.FindControl("lbl_Fecha") as Label;
                DateTime fecha = new DateTime();

                if (lnk != null && img != null && hdf != null && lnk2 != null && lbl != null)
                {
                    if (Miembro && usuario.CodGrupo == Constantes.CONST_Emprendedor)
                    {
                        lnk.Visible = true;
                        lnk.OnClientClick = "return alerta();";
                    }
                    else
                    { lnk.Visible = false; lnk2.Enabled = false; }

                    if (hdf.Value != "")
                    { img.ImageUrl = "../../Images/" + hdf.Value; }
                    else { img.ImageUrl = "../../Images/FileMain.gif"; }

                    if (img.CommandArgument == "17")
                    {
                        //Se redirecciona cuando se cliquee a "OfflineProcesaCarga.asp"
                    }
                    else
                    {
                        //Se abre una nueva ventana emergente "_blank".
                    }

                    try
                    {
                        fecha = DateTime.Parse(lbl.Text);
                        lbl.Text = fecha.ToString();

                        #region Formatear la fecha.

                        //Obtener el nombre del mes (las primeras tres letras).
                        string sMes = fecha.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                        //Obtener la hora en minúscula.
                        string hora = fecha.ToString("hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant();

                        //Reemplazar el valor "am" o "pm" por "a.m" o "p.m" respectivamente.
                        if (hora.Contains("am")) { hora = hora.Replace("am", "a.m"); } if (hora.Contains("pm")) { hora = hora.Replace("pm", "p.m"); }

                        //Formatear la fecha según manejo de FONADE clásico. "Ej: Nov 19 de 2013 07:36:26 p.m.".
                        lbl.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year + " " + hora + ".";

                        #endregion
                    }
                    catch { fecha = DateTime.Today; lbl.Text = ""; }
                }
            }
        }

        #region Métodos de Mauricio Arias Olave.

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

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// Upload.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            HttpPostedFile file = Request.Files["myFile"];

            //check file was submitted
            if (file != null && file.ContentLength > 0)
            {
                string fname = Path.GetFileName(file.FileName);
                file.SaveAs(Server.MapPath(Path.Combine("~/App_Data/", fname)));
            }
        }

        /// <summary>
        /// Limpiar campos.
        /// </summary>
        private void LimpiarCampos()
        {
            pnl_Grilla.Visible = false;
            pnl_datos.Visible = true;
            NomDocumento.Text = "";
            Link.Text = "";
            Comentario.Text = "";
            lblTitulo.Text = "NUEVO DOCUMENTO";
        }

        /// <summary>
        /// RowCommand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_Documentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "eliminar")
            {
                Eliminar(e.CommandArgument.ToString());
            }
            if (e.CommandName == "Descargar")
            {
                //Se tiene que validar a qué página vá a dirigirse, o puede ser "OfflineProcesaCarga.aspx" 
                //o se hace la descarga directa.
            }
            if (e.CommandName == "editar")
            {
                CodDocumento = e.CommandArgument.ToString();
                CargarInfo_Documento();
            }
        }

        /// <summary>
        /// Cerrar ventana emergente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_cerrar_Click(object sender, EventArgs e)
        { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); }
    }
}