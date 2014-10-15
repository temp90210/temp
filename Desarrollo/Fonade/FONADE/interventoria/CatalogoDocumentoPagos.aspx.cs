using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.interventoria
{
    public partial class CatalogoDocumentoPagos : Negocio.Base_Page
    {
        string CodPagoActividad;
        string txtSQL;

        protected void Page_Load(object sender, EventArgs e)
        {
            CodPagoActividad = Session["CodPagoActividad"] != null && !string.IsNullOrEmpty(Session["CodPagoActividad"].ToString()) ? Session["CodPagoActividad"].ToString() : "0";

            contPri.Visible = true;
            contPri.Enabled = true;

            panelEditar.Visible = false;
            panelEditar.Enabled = false;
            
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(CodPagoActividad))
                {
                    txtSQL = "SELECT PagoActividadarchivo.*, PagoActividad.Estado  FROM PagoActividadarchivo INNER JOIN PagoActividad ON PagoActividadarchivo.CodPagoActividad = PagoActividad.Id_PagoActividad  WHERE (PagoActividadarchivo.CodPagoActividad = " + CodPagoActividad + ")";

                    var dt = consultas.ObtenerDataTable(txtSQL,"text");

                    gvpresupuesto.DataSource = dt;
                    gvpresupuesto.DataBind();
                }
            }
        }

        protected void gvpresupuesto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("DocEditar"))
            {
                panelEditar.Visible = true;
                panelEditar.Enabled = true;

                contPri.Visible = false;
                contPri.Enabled = false;

                string[] pram = e.CommandArgument.ToString().Split(';');

                

                ActualizaDoc(pram);
            }
        }

        private void ActualizaDoc(string []param)
        {
            

            txtSQL = "SELECT * FROM PagoActividadArchivo WHERE codPagoactividad=" + param[0] + " AND NomPagoActividadArchivo = '" + param[1] + "'";

            var dt = consultas.ObtenerDataTable(txtSQL,"text");

            string CodTipoArchivo = "";

            if (dt.Rows.Count > 0)
            {
                CodTipoArchivo = dt.Rows[0]["CodTipoArchivo"].ToString();
                txtNomDocumento.Text = dt.Rows[0]["NomPagoActividadarchivo"].ToString();

                Session["paramDoc"] = CodTipoArchivo + ";" + param[1];
            }

            txtSQL = "SELECT Id_PagoTipoArchivo, NomPagoTipoArchivo FROM PagoTipoArchivo";

            dt = consultas.ObtenerDataTable(txtSQL, "text");

            ddltipodocumento.DataSource = dt;
            ddltipodocumento.DataTextField = "NomPagoTipoArchivo";
            ddltipodocumento.DataValueField = "Id_PagoTipoArchivo";

            ddltipodocumento.DataBind();

            ddltipodocumento.SelectedValue = CodTipoArchivo;
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            contPri.Visible = true;
            contPri.Enabled = true;

            panelEditar.Visible = false;
            panelEditar.Enabled = false;
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            string[] param = Session["paramDoc"].ToString().Split(';');

            txtSQL = @" UPDATE PagoActividadArchivo SET NomPagoActividadArchivo ='" + txtNomDocumento.Text + "', CodTipoArchivo=" + ddltipodocumento.SelectedValue +
                    " WHERE CodPagoActividad =" + param[0] + " AND NomPagoActividadArchivo='" + param[1] + "'";

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(txtSQL, conn);
            try
            {
                conn.Open();
                cmd.ExecuteReader();
            }
            catch (SqlException se) {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + se.Message + "')", true);
            }
            finally
            {
                conn.Close();
            }

            contPri.Visible = true;
            contPri.Enabled = true;

            panelEditar.Visible = false;
            panelEditar.Enabled = false;
        }
    }
}