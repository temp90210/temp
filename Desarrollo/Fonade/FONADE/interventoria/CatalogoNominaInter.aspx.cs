using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Datos;
using Fonade.Negocio;
using System.IO;

namespace Fonade.FONADE.interventoria
{
    public partial class CatalogoNominaInter : Negocio.Base_Page
    {
        String CodNomina;
        int Mes;
        String txtSQL;

        /// <summary>
        /// Si este valor contiene datos, significa que se generará un nuevo documento.
        /// </summary>
        String Accion_Docs;

        #region Variables que usa FONADE clásico.

        String CodProyecto;
        String NombreArchivo;
        String RutaHttpDestino;
        String RutaDestino;
        String RutaDestinoTemp;
        String ArchivoSubido;
        String txtFormato;
        String RsFormato;
        String CodFormato;
        String NomFormato;
        String FSObject;
        String Carpeta;
        String SubCarpetas;
        String NuevaCarpeta;
        String txtNomMedio;
        String txtCarpeta;
        String txtDescripcion;
        String numTimeOut;
        String txtTab;
        String txtLink;

        //Variables para upload de interventoría
        String CodConvocatoria;
        String CodActa;
        String txtNomActa;
        String txtNumActa;
        String txtFechaActa;
        String CodInterventoria;
        String mes;
        String TipoInterventor;

        Int64 TamanoArchivo;
        String FormatoArchivo;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CodNomina = Session["CodNomina"] != null && !string.IsNullOrEmpty(Session["CodNomina"].ToString()) ? Session["CodNomina"].ToString() : "0";
                Mes = (int)
                (!string.IsNullOrEmpty(Session["MesDeLaNomina"].ToString())
                     ? Convert.ToInt64(Session["MesDeLaNomina"])
                     : 0);
                Accion_Docs = Session["Accion_Docs"] != null && !string.IsNullOrEmpty(Session["Accion_Docs"].ToString()) ? Session["Accion_Docs"].ToString() : "";

                if (Accion_Docs != "") { pnlPrincipal.Visible = false; pnl_NuevoDoc.Visible = true; CargarTiposDeDocumento(); }

                ValidarGrupo();
            }
        }

        private void ValidarGrupo()
        {
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
            }

            try
            {
                consultas.Parameters = null;

                consultas.Parameters = new[]
                                           {
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@CodCargo",
                                                       Value = CodNomina
                                                   },
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@Mes",
                                                       Value = Mes
                                                   }
                                           };

                var dtDocumentos = consultas.ObtenerDataTable("MD_Consultar_Documento_Nomina");

                if (dtDocumentos.Rows.Count != 0)
                {
                    Session["dtDocumentos"] = dtDocumentos;
                    GrvDocumentos.DataSource = dtDocumentos;
                    GrvDocumentos.DataBind();
                }
                else
                {
                    GrvDocumentos.DataSource = dtDocumentos;
                    GrvDocumentos.DataBind();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            var sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;

                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        protected void GrvDocumentosPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvDocumentos.PageIndex = e.NewPageIndex;
            GrvDocumentos.DataSource = Session["dtDocumentos"];
            GrvDocumentos.DataBind();
        }

        protected void GrvDocumentosSorting(object sender, GridViewSortEventArgs e)
        {
            var dt = Session["dtEmpresas"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GrvDocumentos.DataSource = Session["dtDocumentos"];
                GrvDocumentos.DataBind();
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Redirect(null, "../evaluacion/CatalogoNominaPOInterventoria.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }

        /// <summary>
        /// REVISAR ARCHIVO "UploadInterventoria", "Class_Uploader" y archivos relacionados,
        /// </summary>
        /// <returns></returns>
        private string Validar()
        {
            string msg = "";

            try
            {
                #region Obtener el nombre válido del archivo.

                switch (NombreArchivo)
                {
                    case "%":
                        NombreArchivo = NombreArchivo.Replace("%", "");
                        break;
                    case "'":
                        NombreArchivo = NombreArchivo.Replace("'", "");
                        break;
                    case "&":
                        NombreArchivo = NombreArchivo.Replace("&", "");
                        break;
                    case "?":
                        NombreArchivo = NombreArchivo.Replace("?", "");
                        break;
                    case "#":
                        NombreArchivo = NombreArchivo.Replace("#", "");
                        break;
                    default:
                        break;
                }

                #endregion

                //Si el archivo existe...
                if (File.Exists("path"))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El archivo que intenta subir ya existe, deberá cambiarle el nombre y volver a intentarlo.')", true);
                }

                if (NomDocumento.Text.Trim().Length > 80)
                {
                    //Exceso de caracteres del nombre del documento.
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El archivo que intenta subir posee un nombre demasiado extenso, deberá cambiarle el nombre por uno mas corto.')", true);
                }

                switch (FormatoArchivo)
                {
                    case "jpg":
                    case "bmp":
                    case "gif":
                    case "tif":
                    case "png":
                        if (TamanoArchivo > 10000000)
                        { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('la imagen " + FormatoArchivo + " que intenta subir tiene un tamaño de " + TamanoArchivo + "bytes es muy pesada, debera optimizarla o convertirla a otro formato como jpg de menor tamaño.')", true); }
                        break;
                    default:
                        break;
                }

                return msg;
            }
            catch (Exception ex)
            { msg = "Error: " + ex.Message; return msg; }
        }

        /// <summary>
        /// Crear el nuevo documento...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch { }
        }

        /// <summary>
        /// Cargar DropDownList de tipos de documento.
        /// </summary>
        private void CargarTiposDeDocumento()
        {
            try
            {
                txtSQL = "SELECT * FROM TipoDocumento";
                var RS = consultas.ObtenerDataTable(txtSQL, "text");

                dd_TipoInterventor.Items.Clear();

                foreach (DataRow row in RS.Rows)
                {
                    ListItem item = new ListItem();
                    item.Text = row["Id_TipoDocumento"].ToString();
                    item.Value = row["NomTipoDocumento"].ToString();
                    dd_TipoInterventor.Items.Add(item);
                }
            }
            catch { }
        }
    }
}