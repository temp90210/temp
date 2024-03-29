﻿using Datos;
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
    public partial class InterConceptosFrame : Negocio.Base_Page
    {
        string CodProyecto;
        string CodEmpresa;
        string CodConvocatoria;
        string anioConvocatoria;

        string txtSQL;

        protected void Page_Load(object sender, EventArgs e)
        {
            datosEntrada();

            if (!IsPostBack)
            {
                planOperativo();
                Nomina();
                Produccion();
                Ventas();
                IndicadoresGenericos();
                IndicadoresEspecificicos();
                Riesgos();

                Empresa();
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

        private void planOperativo()
        {
            var varPlanOperativo = consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "PLANOPERATIVO", 0);
            var Aux = (from x in consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "PLANOPERATIVO", 0) select x).Count();

            if (varPlanOperativo != null)
            {
                if (Aux > 0)
                {
                    gvPlanOperativo.DataSource = varPlanOperativo;
                    gvPlanOperativo.DataBind();
                }
                else
                {
                    pnlPlanOperativo.Visible = false;
                    pnlPlanOperativo.Enabled = false;
                }
            }
            else
            {
                pnlPlanOperativo.Visible = false;
                pnlPlanOperativo.Enabled = false;
            }
        }

        private void Nomina()
        {
            var varNomina = consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "NOMINA", 0);
            var Aux = (from x in consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "NOMINA", 0) select x).Count();

            if (varNomina != null)
            {
                if (Aux > 0)
                {
                    gvNomina.DataSource = varNomina;
                    gvNomina.DataBind();
                }
                else
                {
                    pnlNomina.Visible = false;
                    pnlNomina.Enabled = false;
                }
            }
            else
            {
                pnlNomina.Visible = false;
                pnlNomina.Enabled = false;
            }
        }

        private void Produccion()
        {
            var varProduccion = consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "PRODUCCION", 0);
            var Aux = (from x in consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "PRODUCCION", 0) select x).Count();

            if (varProduccion != null)
            {
                if (Aux > 0)
                {
                    gvProduccion.DataSource = varProduccion;
                    gvProduccion.DataBind();
                }
                else
                {
                    pnlProduccion.Visible = false;
                    pnlProduccion.Enabled = false;
                }
            }
            else
            {
                pnlProduccion.Visible = false;
                pnlProduccion.Enabled = false;
            }
        }

        private void Ventas()
        {
            var varVentas = consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "VENTAS", 0);
            var Aux = (from x in consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "VENTAS", 0) select x).Count();

            if (varVentas != null)
            {
                if (Aux > 0)
                {
                    gvVentas.DataSource = varVentas;
                    gvVentas.DataBind();
                }
                else
                {
                    pnlVentas.Visible = false;
                    pnlVentas.Enabled = false;
                }
            }
            else
            {
                pnlVentas.Visible = false;
                pnlVentas.Enabled = false;
            }
        }

        private void IndicadoresGenericos()
        {
            var varGenericos = consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "INDICADORESGENERICOS", 0);
            var Aux = (from x in consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "INDICADORESGENERICOS", 0) select x).Count();

            if (varGenericos != null)
            {
                if (Aux > 0)
                {
                    gvIndicadoresGenericos.DataSource = varGenericos;
                    gvIndicadoresGenericos.DataBind();
                }
                else
                {
                    pnlIndicadoresGenericos.Visible = false;
                    pnlIndicadoresGenericos.Enabled = false;
                }
            }
            else
            {
                pnlIndicadoresGenericos.Visible = false;
                pnlIndicadoresGenericos.Enabled = false;
            }
        }

        private void IndicadoresEspecificicos()
        {
            var varEspecificos = consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "INDICADORESESPECIFICOS", 0);
            var Aux = (from x in consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "INDICADORESESPECIFICOS", 0) select x).Count();

            if (varEspecificos != null)
            {
                if (Aux > 0)
                {
                    gvindicadoresEspecificos.DataSource = varEspecificos;
                    gvindicadoresEspecificos.DataBind();
                }
                else
                {
                    pnlindicadoresEspecificos.Visible = false;
                    pnlindicadoresEspecificos.Enabled = false;
                }
            }
            else
            {
                pnlindicadoresEspecificos.Visible = false;
                pnlindicadoresEspecificos.Enabled = false;
            }
        }

        private void Riesgos()
        {
            var varRiesgos = consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "RIESGOS", 0);
            var Aux = (from x in consultas.Db.MD_ConceptoFinalRecomendacionesInterventor(Convert.ToInt32(CodProyecto), "RIESGOS", 0) select x).Count();

            if (varRiesgos != null)
            {
                if (Aux > 0)
                {
                    gvRiesgos.DataSource = varRiesgos;
                    gvRiesgos.DataBind();
                }
                else
                {
                    pnlRiesgos.Visible = false;
                    pnlRiesgos.Enabled = false;
                }
            }
            else
            {
                pnlRiesgos.Visible = false;
                pnlRiesgos.Enabled = false;
            }
        }

        private void Empresa()
        {
            txtSQL = "Select ObservacionesInt,CodDificultadCentral from Empresa where CodProyecto = " + CodProyecto;

            var dt = consultas.ObtenerDataTable(txtSQL,"text");

            var aux = (from d in consultas.Db.DificultadCentrals select d).Count();

            if (aux > 0)
            {
                var difiCen = from d in consultas.Db.DificultadCentrals select d;
                ddlDificultadCentral.DataSource = difiCen;
                ddlDificultadCentral.DataTextField = "NomDificultadCentral";
                ddlDificultadCentral.DataValueField = "Id_DificultadCentral";
                ddlDificultadCentral.DataBind();

                ddlDificultadCentral.SelectedValue = dt.Rows[0]["CodDificultadCentral"].ToString();
            }
            else
            {
                ddlDificultadCentral.Visible = false;
                ddlDificultadCentral.Enabled = false;
            }

            if (dt != null)
            {
                txtObesrvaciones.Text = dt.Rows[0]["ObservacionesInt"].ToString();
            }

            if (usuario.CodGrupo == Constantes.CONST_Interventor)
            {
                btnGrabar.Visible = true;
                btnGrabar.Enabled = true;
            }
            else
            {
                btnGrabar.Visible = false;
                btnGrabar.Enabled = false;
            }
        }

        protected void btnGrabar_Click(object sender, EventArgs e)
        {
            txtSQL = "update Empresa set ObservacionesInt = '" + txtObesrvaciones.Text + "' ";

            if (!string.IsNullOrEmpty(ddlDificultadCentral.SelectedValue))
            {
                txtSQL = txtSQL + ", CodDificultadCentral='" + ddlDificultadCentral.SelectedValue + "' ";
            }
            else
            {
                txtSQL = txtSQL + ", CodDificultadCentral=null ";
            }
            txtSQL = txtSQL + " where codproyecto = " + CodProyecto;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ToString());
            SqlCommand cmd = new SqlCommand(txtSQL, conn);
            try
            {
                conn.Open();
                cmd.ExecuteReader();
            }
            catch (SqlException se) { }
            finally
            {
                conn.Close();
            }
        }
    }
}