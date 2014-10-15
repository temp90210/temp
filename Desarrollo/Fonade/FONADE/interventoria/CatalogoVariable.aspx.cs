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
    public partial class CatalogoVariable : Negocio.Base_Page
    {
        protected int CodVariable;
        /// <summary>
        /// Variable declarada en el RowCommand, para establecer visibilidad de páneles.
        /// </summary>
        String accion;

        protected void Page_Load(object sender, EventArgs e)
        {
            CodVariable = Convert.ToInt32(Session["id_Variable"]);
            accion = Session["amb_accion"] != null && !string.IsNullOrEmpty(Session["amb_accion"].ToString()) ? Session["amb_accion"].ToString() : "";

            if (!IsPostBack)
            {
                llenarTipoVariable(ddlTipoVariable);
                llenarTipoVariable(ddlTipo);

                if (CodVariable == 0 && accion == "")
                {
                    if (CodVariable != 0 && accion != "")
                    {
                        pnlPrincipal.Visible = false;
                        PanelModificar.Visible = false;
                        PnlActualizar.Visible = true;
                    }
                    else
                    {
                        pnlPrincipal.Visible = true;
                        PanelModificar.Visible = false;
                        PnlActualizar.Visible = false;
                        ValidarVariable();
                        Session["amb_accion"] = "";
                        CodVariable = 0;
                        lbltitulo.Text = "VARIABLE";
                    }
                }
                else
                {
                    pnlPrincipal.Visible = true;
                    PanelModificar.Visible = false;
                    PnlActualizar.Visible = false;
                    ValidarVariable();
                    Session["amb_accion"] = "";
                    CodVariable = 0;
                    lbltitulo.Text = "VARIABLE";
                }
            }

            Session["amb_accion"] = "";
            lbltitulo.Text = "VARIABLE";
        }

        private void ValidarVariable()
        {
            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
            }

            try
            {


                //var dtAmbito = consultas.ObtenerDataTable("MD_listarVariable");
                var dtAmbito = consultas.ObtenerDataTable(" SELECT * FROM Variable ", "text");

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
                    //Establecer valores en variable de sesión y demás campos.
                    string[] palabras = e.CommandArgument.ToString().Split(';');
                    Session["id_Variable"] = palabras[0];
                    if (!String.IsNullOrEmpty(palabras[1])) { ddlTipo.SelectedValue = palabras[1]; }
                    Session["amb_accion"] = "EDITAR";
                    CodVariable = Convert.ToInt32(Session["id_Variable"]);
                    llenarModificacion();

                    //Establecer título.
                    lbl_id_variable.Text = "Id: " + CodVariable.ToString();
                    lbltitulo.Text = "EDITAR";

                    //Ocultar y mostrar ciertos páneles.
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
            lbltitulo.Text = "NUEVO";
            pnlPrincipal.Visible = false;
            PanelModificar.Visible = true;
        }

        protected void ibtn_crearAmbito_Click(object sender, ImageClickEventArgs e)
        {
            lbltitulo.Text = "NUEVO";
            pnlPrincipal.Visible = false;
            PanelModificar.Visible = true;
        }

        protected void llenarTipoVariable(DropDownList lista)
        {
            string SQL = "select id_TipoVariable, nomTipoVariable from TipoVariable";

            DataTable dt = consultas.ObtenerDataTable(SQL, "text");

            lista.DataSource = dt;
            lista.DataTextField = "nomTipoVariable";
            lista.DataValueField = "id_TipoVariable";
            lista.DataBind();
        }

        public void Guardar()
        {


        }

        protected void insertupdateAmbito(string caso, int id_Variable, string nomTipoVariable)
        {
            #region COMENTADO.
            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            //SqlCommand cmd = new SqlCommand("MD_InsertUpdateVariable", con);

            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@nomVariable", nomTipoVariable);
            //cmd.Parameters.AddWithValue("@IdVariable ", id_Variable);
            //cmd.Parameters.AddWithValue("@caso", caso);


            //SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
            //con.Open();
            //cmd2.ExecuteNonQuery();
            //cmd.ExecuteNonQuery();
            //con.Close();
            //con.Dispose();
            //cmd2.Dispose();
            //cmd.Dispose();
            //if (caso == "Create")
            //{
            //    var querysql = (from x in consultas.Db.Variables
            //                    where x.NomVariable == nomTipoVariable

            //                    select new
            //                    {
            //                        x.id_Variable

            //                    }).FirstOrDefault();


            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Creado exitosamente!'); window.location.href('CatalogoVariable.aspx');", true);
            //    txt_Nombre.Text = "";
            //}

            //else
            //{
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Modificado exitosamente!'); window.location.href('CatalogoVariable.aspx');", true);
            //    txtNomUpd.Text = "";
            //} 
            #endregion

            #region Nueva versión.

            switch (caso)
            {
                case "Create":
                    #region create.
                    try
                    {
                        //NEW RESULTS:
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                        SqlCommand cmd = new SqlCommand();
                        //cmd = new SqlCommand(" Update Variable set NomVariable ='" + nomTipoVariable + "', CodTipoVariable = "+id_Variable+ " WHERE Id_Variable = " + id_Variable, con);
                        cmd = new SqlCommand(" INSERT INTO Variale (NomVariable, CodTipoVariable) VALUES ('" + nomTipoVariable + "', " + this.ddlTipoVariable.SelectedValue + ")", con);

                        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                        cmd.Dispose();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Creado exitosamente!'); window.location.href('CatalogoVariable.aspx');", true);
                        txt_Nombre.Text = "";
                    }
                    catch
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El Registro No pudo ser creado ')", true);
                        return;
                    }
                    break;
                    #endregion
                    break;
                case "Update":
                    #region update.
                    try
                    {
                        //NEW RESULTS:
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                        SqlCommand cmd = new SqlCommand();
                        cmd = new SqlCommand(" Update Variable set NomVariable ='" + nomTipoVariable + "', CodTipoVariable = " + ddlTipo.SelectedValue + " WHERE Id_Variable = " + id_Variable, con);

                        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        con.Close();
                        con.Dispose();
                        cmd.Dispose();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Modificado exitosamente!'); window.location.href('CatalogoVariable.aspx');", true);
                        txtNomUpd.Text = "";
                    }
                    catch
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El Registro No pudo ser modificado ')", true);
                        return;
                    }
                    break;
                    #endregion
                default:
                    break;
            }

            #endregion
        }

        protected void btn_actualizar_Click(object sender, EventArgs e)
        {
            if (txt_Nombre.Text.Trim() == "")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe digitar el nombre de la variable.')", true);
                return;
            }
            if (ddlTipoVariable.SelectedValue == "" || ddlTipo.SelectedValue == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe Seleccionar el tipo de la variable.')", true);
                return;
            }
            else { validarInserUpdate("Create", 0, txt_Nombre.Text); lbltitulo.Text = "VARIABLE"; }
        }

        protected void validarInserUpdate(string caso, int IdVariable, string nomTipoVariable)
        {
            if (IdVariable == 0)
            {

                var querysql = (from x in consultas.Db.Variables
                                where x.NomVariable == nomTipoVariable
                                /*select new
                                 {
                                     x.NomTipoAmbito

                                }).FirstOrDefault();*/
                                select new { x }).Count();

                if (querysql == 0)
                {

                    insertupdateAmbito(caso, 0, nomTipoVariable);

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Creado Exitosamente')", true);
                    ValidarVariable();
                    txt_Nombre.Text = "";

                    pnlPrincipal.Visible = true;
                    lbltitulo.Text = "VARIABLE";
                    PanelModificar.Visible = false;
                    PnlActualizar.Visible = false;
                }

                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La Variable ya se encuentra Registrada')", true);
                }

            }

            else
            {
                var sql = (from x in consultas.Db.Variables
                           where x.id_Variable != IdVariable
                          && x.NomVariable == nomTipoVariable
                           select new { x }).Count();

                if (sql == 0)
                {
                    var query2 = (from x2 in consultas.Db.Variables
                                  where x2.id_Variable != IdVariable
                                  && x2.id_Variable == Convert.ToInt32(IdVariable)

                                  select new { x2 }).Count();

                    if (query2 == 0)
                    {
                        insertupdateAmbito(caso, IdVariable, nomTipoVariable);
                        ValidarVariable();
                        pnlPrincipal.Visible = true;
                        PanelModificar.Visible = false;
                        PnlActualizar.Visible = false;
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La Variable ya se encuentra registrado')", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Ya existe Una Variable con ese nombre!')", true);
                }
            }
        }

        protected void llenarModificacion()
        {
            try
            {
                var query = (from x in consultas.Db.Variables
                             where x.id_Variable == CodVariable
                             select new { x }).FirstOrDefault();

                txtNomUpd.Text = query.x.NomVariable;
            }
            catch { }
        }

        /// <summary>
        /// Actualizar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_update_Click(object sender, EventArgs e)
        {
            if (txtNomUpd.Text.Trim() == "")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe digitar el nombre de la variable.')", true);
                return;
            }
            if (ddlTipo.SelectedValue == "" || ddlTipo.SelectedValue == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Debe Seleccionar el tipo de variable.')", true);
                return;
            }
            else
            { validarInserUpdate("Update", CodVariable, txtNomUpd.Text); lbltitulo.Text = "VARIABLE"; }
        }

        /// <summary>
        /// Eliminar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnBorrar_Click(object sender, EventArgs e)
        {
            if (usuario.CodGrupo == Constantes.CONST_Interventor) { }
            try
            {
                #region COMENTADO.
                //consultas.Parameters = null;

                //consultas.Parameters = new[]
                //                           {
                //                               new SqlParameter
                //                                   {
                //                                       ParameterName = "@caso",
                //                                       Value = "Delete"
                //                                   },
                //                                   new SqlParameter
                //                                   {
                //                                       ParameterName= "@IdVariable",
                //                                       Value= CodVariable
                //                                   }
                //                           };

                //var dtAmbito = consultas.ObtenerDataTable("MD_DeleteVariable");

                //if (dtAmbito.Rows.Count != 0)
                //{
                //    Session["Ambito"] = dtAmbito;
                //    gvcAmbito.DataSource = dtAmbito;
                //    gvcAmbito.DataBind();
                //}
                //else
                //{
                //    gvcAmbito.DataSource = dtAmbito;
                //    gvcAmbito.DataBind();
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Registro Borrado con exito')", true);
                //    lbltitulo.Text = "VARIABLE";
                //} 
                #endregion

                //Eliminar.
                try
                {
                    //NEW RESULTS:
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand(" DELETE FROM Variable WHERE Id_Variable = " + CodVariable, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Registro Borrado con exito')", true);
                    ValidarVariable();
                    pnlPrincipal.Visible = true;
                    PanelModificar.Visible = false;
                    PnlActualizar.Visible = false;
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El Registro No pudo ser Borrado ')", true);
                    return;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El Registro No pudo ser Borrado ')", true);
            }
        }
    }
}