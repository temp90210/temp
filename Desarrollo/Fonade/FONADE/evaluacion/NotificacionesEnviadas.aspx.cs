#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>23 - 03 - 2014</Fecha>
// <Archivo>NotificacionesEnviadas.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Fonade.Negocio;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class NotificacionesEnviadas : Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NotificacionEnviadas();
            }
        }

        private void NotificacionEnviadas()
        {
            lfecha.Text = DateTime.Now.ToShortDateString();

            try
            {
                if (Session["codpNotificacion"] != null && Session["codcNotificacion"] != null)
                {
                    int codconvoctoria = Convert.ToInt32(Session["codcNotificacion"].ToString());
                    int codproyecto = Convert.ToInt32(Session["codpNotificacion"].ToString());

                    consultas.Parameters = null;

                    consultas.Parameters = new[]
                                               {
                                                   new SqlParameter
                                                       {
                                                           ParameterName = "@codproyecto",
                                                           Value = codproyecto
                                                       }, new SqlParameter
                                                              {
                                                                  ParameterName = "@codconvocatoria",
                                                                  Value = codconvoctoria
                                                              }
                                               };

                    var notificaciones = consultas.ObtenerDataTable("MD_NotificacionEnviadas");

                    if (notificaciones.Rows.Count != 0)
                    {
                        Session["DtNotificaciones"] = notificaciones;
                        GrvNotificaciones.DataSource = notificaciones;
                        GrvNotificaciones.DataBind();
                    }
                    else
                    {
                        GrvNotificaciones.DataSource = notificaciones;
                        GrvNotificaciones.DataBind();
                    }

                    consultas.Parameters = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                consultas.Parameters = null;
            }
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            RedirectPage(false, string.Empty, "cerrar");
        }

        protected void GrvNotificaciones_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var rol = e.Row.FindControl("lrol") as Label;

                if (rol != null && string.IsNullOrEmpty(rol.Text))
                {
                    GrvNotificaciones.Columns[2].Visible = false;
                }

            }
        }

        protected void GrvNotificaciones_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvNotificaciones.PageIndex = e.NewPageIndex;
            GrvNotificaciones.DataSource = Session["DtNotificaciones"];
            GrvNotificaciones.DataBind();
        }
    }
}