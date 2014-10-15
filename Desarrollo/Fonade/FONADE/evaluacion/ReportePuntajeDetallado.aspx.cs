#region Diego Quiñonez

// <Author>Diego Quiñonez</Author>
// <Fecha>16 - 03 - 2014</Fecha>
// <Archivo>ReportePuntajeDetallado.aspx.cs</Archivo>

#endregion

#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using Fonade.Negocio;

#endregion

namespace Fonade.FONADE.evaluacion
{
    public partial class ReportePuntajeDetallado : Base_Page
    {
        #region Propiedades 

        private double _subTotalSolicitado, _subTotalRecomendado, _subTotalRecomendadoAprobados, _subTotalRecomendadoViables
                       , _subTotalAprobados, _subTotalViables, _subTotalAvalados, _subTotalProyectos, _totalProyectos
                       , _totalSolicitado, _totalRecomendado, _totalRecomendadoAprobados, _totalRecomendadoViables,
                       _totalAprobados, _totalViables, _totalAvaslados;

        private int _numAspectos = 5, PuntajeAspecto, PuntajeTotal, PuntajeComercial, PuntajeTecnico, PuntajeOrganizacional, PuntajeFinanciero;

        #endregion

       

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["codConvocatoria"] != null && !string.IsNullOrEmpty(Session["codConvocatoria"].ToString()))
                {
                     CargarReporteDetallado();
                     
                }
                else
                {
                    CargarReporteDetallado(); exportar();
                }
               

            }
        }

        void exportar()
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "inline;filename=ReporteEvaluacion.xls");
            Response.Charset = "";
            EnableViewState = false;
            var oStringWriter = new System.IO.StringWriter();
            var oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            DtlReporteDetallado.RenderControl(oHtmlTextWriter);
            Response.Write(oStringWriter.ToString());
            Response.End();
        }

        private void CargarReporteDetallado()
        {
            if (!string.IsNullOrEmpty(Session["codConvocatoria"].ToString()))
            {
                ViewState["convocatoria"] = Session["codConvocatoria"];
            }
            else
            {
                ViewState["convocatoria"] = 1;
            }

            if (Convert.ToInt32(ViewState["convocatoria"].ToString()) >= 1)
            {
                _numAspectos = 6;
            }

            ObtenerVariables(Convert.ToInt32(ViewState["convocatoria"].ToString()));
        }

        private void ObtenerVariables(int convocatoria)
        {
            try
            {
                var puntaje = consultas.Db.ConvocatoriaCampos.Join(consultas.Db.Campos,
                                                                   cc => cc.codCampo,
                                                                   c => c.id_Campo
                                                                   , (cc, c) => new
                                                                                    {
                                                                                        CampoId = c.id_Campo,
                                                                                        codcampo = c.codCampo,
                                                                                        codigoCampo = cc.codCampo,
                                                                                        cPuntaje = cc.Puntaje,
                                                                                        CodConvocatoria =
                                                                                    cc.codConvocatoria,
                                                                                    }).Where(
                                                                                        c =>
                                                                                        c.codcampo == null &&
                                                                                        c.CodConvocatoria ==
                                                                                        convocatoria && c.CampoId != 6).
                    Sum(s => s.cPuntaje);
                ViewState["puntaje"] = puntaje;

                var firstOrDefault = consultas.Db.Convocatorias.FirstOrDefault(c => c.Id_Convocatoria == convocatoria);

                if (firstOrDefault != null)
                {
                    ViewState["anio"] = firstOrDefault.FechaFin.Year;
                    ViewState["nombreconvocatoria"] = firstOrDefault.NomConvocatoria;
                }
                else
                {
                    ViewState["anio"] = string.Empty;
                    ViewState["nombreconvocatoria"] = string.Empty;
                }


                bltituloConvocatoria.Text = ViewState["nombreconvocatoria"].ToString();

                ReporteDetalladoEvaluacion();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ReporteDetalladoEvaluacion()
        {
            consultas.Parameters = null;

            consultas.Parameters = new[] { new SqlParameter 
                                               {
                                                   ParameterName = "@codconvocatoria",
                                                   Value =  Convert.ToInt32(ViewState["convocatoria"])
                                               },new SqlParameter
                                                     {
                                                         ParameterName = "@txtViable",
                                                         Value = !string.IsNullOrEmpty(Request["viable"]) ? Request["viable"].ToString() : string.Empty
                                                     }
            
            };
            DataTable dtDetallado = consultas.ObtenerDataTable("MD_ReporteDetalladoEvaluacion");

            if (dtDetallado!=null && dtDetallado.Rows.Count!=0)
            {
                DtlReporteDetallado.DataSource = dtDetallado;
                DtlReporteDetallado.DataBind();
            }
        }

        public List<string> Obtenerpuntaje(int idproyecto,int campo)
        {

          var lstpuntaje =new List<string>();
         
            string query = "select puntaje from evaluacioncampo ec  inner join campo c on c.id_campo = ec.codcampo  " 
                            +"inner join campo v on v.id_campo = c.codcampo  inner join campo a on a.id_campo = v.codcampo where codproyecto= " + idproyecto 
                            +  " and codconvocatoria=" + Convert.ToInt32(ViewState["convocatoria"]) + " and a.id_campo=" + campo 
                            + " order by a.id_campo,v.campo Asc";

            consultas.Parameters = null;

            DataTable puntaje = consultas.ObtenerDataTable(query, "text");

            if (puntaje!=null && puntaje.Rows.Count!=0)
            {
                lstpuntaje.AddRange(from DataRow row in puntaje.Rows select row["puntaje"].ToString());
            }

         return lstpuntaje;
        }

        protected void DtlReporteDetallado_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                #region Variables Generales

                // GENERALES
                var puntajea = e.Item.FindControl("lblga") as Label;
                var puntajeb = e.Item.FindControl("lblgb") as Label;
                var puntajec = e.Item.FindControl("lblgc") as Label;
                var puntajed = e.Item.FindControl("lblgd") as Label;
                var puntajeE = e.Item.FindControl("lblge") as Label;
                var lpuntajetotal = e.Item.FindControl("lpuntajetotal") as Label;
                var puntajetotalG = e.Item.FindControl("lbltotalG") as Label;
                // FIN //

                #endregion

                #region Variables Comerciales

                // COMERCIALES ///

                var lblcc = e.Item.FindControl("lblcc") as Label;
                var lblcg = e.Item.FindControl("lblcg") as Label;
                var lblch = e.Item.FindControl("lblch") as Label;
                var lblci = e.Item.FindControl("lblci") as Label;
                var lblcj = e.Item.FindControl("lblcj") as Label;
                var lblck = e.Item.FindControl("lblck") as Label;
                var lblcl = e.Item.FindControl("lblcl") as Label;
                var lblcm = e.Item.FindControl("lblcm") as Label;
                var lblcn = e.Item.FindControl("lblcn") as Label;
                var lblco = e.Item.FindControl("lblco") as Label;
                var lblcp = e.Item.FindControl("lblcp") as Label;
                var lblcq = e.Item.FindControl("lblcq") as Label;
                var lblcr = e.Item.FindControl("lblcr") as Label;
                var lblcs = e.Item.FindControl("lblcs") as Label;
                var lbltotalC = e.Item.FindControl("lbltotalC") as Label;

                //*****************************////

                #endregion

                #region Variables Tecnicos

                var lbltt = e.Item.FindControl("lbltt") as Label;
                var lbltu = e.Item.FindControl("lbltu") as Label;
                var lbltv = e.Item.FindControl("lbltv") as Label;
                var lbltw = e.Item.FindControl("lbltw") as Label;
                var lbltx = e.Item.FindControl("lbltx") as Label;
                var lblty = e.Item.FindControl("lblty") as Label;
                var lblTotalT = e.Item.FindControl("lblTotalT") as Label;

                #endregion

                #region Variables Organizacionales

                var lbloz = e.Item.FindControl("lbloz") as Label;
                var lbloaa = e.Item.FindControl("lbloaa") as Label;
                var lbloab = e.Item.FindControl("lbloab") as Label;
                var lbloac = e.Item.FindControl("lbloac") as Label;
                var lbload = e.Item.FindControl("lbload") as Label;
                var lbloae = e.Item.FindControl("lbloae") as Label;
                var lbloaf = e.Item.FindControl("lbloaf") as Label;
                var lblTotalO = e.Item.FindControl("lblTotalO") as Label;

                #endregion

                #region Variables Financieros

                var lblfag = e.Item.FindControl("lblfag") as Label;
                var lblfah = e.Item.FindControl("lblfah") as Label;
                var lblfai = e.Item.FindControl("lblfai") as Label;
                var lblfaj = e.Item.FindControl("lblfaj") as Label;
                var lblfak = e.Item.FindControl("lblfak") as Label;
                var lblTotalF = e.Item.FindControl("lblTotalF") as Label;

                #endregion

                #region Variable Medio Ambiente

                var lblTotalM = e.Item.FindControl("lblTotalM") as Label;

                #endregion

                var idproyecto = e.Item.FindControl("lblidproyecto") as Label;
                var viable = e.Item.FindControl("lblviable") as Label;
                var solicitado = e.Item.FindControl("lblsolicitado") as Label;
                var recomendado = e.Item.FindControl("lblrecomendado") as Label;

                if (puntajea != null)
                {
                    if (idproyecto != null)
                    {

                        for (int i = 1; i < _numAspectos; i++)
                        {
                            PuntajeAspecto = 0;
                            PuntajeComercial = 0;
                            List<string> mipuntaje = Obtenerpuntaje(Convert.ToInt32(idproyecto.Text), i);

                            foreach (string mpunt in mipuntaje)
                            {
                                #region Generales

                                if (i < 6)
                                {
                                    /* *********************************** INCIO GENERALES *******************************/

                                    if (string.IsNullOrEmpty(puntajea.Text))
                                    {
                                        puntajea.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(puntajeb.Text))
                                    {
                                        puntajeb.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(puntajec.Text))
                                    {
                                        puntajec.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(puntajed.Text))
                                    {
                                        puntajed.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(puntajeE.Text))
                                    {
                                        puntajeE.Text = mpunt;
                                    }

                                    /* *********************************** FIN GENERALES *******************************/

                                    #endregion

                                    #region Comerciales

                                    /***********************************    INICIO COMERCIALES   *******************************/
                                    //else if (i == 2)
                                    //{
                                    if (string.IsNullOrEmpty(lblcc.Text))
                                    {
                                        lblcc.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcg.Text))
                                    {
                                        lblcg.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblch.Text))
                                    {
                                        lblch.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblci.Text))
                                    {
                                        lblci.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcj.Text))
                                    {
                                        lblcj.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblck.Text))
                                    {
                                        lblck.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcl.Text))
                                    {
                                        lblcl.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcm.Text))
                                    {
                                        lblcm.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcn.Text))
                                    {
                                        lblcn.Text = mpunt;
                                    }

                                    else if (string.IsNullOrEmpty(lblco.Text))
                                    {
                                        lblco.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcp.Text))
                                    {
                                        lblcp.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcq.Text))
                                    {
                                        lblcq.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcr.Text))
                                    {
                                        lblcr.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblcs.Text))
                                    {
                                        lblcs.Text = mpunt;
                                    }

                                    //}
                                    /* *********************************** FIN COMERCIALES *******************************/

                                    #endregion

                                    #region Tecnicos

                                    /***********************************    INICIO TECNICOS   *******************************/
                                    //else if (i == 3)
                                    //{
                                    if (string.IsNullOrEmpty(lbltt.Text))
                                    {
                                        lbltt.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lbltu.Text))
                                    {
                                        lbltu.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lbltv.Text))
                                    {
                                        lbltv.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lbltw.Text))
                                    {
                                        lbltw.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lbltx.Text))
                                    {
                                        lbltx.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblty.Text))
                                    {
                                        lblty.Text = mpunt;
                                    }

                                    //}

                                    /* *********************************** FIN TECNICOS *******************************/

                                    #endregion

                                    #region Organizacionales

                                    /***********************************    INICIO ORGANIZACIONALES   *******************************/
                                    //else if (i == 4)
                                    //{
                                    if (string.IsNullOrEmpty(lbloz.Text))
                                    {
                                        lbloz.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lbloaa.Text))
                                    {
                                        lbloaa.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lbloab.Text))
                                    {
                                        lbloab.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lbloac.Text))
                                    {
                                        lbloac.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lbload.Text))
                                    {
                                        lbload.Text = mpunt;
                                    }


                                    else if (string.IsNullOrEmpty(lbloae.Text))
                                    {
                                        lbloae.Text = mpunt;
                                    }

                                    else if (string.IsNullOrEmpty(lbloaf.Text))
                                    {
                                        lbloaf.Text = mpunt;
                                    }

                                    //}

                                    /* *********************************** FIN ORGANIZACIONALES *******************************/

                                    #endregion

                                    #region Financieros

                                    /***********************************    INICIO FINANCIEROS   *******************************/
                                    //else if (i == 5)
                                    //{
                                    if (string.IsNullOrEmpty(lblfag.Text))
                                    {
                                        lblfag.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblfah.Text))
                                    {
                                        lblfah.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblfai.Text))
                                    {
                                        lblfai.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblfaj.Text))
                                    {
                                        lblfaj.Text = mpunt;
                                    }
                                    else if (string.IsNullOrEmpty(lblfak.Text))
                                    {
                                        lblfak.Text = mpunt;
                                    }

                                    //}

                                    /* *********************************** FIN FINANCIEROS *******************************/

                                    #endregion


                                    if (!string.IsNullOrEmpty(mpunt))
                                    {
                                        PuntajeAspecto += Convert.ToInt16(mpunt);

                                        if (i == 2)
                                        {
                                            PuntajeComercial += Convert.ToInt16(mpunt);
                                        }
                                        if (i == 3)
                                        {
                                            PuntajeTecnico += Convert.ToInt16(mpunt); 
                                        }
                                        if (i == 4)
                                        {
                                            PuntajeOrganizacional += Convert.ToInt16(mpunt);
                                        }

                                        if (i == 5)
                                        {
                                            PuntajeFinanciero += Convert.ToInt16(mpunt);
                                        }


                                        if (i < 6)
                                        {
                                            PuntajeTotal += Convert.ToInt16(mpunt);
                                            lpuntajetotal.Text = PuntajeTotal.ToString();
                                        }
                                    }

                                    #region Totales

                                    if (i == 1)
                                    {
                                        puntajetotalG.Text = PuntajeAspecto.ToString();
                                    }

                                    if (i == 2)
                                    {
                                        lbltotalC.Text = PuntajeComercial.ToString();
                                    }

                                    if (i == 3)
                                    {
                                        lblTotalT.Text = PuntajeTecnico.ToString();
                                    }
                                    if (i == 4)
                                    {
                                        lblTotalO.Text = PuntajeOrganizacional.ToString();
                                    }

                                    if (i == 5)
                                    {
                                        lblTotalF.Text = PuntajeFinanciero.ToString();
                                    }

                                    #endregion

                                }
                            }

                            if (viable != null)
                            {
                                if (viable.Text == "SI")
                                {
                                    _subTotalViables += 1;
                                    _totalViables += 1;
                                    _subTotalRecomendadoViables += Convert.ToInt32(recomendado.Text);
                                    _totalRecomendado += Convert.ToInt32(solicitado.Text);
                                }

                            }
                            consultas.Parameters = null;
                            consultas.Parameters = new[]
                                                       {
                                                           new SqlParameter
                                                               {
                                                                   ParameterName = "@idproyecto",
                                                                   Value = Convert.ToInt32(idproyecto.Text)
                                                               }
                                                       };
                            DataTable dtProyecto = consultas.ObtenerDataTable("MD_obtenerTabs");

                            if (dtProyecto.Rows.Count != 0)
                            {
                                _subTotalAvalados += 1;
                                _totalAvaslados += 1;
                            }

                            _subTotalSolicitado += Convert.ToInt32(solicitado.Text);
                            _totalSolicitado += Convert.ToInt32(solicitado.Text);
                            _subTotalRecomendado += Convert.ToInt32(recomendado.Text);
                            _totalRecomendado += Convert.ToInt32(recomendado.Text);
                            _subTotalProyectos += 1;
                            _totalProyectos += 1;

                            if (PuntajeTotal >= Convert.ToInt32(ViewState["puntaje"].ToString()))
                            {
                                _subTotalAprobados += 1;
                                _totalAprobados += 1;
                                _subTotalRecomendadoAprobados += Convert.ToInt32(recomendado.Text);
                                _subTotalAprobados += _subTotalRecomendadoAprobados + Convert.ToInt32(recomendado.Text);
                                _totalRecomendadoAprobados += Convert.ToInt32(recomendado.Text);
                            }



                        }
                    }
                }

            }
        }


    }
}