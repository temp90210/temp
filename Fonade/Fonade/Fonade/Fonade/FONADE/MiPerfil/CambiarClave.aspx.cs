using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Fonade.FONADE.MiPerfil
{
    public partial class CambiarClave : Negocio.Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            l_usuariolog.Text = usuario.Nombres + " " + usuario.Apellidos;
            l_fechaActual.Text = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");
        }

        protected void Btn_Cancelar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "window.close();", true);
        }

        protected void Btn_cambiarClave_Click(object sender, EventArgs e)
        {
            bool validado = false;

            #region Validando longuitud de caracteres digitados.
            if (txt_claveActual.Text.Trim().Length > 20)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La clave actual digitada no puede tener mas de 20 caracteres.')", true);
                validado = false;
            }
            else
            { validado = true; }
            if (txt_nuevaclave.Text.Trim().Length > 20)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La nueva clave no puede tener mas de 20 caracteres.')", true);
                validado = false;
            }
            else { validado = true; }
            if (txt_confirmaNuevaClave.Text.Trim().Length > 20)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La confirmación de la nueva clave no puede tener mas de 20 caracteres.')", true);
                validado = false;
            }
            else { validado = true; }
            #endregion

            if (validado == true)
            {
                validarClaveActual();
            }
        }

        protected void validarClaveActual()
        {
            var query = (from usu in consultas.Db.Contactos
                         where usu.Id_Contacto == usuario.IdContacto
                         select new
                         {
                             clave = usu.Clave,
                         }).FirstOrDefault();
            if (txt_claveActual.Text != query.clave)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La clave actual es incorrecta!')", true);
            }
            else
            {
                if (txt_claveActual.Text == txt_nuevaclave.Text)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La nueva clave no puede ser igual a la actual!')", true);
                }
                else
                {
                    validarClavesUsadas();
                }
            }
        }

        protected void validarClavesUsadas()
        {
            var query = (from pass in consultas.Db.Claves
                         where pass.NomClave == txt_nuevaclave.Text
                         & pass.codContacto == usuario.IdContacto
                         select new
                        {
                            pass
                        }).Count();

            if (query == 0)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_CambiarClave", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodUsuario", usuario.IdContacto);
                cmd.Parameters.AddWithValue("@nuevaclave", txt_nuevaclave.Text);
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Clave cambiada exitosamente!')", true);
                ScriptManager.RegisterClientScriptBlock(this, this.Page.GetType(), "RedirectScript", "window.close()", true);

            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La nueva clave ya ha sido usada con anterioridad!')", true);
            }

        }
    }
}