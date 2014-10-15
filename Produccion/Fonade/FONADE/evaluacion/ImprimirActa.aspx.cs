#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>15 - 03 - 2014</Fecha>
// <Archivo>ImprimirActa.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using Fonade.Negocio;
using System.Threading;
using System.Globalization;
#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class ImprimirActa : Base_Page
    {
        private int lvalor;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BuscarActaId();
            }
        }

        private void BuscarActaId()
        {
            int idacta = !string.IsNullOrEmpty(Request["CodActa"]) ? Convert.ToInt32(Request["CodActa"]) : 0;
            ViewState["idacta"] = idacta;

            if (idacta != 0)
            {
                var actas = consultas.Db.EvaluacionActas.FirstOrDefault(a => a.Id_Acta == idacta);

                if (actas != null && actas.Id_Acta != 0)
                {
                    nroconvocatoria.Text = actas.NumActa;
                    nomconvocatoria.Text = actas.NomActa;
                    fecha.Text = actas.FechaActa.ToString("dd/mm/yyyy",CultureInfo.InvariantCulture);
                   
                    observaciones.Text = actas.Observaciones;
                    convocatoria.Text = actas.Convocatoria.NomConvocatoria;
                    int  salario = actas.Convocatoria.FechaInicio.Year;
                    Obtenersalariominimo(salario);
                    if (actas.CodConvocatoria != null) CargarProyectoNegocio((int) actas.CodConvocatoria);
                }
            }
        }

       void  Obtenersalariominimo(int ano)
       {
          var salario = consultas.Db.SalariosMinimos.FirstOrDefault(s => s.AñoSalario == ano);

           if (salario!=null &&  salario.Id_SalariosMinimos!=0)
           {
               ViewState["salario"] = salario.SalarioMinimo;
           }
       }

        private void CargarProyectoNegocio(int _convocatoria)
        {
            try
            {
                consultas.Parameters = new[]
                                           {
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@CodActa",
                                                       Value = Convert.ToInt32(ViewState["idacta"])

                                                   },
                                               new SqlParameter
                                                   {
                                                       ParameterName = "@codConvocatoria"
                                                       ,
                                                       Value = _convocatoria
                                                   }
                                           };
                DataTable dtproyectos = consultas.ObtenerDataTable("pr_ProyectosEvaluados");

                if (dtproyectos.Rows.Count != 0)
                {

                    GrvPlanesNegocio.DataSource = dtproyectos;
                    GrvPlanesNegocio.DataBind();
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        protected void GrvPlanesNegocioRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var valor = e.Row.FindControl("lvalor") as Label;
               

                if (valor!=null && !string.IsNullOrEmpty(valor.Text))
                {
                    lvalor += Convert.ToInt32(valor.Text);
                    ltsalario.Text = lvalor.ToString();
                    
                }

                  ltotal.Text = ((lvalor) * (Convert.ToDouble(ViewState["salario"].ToString()))).ToString("c");
            }
        }
    }
}
