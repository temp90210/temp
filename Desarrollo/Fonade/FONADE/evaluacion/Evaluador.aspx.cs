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
using Fonade.Account;
using LinqKit;
using AjaxControlToolkit;
using System.ComponentModel;

namespace Fonade.FONADE.evaluacion
{
    public partial class Evaluador : Negocio.Base_Page
    {
        protected int CodContacto;

        protected void Page_Load(object sender, EventArgs e)
        {
            CodContacto = Convert.ToInt32(Session["ContactoEvaluador"]);

            if (!IsPostBack)
            {
                if (CodContacto == 0)
                {
                    lbl_Titulo.Text = void_establecerTitulo("CREAR EVALUADOR");
                    PanelCrear.Visible = true;
                    llenarLista(cbl_listaSectorCrear);
                    llenarTipoIdentificacion(ddl_tidentificacionCrear);
                }
                else
                {
                    lbl_Titulo.Text = void_establecerTitulo("MODIFICAR EVALUADOR");
                    PanelModificar.Visible = true;
                    llenarLista(cbl_listaSectormod);
                    llenarTipoIdentificacion(ddl_tidentificacionmod);
                    llenarModificacion();
                }
            }
        }

        protected void llenarLista(CheckBoxList lista)
        {
            var query = from x in consultas.Db.Sectors
                         select new
                         {
                            idsector =  x.Id_Sector,
                            nombsector = x.NomSector
                         };
            lista.DataSource = query;
            lista.DataTextField = "nombsector";
            lista.DataValueField = "idsector";
            lista.DataBind();
        }

        protected void llenarTipoIdentificacion(DropDownList lista)
        {
            var query = (from x in consultas.Db.TipoIdentificacions
                         select new
                         {
                             id_tipo = x.Id_TipoIdentificacion,
                             nom_tipo = x.NomTipoIdentificacion,
                         });
            lista.DataSource = query;
            lista.DataTextField = "nom_tipo";
            lista.DataValueField = "id_tipo";
            lista.DataBind();
        }

        protected void btn_crear_Click(object sender, EventArgs e)
        {
            Boolean validacheck = false;
            foreach (ListItem item in cbl_listaSectorCrear.Items)
            {
                if (item.Selected)
                {
                    validacheck = true;
                }
            }
            if (!validacheck)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: Debe seleccionar por lo menos un sector!')", true);
            }
            else
            {
                validarInserUpdate("Create", 0, txt_nombreCrear.Text, txt_apellidosCrear.Text, txt_emailCrear.Text, ddl_tidentificacionCrear.SelectedValue, txt_nidentificacionCrear.Text, ddl_personaCrear.SelectedValue);
            }
        }

        protected void validarInserUpdate(string caso, int IdContacto, string nombre, string apellido, string email, string tipoidentificacion, string identificacion, string tipopersona)
        {
            if (IdContacto == 0)
            {
                var query = (from x in consultas.Db.Contactos
                             where x.Email == txt_emailCrear.Text
                             || (x.CodTipoIdentificacion == Convert.ToInt32(tipoidentificacion)
                                 && x.Identificacion == Convert.ToInt64(identificacion))
                             select new { x }).Count();
                if (query == 0)
                {

                    insertupdatecoordEval(caso, IdContacto, nombre, apellido, email, tipoidentificacion, identificacion, tipopersona);

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Ya existe un usuario con ese correo electrónico o documento de identificación!')", true);
                }
            }
            else
            {
                var query = (from x in consultas.Db.Contactos
                             where x.Id_Contacto != IdContacto
                             && x.Email == email
                             select new { x }).Count();

                if (query == 0)
                {
                    var query2 = (from x2 in consultas.Db.Contactos
                                  where x2.Id_Contacto != IdContacto
                                  && x2.CodTipoIdentificacion == Convert.ToInt32(tipoidentificacion)
                                  && x2.Identificacion == Convert.ToInt64(identificacion)
                                  select new { x2 }).Count();

                    if (query2 == 0)
                    {
                        insertupdatecoordEval(caso, IdContacto, nombre, apellido, email, tipoidentificacion, identificacion, tipopersona);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Ya existe un usuario con ese documento de identificación!')", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Ya existe un usuario con ese correo electrónico!')", true);
                }
            }
        }

        protected void insertupdatecoordEval(string caso, int IdContacto, string nombre, string apellido, string email, string tipoidentificacion, string identificacion, string tipopersona)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_InsertUpdateEvaluador", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nombres", nombre);
            cmd.Parameters.AddWithValue("@apellidos", apellido);
            cmd.Parameters.AddWithValue("@codtipoIdentificacion", Convert.ToInt32(tipoidentificacion));
            cmd.Parameters.AddWithValue("@identificacion", Convert.ToInt64(identificacion));
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@clave", GeneraClave());
            cmd.Parameters.AddWithValue("@CodGrupo", Constantes.CONST_Evaluador);
            cmd.Parameters.AddWithValue("@Id_Contacto", IdContacto);
            cmd.Parameters.AddWithValue("@persona", tipopersona);
            cmd.Parameters.AddWithValue("@caso", caso);
            SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
            con.Open();
            cmd2.ExecuteNonQuery();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
            cmd2.Dispose();
            cmd.Dispose();

            if (caso == "Create")
            {
                var querysql = (from x in consultas.Db.Contactos
                                where x.Email == email
                                select new
                                 {
                                     x.Id_Contacto
                                 }).FirstOrDefault();

                GuardarcheckLista(cbl_listaSectorCrear, querysql.Id_Contacto);

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Creado exitosamente!'); window.location = 'CatalogoEvaluador.aspx';", true);
            }
            else
            {
                eliminarSectores();
                GuardarcheckLista(cbl_listaSectormod, CodContacto);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Modificado exitosamente!'); window.location = 'CatalogoEvaluador.aspx';", true);
            }
        }

        protected void btn_modificar_Click(object sender, EventArgs e)
        {
            Boolean validacheck = false;
            foreach (ListItem item in cbl_listaSectormod.Items)
            {
                if (item.Selected)
                {
                    validacheck = true;
                }
            }
            if (!validacheck)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: Debe seleccionar por lo menos un sector!')", true);
            }
            else
            {
                validarInserUpdate("Update", CodContacto, txt_nombremod.Text, txt_apellidosmod.Text, txt_emailmod.Text, ddl_tidentificacionmod.SelectedValue, txt_nidentificacionmod.Text, ddl_personaMod.SelectedValue);
            }
        }

        protected void GuardarcheckLista(CheckBoxList lista, int codContactoEval)
        {


            foreach (ListItem item in lista.Items)
            {
                if (item.Selected)
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    SqlCommand cmd = new SqlCommand("MD_InsertSectorEvaluador", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CodEvaluador", codContactoEval);
                    cmd.Parameters.AddWithValue("@CodSector", Convert.ToInt32(item.Value));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
            }
        }

        protected void eliminarSectores()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_DeleteSectorEvaluador", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodEvaluador", CodContacto);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
            cmd.Dispose();
        }

        protected void llenarModificacion()
        {
            try
            {
                var queryContacto = (from x in consultas.Db.Contactos
                                     where x.Id_Contacto == CodContacto
                                     select new { x }).FirstOrDefault();

                txt_nombremod.Text = queryContacto.x.Nombres;
                txt_apellidosmod.Text = queryContacto.x.Apellidos;
                txt_emailmod.Text = queryContacto.x.Email;
                txt_nidentificacionmod.Text = queryContacto.x.Identificacion.ToString();
                ddl_tidentificacionmod.SelectedValue = queryContacto.x.TipoIdentificacion.ToString();
                lcargo.Text = queryContacto.x.Cargo;
                ltelefono.Text = queryContacto.x.Telefono;
                lfax.Text = queryContacto.x.Fax;
                lexperienciageneral.Text = queryContacto.x.Experiencia;
                lintereses.Text = queryContacto.x.Intereses;
                lHojaDeVida.Text = queryContacto.x.HojaVida;
            }
            catch (Exception)
            {}

            try
            {
                var querydemas = (from x in consultas.Db.Evaluadors
                                  from x2 in consultas.Db.Bancos
                                  where x.CodBanco == x2.Id_Banco
                                  && x.CodContacto == CodContacto
                                  select new
                                  {
                                      x.Cuenta,
                                      x.MaximoPlanes,
                                      x.ExperienciaPrincipal,
                                      x.Experienciasecundaria,
                                      x.Persona,
                                      x2.nomBanco
                                  }).FirstOrDefault();
                l_cuenta.Text = querydemas.Cuenta;
                l_numPlanes.Text = querydemas.MaximoPlanes.ToString();
                lexperienciaprincipal.Text = querydemas.ExperienciaPrincipal;
                lexperienciasecundario.Text = querydemas.Experienciasecundaria;
                ddl_personaMod.SelectedValue = querydemas.Persona;
                l_banco.Text = querydemas.nomBanco;

            }
            catch (Exception)
            {}

            try
            {
                var queryp = (from x in consultas.Db.EvaluadorSectors
                              from x2 in consultas.Db.Sectors
                              where x.CodSector == x2.Id_Sector
                              && x.Experiencia == "P"
                              && x.CodContacto == CodContacto
                              select new { x2.NomSector }).FirstOrDefault();
                lsectorprincipalmod.Text = queryp.NomSector;
            }
            catch (Exception)
            {}
            try
            {
                var queryp = (from x in consultas.Db.EvaluadorSectors
                              from x2 in consultas.Db.Sectors
                              where x.CodSector == x2.Id_Sector
                              && x.Experiencia == "S"
                              && x.CodContacto == CodContacto
                              select new { x2.NomSector }).FirstOrDefault();
                lsectorsecundariomod.Text = queryp.NomSector;
            }
            catch (Exception)
            { }

            try
            {
                var query = from x in consultas.Db.Sectors
                        from x2 in consultas.Db.EvaluadorSectors
                        where x.Id_Sector == x2.CodSector
                        && x2.CodContacto == CodContacto
                        && (x2.Experiencia == "A" || x2.Experiencia == null)
                        select new
                        {
                            x.Id_Sector,
                        };
                
                foreach (var q in query)
                {
                    foreach (ListItem item in cbl_listaSectormod.Items)
                    {
                        if (item.Value == q.Id_Sector.ToString())
                        {
                            item.Selected = true;
                        }
                    }
                }

            }
            catch (Exception)
            {}


        }

        protected void btn_vercontrator_Click(object sender, EventArgs e)
        {
            Session["ContactoEvaluador"] = CodContacto;
            Response.Redirect("CatalogoContratoEvaluador.aspx");
        }

    }
}