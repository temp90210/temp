using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;

namespace Fonade.Controles
{
    public partial class CargarArchivos : System.Web.UI.UserControl
    {
        RespuestaCargue respuesta = new RespuestaCargue();

        /// <summary>
        /// Array de strings en la cual se mensionan las extenciones que permite el control(Eje: new string[] { "xls", "xlsx" })
        /// </summary>
        private string[] ExtensionesPermitidas;
        private string PathDestino;
        private string PathDestinoTEMP = ConfigurationManager.AppSettings.Get("RutaDocumentosTEMP");
        private string NombreDocumento;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["ExtensionesPermitidas"] = "";
                Session["PathDestino"] = "";
                Session["PathDestinoTEMP"] = "";
                Session["NombreDocumento"] = "";
            }
            lblErrorDocumento.Text = "";
            if (Session["ExtensionesPermitidas"].ToString() != "")
                LlenarVariables();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
        }

        public void show(string[] extensionesPermitidas, string pathDestino, string nombreDocumento)
        {
            Session["ExtensionesPermitidas"] = extensionesPermitidas;
            Session["PathDestino"] = pathDestino;
            Session["PathDestinoTEMP"] = PathDestinoTEMP + pathDestino.Substring(2);
            Session["NombreDocumento"] = nombreDocumento;
            LlenarVariables();
            pnlCargue.Visible = true;
        }

        protected void LlenarVariables()
        {
            ExtensionesPermitidas = (string[])Session["ExtensionesPermitidas"];
            PathDestino = Session["PathDestino"].ToString();
            PathDestinoTEMP = Session["PathDestinoTEMP"].ToString();
            NombreDocumento = Session["NombreDocumento"].ToString();
        }

        protected void btnSubirDocumento_Click(object sender, EventArgs e)
        {
            string[] extencion;

            if (ValidarFormatoDocumento())
            {
                if (fuArchivo.PostedFile.ContentLength > 10485760) // = 10MB
                {
                    respuesta.Mensaje = "El tamaño del archivo debe ser menor a 10 Mb.";
                    lblErrorDocumento.Text = respuesta.Mensaje;
                    return;
                }
                else
                {
                    extencion = fuArchivo.PostedFile.FileName.ToString().Trim().Split('.');
                    respuesta.Extencion = extencion[extencion.Length - 1];

                    if (CargarTemporal())
                    {
                        CargarDocumentoFinal();
                    }
                    else
                    {
                        //El valor de la variable "respuesta.Mensaje" ya fue asignado en el método "CargarTemporal()".
                        //respuesta.Mensaje = "El archivo no pudo ser validado.";
                        lblErrorDocumento.Text = respuesta.Mensaje;
                        return;
                    }
                }
            }
            else
            {
                respuesta.Mensaje = "El archivo no pudo ser validado.";
                lblErrorDocumento.Text = respuesta.Mensaje;
                return;
            }
        }

        /// <summary>
        /// Se agrega la validación del tamaño del archivo seleccionado.
        /// </summary>
        /// <returns>TRUE = Puede continuar. // FALSE = No puede continuar.</returns>
        protected bool ValidarFormatoDocumento()
        {
            if (fuArchivo.PostedFile.FileName.ToString().Trim() != "")
            {
                if (ExtensionesPermitidas.Any(ext => fuArchivo.PostedFile.FileName.EndsWith(ext)) == false)
                {
                    respuesta.Mensaje = "El archivo seleccionado no es valido";
                    lblErrorDocumento.Text = respuesta.Mensaje;
                    return false;
                }
                if (fuArchivo.PostedFile.ContentLength > 10485760) // = 10MB
                {
                    respuesta.Mensaje = "El tamaño del archivo debe ser menor a 10 Mb.";
                    lblErrorDocumento.Text = respuesta.Mensaje;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                respuesta.Mensaje = "Debe Seleccionar un archivo";
                lblErrorDocumento.Text = respuesta.Mensaje;
                return false;
            }
        }

        protected bool CargarTemporal()
        {
            if (File.Exists(PathDestinoTEMP) == false)
            {
                try
                {
                    System.IO.Directory.CreateDirectory(PathDestinoTEMP);
                    try
                    {
                        fuArchivo.PostedFile.SaveAs(PathDestinoTEMP + NombreDocumento + "." + respuesta.Extencion);
                        respuesta.PathTemporal = PathDestinoTEMP + NombreDocumento + "." + respuesta.Extencion;
                        return true;
                    }
                    catch
                    {
                        respuesta.Mensaje = "Error No se pudo subir el documento a la carpeta TMP: ";
                        lblErrorDocumento.Text = respuesta.Mensaje;
                        return false;
                    }
                }
                catch
                {
                    respuesta.Mensaje = "Error No se pudo crear la carpeta TMP: " + PathDestinoTEMP;
                    lblErrorDocumento.Text = respuesta.Mensaje;
                    return false;
                }
            }
            //respuesta.Mensaje = "OK"; //Lo comenté, pero lo lógico es que aquí devuelva este valor "junto con el TRUE".
            return true;
        }

        protected bool CargarDocumentoFinal()
        {
            if (File.Exists(PathDestino) == false)
            {
                try
                {
                    System.IO.Directory.CreateDirectory(PathDestino);
                }
                catch
                {
                    respuesta.Mensaje = "Error No se pudo crear la carpeta: " + PathDestino;
                    lblErrorDocumento.Text = respuesta.Mensaje;
                    return false;
                }
            }

            try
            {
                byte[] archivoPlano = File.ReadAllBytes(PathDestinoTEMP + NombreDocumento + "." + respuesta.Extencion);
                try
                {
                    File.WriteAllBytes(PathDestino + NombreDocumento + "." + respuesta.Extencion, archivoPlano);
                    respuesta.Mensaje = "OK";
                    lblErrorDocumento.Text = respuesta.Mensaje;
                    respuesta.PathFisico = PathDestino + NombreDocumento + "." + respuesta.Extencion;
                    File.Delete(PathDestinoTEMP + NombreDocumento + "." + respuesta.Extencion);

                    return true;
                }
                catch
                {
                    respuesta.Mensaje = "Error al mover el archivo temporal a la ruta final: " + PathDestino;
                    lblErrorDocumento.Text = respuesta.Mensaje;
                    respuesta.PathFisico = PathDestino + NombreDocumento + "." + respuesta.Extencion;
                    return false;
                }
            }
            catch
            {
                respuesta.Mensaje = "Error No se pudo crear la carpeta: " + PathDestino;
                lblErrorDocumento.Text = respuesta.Mensaje;
                return false;
            }

        }

        public RespuestaCargue Resultado()
        {
            return respuesta;
        }
    }

    public class RespuestaCargue
    {
        public string Mensaje { get; set; }
        public string PathTemporal { get; set; }
        public string PathFisico { get; set; }
        public string Extencion { get; set; }
    }
}