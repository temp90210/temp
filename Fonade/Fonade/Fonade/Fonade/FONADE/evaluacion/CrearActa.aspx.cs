using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;
using Fonade.Negocio;
using Newtonsoft.Json;

namespace Fonade.FONADE.evaluacion
{
    public partial class CrearActa : Base_Page
    {
        string[] _arrQuery;
        string[] _arrCriterio;
        string[] _arrIncidencia;
        private double Total;
        String bPublicado;
        Int32 editar;
        Int32 id;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            bPublicado = Session["publicado"] != null && !string.IsNullOrEmpty(Session["publicado"].ToString()) ? Session["publicado"].ToString() : "false";
            id = Session["idacta"] != null && !string.IsNullOrEmpty(Session["idacta"].ToString()) ? Convert.ToInt32(Session["idacta"].ToString()) : 0;

            //Validacion de permisos, si pertenece a alguno de estos grupos, no se permite editar solo consulta.
            if (usuario.CodGrupo != Constantes.CONST_GerenteInterventor && usuario.CodGrupo != Constantes.CONST_CoordinadorInterventor && usuario.CodGrupo != Constantes.CONST_Interventor)
            { editar = 1; }
            else { editar = 0; }

            //CargarConvocatorias();
            if (!IsPostBack)
                Titulos();
        }

        #region metodos load

        /// <summary>
        /// Establecer acciones sobre los controles de esta pantalla, así como su 
        /// visualización "en algunos casos y de algunos controles".
        /// </summary>
        void Titulos()
        {
            if (!string.IsNullOrEmpty(Request["a"]))
            {
                lbltitulo.Text = "Crear Acta";
                imgadicionarplan.Visible = false;
                lnkadcionarplan.Visible = false;
                panelNegocioGrid.Visible = false;
            }
            else
            {
                if (Session["idacta"] != null)
                {
                    //Ver acta:
                    lbltitulo.Text = "Ver Acta";
                    if (id == 0 || String.IsNullOrEmpty(Session["idacta"].ToString())) { btnCrearActa.Visible = true; }
                    BuscarActaId(id);
                }

                //Si está publicado, los campos estarán deshabilitados.
                if (Boolean.Parse(bPublicado))
                {
                    txtNroActa.Enabled = false;
                    txtnomActa.Enabled = false;
                    txtfecha.Enabled = false;
                    txtObservaciones.Enabled = false;
                    DdlCodConvocatoria.Enabled = false;
                    pnlNegocioPublico.Visible = false;

                    imgadicionarplan.Visible = false;
                    lnkadcionarplan.Visible = false;
                    btnimprimir.Visible = true;
                }
                else /*De lo contrario no.*/ //!Boolean.Parse(bPublicado)
                {
                    if (editar == 0)
                    {
                        txtNroActa.Enabled = false;
                        txtnomActa.Enabled = false;
                        txtfecha.Enabled = false;
                        txtObservaciones.Enabled = false;
                        DdlCodConvocatoria.Enabled = false;
                        pnlNegocioPublico.Visible = false;
                    }
                    else //Si puede editar, podrá adicionar planes de negocio al acta.
                    {
                        txtNroActa.Enabled = true;
                        txtnomActa.Enabled = true;
                        txtfecha.Enabled = true;
                        txtObservaciones.Enabled = true;
                        DdlCodConvocatoria.Enabled = false;

                        //Publicar.
                        pnlNegocioPublico.Visible = true;
                        imgadicionarplan.Visible = true;
                        lnkadcionarplan.Visible = true;
                        btnupdate.Visible = true;
                    }
                }
            }
        }

        #endregion

        #region metodos Crud

        void Crear()
        {
            var oEvaluacionActa = new Datos.EvaluacionActa();

            try
            {
                var idactas = consultas.Db.EvaluacionActas.FirstOrDefault(a => a.Id_Acta == Convert.ToInt32(txtNroActa.Text));
                if (idactas == null)
                {

                    var validaActa = consultas.Db.EvaluacionActas.FirstOrDefault(a => a.NomActa.Contains(txtnomActa.Text));

                    if (validaActa == null)
                    {
                        if (string.IsNullOrEmpty(DdlCodConvocatoria.SelectedValue))
                        {
                            RedirectPage(false, "seleccione una convocatoria");
                        }
                        else
                        {
                            Session["idacta"] = txtNroActa.Text.Trim();
                            Session["publicado"] = false;
                            oEvaluacionActa.NumActa = txtNroActa.Text.Trim();
                            oEvaluacionActa.NomActa = txtnomActa.Text;
                            oEvaluacionActa.FechaActa = Convert.ToDateTime(txtfecha.Text.Trim());
                            oEvaluacionActa.Observaciones = txtObservaciones.Text.Trim();
                            oEvaluacionActa.CodConvocatoria = Convert.ToInt32(DdlCodConvocatoria.SelectedValue);
                            oEvaluacionActa.publicado = false;

                            Session["oEvaluacionActa"] = oEvaluacionActa;

                            consultas.Db.EvaluacionActas.InsertOnSubmit(oEvaluacionActa);
                            consultas.Db.SubmitChanges();
                            var actaid = consultas.Db.EvaluacionActas.FirstOrDefault(a => a.NomActa.Contains(txtnomActa.Text));
                            if (actaid != null && actaid.Id_Acta != 0)
                            {
                                lidacta.Text = actaid.Id_Acta.ToString();
                            }
                            DeshabilitarPanelCrear(false, true);

                            Response.Redirect(Request.Url.ToString().Split('?')[0]);

                            Response.Redirect(Request.Url.ToString().Split('?')[0]);

                            lbltitulo.Text = "Ver Acta";
                            int id = Convert.ToInt32(Session["idacta"].ToString());

                            bPublicado = "false";
                            pnlNegocioPublico.Visible = true;
                            DdlCodConvocatoria.Enabled = false;
                            btnCrearActa.Visible = false;
                            btnupdate.Visible = true;

                            txtNroActa.Enabled = true;
                            txtnomActa.Enabled = true;
                            txtfecha.Enabled = true;
                            txtObservaciones.Enabled = true;
                            DdlCodConvocatoria.Enabled = false;
                            pnlNegocioPublico.Visible = true;

                            imgadicionarplan.Visible = true;
                            lnkadcionarplan.Visible = true;
                        }
                    }
                    else
                    {
                        RedirectPage(false, "Ya existe un Acta con ese nombre");
                        btnupdate.Visible = false;
                        btnCrearActa.Visible = true;
                    }
                }
                else
                {
                    RedirectPage(false, "Ya existe un Acta con ese numero");
                    btnupdate.Visible = false;
                    btnCrearActa.Visible = true;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [WebMethod]
        void BuscarActaId(int idacta)
        {
            if (idacta != 0)
            {
                if (Session["oEvaluacionActa"] != null)
                {

                }
                var actas = consultas.Db.EvaluacionActas.FirstOrDefault(a => a.Id_Acta == idacta);

                if (actas != null && actas.Id_Acta != 0)
                {
                    Session["oEvaluacionActa"] = actas;
                }

                if (Session["oEvaluacionActa"] != null)
                {
                    actas = (Datos.EvaluacionActa)Session["oEvaluacionActa"];
                }

                if (actas != null && actas.Id_Acta != 0)
                {
                    txtNroActa.Text = actas.NumActa;
                    txtnomActa.Text = actas.NomActa;
                    txtfecha.Text = actas.FechaActa.ToShortDateString();
                    txtObservaciones.Text = actas.Observaciones;
                    DdlCodConvocatoria.SelectedValue = actas.CodConvocatoria.ToString();
                    lidacta.Text = actas.Id_Acta.ToString();
                    if (actas.publicado != null && (bool)actas.publicado)
                    {
                        pnlNegocioPublico.Visible = false;
                        bPublicado = "true";
                        DeshabilitarEdit(false);
                        btnCrearActa.Visible = false;
                        //btnupdate.Visible = true;
                    }
                    else
                    {
                        bPublicado = "false";
                        pnlNegocioPublico.Visible = true;
                        DdlCodConvocatoria.Enabled = false;
                        btnCrearActa.Visible = false;
                        btnupdate.Visible = false;
                    }

                    //if (actas.CodConvocatoria != null) CargarProyectoNegocio((int)actas.CodConvocatoria);
                }
            }
        }

        void Actualizar()
        {
            try
            {
                var cActa = consultas.Db.EvaluacionActas.FirstOrDefault(e => e.NomActa.Contains(txtnomActa.Text) && e.Id_Acta == Convert.ToInt32(lidacta.Text));

                if (cActa != null)
                {
                    string txtSQL = "update EvaluacionActa set " +
                     "NumActa = '" + txtNroActa.Text + "', " +
                     "NomActa = '" + txtnomActa.Text + "', " +
                     "FechaActa = '" + Convert.ToDateTime(txtfecha.Text) + "', " +
                     "Observaciones = '" + txtObservaciones.Text + "'";

                    cActa.NomActa = txtnomActa.Text;
                    cActa.NumActa = txtNroActa.Text;
                    cActa.FechaActa = Convert.ToDateTime(txtfecha.Text);
                    cActa.Observaciones = txtObservaciones.Text;
                    if (chkpublico.Checked)
                    {
                        txtSQL = txtSQL + ", Publicado=1";
                        cActa.publicado = chkpublico.Checked;
                    }

                    txtSQL = txtSQL + " where id_acta=" + cActa.Id_Acta;

                    consultas.Db.SubmitChanges();

                    ejecutaReader(txtSQL, 2);

                    ///////////////////////////////////////////

                    if (GrvPlanesNegocio.Rows.Count != 0)
                    {
                        foreach (GridViewRow row in GrvPlanesNegocio.Rows)
                        {

                            var lblidproyecto = row.FindControl("lblidproyecto") as Label;
                            var radio = row.FindControl("rdbViable") as RadioButtonList;


                            var consulta = consultas.Db.EvaluacionActaProyectos.FirstOrDefault(
                                a =>
                                a.CodActa == Convert.ToInt32(lidacta.Text) &&
                                a.CodProyecto == Convert.ToInt32(lblidproyecto.Text));

                            if (consulta != null)
                            {
                                if (radio != null) consulta.Viable = Convert.ToBoolean(radio.SelectedValue);
                                consultas.Db.SubmitChanges();
                            }

                            if (chkpublico.Checked)
                            {
                                if (radio != null && radio.SelectedValue == "1")
                                {
                                    var proyecto = consultas.Db.Proyectos.FirstOrDefault(p => p.Id_Proyecto == Convert.ToInt32(lblidproyecto.Text));

                                    if (proyecto != null && proyecto.Id_Proyecto != 0)
                                    {
                                        proyecto.CodEstado = Constantes.CONST_AsignacionRecursos;
                                    }

                                    // vamos a modificar el proyecto y todas sus relaciones

                                    if (lblidproyecto != null)

                                        consultas.Parameters = new[]
                                                                 {
                                                                     new SqlParameter
                                                                         {
                                                                             ParameterName =  "@idproyecto",
                                                                             Value = Convert.ToInt32(lblidproyecto.Text)
                                                                         }
                                                                 };

                                    DataTable funciones = consultas.ObtenerDataTable("MD_ObtenerFuncionesProyectoNegocio");

                                    if (funciones.Rows.Count != 0)
                                    {
                                        int j = 0;
                                        int x = 0;

                                        foreach (DataRow fRow in funciones.Rows)
                                        {
                                            Array.Resize(ref _arrQuery, j);
                                            Array.Resize(ref _arrCriterio, j);
                                            Array.Resize(ref _arrIncidencia, j);

                                            if (fRow["codconvocatoria"] != null)
                                            {
                                                _arrQuery[j] = fRow["Query"].ToString();
                                                _arrQuery[j] = _arrQuery[j].Replace("Parametros", fRow["Parametros"].ToString());
                                                _arrQuery[j] = _arrQuery[j].Replace("CodConvocatoria", DdlCodConvocatoria.SelectedValue);
                                                _arrCriterio[j] = fRow["id_criterio"].ToString();
                                                _arrIncidencia[j] = fRow["Incidencia"].ToString();

                                                j += 1;
                                                x += 1;
                                            }
                                            else
                                            {
                                                _arrQuery[j] = "";
                                                _arrCriterio[j] = "";
                                                _arrIncidencia[j] = "";
                                            }
                                        }

                                        Total = 0;

                                        for (int i = 0; i <= _arrQuery.Length; i++)
                                        {
                                            if (!string.IsNullOrEmpty(_arrQuery[i]))
                                            {
                                                if (lblidproyecto != null)
                                                {
                                                    string query = "Select isnull(dbo." + _arrQuery[i].Replace("CodProyecto", lblidproyecto.Text) + ",0) as puntaje";
                                                    DataTable puntaje = consultas.ObtenerDataTable(query);
                                                    // obtenemos el puntaje
                                                    if (puntaje.Rows.Count != 0)
                                                    {
                                                        Total += (Convert.ToDouble(puntaje.Rows[0]["puntaje"])
                                                                  * (Convert.ToDouble(_arrIncidencia[i])) / 100);

                                                        //'Guardar puntaje obtenido para el criterio

                                                        var puntajecriteriopriorizacion = new PuntajeCriterioPriorizacion();

                                                        puntajecriteriopriorizacion.CodProyecto = Convert.ToInt32(lblidproyecto.Text);
                                                        puntajecriteriopriorizacion.CodConvocatoria =
                                                            Convert.ToInt32(DdlCodConvocatoria.SelectedValue);
                                                        puntajecriteriopriorizacion.CodCriterioPriorizacion = Convert.ToInt16(_arrCriterio[i]);
                                                        puntajecriteriopriorizacion.Valor = (double)puntaje.Rows[0]["puntaje"];
                                                        consultas.Db.PuntajeCriterioPriorizacions.InsertOnSubmit(puntajecriteriopriorizacion);
                                                        consultas.Db.SubmitChanges();
                                                    }
                                                }


                                            }
                                        }

                                        var puntajeP = new PuntajeTotalPriorizacion();

                                        if (lblidproyecto != null)
                                        {
                                            puntajeP.CodProyecto = Convert.ToInt32(lblidproyecto.Text);
                                            puntajeP.CodConvocatoria = Convert.ToInt32(DdlCodConvocatoria.SelectedValue);
                                            puntajeP.Total = Total;
                                            consultas.Db.PuntajeTotalPriorizacions.InsertOnSubmit(puntajeP);
                                            consultas.Db.SubmitChanges();

                                        }
                                    }

                                    // fin de la modificacion ///
                                }

                                else
                                {
                                    // si no esta chequeado

                                    if (lblidproyecto != null)

                                        consultas.Parameters = new[]
                                                                 {
                                                                     new SqlParameter
                                                                         {
                                                                             ParameterName =  "@idproyecto",
                                                                             Value = Convert.ToInt32(lblidproyecto.Text)
                                                                         }
                                                                 };

                                    consultas.InsertarDataTable("MD_ActualizarProyectoNegocio");
                                }

                            }

                        }
                    }

                    ////////////////////////////////////////////////////////////

                    //// validamos que si el check este activo para ocultar los registros.
                    if (chkpublico.Checked)
                    {
                        pnlNegocioPublico.Visible = false;
                        panelNegocioGrid.Visible = true;
                        DeshabilitarEdit(false);
                    }
                    else
                    {
                        DdlCodConvocatoria.Enabled = false;
                        return;
                    }

                }

                //SELECT Id_Acta FROM EvaluacionActa WHERE NomActa='" & txtNomActa & "' && id_acta<>"&CodActa
            }
            catch (Exception exception)
            {

                throw new Exception(exception.Message);
            }

            btnupdate.Visible = false;
            btnimprimir.Visible = true;
            btnCrearActa.Visible = false;
        }

        [WebMethod]
        public static string EliminarProyecto(int idacta, int codproyecto)
        {
            var consulta = new Datos.Consultas();

            string mensajeDeError;

            var eproyecto = consulta.Db.EvaluacionActaProyectos.FirstOrDefault(p => p.CodActa == idacta && p.CodProyecto == codproyecto);

            if (eproyecto != null)
            {
                consulta.Db.EvaluacionActaProyectos.DeleteOnSubmit(eproyecto);
                consulta.Db.SubmitChanges();
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

        #region deshabilitar controles

        void DeshabilitarPanelCrear(bool bandera, bool panel)
        {
            // deshabilita los controles al momento que crea el acta.

            if (panel)
            {

                pnlNegocioPublico.Visible = true;
                DdlCodConvocatoria.Enabled = bandera;
                panelNegocioGrid.Visible = true;
                btnCrearActa.Visible = false;
                btnimprimir.Visible = false;
                btnupdate.Visible = true;
            }


        }

        void DeshabilitarEdit(bool bandera)
        {
            // los controles al momento de editar

            txtNroActa.Enabled = bandera;
            txtnomActa.Enabled = bandera;
            txtfecha.Enabled = bandera;
            txtObservaciones.Enabled = bandera;
            DdlCodConvocatoria.Enabled = bandera;
            btnupdate.Visible = false;
            btnDate2.Enabled = false;
            btnimprimir.Visible = true;
            btnCrearActa.Visible = false;
            panelNegocioGrid.Visible = true;
            CalendarExtender1.Enabled = false;

        }

        #endregion

        #region Eventos Botones

        protected void btnCrearActa_Click(object sender, EventArgs e)
        {
            Crear();
        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            Actualizar();
        }

        #endregion

        #region Eventos Grid

        /// <summary>
        /// RowDataBpund...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrvPlanesNegocio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var img = e.Row.FindControl("imgborrar") as Image;
                var lnk = e.Row.FindControl("lnk_del") as LinkButton;
                var lblviable = e.Row.FindControl("lblviable") as Label;
                var radio = e.Row.FindControl("rdbViable") as RadioButtonList;
                var lblviableevaluador = e.Row.FindControl("lblviableevaluador") as Label;

                //if ((bool)ViewState["publico"])
                if (Boolean.Parse(bPublicado))
                {
                    lnk.Visible = false;
                    img.Visible = false;
                    radio.Enabled = false;
                }
                else
                {
                    if (usuario.CodGrupo != Constantes.CONST_GerenteInterventor &&
                         usuario.CodGrupo != Constantes.CONST_CoordinadorInterventor
                         && usuario.CodGrupo != Constantes.CONST_Interventor)
                    { radio.Text = string.Empty; }
                    else
                    { radio.Enabled = false; }

                    if (editar == 1)
                    {
                        lnk.Visible = true;
                        img.Visible = true;
                    }
                    else
                    {
                        lnk.Visible = false;
                        img.Visible = false;
                    }
                }

                if (lblviable != null)
                {
                    if (lblviable.Text == "True")
                    {
                        radio.SelectedValue = "1";
                    }
                    else if (lblviableevaluador.Text.Trim() == "SI" && lblviable.Text == "True")
                    {
                        radio.SelectedValue = "1";
                    }
                    else
                    {
                        radio.SelectedValue = "0";
                    }
                }
            }
        }

        /// <summary>
        /// RowCommand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrvPlanesNegocio_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "proyecto":
                    Session["CodProyecto"] = e.CommandArgument.ToString();
                    Session["CodConvocatoria"] = DdlCodConvocatoria.SelectedValue;
                    Response.Redirect("EvaluacionFrameSet.aspx");
                    break;
                case "evaluador":
                    Session["codcontacto"] = e.CommandArgument;
                    Redirect(null, "VerPerfilContacto.aspx", "_blank", "menubar=0,scrollbars=1,width=710,height=400,top=100");
                    break;
                default:
                    break;
            }
        }

        protected void GrvPlanesNegocio_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GrvPlanesNegocio.PageIndex = e.NewPageIndex;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkadcionarplan_Click(object sender, EventArgs e)
        {
            Session["CodConvocatoria"] = DdlCodConvocatoria.SelectedValue;
            Session["CodActa"] = txtNroActa.Text;
            Redirect(null, "AdicionarProyectoActa.aspx", "_Blank", "width=730,height=585");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgadicionarplan_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodConvocatoria"] = DdlCodConvocatoria.SelectedValue;
            Session["CodActa"] = txtNroActa.Text;
            Redirect(null, "AdicionarProyectoActa.aspx", "_Blank", "width=730,height=585");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lds_planesnegocio_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            int idActaEval = -1;

            try
            {
                idActaEval = !string.IsNullOrEmpty(Session["idacta"].ToString()) ? Convert.ToInt32(Session["idacta"].ToString()) : 0;
            }
            catch (NullReferenceException) { }

            var result = from p in consultas.Db.pr_ProyectosEvaluados(idActaEval, 0) select p;

            e.Result = result.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lds_convocatoria_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var convocatoria = from c in consultas.Db.Convocatorias
                               orderby c.NomConvocatoria
                               select new
                               {
                                   Id = c.Id_Convocatoria,
                                   Nombre = c.NomConvocatoria
                               };

            e.Result = convocatoria.ToList();
        }
    }
}