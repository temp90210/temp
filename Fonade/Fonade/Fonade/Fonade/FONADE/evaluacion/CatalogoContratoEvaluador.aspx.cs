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

namespace Fonade.FONADE.evaluacion
{
    public partial class CatalogoContratoEvaluador : Negocio.Base_Page
    {
        private string CodEvaluador;

        protected void Page_Load(object sender, EventArgs e)
        {
            CodEvaluador = Session["ContactoEvaluador"].ToString();
            int CodigoEvaluador = Convert.ToInt32(CodEvaluador);
            var query = (from x in consultas.Db.Contactos where x.Id_Contacto == CodigoEvaluador select new { x.Nombres, x.Apellidos }).FirstOrDefault();
            lbl_Titulo.Text = void_establecerTitulo("CONTRATOS DEL EVALUADOR: " + query.Nombres + " " + query.Apellidos);
        }

        protected void lds_listaCoordEval_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {

            try
            {
                var query = from P in consultas.Db.MD_VerContratosEvaluador(Convert.ToInt32(CodEvaluador))
                            select P;
                e.Result = query;
            }
            catch (Exception exc)
            { }

        }

        protected void gvcCoordinadoresEval_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName.ToString())
            {
                case "editarContrato":
                    Session["ContactoEvaluador"] = CodEvaluador;
                    Session["ContratoEvaluador"] = e.CommandArgument.ToString();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('ContratoEvaluador.aspx','_blank','width=502,height=300,toolbar=no, scrollbars=no, resizable=no');", true);
                    break;
            }
        }

        protected void lbtn_crearContrato_Click(object sender, EventArgs e)
        {
            abirnuevocontrato();   
        }

        private void abirnuevocontrato()
        {
            Session["ContactoEvaluador"] = CodEvaluador;
            Session["ContratoEvaluador"] = "0";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "window.open('ContratoEvaluador.aspx','_blank','width=502,height=200,toolbar=no, scrollbars=no, resizable=no');", true);
        }

        protected void ibtn_crearCntrato_Click(object sender, ImageClickEventArgs e)
        {
            abirnuevocontrato();
        }

    }
}