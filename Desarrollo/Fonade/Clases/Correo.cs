using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;

namespace Fonade.Clases
{
    public class Correo
    {
        private string DeEmail { get; set; }
        private string DeNombre { get; set; }
        private string ParaEmail { get; set; }
        private string ParaNombre { get; set; }
        private string Asunto { get; set; }
        private string Cuerpo { get; set; }
        private string SMTP = ConfigurationManager.AppSettings.Get("SMTP");
        private string SMTPUsuario = ConfigurationManager.AppSettings.Get("SMTPUsuario");
        private string SMTPPassword = ConfigurationManager.AppSettings.Get("SMTPPassword");
        private int SMTP_UsedPort = Int32.Parse(ConfigurationManager.AppSettings.Get("SMTP_UsedPort"));

        public Correo(string deEmail, string deNombre, string paraEmail, string paraNombre, string asunto, string cuerpo)
        {
            //Pruebas.
            this.DeEmail = "felipe@dominiopublico.com.co"; //deEmail;
            this.DeNombre = "usuario pruebas emails.";//deNombre;
            this.ParaEmail = "pruebasemail@mailinator.com"; //paraNombre;
            this.ParaNombre = "usuario recibir emails."; //paraEmail;
            this.Asunto = asunto;
            this.Cuerpo = cuerpo;

            #region DESCOMENTAR PARA PRODUCCIÓN.
            //this.DeEmail = deEmail;
            //this.DeNombre = deNombre;
            //this.ParaEmail = paraNombre;
            //this.ParaNombre = paraEmail;
            //this.Asunto = asunto;
            //this.Cuerpo = cuerpo; 
            #endregion
        }
        public void Enviar()
        {
            string strBody = "";
            strBody = "<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML//EN\">" + "\n";
            strBody = strBody + "<html>";
            strBody = strBody + "<head>";
            strBody = strBody + "<meta http-equiv=\"Content-Type\"";
            strBody = strBody + "content=\"text/html; charset=iso-8859-1\">";
            strBody = strBody + "<meta name=\"GENERATOR\" content=\"Microsoft FrontPage 2.0\">";
            strBody = strBody + "<title>Fondo Emprender</title>";
            strBody = strBody + "</head>";
            strBody = strBody + this.Cuerpo;
            strBody = strBody + "</html>";

            MailMessage correo = new MailMessage();
            correo.From = new MailAddress(this.DeEmail, this.DeNombre);
            correo.To.Add(new MailAddress(this.ParaEmail, this.ParaNombre));

            correo.Subject = this.Asunto;
            //generacion del cuerpo del correo
            correo.Body = strBody;
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;

            string smtpservidor = this.SMTP;
            string userSmtp = this.SMTPUsuario;
            string passwordSmtp = this.SMTPPassword;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = smtpservidor;
            smtp.Port = SMTP_UsedPort; //Puerto para envío de correos.
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(userSmtp, passwordSmtp);

            smtp.Credentials = credentials;
            smtp.Send(correo);
        }
    }
}