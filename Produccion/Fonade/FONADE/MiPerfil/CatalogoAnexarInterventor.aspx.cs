using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.IO;
using System.Web;

namespace Fonade.FONADE.MiPerfil
{
    public partial class CatalogoAnexarInterventor : Negocio.Base_Page
    {
        #region Variables globales.

        /// <summary>
        /// Contiene las consultas en SQL.
        /// </summary>
        String txtSQL;

        /// <summary>
        /// Nombre del documento a "eliminar".
        /// </summary>
        String v_NomDocumento;

        /// <summary>
        /// Cadena de conexión.
        /// </summary>
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

        /// <summary>
        /// SqlCommand.
        /// </summary>
        SqlCommand cmd;

        /// <summary>
        /// Determina si se vá a crear o actualizar un documento, el contenido de esta variable
        /// se colocará en el Button de este formulario.
        /// </summary>
        String Accion_Docs;

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 13/08/2014.
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                v_NomDocumento = Session["v_NomDocumento"] != null && !string.IsNullOrEmpty(Session["v_NomDocumento"].ToString()) ? Session["v_NomDocumento"].ToString() : "";
                Accion_Docs = Session["Accion_Docs"] != null && !string.IsNullOrEmpty(Session["Accion_Docs"].ToString()) ? Session["Accion_Docs"].ToString() : "Crear";
            }
            catch { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); }

            if (!IsPostBack)
            {
                btn_Accion.Text = Accion_Docs;

                if (Accion_Docs == "Vista")
                {
                    pnl_Grilla.Visible = true;
                    pnl_datos.Visible = false;
                }
                else
                {
                    pnl_Grilla.Visible = false;
                    pnl_datos.Visible = true;
                }
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
                txtSQL = " select * from InterventorAnexos, documentoformato " +
                         " where id_documentoformato=coddocumentoformato and borrado=0 " +
                         " and CodContacto =" + usuario.IdContacto + " order by nomdocumento";

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
                    #region Comentarios.
                    //if (Miembro && usuario.CodGrupo == Constantes.CONST_Emprendedor)
                    //{
                    //    lnk.Visible = true;
                    //    lnk.OnClientClick = "return alerta();";
                    //}
                    //else
                    //{ lnk.Visible = false; lnk2.Enabled = false; } 
                    #endregion

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

        /// <summary>
        /// Mauricio Arias Olave.
        /// 13/08/2014.
        /// Eliminar el "documento" seleccionado.
        /// </summary>
        /// <param name="P_NomDocumento">Nombre del documento.</param>
        private void Borrar(String P_NomDocumento)
        {
            try
            {
                //Borrar documento
                txtSQL = "update InterventorAnexos set borrado=1 where CodContacto =" + usuario.IdContacto + " and NomDocumento = '" + P_NomDocumento + "'";

                //NEW RESULTS:
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                cmd = new SqlCommand(txtSQL, con);

                if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd.Dispose();

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Documento eliminado.'); window.opener.location.reload();window.close();", true);
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 13/08/2014.
        /// Actualizar documento.
        /// </summary>
        private void Actualizar()
        {
            try
            {
                txtSQL = " Update InterventorAnexos set NomDocumento ='" + NomDocumento.Text + "'," +
                         " Comentario = '" + Comentario.Text + "'" +
                         " WHERE CodContacto = " + usuario.IdContacto + " AND NomDocumento='" + NomDocumentoAnt.Value + "'";


                //NEW RESULTS:
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                cmd = new SqlCommand(txtSQL, con);

                if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd.Dispose();

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Documento actualizado.'); window.opener.location.reload();window.close();", true);
            }
            catch { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo actualizar el documento.'); ", true); return; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 13/08/2014.
        /// Validar antes de guardar/actualizar el documento.
        /// </summary>
        /// <returns></returns>
        private string Validar()
        {
            string msg = "";

            try
            {
                if (NomDocumento.Text.Trim() == "") { msg = Texto("TXT_NOMBRE_REQ"); }
                if (btn_Accion.Text == "Crear" && NomDocumento.Text.Trim() == "") { msg = "El archivo del documento es requerido"; }

                return msg;
            }
            catch { msg = "Error desconocido."; return msg; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 01/08/2014.
        /// Cargar la información "detalles" del documento seleccionado.
        /// </summary>
        /// <param name="CodDocumento">Código del documento seleccionado.</param>
        /// <param name="NomDocumento">Nombre del documento seleccionado.</param>
        private void CargarInfo_Documento(String P_NomDocumento)
        {
            //Inicializar variables.
            DataTable RsDocumento = new DataTable();

            try
            {
                //Mostrar panel.
                pnl_datos.Visible = true;
                pnl_Grilla.Visible = false;

                txtSQL = " SELECT * FROM InterventorAnexos  WHERE codContacto=" + usuario.IdContacto + " AND NomDocumento = '" + P_NomDocumento + "'";

                RsDocumento = consultas.ObtenerDataTable(txtSQL, "text");

                lblTitulo.Text = "EDITAR DOCUMENTO";

                if (RsDocumento.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(RsDocumento.Rows[0]["nomdocumento"].ToString()))
                    { NomDocumento.Text = RsDocumento.Rows[0]["nomdocumento"].ToString(); }

                    //if (!String.IsNullOrEmpty(RsDocumento.Rows[0]["Extension"].ToString()) && RsDocumento.Rows[0]["Extension"].ToString() == "Link")
                    //{ Link.Visible = true; Link.Text = RsDocumento.Rows[0]["Extension"].ToString(); }

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
        /// Cerrar ventana emergente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_cerrar_Click(object sender, EventArgs e)
        { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); }

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
                Borrar(e.CommandArgument.ToString());
            }
            if (e.CommandName == "Descargar")
            {
                //Se tiene que validar a qué página vá a dirigirse, o puede ser "OfflineProcesaCarga.aspx" 
                //o se hace la descarga directa.
            }
            if (e.CommandName == "editar")
            {
                CargarInfo_Documento(e.CommandArgument.ToString());
            }
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
                if (btn_Accion.Text == "Adicionar")
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
    }
}