using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Negocio;
using System.Configuration;
using System.Web.UI;

namespace Fonade.FONADE.Administracion
{
    public partial class LegalizacionEmpresasActa : Base_Page
    {
        Int32 CodActa;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    this.Page.Title = "FONADE - Memorando de Legalización de Empresas";
                    GenerarFecha_Year();
                    EvaluarEnunciado();
                }
                catch (Exception)
                {
                    Response.Redirect("~/Account/Login.aspx");
                }
            }
        }

        private void GenerarFecha_Year()
        {
            try
            {
                int currentYear = DateTime.Today.AddYears(-11).Year;
                int futureYear = DateTime.Today.AddYears(5).Year;

                for (int i = currentYear; i < futureYear; i++)
                {
                    ListItem item = new ListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    dd_fecha_year_Memorando.Items.Add(item);
                }
            }
            catch { }
        }

        private void EvaluarEnunciado()
        {
            if (pnlPrincipal.Visible == true)
            {
                pnl_detalles.Visible = false;
                lbl_enunciado.Text = "MEMORANDO DE LEGALIZACION DE EMPRESAS";
            }
            if (pnl_detalles.Visible == true)
            {
                pnlPrincipal.Visible = false;
                lbl_enunciado.Text = "VER MEMORANDO";
            }
        }

        private void CargarDetallesCamposMemorandoSeleccionado()
        {
            try
            {
                if (CodActa != 0)
                {
                    var result = (from la in consultas.Db.LegalizacionActas
                                  where la.Id_Acta == CodActa
                                  select new
                                  {
                                      la.NumActa,
                                      la.NomActa,
                                      la.FechaActa,
                                      la.Observaciones,
                                      la.Publicado
                                  }).FirstOrDefault();

                    if (result != null)
                    {
                        txt_noMemorando.Text = result.NumActa;
                        sp_noActa.InnerText = result.NumActa;
                        txtNombreMemorando.Text = result.NomActa;
                        sp_Nombre.InnerText = result.NomActa;
                        txt_observaciones.Text = result.Observaciones;
                        sp_observaciones.InnerText = result.Observaciones;

                        sp_FechaFormateada.InnerText = result.FechaActa.Value.Month + " " + result.FechaActa.Value.Day + " de " + result.FechaActa.Value.Year;

                        dd_fecha_dias_Memorando.SelectedValue = Convert.ToString(result.FechaActa.Value.Day);
                        dd_fecha_mes_Memorando.SelectedValue = Convert.ToString(result.FechaActa.Value.Month);
                        dd_fecha_year_Memorando.SelectedValue = Convert.ToString(result.FechaActa.Value.Year);
                    }
                }
                else
                {
                    pnl_detalles.Visible = false;
                    pnlPrincipal.Visible = true;
                    EvaluarEnunciado();
                    return;
                }
            }
            catch
            {
                pnl_detalles.Visible = false;
                pnlPrincipal.Visible = true;
                EvaluarEnunciado();
                return;
            }
        }

        private void CargarGrillaDetallesMemorandoSeleccionado()
        {
            String txtSQL;

            try
            {
                if (CodActa != 0)
                {
                    txtSQL = " SELECT Proyecto.Id_Proyecto, Proyecto.NomProyecto, LegalizacionActaProyecto.Garantia, " +
                                  " LegalizacionActaProyecto.Pagare, LegalizacionActaProyecto.Contrato, " +
                                  " LegalizacionActaProyecto.PlanOperativo, LegalizacionActaProyecto.Legalizado, Empresa.razonsocial " +
                                  " FROM LegalizacionActaProyecto INNER JOIN LegalizacionActa " +
                                  " ON LegalizacionActaProyecto.CodActa = LegalizacionActa.Id_Acta INNER JOIN Proyecto " +
                                  " ON LegalizacionActaProyecto.CodProyecto = Proyecto.Id_Proyecto INNER JOIN Empresa " +
                                  " ON Proyecto.Id_Proyecto = Empresa.codproyecto AND Proyecto.Id_Proyecto = Empresa.codproyecto " +
                                  " WHERE (LegalizacionActa.Id_Acta = " + CodActa + ")";

                    var dtEmpresas = consultas.ObtenerDataTable(txtSQL, "text");

                    if (dtEmpresas.Rows.Count > 0)
                    {
                        Session["dtEmpresas"] = dtEmpresas;
                        gv_imprimir_planesNegocio.DataSource = dtEmpresas;
                        gv_imprimir_planesNegocio.DataBind();
                        gv_detallesMemorando.DataSource = dtEmpresas;
                        gv_detallesMemorando.DataBind();
                    }
                }
                else
                {
                    pnl_detalles.Visible = false;
                    pnlPrincipal.Visible = true;
                    EvaluarEnunciado();
                    return;
                }
            }
            catch
            {
                pnl_detalles.Visible = false;
                pnlPrincipal.Visible = true;
                EvaluarEnunciado();
                return;
            }
        }

        protected void gv_MemorandoEmpresas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var link_docs = e.Row.FindControl("lnkmostrar") as HyperLink;
                string numActa_Eval = link_docs.NavigateUrl;

                try
                {
                    if (link_docs != null)
                    { link_docs.NavigateUrl = "http://fondoemprender.com/fonade/Confecamaras/CargueConfecamaras" + numActa_Eval + ".csv"; }
                    //{ link_docs.NavigateUrl = "Confecamaras/CargueConfecamaras" + numActa_Eval + ".csv"; }
                }
                catch { link_docs.NavigateUrl = "#"; }
            }
        }

        protected void gv_MemorandoEmpresas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "mostrar")
            {
                CodActa = Convert.ToInt32(e.CommandArgument.ToString());
                pnlPrincipal.Visible = false;
                pnl_detalles.Visible = true;
                EvaluarEnunciado();
                CargarDetallesCamposMemorandoSeleccionado();
                CargarGrillaDetallesMemorandoSeleccionado();
            }
        }

        protected void gv_MemorandoEmpresas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_MemorandoEmpresas.PageIndex = e.NewPageIndex;
        }

        protected void gv_detallesMemorando_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int valor = 0;
                Label lbl = e.Row.FindControl("lbl_legal") as Label;
                RadioButtonList rb_listado = e.Row.FindControl("rb_EstaLegalizado") as RadioButtonList;

                try
                {
                    if (rb_listado != null && lbl != null)
                    {
                        if (lbl.Text == "True") { valor = 0; } else { valor = 1; }
                        rb_listado.Items[valor].Selected = true;
                    }
                }
                catch { }
            }
        }

        protected void ldsmemorandoempresas_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var result = (from la in consultas.Db.LegalizacionActas
                          orderby la.NumActa
                          select new
                          {
                              la.Id_Acta,
                              la.NumActa,
                              la.NomActa,
                              la.Publicado
                          });

            e.Result = result.ToList();
        }
    }
}