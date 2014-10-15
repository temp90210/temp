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
using Datos;


namespace Fonade.FONADE.interventoria
{
    public partial class CatalogoAmbito : Negocio.Base_Page
    {
        protected int CodAmbito;
       
        protected void Page_Load(object sender, EventArgs e)
        {

            CodAmbito = Convert.ToInt32(Session["id_TipoAmbito"]);
            if (!IsPostBack)
            {
            if(CodAmbito==0)
                 {

                     //lbl_Titulo.Text = void_establecerTitulo("Nuevo Usuario Coordinador de Interventoria");
                    pnlPrincipal.Visible = true;

                     ValidarAmbito();
                     //llenarTipoAmbito(ddlAmbito);
                 }
            else
            {
               
                    
                    PanelModificar.Visible = false;
                    pnlPrincipal.Visible = true;
                    ValidarAmbito();
                    llenarModificacion();
                  
            }
            }

            }
        
        private void ValidarAmbito()
        {
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
            }

            try
            {


                var dtAmbito = consultas.ObtenerDataTable("MD_listarTipoAmbitos");

                if (dtAmbito.Rows.Count != 0)
                {
                    Session["dtAmbito"] = dtAmbito;
                    gvcAmbito.DataSource = dtAmbito;
                    gvcAmbito.DataBind();
                }
                else
                {
                    gvcAmbito.DataSource = dtAmbito;
                    gvcAmbito.DataBind();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected void gvcAmbitoPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvcAmbito.PageIndex = e.NewPageIndex;
            gvcAmbito.DataSource = Session["dtAmbito"];
            gvcAmbito.DataBind();
        }

        protected void gvcAmbitoRowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName.ToString())
            {


                case "editacontacto":
                    Session["id_TipoAmbito"] = e.CommandArgument.ToString();
                    CodAmbito = Convert.ToInt32(Session["id_TipoAmbito"]);
                    llenarModificacion();
                    
                    PnlActualizar.Visible = true;
                    PanelModificar.Visible = false;
                    pnlPrincipal.Visible = false;
                    break;
                  
            }

            
        }

        protected void gvcAmbitoSorting(object sender, GridViewSortEventArgs e)
        {

        }

        protected void lbtn_crearAmbito_Click(object sender, EventArgs e)
        {

            PanelModificar.Visible = true;
            pnlPrincipal.Visible = false;
        }

        protected void ibtn_crearAmbito_Click(object sender, ImageClickEventArgs e)
        {
            PanelModificar.Visible = true;
            validarInserUpdate("Create",0,txt_Nombre.Text);
         
        }


      
        public void Guardar()
        {
         

        }
        protected void insertupdateAmbito(string caso, int id_Ambito, string nomTipoAmbito)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_InsertUpdateTipoAmbito", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nomTipoAmbito", nomTipoAmbito);
            cmd.Parameters.AddWithValue("@IdTipoAmbito ", id_Ambito);
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
                var querysql = (from x in consultas.Db.TipoAmbitos
                                where x.NomTipoAmbito == nomTipoAmbito
                                
                                select new
                                {
                                    x.Id_TipoAmbito

                                }).FirstOrDefault();

                
               ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Creado exitosamente!'); window.location.href('CatalogoTipoAmbito.aspx');", true);

            }
           
            else
            {

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Modificado exitosamente!'); window.location.href('CatalogoTipoAmbito.aspx');", true);
            }
        }

 

        protected void btn_actualizar_Click(object sender, EventArgs e)
        {
             validarInserUpdate("Create",0,txt_Nombre.Text);
             


           
             
        }
        protected void validarInserUpdate(string caso,int IdAmbito, string nomTipoAmbito)
        {
            if (IdAmbito == 0)
            {

               var querysql = (from x in consultas.Db.TipoAmbitos
                              where x.NomTipoAmbito == nomTipoAmbito
                              
                               select new { x }).Count();
                     
                 if (querysql == 0)
                 {

                      insertupdateAmbito(caso, 0,nomTipoAmbito);
                    
                      ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Creado Exitosamente')", true);
                      ValidarAmbito();
                
                      pnlPrincipal.Visible = true;
                      PanelModificar.Visible = false;
                      PnlActualizar.Visible = false;
                     

                 }

                 else
                     {
                         ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El codigo del Usuario ya se encuentra registrado')", true);
                     }
                            
        

                    

                        
                    
            
          }

            else
            {
                var sql = (from x in consultas.Db.TipoAmbitos
                           where x.Id_TipoAmbito !=IdAmbito
                          && x.NomTipoAmbito == nomTipoAmbito
                           select new { x }).Count();

                if (sql == 0)
                {
                    var query2 = (from x2 in consultas.Db.TipoAmbitos
                                  where x2.Id_TipoAmbito != IdAmbito
                                  && x2.Id_TipoAmbito == Convert.ToInt32(IdAmbito)
                                 
                                  select new { x2 }).Count();

                    if (query2 == 0)
                    {
                        insertupdateAmbito(caso, IdAmbito,nomTipoAmbito);
                        ValidarAmbito();
                        pnlPrincipal.Visible = true;
                        PanelModificar.Visible = false;
                        PnlActualizar.Visible = false;
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El codigo de Usuario ya se encuentra registrado')", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Ya existe un usuario con ese correo electrónico!')", true);
                }
            }
        }
        protected void llenarModificacion()
        {
            try
            {
                var query = (from x in consultas.Db.TipoAmbitos
                             where x.Id_TipoAmbito == CodAmbito
                                     select new { x}).FirstOrDefault();

             txtNomUpd.Text = query.x.NomTipoAmbito;
      
               
                
            }
            catch (Exception exc)
            { }




        }

        protected void btn_update_Click(object sender, EventArgs e)
        {
            validarInserUpdate("Update",CodAmbito,txtNomUpd.Text);

        }

        protected void BtnBorrar_Click(object sender, EventArgs e)
        {
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
            }

            try
            {
                consultas.Parameters = null;

                consultas.Parameters = new[]
                                           {
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@caso",
                                                       Value = "Delete"
                                                   },
                                                   new SqlParameter
                                                   {
                                                       ParameterName= "@IdTipoAmbito",
                                                       Value= CodAmbito
                                                   }
                                           };

                var dtAmbito = consultas.ObtenerDataTable("MD_DeleteTipoAmbito");

                if (dtAmbito.Rows.Count != 0)
                {
                    Session["Ambito"] = dtAmbito;
                    gvcAmbito.DataSource = dtAmbito;
                    gvcAmbito.DataBind();
                }
                else
                {
                    gvcAmbito.DataSource = dtAmbito;
                    gvcAmbito.DataBind();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Registro Borrado con exito')", true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

     

    }


}
 