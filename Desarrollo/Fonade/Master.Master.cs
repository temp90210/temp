using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Runtime.Caching;
using Datos;
using Fonade.Negocio;
using Fonade.Account;
using System.Data;




namespace Fonade
{
    public partial class Master : System.Web.UI.MasterPage
    {
        protected Consultas consultas;
        protected FonadeUser usuario;

        protected override void OnLoad(EventArgs e)
        {
            try
            {

                usuario = (FonadeUser)Membership.GetUser(Page.User.Identity.Name, true);
                consultas = new Consultas();
            }
            catch
            {
                throw new Exception("the user session doesnt exist");
            };
            base.OnLoad(e);
        }

        void void_Menu()
        {
            //Inicializar variables.
            LinkButton link = new LinkButton();
            Image img = new Image();

            try
            {
                //Páginas.
                var menu = (from pg in consultas.Db.Pagina_Grupos
                            from p in consultas.Db.Paginas
                            where pg.Id_Grupo == usuario.CodGrupo & p.Id_Pagina == pg.Id_Pagina
                            orderby p.Orden
                            select (p)).ToList();

                //Proyecto del usuario
                var proyectousuario = (from pc in consultas.Db.ProyectoContactos
                                       where pc.Inactivo == false & pc.CodContacto == usuario.IdContacto
                                       select (pc)).FirstOrDefault();
                #region Anterior funcional.
                //int incr = 0;
                //foreach (Pagina pag in menu)
                //{
                //    link = new LinkButton();
                //    link.ID = "lnk_" + incr.ToString();
                //    link.ForeColor = System.Drawing.Color.Black;
                //    link.Command += new CommandEventHandler(DynamicCommand_Redirect);

                //    img = new Image();
                //    img.ID = "img_" + incr.ToString();
                //    img.ImageUrl = "Images/Img/Pxmenufather.png";

                //    if (pag.Id_Pagina == 21) { Session["CodProyecto"] = proyectousuario.CodProyecto.ToString(); pag.url_Pagina = pag.url_Pagina.TrimEnd(); }
                //    else { pag.url_Pagina = pag.url_Pagina.TrimEnd(); }

                //    //Item planes a acreditar segun condicion
                //    if (usuario.CodGrupo != 2)
                //    {
                //        var result = (from c in consultas.Db.Contactos
                //                      where c.flagAcreditador == true
                //                      & c.Id_Contacto == usuario.IdContacto
                //                      select c.Id_Contacto).Count();

                //        if (result > 0)
                //        {
                //            //Pagina pg = new Pagina();

                //            //pg.Id_Pagina = 3092;
                //            //pg.Titulo = "Mis Planes De Negocio A Acreditar";
                //            //pg.url_Pagina = "/Fonade/Interventoria/PlanesaAcreditar.aspx";
                //            //pg.Orden = 3092;

                //            //menu.Add(pg);


                //            link.Text = "Mis Planes De Negocio A Acreditar";
                //            link.CommandName = link.Text + ";" + pag.Id_Pagina;
                //            link.CommandArgument = "/Fonade/Interventoria/PlanesaAcreditar.aspx";
                //        }
                //        else
                //        {
                //            link.Text = pag.Titulo;
                //            link.CommandName = link.Text + ";" + pag.Id_Pagina;
                //            link.CommandArgument = pag.url_Pagina;
                //        }

                //        if (pag.Id_Pagina == 3) { }

                //        panelMenu.Controls.Add(link);
                //        panelMenu.Controls.Add(img);
                //    }

                //    if (menu.Count() == 0)
                //    {
                //        ASPxNavBar1.Visible = true;
                //        //gv_Menu.Visible = false;
                //        panelMenu.Visible = false;
                //    }
                //    else
                //    {
                //        //gv_Menu.Visible = true;
                //        panelMenu.Visible = true;
                //        ASPxNavBar1.Visible = false;
                //    };

                //    incr++;
                //} 
                #endregion

                foreach (Pagina p in menu)
                {
                    if (p.Id_Pagina == 21)
                    {
                        try
                        {
                            Session["CodProyecto"] = proyectousuario.CodProyecto.ToString();
                            p.url_Pagina = p.url_Pagina.TrimEnd();

                        }
                        catch { }
                    }
                    else
                    {
                        p.url_Pagina = p.url_Pagina.TrimEnd();
                    }
                }

                //string txtSQL = "SELECT ID_CONTACTO FROM CONTACTO WHERE FLAGACREDITADOR=1 AND ID_CONTACTO=" + usuario.IdContacto;

                //Item planes a acreditar segun condicion
                if (usuario.CodGrupo != 2)
                {

                    var result = (from c in consultas.Db.Contactos
                                  where c.flagAcreditador == true
                                  & c.Id_Contacto == usuario.IdContacto
                                  select c.Id_Contacto).Count();

                    if (result > 0)
                    {
                        Pagina pg = new Pagina();

                        pg.Id_Pagina = 3092;
                        pg.Titulo = "Mis Planes De Negocio A Acreditar";
                        pg.url_Pagina = "/Fonade/Interventoria/PlanesaAcreditar.aspx";
                        pg.Orden = 3092;

                        menu.Add(pg);
                    }
                }

                gv_Menu.DataSource = menu;
                gv_Menu.DataBind();

                if (menu.Count() == 0)
                {
                    ASPxNavBar1.Visible = true;
                    gv_Menu.Visible = false;
                }
                else
                {
                    gv_Menu.Visible = true;
                    ASPxNavBar1.Visible = false;
                };
            }
            catch (Exception)
            { }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                tb_Fecha.Text = DateTime.Today.ToShortDateString();
            }
            void_Menu();
        }

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void HeadLoginStatus_onloggedout(object sender, EventArgs e)
        {
            //Response.Cookies.Remove
            //HttpCookie httpcookie;
            //(httpcookie).Expires = DateTime.Now.AddMinutes(1);


            MemoryCache.Default.Dispose();


            Response.Cache.SetAllowResponseInBrowserHistory(true);


            try
            {
                foreach (var gall in Response.Cookies)
                {
                    Response.Cookies.Remove(gall.ToString());
                }
            }
            catch (InvalidOperationException) { }


            Session.Clear(); //this will clear session
            Session.Abandon();
            //this will Abandon session
            FormsAuthentication.SignOut();
            Response.Cache.SetExpires(DateTime.Now);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

        }

        protected void LoginStatus_LoggedOut(Object sender, System.EventArgs e)
        {// Session.Abandon();
        }

        protected void img_BuscarConsulta_Click(object sender, ImageClickEventArgs e)
        {
            Session["consultarMaster"] = txt_busqueda.Value;
            Response.Redirect("/FONADE/MiPerfil/Consultas.aspx");
        }

        /// <summary>
        /// Método para alternar si la opción seleccionada es "Agendar Tarea".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DynamicCommand_Redirect(Object sender, CommandEventArgs e)
        {
            string[] valores = e.CommandArgument.ToString().Split(';');

            //if (valores[1].ToString() == "3") { Session["Id_tareaRepeticion"] = null; }
            if (valores[1].ToString() == "3") { Session["Id_tareaRepeticion"] = null; }
            Response.Redirect(valores[0].ToString());
        }
    }
}