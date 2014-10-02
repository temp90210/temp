using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Fonade.FONADE.interventoria
{
    public partial class SeguimientoPptal : Negocio.Base_Page
    {
        string CodProyecto;
        string CodEmpresa;
        string CodConvocatoria;
        string anioConvocatoria;

        string txtSQL;
        protected void Page_Load(object sender, EventArgs e)
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

            if (!IsPostBack)
            {
                InfroHead();
                llenarGid();
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 12/05/2014.
        /// Modificación al código fuente.
        /// </summary>
        private void InfroHead()
        {
            #region Versión de Mauricio Arias Olave.
            //Inicializar variables:
            Double SMLV_Obtenido = 0;
            Int32 ValorRecomendado = 0;
            Double presupuesto_recomendado = 0;
            Double presupuesto_disponible = 0;
            Double presupuesto_aprobado = 0;

            try
            {
                //Primera consulta.
                txtSQL = " SELECT CodConvocatoria, ValorRecomendado, DatePart(yyyy,fechaInicio) AS AnnoConvocatoria " +
                         " FROM Evaluacionobservacion, Convocatoria  WHERE CodProyecto = " + CodProyecto +
                         " AND CodConvocatoria = Id_Convocatoria ORDER BY CodConvocatoria DESC ";

                //Asignar resultados de la primera consulta.
                var dt_sql_1 = consultas.ObtenerDataTable(txtSQL, "text");

                //Revisar si tiene datos... "debería tener"...
                if (dt_sql_1.Rows.Count > 0)
                {
                    //Asignar el valor de "ValorRecomendado" a la variable con el mismo nombre.
                    ValorRecomendado = Int32.Parse(dt_sql_1.Rows[0]["ValorRecomendado"].ToString());

                    //Asignar el valor de "CodConvocatoria" a la variable con el mismo nombre.
                    CodConvocatoria = dt_sql_1.Rows[0]["CodConvocatoria"].ToString();

                    //Obtener el SMLV dependiendo de la consulta anterior.
                    txtSQL = " SELECT SalarioMinimo FROM SalariosMinimos WHERE AñoSalario = " + dt_sql_1.Rows[0]["AnnoConvocatoria"].ToString();

                    //Asignar resultados de la segunda consulta a variable DataTable.
                    var dt_sql_2 = consultas.ObtenerDataTable(txtSQL, "text");

                    //Revisar si tiene datos... "debería tener"...
                    if (dt_sql_2.Rows.Count > 0)
                    {
                        //Asignar resultados a variables internas.
                        SMLV_Obtenido = Double.Parse(dt_sql_2.Rows[0]["SalarioMinimo"].ToString());

                        //Presupuesto Recomendado por Fondo Emprender:
                        presupuesto_recomendado = ValorRecomendado * SMLV_Obtenido;

                        //Mostrar resultados (Presupuesto Recomendado por Fondo Emprender:)
                        lblemprender.Text = "$" + presupuesto_recomendado.ToString("0,0.00", CultureInfo.InvariantCulture);

                        //Consulta #3: Obtener Presupuesto Aprobado por Interventoría:
                        txtSQL = " SELECT SUM(CantidadDinero) AS Total FROM PagoActividad " +
                                 " WHERE Estado >= 1 AND Estado < 5 " +
                                 " AND CodProyecto =  " + CodProyecto;

                        //Asignar resultados de la tercera consulta a variable DataTable.
                        var dt_sql_3 = consultas.ObtenerDataTable(txtSQL, "text");

                        //Revisar si tiene datos... "debería tener"...
                        if (dt_sql_3.Rows.Count > 0)
                        {
                            //Presupuesto Aprobado por Interventoría: //drDet["fondoTotal"] = "$" + totalFondo.ToString("0,0.00", CultureInfo.InvariantCulture);
                            presupuesto_aprobado = Double.Parse(dt_sql_3.Rows[0]["Total"].ToString());
                            lblinterventoria.Text = "$" + presupuesto_aprobado.ToString("0,0.00", CultureInfo.InvariantCulture);

                            //Presupuesto Disponible:
                            presupuesto_disponible = presupuesto_recomendado - presupuesto_aprobado;
                            lbldisponible.Text = "$" + presupuesto_disponible.ToString("0,0.00", CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblemprender.Text = "";
                lblinterventoria.Text = "";
                lbldisponible.Text = "";
            }
            #endregion

            #region Comentarios del desarrollo anterior.
            //var dt = consultas.ObtenerDataTable(txtSQL, "text");

            //if (dt.Rows.Count > 0)
            //{
            //    CodConvocatoria = dt.Rows[0]["CodConvocatoria"].ToString();
            //    lblemprender.Text = "$ " + (Convert.ToDouble(dt.Rows[0]["ValorRecomendado"].ToString()) * Convert.ToDouble(dt.Rows[0]["AnnoConvocatoria"].ToString()));
            //    lbldisponible.Text = "" + (Convert.ToDouble(dt.Rows[0]["ValorRecomendado"].ToString()) * Convert.ToDouble(dt.Rows[0]["AnnoConvocatoria"].ToString()));
            //}
            //else
            //{
            //    lblemprender.Text = "$ 0";
            //    lbldisponible.Text = "0";
            //}

            //txtSQL = "SELECT SUM(CantidadDinero) AS Total FROM PagoActividad  WHERE Estado >= 1 AND Estado < 5  AND CodProyecto = " + CodProyecto;

            //var dt1 = consultas.ObtenerDataTable(txtSQL, "text");

            //if (dt.Rows.Count > 0)
            //{
            //    try
            //    {
            //        lblinterventoria.Text = "$ " + Convert.ToDouble(dt1.Rows[0]["Total"].ToString());
            //    }
            //    catch (FormatException)
            //    {
            //        lblinterventoria.Text = "$ 0";
            //    }
            //}
            //else
            //    lblinterventoria.Text = "$ 0";
            //try
            //{
            //    lbldisponible.Text = "$ " + (Convert.ToDouble(lbldisponible.Text) * Convert.ToDouble(dt1.Rows[0]["Total"].ToString()));
            //}
            //catch (FormatException) { } 
            #endregion
        }

        protected void gvpresupuesto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string param = e.CommandArgument.ToString();

            Session["CodPagoActividad"] = param;

            Response.Redirect("CatalogoDocumentoPagos.aspx");
        }

        private void llenarGid()
        {
            if (!string.IsNullOrEmpty(CodProyecto))
            {
                var dt = from pi in consultas.Db.MD_PresupuestoInterventor(Convert.ToInt32(CodProyecto))
                         select new
                         {
                             pi.Id_PagoActividad,
                             pi.NomPagoActividad,
                             pi.FechaInterventor,
                             pi.CantidadDinero,
                             Estado = new Func<string>(() =>
                                 {
                                     switch (Convert.ToInt32(pi.Estado))
                                     {
                                         case Constantes.CONST_EstadoInterventor:
                                             return "Interventor";
                                         case Constantes.CONST_EstadoCoordinador:
                                             return "Coordinador";
                                         case Constantes.CONST_EstadoFiduciaria:
                                             return "Fiduciaria";
                                         case Constantes.CONST_EstadoAprobadoFA:
                                             return "Aprobado";
                                         case Constantes.CONST_EstadoRechazadoFA:
                                             return "Rechazado";
                                         default:
                                             return "";
                                     }
                                 })(),
                             pi.NomTipoIdentificacion,
                             pi.NumIdentificacion,
                             pi.Nombre,
                             pi.Apellido,
                             pi.RazonSocial,
                             pi.FechaRtaFA,
                             ValorReteFuente = new Func<string>(() =>
                                 {
                                     try
                                     {
                                         return "" + Convert.ToInt32(pi.ValorReteFuente);
                                     }
                                     catch (FormatException) { return "0"; }
                                 })(),
                             ValorReteIVA = new Func<string>(() =>
                                 {
                                     try
                                     {
                                         return "" + Convert.ToInt32(pi.ValorReteIVA);
                                     }
                                     catch (FormatException) { return "0"; }
                                 })(),
                             ValorReteICA = new Func<string>(() =>
                                 {
                                     try
                                     {
                                         return "" + Convert.ToInt32(pi.ValorReteICA);
                                     }
                                     catch (FormatException) { return "0"; }
                                 })(),
                             OtrosDescuentos = new Func<string>(() =>
                                 {
                                     try
                                     {
                                         return "" + Convert.ToInt32(pi.OtrosDescuentos);
                                     }
                                     catch (FormatException) { return "0"; }
                                 })(),
                             ValorPagado = new Func<string>(() =>
                                 {
                                     try
                                     {
                                         return "" + Convert.ToInt32(pi.ValorPagado);
                                     }
                                     catch (FormatException) { return "0"; }
                                 })(),
                             pi.CodigoPago,
                             pi.ObservacionesFA
                         };

                gvpresupuesto.DataSource = dt;
                gvpresupuesto.DataBind();
            }
        }
    }
}