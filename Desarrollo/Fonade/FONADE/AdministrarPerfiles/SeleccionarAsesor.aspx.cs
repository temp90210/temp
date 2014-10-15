using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.AdministrarPerfiles
{
    public partial class SeleccionarAsesor :  Negocio.Base_Page
    {
       
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Panel1.Visible= false;
                Panel2.Visible= true;
                Panel3.Visible = false;
                var query3 = (from ti in consultas.Db.TipoIdentificacions
                              select new
                              {
                                  Id_TipoIdentificacion = ti.Id_TipoIdentificacion,
                                  NomTipoIdentificacion = ti.NomTipoIdentificacion,

                              }
                             );
                ddl_tipoDocumento.DataTextField = "NomTipoIdentificacion";

                ddl_tipoDocumento.DataValueField = "Id_TipoIdentificacion";

                ddl_tipoDocumento.DataSource = query3;
                ddl_tipoDocumento.DataBind();
            }

        }

        protected void Buscar_onclick(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
            Session["tb_TipodeIdentificacion"] =ddl_tipoDocumento.SelectedValue.ToString();
            Session["tb_NumeroDocumento"] = tb_NumeroDocumento.Text;
            short prueba1 = short.Parse(ddl_tipoDocumento.SelectedValue);
            double prueb2 = double.Parse(tb_NumeroDocumento.Text);
           var identificacion = (
                         from c in consultas.Db.Contactos
                         where c.Identificacion == prueb2
                              & c.CodTipoIdentificacion == prueba1
                         select new
                         {
                             Nombres = c.Nombres,
                             Apellidos = c.Apellidos,
                             IdContacto = c.Id_Contacto,
                             Email = c.Email,
                             NumeroDocumento = c.Identificacion,
                             TipoID = c.TipoIdentificacion

                         }).FirstOrDefault();

            

            if (identificacion != null && identificacion.IdContacto != 0)
            {
                var grupo = (

                 from g in consultas.Db.Grupos
                 from gc in consultas.Db.GrupoContactos
                 where gc.CodGrupo == g.Id_Grupo
                       & gc.CodContacto == identificacion.IdContacto
                 select new
                 {

                     Rol = g.NomGrupo
                 }).FirstOrDefault();

                if (grupo != null)
                {
                    btn_SeleccionarAsesor.Enabled = false;
                    btn_SeleccionarAsesor.CssClass += "boton_Link_Grid";
                    pruebas.Text = " " + prueb2 + " - "   + identificacion.Nombres + " " + identificacion.Apellidos + " ";
                    btn_SeleccionarAsesor.Text = pruebas.Text;
                    lbl_rol.Text = grupo.Rol;
                    Label3.Text = "El usuario ya tiene un rol asignado y no puede ser cambiado.";
                   
                    Session["tb_Email"]  = identificacion.Email;
                    Session["tb_NombreAsesor"]  = identificacion.Nombres;
                    Session["tb_NumeroDocumento"] = prueb2.ToString();
                    Session["tb_ApellidoAsesor"]  = identificacion.Apellidos;
                    Session["tb_NumeroIdentificacion"] =Convert.ToString(identificacion.NumeroDocumento);
                    Session["tb_TipodeIdentificacion"] =identificacion.TipoID.NomTipoIdentificacion;
                }
                else
                {
                    btn_SeleccionarAsesor.Enabled = true;
                    pruebas.Text = " " + prueb2 + " - " + identificacion.Nombres + " " + identificacion.Apellidos + " ";
                    btn_SeleccionarAsesor.Text = pruebas.Text;
                    lbl_rol.Text = "Usuario sin Rol";
                    Label3.Text = "Seleccione el usuario a asignar";
                    Session["tb_Email"]  =  identificacion.Email;
                    Session["tb_NombreAsesor"]  = identificacion.Nombres;
                    Session["tb_NumeroDocumento"]  = prueb2.ToString();
                    Session["tb_ApellidoAsesor"]  = identificacion.Apellidos;
                    Session["tb_NumeroIdentificacion"]  = Convert.ToString(identificacion.NumeroDocumento);
                    Session["tb_TipodeIdentificacion"]  = identificacion.TipoID.NomTipoIdentificacion;
                    Session["CodContacto"] = identificacion.IdContacto;

                }
                 
            }
            else
            {
                Panel1.Visible = false;
                Panel2.Visible = false;
                Panel3.Visible = true;
                tb_NumeroDocumento.Enabled = true;
                tb_Email.Enabled = true;
                tb_NombreAsesor.Enabled = true;
                tb_ApellidoAsesor.Enabled = true;             
                 
                
            }




        }

        protected void btn_Asesor_click(object sender, EventArgs e) {

            ClientScriptManager cm = this.ClientScript;
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location =window.opener.location;window.close();</script>");
        
        }

        protected void btn_nuevabusqueda_click(object sender, EventArgs e){
         Panel1.Visible= false;
         Panel2.Visible= true;
         Session["tb_Email"] = null;
         Session["tb_NombreAsesor"] = null;
         Session["tb_NumeroDocumento"] = null;
         Session["tb_ApellidoAsesor"] = null;
         Session["tb_NumeroIdentificacion"] = null;
         Session["tb_TipodeIdentificacion"] = null;
         tb_NumeroDocumento.Text = null;
         pruebas.Text = "";
          
            
        }

        protected void CrearPerfil_onclick(object sender, EventArgs e) {

            CrearJefe();

            
        }

        private void CrearJefe()
        {
            SqlDataReader reader = null;

            string txtNombres = tb_NombreAsesor.Text;
            string txtApellidos = tb_ApellidoAsesor.Text;
            string txtEmail = tb_Email.Text;
            string CodTipoIdentificacion = Session["tb_TipodeIdentificacion"].ToString();
            string numIdentificacion = Session["tb_NumeroDocumento"].ToString();
            Session["tb_Email"] = txtEmail;
            Session["tb_NombreAsesor"] = txtNombres;
            Session["tb_NumeroDocumento"] = tb_NumeroDocumento.Text;
            Session["tb_ApellidoAsesor"] = txtApellidos;
            Session["tb_NumeroIdentificacion"] = numIdentificacion;

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

            if (resul.Rows.Count != 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El usuario con el correo electrónico ingresado ya existe en el sistema.')", true);
                return;
            }
            else
            {
                #region crearAsesor
                txtSQL = "INSERT INTO Contacto (Nombres, Apellidos, CodTipoIdentificacion, Identificacion,Email,Clave) VALUES('" + txtNombres + "','" + txtApellidos + "'," + CodTipoIdentificacion + "," + numIdentificacion + ",'" + txtEmail + "','" + txtClave + "')";
                ejecutaReader(txtSQL, 1);

                txtSQL = "SELECT Id_Contacto FROM Contacto WHERE CodTipoIdentificacion = " + CodTipoIdentificacion + " AND Identificacion = " + numIdentificacion;
                reader = ejecutaReader(txtSQL, 1);

                int CodContacto = 0;
                string txtNomTipoDoc;

                if (reader != null)
                    if (reader.Read())
                        CodContacto = Convert.ToInt32(reader[0].ToString());
                Session["CodContacto"] = CodContacto;


                txtSQL = "SELECT NomTipoIdentificacion FROM TipoIdentificacion WHERE Id_TipoIdentificacion = " + CodTipoIdentificacion;
                reader = ejecutaReader(txtSQL, 1);

                if (reader != null)
                    if (reader.Read())
                        txtNomTipoDoc = reader[0].ToString();
                #endregion

                ClientScriptManager cm = this.ClientScript;
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>window.opener.location =window.opener.location;window.close();</script>");

              
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