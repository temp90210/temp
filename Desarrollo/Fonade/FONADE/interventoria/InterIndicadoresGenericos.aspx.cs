using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.interventoria
{
    public partial class InterIndicadoresGenericos : Negocio.Base_Page
    {
        string CodProyecto;
        string CodEmpresa;

        string txtSQL;

        delegate string del(string x, string y);

        protected void Page_Load(object sender, EventArgs e)
        {
            CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
            CodEmpresa = Session["CodEmpresa"] != null && !string.IsNullOrEmpty(Session["CodEmpresa"].ToString()) ? Session["CodEmpresa"].ToString() : "0";

            if (!IsPostBack)
            {
                //Mauricio Arias Olave. "08/05/2014": El botón "Guardar" solo puede ser visible para los interventores.
                if (usuario.CodGrupo == Datos.Constantes.CONST_CoordinadorInterventor)
                { btnGuardar.Visible = false; Post_It1.Visible = false; }

                del myDelegate = (x, y) =>
                    {
                        if (string.IsNullOrEmpty(x))
                            return "";
                        else
                            return x.Split('/')[Convert.ToInt32(y)];
                    };

                var result = from ig in consultas.Db.IndicadorGenericos
                             where ig.CodEmpresa == Convert.ToInt32(CodEmpresa)
                             select new
                             {
                                 ig.Id_IndicadorGenerico,
                                 ig.NombreIndicador,
                                 NueradorDescripcion = myDelegate(ig.Descripcion, "0"),
                                 DenominadorDescripcion = myDelegate(ig.Descripcion, "1"),
                                 ig.Numerador,
                                 ig.Denominador,
                                 ig.Evaluacion,
                                 ig.Observacion
                             };

                gvindicadoresgenericos.DataSource = result;
                gvindicadoresgenericos.DataBind();

                if (result.Count() <= 0)
                {
                    btnGuardar.Visible = false;
                    btnGuardar.Enabled = false;
                }
            }

            RestringirLetras(0);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow fila in gvindicadoresgenericos.Rows)
            {
                TextBox txtNumerador = (TextBox)fila.FindControl("txtNumerador");
                TextBox txtDenominador = (TextBox)fila.FindControl("txtDenominador");
                TextBox txtObservacion = (TextBox)fila.FindControl("txtObservacion");

                if (string.IsNullOrEmpty(txtNumerador.Text)) txtNumerador.Text = "0";
                if (string.IsNullOrEmpty(txtDenominador.Text)) txtDenominador.Text = "0";

                Int32 index = Convert.ToInt32(gvindicadoresgenericos.DataKeys[fila.RowIndex].Value.ToString());

                txtSQL = "Update IndicadorGenerico Set Numerador=" + txtNumerador.Text +
                        ", Denominador=" + txtDenominador.Text + ", Observacion = '" + txtObservacion.Text + "' Where Id_IndicadorGenerico=" + index;

                ejecutaReader(txtSQL, 2);
            }
        }

        private void RestringirLetras(int serie)
        {
            try
            {
                GridViewRow fila = gvindicadoresgenericos.Rows[serie];
                TextBox textbox = (TextBox)fila.FindControl("txtNumerador");
                textbox.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
                textbox = (TextBox)fila.FindControl("txtDenominador");
                textbox.Attributes.Add("onkeypress", "javascript: return ValidNum(event);");
                RestringirLetras(serie + 1);
            }
            catch (Exception) { }
        }
    }
}