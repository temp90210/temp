using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.IO;
using Fonade.Controles;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Globalization;

namespace Fonade.FONADE.Proyecto
{
    public partial class ProyectoAnexos : Negocio.Base_Page
    {
        private string codProyecto;
        private string codConvocatoria;
        public int txtTab = Constantes.CONST_Anexos;
        String[] yyyys = { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sept", "Oct", "Nov", "Dic" };
        public Boolean esMiembro;
        /// <summary>
        /// Determina si está o no "realizado"...
        /// </summary>
        public Boolean bRealizado;
        /// <summary>
        /// Código del estado.
        /// </summary>
        public Int32 CodigoEstado;

        protected void Page_Load(object sender, EventArgs e)
        {
            ///Código anterior.
            //if (Session["codProyecto"].ToString() != string.Empty)
            //    codProyecto = Session["codProyecto"].ToString();

            #region Esta página NO usa la variable "CodConvocatoria".

            ///Código anterior al 15/09/2014.
            //if (Request.QueryString["codConvocatoria"] != null)
            //    codConvocatoria = Request.QueryString["codConvocatoria"].ToString();

            ///Código hecho el 15/09/2014, pero NO se usa porque la página no lo requiere.
            //if (Session["codConvocatoria"].ToString() != string.Empty)
            //    codConvocatoria = Session["codConvocatoria"].ToString(); 

            #endregion

            //Obtener los valores necesarios
            codProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            codConvocatoria = Session["CodConvocatoria"] != null && !string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Session["CodConvocatoria"].ToString() : "";

            inicioEncabezado(codProyecto, codConvocatoria, txtTab);

            //Consultar si es miembro.
            esMiembro = fnMiembroProyecto(usuario.IdContacto, codProyecto);

            //Consultar si está "realizado".
            bRealizado = esRealizado(txtTab.ToString(), codProyecto, codConvocatoria);

            //Consultar el "Estado" del proyecto.
            CodigoEstado = CodEstado_Proyecto(txtTab.ToString(), codProyecto, codConvocatoria);

            if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && CodigoEstado == Constantes.CONST_Inscripcion)
            { pnlAdicionarAnexos.Visible = true; }

            if (CodigoEstado == Constantes.CONST_Evaluacion)
            {
                //pnlDocumentosDeEvaluacion.Visible = true;
                tb_eval.Visible = true;
                if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor)
                { pnlAdicionarDocumentoEvaluacion.Visible = true; }
            }

            if (!IsPostBack)
            {
                CargarGridAnexos();
                CargarGridDocumentosEvaluacion();
                CargarGridDocumentosAcreditacion();
            }
        }

        protected void CargarGridAnexos()
        {
            string consulta = " select cast(id_documento as int) as Id_Documento, cast(nomdocumento as varchar(100)) as NombreDocumento, cast(fecha as datetime) as Fecha, cast(url as varchar(100)) as URL, cast(nomdocumentoformato as varchar(100)) as NombreDocumentoFormato, case icono when NULL then 'IcoDocNormal.gif' else  cast(icono as varchar(100)) end as Icono, cast(CodDocumentoFormato as int) as CodigoDocumentoFormato, cast(nomtab as varchar(100))  as NomTab";
            consulta += " from  documentoformato, tab  RIGHT OUTER JOIN documento d on id_tab=d.codtab ";
            consulta += " where id_documentoformato=coddocumentoformato ";
            consulta += " and codestado={0} and borrado=0 and codproyecto ={1}  ";
            consulta += " and CodDocumentoFormato <> 19 and CodDocumentoFormato <>17 order by nomdocumento ";

            IEnumerable<BORespuestaAnexos> respuesta = consultas.Db.ExecuteQuery<BORespuestaAnexos>(consulta, Constantes.CONST_Inscripcion, Convert.ToInt32(codProyecto));

            DataTable datos = new DataTable();
            datos.Columns.Add("CodProyecto");
            datos.Columns.Add("Id_Documento");
            datos.Columns.Add("URL");
            datos.Columns.Add("icono");
            datos.Columns.Add("nombre");
            datos.Columns.Add("tab");
            datos.Columns.Add("fecha");

            foreach (BORespuestaAnexos item in respuesta)
            {
                DataRow dr = datos.NewRow();
                dr["CodProyecto"] = codProyecto;
                dr["Id_Documento"] = item.Id_Documento;
                dr["URL"] = item.URL;
                dr["icono"] = item.Icono;
                dr["nombre"] = item.NombreDocumento;
                dr["tab"] = item.NomTab;
                //dr["fecha"] = string.Format("{0: MMM d} de {1: yyyy HH:mm:ss tt}", item.Fecha, item.Fecha);
                dr["fecha"] = item.Fecha.ToString(yyyys[item.Fecha.Month] + " d 'de' yyyy hh:mm:ss tt", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                #region Re-ajustar la fecha "debido a que NO se queda establecido correctamente el formato de la fecha.

                if (dr["fecha"].ToString().Contains("a. m."))
                { dr["fecha"] = dr["fecha"].ToString().Replace("a. m.", "a.m."); }
                if (dr["fecha"].ToString().Contains("p. m."))
                { dr["fecha"] = dr["fecha"].ToString().Replace("p. m.", "p.m."); }

                if (dr["fecha"].ToString().Contains("1ne"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("1ne", "Ene"); }
                if (dr["fecha"].ToString().Contains("2eb"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("2eb", "Feb"); }
                if (dr["fecha"].ToString().Contains("3ar"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("3ar", "Mar"); }
                if (dr["fecha"].ToString().Contains("4br"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("4br", "Abr"); }
                if (dr["fecha"].ToString().Contains("5ay"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("5ay", "May"); }
                if (dr["fecha"].ToString().Contains("6un"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("6un", "Jun"); }
                if (dr["fecha"].ToString().Contains("7ul"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("7ul", "Jul"); }
                if (dr["fecha"].ToString().Contains("8go"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("8go", "Ago"); }
                if (dr["fecha"].ToString().Contains("9ep"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("9ep", "Sep"); }
                if (dr["fecha"].ToString().Contains("10ct"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("10ct", "Oct"); }
                if (dr["fecha"].ToString().Contains("11ov"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("11ov", "Nov"); }
                if (dr["fecha"].ToString().Contains("12ic"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("12ic", "Dic"); }


                #endregion

                datos.Rows.Add(dr);
            }

            gw_Anexos.DataSource = datos;
            gw_Anexos.DataBind();

            for (int i = 0; i < gw_Anexos.Rows.Count; i++)
            {
                if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && CodigoEstado == Constantes.CONST_Inscripcion)
                {
                    ((ImageButton)gw_Anexos.Rows[i].Cells[0].FindControl("btn_Borrar")).Visible = true;
                    ((LinkButton)gw_Anexos.Rows[i].Cells[2].FindControl("btnEditar")).Visible = true;
                    ((Label)gw_Anexos.Rows[i].Cells[2].FindControl("lblEditar")).Visible = false;
                }
            }
        }

        protected void CargarGridDocumentosEvaluacion()
        {
            string consulta = " select cast(id_documento as int) as Id_Documento, cast(nomdocumento as varchar(100)) as NombreDocumento, cast(fecha as datetime) as Fecha, cast(url as varchar(100)) as URL, cast(nomdocumentoformato as varchar(100)) as NombreDocumentoFormato, case icono when NULL then 'IcoDocNormal.gif' else  cast(icono as varchar(100)) end as Icono, cast(CodDocumentoFormato as int) as CodigoDocumentoFormato, cast(nomtab as varchar(100))  as NomTab ";
            consulta += " from  documentoformato, tab RIGHT OUTER JOIN documento d on id_tab=d.codtab ";
            consulta += " where id_documentoformato=coddocumentoformato  and codestado={0} and borrado=0 ";
            consulta += " and codproyecto ={1} and CodDocumentoFormato <> 19 and CodDocumentoFormato <>17 order by nomdocumento ";

            IEnumerable<BORespuestaAnexos> respuesta = consultas.Db.ExecuteQuery<BORespuestaAnexos>(consulta, Constantes.CONST_Evaluacion, Convert.ToInt32(codProyecto));

            DataTable datos = new DataTable();
            datos.Columns.Add("CodProyecto");
            datos.Columns.Add("Id_Documento");
            datos.Columns.Add("URL");
            datos.Columns.Add("icono");
            datos.Columns.Add("nombre");
            datos.Columns.Add("tab");
            datos.Columns.Add("fecha");

            foreach (BORespuestaAnexos item in respuesta)
            {
                DataRow dr = datos.NewRow();
                dr["CodProyecto"] = codProyecto;
                dr["Id_Documento"] = item.Id_Documento;
                dr["URL"] = item.URL;
                dr["icono"] = item.Icono;
                dr["nombre"] = item.NombreDocumento;
                dr["tab"] = item.NomTab;
                //dr["fecha"] = string.Format("{0: MMM d} de {1: yyyy HH:mm:ss tt}", item.Fecha, item.Fecha);
                dr["fecha"] = item.Fecha.ToString(yyyys[item.Fecha.Month] + " d 'de' yyyy hh:mm:ss tt", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                #region Re-ajustar la fecha "debido a que NO se queda establecido correctamente el formato de la fecha.

                if (dr["fecha"].ToString().Contains("a. m."))
                { dr["fecha"] = dr["fecha"].ToString().Replace("a. m.", "a.m."); }
                if (dr["fecha"].ToString().Contains("p. m."))
                { dr["fecha"] = dr["fecha"].ToString().Replace("p. m.", "p.m."); }

                if (dr["fecha"].ToString().Contains("1ne"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("1ne", "Ene"); }
                if (dr["fecha"].ToString().Contains("2eb"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("2eb", "Feb"); }
                if (dr["fecha"].ToString().Contains("3ar"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("3ar", "Mar"); }
                if (dr["fecha"].ToString().Contains("4br"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("4br", "Abr"); }
                if (dr["fecha"].ToString().Contains("5ay"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("5ay", "May"); }
                if (dr["fecha"].ToString().Contains("6un"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("6un", "Jun"); }
                if (dr["fecha"].ToString().Contains("7ul"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("7ul", "Jul"); }
                if (dr["fecha"].ToString().Contains("8go"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("8go", "Ago"); }
                if (dr["fecha"].ToString().Contains("9ep"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("9ep", "Sep"); }
                if (dr["fecha"].ToString().Contains("10ct"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("10ct", "Oct"); }
                if (dr["fecha"].ToString().Contains("11ov"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("11ov", "Nov"); }
                if (dr["fecha"].ToString().Contains("12ic"))
                { dr["fecha"] = dr["fecha"].ToString().Replace("12ic", "Dic"); }


                #endregion
                datos.Rows.Add(dr);
            }

            gw_DocumentosEvaluacion.DataSource = datos;
            gw_DocumentosEvaluacion.DataBind();

            for (int i = 0; i < gw_DocumentosEvaluacion.Rows.Count; i++)
            {
                if (esMiembro && usuario.CodGrupo == Constantes.CONST_Emprendedor && CodigoEstado == Constantes.CONST_Evaluacion)
                {
                    ((ImageButton)gw_DocumentosEvaluacion.Rows[i].Cells[0].FindControl("btn_Borrar")).Visible = true;
                    ((LinkButton)gw_DocumentosEvaluacion.Rows[i].Cells[2].FindControl("btnEditar")).Visible = true;
                    ((Label)gw_DocumentosEvaluacion.Rows[i].Cells[2].FindControl("lblEditar")).Visible = false;
                }
            }
        }

        protected void CargarGridDocumentosAcreditacion()
        {

            string consulta = " SELECT  cast(a.Ruta as varchar(100)) as Ruta, cast(e.TituloObtenido as varchar(100)) as TituloObtenido, cast(es.NomNivelEstudio as varchar(100)) as NomNivelEstudio, cast(c.Nombres as varchar(100)) as Nombres, cast(c.Apellidos as varchar(100)) as Apellidos, cast(T1.Texto as varchar(100)) as TipoArchivo, cast(T2.Texto as varchar(100)) as TipoArchivoDescripcion ";
            consulta += " , cast((case when a.CodContactoEstudio is null then '' else  e.TituloObtenido + ' (' +  es.NomNivelEstudio + ')' end) as varchar(100) ) as Descripcion  ";
            consulta += " , cast((isnull(e.anotitulo,datepart(year,getdate())) ) as varchar(10)) as ano_titulo ";
            consulta += " FROM ContactoArchivosAnexos AS a ";
            consulta += " LEFT OUTER JOIN Contacto c ON c.Id_Contacto = a.CodContacto ";
            consulta += " LEFT OUTER JOIN ContactoEstudio AS e  on e.Id_ContactoEstudio = a.CodContactoEstudio";
            consulta += " LEFT OUTER JOIN NivelEstudio AS es ON e.CodNivelEstudio = es.Id_NivelEstudio ";
            consulta += " LEFT OUTER JOIN texto AS T1 ON T1.NomTexto=A.TipoArchivo ";
            consulta += " LEFT OUTER JOIN texto AS T2 ON T2.NomTexto=CONCAT(A.TipoArchivo,'_desc')  ";
            consulta += " WHERE a.CodProyecto = {0}";
            consulta += " ORDER BY a.TipoArchivo, c.Id_Contacto, ano_titulo Desc";

            IEnumerable<BORespuestaDocumentosAcreditacion> respuesta = consultas.Db.ExecuteQuery<BORespuestaDocumentosAcreditacion>(consulta, Convert.ToInt32(codProyecto));

            DataTable datos = new DataTable();
            datos.Columns.Add("CodProyecto");
            datos.Columns.Add("Id_Documento");
            datos.Columns.Add("URL");
            datos.Columns.Add("icono");
            datos.Columns.Add("tipo");
            datos.Columns.Add("nombre");
            datos.Columns.Add("descripcion");

            foreach (BORespuestaDocumentosAcreditacion item in respuesta)
            {

                DataRow dr = datos.NewRow();
                dr["CodProyecto"] = codProyecto;
                dr["URL"] = item.Ruta;
                dr["tipo"] = item.TipoArchivo;
                dr["nombre"] = item.Nombres + " " + item.Apellidos;
                if (item.Descripcion != "")
                    dr["descripcion"] = item.TipoArchivo + " - " + item.Descripcion;
                else
                    dr["descripcion"] = item.TipoArchivo + " - " + item.TipoArchivoDescripcion;

                datos.Rows.Add(dr);
            }

            gw_DocumentosAcreditacion.DataSource = datos;
            gw_DocumentosAcreditacion.DataBind();
        }

        private Documento getDocumentoActual(string idDocumento)
        {
            var query = (from p in consultas.Db.Documentos
                         where p.Id_Documento == Convert.ToInt32(idDocumento)
                         select p).First();

            return query;

        }

        protected void gw_Anexos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            AccionGrid(e.CommandName.ToString(), e.CommandArgument.ToString());
        }

        protected void gw_DocumentosEvaluacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            AccionGrid(e.CommandName.ToString(), e.CommandArgument.ToString());
        }

        protected void gw_DocumentosAcreditacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            AccionGrid(e.CommandName.ToString(), e.CommandArgument.ToString());
        }

        protected void AccionGrid(string accion, string argumento)
        {
            string Id_Documento = "0";

            switch (accion)
            {
                case "VerDocumento":

                    string url = argumento;
                    string rutaFisica = url.Replace("Documentos/Proyecto/", ConfigurationManager.AppSettings.Get("RutaDocumentosProyecto"));
                    if (File.Exists(rutaFisica))
                    {
                        url = rutaFisica;
                    }
                    DescargarArchivo(url);
                    break;
                case "Editar":
                    Id_Documento = argumento;
                    CarcarFormularioEdicion(Id_Documento);
                    break;
                case "Borrar":
                    Id_Documento = argumento;

                    Documento datoActual = getDocumentoActual(Id_Documento);
                    datoActual.Borrado = true;

                    consultas.Db.SubmitChanges();
                    CargarGridAnexos();
                    break;
            }
        }

        protected void btnAdicionarInversion_Click(object sender, EventArgs e)
        {
            pnlPrincipal.Visible = false;
            pnlCrearDocumento.Visible = true;
            btnCrearAnexo.Text = "Crear";
            txtNombreDocumento.Text = "";
            txtLink.Text = "";
            txtComentario.Text = "";
        }

        private void CarcarFormularioEdicion(string idDocumento)
        {
            pnlPrincipal.Visible = false;
            pnlCrearDocumento.Visible = true;

            txtNombreDocumento.Text = "";
            txtLink.Text = "";
            txtComentario.Text = "";

            var query = (from d in consultas.Db.Documentos
                         from pf in consultas.Db.DocumentoFormatos
                         where d.CodDocumentoFormato == pf.Id_DocumentoFormato &&
                         d.Id_Documento == Convert.ToInt32(idDocumento)
                         select new { d.NomDocumento, d.URL, d.Comentario, pf.Extension }
                             ).First();

            txtNombreDocumento.Text = query.NomDocumento;
            tdLink.Visible = false;
            if (query.Extension.Trim().ToLower() == "link")
            {
                txtLink.Text = query.URL;
                tdLink.Visible = true;
            }
            hddIdDocumento.Value = idDocumento;
            tdSubir.Visible = false;
            txtComentario.Text = query.Comentario;
            btnCrearAnexo.Text = "Actualizar";
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/05/2014.
        /// Limitado por tamaño la carga de archivos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCrearAnexo_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            ClientScriptManager cm = this.ClientScript;

            if (!Archivo.HasFile)
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('No ha subido ningún archivo.');</script>");
                return;
            }
            else
            {
                if (Archivo.PostedFile.ContentLength > 10485760) // = 10MB
                {
                    cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El tamaño del archivo debe ser menor a 10 Mb.');</script>");
                    return;
                }
                else
                {
                    #region Procesar la información adjunta al archivo seleccionado.
                    if (hddIdDocumento.Value == "")
                    {
                        string txtFormato = "Link";
                        string codFormato = "";
                        string NomFormato = "";
                        string RutaHttpDestino = "";

                        if (txtLink.Text == "")
                        {
                            string[] extencion = Archivo.PostedFile.FileName.ToString().Trim().Split('.');
                            txtFormato = "." + extencion[extencion.Length - 1];
                        }
                        if (txtFormato.ToLower() == ".asp" || txtFormato.ToLower() == "php" || txtFormato.ToLower() == "xml" || txtFormato.ToLower() == "aspx" || txtFormato.ToLower() == "exe")
                        {
                            lblMensajeError.Text = "El Archivo que intenta adjuntar no es permitido";
                        }

                        try
                        {
                            var query = (from df in consultas.Db.DocumentoFormatos
                                         where df.Extension == txtFormato
                                         select new { df.Id_DocumentoFormato, df.NomDocumentoFormato }).FirstOrDefault();

                            codFormato = query.Id_DocumentoFormato.ToString(); ;
                            NomFormato = query.NomDocumentoFormato;
                        }
                        catch
                        {

                            DocumentoFormato datos = new DocumentoFormato();
                            datos.NomDocumentoFormato = "Archivo " + txtFormato;
                            datos.Extension = txtFormato;
                            datos.Icono = "IcoDocNormal.gif";

                            consultas.Db.DocumentoFormatos.InsertOnSubmit(datos);
                            consultas.Db.SubmitChanges();

                            var query = (from df in consultas.Db.DocumentoFormatos
                                         where df.Extension == txtFormato
                                         select new { df.Id_DocumentoFormato, df.NomDocumentoFormato }).FirstOrDefault();

                            codFormato = query.Id_DocumentoFormato.ToString(); ;
                            NomFormato = query.NomDocumentoFormato;
                        }
                        if (txtLink.Text == "")
                        {

                            string ruta = ConfigurationManager.AppSettings.Get("RutaDocumentosProyecto") + Math.Abs(Convert.ToInt32(codProyecto) / 2000) + @"\Proyecto_" + codProyecto + @"\";

                            if (!CargarArchivoServidor(Archivo, ruta, Archivo.FileName.ToString().Substring(0, Archivo.FileName.ToString().IndexOf('.')), txtFormato.Substring(1), "RutaDocumentosTEMP"))
                            {
                                lblMensajeError.Text = respuesta.Mensaje;
                                return;
                            }

                            RutaHttpDestino = "Documentos/Proyecto/" + Math.Abs(Convert.ToInt32(codProyecto) / 2000) + "/" + @"\Proyecto_" + codProyecto + @"\" + Archivo.FileName.ToString();
                        }
                        else
                        {
                            if (txtLink.Text.Contains("http://") == false)
                            {
                                txtLink.Text = "http://" + txtLink.Text;
                            }
                            RutaHttpDestino = txtLink.Text;
                        }


                        Documento datosNuevoDocumento = new Documento();
                        datosNuevoDocumento.CodProyecto = Convert.ToInt32(codProyecto);
                        datosNuevoDocumento.NomDocumento = txtNombreDocumento.Text;
                        datosNuevoDocumento.CodTab = (short?)txtTab;
                        datosNuevoDocumento.CodDocumentoFormato = Convert.ToByte(codFormato);
                        datosNuevoDocumento.URL = RutaHttpDestino;
                        datosNuevoDocumento.Comentario = txtComentario.Text;
                        datosNuevoDocumento.Fecha = DateTime.Now;
                        datosNuevoDocumento.CodContacto = usuario.IdContacto;
                        datosNuevoDocumento.CodEstado = (byte?)codEstado;

                        consultas.Db.Documentos.InsertOnSubmit(datosNuevoDocumento);

                    }
                    else
                    {
                        Documento datoActual = getDocumentoActual(hddIdDocumento.Value);
                        datoActual.NomDocumento = txtNombreDocumento.Text;
                        datoActual.Comentario = txtComentario.Text;
                        datoActual.CodTab = (short?)txtTab;
                        if (txtLink.Text != "")
                            datoActual.URL = txtLink.Text;

                        hddIdDocumento.Value = "";
                    }
                    consultas.Db.SubmitChanges();
                    CargarGridAnexos();

                    pnlPrincipal.Visible = true;
                    pnlCrearDocumento.Visible = false;
                    #endregion
                }
            }
        }

        protected void btnCerrarAnexo_Click(object sender, EventArgs e)
        {
            pnlPrincipal.Visible = true;
            pnlCrearDocumento.Visible = false;
            hddIdDocumento.Value = "";
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

        protected void Image2_Click(object sender, ImageClickEventArgs e)
        {
            pnlPrincipal.Visible = false;
            pnlCrearDocumento.Visible = true;
            btnCrearAnexo.Text = "Crear";
            txtNombreDocumento.Text = "";
            txtLink.Text = "";
            txtComentario.Text = "";
        }
    }

    public class BORespuestaAnexos
    {
        public int Id_Documento { get; set; }
        public string NombreDocumento { get; set; }
        public DateTime Fecha { get; set; }
        public string URL { get; set; }
        public string NombreDocumentoFormato { get; set; }
        public string Icono { get; set; }
        public int CodigoDocumentoFormato { get; set; }
        public string NomTab { get; set; }
    }

    public class BORespuestaDocumentosAcreditacion
    {

        public string Ruta { get; set; }
        public string TituloObtenido { get; set; }
        public string NomNivelEstudio { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoArchivo { get; set; }
        public string TipoArchivoDescripcion { get; set; }
        public string Descripcion { get; set; }
        public string ano_titulo { get; set; }
    }
}
