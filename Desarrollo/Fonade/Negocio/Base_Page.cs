using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fonade.Account;
using Datos;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;
using LinqKit;
using System.Data.Linq.SqlClient;
using System.Configuration;
using Fonade.Controles;
using System.IO;
using Fonade.FONADE.Proyecto.Templates;
using TSHAK.Components;
using System.Data.SqlClient;
using System.Data;
//using CAPICOM;
using System.Runtime.InteropServices;

namespace Fonade.Negocio
{
    public class Base_Page : System.Web.UI.Page
    {
        protected FonadeUser usuario;
        protected Consultas consultas;
        protected bool miembro = false;
        protected bool redirect = false;
        protected bool realizado = false;
        protected int codEstado;
        protected RespuestaCargue respuesta;
        protected string void_establecerTitulo(string[] codgrupo, string Accion, string defaulttitle)
        {
            string temp_title = "";
            string title = "";
            if (codgrupo.Contains(Constantes.CONST_Perfil_Fiduciario.ToString())) temp_title = "USUARIO FIDUCIARIA";
            else if (codgrupo.Contains(Constantes.CONST_CallCenter.ToString())) temp_title = "USUARIO CALL CENTER";
            else if (codgrupo.Contains(Constantes.CONST_GerenteEvaluador.ToString())) temp_title = "USUARIO GERENTE EVALUADOR";
            else if (codgrupo.Contains(Constantes.CONST_GerenteInterventor.ToString())) temp_title = "USUARIO GERENTE INTERVENTOR";
            else if (codgrupo.Contains(Constantes.CONST_AdministradorFonade.ToString()) ||
                codgrupo.Contains(Constantes.CONST_AdministradorSena.ToString())) temp_title = "USUARIO ADMINISTRADOR";
            else
            {
                temp_title = defaulttitle;
            }

            switch (Accion)
            {
                case "Editar":
                    title = "EDITAR " + temp_title;
                    break;
                case "Crear":
                    title = "NUEVO " + temp_title;
                    break;
                case "":
                    title = temp_title;
                    break;
                default: title = temp_title; break;

            }
            return title;
        }
        protected string void_establecerTitulo(string defaulttitle)
        {

            string title = "";
            title = defaulttitle;
            return title;
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                usuario = (FonadeUser)Membership.GetUser(User.Identity.Name, true);
                consultas = new Consultas();
            }
            catch
            {
                throw new Exception("the user session doesnt exist");
            };
            base.OnLoad(e);
        }

        public Base_Page()
        {
            consultas = new Consultas();
            usuario = (FonadeUser)Membership.GetUser(User.Identity.Name, true);
        }

        protected void procesarCampo(ref TextBox txtBox, ref HtmlEditorExtender htmlExtender, ref HtmlGenericControl panel, string txtValue, bool Miembro, bool bRealizado, string codConvocatoria)
        {
            txtBox.Text = txtValue;
            if (Miembro == true && usuario.CodGrupo == Constantes.CONST_Emprendedor && !bRealizado && codConvocatoria == "")
            {
                //          txtDisabled = "";
                panel.Visible = false;
            }
            else
            {
                if (Miembro == true && usuario.CodGrupo == Constantes.CONST_Evaluador && !bRealizado && codConvocatoria != "")
                {
                    panel.Visible = false;
                    //txtDisabled = ""		;
                }

                else
                {
                    panel.Visible = true;
                    panel.InnerHtml = txtValue;
                    txtBox.Visible = false;
                }
            }
        }

        protected void inicioEncabezado(string codProyecto, string codConvocatoria, int txtTab)
        {

            if (usuario.CodGrupo == Constantes.CONST_Asesor
               || usuario.CodGrupo == Constantes.CONST_Emprendedor
               || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador
               || usuario.CodGrupo == Constantes.CONST_Evaluador)
            {
                if (!fnMiembroProyecto(usuario.IdContacto, codProyecto))
                {
                    redirect = true;
                    miembro = false;
                }
                else
                {
                    redirect = false;
                    miembro = true;
                }
            }


            //Validar si el proyecto es de la unidad 
            if (usuario.CodGrupo == Constantes.CONST_JefeUnidad)
            {
                int codInstitucion = consultas.Db.Proyectos.Where(
                t => t.Id_Proyecto == Convert.ToInt32(codProyecto)).FirstOrDefault().CodInstitucion;

                if (codInstitucion != 0)
                {
                    if (usuario.CodInstitucion != codInstitucion)
                    {
                        redirect = true;
                    }
                }
            }


            if (txtTab != null)
            {
                //'Hallar numero de post it por tab
                var query = from tur in consultas.Db.TareaUsuarioRepeticions
                            from tu in consultas.Db.TareaUsuarios
                            from tp in consultas.Db.TareaProgramas
                            where tp.Id_TareaPrograma == tu.CodTareaPrograma
                            && tu.Id_TareaUsuario == tur.CodTareaUsuario
                            && tu.CodProyecto == Convert.ToInt32(codProyecto)
                            && tp.Id_TareaPrograma == Constantes.CONST_PostIt
                            && tur.FechaCierre == null
                            select tur;

                var predicate = PredicateBuilder.False<Datos.TareaUsuarioRepeticion>();

                if (codConvocatoria == "")
                {
                    predicate.And(t => SqlMethods.Like(t.Parametros, "%tab=" + txtTab + "%"));
                }
                else
                {
                    predicate.And(t => SqlMethods.Like(t.Parametros, "%tabEval=" + txtTab + "%"));
                }

                int numPostIt = query.Count();

                //'Estado de proyecto y del tab

                if (codConvocatoria == "")
                {
                    var sql = from p in consultas.Db.Proyectos
                              join t in consultas.Db.TabProyectos on p.Id_Proyecto equals t.CodProyecto into ps
                              from t in ps.DefaultIfEmpty()
                              where t.CodTab == txtTab && p.Id_Proyecto == Convert.ToInt32(codProyecto)
                              select new { codEstado = p.CodEstado, realizado = t == null ? false : t.Realizado };
                    foreach (var obj in sql)
                    {
                        if (obj.realizado != null)
                            realizado = obj.realizado;
                        codEstado = obj.codEstado;
                        break;
                    }
                }
                else
                {
                    var sql1 = from p in consultas.Db.Proyectos
                               join t in consultas.Db.TabEvaluacionProyectos on p.Id_Proyecto equals t.CodProyecto into ps
                               from t in ps.DefaultIfEmpty()
                               where t.CodTabEvaluacion == txtTab && p.Id_Proyecto == Convert.ToInt32(codProyecto)
                               && t.CodConvocatoria == Convert.ToInt32(codConvocatoria)
                               select new { codEstado = p.CodEstado, realizado = t.Realizado };
                    foreach (var obj in sql1)
                    {
                        if (obj.realizado != null)
                            realizado = obj.realizado;
                        codEstado = obj.codEstado;
                        break;
                    }
                }

            }

            //Para las páginas de evaluación validar que tiene acceso
            if (codConvocatoria != "")
            {
                if (usuario.CodGrupo != Constantes.CONST_CoordinadorEvaluador
                    && usuario.CodGrupo != Constantes.CONST_Evaluador
                    && usuario.CodGrupo != Constantes.CONST_GerenteEvaluador)
                {
                    redirect = true;
                }
            }
            else
            {
                //    'Validar el acceso del gerente evaluador cuando el proyecto esta en estado de evaluacion
                if (codEstado < Constantes.CONST_Convocatoria
                    && usuario.CodGrupo == Constantes.CONST_GerenteEvaluador)
                {
                    redirect = true;
                }
            }
        }

        protected void construirEncabezado()
        {

        }

        /// <summary>
        /// determina si el usuario con el id de contacto suministrado es miembro del proyecto
        /// </summary>
        /// <param name="codProyecto"></param>
        /// <param name="idContacto"></param>
        /// <returns></returns>
        protected bool fnMiembroProyecto(int idContacto, string codProyecto)
        {
            //Inicializar variables.
            String txtSQL = "";

            try
            {
                var query = (from pc in consultas.Db.ProyectoContactos
                             where pc.CodProyecto == Convert.ToInt32(codProyecto)
                             && pc.CodContacto == idContacto && pc.Inactivo == false
                             && pc.FechaInicio < DateTime.Now && pc.FechaFin == null
                             //select pc;
                             select new { pc.CodContacto, pc.CodRol }).FirstOrDefault();

                Session["CodRol"] = "";

                //if (query.Count() > 0)
                if (query.CodContacto > 0)
                { Session["CodRol"] = query.CodRol; return true; }
                else { return false; }
            }
            catch { return false; }
        }

        /// <summary>
        /// Metodo usado para descargar cualquier tipo de archivo
        /// </summary>
        /// <param name="path">Ruta Fisica o virtual del archivo (Fisica Eje: c:\\directorio\documento.pdf),(Virtual Eje: ~\Directorio\documento.pdf)  </param>
        /// <param name="rutaFisica">Dato Booleano que identifica si la ruta sera fisica o virtual</param>
        protected void DescargarArchivo(string path, bool rutaFisica = true)
        {
            System.IO.FileInfo toDownload;
            if (rutaFisica)
            {
                toDownload = new System.IO.FileInfo(path);
            }
            else
            {
                toDownload = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(path));
            }

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + toDownload.Name);
            HttpContext.Current.Response.AddHeader("Content-Length",
                       toDownload.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.WriteFile(path);
            HttpContext.Current.Response.End();
        }

        protected bool CargarArchivoServidor(FileUpload archivoCarga, string pathDestino, string nombreDocumento, string extencion, string keyrutaTemp)
        {
            respuesta = new RespuestaCargue();
            respuesta.Extencion = extencion;
            if (CargarTemporal(archivoCarga, pathDestino, nombreDocumento, keyrutaTemp))
            {
                if (CargarDocumentoFinal(pathDestino, nombreDocumento, keyrutaTemp))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        protected bool CargarTemporal(FileUpload archivoCarga, string pathDestino, string nombreDocumento, string keyRutaTemp)
        {
            string pathDestinoTEMP = ConfigurationManager.AppSettings.Get(keyRutaTemp);
            pathDestinoTEMP = pathDestinoTEMP + pathDestino.Substring(2);

            if (File.Exists(pathDestinoTEMP) == false)
            {
                try
                {
                    System.IO.Directory.CreateDirectory(pathDestinoTEMP);
                    try
                    {
                        archivoCarga.PostedFile.SaveAs(pathDestinoTEMP + nombreDocumento + "." + respuesta.Extencion);
                        respuesta.PathTemporal = pathDestinoTEMP + nombreDocumento + "." + respuesta.Extencion;
                        return true;
                    }
                    catch
                    {
                        respuesta.Mensaje = "Error No se pudo subir el documento a la carpeta TMP: ";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    respuesta.Mensaje = "Error No se pudo crear la carpeta TMP: " + pathDestinoTEMP + ex.Message + ex.StackTrace;
                    return false;
                }
            }
            return true;
        }

        protected bool CargarDocumentoFinal(string pathDestino, string nombreDocumento, string keyRutaTemp)
        {
            string pathDestinoTEMP = ConfigurationManager.AppSettings.Get(keyRutaTemp);
            pathDestinoTEMP = pathDestinoTEMP + pathDestino.Substring(2);


            if (File.Exists(pathDestino) == false)
            {
                try
                {
                    System.IO.Directory.CreateDirectory(pathDestino);
                }
                catch
                {
                    respuesta.Mensaje = "Error No se pudo crear la carpeta: " + pathDestino;
                    return false;
                }
            }

            try
            {
                byte[] archivoPlano = File.ReadAllBytes(pathDestinoTEMP + nombreDocumento + "." + respuesta.Extencion);
                try
                {
                    File.WriteAllBytes(pathDestino + nombreDocumento + "." + respuesta.Extencion, archivoPlano);
                    respuesta.Mensaje = "OK";
                    respuesta.PathFisico = pathDestino + nombreDocumento + "." + respuesta.Extencion;
                    File.Delete(pathDestinoTEMP + nombreDocumento + "." + respuesta.Extencion);

                    return true;
                }
                catch
                {
                    respuesta.Mensaje = "Error al mover el archivo temporal a la ruta final: " + pathDestino; ;
                    respuesta.PathFisico = pathDestino + nombreDocumento + "." + respuesta.Extencion;
                    return false;
                }
            }
            catch
            {
                respuesta.Mensaje = "Error No se pudo crear la carpeta: " + pathDestino;
                return false;
            }
        }

        protected void PintarFilasGrid(GridView obj, int posicion, string[] texto)
        {
            for (int i = 0; i < obj.Rows.Count; i++)
            {
                if (texto.Any(ext => obj.Rows[i].Cells[posicion].Text.EndsWith(ext)))
                {
                    obj.Rows[i].Cells[posicion].Text = "<span class='TitulosRegistrosGrilla'>" + obj.Rows[i].Cells[posicion].Text + "</span>";
                }
            }
        }

        protected bool habilitarGuardado(string CodigoProyecto, string CodigoConvocatoria, int ConstanteIndice)
        {
            bool retorno = false;
            inicioEncabezado(CodigoProyecto, CodigoConvocatoria, ConstanteIndice);

            if (miembro == true && usuario.CodGrupo == Constantes.CONST_Emprendedor && realizado == false)
            {
                retorno = true;
            }

            return retorno;
        }

        protected bool habilitarGuardadoEval(string CodigoProyecto, string CodigoConvocatoria, int ConstanteIndice, int usuarioEvaluacion)
        {
            bool retorno = false;
            inicioEncabezado(CodigoProyecto, CodigoConvocatoria, ConstanteIndice);

            if (miembro == true && usuario.CodGrupo == usuarioEvaluacion && realizado == false)
            {
                retorno = true;
            }

            return retorno;
        }

        protected string UsuarioActual()
        {
            string query = "DECLARE @loginC VARCHAR(30), @infoC VARBINARY(128) SET @loginC=" + usuario.IdContacto.ToString() + " SET @infoC=CAST(@loginC AS VARBINARY(128)) SET CONTEXT_INFO @infoC";

            return query;
        }

        public string obtenerUltimaActualizacion(int idTab, string codProyecto)
        {
            consultas = new Consultas();

            bool nuevo = true;
            bool disabled = false;
            bool guardar = false;

            var conv = (from con in consultas.Db.ConvocatoriaProyectos
                        where con.CodProyecto == Convert.ToInt32(codProyecto)
                        orderby con.CodConvocatoria descending
                        select con).FirstOrDefault();

            if (conv != null)
            {
                if (conv.CodConvocatoria == 1 && codEstado >= Constantes.CONST_Evaluacion)
                    nuevo = false;
            }


            var query = (from tbp in consultas.Db.TabProyectos
                         from con in consultas.Db.Contactos
                         where tbp.CodContacto == con.Id_Contacto && tbp.CodTab == idTab && tbp.CodProyecto == Convert.ToInt32(codProyecto)
                         select tbp).FirstOrDefault();

            if (query != null)
            {
                var proy_c = (from pc in consultas.Db.ProyectoContactos
                              where pc.CodProyecto == Convert.ToInt32(codProyecto)
                              && pc.CodContacto == usuario.IdContacto
                              select pc).FirstOrDefault();

                if (proy_c != null)
                {
                    if (!(miembro &&
                        //num ´post it == 0 &&
                         (proy_c.CodRol == Constantes.CONST_RolAsesorLider && codEstado == Constantes.CONST_Inscripcion)
                        || (codEstado == Constantes.CONST_Evaluacion && proy_c.CodRol == Constantes.CONST_RolEvaluador && nuevo))
                        || query.Contacto.Nombres == "")
                    {
                        disabled = true;
                    }
                    if (miembro &&
                        //num ´post it == 0 &&
                         (proy_c.CodRol == Constantes.CONST_RolAsesorLider && codEstado == Constantes.CONST_Inscripcion)
                        || (codEstado == Constantes.CONST_Evaluacion && proy_c.CodRol == Constantes.CONST_RolEvaluador && nuevo)
                        || query.Contacto.Nombres != "")
                    {

                        guardar = true;
                    }
                }
            }
            else
            {
                query = new TabProyecto();
                disabled = true;
            }

            ProyectoTabRealizado tb_realizado = new ProyectoTabRealizado(query, nuevo, disabled, guardar);
            return tb_realizado.TransformText();
        }

        public void RedirectPage(bool cerrar = false, string mensaje = "", string close = "")
        {
            if (cerrar && !string.IsNullOrEmpty(mensaje))
            {
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "Mensaje", "alert('" + mensaje + "');", true);
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "", "window.opener.location.reload();window.close();", true);
            }
            else if (cerrar && string.IsNullOrEmpty(mensaje))
            {
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "", "window.opener.location.reload();window.close();", true);
            }
            else if (!string.IsNullOrEmpty(mensaje) && !cerrar)
            {
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "Mensaje", "alert('" + mensaje + "');", true);
            }
            else
            {
                if (!string.IsNullOrEmpty(close))
                {
                    ScriptManager.RegisterClientScriptBlock(this, GetType(), "Mensaje", "window.close();", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, GetType(), "Mensaje", "location.reload();", true);
                }

            }

        }

        public string GeneraClave()
        {
            int num1;
            int num2;
            int num3;
            int NumAleatorio;
            string fnGeneraClave;
            Random RandomClass = new Random();
            num1 = (Int32)((RandomClass.Next(1, 9)) + 1);	// Generate random value between 1 and 9.
            num2 = (Int32)((RandomClass.Next(1, 9)) + 1);// Generate random value between 1 and 9.
            num3 = (Int32)((9 * RandomClass.Next(1, 9)) + 1);	// Generate random value between 1 and 9.
            var cuantos = (from pm in consultas.Db.PasswordModelos
                           select pm).Count();
            NumAleatorio = (Int32)((RandomClass.Next(1, cuantos)) + 1);	// Generate random value between 1 and 9.
            var txtPalabra = (from pm in consultas.Db.PasswordModelos
                              where pm.Id_PasswordModelo == NumAleatorio
                              select pm).FirstOrDefault();
            fnGeneraClave = String.Concat(txtPalabra.Palabra.ToString(), num1.ToString(), num2.ToString(), num3.ToString());
            return fnGeneraClave;
        }

        public static void Redirect(HttpResponse response, string url, string target, string windowFeatures)
        {

            if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) && String.IsNullOrEmpty(windowFeatures))
            {
                response.Redirect(url);
            }
            else
            {
                Page page = (Page)HttpContext.Current.Handler;

                if (page == null)
                {
                    throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
                }
                url = page.ResolveClientUrl(url);

                string script;
                if (!String.IsNullOrEmpty(windowFeatures))
                {
                    script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
                }
                else
                {
                    script = @"window.open(""{0}"", ""{1}"");";
                }
                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
            }
        }

        public SqlDataReader ejecutaReader(String sql, int obj)
        {
            SqlDataReader reader = null;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                        reader.Close();
                }

                if (conn != null)
                    conn.Close();

                conn.Open();

                if (obj == 1)
                    reader = cmd.ExecuteReader();
                else
                    cmd.ExecuteReader();
            }
            catch (SqlException se)
            {
                reader = null;
                if (conn != null)
                    conn.Close();
                return null;
            }

            return reader;
        }

        #region metodos para encriptar url

        public SecureQueryString CreateQueryStringUrl(string parametros = "")
        {
            SecureQueryString querystringSeguro;

            if (!string.IsNullOrEmpty(parametros))
            {
                querystringSeguro = new SecureQueryString(new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 8 }, Request[parametros]);
            }
            else
            {
                querystringSeguro = new SecureQueryString(new Byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 8 });
            }


            return querystringSeguro;
        }



        #endregion

        #region Métodos creados por Mauricio Arias Olave.

        ///NOTA: es posible que, debido a temas de tiempo, se tuvo que duplicar el siguiente código en diferentes 
        ///formularios web; se puede eliminar el código repetido de las demas pantallas.

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/05/2014.
        /// Obtener la prórroga del proyecto seleccionado.
        /// </summary>
        /// <param name="codProyecto">Código del proyecto.</param>
        /// <returns>Prórroga obtenida.</returns>
        public Int32 ObtenerProrroga(string codProyecto)
        {
            //Inicializar variables.
            String sqlConsulta = "";
            System.Data.DataTable tabla = new System.Data.DataTable();
            Int32 prorroga_obtenida = 0;
            try
            {

                //Consulta.
                sqlConsulta = "select prorroga from ProyectoProrroga where codproyecto = " + codProyecto;

                //Asignar valores a variable DataTable.
                tabla = consultas.ObtenerDataTable(sqlConsulta, "text");

                //Si tiene datos, convierte el valor obtenido, de lo contrario devuleve cero.
                if (tabla.Rows.Count > 0)
                {
                    //Convertir el valor obtenido.
                    prorroga_obtenida = Int32.Parse(tabla.Rows[0]["prorroga"].ToString());
                    return prorroga_obtenida;
                }
                else
                    return prorroga_obtenida;
            }
            catch (Exception)
            { return prorroga_obtenida; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Método usado en "DeclaraVariables.inc" de FONADE Clásico.
        /// Usado para obtener el valor "Texto" de la tabla "Texto", este valor será usado en la creación
        /// de mensajes cuando el CheckBox "chk_actualizarInfo" esté chequeado; Si el resultado de la consulta
        /// NO trae datos, según FONADE Clásico, crea un registro con la información dada.
        /// </summary>
        /// <param name="NomTexto">Nombre del texto a consultar.</param>
        /// <returns>NomTexto consultado.</returns>
        public string Texto(String NomTexto)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String RSTexto;
            String txtSQL;
            bool correcto = false;

            //Consulta
            txtSQL = "SELECT Texto FROM Texto WHERE NomTexto='" + NomTexto + "'";

            var resultado = consultas.ObtenerDataTable(txtSQL, "text");

            if (resultado.Rows.Count > 0)
                return resultado.Rows[0]["Texto"].ToString();
            else
            {
                #region Si no existe la palabra "consultada", la crea.

                txtSQL = "INSERT INTO Texto (NomTexto, Texto) VALUES ('" + NomTexto + "','" + NomTexto + "')";

                //Asignar SqlCommand para su ejecución.
                cmd = new SqlCommand(txtSQL, conn);

                //Ejecutar SQL.
                correcto = EjecutarSQL(conn, cmd);

                if (correcto == false)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción de TEXTO.')", true);
                    return NomTexto; //""; //Debería retornar vacío y validar en el método donde se llame si esté validado.
                }
                else
                { return NomTexto; }

                #endregion
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/04/2014.
        /// Generar registros en tabla "LogEnvios".
        /// </summary>
        /// <param name="p_Asunto">Asunto.</param>
        /// <param name="p_EnviadoPor">Enviado Por.</param>
        /// <param name="p_EnviadoA">Enviado A:</param>
        /// <param name="p_Programa">Programa:</param>
        /// <param name="codProyectoActual">Código del proyecto</param>
        /// <param name="p_Exitoso">Exitoso "1/0".</param>
        public void prLogEnvios(String p_Asunto, String p_EnviadoPor, String p_EnviadoA, String p_Programa, Int32 codProyectoActual, Boolean p_Exitoso)
        {
            //Inicializar variables.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand();
            String sqlConsulta = "";
            bool correcto = false;

            try
            {
                sqlConsulta = " INSERT INTO LogEnvios (Fecha, Asunto, EnviadoPor, EnviadoA, Programa, CodProyecto, Exitoso) " +
                              " VALUES (GETDATE(),'" + p_Asunto + "','" + p_EnviadoPor + "','" + p_EnviadoA + "','" + p_Programa + "'," + codProyectoActual + "," + p_Exitoso + ") ";

                //Asignar SqlCommand para su ejecución.
                cmd = new SqlCommand(sqlConsulta, conn);

                //Ejecutar SQL.
                correcto = EjecutarSQL(conn, cmd);

                if (correcto == false)
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en inserción de log.')", true);
                    //return;
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Ejecutar SQL.
        /// Método que recibe la conexión y la consulta SQL y la ejecuta.
        /// </summary>
        /// <param name="p_connection">Conexión</param>
        /// <param name="p_cmd">Consulta SQL.</param>
        /// <returns>TRUE = Sentencia SQL ejecutada correctamente. // FALSE = Error.</returns>
        public bool EjecutarSQL(SqlConnection p_connection, SqlCommand p_cmd)
        {
            //Ejecutar controladamente la consulta SQL.
            try
            {
                p_connection.Open();
                p_cmd.ExecuteReader();
                p_connection.Close();
                return true;
            }
            catch (Exception) { return false; }
            //finally
            //{ p_connection.Close(); }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 06/06/2014.
        /// Obtener el código de la convocatoria, este método DEBE ser empleado cuando la variable de sesión
        /// y/u otras variables relacionadas al código de la convocatoria del proyecto seleccionado.
        /// </summary>
        /// <returns>String con datos = código de la convocatoria obtenida. // String vacío = No hay datos.</returns>
        public string ObtenerCodigoConvocatoria(string codProyecto)
        {
            //Inicializar variables.
            string txtSQL;
            string cod_convoc = "";
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                txtSQL = "SELECT CodConvocatoria FROM ConvocatoriaProyecto WHERE CodProyecto = " + codProyecto;
                dt = consultas.ObtenerDataTable(txtSQL, "text");
                if (dt.Rows.Count > 0)
                { cod_convoc = dt.Rows[0]["CodConvocatoria"].ToString(); txtSQL = null; dt = null; return cod_convoc; }
                else { txtSQL = null; dt = null; return ""; }
            }
            catch { txtSQL = null; dt = null; return ""; }
        }

        #endregion

        public String String_EjecutarSQL(SqlConnection p_connection, SqlCommand p_cmd)
        {
            //Ejecutar controladamente la consulta SQL.
            try
            {
                p_connection.Open();
                p_cmd.ExecuteReader();
                p_connection.Close();
                return "";
            }
            catch (Exception ex) { p_connection.Close(); return ex.Message; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/06/2014.
        /// Actualizar tab de evaluación.
        /// </summary>
        /// <param name="txtTab">Tab.</param>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <param name="CodConvocatoria">Código de la convocatoria.</param>
        public void prActualizarTabEval(String txtTab, String CodProyecto, String CodConvocatoria)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            String txtSQL;

            try
            {
                txtSQL = " SELECT * FROM tabEvaluacionProyecto WHERE codTabEvaluacion = " + txtTab +
                         " AND codproyecto = " + CodProyecto + " AND codConvocatoria = " + CodConvocatoria;

                var RSTab = consultas.ObtenerDataTable(txtSQL, "text");

                //Si no tiene datos, lo ingresa, de lo contrario, lo actualiza.
                if (RSTab.Rows.Count == 0)
                {
                    #region Inserción.
                    try
                    {
                        cmd = new SqlCommand("MD_prActualizarTabEval", con);
                        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodTabEvaluacion", txtTab);
                        cmd.Parameters.AddWithValue("@CodProyecto", CodProyecto);
                        cmd.Parameters.AddWithValue("@CodConvocatoria", CodConvocatoria);
                        cmd.Parameters.AddWithValue("@CodContacto", usuario.IdContacto);
                        cmd.Parameters.Add("@FechaModificacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Caso", "INSERT");
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                        cmd.Dispose();
                    }
                    catch { con.Close(); con.Dispose(); cmd.Dispose(); }
                    #endregion
                }
                else
                {
                    #region Actualización.
                    try
                    {
                        cmd = new SqlCommand("MD_prActualizarTabEval", con);
                        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodTabEvaluacion", txtTab);
                        cmd.Parameters.AddWithValue("@CodProyecto", CodProyecto);
                        cmd.Parameters.AddWithValue("@CodConvocatoria", CodConvocatoria);
                        cmd.Parameters.AddWithValue("@CodContacto", usuario.IdContacto);
                        cmd.Parameters.AddWithValue("@FechaModificacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Caso", "UPDATE");
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                        cmd.Dispose();
                    }
                    catch { con.Close(); con.Dispose(); cmd.Dispose(); }
                    #endregion
                }

                //Eliminar variable.
                RSTab = null;
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/06/2014.
        /// Actualizar tab padre....
        /// </summary>
        /// <param name="txtTab">Tab.</param>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <param name="CodConvocatoria">Código de la convocatoria.</param>
        public void prActualizarTabPadre(String txtTab, String CodProyecto, String CodConvocatoria)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            String txtSQL;
            DataTable RS = new DataTable();
            String txtTabs = "";
            String txtTabPadre = "";
            int flag = 1;

            try
            {
                //si es tab de proyecto.
                if (CodConvocatoria != "")
                    txtSQL = " select codtab from tab where id_tab = " + txtTab;
                else
                    txtSQL = " select codtabEvaluacion as codTab from tabEvaluacion where id_tabEvaluacion = " + txtTab;

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                #region if #1.
                //Encontrar el tab padre.
                if (RS.Rows.Count == 0)
                {
                    txtTabs = txtTab;
                    txtTabPadre = txtTab;
                }
                else
                {
                    #region Asignar tab y tabs en variable.

                    txtTabPadre = RS.Rows[0]["CodTab"].ToString();
                    //si es tab de proyecto.
                    if (CodConvocatoria != "") // "" instead null 
                    { txtSQL = " select id_tab from tab where codtab = " + txtTabPadre; }
                    else { txtSQL = " select id_tabEvaluacion as id_tab from tabEvaluacion where codtabEvaluacion = " + txtTabPadre; }

                    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    foreach (DataRow row in RS.Rows)
                    {
                        if (txtTabs == "")
                            txtTabs = txtTabs + row["id_tab"].ToString();
                        else
                            txtTabs = txtTabs + "," + row["id_tab"].ToString();
                    }

                    #endregion
                }
                #endregion

                #region if #2.
                //Verificar si estan marcados como realizados todos los hijos del tab padre.                
                if (CodConvocatoria == "") //si es tab de proyecto.
                {
                    if (txtTabPadre == Constantes.CONST_Impacto.ToString() || txtTabPadre == Constantes.CONST_ResumenEjecutivo.ToString())
                    {
                        txtSQL = " select realizado from tabproyecto where CodTab in (" + txtTabs + ") and codproyecto = " + CodProyecto;
                        RS = consultas.ObtenerDataTable(txtSQL, "text");

                        #region Establecer flag en el ciclo.

                        flag = 1;
                        if (RS.Rows.Count > 0)
                        {
                            foreach (DataRow row in RS.Rows)
                            {
                                if (row["realizado"].ToString() == "False" || row["realizado"].ToString() == "0")
                                {
                                    flag = 0;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            flag = 0;
                        }
                        #endregion
                    }
                    else
                    {
                        #region Establecer flag de la primera fila.
                        txtSQL = " SELECT count(T.id_tab)- sum( case when isnull(realizado, 0)=1 then 1 else 0 end) Faltan " +
                                 " FROM tab T LEFT JOIN tabProyecto TP ON T.id_tab=TP.codTab " +
                                 " AND codproyecto = " + CodProyecto +
                                 " WHERE T.codTab = " + txtTabPadre;

                        RS = consultas.ObtenerDataTable(txtSQL, "text");

                        flag = 0;

                        if (RS.Rows.Count > 0) { if (RS.Rows[0]["Faltan"].ToString() == "0" || String.IsNullOrEmpty(RS.Rows[0]["Faltan"].ToString())) { flag = 1; } }
                        #endregion
                    }
                }
                else
                {
                    #region Conteo.
                    txtSQL = " SELECT count(T.id_tabEvaluacion)- sum( case when isnull(realizado, 0)=1 then 1 else 0 end) Faltan " +
                             " FROM tabEvaluacion T LEFT JOIN tabEvaluacionProyecto TP ON " +
                             " T.id_tabEvaluacion=TP.codTabEvaluacion AND codConvocatoria = " + CodConvocatoria +
                             " AND codproyecto=" + CodProyecto + " WHERE  T.codTabEvaluacion = " + txtTabPadre;

                    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    flag = 0;

                    if (RS.Rows.Count > 0)
                    {
                        if (RS.Rows[0]["Faltan"].ToString() == "0" || String.IsNullOrEmpty(RS.Rows[0]["Faltan"].ToString()))
                        { flag = 1; }
                    }
                    #endregion
                }
                #endregion

                #region if #3.

                //Actualizar el estado del tab padre.
                if (CodConvocatoria == "")
                    txtSQL = " select realizado from tabproyecto where codtab = " + txtTabPadre + " and codproyecto = " + CodProyecto;
                else
                    txtSQL = " select realizado from tabEvaluacionProyecto where codtabEvaluacion = " + txtTabPadre + " and codConvocatoria = " + CodConvocatoria + " and codproyecto = " + CodProyecto;

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                #endregion

                #region if #4.

                if (RS.Rows.Count == 0)
                {
                    #region Inserción.
                    if (CodConvocatoria == "") //Si es tab de proyecto.
                    {
                        txtSQL = " insert into tabproyecto(codtab,codproyecto,codcontacto,fechamodificacion,realizado) " +
                                 " values(" + txtTabPadre + "," + CodProyecto + "," + usuario.IdContacto + ",GETDATE()," + flag + ")";
                    }
                    else
                    {
                        txtSQL = " insert into tabEvaluacionproyecto(codtabEvaluacion,codproyecto,codConvocatoria,codcontacto,fechamodificacion,realizado) " +
                                 " values(" + txtTabPadre + "," + CodProyecto + "," + CodConvocatoria + "," + usuario.IdContacto + ",GETDATE()," + flag + ")";
                    }
                    #endregion
                }
                else
                {
                    #region Actualización.

                    if (CodConvocatoria == "")
                        txtSQL = " update tabproyecto set realizado = " + flag + " where codtab = " + txtTabPadre + " and codproyecto = " + CodProyecto;
                    else
                        txtSQL = " update tabEvaluacionproyecto set realizado = " + flag + " where codtabEvaluacion = " + txtTabPadre + " and codConvocatoria = " + CodConvocatoria + " and codproyecto = " + CodProyecto;

                    #endregion

                    if (RS.Rows[0]["realizado"].ToString() != flag.ToString()) { return;/*txtScript = "window.top.location.reload();" & vbCrLf & "window.close();"*/}
                }

                #endregion

                //Ejecutar insert o update del "if #4".
                ejecutaReader(txtSQL, 2);
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 24/06/2014.
        /// Actualizar tab "interventor"...
        /// </summary>
        /// <param name="txtTab">Tab.</param>
        /// <param name="CodProyecto">Código del proyecto.</param>
        public void prActualizarTabInter(String txtTab, String CodProyecto)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            String txtSQL;
            DataTable RSTab = new DataTable();
            String txtTabs = "";
            String txtTabPadre = "";
            int flag = 1;

            try
            {
                txtSQL = " SELECT * FROM tabInterventorProyecto WHERE codTabInterventor = " + txtTab + " AND codproyecto = " + CodProyecto;
                RSTab = consultas.ObtenerDataTable(txtSQL, "text");

                if (RSTab.Rows.Count > 0)
                {
                    txtSQL = " INSERT INTO tabInterventorProyecto (CodTabInterventor, CodProyecto, CodContacto, FechaModificacion) " +
                             " VALUES (" + txtTab + "," + CodProyecto + ",  " + usuario.IdContacto + ",GETDATE()) ";
                }
                else
                {
                    txtSQL = " UPDATE tabInterventorProyecto " +
                             " SET codcontacto = " + usuario.IdContacto + ", fechamodificacion = GETDATE() " +
                             " WHERE codTabInterventor = " + txtTab + " and Codproyecto = " + CodProyecto;
                }

                #region Ejecutar insert o update "dependiendo del conteo de registros de la tabla".

                try
                {
                    cmd = new SqlCommand(txtSQL, con);
                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
                catch { con.Close(); con.Dispose(); cmd.Dispose(); }

                #endregion

                //Destruir la variable.
                RSTab = null;
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 07/07/2014.
        /// Actualizar tab "páginas de Proyecto".
        /// </summary>
        /// <param name="txtTab">Tab</param>
        /// <param name="CodProyecto">Código del proyecto.</param>
        public void prActualizarTab(String txtTab, String CodProyecto)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            String txtSQL;
            DataTable RSTab = new DataTable();
            String txtTabs = "";
            String txtTabPadre = "";
            int flag = 1;

            try
            {
                txtSQL = " select * from tabproyecto where codTab = " + txtTab + " and codproyecto = " + CodProyecto;
                RSTab = consultas.ObtenerDataTable(txtSQL, "text");

                if (RSTab.Rows.Count == 0)//>0 "es si está vacío!"
                {
                    txtSQL = " INSERT INTO tabproyecto (codtab,codproyecto,codcontacto,fechamodificacion) " +
                             " VALUES (" + txtTab + "," + CodProyecto + ",  " + usuario.IdContacto + ",GETDATE()) ";
                }
                else
                {
                    txtSQL = " UPDATE tabproyecto" +
                             " SET codcontacto = " + usuario.IdContacto + ", fechamodificacion = GETDATE() " +
                             " WHERE codtab = " + txtTab + "and Codproyecto = " + CodProyecto;
                }

                #region Ejecutar insert o update "dependiendo del conteo de registros de la tabla".

                #region Versión comentada.
                //try
                //{
                //    cmd = new SqlCommand(txtSQL, con);
                //    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                //    cmd.CommandType = CommandType.Text;
                //    cmd.ExecuteNonQuery();
                //    con.Close();
                //    con.Dispose();
                //    cmd.Dispose();
                //}
                //catch { con.Close(); con.Dispose(); cmd.Dispose(); } 
                #endregion

                //Ejecutar setencia.
                ejecutaReader(txtSQL, 2);

                #endregion

                //Destruir la variable.
                RSTab = null;
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 25/06/2014.
        /// Última Convocatoria en la que participó el proyecto.
        /// </summary>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <returns>boolean.</returns>
        public bool es_bNuevo(String CodProyecto)
        {
            bool re = true;

            try
            {
                string txtSQL = " select codconvocatoria from convocatoriaproyecto " +
                                " where codproyecto = " + CodProyecto + " order by codconvocatoria desc";

                var dt = consultas.ObtenerDataTable(txtSQL, "text");

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["codconvocatoria"].ToString() == "1" && codEstado >= Constantes.CONST_Evaluacion)
                    { re = false; return re; }
                }
                return re;
            }
            catch { return re; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 25/06/2014.
        /// Averiguar si "está en acta".
        /// </summary>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <param name="CodConvocatoria">Código de la convocatoria.</param>
        /// <returns>boolean.</returns>
        public bool es_EnActa(String CodProyecto, String CodConvocatoria)
        {
            bool re = false;

            try
            {
                string txtSQL = " select count(codproyecto) from evaluacionactaproyecto, evaluacionacta  " +
                                " where id_acta=codacta and codproyecto = " + CodProyecto +
                                " and codconvocatoria = " + CodConvocatoria;

                var dt = consultas.ObtenerDataTable(txtSQL, "text");

                if (dt.Rows.Count > 0) { re = true; return re; }

                dt = null;
                return re;
            }
            catch { return re; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/07/2014.
        /// prActualizarTabPadre CON UN SÓLO PARÁMETRO!
        /// Al parecer sólo lo usa la página "ProyectoResumenEquipo.aspx".
        /// </summary>
        /// <param name="txtTab">Tab.</param>
        public void m_prActualizarTabPadre(String txtTab, String CodProyecto, String CodConvocatoria)
        {
            //Inicializar variables.
            String txtSQL;
            String txtTabs = "";
            Int32 txtTabPadre = 0;
            Boolean flag = false;
            DataTable RS = new DataTable();
            SqlCommand cmd = new SqlCommand();

            if (CodConvocatoria == "") { txtSQL = " select codtab from tab where id_tab = " + txtTab; }//si es tab de proyecto
            else { txtSQL = " select codtabEvaluacion as codTab from tabEvaluacion where id_tabEvaluacion = " + txtTab; }

            RS = consultas.ObtenerDataTable(txtSQL, "text");

            if (RS.Rows.Count > 0)
            {
                #region Encontrar el tab padre.

                if (String.IsNullOrEmpty(RS.Rows[0]["codtab"].ToString()))
                {
                    txtTabs = txtTab;
                    txtTabPadre = Int32.Parse(txtTab);
                }
                else
                {
                    txtTabPadre = Int32.Parse(RS.Rows[0]["codtab"].ToString());
                    if (CodConvocatoria == "") { txtSQL = "select id_tab from tab where codtab = " + txtTabPadre; }//si es tab de proyecto
                    else { txtSQL = "select id_tabEvaluacion as id_tab from tabEvaluacion where codtabEvaluacion = " + txtTabPadre; }

                    RS = new DataTable();
                    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    foreach (DataRow row in RS.Rows)
                    {
                        if (txtTabs == "")
                        { txtTabs = txtTabs + row["id_tab"].ToString(); }
                        else
                        { txtTabs = txtTabs + ", " + row["id_tab"].ToString(); }
                    }
                }

                #endregion
            }

            //Verificar si estan marcados como realizados todos los hijos del tab padre.
            if (CodConvocatoria == "") //si es tab de proyecto
            {
                if (txtTabPadre == Constantes.CONST_Impacto || txtTabPadre == Constantes.CONST_ResumenEjecutivo)
                {
                    #region if (1)...

                    txtSQL = " select realizado from tabproyecto where CodTab IN (" + txtTabs + ") AND codproyecto = " + CodProyecto;

                    RS = new DataTable();
                    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    flag = true;//1;
                    if (RS.Rows.Count > 0)
                    {
                        foreach (DataRow row in RS.Rows)
                        {
                            try
                            {
                                if (!Boolean.Parse(row["realizado"].ToString()))
                                { flag = false;/*0;*/ break; }
                            }
                            catch { }
                        }
                    }
                    else { flag = false;/*0;*/ }

                    #endregion
                }
                else
                {
                    #region else (1)...

                    txtSQL = " SELECT count(T.id_tab)- sum( case when isnull(realizado, 0)=1 : 1 else 0 end) AS Faltan " +
                             " FROM tab T " +
                             " LEFT JOIN tabProyecto TP ON T.id_tab=TP.codTab AND codproyecto = " + CodProyecto +
                             " WHERE T.codTab = " + txtTabPadre;

                    RS = new DataTable();
                    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    flag = false;//0;

                    if (RS.Rows.Count > 0)
                    {
                        try { if (Int32.Parse(RS.Rows[0]["Faltan"].ToString()) == 0) { flag = true;/*1;*/ } }
                        catch { }
                    }

                    #endregion
                }
            }
            else
            {
                #region else (2)...

                txtSQL = " SELECT count(T.id_tabEvaluacion)- sum( case when isnull(realizado, 0)=1 : 1 else 0 end) AS Faltan " +
                         " FROM tabEvaluacion T " +
                         " LEFT JOIN tabEvaluacionProyecto TP ON " +
                         " T.id_tabEvaluacion=TP.codTabEvaluacion AND " +
                         " codConvocatoria = " + CodConvocatoria + " AND " +
                         " codproyecto = " + CodProyecto +
                         " WHERE T.codTabEvaluacion = " + txtTabPadre;

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                flag = false;//0;

                if (RS.Rows.Count > 0)
                {
                    try { if (Int32.Parse(RS.Rows[0]["Faltan"].ToString()) == 0) { flag = true;/*1;*/ } }
                    catch { }
                }

                #endregion
            }

            //Re-inicializar la variable.
            RS = new DataTable();

            //Actualizar el estado del tab padre.
            if (CodConvocatoria == "")//si es tab de proyecto.
            { txtSQL = " select realizado from tabproyecto where codtab = " + txtTabPadre + " and codproyecto = " + CodProyecto; }
            else { txtSQL = " select realizado from tabEvaluacionProyecto where codtabEvaluacion = " + txtTabPadre + " and codConvocatoria = " + CodConvocatoria + " and codproyecto = " + CodProyecto; }

            RS = consultas.ObtenerDataTable(txtSQL, "text");

            //If rs.eof
            if (RS.Rows.Count == 0)
            {
                if (CodConvocatoria == "") //si es tab de proyecto
                {
                    txtSQL = " insert into tabproyecto(codtab,codproyecto,codcontacto,fechamodificacion,realizado) " +
                             " values(" + txtTabPadre + ", " + CodProyecto + ", " + usuario.IdContacto + ", GETDATE(), " + flag + ") ";
                }
                else
                {
                    txtSQL = " insert into tabEvaluacionproyecto(codtabEvaluacion,codproyecto,codConvocatoria,codcontacto,fechamodificacion,realizado) " +
                             " values(" + txtTabPadre + ", " + CodProyecto + ", " + CodConvocatoria + ", " + usuario.IdContacto + ", GETDATE(), " + flag + ")";
                }
            }
            else
            {
                if (CodConvocatoria == "") //si es tab de proyecto
                { txtSQL = " update tabproyecto set realizado = " + flag + " where codtab = " + txtTabPadre + " and codproyecto = " + CodProyecto; }
                else
                { txtSQL = " update tabEvaluacionproyecto set realizado = " + flag + " where codtabEvaluacion = " + txtTabPadre + " and codConvocatoria = " + CodConvocatoria + " and codproyecto = " + CodProyecto; }

                try { if (Boolean.Parse(RS.Rows[0]["realizado"].ToString()) != flag) { } }//{ txtScript = "window.top.location.reload();" & vbCrLf & "window.close();";}}
                catch { }
            }

            #region Ejecutar INSERT o UPDATE (obtenido en la condición if/else anterior).

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

            #endregion

            RS = null;

        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 28/07/2014.
        /// Mostrar si el "tab" está "realizado" o "no".
        /// </summary>
        /// <param name="txtTab">Tab a procesar.</param>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <param name="CodConvocatoria">Código de la convocatoria. (valor vacío NUNCA NULL)</param>
        /// <returns>Booelan.</returns>
        public bool esRealizado(String txtTab, String CodProyecto, String CodConvocatoria)
        {
            //Inicializar variables.
            String txtSQL;
            Boolean realizado = false;

            try
            {
                #region Estado de proyecto y del tab (determina si se muestran o no los PostIt).

                txtSQL = "select codestado,realizado from proyecto ";

                if (CodConvocatoria == "") { txtSQL = txtSQL + " left join tabproyecto on  id_proyecto=codproyecto and codtab = " + txtTab; }
                else { txtSQL = txtSQL + " left join tabevaluacionproyecto on  id_proyecto=codproyecto and codtabevaluacion = " + txtTab; }

                txtSQL = txtSQL + " where id_proyecto = " + CodProyecto;

                var RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (!String.IsNullOrEmpty(RS.Rows[0]["realizado"].ToString())) { try { realizado = Boolean.Parse(RS.Rows[0]["realizado"].ToString()); } catch { realizado = false; } }

                RS = null;

                #endregion

                //Retornar lo procesado.
                return realizado;
            }
            catch
            { return realizado; }
        }

        #region Métodos de Proyecto.

        /// <summary>
        /// Mauricio Arias Olave.
        /// Ultima Convocatoria en la que participo el proyecto...
        /// </summary>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <returns>boolean.</returns>
        public bool es_nuevo_proyecto(String CodProyecto)
        {
            //Inicializar variables.
            String txtSQL;
            DataTable rs = new DataTable();
            bool bNuevo = true;

            try
            {
                txtSQL = " select codconvocatoria from convocatoriaproyecto where codproyecto = " + CodProyecto +
                         " order by codconvocatoria desc ";
                rs = consultas.ObtenerDataTable(txtSQL, "text");

                if (rs.Rows.Count > 0)
                {
                    if (Int32.Parse(rs.Rows[0]["codconvocatoria"].ToString()) == 1 && usuario.CodGrupo >= Constantes.CONST_Evaluacion) { bNuevo = false; }
                }

                return bNuevo;
            }
            catch { return bNuevo; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 25/07/2014.
        /// Obtener el estado de proyecto y del tab.
        /// </summary>
        /// <param name="txtTab">tab.</param>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <param name="CodConvocatoria">Código de la convocatoria "puede ser un valor vacío pero NO un (valor) NULL."</param>
        /// <returns>CodigoEstado.</returns>
        public Int32 CodEstado_Proyecto(String txtTab, String CodProyecto, String CodConvocatoria)
        {
            //Inicializar variables.
            String txtSQL;
            Int32 CodigoEstado = 0;
            DataTable RS = new DataTable();

            try
            {

                //Estado de proyecto y del tab
                txtSQL = "select codestado,realizado from proyecto ";

                if (CodConvocatoria.Trim() == "")
                { txtSQL = txtSQL + "left join tabproyecto on  id_proyecto=codproyecto and codtab = " + txtTab; }
                else
                { txtSQL = txtSQL + "left join tabevaluacionproyecto on  id_proyecto=codproyecto and codtabevaluacion =" + txtTab; }

                txtSQL = txtSQL + " where id_proyecto = " + CodProyecto;

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(RS.Rows[0]["CodEstado"].ToString()))
                    { CodigoEstado = Int32.Parse(RS.Rows[0]["CodEstado"].ToString()); Session["CodEstado"] = CodigoEstado.ToString(); }
                }

            }
            catch { return codEstado; }

            //Devolver el valor obtenido.
            Session["CodEstado"] = CodigoEstado;
            return CodigoEstado;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 31/07/2014.
        /// Marcar si está realizado o no...
        /// </summary>
        /// <param name="txtTab">Tab.</param>
        /// <param name="CodProyecto">Código del proyecto.</param>
        /// <param name="CodConvocatoria">Código de la convocatoria "con datos para ejecutar en las pestañas de EVALUACIÓN".</param>
        /// <param name="Realizado">Si está realizado o no.</param>
        public void Marcar(String txtTab, String CodProyecto, String CodConvocatoria, Boolean Realizado)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            String txtSQL = "";
            DataTable rs = new DataTable();
            Int32 numTabsEval = 0;
            Int32 valor = 0;

            try
            {
                if (CodConvocatoria == "")
                { txtSQL = " update tabproyecto "; }
                else { txtSQL = " update tabEvaluacionproyecto "; }

                if (Realizado)
                { txtSQL = txtSQL + " set realizado = 1 "; }
                else
                { txtSQL = txtSQL + " set realizado = 0 "; }

                if (CodConvocatoria == "")
                { txtSQL = txtSQL + " where codtab = " + txtTab + " and codproyecto = " + CodProyecto; }
                else { txtSQL = txtSQL + " where codtabEvaluacion = " + txtTab + " and codConvocatoria = " + CodConvocatoria + " and codproyecto = " + CodProyecto; }

                //Ejecutar SQL.
                ejecutaReader(txtSQL, 2);

                //Actualizar el tab padre
                prActualizarTabPadre(txtTab, CodProyecto, CodConvocatoria);

                //Si el coordinador aprueba una evaluación el emprendedor no debe poder realizar ningún cambio
                if (CodConvocatoria != "")
                {
                    //Calcular número de tabs de evaluación
                    rs = consultas.ObtenerDataTable("SELECT isnull(COUNT(0),0) as Total FROM TabEvaluacion WHERE codTabEvaluacion is NULL", "text");
                    valor = Int32.Parse(rs.Rows[0]["Total"].ToString());
                    numTabsEval = valor - 2; //No tomar en cuenta los tab informes y desempeño de evaluador.

                    //Calcular cuantos tabs estan aprobados
                    txtSQL = " select count(tep.codtabevaluacion) as conteo from tabevaluacionproyecto tep,tabevaluacion te " +
                             " where id_tabevaluacion=tep.codtabevaluacion  and realizado = 1 and te.codtabevaluacion is null " +
                             " and codproyecto = " + CodProyecto + " and codconvocatoria = " + CodConvocatoria;
                    rs = consultas.ObtenerDataTable(txtSQL, "text");

                    if (Int32.Parse(rs.Rows[0]["conteo"].ToString()) == numTabsEval)
                    {
                        //Si todos los tabs se encuentran aprobados el emprendedor no debe poder realizar cambios
                        txtSQL = " Update tabproyecto Set realizado = 1 where codproyecto = " + CodProyecto;

                        //Ejecutar SQL.
                        ejecutaReader(txtSQL, 2);
                    }

                    rs = null;
                }
            }
            catch { }
        }

        #endregion

        #region Métodos de la página "Pagos.inc".

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Retornar el String para colocar en la grilla de acuerdo al valor obtenido.
        /// </summary>
        /// <param name="Estado">Estado "numérico".</param>
        /// <returns>EstadoPago</returns>
        public string EstadoPago(Int32 Estado)
        {
            //Inicializar variables.
            String EstadoPago = "";

            try
            {
                switch (Estado)
                {
                    case Constantes.CONST_EstadoEdicion: EstadoPago = "Edición"; break;
                    case Constantes.CONST_EstadoInterventor: EstadoPago = "Interventor"; break;
                    case Constantes.CONST_EstadoCoordinador: EstadoPago = "Coordinador"; break;
                    case Constantes.CONST_EstadoFiduciaria: EstadoPago = "Fiduciaria"; break;
                    case Constantes.CONST_EstadoAprobadoFA: EstadoPago = "Aprobado"; break;
                    case Constantes.CONST_EstadoRechazadoFA: EstadoPago = "Rechazado"; break;
                    default: EstadoPago = ""; break; //NO debería caer aquí.
                }

                return EstadoPago;
            }
            catch { return EstadoPago; } //Error 
        }

        #endregion

        #region Métodos de CAPICOM...

        /// <summary>
        /// Función para detectar si Capicom se encuentra instalado en la máquina
        /// </summary>
        /// <returns>true = está instalado / false = no está instalado.</returns>
        public bool isCapicomAvailable()
        {
            //f3l
            /*
            bool instalado = true;
            int code = 0;

            try
            {
                Store oStore = new Store();
                oStore.Open(CAPICOM_STORE_LOCATION.CAPICOM_LOCAL_MACHINE_STORE, "Root", CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_READ_ONLY);
                code = System.Runtime.InteropServices.Marshal.GetExceptionCode();
                if (code != 0) { instalado = false; }

                return instalado;
            }
            catch (Exception)
            { instalado = false; return instalado; }
            */
            return false;//f3l
             
            }

        /// <summary>
        /// Función para firmar digitalmente un mensaje de datos.
        /// </summary>
        /// <param name="Datos">Datos "Xml"</param>
        /// <param name="Firma">Firma</param>
        public string SignData(String Datos, String Firma)
        {
            return "";//f3l
            /*
            SignedData SignedData;
            String FirmaDigital;
            string sign = "";

            try
            {
                //Verificar que Capicom se encuentra instalada en la máquina.
                if (!isCapicomAvailable())
                {
                    sign = "CAPICOM No esta Instalado. vbCritical Certicámara::CAPICOM::";
                    return sign;
                }

                //Crear el objeto Capicom que gestiona firmas digitales.
                SignedData = new SignedData();

                //Capturar los datos que van a ser firmados.
                SignedData.Content = Datos;

                //Crear la firma digital
                FirmaDigital = SignedData.Sign(null, true);

                Firma = FirmaDigital;

                return sign;

            }
            catch (Exception ex)
            {
                #region Se debe "evaluar" el tipo de código del error "en caso de que lo arroje".
                //int code = ex.HResult;

                //if (code == 0)//8010000C)
                //{

                //}
                #endregion
                int hr = Marshal.GetHRForException(ex);
                string hexValue = hr.ToString("X");
                switch (hexValue)
                {
                    case "8010000C":
                        sign = "Error: usted ha seleccionado un certificado que no posee una llave privada asociada. Verifique que el Token se encuentra conectado a la máquina";
                        break;

                    case "80090016":
                        sign = "Error: usted ha seleccionado un certificado que no posee una llave privada asociada. Verifique que el Token se encuentra conectado a la máquina";
                        break;

                    case "80880902":
                        sign = "Error: el proceso de firma falló. La selección del certificado fue cancelada por el usuario";
                        break;

                    case "8009000D":
                        sign = "Error: el proceso de firma falló. La selección del certificado fue cancelada por el usuario";
                        break;

                    case "80880231":
                        sign = "Error: no hay ningún certificado registrado en el almacén de certificados de Windows. Verifique que el Token se encuentra conectado a la máquina";
                        break;
                    default:
                        sign = "Error: not categorized. HResult: ''" + hr + "''  - Hex Error: ''" + hexValue + "''";
                        break;
                }
                return sign;
            }
             * */
        }

        /// <summary>
        /// Función para verificar la firma digital sobre un mensaje de datos. Verifica Integridad.
        /// EN FONADE clásico NO tiene parámetros, aquí se agregan para mejorar la comprensión del código fuente.
        /// </summary>
        /// <param name="Datos">Datos "Xml"</param>
        /// <param name="Firma">Firma</param>
        public bool VerifySign(String Datos, String Firma)
        {
            bool verificado = false;
            return verificado;//f3l
            /*
            try
            {
                //Crear el objeto Capicom que gestiona firmas digitales
                SignedData Verifier;

                //Capturar los datos que fueron firmados digitalmente
                Verifier = new SignedData();
                Verifier.Content = Datos;

                //Verificar la firma digital
                Verifier.Verify(Firma, true);

                //Validar si devuelve algún código de error.
                verificado = true;
                return verificado;
            }
            catch (Exception)
            { verificado = false; return verificado; }
            */
        }

        /// <summary>
        /// Función para verificar si el certificado de firma se encuentra en la Lista de Certificados Revocados CRL
        /// </summary>
        /// <param name="Datos">Datos "Xml"</param>
        /// <param name="Firma">Firma</param>
        /// <returns>true = continuar / false = stop.</returns>
        public bool Validatecrl(String Datos, String Firma)
        {
            bool bValidatecrl = false;
            
            return false;
            //f3l
            /*
            SignedData SingData;
            
            //int CAPICOM_VERIFY_SIGNATURE_ONLY = 0;
            //Inicializamos la constante CAPICOM_CHECK_ONLINE_REVOCATION_STATUS que permite hacer la verificación siempre en línea
            //CAPICOM.CAPICOM_CHECK_FLAG.CAPICOM_CHECK_ONLINE_REVOCATION_STATUS = //&H8;
            //int CAPICOM_CHECK_ONLINE_REVOCATION_STATUS = 8;

            try
            {
                SingData = new SignedData();

                //Capturar los datos que fueron firmados digitalmente
                SingData.Content = Datos;

                //Inicializar el valor por defecto de la verificación de CRL
                bValidatecrl = false;

                //Verificar la firma digital
                SingData.Verify(Firma, true, CAPICOM_SIGNED_DATA_VERIFY_FLAG.CAPICOM_VERIFY_SIGNATURE_ONLY);

                //Entrar a un foreach...
                foreach (Certificate cert in SingData.Certificates)
                {
                    if (cert.IsValid().Result)
                    {
                        //'MsgBox "El certificado digital no se encuentra la CRL. El certificado es valido", vbInformation
                        bValidatecrl = true;
                    }
                    else
                    {
                        //'MsgBox "El certificado digital se encuentra la CRL. El certificado no es valido";
                    }
                }

                return bValidatecrl;
            }
            catch (Exception ex)
            { bValidatecrl = false; return bValidatecrl; }
            */
        }

        /// <summary>
        /// Función para validar si el certificado de firma proviene de una Autoridad de Certificación de confianza
        /// </summary>
        /// <param name="Datos">Datos</param>
        /// <param name="Firma">Firma</param>
        /// <returns>boolean.</returns>
        public bool ValidateRoot(String Datos, String Firma)
        {
            return false;//f3l
            /* f3l
            SignedData SignedData;
            int resultado = 0;
            bool bValidateRoot = false;
            //'Inicializar la constante CAPICOM_CHECK_TRUSTED_ROOT
            //Const CAPICOM_CHECK_TRUSTED_ROOT = &H1

            //'Inicializar el valor por defecto de la verificación de cadena de certificación
            bValidateRoot = false;

            try
            {
                SignedData = new SignedData();

                //Capturar los datos que fueron firmados
                SignedData.Content = Datos;

                //Validar la firma digital
                SignedData.Verify(Firma, true, CAPICOM_SIGNED_DATA_VERIFY_FLAG.CAPICOM_VERIFY_SIGNATURE_ONLY);

                //Validar la cadena de certificación para cada uno de los certificados de firma

                foreach (Certificate certificado in SignedData.Certificates)
                {
                    certificado.IsValid().CheckFlag = CAPICOM_CHECK_FLAG.CAPICOM_CHECK_TRUSTED_ROOT;

                    if (!certificado.IsValid().Result) { resultado = resultado + 1; }
                }

                if (resultado <= 0) { bValidateRoot = true; }

                return bValidateRoot;
            }
            catch { bValidateRoot = false; return bValidateRoot; }
            */
        }

        /// <summary>
        /// Función para validar si el certificado de firma se encuentra dentro de su período de validez
        /// </summary>
        /// <param name="Datos">Datos</param>
        /// <param name="Firma">Firma</param>
        /// <returns>boolean.</returns>
        public bool Validatetime(String Datos, String Firma)
        {
            return false;
            /* f3l
            SignedData SignedData;
            int resultado = 0;
            bool ValidateTime = false;
            //'Inicializar la constante CAPICOM_CHECK_TRUSTED_ROOT
            //Const CAPICOM_CHECK_TIME_VALIDITY = &H2

            try
            {
                //Inicializar el valor por defecto de la verificación de cadena de certificación
                ValidateTime = false;

                //Crear el objeto Capicom que gestiona las firmas digitales
                SignedData = new SignedData();

                //Capturar los datos que fueron firmados
                SignedData.Content = Datos;

                //Validar la firma digital
                SignedData.Verify(Firma, true, CAPICOM_SIGNED_DATA_VERIFY_FLAG.CAPICOM_VERIFY_SIGNATURE_ONLY);

                //Validar la vigencia de cada uno de los certificados de firma
                foreach (Certificate certificado in SignedData.Certificates)
                {
                    certificado.IsValid().CheckFlag = CAPICOM_CHECK_FLAG.CAPICOM_CHECK_TIME_VALIDITY;

                    if (!certificado.IsValid().Result) { resultado = resultado + 1; }
                }

                if (resultado <= 0) { ValidateTime = true; }

                return ValidateTime;
            }
            catch { ValidateTime = false; return ValidateTime; }
            */
        }

        /// <summary>
        /// Función para desplegar los certificados empleados en el proceso de firma
        /// </summary>
        /// <param name="Datos">Datos</param>
        /// <param name="Firma">Firma</param>
        public void Viewcertificate(String Datos, String Firma)
        {
            //f3l
            /*
            SignedData Verifier;
            //Dim count //NO SE USA!

            try
            {
                //Crear el objeto que gestiona las firmas digitales
                Verifier = new SignedData();

                //Capturar los datos que fueron firmados digitalmente
                Verifier.Content = Datos;

                //Verificar la firma digital
                Verifier.Verify(Firma, true, CAPICOM_SIGNED_DATA_VERIFY_FLAG.CAPICOM_VERIFY_SIGNATURE_ONLY);

                //Desplegar los certificados empleados para el proceso de firma
                foreach (Certificate Certificate in Verifier.Certificates) { Certificate.Display(); }
            }
            catch { }
            */
        }

        /// <summary>
        /// Función para obtener los datos de los firmantes que intervienen en una firma.
        /// </summary>
        /// <param name="Datos">Datos</param>
        /// <param name="Firma">Firma</param>
        public void GetSigners(String Datos, String Firma)
        {
            /* f3l
            SignedData Verifier;
            String Msgbox = "";

            try
            {
                //Inicializar el objeto Capicom que gestiona las firmas digitales
                Verifier = new SignedData();

                Verifier.Content = Datos;

                //Verificar la firma digital
                Verifier.Verify(Firma, true, CAPICOM_SIGNED_DATA_VERIFY_FLAG.CAPICOM_VERIFY_SIGNATURE_ONLY);

                foreach (Certificate Certificate in Verifier.Certificates)
                {
                    if (Msgbox == "") { Msgbox = "Los datos del fimante del certificado son: " + Certificate.SubjectName + " <br/>"; }
                    else { Msgbox = "Los datos del fimante del certificado son: " + Certificate.SubjectName + " <br/>"; }
                }


                if (Msgbox != "")
                { System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + Msgbox + "')", true); }

            }
            catch { }
            */
        }

        #endregion

        #region Métodos de CrytoAPI...

        /// <summary>
        /// Funcion para traer el codigo de rechazo de la firma digital
        /// </summary>
        /// <param name="Hexa">Valor.</param>
        /// <returns>int</returns>
        public int TraerCodRechazoFirmaDigital(String Hexa)
        {
            String txtSQL;
            DataTable rsRechazo = new DataTable();
            DataTable rsRechazoAux = new DataTable();
            int i_TraerCodRechazoFirmaDigital = 0;

            try
            {
                txtSQL = "SELECT Id_RechazoFirmaDigital FROM RechazoFirmaDigital WHERE Hexadecimal = '" + Hexa.Trim() + "'";
                rsRechazo = consultas.ObtenerDataTable(txtSQL, "text");

                if (rsRechazo.Rows.Count > 0)
                {
                    i_TraerCodRechazoFirmaDigital = Int32.Parse(rsRechazo.Rows[0]["Id_RechazoFirmaDigital"].ToString());
                }
                else
                {
                    txtSQL = "SELECT Id_RechazoFirmaDigital FROM RechazoFirmaDigital WHERE Hexadecimal = 'Otro'";
                    rsRechazoAux = consultas.ObtenerDataTable(txtSQL, "text");

                    if (rsRechazoAux.Rows.Count > 0) { i_TraerCodRechazoFirmaDigital = Int32.Parse(rsRechazoAux.Rows[0]["Id_RechazoFirmaDigital"].ToString()); }
                    else { i_TraerCodRechazoFirmaDigital = 10; }
                    rsRechazoAux = null;
                }

                rsRechazo = null;
                i_TraerCodRechazoFirmaDigital = 1;
                return i_TraerCodRechazoFirmaDigital;
            }
            catch { return i_TraerCodRechazoFirmaDigital; }
        }

        /// <summary>
        /// Ver "CryptoAPI.asp" (línea 418). siempre devuelve true...
        /// </summary>
        /// <param name="Datos">Datos</param>
        /// <param name="Firma">Firma</param>
        /// <returns>siempre devuelve true...</returns>
        public bool ValidaFirmantes(String Datos, String Firma)
        { return true; /* ValidaFirmantes = true;*/ }

        #endregion
    }
}