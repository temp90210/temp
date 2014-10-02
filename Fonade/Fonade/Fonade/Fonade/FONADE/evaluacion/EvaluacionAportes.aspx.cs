using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using Newtonsoft.Json;
using System.Data;

namespace Fonade.FONADE.evaluacion
{
    public partial class EvaluacionAportes : Negocio.Base_Page
    {
        #region Variables

        private double totalInversion, totalCapital, totalDiferido, totalInversion1, totalCapital1, totalDiferido1;
        public int CodProyecto, CodConvocatoria;
        public int _salariominimo;
        private static Consultas _consultas = new Consultas();
        public int _anioConvocatoria = 0;
        public bool bandera;
        private GridViewHelper helper;
        public double TotalSolicitadoAporte, TotalRecomendadoAporte,
            TotalSolicitadoAporteC, TotalRecomendadoAporteC, TotalSolicitadoAporteD, TotalRecomendadoAporteD;
        // variable para acoumular el porcentaje de los totales solicitados y recomendados
        public float dif1D, dif2D, dif3D, dif1A, dif2A, dif3A, dif1C, dif2c, dif3c;
        public double aporteTotal;
        public Boolean esMiembro;
        /// <summary>
        /// Determina si "está" o "no" realizado...
        /// </summary>
        public Boolean bRealizado;
        /// <summary>
        /// Variable que se debe cambiar.
        /// </summary>
        String Accion = "CAMBIAR";
        /// <summary>
        /// Contiene las consultas SQL.
        /// </summary>
        String txtSQL;

        #endregion

        #region Metodos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CodProyecto = (int)(!string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Convert.ToInt64(Session["CodProyecto"]) : 0);
                CodConvocatoria = (int)(!string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Convert.ToInt64(Session["CodConvocatoria"]) : 0);
                inicioEncabezado(Convert.ToString(CodProyecto), Convert.ToString(CodConvocatoria),
                                 Constantes.CONST_subAportes);

                //Consultar si es miembro.
                esMiembro = fnMiembroProyecto(usuario.IdContacto, CodProyecto.ToString());

                //Consultar si está "realizado".
                bRealizado = esRealizado(Constantes.CONST_subAportes.ToString(), CodProyecto.ToString(), "");

                if (!(esMiembro && !bRealizado)) { this.div_Post_It1.Visible = true; }

                ObtenerAñoConvocatoria();
                Permisos();
                ObtenerSalariosMinimos();
                LlenarGrillas();
                ObtenerDatosUltimaActualizacion();
            }

            if (usuario.CodGrupo == Constantes.CONST_GerenteEvaluador || usuario.CodGrupo == Constantes.CONST_CoordinadorEvaluador)
            {
                //Mostrar labels.
                lbl_solicitado.Visible = true;
                lbl_recomendado.Visible = true;

                //Inhabilitar y ocultar determinados campos.
                pnlagregar.Visible = false;
                txtobservaciones.Enabled = false;
                txtsolicitado.Visible = false;
                txtrecomendado.Visible = false;
                Btnupdate.Visible = false;

                try { GrAportes.Columns[0].Visible = false; }
                catch { }
                try { GrAportes.Columns[1].Visible = false; }
                catch { }

                try { GrvCapital.Columns[0].Visible = false; }
                catch { }
                try { GrvCapital.Columns[1].Visible = false; }
                catch { }

                try { GrvDifereridas.Columns[0].Visible = false; }
                catch { }
                try { GrvDifereridas.Columns[1].Visible = false; }
                catch { }

                GrvIntegrantes.Columns[0].Visible = false;
            }
        }

        public void LlenarGrillas()
        {
            if (CodConvocatoria != 0 && CodProyecto != 0)
            {
                #region Comentarios NO BORRAR!.
                //// cargo la grilla de aportes

                //var list = consultas.ObtenerAportes(Convert.ToString(CodProyecto), Convert.ToString(CodConvocatoria));
                //var lstaportes = new List<EvaluacionProyectoAporte>();
                //var lstcapital = new List<EvaluacionProyectoAporte>();
                //var lstdiferencia = new List<EvaluacionProyectoAporte>();

                //if (list.Any())
                //{
                //    foreach (var items in list)
                //    {
                //        if (items.id_TipoIndicador == 1)
                //        {
                //            lstaportes = consultas.Db.EvaluacionProyectoAportes
                //                .Where(
                //                    ea =>
                //                    ea.CodProyecto == CodProyecto && ea.CodConvocatoria == CodConvocatoria &&
                //                    ea.CodTipoIndicador == items.id_TipoIndicador).ToList();

                //            var d = !string.IsNullOrEmpty(items.TotalSolicitado.ToString()) ? items.TotalSolicitado : 0;
                //            if (d != null)
                //                TotalSolicitadoAporte = (double)d;

                //            TotalRecomendadoAporte = !string.IsNullOrEmpty(items.TotalRecomendado.ToString()) ? items.TotalRecomendado : 0;
                //        }
                //        else if (items.id_TipoIndicador == 2)
                //        {
                //            lstcapital = consultas.Db.EvaluacionProyectoAportes
                //                .Where(
                //                    ea =>
                //                    ea.CodProyecto == CodProyecto && ea.CodConvocatoria == CodConvocatoria &&
                //                    ea.CodTipoIndicador == items.id_TipoIndicador).ToList();

                //            var d = !string.IsNullOrEmpty(items.TotalSolicitado.ToString()) ? items.TotalSolicitado : 0;
                //            if (d != null)
                //                TotalSolicitadoAporteC = (double)d;

                //            TotalRecomendadoAporteC = !string.IsNullOrEmpty(items.TotalRecomendado.ToString()) ? items.TotalRecomendado : 0;
                //        }
                //        else if (items.id_TipoIndicador == 3)
                //        {
                //            lstdiferencia = consultas.Db.EvaluacionProyectoAportes
                //                .Where(
                //                    ea =>
                //                    ea.CodProyecto == CodProyecto && ea.CodConvocatoria == CodConvocatoria &&
                //                    ea.CodTipoIndicador == items.id_TipoIndicador).ToList();

                //            var d = !string.IsNullOrEmpty(items.TotalSolicitado.ToString()) ? items.TotalSolicitado : 0;
                //            if (d != null)
                //                TotalSolicitadoAporteD = (double)d;

                //            TotalRecomendadoAporteD = !string.IsNullOrEmpty(items.TotalRecomendado.ToString()) ? items.TotalRecomendado : 0;
                //        }
                //    }



                //    if (lstaportes.Any())
                //    {
                //        GrAportes.DataSource = lstaportes;
                //        GrAportes.DataBind();
                //    }
                //    else PnlInversiones.Visible = false;

                //    if (lstdiferencia.Any())
                //    {
                //        GrvDifereridas.DataSource = lstdiferencia;
                //        GrvDifereridas.DataBind();
                //    }
                //    else PnlInversionsDiferidas.Visible = false;

                //    if (lstcapital.Any())
                //    {
                //        GrvCapital.DataSource = lstcapital;
                //        GrvCapital.DataBind();
                //    }
                //    else panelcapital.Visible = false;

                //}

                //// cargo la grilla de integrantes
                //var consulta = consultas.ObtenerIntegrantesIniciativa(Convert.ToString(CodProyecto),
                //                                                        Convert.ToString(CodConvocatoria));
                //if (consulta.Any())
                //{
                //    GrvIntegrantes.DataSource = consulta;
                //    GrvIntegrantes.DataBind();
                //}
                #endregion

                #region Versión SQL.

                //Inicializar variables.
                DataTable rs = new DataTable();
                DataTable rsAux = new DataTable();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();

                txtSQL = " SELECT nomTipoIndicador, id_TipoIndicador, sum(solicitado) as TotalSolicitado, isnull(sum(Recomendado),0) as TotalRecomendado " +
                         " FROM TipoIndicadorGestion T, EvaluacionProyectoAporte E " +
                         " WHERE E.codProyecto=" + CodProyecto + " AND E.codConvocatoria=" + CodConvocatoria + " AND codTipoIndicador= id_tipoindicador " +
                         " GROUP BY nomTipoIndicador, id_TipoIndicador " +
                         " ORDER BY id_TipoIndicador";

                rs = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow row_rs in rs.Rows)
                {
                    if (row_rs["id_TipoIndicador"].ToString() == "1")
                    {
                        txtSQL = " SELECT id_Aporte, Nombre, Detalle, Solicitado, isnull(Recomendado,0) as recomendado, Protegido, CodTipoIndicador " +
                                 " FROM EvaluacionProyectoAporte E " +
                                 " WHERE E.codProyecto = " + CodProyecto + " AND E.codConvocatoria=" + CodConvocatoria + " AND codTipoIndicador= 1 " +
                                 " ORDER BY id_Aporte";

                        dt1 = consultas.ObtenerDataTable(txtSQL, "text");
                        GrAportes.DataSource = dt1;
                        GrAportes.DataBind();

                    }
                    if (row_rs["id_TipoIndicador"].ToString() == "2")
                    {
                        txtSQL = " SELECT id_Aporte, Nombre, Detalle, Solicitado, isnull(Recomendado,0) as recomendado, Protegido, CodTipoIndicador " +
                                 " FROM EvaluacionProyectoAporte E " +
                                 " WHERE E.codProyecto = " + CodProyecto + " AND E.codConvocatoria=" + CodConvocatoria + " AND codTipoIndicador= 2 " +
                                 " ORDER BY id_Aporte";

                        dt2 = consultas.ObtenerDataTable(txtSQL, "text");
                        GrvCapital.DataSource = dt2;
                        GrvCapital.DataBind();
                    }
                    if (row_rs["id_TipoIndicador"].ToString() == "3")
                    {
                        txtSQL = " SELECT id_Aporte, Nombre, Detalle, Solicitado, isnull(Recomendado,0) as recomendado, Protegido, CodTipoIndicador " +
                                 " FROM EvaluacionProyectoAporte E " +
                                 " WHERE E.codProyecto = " + CodProyecto + " AND E.codConvocatoria=" + CodConvocatoria + " AND codTipoIndicador= 3 " +
                                 " ORDER BY id_Aporte";

                        dt2 = consultas.ObtenerDataTable(txtSQL, "text");
                        GrvDifereridas.DataSource = dt3;
                        GrvDifereridas.DataBind();
                    }
                }

                #endregion
            }
        }

        public void Permisos()
        {
            int codgrupo = Convert.ToInt32(usuario.CodGrupo);


            if (codgrupo == Constantes.CONST_Evaluador)
            {
                //miembro && !realizado &&
                txtobservaciones.Enabled = true;
                Btnupdate.Enabled = true;
                GrAportes.Columns[1].Visible = false;
                GrvDifereridas.Columns[1].Visible = false;
                GrvCapital.Columns[1].Visible = false;

            }
            else
            {
                if (codgrupo == Constantes.CONST_GerenteEvaluador)
                {
                    pnlagregar.Visible = false;
                    GrvCapital.Columns[0].Visible = false;
                    GrvDifereridas.Columns[0].Visible = false;
                    GrvIntegrantes.Columns[0].Visible = false;
                    GrAportes.Columns[0].Visible = false;

                    GrvCapital.Columns[1].Visible = false;
                    GrvDifereridas.Columns[1].Visible = false;
                    GrAportes.Columns[1].Visible = false;

                    Btnupdate.Visible = false;
                }
                else
                {
                    txtobservaciones.Enabled = false;
                    txtobservaciones.Enabled = false;
                    GrAportes.Columns[1].Visible = false;
                    GrvDifereridas.Columns[1].Visible = false;
                    GrvCapital.Columns[1].Visible = false;
                }
            }

            CargarObservaciones();
        }

        public void CargarObservaciones()
        {
            var LeftJoin = (from p in _consultas.Db.ProyectoFinanzasIngresos
                            join e in _consultas.Db.EvaluacionObservacions
                                on p.CodProyecto equals e.CodProyecto
                                into joinedEmpDept
                            from e in joinedEmpDept.DefaultIfEmpty()
                            where p.CodProyecto == CodProyecto && e.CodConvocatoria == CodConvocatoria
                            select new
                            {
                                p.Recursos,
                                valorRe = e.ValorRecomendado != null ? (short?)e.ValorRecomendado : 0,
                                e.EquipoTrabajo

                            }).FirstOrDefault();
            if (LeftJoin != null)
            {

                txtsolicitado.Text = LeftJoin.Recursos.ToString();
                lbl_solicitado.Text = LeftJoin.Recursos.ToString();
                txtrecomendado.Text = LeftJoin.valorRe.ToString();
                lbl_recomendado.Text = LeftJoin.valorRe.ToString();
                txtobservaciones.Text = LeftJoin.EquipoTrabajo;
                lblvalidador.Text = "validar";

            }
            else
            {
                lblvalidador.Text = "";
            }
        }

        public void ObtenerAñoConvocatoria()
        {
            if (CodConvocatoria != 0)
            {
                DateTime fechainicio =
                    _consultas.Db.Convocatorias.Single(c => c.Id_Convocatoria == CodConvocatoria).FechaInicio;
                _anioConvocatoria = fechainicio.Year;
            }


        }

        public void ObtenerSalariosMinimos()
        {
            if (CodProyecto != 0)
            {
                _salariominimo =
                    _consultas.Db.ProyectoGastosPersonals.Count(p => p.CodProyecto == CodProyecto) +
                    _consultas.Db.ProyectoInsumos.Count(
                        pi => pi.codTipoInsumo == 2 && pi.CodProyecto == CodProyecto);

                if (_salariominimo <= 5)
                {
                    _salariominimo = 150;
                }
                else if (_salariominimo >= 6)
                {
                    _salariominimo = 180;
                }
                else _salariominimo = 150;
            }


        }

        [WebMethod]
        public static string Eliminar(string codigo)
        {
            string mensajeDeError = string.Empty;

            var entity = _consultas.Db.EvaluacionProyectoAportes.Single(
                p => p.Id_Aporte == Convert.ToInt64(codigo));

            if (entity.Id_Aporte != 0)
            {
                _consultas.Db.EvaluacionProyectoAportes.DeleteOnSubmit(entity);
                _consultas.Db.SubmitChanges();
                ////Actualizar fecha de actualización.
                //prActualizarTabEval(Constantes.CONST_subAportes.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());
                mensajeDeError = "ok";
            }
            else
            {
                mensajeDeError = "El registro no se puede eliminar";
            }


            return JsonConvert.SerializeObject(new
            {
                mensaje = mensajeDeError
            });
        }

        #endregion

        #region Eventos Grid

        #region  RowDataBound

        protected void GrvDiferidas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                var ltotalsolicitado = e.Row.FindControl("lblTotalSolicitadoD") as Label;
                var ltotalRecomendado = e.Row.FindControl("lblTotalRecomendadoD") as Label;
                var PorcentajeSolicitado = e.Row.FindControl("lblPorcentajeSolicitadoD") as Label;
                var PorcentajeRecomendado = e.Row.FindControl("lblPorcentajeRecomendadoD") as Label;


                double dcantidad = !string.IsNullOrEmpty(ltotalsolicitado.Text)
                                       ? double.Parse(ltotalsolicitado.Text.Replace("$", ""))
                                       : 0;
                double dvalor = !string.IsNullOrEmpty(ltotalRecomendado.Text)
                                    ? double.Parse(ltotalRecomendado.Text.Replace("$", ""))
                                    : 0;
                // Sumamos el total de la cantidad solicitada
                if (dcantidad != 0)
                {
                    totalDiferido1 += dcantidad;
                    LblTotalDiferida.Text = totalDiferido1.ToString("C");
                }
                else ltotalsolicitado.Text = dcantidad.ToString("C");

                //sumamos el total de la cantidad recomendad
                if (dvalor != 0)
                {
                    totalDiferido += dvalor;
                    LblTotalDiferida.Text = totalDiferido.ToString("C");
                }
                else ltotalRecomendado.Text = dvalor.ToString("C");

                // Sacamos el porcentaje de lo solicitado y tambien de lo recomendado

                if (TotalSolicitadoAporteD != 0)
                {
                    PorcentajeSolicitado.Text = string.Format("{0:00.00}", ((dcantidad * 100) / TotalSolicitadoAporteD)).ToString();
                    dif1D += (float)Convert.ToDecimal(PorcentajeSolicitado.Text);
                    ldif1.Text = dif1D.ToString();
                }
                else PorcentajeSolicitado.Text = "0"; ldif2.Text = "0";

                if (TotalRecomendadoAporteD != 0)
                {
                    PorcentajeRecomendado.Text = string.Format("{0:00.00}",
                                                              ((dvalor * 100) / TotalRecomendadoAporteD).ToString());
                    dif2D += Convert.ToInt32(PorcentajeRecomendado.Text);
                    ldif2.Text = dif2D.ToString();
                    ldif2.Text = TotalRecomendadoAporteD.ToString("C");
                }
                else PorcentajeRecomendado.Text = "0"; ldif2.Text = "0";

            }


        }

        protected void GrvCapital_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ltotalsolicitado = e.Row.FindControl("lblTotalSolicitadoC") as Label;
                var ltotalRecomendado = e.Row.FindControl("lblTotalRecomendadoC") as Label;
                var PorcentajeSolicitado = e.Row.FindControl("lblPorcentajeSolicitadoC") as Label;
                var PorcentajeRecomendado = e.Row.FindControl("lblPorcentajeRecomendadoC") as Label;

                double dcantidad = !string.IsNullOrEmpty(ltotalsolicitado.Text)
                                       ? double.Parse(ltotalsolicitado.Text.Replace("$", ""))
                                       : 0;

                ltotalsolicitado.Text = dcantidad.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                double dvalor = !string.IsNullOrEmpty(ltotalRecomendado.Text)
                                    ? double.Parse(ltotalRecomendado.Text.Replace("$", ""))
                                    : 0;

                ltotalRecomendado.Text = dvalor.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                // Sumamos el total de la cantidad solicitada
                if (dcantidad != 0)
                {
                    totalCapital1 += dcantidad;
                    LblTotalCapital.Text = totalCapital1.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                }
                else ltotalsolicitado.Text = dcantidad.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                //sumamos el total de la cantidad recomendad
                if (dvalor != 0)
                {
                    totalCapital += dvalor;
                    LblTotalCapital.Text = totalCapital.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                }
                else ltotalRecomendado.Text = dvalor.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                // Sacamos el porcentaje de lo solicitado y tambien de lo recomendado

                if (TotalSolicitadoAporteC != 0)
                {
                    PorcentajeSolicitado.Text = string.Format("{0:00.00}", ((dcantidad * 100) / TotalSolicitadoAporteC)).ToString();
                    dif1C += (float)Convert.ToDecimal(PorcentajeSolicitado.Text);
                    ldif1c.Text = dif1C.ToString();
                }
                else { PorcentajeSolicitado.Text = "0"; ldif1c.Text = "0"; }

                if (TotalRecomendadoAporteC != 0)
                {
                    PorcentajeRecomendado.Text = string.Format("{0:00.00}", ((dvalor * 100) / TotalRecomendadoAporteC).ToString());
                    dif2c += (float)Convert.ToDecimal(PorcentajeRecomendado.Text);
                    ldif2c.Text = dif2c.ToString();
                    ldif2c.Text = TotalRecomendadoAporteC.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                }
                else
                {
                    PorcentajeRecomendado.Text = "0";
                    ldif2c.Text = "0";
                }
            }
        }

        protected void GrAportes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var ltotalsolicitado = e.Row.FindControl("lblTotalSolicitado") as Label;
                var ltotalRecomendado = e.Row.FindControl("lblTotalRecomendado") as Label;
                var PorcentajeSolicitado = e.Row.FindControl("lblPorcentajeSolicitado") as Label;
                var PorcentajeRecomendado = e.Row.FindControl("lblPorcentajeRecomendado") as Label;
                var img_1 = e.Row.FindControl("imgeditar") as Image;

                double dcantidad = !string.IsNullOrEmpty(ltotalsolicitado.Text)
                                       ? double.Parse(ltotalsolicitado.Text.Replace("$", ""))
                                       : 0;
                ltotalsolicitado.Text = dcantidad.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                double dvalor = !string.IsNullOrEmpty(ltotalRecomendado.Text)
                                    ? double.Parse(ltotalRecomendado.Text.Replace("$", ""))
                                    : 0;

                ltotalRecomendado.Text = dvalor.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                //sumamos el total de la cantidad recomendad
                if (dvalor != 0)
                {
                    totalInversion1 += dvalor;
                    LblTotal.Text = totalInversion1.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                }
                else ltotalRecomendado.Text = dvalor.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                // Sumamos el total de la cantidad solicitada
                if (dcantidad != 0)
                {
                    totalInversion += dcantidad;
                    LblTotal.Text = totalInversion.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                }
                else ltotalsolicitado.Text = dcantidad.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                // Sacamos el porcentaje de lo solicitado y tambien de lo recomendado

                if (TotalSolicitadoAporte != 0)
                {
                    PorcentajeSolicitado.Text = string.Format("{0:00.00}", ((dcantidad * 100) / TotalSolicitadoAporte)).ToString();
                    dif1A += (float)Convert.ToDecimal(PorcentajeSolicitado.Text);
                    ldif1A.Text = dif1A.ToString();
                    //ldif1A.Text = TotalSolicitadoAporte.ToString();
                }
                else { PorcentajeSolicitado.Text = "0"; ldif1A.Text = "0"; }

                if (TotalRecomendadoAporte != 0)
                {
                    PorcentajeRecomendado.Text = string.Format("{0:00.00}", ((dvalor * 100) / TotalRecomendadoAporte)).ToString();
                    dif2A += (float)Convert.ToDecimal(PorcentajeRecomendado.Text);
                    ldif2A.Text = dif2A.ToString();
                    ldif2A.Text = TotalRecomendadoAporte.ToString("C");
                }
                else
                {
                    PorcentajeRecomendado.Text = "0";
                    ldif2A.Text = "0";
                }
            }
        }

        protected void GrvIntegrantes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                var laporteDinero = e.Row.FindControl("lblAporteDinero") as Label;
                var laporteEspecie = e.Row.FindControl("lblAporteEspecie") as Label;
                var laporteTotal = e.Row.FindControl("lblAporteTotal") as Label;
                var lbeneficiario = e.Row.FindControl("lblEmprendedor") as Label;
                var lotro = e.Row.FindControl("lblotro") as Label;

                if (!string.IsNullOrEmpty(laporteDinero.Text)
                    && laporteDinero.Text != "0" && !string.IsNullOrEmpty(laporteEspecie.Text)
                    && laporteEspecie.Text != "0")
                {
                    aporteTotal += ((Convert.ToDouble(laporteDinero.Text) + Convert.ToDouble(laporteEspecie.Text)) / 1000);
                    laporteTotal.Text = aporteTotal.ToString();
                }
                else
                {
                    laporteTotal.Text = "0";
                }
                if (lbeneficiario.Text == "True")
                {
                    lotro.Text = string.Empty;
                    lbeneficiario.Text = "<img src='../../Images/chulo.gif' />";
                }
                else
                {
                    lbeneficiario.Text = string.Empty;
                    lotro.Text = "<img src='../../Images/chulo.gif' />";
                }

            }
        }

        #endregion

        #region PageIndexChangin


        protected void GrAportes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrAportes.PageIndex = e.NewPageIndex;
            LlenarGrillas();
        }

        protected void GrvDiferidas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvDifereridas.PageIndex = e.NewPageIndex;
            LlenarGrillas();
        }

        protected void GrvCapital_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvCapital.PageIndex = e.NewPageIndex;
            LlenarGrillas();
        }


        protected void GrvIntegrantes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvIntegrantes.PageIndex = e.NewPageIndex;
            LlenarGrillas();
        }

        #endregion

        protected void Btnupdate_Click(object sender, EventArgs e)
        {
            Actualizar();
            prActualizarTabEval(Constantes.CONST_subAportes.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());
            ObtenerDatosUltimaActualizacion();
        }

        public void Actualizar()
        {
            CodProyecto = (int)(!string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Convert.ToInt64(Session["CodProyecto"]) : 0);
            CodConvocatoria = (int)(!string.IsNullOrEmpty(Session["CodConvocatoria"].ToString()) ? Convert.ToInt64(Session["CodConvocatoria"]) : 0);

            var evaluacionProyectoAporte = consultas.Db.EvaluacionObservacions.FirstOrDefault(o => o.CodProyecto == CodProyecto && o.CodConvocatoria == CodConvocatoria);
            var evaluacion = new EvaluacionObservacion();


            if (evaluacionProyectoAporte != null)
            {
                evaluacionProyectoAporte.ValorRecomendado = (short)Convert.ToInt64(txtrecomendado.Text);
                evaluacionProyectoAporte.EquipoTrabajo = txtobservaciones.Text;
                consultas.Db.SubmitChanges();
            }
            else
            {
                evaluacion.CodProyecto = CodProyecto;
                evaluacion.CodConvocatoria = CodConvocatoria;
                evaluacion.Actividades = string.Empty;
                evaluacion.ProductosServicios = string.Empty;
                evaluacion.EstrategiaMercado = string.Empty;
                evaluacion.ProcesoProduccion = string.Empty;
                evaluacion.EstructuraOrganizacional = string.Empty;
                evaluacion.TamanioLocalizacion = string.Empty;
                evaluacion.Generales = string.Empty;
                evaluacion.ValorRecomendado = (short)Convert.ToInt64(txtrecomendado.Text);
                evaluacion.EquipoTrabajo = txtobservaciones.Text;
                consultas.Db.EvaluacionObservacions.InsertOnSubmit(evaluacion);
            }

        }

        #endregion

        #region Métodos de Mauricio Arias Olave.

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        private void ObtenerDatosUltimaActualizacion()
        {
            //Inicializar variables.
            DateTime fecha = new DateTime();
            DataTable tabla = new DataTable();
            bool bNuevo = true; //Indica si las aprobaciones de las pestañas pueden ser levantadas por el evaluador.
            bool bRealizado = false;
            bool bEnActa = false; //Determinar si el proyecto esta incluido en un acta de comite evaluador.
            bool EsMiembro = false;
            Int32 CodigoEstado = 0;

            try
            {
                //Consultar si es "Nuevo".
                bNuevo = es_bNuevo(CodProyecto.ToString());

                //Determinar si "está en acta".
                bEnActa = es_EnActa(CodProyecto.ToString(), CodConvocatoria.ToString());

                //Consultar si es "Miembro".
                EsMiembro = fnMiembroProyecto(usuario.IdContacto, CodProyecto.ToString());

                //Consultar el "Estado" del proyecto.
                CodigoEstado = CodEstado_Proyecto(Constantes.CONST_subAportes.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString());

                #region Obtener el rol.

                //Consulta.
                txtSQL = " SELECT CodContacto, CodRol From ProyectoContacto " +
                         " Where CodProyecto = " + CodProyecto + " And CodContacto = " + usuario.IdContacto +
                         " and inactivo=0 and FechaInicio<=getdate() and FechaFin is null ";

                //Asignar variables a DataTable.
                var rs = consultas.ObtenerDataTable(txtSQL, "text");

                if (rs.Rows.Count > 0)
                {
                    //Crear la variable de sesión.
                    Session["CodRol"] = rs.Rows[0]["CodRol"].ToString();
                }

                //Destruir la variable.
                rs = null;

                #endregion

                //Consultar los datos a mostrar en los campos correspondientes a la actualización.
                txtSQL = " select nombres+' '+apellidos as nombre, fechamodificacion, realizado  " +
                         " from tabEvaluacionproyecto, contacto " +
                         " where id_contacto = codcontacto and codtabEvaluacion = " + Constantes.CONST_subAportes +
                         " and codproyecto = " + CodProyecto +
                         " and codconvocatoria = " + CodConvocatoria;

                //Asignar resultados de la consulta a variable DataTable.
                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                //Si tiene datos "y debe tenerlos" ejecuta el siguiente código.
                if (tabla.Rows.Count > 0)
                {
                    //Nombre del usuario quien hizo la actualización.
                    lbl_nombre_user_ult_act.Text = tabla.Rows[0]["nombre"].ToString().ToUpperInvariant();

                    #region Formatear la fecha.

                    //Convertir fecha.
                    try { fecha = Convert.ToDateTime(tabla.Rows[0]["FechaModificacion"].ToString()); }
                    catch { fecha = DateTime.Today; }

                    //Obtener el nombre del mes (las primeras tres letras).
                    string sMes = fecha.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                    //Obtener la hora en minúscula.
                    string hora = fecha.ToString("hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant();

                    //Reemplazar el valor "am" o "pm" por "a.m" o "p.m" respectivamente.
                    if (hora.Contains("am")) { hora = hora.Replace("am", "a.m"); } if (hora.Contains("pm")) { hora = hora.Replace("pm", "p.m"); }

                    //Formatear la fecha según manejo de FONADE clásico. "Ej: Nov 19 de 2013 07:36:26 p.m.".
                    lbl_fecha_formateada.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year + " " + hora + ".";

                    #endregion

                    //Valor "bRealizado".
                    bRealizado = Convert.ToBoolean(tabla.Rows[0]["Realizado"].ToString());
                }

                //Asignar check de acuerdo al valor obtenido en "bRealizado".
                chk_realizado.Checked = bRealizado;

                //Evaluar "habilitación" del CheckBox.
                //if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                if (!(EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString()) || lbl_nombre_user_ult_act.Text.Trim() == "" || CodigoEstado != Constantes.CONST_Evaluacion || bEnActa)
                { chk_realizado.Enabled = false; }

                //if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString() && lbl_nombre_user_ult_act.Text.Trim() != "" && CodigoEstado == Constantes.CONST_Evaluacion && (!bEnActa))
                if (EsMiembro && Session["CodRol"].ToString() == Constantes.CONST_RolCoordinadorEvaluador.ToString() && lbl_nombre_user_ult_act.Text.Trim() != "" && CodigoEstado == Constantes.CONST_Evaluacion && (!bEnActa))
                {
                    btn_guardar_ultima_actualizacion.Enabled = true;
                    btn_guardar_ultima_actualizacion.Visible = true;
                }

                //Destruir variables.
                tabla = null;
                txtSQL = null;
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error: " + ex.Message + ".')", true);
                //Destruir variables.
                tabla = null;
                txtSQL = null;
                return;
            }
        }

        private int Obtener_numPostIt()
        {
            Int32 numPosIt = 0;

            //Hallar numero de post it por tab
            var query = from tur in consultas.Db.TareaUsuarioRepeticions
                        from tu in consultas.Db.TareaUsuarios
                        from tp in consultas.Db.TareaProgramas
                        where tp.Id_TareaPrograma == tu.CodTareaPrograma
                        && tu.Id_TareaUsuario == tur.CodTareaUsuario
                        && tu.CodProyecto == CodProyecto
                        && tp.Id_TareaPrograma == Constantes.CONST_PostIt
                        && tur.FechaCierre == null
                        select tur;

            numPosIt = query.Count();

            return numPosIt;
        }

        protected void btn_guardar_ultima_actualizacion_Click(object sender, EventArgs e)
        { prActualizarTabEval(Constantes.CONST_subAportes.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString()); Marcar(Constantes.CONST_subAportes.ToString(), CodProyecto.ToString(), CodConvocatoria.ToString(), chk_realizado.Checked); ObtenerDatosUltimaActualizacion(); }

        #endregion
    }
}




