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

namespace Fonade.FONADE.Priorizacion_de_Proyectos
{
    public partial class VerActaAsignacionRecursos : Negocio.Base_Page
    {
        public int IdDelActa;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("DETALLES DEL ACTA");
                IdDelActa = Convert.ToInt32(Session["Id_Acta"]);
                llenardatosActa();
                Session["Codigo_Acta"] = IdDelActa.ToString();
            }
        }

        protected void lds_ActaAsignacion_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            try
            {
                var query = from P in consultas.Db.MD_VerActaAsignacionRecursos(IdDelActa)
                            select P;
                e.Result = query;
            }
            catch (Exception)
            { }
        }

        protected void llenardatosActa()
        {
            var query = (from Act in consultas.Db.AsignacionActas
                         from conv in consultas.Db.Convocatorias
                         where Act.CodConvocatoria == conv.Id_Convocatoria
                         && Act.Id_Acta == IdDelActa
                         select new
                         {
                             numero = Act.NumActa,
                             nombre = Act.NomActa,
                             fecha = Act.FechaActa,
                             convocatoria = conv.NomConvocatoria,
                             observaciones = Act.Observaciones,
                         }).FirstOrDefault();
            l_convocatoria.Text = query.convocatoria;
            DateTime fechas = Convert.ToDateTime(query.fecha);
            l_fecha.Text = fechas.ToString("dd ' de ' MMMM ' de ' yyyy");
            l_nombreActa.Text = query.nombre;
            l_numActa.Text = query.numero;
            txt_observaciones.Text = query.observaciones;
        }

        protected void gv_ActaAsignacion_Load(object sender, EventArgs e)
        {
            double TotalSalarios = 0;
            double TotalDinero = 0;
            foreach (GridViewRow grd_Row in this.gv_ActaAsignacion.Rows)
            {
                TotalSalarios += Convert.ToDouble(((Label)grd_Row.FindControl("lSalarios")).Text);
                TotalDinero += Convert.ToDouble(((HiddenField)grd_Row.FindControl("hValor")).Value);
            }
            l_totalsalarios.Text = TotalSalarios.ToString();
            l_Total.Text = TotalDinero.ToString("C");
            
        }

        protected void btn_Imprimir_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('ImprimirActaAsignacion.aspx','_blank','width=680,height=680,toolbar=no, scrollbars=1, resizable=no');", true);
        }

    }
}