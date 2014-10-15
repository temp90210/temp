using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Fonade.Account;
using Fonade.Clases;

namespace Fonade.FONADE.AdministrarPerfiles
{
    public partial class ActivarEmprendedor : Negocio.Base_Page
    {
        public Int64 CodContacto;

        protected void Page_Load(object sender, EventArgs e)
        {
            CodContacto = Convert.ToInt64(Request.QueryString["CodContacto"]);

            if (!IsPostBack)
            {
                try
                {
                    lbl_Titulo.Text = "EMPRENDEDORES INACTIVOS - REACTIVAR";
                    var query = (from Cont in consultas.Db.Contactos
                                 where Cont.Id_Contacto == CodContacto
                                 select new
                                 {
                                     NombreInactivo = Cont.Nombres,
                                     ApellidoInactivo = Cont.Apellidos,
                                     IdentInactivo = Cont.Identificacion,
                                     emailInactivo = Cont.Email,
                                     direccionInactivo = Cont.Direccion,
                                     telefonoInactivo = Cont.Telefono
                                 }).FirstOrDefault();

                    txnombres.Text = Convert.ToString(query.NombreInactivo);
                    txapellidos.Text = Convert.ToString(query.ApellidoInactivo);
                    txdireccion.Text = Convert.ToString(query.direccionInactivo);
                    txemail.Text = Convert.ToString(query.emailInactivo);
                    txidentificacion.Text = Convert.ToString(query.IdentInactivo);
                    txtelefono.Text = Convert.ToString(query.telefonoInactivo);
                }

                catch (Exception)
                { }
            }
        }

        /// <summary>
        /// Reactivar asesor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnReactivar_Click(object sender, EventArgs e)
        {
            #region NO BORRAR.
            //try
            //{
            //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            //    SqlCommand cmd = new SqlCommand("MD_ActivarEmprendedor", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@id_contactoQ", CodContacto);
            //    cmd.Parameters.AddWithValue("@NombreQ", txnombres.Text);
            //    cmd.Parameters.AddWithValue("@ApellidoQ", txapellidos.Text);
            //    cmd.Parameters.AddWithValue("@emailQ", txemail.Text);
            //    cmd.Parameters.AddWithValue("@IdentificacionQ", Convert.ToInt64(txidentificacion.Text));
            //    cmd.Parameters.AddWithValue("@direccionQ", txdireccion.Text);
            //    cmd.Parameters.AddWithValue("@telefonoQ", txtelefono.Text);
            //    cmd.Parameters.AddWithValue("@id_contactoLogQ", usuario.IdContacto);
            //    SqlParameter retornoConteo = new SqlParameter("@Conteo", SqlDbType.Int);
            //    retornoConteo.Direction = ParameterDirection.ReturnValue;
            //    cmd.Parameters.Add(retornoConteo);
            //    SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
            //    con.Open();
            //    cmd2.ExecuteNonQuery();
            //    cmd.ExecuteNonQuery();
            //    int conteo = (int)cmd.Parameters["@Conteo"].Value;
            //    con.Close();
            //    con.Dispose();
            //    cmd.Dispose();
            //    cmd2.Dispose();
            //    if (conteo == 0)
            //    {
            //        //Correo("kharin.alfonso@glogic.com.co", "prueba");
            //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Emprendedor activado satisfactoriamente!'); document.location=('FiltroEmprendedorInactivo.aspx');", true);
            //       // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "window.location.href('FiltroEmprendedorInactivo.aspx');", true);
            //        //Response.Redirect("FiltroEmprendedorInactivo.aspx");
            //    }
            //    else
            //    {
            //        ScriptManager.RegisterClientScriptBlock(BtnReactivar, this.GetType(), "Mensaje", "alert('El correo ingresado ya se encuentra registrado con otra cuenta, por favor intente con un correo diferente!')", true);
            //    }

            //}
            //catch (Exception exc)
            //{ //Response.Write( exc.Message); 
            //}             
            #endregion

            #region Versión SQL mejorada.

            //Inicializar variables.
            String nombre = txnombres.Text;
            String apellido = txapellidos.Text;
            String Email = txemail.Text;
            String Identificacion = txidentificacion.Text;
            String dire = txdireccion.Text;
            String telefono = txtelefono.Text;
            String txtClave = "";
            string sentenciaSQL1 = "";
            DataTable rsupdate = new DataTable();
            DataTable rsClave = new DataTable();
            String txtMensaje = "";
            bool varExitoso = false;

            try
            {
                #region Validar los campos para omitir "comilla sencilla".

                if (nombre.Contains("'")) { nombre = nombre.Replace("'", ""); }

                if (apellido.Contains("'")) { apellido = apellido.Replace("'", ""); }

                if (Email.Contains("'")) { Email = Email.Replace("'", ""); }

                if (Identificacion.Contains("'")) { Identificacion = Identificacion.Replace("'", ""); }

                if (dire.Contains("'")) { dire = dire.Replace("'", ""); }

                if (telefono.Contains("'")) { telefono = telefono.Replace("'", ""); }

                #endregion

                //Verifica que el email sea unico
                sentenciaSQL1 = "select Email from contacto where email='" + Email + "' and id_contacto<>" + CodContacto;
                rsupdate = consultas.ObtenerDataTable(sentenciaSQL1, "text");

                if (rsupdate.Rows.Count == 0)
                {
                    //Se entra a modificar:
                    sentenciaSQL1 = " Update contacto set Inactivo=0, nombres='" + nombre + "', apellidos='" + apellido + "'," +
                                    " Email='" + Email + "', identificacion=" + Identificacion + "," +
                                    " direccion='" + dire + "',telefono='" + telefono + "'" +
                                    " where id_contacto=" + CodContacto;

                    //Ejecutar SQL UPDATE.
                    ejecutaReader(sentenciaSQL1, 2);

                    #region Consultar la clave.

                    sentenciaSQL1 = "select Clave from Contacto where id_contacto=" + CodContacto;
                    rsClave = consultas.ObtenerDataTable(sentenciaSQL1, "text");

                    if (rsClave.Rows.Count > 0)
                    { txtClave = rsClave.Rows[0]["Clave"].ToString(); }

                    //Destruir la variable.
                    rsClave = null;

                    #endregion

                    #region Se registra la activación en la tabla de activaciones-

                    sentenciaSQL1 = " insert into ContactoReActivacion (CodContacto, FechaReActivacion, CodContactoQReActiva) " +
                                    " values(" + CodContacto + ", GETDATE(), " + usuario.IdContacto + ")";

                    //Ejecutar consulta.
                    ejecutaReader(sentenciaSQL1, 2);

                    #endregion

                    #region Se agrega campo para verificar actualizacion de informacion despues de reactivacion sandraem 30/07/2010

                    sentenciaSQL1 = " insert into ContactoActualizoReactivacion (CodContacto,ActualizoDatos,CambioClave, FechaReActivacion) " +
                                    " values(" + CodContacto + ",0,0, GETDATE())";

                    //Ejecutar consulta.
                    ejecutaReader(sentenciaSQL1, 2);

                    #endregion

                    #region Envíar el mail al usuario.

                    #region Consultar el "TEXTO" del mensaje.

                    //Variable que contiene el cuerpo del mensaje.
                    txtMensaje = Texto("TXT_EMAILENVIOCLAVE");

                    //Sólo por si acaso, si el resultado de "txtMensaje" NO devuelve los datos según el texto esperado,
                    //se debe asignar el texto tal cual se vió en BD el "28/04/2014".
                    if (txtMensaje.Contains("Señor Usuario") || txtMensaje.Trim() == null)
                    {
                        txtMensaje = "Señor Usuario Con el usuario {{Email}} y contraseña {{Clave}},  podrá acceder al sistema de información por medio de la pagina www.fondoemprender.com,  allí encontrara en la parte superior del sistema específicamente en el botón con el signo de interrogación  (?) el manual de su perfil ''{{Rol}}''";
                    }

                    //Reemplazar determinados caracteres por caracteres definidos específicamente para esta acción.
                    txtMensaje = txtMensaje.Replace("{{Rol}}", "Emprendedor");
                    txtMensaje = txtMensaje.Replace("{{Email}}", Email.Trim());
                    txtMensaje = txtMensaje.Replace("{{Clave}}", txtClave);

                    #endregion

                    try
                    {
                        //Generar y enviar mensaje.
                        Correo correo = new Correo(ConfigurationManager.AppSettings.Get("Email").ToString(),
                                                   "Fondo Emprender",
                                                   Email.Trim(),
                                                   nombre.Trim() + " " + apellido.Trim(),
                                                   "Re-Activación a Fondo Emprender",
                                                   txtMensaje);
                        correo.Enviar();

                        //El mensaje fue enviado.
                        varExitoso = true;

                        //Inserción en tabla "LogEnvios".
                        prLogEnvios("Fondo Emprender", ConfigurationManager.AppSettings.Get("Email").ToString(), Email.Trim(), "Reactivación emprendedor", 0, varExitoso);
                    }
                    catch
                    {
                        //El mensaje no pudo ser enviado.
                        if (!varExitoso)
                        {
                            //Inserción en tabla "LogEnvios".
                            prLogEnvios("Fondo Emprender", ConfigurationManager.AppSettings.Get("Email").ToString(), Email.Trim(), "Reactivación emprendedor", 0, varExitoso);
                        }
                    }

                    #endregion
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(BtnReactivar, this.GetType(), "Mensaje", "alert('El Email Ingresado ya existe Por favor Verificar')", true);
                    return;
                }
            }
            catch { }

            //Finalmente se redirige al usuario a "FiltroEmprendedorInactivo.aspx".
            Response.Redirect("FiltroEmprendedorInactivo.aspx");

            #endregion
        }
    }
}