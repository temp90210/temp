#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>16 - 07 - 2014</Fecha>
// <Archivo>Convocatoria.cs</Archivo>

#endregion

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

namespace Fonade.FONADE.Convocatoria
{
    public partial class Convocatoria : Negocio.Base_Page
    {
        public int IdConvocatoria;
        public bool publicado;

        delegate string del(string x, string y, int z);

        protected void Page_Load(object sender, EventArgs e)
        {

            IdConvocatoria = Convert.ToInt32(Session["IdConvocatoria"]);
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("CONVOCATORIA");
                //txt_horafin.Text = DateTime.Now.ToString("hh:mm tt");
                llenarConvenios();

                if (IdConvocatoria == 0)
                {
                    panelBotonesCrear.Visible = true;
                    pnl_confirmacion.Visible = false;
                    pnlcriteriosselecion.Visible = false;
                }
                else
                {
                    pnl_confirmacion.Visible = true;
                    pnlcriteriosselecion.Visible = true;
                    llenarInfoConvocatoria(IdConvocatoria);
                    PanelUpdate.Visible = true;
                    PanelBotonesactualizar.Visible = true;
                    panelApertura.Visible = true;

                }
            }
        }

        protected void llenarConvenios()
        {
            var query = from c in consultas.Db.Convenios
                        select new
                        {
                            idConvenio = c.Id_Convenio,
                            nomConvenio = c.Nomconvenio,
                        };
            ddl_convenios.DataSource = query.ToList();
            ddl_convenios.DataTextField = "nomConvenio";
            ddl_convenios.DataValueField = "idConvenio";
            ddl_convenios.DataBind();
        }

        protected void btn_CrearConv_Click(object sender, EventArgs e)
        {
            string fechafin = Convert.ToDateTime(txt_fechaFin.Text).ToString("yyyy-MM-dd");
            //fechafin += " " + Convert.ToDateTime(txt_horafin.Text).ToString("HH:mm:ss");

            if (ddltiempo.SelectedValue.Equals("am"))
                fechafin += " " + ddlhora.SelectedValue + ":" + ddlminuto.SelectedValue + ":00";
            else
                fechafin += " " + (Convert.ToInt32(ddlhora.SelectedValue) + 12).ToString() + ":" + ddlminuto.SelectedValue + ":00";

            if (Convert.ToDateTime(txt_frechaInicio.Text) > Convert.ToDateTime(fechafin))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La fecha de inicio de convocatoria no puede ser mayor a la de finalización!')", true);
            }
            else
            {
                insert_update(txt_frechaInicio.Text, fechafin, txt_Presupuesto.Text, txt_valorminimo.Text, 0, Convert.ToInt32(ddl_convenios.SelectedValue), IdConvocatoria, "Create");
                // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('fecha final: " + fechafin + "!')", true);
            }
        }

        /// <summary>
        /// llenarInfoConvocatoria
        /// carga la informacion de session de la convocatoria solicitada
        /// </summary>
        /// <param name="IdConvoct"></param>
        protected void llenarInfoConvocatoria(int IdConvoct)
        {
            var query = (from conv in consultas.Db.Convocatorias
                         where conv.Id_Convocatoria == IdConvoct
                         select new
                         {
                             conv
                         }).FirstOrDefault();

            txt_nombre.Text = query.conv.NomConvocatoria;
            txt_descripcion.Text = query.conv.Descripcion;
            txt_encargo.Text = query.conv.encargofiduciario;
            txt_fechaFin.Text = query.conv.FechaFin.ToString("dd/MM/yyyy");
            txt_frechaInicio.Text = query.conv.FechaInicio.ToString("dd/MM/yyyy");
            string hora = query.conv.FechaFin.ToString("hh:mm tt");
            ddlhora.SelectedValue = int.Parse(hora.Substring(0, 2)).ToString();
            ddlminuto.SelectedValue = int.Parse(hora.Substring(3, 2)).ToString();
            ddltiempo.SelectedValue = hora.Substring(6, 4).Replace(".","");
            //txt_horafin.Text = query.conv.FechaFin.ToString("hh:mm tt");
            txt_Presupuesto.Text = query.conv.Presupuesto.ToString();
            txt_valorminimo.Text = query.conv.MinimoPorPlan.ToString();
            ddl_convenios.SelectedValue = query.conv.CodConvenio.ToString();
            Ch_publicado.Checked = (bool)query.conv.Publicado;

            #region desabilitar o activar ambitos segun publicacion
            lnk_adicionarconfirmacion.Visible = !(bool)query.conv.Publicado;
            lnkcriteriosseleccion.Visible = !(bool)query.conv.Publicado;
            foreach (GridViewRow gvr in gvr_confirmacion.Rows)
            {
                ((LinkButton)gvr.FindControl("lnkeditar")).Enabled = !(bool)query.conv.Publicado;
                ((LinkButton)gvr.FindControl("lnkeliminar")).Visible = !(bool)query.conv.Publicado;
            }
            foreach (GridViewRow gvr in gvr_criteriosseleccion.Rows)
            {
                ((LinkButton)gvr.FindControl("lnkeditar")).Enabled = !(bool)query.conv.Publicado;
                ((LinkButton)gvr.FindControl("lnkeliminar")).Visible = !(bool)query.conv.Publicado;
            }
            #endregion

            l_numeroapertura.Text = query.conv.Id_Convocatoria.ToString("D6");
            if ((bool)query.conv.Publicado == true)
            {
                txt_nombre.ReadOnly = true;
                txt_descripcion.ReadOnly = true;
                btnDate1.Visible = false;
                txt_valorminimo.ReadOnly = true;
                Ch_publicado.Enabled = false;
                btn_Proyectos.Enabled = true;
                publicado = true;
            }
            else
            {
                publicado = false;
            }
        }

        /// <summary>
        /// permite la insercion y/o la actualizacion
        /// de una convocatoria
        /// </summary>
        /// <param name="FechaInicioV"></param>
        /// <param name="FechaFinV"></param>
        /// <param name="PresupuestoV"></param>
        /// <param name="MinimoPorPlanV"></param>
        /// <param name="PublicadoV"></param>
        /// <param name="CodConvenioV"></param>
        /// <param name="idConvocatoriaV"></param>
        /// <param name="caso"></param>
        protected void insert_update(string FechaInicioV, string FechaFinV, string PresupuestoV, string MinimoPorPlanV, int PublicadoV, int CodConvenioV, int idConvocatoriaV, string caso)
        {
            Int64 presupuesto = Convert.ToInt64(PresupuestoV);
            Int64 valorminimo = Convert.ToInt64(MinimoPorPlanV);

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand("MD_Insert_Update_Convocatorias", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NomConvocatoriaV", txt_nombre.Text);
            cmd.Parameters.AddWithValue("@DescripcionV", txt_descripcion.Text);
            cmd.Parameters.AddWithValue("@FechaInicioV", Convert.ToDateTime(FechaInicioV).ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@FechaFinV", FechaFinV);
            cmd.Parameters.AddWithValue("@PresupuestoV", presupuesto);
            cmd.Parameters.AddWithValue("@MinimoPorPlanV", valorminimo);
            cmd.Parameters.AddWithValue("@PublicadoV", PublicadoV);
            cmd.Parameters.AddWithValue("@codContactoV", usuario.IdContacto);
            cmd.Parameters.AddWithValue("@EncargoFiduciarioV", txt_encargo.Text);
            cmd.Parameters.AddWithValue("@CodConvenioV", CodConvenioV);
            cmd.Parameters.AddWithValue("@idConvocatoriaV", idConvocatoriaV);
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
                var query = (from conv in consultas.Db.Convocatorias
                             select new
                             {
                                 conv
                             }).Max(x => x.conv.Id_Convocatoria);
                int IdConvoca = query;
                Session["IdConvocatoria"] = IdConvoca;
                Response.Redirect("Convocatoria.aspx");
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Actualizado exitosamente!')", true);
                Session["IdConvocatoria"] = idConvocatoriaV;
                Response.Redirect("Convocatoria.aspx");
            }
        }

        protected void btn_Proyectos_Click(object sender, EventArgs e)
        {
            Session["Id_ProyPorConvoct"] = IdConvocatoria;
            Response.Redirect("ProyectosPorConvocatoria.aspx");
        }

        protected void btn_actualizar_Click(object sender, EventArgs e)
        {
            string fechafin = Convert.ToDateTime(txt_fechaFin.Text).ToString("yyyy-MM-dd");
            if (ddltiempo.SelectedValue.Equals("am"))
                fechafin += " " + ddlhora.SelectedValue + ":" + ddlminuto.SelectedValue + ":00";
            else
                fechafin += " " + (Convert.ToInt32(ddlhora.SelectedValue) + 12).ToString() + ":" + ddlminuto.SelectedValue + ":00";
            string caso = "";
            if (publicado)
            {
                caso = "Update2";
            }
            else
            {
                caso = "Update1";
            }

            int publicar = 0;
            if (Ch_publicado.Checked == true)
            {
                publicar = 1;
            }

            insert_update(txt_frechaInicio.Text, fechafin, txt_Presupuesto.Text, txt_valorminimo.Text, publicar, Convert.ToInt32(ddl_convenios.SelectedValue), IdConvocatoria, caso);
        }

        protected void btn_Criterios_Click(object sender, EventArgs e)
        {
            Session["Id_ConvocatCriterios"] = IdConvocatoria;
            Response.Redirect("CatalogoConvocatoriaCriterioPriorizacion.aspx");
        }

        protected void gvreglasalarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToString() == "ModificarCondicion")
            {
                string NumCondicion = e.CommandArgument.ToString();
                Session["IdConvocatoriaRegla"] = IdConvocatoria;
                Session["condicionRegla"] = NumCondicion;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('ConvocatoriaReglaSalarios.aspx','_blank','width=685,height=157,toolbar=no, scrollbars=no, resizable=no');", true);
            }
            if (e.CommandName.ToString() == "EliminarCondicion")
            {
                string NumCondicion = e.CommandArgument.ToString();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_convocatoria_regla_salarios", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodConvocatoriaR", IdConvocatoria);
                cmd.Parameters.AddWithValue("@ExpresionLogicaR", "");
                cmd.Parameters.AddWithValue("@EmpleosGenerados1R", 0);
                cmd.Parameters.AddWithValue("@EmpleosGenerados2R", DBNull.Value);
                cmd.Parameters.AddWithValue("@SalariosAPrestarR", 0);
                cmd.Parameters.AddWithValue("@NoReglaR", Convert.ToInt32(NumCondicion));
                cmd.Parameters.AddWithValue("@caso", "Delete");
                SqlCommand cmd2 = new SqlCommand(UsuarioActual(), con);
                con.Open();
                cmd2.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd2.Dispose();
                cmd.Dispose();
                gvreglasalarios.DataBind();
                gvreglasalarios_Load(null, null);
            }
        }

        protected void gvreglasalarios_Load(object sender, EventArgs e)
        {
            foreach (GridViewRow grd_Row in this.gvreglasalarios.Rows)
            {
                if (((HiddenField)grd_Row.FindControl("hiddenNumero")).Value == gvreglasalarios.Rows.Count.ToString())
                {
                    ((ImageButton)grd_Row.FindControl("btnEliminarcondicion")).Visible = true;
                }
            }
        }

        protected void lds_regla_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var query = from P in consultas.VerReglasConvocatoria(IdConvocatoria)
                            select P;
                e.Result = query;
            }
            catch (Exception)
            { }
        }

        protected void lbtn_adicionarRegla_Click(object sender, EventArgs e)
        {
            if (gvreglasalarios.Rows.Count < 6)
            {
                Session["IdConvocatoriaRegla"] = IdConvocatoria;
                Session["condicionRegla"] = "0";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('ConvocatoriaReglaSalarios.aspx','_blank','width=685,height=157,toolbar=no, scrollbars=no, resizable=no');", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El máximo de condiciones para esta regla es de 6!')", true);
            }
        }

        #region cofinanciacion y criterios de seleccion

        protected void ldsconfirmacion_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var result = (from k in consultas.Db.ConvocatoriaCofinanciacions
                          from c in consultas.Db.Ciudads
                          from d in consultas.Db.departamentos
                          where k.CodCiudad == c.Id_Ciudad
                          && c.CodDepartamento == d.Id_Departamento
                          && k.CodConvocatoria == IdConvocatoria
                          select new
                          {
                              k.CodCiudad,
                              nomCiudad = c.NomCiudad + " (" + d.NomDepartamento + ") ",
                              k.Cofinanciacion
                          });

            e.Result = result.ToList();
        }

        protected void gvr_confirmacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToString().Equals("Borrar"))
            {
                ConvocatoriaCofinanciacion Cofinanciacion = (from cc in consultas.Db.ConvocatoriaCofinanciacions
                                                             where cc.CodConvocatoria == IdConvocatoria
                                                             && cc.CodCiudad == Convert.ToInt32(e.CommandArgument.ToString())
                                                             select cc).FirstOrDefault();

                consultas.Db.ConvocatoriaCofinanciacions.DeleteOnSubmit(Cofinanciacion);

                try
                {
                    consultas.Db.SubmitChanges();
                }
                catch (Exception) { }
                finally
                {
                    gvr_confirmacion.DataBind();
                }
            }

            if (e.CommandName.ToString().Equals("Editar"))
            {
                Session["CodCiudad"] = e.CommandArgument.ToString();
                Session["Accion"] = "Editar";
                Session["codConvocatoria"] = IdConvocatoria;

                Redirect(null, "CatalogoCofinanciacion.aspx", "_Blank", "width=600,height=220");
            }
        }

        protected void lnk_adicionarconfirmacion_Click(object sender, EventArgs e)
        {
            Session["Accion"] = "Nuevo";
            Session["codConvocatoria"] = IdConvocatoria;

            Redirect(null, "CatalogoCofinanciacion.aspx", "_Blank", "width=600,height=220");
        }

        protected void lds_criterioseleccion_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var criterios = (from cc in consultas.Db.ConvocatoriaCriterios
                             orderby cc.NomCriterio
                             where cc.CodConvocatoria == IdConvocatoria
                             select new
                             {
                                 cc.Id_Criterio,
                                 cc.NomCriterio
                             });

            e.Result = criterios.ToList();
        }

        protected void gvr_criteriosseleccion_RowCreated(object sender, GridViewRowEventArgs e)
        {
            del delegado = (string x, string y, int z) =>
            {
                string valor = string.Empty;

                if (z == 1)
                {
                    if (string.IsNullOrEmpty(x))
                        valor += " (Todo el pais) ";
                    else
                        valor += " " + x + " ";

                    if (string.IsNullOrEmpty(y))
                        valor += " (Todos los Municipios) ";
                    else
                        valor += " " + y + " ";
                }
                else
                {
                    if (string.IsNullOrEmpty(x))
                        valor += " (Todos los Sectores) ";
                    else
                        valor += " " + x + " ";
                }
                return valor;
            };

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int idcriterio = Convert.ToInt32(gvr_criteriosseleccion.DataKeys[e.Row.RowIndex].Value.ToString());

                var geografico = (from k in consultas.Db.ConvocatoriaCriterioCiudads
                                  join d in consultas.Db.departamentos on k.CodDepartamento equals d.Id_Departamento into r1
                                  from c1 in r1.DefaultIfEmpty()
                                  join c in consultas.Db.Ciudads on k.CodCiudad equals c.Id_Ciudad into r2
                                  from c2 in r2.DefaultIfEmpty()
                                  orderby c1.NomDepartamento, c2.NomCiudad
                                  where k.CodCriterio == idcriterio
                                  select new
                                  {
                                      Ciudad = delegado(c1.NomDepartamento, c2.NomCiudad, 1)
                                  });

                ((GridView)e.Row.FindControl("gvr_ambitos")).DataSource = geografico;
                ((GridView)e.Row.FindControl("gvr_ambitos")).DataBind();

                var economico = (from k in consultas.Db.ConvocatoriaCriterioSectors
                                 join s in consultas.Db.Sectors on k.CodSector equals s.Id_Sector into r1
                                 from c1 in r1.DefaultIfEmpty()
                                 orderby c1.NomSector
                                 where k.CodCriterio == idcriterio
                                 select new
                                 {
                                     nombreSector = delegado(c1.NomSector, "", 2)
                                 });

                ((GridView)e.Row.FindControl("gvr_ambitos1")).DataSource = economico;
                ((GridView)e.Row.FindControl("gvr_ambitos1")).DataBind();
            }
        }

        protected void gvr_criteriosseleccion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName.ToString())
            {
                case "Borrar":

                    try
                    {
                        var convocatoriacc = (from ccc in consultas.Db.ConvocatoriaCriterioCiudads
                                              where ccc.CodCriterio == Convert.ToInt32(e.CommandArgument.ToString())
                                              select ccc);

                        consultas.Db.ConvocatoriaCriterioCiudads.DeleteAllOnSubmit(convocatoriacc);
                        consultas.Db.SubmitChanges();

                        var convocatoriacs = (from ccs in consultas.Db.ConvocatoriaCriterioSectors
                                              where ccs.CodCriterio == Convert.ToInt32(e.CommandArgument.ToString())
                                              select ccs);

                        consultas.Db.ConvocatoriaCriterioSectors.DeleteAllOnSubmit(convocatoriacs);
                        consultas.Db.SubmitChanges();

                        var convocatoriac = (from cc in consultas.Db.ConvocatoriaCriterios
                                             where cc.Id_Criterio == Convert.ToInt32(e.CommandArgument.ToString())
                                             select cc).First();

                        consultas.Db.ConvocatoriaCriterios.DeleteOnSubmit(convocatoriac);
                        consultas.Db.SubmitChanges();
                    }
                    catch (Exception) { }
                    finally
                    {
                        gvr_criteriosseleccion.DataBind();
                    }

                    break;

                case "Editar":

                    Session["codCriterio"] = e.CommandArgument.ToString();
                    Session["Accion"] = "Editar";
                    Session["codConvocatoria"] = IdConvocatoria;

                    Redirect(null, "CatalogoCriterio.aspx", "_Blank", "width=860,height=550");

                    break;
            }
        }

        protected void lnkcriteriosseleccion_Click(object sender, EventArgs e)
        {
            Session["Accion"] = "Nuevo";
            Session["codConvocatoria"] = IdConvocatoria;

            Redirect(null, "CatalogoCriterio.aspx", "_Blank", "width=860,height=550");
        }

        #endregion
    }
}