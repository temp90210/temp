using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fonade.Clases;

namespace Fonade.FONADE.AdministrarPerfiles
{
    public partial class CrearAsesores :  Negocio.Base_Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // llenar opciondes del dropdown de t.ipo de identificación
            //Detección de click en el panel de busqueda de asesor
            var controlName = Request.Params.Get("__EVENTTARGET");
            var argument = Request.Params.Get("__EVENTARGUMENT");
            tb_ApellidoAsesor.ReadOnly = true;
            tb_Email.ReadOnly = true;
            tb_NombreAsesor.ReadOnly = true;
            tb_NumeroIdentificacion.ReadOnly = true;
            tb_TipodeIdentificacion.ReadOnly = true;
                try {
                    
                  
                        if (Session["Flag"] == "2")
                        {
                            Session["Flag"] = null;
                            tb_ApellidoAsesor.Enabled = true;
                            tb_Email.Enabled = true;
                            tb_NombreAsesor.Enabled = true;
                            tb_NumeroIdentificacion.Enabled = true;
                            tb_TipodeIdentificacion.Enabled = true;
                            tb_ApellidoAsesor.Text = Session["tb_ApellidoAsesor"].ToString();
                            tb_Email.Text = Session["tb_Email"].ToString();
                            tb_NombreAsesor.Text = Session["tb_NombreAsesor"].ToString();
                            tb_NumeroIdentificacion.Text = Session["tb_NumeroDocumento"].ToString();
                            tb_TipodeIdentificacion.Text = Session["tb_TipodeIdentificacion"].ToString();
                            Session["Flag"] = "3";



                        }
                        if (Session["Flag"] != "1")
                        {
                            if (controlName == "Panel1" && argument == "Click")
                            {
                                Session["Flag"] = null;
                                Session["Flag"] = "2";
                                Redirect(null, "SeleccionarAsesor.aspx", "_Blank", "width=300,height=300");

                            }

                        }
                        if (!Page.IsPostBack)
                        {
                            Session["Flag"] = "0";
                            tb_ApellidoAsesor.Enabled = false;
                            tb_Email.Enabled = false;
                            tb_NombreAsesor.Enabled = false;
                            tb_NumeroIdentificacion.Enabled = false;
                            tb_TipodeIdentificacion.Enabled = false;

                             
                        }
                    
                 
                    
                   
                }catch{
                
                }
                
            
            
                    
           
            
        }

        protected void CrearPerfil_onclick(object sender, EventArgs e)
        {
            CrearJefe();
        }

    

        private SqlDataReader ejecutaReader(String sql, int obj)
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
                if (conn != null)
                    conn.Close();
                return null;
            }

            return reader;
        }
        
        private void CrearJefe()
        {
            SqlDataReader reader = null;

            string txtNombres = tb_NombreAsesor.Text;
	        string txtApellidos = tb_ApellidoAsesor.Text;
	        string txtEmail = tb_Email.Text;
            string CodTipoIdentificacion = Session["tb_TipodeIdentificacion"].ToString();
            string numIdentificacion = Session["tb_NumeroDocumento"].ToString();
            string Texto_Obtenido;

            #region validarCampos
            if (String.IsNullOrEmpty(txtNombres) || String.IsNullOrEmpty(txtApellidos) || String.IsNullOrEmpty(txtEmail) || String.IsNullOrEmpty(numIdentificacion))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Todos los campos son requeridos')", true);
                return;
            }

            if (!IsValidEmail(txtEmail))
            {
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El correo no es valido')", true);
                    return;
                }
            }

            Int64 valida;

            if (!Int64.TryParse(numIdentificacion, out valida))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El campo numero de identificacion tiene que ser numero')", true);
                return;
            }
            #endregion 

            string txtClave = GeneraClave(); 

            string txtSQL = "SELECT Email FROM Contacto WHERE Email like'%" + txtEmail + "%'";

            var resul = consultas.ObtenerDataTable(txtSQL, "text");

            if (resul == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El usuario ya tiene un rol asignado y no puede ser cambiado.')", true);
                return;
            }
            else
            {
                int CodContacto = Int32.Parse(Session["CodContacto"].ToString());

                txtSQL = "SELECT Id_Grupo FROM Grupo WHERE NomGrupo = 'Asesor'";
                reader = ejecutaReader(txtSQL, 1);

                int CodGrupo = 5;
                    if (reader != null)
                        if (reader.Read())
                    CodGrupo = Convert.ToInt32(reader[0].ToString());

                txtSQL = "UPDATE contacto SET Inactivo = 0, CodInstitucion = " + usuario.CodInstitucion + " WHERE Id_Contacto = " + CodContacto;

                ejecutaReader(txtSQL, 2);

                txtSQL = "DELETE FROM GrupoContacto WHERE CodContacto = " + CodContacto;

                ejecutaReader(txtSQL, 2);

                txtSQL = "INSERT INTO GrupoContacto (CodGrupo,CodContacto) VALUES (" + CodGrupo + "," + CodContacto + ")";

                ejecutaReader(txtSQL, 2);
                #region Envio de correo Asesor Creado
                //Consultar el "TEXTO".
                Texto_Obtenido = Texto("TXT_EMAILENVIOCLAVE");

                //Sólo por si acaso, si el resultado de "Texto_Obtenido" NO devuelve los datos según el texto esperado,
                //se debe asignar el texto tal cual se vió en BD el "28/04/2014".

               
                //Reemplazar determinados caracteres por caracteres definidos específicamente para esta acción.
                Texto_Obtenido = Texto_Obtenido.Replace("{{Rol}}", "Asesor");
                Texto_Obtenido = Texto_Obtenido.Replace("{{Email}}", txtEmail.Trim());
                Texto_Obtenido = Texto_Obtenido.Replace("{{Clave}}", txtClave);

                try
                {
                    //Generar y enviar mensaje.
                    Correo correo = new Correo(usuario.Email,
                                               "Fondo Emprender",
                                               txtEmail.Trim(),
                                               txtNombres.Trim() + " " + txtApellidos.Trim(),
                                               "Registro a Fondo Emprender",
                                               Texto_Obtenido);
                    correo.Enviar();
                    prLogEnvios("Registro a Fondo Emprender", usuario.Email, txtEmail.Trim(), "Crear Asesor",0 , true);
                  
                }
                catch
                {
                    //El mensaje no pudo ser enviado.
                    

                    //Inserción en tabla "LogEnvios".
                    prLogEnvios("Registro a Fondo Emprender", usuario.Email, txtEmail.Trim(), "Crear Asesor", 0, false);
                }
                   
                #endregion
                tb_ApellidoAsesor.Enabled = false;
                tb_Email.Enabled = false;
                tb_NombreAsesor.Enabled = false;
                tb_NumeroIdentificacion.Enabled = false;
                tb_TipodeIdentificacion.Enabled = false;
                Session["tb_ApellidoAsesor"]=null;
                Session["tb_Email"] = null;
                  Session["tb_NombreAsesor"]=null;
                  Session["tb_NumeroDocumento"] = null;
                  Session["tb_TipodeIdentificacion"] = null;

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El Asesor fue creado exitosamente');window.location='AdministrarAsesores.aspx'", true);
               // Response.Redirect("AdministrarAsesores.aspx");
               
            }
        }

        public bool IsValidEmail(string strIn)
        {
            return

            System.Text.RegularExpressions.Regex.IsMatch(strIn, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

    }
}