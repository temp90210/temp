using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.FONADE.AdministrarPerfiles.Convenios
{
    public partial class CatalogoConvenios : Negocio.Base_Page
    {
        string crearAdmin = "Crear Administrador";
        string crearGerenteEvaluador = "Crear GerenteEvaluador";
        string crearCallCenter = "Crear Call Center";
        string crearGerenteInterventor = "Crear Gerente Interventor";
        string crearPerfilFiduciario = "Crear Perfil Fiduciario";
        String grupos1;
        int idusuario;//id del usuario seleccionado en la grilla
        public String[] grupos = { "0" };
        string[] codgrupousuario;
        protected void void_show(string texto, bool mostrar)
        {
            lbl_popup.Visible = mostrar;
            lbl_popup.Text = texto;
            mpe1.Enabled = mostrar;
            mpe1.Show();
        }
        protected void void_ModificarDatos(string[] codgrupo, int Grupocontacto)
        {
            if (Request.QueryString["CodCriterio"] != null)
            {
                int codigoContacto = Int32.Parse(Request.QueryString["CodCriterio"]);
                var query = (from c in consultas.Db.Convenios
                             where c.Id_Convenio == codigoContacto
                             select c);
                foreach (Convenio c in query)
                {
                    c.Nomconvenio = tb_Convenio.Text;
                    c.FechaFin = Calendarextender1.SelectedDate;
                    c.FechaFin =  calExtender2.SelectedDate;
                    c.Descripcion =  tb_Descripcion.Text;
                    c.CodcontactoFiduciaria =  Int32.Parse(ddl_fiduciaria.SelectedValue);
                }
                // Submit the changes to the database. 
                try
                {
                    consultas.Db.SubmitChanges();
                    void_show("La información del usuario ha sido actualizado correctamente", true);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(e);
                    // Provide for exceptions.
                }

            }
        }
        protected void void_CrearDatos(string[] codgrupo, int codusuario)
        {
            Convenio Criterio = new Convenio()
            {
                Nomconvenio =  tb_Convenio.Text,
                Fechainicio =  Calendarextender1.SelectedDate,
                FechaFin =  calExtender2.SelectedDate,
                Descripcion = tb_Descripcion.Text,
                CodcontactoFiduciaria =  Int32.Parse(ddl_fiduciaria.SelectedValue)
            };
            try
            {
                    consultas.Db.Convenios.InsertOnSubmit(Criterio);
                    consultas.Db.SubmitChanges();
                    Lbl_Resultados.Visible = true;
                    Lbl_Resultados.Text = "el Criterio " + tb_Convenio.Text + " ha sido agregado";
               }
            catch (Exception ex)
            {}
        }
        protected  void void_HideControls(string accion, string[] codgrupo)
        {
            switch (accion)
            {
                case "Crear":
                    calExtender2.SelectedDate = DateTime.Today;
                    Calendarextender1.SelectedDate = DateTime.Today;
                    btn_crearActualizar.Text = "Crear";
                    break;
                case "Editar":
                    btn_crearActualizar.Text = "Actualizar";
                    break;
                default: break;
            }

        }
        protected void void_traerDatos(string[] codgrupo, int codusuario)
        {
            if (Request.QueryString["CodCriterio"] != null)
            {
                int codigoContacto = Int32.Parse(Request.QueryString["CodCriterio"]);
                var query = (from c in consultas.Db.Convenios
                             where c.Id_Convenio == codigoContacto
                             select c).FirstOrDefault();

                calExtender2.SelectedDate = query.FechaFin;
                Calendarextender1.SelectedDate = query.Fechainicio;
                tb_Descripcion.Text = query.Descripcion;
                tb_Convenio.Text = query.Nomconvenio;
                ddl_fiduciaria.SelectedValue = query.CodcontactoFiduciaria.ToString();
            }
        }
        protected void void_ObtenerParametros()
        {
            if (!String.IsNullOrEmpty(Request.QueryString["Accion"]))
            {
                pnl_Convenios.Visible = false;
                pnl_crearEditar.Visible = true;
                AgregarConvenio.Visible = false;
                if (!String.IsNullOrEmpty(Request.QueryString["Accion"]))
                {
                    if (Request.QueryString["Accion"].ToString() == "Editar")
                    {
                        AgregarConvenio.Text = "Actualizar";
                        idusuario = Int32.Parse(Request.QueryString["CodCriterio"]);
                        void_traerDatos(codgrupousuario, idusuario);//trae la información segun el usuario y el grupo al cual 
                    }
                    if (Request.QueryString["Accion"].ToString() == "Crear")
                    {
                        AgregarConvenio.Text = "Crear";
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string accion ="";
                var factor = from c in consultas.Db.Contactos
                             from gc in consultas.Db.GrupoContactos
                             where c.Id_Contacto == gc.CodContacto & gc.CodGrupo == Constantes.CONST_Perfil_Fiduciario & c.Inactivo ==false
                             select c;
                ddl_fiduciaria.DataSource = factor;
                ddl_fiduciaria.DataTextField = "Email";
                ddl_fiduciaria.DataValueField = "id_contacto";
                ddl_fiduciaria.DataBind();
                int codigoContacto = 0;
                if (!String.IsNullOrEmpty(Request.QueryString["CodCriterio"]))
                {
                    codigoContacto = Int32.Parse(Request.QueryString["CodCriterio"]);
                }
                if (!String.IsNullOrEmpty(Request.QueryString["Accion"]))
                {
                     accion = Request.QueryString["Accion"].ToString();
                     void_HideControls(accion, grupos);
                };
                
                pnl_crearEditar.Visible = false;
                lbl_Titulo.Text= void_establecerTitulo(grupos, accion,"CONVENIO");
                void_ObtenerParametros();
           }
        }
        protected void gv_Convenios_DataBound(object sender, EventArgs e)
        {}
        protected void gv_Convenios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string Conteo="";
            ImageButton imbtton = new ImageButton();
            imbtton = ((ImageButton)e.Row.Cells[0].FindControl("btn_Inactivar"));
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Conteo == "0")
                {
                    imbtton.Visible = true;
                }
                else
                {
                    imbtton.Visible = false;
                }
            }
        }
        protected void gv_Convenios_PageIndexChanged(object sender, GridViewPageEventArgs e)
        {}
        protected void lds_Convenios_Selecting(object sender, LinqDataSourceSelectEventArgs e)      
        {
            var convenios = (from cv in consultas.Db.Convenios select cv into convenio
                             from c in consultas.Db.Contactos
                             where c.Id_Contacto == convenio.CodcontactoFiduciaria 
                             select new
                             {
                                 Id_convenio = convenio.Id_Convenio,
                                 nomconvenio = convenio.Nomconvenio,
                                 FechaInicio=convenio.Fechainicio,
                                 FechaFin=convenio.FechaFin,
                                 EmailFiduciaria = c.Email                               
                                 
                             });
            e.Arguments.TotalRowCount = convenios.Count();
            if (e.Arguments.TotalRowCount == 0)
            {
                Lbl_Resultados.Visible = true;
                Lbl_Resultados.Text = "No tiene actividades  para este rango de fechas";
            }
            else
            {
                Lbl_Resultados.Visible = false;
            }

            e.Result = convenios.ToList();
        }
        protected void btn_Inactivar_click(object sender, CommandEventArgs e)
        {

            var criterio = (from c in consultas.Db.CriterioPriorizacions
                            where c.Id_Criterio == short.Parse(e.CommandArgument.ToString())
                            select c).FirstOrDefault();
            consultas.Db.CriterioPriorizacions.DeleteOnSubmit(criterio);
            consultas.Db.SubmitChanges();
            // deshabilitar Validadores
            RequiredFieldValidator1.Enabled = false;
            RequiredFieldValidator2.Enabled = false;
            RequiredFieldValidator6.Enabled = false;
            //bindear dataview            
            gv_Convenios.DataBind();
        }
        protected void btn_crearActualizar_onclick(object sender, EventArgs e)
        {
            int codigoContacto = 0;
            if (!String.IsNullOrEmpty(Request.QueryString["CodCriterio"]))
            {
                codigoContacto = Int32.Parse(Request.QueryString["CodCriterio"]);
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
        protected void gv_Convenios_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.HasControls())
                    {
                        // buscar el link del header
                        LinkButton lnk = (LinkButton)tc.Controls[0];
                        if (lnk != null && gv_Convenios.SortExpression == lnk.CommandArgument)
                        {
                            // inicializar nueva imagen
                            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                            // url de la imagen dinamicamente
                            img.ImageUrl = "/Images/ImgFlechaOrden" + (gv_Convenios.SortDirection == SortDirection.Ascending ? "Up" : "Down") + ".gif";
                            // a ñadir el espacio de la imagen
                            tc.Controls.Add(new LiteralControl(" "));
                            tc.Controls.Add(img);

                        }
                    }
                }
            }
        }
    }
}