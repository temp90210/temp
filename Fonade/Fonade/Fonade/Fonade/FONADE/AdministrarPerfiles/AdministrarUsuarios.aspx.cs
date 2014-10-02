using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Account;

namespace Fonade.FONADE.AdministrarPerfiles
{
    public partial class AdministrarUsuarios : Negocio.Base_Page
    {
        string crearAdmin = "Crear Administrador";
        string crearGerenteEvaluador = "Crear GerenteEvaluador";
        string crearCallCenter = "Crear Call Center";
        string crearGerenteInterventor = "Crear Gerente Interventor";
        string crearPerfilFiduciario = "Crear Perfil Fiduciario";
        String grupos1;
        public String[] grupos = { "0" };
        int idusuario;
        string[] codgrupousuario;
        /// <summary>
        /// Variable que contiene la URL actual.
        /// </summary>
        string var_url;

        protected string GeneraClave()
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

        protected void void_ModificarDatos(string[] codgrupo, int Grupocontacto)
        {
            if (Request.QueryString["CodContacto"] != null)
            {
                int codigoContacto = Int32.Parse(Request.QueryString["CodContacto"]);

                var query = (from c in consultas.Db.Contactos
                             where c.Id_Contacto == codigoContacto
                             select c);

                foreach (Contacto c in query)
                {
                    c.Apellidos = tb_apelidos.Text;
                    c.Cargo = tb_cargo.Text;
                    c.Email = tb_email.Text;
                    c.Fax = tb_fax.Text;
                    c.Identificacion = Int32.Parse(tb_no.Text);
                    c.Nombres = tb_nombres.Text;
                    c.Telefono = tb_telefono.Text;
                    c.CodTipoIdentificacion = short.Parse(ddl_identificacion.SelectedValue);
                }

                try
                {
                    consultas.Db.ExecuteCommand(UsuarioActual());
                    consultas.Db.SubmitChanges();
                    void_show("La información del usuario ha sido actualizado correctamente", true);
                }
                catch (Exception ex) { }
            }
        }
        protected void void_CrearDatos(string[] codgrupo, int codusuario)
        {
            try
            {
                string nuevaclave;
                nuevaclave = GeneraClave();
                Contacto contacto = new Contacto()
                {
                    Apellidos = tb_apelidos.Text,
                    Cargo = tb_cargo.Text,
                    Email = tb_email.Text,
                    Fax = tb_fax.Text,
                    Identificacion = Int32.Parse(tb_no.Text),
                    Nombres = tb_nombres.Text,
                    Clave = nuevaclave,
                    Telefono = tb_telefono.Text,
                    CodTipoIdentificacion = short.Parse(ddl_identificacion.SelectedValue)
                };
                GrupoContacto Grupocontacto = new GrupoContacto()
                {
                    CodGrupo = Int32.Parse(ddl_perfil.SelectedValue),
                    CodContacto = contacto.Id_Contacto
                };
                //enviar datos 
                try
                {
                    var vaidarcontacto = (from c1 in consultas.Db.Contactos
                                          where c1.Email == contacto.Email
                                          select c1).FirstOrDefault();

                    if (vaidarcontacto == null)
                    {
                        consultas.Db.Contactos.InsertOnSubmit(contacto);
                        consultas.Db.SubmitChanges();
                        var nuevocontacto = (from c in consultas.Db.Contactos
                                             where c.Email == contacto.Email
                                             select c).FirstOrDefault();
                        Grupocontacto.CodContacto = nuevocontacto.Id_Contacto;
                        consultas.Db.GrupoContactos.InsertOnSubmit(Grupocontacto);
                        consultas.Db.SubmitChanges();
                        void_show("el usuario " + nuevocontacto.Email + " ha sido agregado", true);


                    }
                    else
                    {
                        void_show("el usuario " + vaidarcontacto.Email + " ya existe intente nuevamente", true);

                    }

                }
                catch (Exception ex)
                { }
            }
            catch (OverflowException) { }
        }
        protected void void_show(string texto, bool mostrar)
        {
            //lbl_popup.Visible = mostrar;           
            //lbl_popup.Text = texto;
            //mpe1.Enabled = mostrar;
            //mpe1.Show();
            string queryRedir = Request.Url.Query;

            queryRedir = queryRedir.Substring(0, queryRedir.IndexOf('&'));

            string redirePagina = string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, Request.Url.AbsolutePath, queryRedir);

            ClientScriptManager cm = this.ClientScript;
            cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('" + texto + "');location.href='" + redirePagina + "'</script>");
        }
        protected void void_HideControls(string accion, string[] codgrupo)
        {
            switch (accion)
            {
                case "Crear":
                    tbl_infousuario.Rows[3].Visible = false;
                    if (codgrupo.Contains(Constantes.CONST_AdministradorFonade.ToString()) ||
                        codgrupo.Contains(Constantes.CONST_AdministradorSena.ToString()) || codgrupo.Equals("" + Constantes.CONST_AdministradorFonade.ToString() + "," + Constantes.CONST_AdministradorSena.ToString()))
                    {
                        tbl_infousuario.Rows[6].Visible = true;
                    }
                    else
                    {
                        tbl_infousuario.Rows[6].Visible = false;
                    };
                    tbl_infousuario.Rows[5].Visible = false;

                    if (codgrupo.Contains(Constantes.CONST_AdministradorFonade.ToString()) ||
                        codgrupo.Contains(Constantes.CONST_AdministradorSena.ToString()) || codgrupo.Equals("" + Constantes.CONST_AdministradorFonade.ToString() + "," + Constantes.CONST_AdministradorSena.ToString()))
                    {
                        tbl_infousuario.Rows[5].Visible = true;
                    }

                    btn_crearActualizar.Text = "Crear";
                    break;
                case "Editar":
                    tbl_infousuario.Rows[3].Visible = false;
                    if (codgrupo.Contains(Constantes.CONST_AdministradorFonade.ToString()) ||
                        codgrupo.Contains(Constantes.CONST_AdministradorSena.ToString()))
                    {
                        tbl_infousuario.Rows[6].Visible = true;
                    }
                    else
                    {
                        tbl_infousuario.Rows[6].Visible = false;
                    };
                    tbl_infousuario.Rows[5].Visible = false;

                    btn_crearActualizar.Text = "Actualizar";

                    break;
                default: break;
            }

        }
        protected void void_traerDatos(string[] codgrupo, int codusuario)
        {
            var tipoidentificación = (from ti in consultas.Db.TipoIdentificacions
                                      select new
                                      {
                                          Id_TipoIdentificacion = ti.Id_TipoIdentificacion,
                                          NomTipoIdentificacion = ti.NomTipoIdentificacion,

                                      });
            var Roles = (from r in consultas.Db.Grupos
                         select new
                         {
                             Rol = r.NomGrupo,
                             codRol = r.Id_Grupo,
                         });
            if (codgrupo != null)
            {
                Roles.Where(r => codgrupo.Contains(r.codRol.ToString()));
            }

            ddl_identificacion.DataTextField = "NomTipoIdentificacion";
            ddl_identificacion.DataValueField = "Id_TipoIdentificacion";
            ddl_identificacion.DataSource = tipoidentificación;
            ddl_identificacion.DataBind();
            ddl_perfil.DataTextField = "Rol";
            ddl_perfil.DataValueField = "codRol";
            ddl_perfil.DataSource = Roles;
            ddl_perfil.DataBind();

            var contacto = (from c in consultas.Db.Contactos
                            from gc in consultas.Db.GrupoContactos
                            where c.Id_Contacto == codusuario & c.Id_Contacto == gc.CodContacto
                            select new
                            {
                                nombres = c.Nombres,
                                apellidos = c.Apellidos,
                                codtipoidentificacion = c.CodTipoIdentificacion,
                                Tipoidentificacion = c.TipoIdentificacion,
                                identificacion = c.Identificacion,
                                cargo = c.Cargo,
                                email = c.Email,
                                telefono = c.Telefono,
                                fax = c.Fax,
                                codgrupo = gc.CodGrupo,
                                NomGrupo = gc.Grupo
                            }).FirstOrDefault();

            tb_apelidos.Text = contacto.apellidos;
            tb_cargo.Text = contacto.cargo;
            tb_email.Text = contacto.email;
            tb_fax.Text = contacto.fax;
            tb_no.Text = contacto.identificacion.ToString();
            tb_nombres.Text = contacto.nombres;
            tb_telefono.Text = contacto.telefono;
            ddl_identificacion.SelectedValue = contacto.codtipoidentificacion.ToString();
            ddl_perfil.SelectedValue = contacto.NomGrupo.ToString();
        }
        protected void void_traerDatosRol(string[] codgrupo, int codusuario)
        {
            var tipoidentificación = (from ti in consultas.Db.TipoIdentificacions
                                      select new
                                      {
                                          Id_TipoIdentificacion = ti.Id_TipoIdentificacion,
                                          NomTipoIdentificacion = ti.NomTipoIdentificacion,
                                      });
            var Roles = (from g in consultas.Db.Grupos
                         where codgrupo.Contains(g.Id_Grupo.ToString())
                         select new
                         {
                             Rol = g.NomGrupo,
                             codRol = g.Id_Grupo,
                         });
            ddl_identificacion.DataTextField = "NomTipoIdentificacion";
            ddl_identificacion.DataValueField = "Id_TipoIdentificacion";
            ddl_identificacion.DataSource = tipoidentificación;
            ddl_identificacion.DataBind();
            ddl_perfil.DataTextField = "Rol";
            ddl_perfil.DataValueField = "codRol";
            ddl_perfil.DataSource = Roles;
            ddl_perfil.DataBind();
            var contacto = (from c in consultas.Db.Contactos
                            from gc in consultas.Db.GrupoContactos
                            where c.Id_Contacto == codusuario & c.Id_Contacto == gc.CodContacto
                            select new
                            {
                                nombres = c.Nombres,
                                apellidos = c.Apellidos,
                                codtipoidentificacion = c.CodTipoIdentificacion,
                                Tipoidentificacion = c.TipoIdentificacion,
                                identificacion = c.Identificacion,
                                cargo = c.Cargo,
                                email = c.Email,
                                telefono = c.Telefono,
                                fax = c.Fax,
                                codgrupo = gc.CodGrupo,
                                NomGrupo = gc.Grupo
                            }).FirstOrDefault();

        }
        protected void void_ObtenerParametros()
        {
            try
            {
                if (!String.IsNullOrEmpty(Request.QueryString["codGrupo"]))
                {
                    grupos1 = Request.QueryString["codGrupo"];
                    grupos = grupos1.Split(',');
                }
                if (!String.IsNullOrEmpty(Request.QueryString["Accion"]))
                {
                    pnl_Administradores.Visible = false;
                    pnl_crearEditar.Visible = true;
                    AgregarUsuario.Visible = false;
                    if (!String.IsNullOrEmpty(Request.QueryString["Accion"]))
                    {
                        if (Request.QueryString["Accion"].ToString() == "Editar")
                        {
                            AgregarUsuario.Text = "Actualizar";
                            idusuario = Int32.Parse(Request.QueryString["CodContacto"]);
                            void_traerDatos(codgrupousuario, idusuario);
                        }
                        if (Request.QueryString["Accion"].ToString() == "Crear")
                        {
                            AgregarUsuario.Text = "Crear";
                        }
                    }
                }
            }
            catch
            {
                #region Código que redirecciona al usuario de acuerdo a la URL determinada.

                //Instanciar la variable que contiene la URL.
                string val = "";
                val = Session["URL_redirect"] != null ? val = Session["URL_redirect"].ToString() : "";

                //Si NO contiene  URL
                if (val == "")
                { Response.Redirect("../MiPerfil/Home.aspx"); }
                else //De lo contrario, la usa.
                { Response.Redirect(val); }

                #endregion
            }
        }
        /// <summary>
        /// Redirecciona al usuario a la página correspondiente.
        /// </summary>
        /// <param name="parametro">Si el parámetro NO  está vacío, se crea una variable de sesión usada mas adelante.</param>
        protected void void_ModificarTextolink()
        {
            if (grupos.Contains(Constantes.CONST_AdministradorFonade.ToString()) ||
               grupos.Contains(Constantes.CONST_AdministradorSena.ToString()))
            {
                AgregarUsuario.Text = crearAdmin;

            }
            if (grupos.Contains(Constantes.CONST_GerenteEvaluador.ToString()))
            {
                AgregarUsuario.Text = crearGerenteEvaluador;
            }
            if (grupos.Contains(Constantes.CONST_CallCenter.ToString()))
            {
                AgregarUsuario.Text = crearCallCenter;
            }
            if (grupos.Contains(Constantes.CONST_GerenteInterventor.ToString()))
            {
                AgregarUsuario.Text = crearGerenteInterventor;
            }
            if (grupos.Contains(Constantes.CONST_Perfil_Fiduciario.ToString()))
            {
                AgregarUsuario.Text = crearPerfilFiduciario;
            }

            string url = Request.Url.GetLeftPart(UriPartial.Path);
            url += (Request.QueryString.ToString() == "") ? "?Accion=Crear" : "?" + Request.QueryString.ToString() + "&Accion=Crear";
            AgregarUsuario.NavigateUrl = url;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string Accion = "";
                string codigoGrupo = "0";
                int codigoContacto = 0;
                if (!String.IsNullOrEmpty(Request.QueryString["CodGrupo"]))
                {
                    codigoGrupo = Request.QueryString["CodGrupo"];
                    grupos1 = Request.QueryString["codGrupo"];
                    grupos = grupos1.Split(',');
                }
                if (!String.IsNullOrEmpty(Request.QueryString["CodContacto"]))
                {
                    codigoContacto = Int32.Parse(Request.QueryString["CodContacto"]);
                }
                if (!String.IsNullOrEmpty(Request.QueryString["Accion"]))
                {
                    Accion = Request.QueryString["Accion"].ToString();
                    void_HideControls(Accion, grupos);
                };
                void_traerDatosRol(grupos, codigoContacto);
                pnl_crearEditar.Visible = false;
                string url = Request.Url.GetLeftPart(UriPartial.Path);
                url += (Request.QueryString.ToString() == "") ? "?Accion=Crear" : "?" + Request.QueryString.ToString() + "&Accion=Crear";
                AgregarUsuario.NavigateUrl = url;
                void_ObtenerParametros();
                lbl_Titulo.Text = void_establecerTitulo(grupos, Accion, "Texto");
                void_ModificarTextolink();
            }
        }
        protected void gv_administradores_DataBound(object sender, EventArgs e)
        {
            String[] grupos2 = { "0" };
            String grupos3;
            if (!String.IsNullOrEmpty(Request.QueryString["codGrupo"]))
            {
                grupos3 = Request.QueryString["codGrupo"];
                grupos2 = grupos1.Split(',');
                if (grupos2.Contains(Constantes.CONST_AdministradorFonade.ToString()) | grupos2.Contains(Constantes.CONST_AdministradorSena.ToString()))
                {
                    gv_administradores.Columns[3].Visible = true;
                }
                else
                {
                    gv_administradores.Columns[3].Visible = false;
                }
            }

        }
        protected void gv_administradores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HyperLink nombreProyecto = new HyperLink();
            nombreProyecto = ((HyperLink)e.Row.Cells[1].FindControl("hl_nombre"));
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["codGrupo"]))
                    nombreProyecto.NavigateUrl = nombreProyecto.NavigateUrl + "&codGrupo=" + Request.QueryString["codGrupo"].ToString();
            }
        }
        protected void gv_administradores_PageIndexChanged(object sender, GridViewPageEventArgs e)
        {

        }

        protected void lds_Administradores_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["CodGrupo"]))
            {
                grupos1 = Request.QueryString["codGrupo"];
                grupos = grupos1.Split(',');
            }
            var usuarios = from c in consultas.Db.Contactos
                           from gc in consultas.Db.GrupoContactos
                           from g in consultas.Db.Grupos
                           orderby c.Nombres ascending
                           where c.Inactivo == false & c.Id_Contacto == gc.CodContacto &
                                 g.Id_Grupo == gc.CodGrupo & grupos.Contains(gc.CodGrupo.ToString())
                           select new
                           {
                               Id_contacto = c.Id_Contacto,
                               Nombres = c.Nombres + ' ' + c.Apellidos,
                               Email = c.Email,
                               codgrupo = gc.CodGrupo,
                               nomgrupo = g.NomGrupo
                           };
            e.Arguments.TotalRowCount = usuarios.Count();
            if (e.Arguments.TotalRowCount == 0)
            {
                Lbl_Resultados.Visible = true;
                Lbl_Resultados.Text = "No tiene actividades  para este rango de fechas";
            }
            else
            {
                Lbl_Resultados.Visible = false;
            }
            e.Result = usuarios.ToList();
        }
        protected void btn_Inactivar_click(object sender, CommandEventArgs e)
        {
            GrupoContacto Grupocontacto = new GrupoContacto()
            {
                CodGrupo = Int32.Parse(ddl_perfil.SelectedValue),
                CodContacto = Int32.Parse(e.CommandArgument.ToString())
            };
            consultas.Db.GrupoContactos.Attach(Grupocontacto);
            consultas.Db.GrupoContactos.DeleteOnSubmit(Grupocontacto);
            consultas.Db.SubmitChanges();
            var query = (from c in consultas.Db.Contactos
                         where c.Id_Contacto == Int32.Parse(e.CommandArgument.ToString())
                         select c);
            foreach (Contacto c in query)
            {
                c.Inactivo = true;
            }
            consultas.Db.SubmitChanges();
            RequiredFieldValidator1.Enabled = false;
            RequiredFieldValidator2.Enabled = false;
            RequiredFieldValidator3.Enabled = false;
            RequiredFieldValidator4.Enabled = false;
            RequiredFieldValidator8.Enabled = false;
            RequiredFieldValidator9.Enabled = false;
            RegularExpressionValidator1fax.Enabled = false;
            gv_administradores.DataBind();
        }
        protected void btn_crearActualizar_onclick(object sender, EventArgs e)
        {
            int codigoContacto = 0;
            if (!String.IsNullOrEmpty(Request.QueryString["CodGrupo"]))
            {
                grupos1 = Request.QueryString["codGrupo"];
                grupos = grupos1.Split(',');
                Menu_Data(grupos1);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["CodContacto"]))
            {
                codigoContacto = Int32.Parse(Request.QueryString["CodContacto"]);
            }
            string accion = Request.QueryString["Accion"].ToString();
            switch (accion)
            {
                case "Crear":
                    void_CrearDatos(grupos, codigoContacto);
                    break;
                case "Editar":
                    void_ModificarDatos(grupos, codigoContacto);
                    break;
                default: break;
            }
        }
        protected void gv_administradores_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.HasControls())
                    {
                        LinkButton lnk = (LinkButton)tc.Controls[0];
                        if (lnk != null && gv_administradores.SortExpression == lnk.CommandArgument)
                        {
                            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                            img.ImageUrl = "/Images/ImgFlechaOrden" + (gv_administradores.SortDirection == SortDirection.Ascending ? "Up" : "Down") + ".gif";
                            tc.Controls.Add(new LiteralControl(" "));
                            tc.Controls.Add(img);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 27/06/2014.
        /// De acuerdo a la pantalla activa y el usuario a crear, redirigir al usuario según el rol.
        /// </summary>
        private string Menu_Data(string valor)
        {
            try
            {
                if (valor == "8") //Call Center
                {
                    Session["URL_redirect"] = "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=8";
                    return "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=8";
                }
                if (valor == "2,3") //Administrador
                {
                    Session["URL_redirect"] = "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=2,3";
                    return "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=2,3";
                }
                if (valor == "12") //Gerente Interventor
                {
                    Session["URL_redirect"] = "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=12";
                    return "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=12";
                }
                if (valor == "9") //Gerente Evaluador
                {
                    Session["URL_redirect"] = "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=9";
                    return "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=9";
                }
                else //Cualquier dato.
                {
                    Session["URL_redirect"] = "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=2,3";
                    return "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=2,3";
                }
            }
            catch
            {
                /*Cualquier dato*/
                Session["URL_redirect"] = "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=2,3";
                return "~/FONADE/AdministrarPerfiles/AdministrarUsuarios.aspx?codGrupo=2,3";
            }
        }

        /// <summary>
        /// RowCommand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_administradores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "eliminar":
                    #region Eliminar usuario seleccionado.
                    try
                    {
                        GrupoContacto Grupocontacto = new GrupoContacto()
                            {
                                CodGrupo = Int32.Parse(ddl_perfil.SelectedValue),
                                CodContacto = Int32.Parse(e.CommandArgument.ToString())
                            };
                        consultas.Db.GrupoContactos.Attach(Grupocontacto);
                        consultas.Db.GrupoContactos.DeleteOnSubmit(Grupocontacto);
                        consultas.Db.SubmitChanges();
                        var query = (from c in consultas.Db.Contactos
                                     where c.Id_Contacto == Int32.Parse(e.CommandArgument.ToString())
                                     select c);
                        foreach (Contacto c in query)
                        {
                            c.Inactivo = true;
                        }
                        consultas.Db.SubmitChanges();
                        RequiredFieldValidator1.Enabled = false;
                        RequiredFieldValidator2.Enabled = false;
                        RequiredFieldValidator3.Enabled = false;
                        RequiredFieldValidator4.Enabled = false;
                        RequiredFieldValidator8.Enabled = false;
                        RequiredFieldValidator9.Enabled = false;
                        RegularExpressionValidator1fax.Enabled = false;
                        gv_administradores.DataBind();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Usuario eliminado correctamente.')", true);
                    }
                    catch
                    { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No se pudo eliminar el usuario seleccionado.')", true); }

                    #endregion
                    break;
                default:
                    break;
            }
        }
    }
}