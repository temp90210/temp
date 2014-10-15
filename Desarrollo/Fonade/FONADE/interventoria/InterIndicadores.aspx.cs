using Datos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.interventoria
{
    public partial class InterIndicadores : Negocio.Base_Page
    {
        string CodProyecto;
        string CodEmpresa;
        string CodConvocatoria;
        string anioConvocatoria;
        Boolean bRealizado = true;

        string txtSQL;
        delegate string del(string x);

        protected void Page_Load(object sender, EventArgs e)
        {
            datosEntrada();

            if (usuario.CodGrupo != Constantes.CONST_Interventor)
            {
                btn_agregar.Enabled = false;
                btn_agregar.Visible = false;
                IB_AgregarIndicador.Enabled = false;
                IB_AgregarIndicador.Visible = false;
                GV_Indicador.Columns[0].Visible = false;
            }

            //Obtener el valor "Realizado".
            if (!bRealizado) { post_it_show.Visible = true; }

            if (!IsPostBack)
            {
                if (usuario.CodGrupo == Constantes.CONST_Interventor)
                {
                    txtSQL = "SELECT COUNT(*) as contador FROM InterventorIndicadorTMP";

                    var rdt = consultas.ObtenerDataTable(txtSQL, "text");

                    if (rdt.Rows.Count > 0)
                        lblRiesgosAprobar.Text = rdt.Rows[0]["contador"].ToString();
                }
                else
                {
                    lblRiesgosAprobar.Visible = false;
                    lblrisgostotal.Visible = false;
                }
            }
        }

        private void datosEntrada()
        {
            CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            CodEmpresa = Session["CodEmpresa"] != null && !string.IsNullOrEmpty(Session["CodEmpresa"].ToString()) ? Session["CodEmpresa"].ToString() : "0";

            txtSQL = "SELECT Max(CodConvocatoria) AS CodConvocatoria FROM ConvocatoriaProyecto WHERE CodProyecto = " + CodProyecto;

            var dt = consultas.ObtenerDataTable(txtSQL, "text");

            if (dt.Rows.Count > 0)
                CodConvocatoria = dt.Rows[0]["CodConvocatoria"].ToString();

            if (!string.IsNullOrEmpty(CodConvocatoria))
            {
                txtSQL = "select year(fechainicio) from convocatoria where id_Convocatoria=" + CodConvocatoria;

                dt = consultas.ObtenerDataTable(txtSQL, "text");

                if (dt.Rows.Count > 0)
                    anioConvocatoria = dt.Rows[0][0].ToString();
            }
        }

        protected void btn_agregar_Click(object sender, EventArgs e)
        {
            Session["Accion"] = "CREAR";
            Redirect(null, "CatalogoIndicadorInter.aspx", "_Blank", "width=800,height=600");
        }

        protected void IB_AgregarIndicador_Click(object sender, ImageClickEventArgs e)
        {
            Redirect(null, "CatalogoIndicadorInter.aspx", "_Blank", "width=800,height=600");
        }

        public IQueryable llenarGriView()
        {
            datosEntrada();
            del myDelegate = (x) =>
            {
                if (string.IsNullOrEmpty(x))
                    return "Indicadores Cualitativos y de Cumplimiento";
                else
                    return "Indicadores de Gestión";
            };

            var result = from ii in consultas.Db.InterventorIndicadors
                         where ii.CodProyecto == Convert.ToInt32(CodProyecto)
                         orderby ii.Denominador
                         select new
                         {
                             ii.Id_IndicadorInter,
                             ii.Aspecto,
                             ii.FechaSeguimiento,
                             tipoInidicador = myDelegate(ii.Denominador),
                             ii.Numerador,
                             ii.Denominador,
                             ii.Descripcion,
                             ii.RangoAceptable,
                             ii.Observacion
                         };

            return result;
        }

        protected void DD_TipoIndicador_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indicefila = ((GridViewRow)((Control)sender).NamingContainer).RowIndex;
            GridViewRow GVInventario = GV_Indicador.Rows[indicefila];
            DropDownList TBCantidades = (DropDownList)GVInventario.FindControl("DD_TipoIndicador");
            TextBox textbox = (TextBox)GVInventario.FindControl("TB_Denominador");
            String cantidad = TBCantidades.SelectedValue;

            if (cantidad.Equals("Indicadores Cualitativos y de Cumplimiento"))
            {
                textbox.Visible = false;
            }
            else
            {
                textbox.Visible = true;
            }
        }

        protected void GV_Indicador_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            datosEntrada();
            if (e.CommandName.Equals("eliminarGV"))
            {
                Int32 idinidicador = Convert.ToInt32(e.CommandArgument.ToString());

                txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;

                SqlDataReader reader = ejecutaReader(txtSQL, 1);

                if (reader != null)
                {
                    if (reader.Read())
                    {
                        txtSQL = "SELECT * FROM InterventorIndicador WHERE Id_indicadorinter=" + e.CommandArgument.ToString();

                        reader = ejecutaReader(txtSQL, 1);

                        if (reader.Read())
                        {
                            txtSQL = "Insert into InterventorIndicadorTMP (Id_indicadorinter,CodProyecto,Aspecto,FechaSeguimiento,Numerador,Denominador,Descripcion,RangoAceptable,CodTipoIndicadorInter,Observacion,Tarea) " +
                                    "values (" + e.CommandArgument.ToString() + "," + CodProyecto + ",'" + reader["Aspecto"].ToString() + "','" + reader["FechaSeguimiento"].ToString() + "', '" + reader["Numerador"].ToString() + "', '" + reader["Denominador"].ToString() + "', '" + reader["Descripcion"].ToString() + "', " + reader["RangoAceptable"].ToString() + "," + reader["CodTipoIndicadorInter"].ToString() + ",'" + reader["Observacion"].ToString() + "','Borrar')";

                            ejecutaReader(txtSQL, 2);
                        }
                    }
                    else
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
                }
            }
            if (e.CommandName == "Edit_Inter")
            {
                Session["id_indicadorinter"] = e.CommandArgument;
                Redirect(null, "CatalogoIndicadorInter.aspx", "_Blank", "width=800,height=600");
            }
        }

        public void actualizar(String Id_IndicadorInter, String Aspecto, String FechaSeguimiento, String tipoInidicador, String Numerador, String Denominador, String Descripcion, String RangoAceptable, String Observacion)
        {
            datosEntrada();
            if (tipoInidicador.Equals("Indicadores de Gestión"))
            {
                if (String.IsNullOrEmpty(Denominador))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('El Campo Denominador es requerido')", true);
                    return;
                }
            }

            int tipoIndi;

            if (tipoInidicador.Equals("Indicadores de Gestión"))
                tipoIndi = 1;
            else
                tipoIndi = 2;

            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
                txtSQL = "select CodCoordinador from interventor where codcontacto=" + usuario.IdContacto;

                SqlDataReader reader = ejecutaReader(txtSQL, 1);

                if (reader != null)
                {
                    if (reader.Read())
                    {
                        txtSQL = "Insert into InterventorIndicadorTMP (Id_indicadorinter,CodProyecto,Aspecto,FechaSeguimiento,Numerador,Denominador,Descripcion,RangoAceptable,CodTipoIndicadorInter,Observacion,Tarea) " +
                                "values (" + Id_IndicadorInter + "," + CodProyecto + ",'" + Aspecto + "','" + FechaSeguimiento + "', '" + Numerador + "', '" + Denominador + "', '" + Descripcion + "', " + RangoAceptable + "," + tipoIndi + ",'" + Observacion + "','Modificar')";

                        reader = ejecutaReader(txtSQL, 1);
                    }
                    else
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No tiene ningún coordinador asignado.')", true);
                }
            }
        }

        /// <summary>
        /// RowDataBound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GV_Indicador_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lnk_a = e.Row.FindControl("LBA_Aspecto") as LinkButton;
                var lnk_b = e.Row.FindControl("LBC_Aspecto") as LinkButton;
                var lnk_btn = e.Row.FindControl("LinkButton1") as LinkButton;

                var lnk_it = e.Row.FindControl("LB_Aspecto") as LinkButton;
                var img = e.Row.FindControl("I_EliminarIndicador") as Image;

                if (lnk_it != null && img != null && lnk_btn != null)
                {
                    if (usuario.CodGrupo != Constantes.CONST_Interventor)
                    {
                        lnk_btn.Visible = false;

                        lnk_it.Style.Add("text-decoration", "none");
                        lnk_it.ForeColor = System.Drawing.Color.Black;
                        lnk_it.Enabled = false;
                    }
                }
            }
        }
    }
}