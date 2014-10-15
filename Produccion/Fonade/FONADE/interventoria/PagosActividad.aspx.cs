using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Datos;
using System.Data.SqlClient;
using System.Configuration;

namespace Fonade.FONADE.interventoria
{
    public partial class PagosActividad : Negocio.Base_Page //System.Web.UI.Page
    {
        #region Variables globales.
        String CodProyecto;
        String TipoPago;
        String txtSQL;
        /// <summary>
        /// Inidica el valor para redireccionar a "PagosActividad.aspx (si es TRUE)" o a "PagosActividadSinConvenio.aspx" (si es FALSE).
        /// </summary>
        Boolean esFechaFin;
        int prorroga;
        int prorrogaTotal;
        String CodPagoActividad;
        /// <summary>
        /// Código del estado.
        /// </summary>
        String CodEstado;

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                CodProyecto = Session["CodProyecto"] != null && !string.IsNullOrEmpty(Session["CodProyecto"].ToString()) ? Session["CodProyecto"].ToString() : "0";
                TipoPago = Session["TipoPago"] != null && !string.IsNullOrEmpty(Session["TipoPago"].ToString()) ? Session["TipoPago"].ToString() : "0";
                CodEstado = Session["CodEstado"] != null && !string.IsNullOrEmpty(Session["CodEstado"].ToString()) ? Session["CodEstado"].ToString() : "0";
            }
            catch { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); }

            //Se valida que si tenga datos "válidos".
            if (CodProyecto == "0" || TipoPago == "0") { ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "window.close();", true); }

            if (!IsPostBack)
            {
                #region Obtener el valor de la prórroga para sumarla a la constante de meses generar la tabla.
                prorroga = 0;
                prorroga = ObtenerProrroga(CodProyecto.ToString());
                prorrogaTotal = prorroga + Constantes.CONST_Meses;
                #endregion

                #region Establecer el título y demás datos correspondientes.

                if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoActividad)
                {
                    lbl_titutlo.Text = "PAGOS POR ACTIVIDAD";
                    lnkBtn_Add_PagosActividadSinConvenio.Text = "Adicionar Pago por Actividad";
                }
                else if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoNomina)
                {
                    lbl_titutlo.Text = "PAGOS DE NOMINA";
                    lnkBtn_Add_PagosActividadSinConvenio.Text = "Adicionar Pago de Nómina";
                }

                #endregion

                CargarActividadCargo();
                CargarBeneficiarios();
                CargarConceptos();
                CargarFomaDePago();
                CargarMeses();
                CargarNumSolicitudesRechazadas();
                CargarPagosActividades();
                ValidarConvenios();
            }
        }

        /// <summary>
        /// Se inicia Validación de convenios relacionados con la convocatoria y si estos convenios están vigentes.
        /// </summary>
        private void ValidarConvenios()
        {
            //Inicializar variables.
            DataTable RSAux = new DataTable();
            DateTime FechaFin = new DateTime();

            try
            {
                txtSQL = " SELECT Max(codConvocatoria)as CodConvocatoria FROM ConvocatoriaProyecto WHERE viable=1 and CodProyecto = " + CodProyecto;

                RSAux = consultas.ObtenerDataTable(txtSQL, "text");

                if (RSAux.Rows.Count > 0)
                {
                    txtSQL = " SELECT CodConvenio FROM Convocatoria WHERE Id_Convocatoria = " + RSAux.Rows[0]["CodConvocatoria"].ToString();
                    RSAux = consultas.ObtenerDataTable(txtSQL, "text");

                    if (RSAux.Rows.Count > 0)
                    {
                        if (RSAux.Rows[0]["CodConvenio"].ToString() != "")
                        {
                            #region Si tiene convenio...

                            txtSQL = "SELECT FechaFin FROM Convenio WHERE Id_Convenio = " + RSAux.Rows[0]["CodConvenio"].ToString();
                            RSAux = consultas.ObtenerDataTable(txtSQL, "text");

                            if (RSAux.Rows.Count > 0)
                            {
                                try { FechaFin = DateTime.Parse(RSAux.Rows[0]["FechaFin"].ToString()); }
                                catch { }

                                if (FechaFin >= DateTime.Now)
                                {
                                    //Establecer variable en TRUE;
                                    esFechaFin = true;

                                    if (TipoPago == Constantes.CONST_TipoPagoActividad.ToString())
                                    {
                                        lnkBtn_Add_PagosActividadSinConvenio.Text = "Adicionar Pago por Actividad";
                                    }
                                    else if (TipoPago == Constantes.CONST_TipoPagoNomina.ToString())
                                    {
                                        lnkBtn_Add_PagosActividadSinConvenio.Text = "Adicionar Pago de Nómina";
                                    }
                                }
                                else
                                {
                                    //Establecer variable en FALSE;
                                    esFechaFin = false;

                                    if (TipoPago == Constantes.CONST_TipoPagoActividad.ToString())
                                    {
                                        lnkBtn_Add_PagosActividadSinConvenio.Text = "Adicionar Pago por Actividad";
                                    }
                                    else if (TipoPago == Constantes.CONST_TipoPagoNomina.ToString())
                                    {
                                        lnkBtn_Add_PagosActividadSinConvenio.Text = "Adicionar Pago de Nómina";
                                    }
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Se establece el MISMO valor que como si la "FechaFin" no fuera mayor o igual a la fecha actual.

                            //Establecer variable en FALSE;
                            esFechaFin = false;

                            if (TipoPago == Constantes.CONST_TipoPagoActividad.ToString())
                            {
                                lnkBtn_Add_PagosActividadSinConvenio.Text = "Adicionar Pago por Actividad";
                            }
                            else if (TipoPago == Constantes.CONST_TipoPagoNomina.ToString())
                            {
                                lnkBtn_Add_PagosActividadSinConvenio.Text = "Adicionar Pago de Nómina";
                            }

                            #endregion
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Cargar los Pagos de actividades.
        /// </summary>
        private void CargarPagosActividades()
        {
            //Inicializar variables.
            DataTable tabla = new DataTable();

            try
            {
                txtSQL = " SELECT Id_PagoActividad, NomPagoActividad, Estado FROM PagoActividad WHERE CodProyecto = " + CodProyecto + " AND TipoPago = " + TipoPago;
                tabla = consultas.ObtenerDataTable(txtSQL, "text");

                Session["tabla_actividades"] = tabla;

                gv_PagosActividades.DataSource = tabla;
                gv_PagosActividades.DataBind();
            }
            catch { }
        }

        /// <summary>
        /// Adicionar pago.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>mo
        protected void imgBtn_Add_PagosActividadSinConvenio_Click(object sender, ImageClickEventArgs e)
        {
            ValidarConvenios();
            if (esFechaFin)
            {
                LimpiarCampos(); //Redirect(null, "PagosActividad.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
            }
            else { Redirect(null, "PagosActividadSinConvenio.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100"); }
        }

        /// <summary>
        /// Adicionar pago.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkBtn_Add_PagosActividadSinConvenio_Click(object sender, EventArgs e)
        {
            ValidarConvenios();
            if (esFechaFin)
            {
                LimpiarCampos(); //Redirect(null, "PagosActividad.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
            }
            else { Redirect(null, "PagosActividadSinConvenio.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100"); }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Limpiar los campos "permitir generar datos.".
        /// </summary>
        private void LimpiarCampos()
        {
            lbl_newOrEdit.Text = "NUEVO";
            pnl_PagosActividad.Visible = false;
            pnl_Datos.Visible = true;
            btn_accion.Text = "Adicionar";
            ddl_Tipo.SelectedValue = "0";
            lbl_tipo_seleccionado.Text = "";
            hdf_tipo.Value = "";
            ddl_NumSolicitudRechazada.SelectedValue = "0";
            hdf_numsolicitud.Value = "";
            ddl_meses.SelectedValue = "0";
            lbl_mes_seleccionado.Text = "";
            ddl_actividad_cargo.SelectedValue = "0";
            lbl_loaded_actividad_cargo.Text = "";
            ddl_Concepto.SelectedValue = "0";
            ddl_CodPagoBeneficiario.SelectedValue = "0";
            lblNombreBeneficiario.Text = "";
            ddl_CodPagoForma.SelectedValue = "0";
            lbl_FormaDePago.Text = "";
            Observaciones.Text = "";
            CantidadDinero.Text = "";
            btn_accion.Visible = true;
        }

        /// <summary>
        /// Se valida que si el pago ha estado en un acta, no se pueda borrar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PagosActividades_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                try
                {
                    var lbl_id = e.Row.FindControl("lbl_id") as Label;
                    var lnk_elim = e.Row.FindControl("lnk_eliminar") as LinkButton;
                    var imgEditar = e.Row.FindControl("imgeditar") as Image;
                    var lbl_Est = e.Row.FindControl("lbl_Estado") as Label;

                    if (lbl_id != null && imgEditar != null && lbl_Est != null)
                    {
                        //Establecer el valor a mostar en el Label "Estado".
                        lbl_Est.Text = EstadoPago(Int32.Parse(lbl_Est.Text));

                        txtSQL = " SELECT Aprobado FROM PagosActaSolicitudPagos WHERE CodPagoActividad = " + lbl_id.Text;
                        var rsPagoActa = consultas.ObtenerDataTable(txtSQL, "text");

                        if (rsPagoActa.Rows.Count == 0)
                        {
                            if (lnk_elim != null)
                            { lnk_elim.Visible = true; }
                            if (imgEditar != null)
                            { imgEditar.Visible = true; }

                        }
                        //else
                        //{
                        //    lnk_elim.Visible = false;
                        //    imgEditar.Visible = false;
                        //}
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Paginación.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PagosActividades_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["tabla_actividades"] as DataTable;

            if (dt != null)
            {
                gv_PagosActividades.PageIndex = e.NewPageIndex;
                gv_PagosActividades.DataSource = dt;
                gv_PagosActividades.DataBind();
            }
        }

        /// <summary>
        /// RowCommand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_PagosActividades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "eliminar")
            {

            }
            if (e.CommandName == "editar")
            {
                string[] palabras = e.CommandArgument.ToString().Split(';');
                CodPagoActividad = palabras[0];
                Session["CodPagoActividad"] = CodPagoActividad;
                CargarDatosPagoSeleccionado(CodPagoActividad);
                pnl_PagosActividad.Visible = false;
                pnl_Datos.Visible = true;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Cargar las solicitudes rechazadas.
        /// </summary>
        private void CargarNumSolicitudesRechazadas()
        {
            //Inicializar variables.
            DataTable rs = new DataTable();
            ListItem item = new ListItem();

            try
            {
                txtSQL = " SELECT Id_PagoActividad FROM PagoActividad WHERE CodProyecto = " + CodProyecto + " AND Estado = " + Constantes.CONST_EstadoRechazadoFA;
                rs = consultas.ObtenerDataTable(txtSQL, "text");

                ddl_NumSolicitudRechazada.Items.Clear();

                //Valor por defecto.
                item = new ListItem();
                item.Text = "Seleccione una solicitud";
                item.Value = "0";
                ddl_NumSolicitudRechazada.Items.Add(item);

                foreach (DataRow row in rs.Rows)
                {
                    item = new ListItem();
                    item.Text = row["Id_PagoActividad"].ToString();
                    item.Value = row["Id_PagoActividad"].ToString();
                    ddl_NumSolicitudRechazada.Items.Add(item);
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Cargar los meses "con su prórroga ya obtenida".
        /// </summary>
        private void CargarMeses()
        {
            //Inicializar variables.
            DataTable rs = new DataTable();
            ListItem item = new ListItem();

            try
            {
                txtSQL = " SELECT Id_PagoActividad FROM PagoActividad WHERE CodProyecto = " + CodProyecto + " AND Estado = " + Constantes.CONST_EstadoRechazadoFA;
                rs = consultas.ObtenerDataTable(txtSQL, "text");

                ddl_meses.Items.Clear();

                //Valor por defecto.
                item = new ListItem();
                item.Text = "Seleccione";
                item.Value = "0";
                ddl_meses.Items.Add(item);

                for (int i = 1; i < prorrogaTotal + 1; i++)
                {
                    item = new ListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    ddl_meses.Items.Add(item);
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Dependiendo de la validación interna, se carga la "Actividad" o "Cargo" correspondiente.
        /// </summary>
        private void CargarActividadCargo()
        {
            //Inicializar variables.
            DataTable rsActividad = new DataTable();
            ListItem item = new ListItem();

            try
            {
                if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoActividad)
                {
                    txtSQL = "SELECT Id_Actividad, Nomactividad " +
                                 " FROM ProyectoActividadPOInterventor " +
                                 " WHERE CodProyecto = " + CodProyecto + " ORDER BY Item ";
                    lbl_Actividad_Cargo.Text = "Actividad";
                }
                else if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoNomina)
                {
                    txtSQL = " SELECT Id_Nomina AS Id_Actividad, Cargo AS NomActividad " +
                             " FROM InterventorNomina WHERE CodProyecto = " + CodProyecto +
                             " ORDER BY Id_Nomina ";
                    lbl_Actividad_Cargo.Text = "Cargo";
                }

                ddl_actividad_cargo.Items.Clear();

                //Valor por defecto.
                item = new ListItem();
                item.Text = "Seleccione";
                item.Value = "0";
                ddl_actividad_cargo.Items.Add(item);

                rsActividad = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow row in rsActividad.Rows)
                {
                    item = new ListItem();
                    item.Text = row["NomActividad"].ToString();
                    item.Value = row["Id_Actividad"].ToString();
                    ddl_actividad_cargo.Items.Add(item);
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Cargar los conceptos.
        /// </summary>
        private void CargarConceptos()
        {
            //Inicializar variables.
            DataTable RS = new DataTable();
            ListItem item = new ListItem();

            try
            {
                txtSQL = " SELECT * FROM PagoConcepto ";

                ddl_Concepto.Items.Clear();

                //Valor por defecto.
                item = new ListItem();
                item.Text = "Seleccione el concepto";
                item.Value = "0";
                ddl_Concepto.Items.Add(item);

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow row in RS.Rows)
                {
                    item = new ListItem();
                    item.Text = row["NomPagoConcepto"].ToString();
                    item.Value = row["Id_PagoConcepto"].ToString();
                    ddl_Concepto.Items.Add(item);
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Cargar los beneficiarios.
        /// </summary>
        private void CargarBeneficiarios()
        {
            //Inicializar variables.
            DataTable RS = new DataTable();
            ListItem item = new ListItem();

            try
            {
                txtSQL = " SELECT Id_PagoBeneficiario, razonsocial, Nombre, Apellido FROM PagoBeneficiario WHERE CodEmpresa in (SELECT id_empresa FROM empresa WHERE CodProyecto = " + CodProyecto + ") ";

                ddl_CodPagoBeneficiario.Items.Clear();

                //Valor por defecto.
                item = new ListItem();
                item.Text = "Seleccione el nombre del beneficiario";
                item.Value = "0";
                ddl_CodPagoBeneficiario.Items.Add(item);

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow row in RS.Rows)
                {
                    item = new ListItem();
                    item.Text = row["Nombre"].ToString() + " " + row["Apellido"].ToString() + " " + row["razonsocial"].ToString();
                    item.Value = row["Id_PagoBeneficiario"].ToString();
                    ddl_CodPagoBeneficiario.Items.Add(item);
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Cargar las formas de pago.
        /// </summary>
        private void CargarFomaDePago()
        {
            //Inicializar variables.
            DataTable RS = new DataTable();
            ListItem item = new ListItem();

            try
            {
                txtSQL = " SELECT * FROM PagoForma ";

                ddl_CodPagoForma.Items.Clear();

                //Valor por defecto.
                item = new ListItem();
                item.Text = "Seleccione la forma de pago";
                item.Value = "0";
                ddl_CodPagoForma.Items.Add(item);

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow row in RS.Rows)
                {
                    item = new ListItem();
                    item.Text = row["NomPagoForma"].ToString();
                    item.Value = row["Id_PagoForma"].ToString();
                    ddl_CodPagoForma.Items.Add(item);
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Validar la información antes de crearla/editarla.
        /// </summary>
        /// <returns>string vacío = puede continuar. // string con datos = campo requerido u ocurrió un error.</returns>
        private string Validar()
        {
            //Inicializar variables.
            String mensaje = "";

            try
            {
                //Según el clásico, se bloquean los botones...
                btn_accion.Enabled = false;
                btnEnviar.Enabled = false;

                if (ddl_Tipo.SelectedValue == "0") { mensaje = "Debe seleccionar el tipo"; }

                if (ddl_Tipo.SelectedValue == "2" && ddl_NumSolicitudRechazada.SelectedValue == "0")
                { mensaje = "Debe ingresar el número de la solicitud de orden que se va a reemplazar con la nueva"; }

                if (ddl_meses.SelectedValue == "0")
                { mensaje = "Debe ingresar un mes"; }

                if (ddl_actividad_cargo.SelectedValue == "0")
                { mensaje = "Debe ingresar la actividad"; }

                if (ddl_Concepto.SelectedValue == "0")
                { mensaje = "Debe seleccionar el concepto"; }

                if (ddl_CodPagoBeneficiario.SelectedValue == "0")
                { mensaje = "Debe seleccionar el beneficiario"; }

                if (ddl_CodPagoForma.SelectedValue == "0")
                { mensaje = "Debe seleccionar la forma de pago"; }

                if (Observaciones.Text.Trim() == "")
                { mensaje = "Debe ingresar las observaciones"; }

                try
                {
                    Int32 valor = Int32.Parse(CantidadDinero.Text.Trim());
                    if (CantidadDinero.Text.Trim() == "")
                    { mensaje = "Debe ingresar la cantidad de dinero solicitado al fondo emprender"; }
                }
                catch { mensaje = "Debe ingresar la cantidad de dinero solicitado al fondo emprender"; }

                return mensaje;
            }
            catch { mensaje = "Error inesperado"; return mensaje; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Cargar la información del pago seleccionado.
        /// </summary>
        /// <param name="CodPagoActividad">Pago seleccionado.</param>
        private void CargarDatosPagoSeleccionado(String CodPagoActividad)
        {
            //Inicializar variables.
            DataTable RS = new DataTable();
            Int32 numArchivos = 0;
            DataTable RsPagoActividad = new DataTable();

            try
            {
                //Establecer texto del botón
                btn_accion.Text = "Actualizar";

                txtSQL = " SELECT COUNT(Id_PagoActividadArchivo) AS Cuantos FROM PagoActividadarchivo " +
                         " WHERE CodPagoActividad = " + CodPagoActividad;

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                numArchivos = 0;

                try { if (RS.Rows.Count > 0) { numArchivos = Int32.Parse(RS.Rows[0]["Cuantos"].ToString()); } }
                catch { numArchivos = 0; }

                txtSQL = "SELECT * FROM PagoActividad WHERE Id_PagoActividad = " + CodPagoActividad;
                RsPagoActividad = consultas.ObtenerDataTable(txtSQL, "text");

                if (RsPagoActividad.Rows.Count > 0)
                {
                    lbl_newOrEdit.Text = "EDITAR";

                    //NomPagoActividad = RsPagoActividad.Rows[0]["NomPagoActividad"].ToString();
                    ddl_actividad_cargo.Visible = false;
                    lbl_loaded_actividad_cargo.Visible = true;
                    lbl_loaded_actividad_cargo.Text = RsPagoActividad.Rows[0]["NomPagoActividad"].ToString();

                    if (RsPagoActividad.Rows[0]["Tipo"].ToString() == "1") { ddl_Tipo.Items[0].Selected = true; }// = "Nueva"; }
                    if (RsPagoActividad.Rows[0]["Tipo"].ToString() == "2") { ddl_Tipo.Items[1].Selected = true; }// = "Rechazada"; }

                    ddl_meses.Visible = false;
                    lbl_mes_seleccionado.Visible = true;
                    lbl_mes_seleccionado.Text = "Mes " + RsPagoActividad.Rows[0]["Mes"].ToString();
                    ddl_NumSolicitudRechazada.Visible = false;

                    ddl_Tipo.Visible = false;
                    ddl_Tipo.SelectedValue = RsPagoActividad.Rows[0]["Tipo"].ToString();
                    lbl_tipo_seleccionado.Visible = true;
                    lbl_tipo_seleccionado.Text = ddl_Tipo.SelectedItem.Text;
                    hdf_tipo.Value = RsPagoActividad.Rows[0]["Tipo"].ToString();

                    td_archivosAdjuntos.Visible = true;

                    //Cargar el número de la solicitud (sólo si es diferente de cero.).
                    if (RsPagoActividad.Rows[0]["CodPagoActividadRechazada"].ToString() != "0")
                    {
                        lbl_NumSolic.Visible = true;
                        lbl_NumSolic.Text = RsPagoActividad.Rows[0]["CodPagoActividadRechazada"].ToString();
                        hdf_numsolicitud.Value = RsPagoActividad.Rows[0]["CodPagoActividadRechazada"].ToString();
                    }
                    else { lbl_NumSolic.Visible = false; }

                    ddl_Concepto.SelectedValue = RsPagoActividad.Rows[0]["CodPagoConcepto"].ToString();
                    ddl_CodPagoBeneficiario.SelectedValue = RsPagoActividad.Rows[0]["CodPagoBeneficiario"].ToString();
                    ddl_CodPagoForma.SelectedValue = RsPagoActividad.Rows[0]["CodPagoForma"].ToString();
                    Observaciones.Text = RsPagoActividad.Rows[0]["Observaciones"].ToString();
                    CantidadDinero.Text = double.Parse(RsPagoActividad.Rows[0]["CantidadDinero"].ToString()).ToString();
                    hdf_estado.Value = RsPagoActividad.Rows[0]["Estado"].ToString();

                    if (CodEstado == Constantes.CONST_EstadoEdicion.ToString())
                    {
                        btn_accion.Text = "Actualizar";
                        btn_accion.Visible = true;
                        btnEnviar.Visible = true;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Adicionar pago.
        /// </summary>
        private void Adicionar()
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            DataTable RS = new DataTable();
            Boolean bolUnicaActividad = true;
            DataTable rsCuantasActividades = new DataTable();
            String NombreActividad = "";
            Int32 NumSolicitudRechazada = 0;
            Int32 Consecutivo = 1;

            try
            {
                //Se valida que tenga avance sobre el mes y actividad de la solicitud de pago que esta ingresando.
                if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoActividad)
                {
                    //Para plan operativo
                    txtSQL = " SELECT Valor FROM AvanceActividadPOMes WHERE CodActividad = " + ddl_actividad_cargo.SelectedValue + " AND Mes = " + ddl_meses.SelectedValue;
                }
                else if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoNomina)
                {
                    //Para Nomina
                    txtSQL = " SELECT Valor FROM AvanceCargoPOMes WHERE CodCargo = " + ddl_actividad_cargo.SelectedValue + " AND Mes = " + ddl_meses.SelectedValue;
                }

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0)
                {
                    #region Para pagos de tipo Nomina se valida que solo tenga un pago por mes registrado.

                    bolUnicaActividad = true;
                    if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoNomina)
                    {
                        txtSQL = " SELECT COUNT(*) AS Cuantos FROM PagoActividad " +
                                 " WHERE TipoPago = " + Constantes.CONST_TipoPagoNomina +
                                 " AND Tipo = " + ddl_Tipo.SelectedValue +
                                 " AND Mes = " + ddl_meses.SelectedValue +
                                 " AND CodActividad = " + ddl_actividad_cargo.SelectedValue +
                                 " AND codproyecto = " + CodProyecto +
                                 " AND Estado <> " + Constantes.CONST_EstadoRechazadoFA;

                        rsCuantasActividades = consultas.ObtenerDataTable(txtSQL, "text");

                        if (rsCuantasActividades.Rows.Count > 0)
                        {
                            if (Int32.Parse(rsCuantasActividades.Rows[0]["Cuantos"].ToString()) > 0) { bolUnicaActividad = false; }
                        }
                    }

                    #endregion

                    #region  Para pagos de tipo PO se valida que solo tenga un pago por mes registrado.

                    if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoActividad)
                    {
                        txtSQL = " SELECT COUNT(*) AS Cuantos FROM PagoActividad " +
                                 " WHERE TipoPago = " + Constantes.CONST_TipoPagoActividad +
                                 " AND Tipo = " + ddl_Tipo.SelectedValue +
                                 " AND Mes = " + ddl_meses.SelectedValue +
                                 " AND CodActividad = " + ddl_actividad_cargo.SelectedValue +
                                 " AND codproyecto = " + CodProyecto +
                                 " AND Estado <> " + Constantes.CONST_EstadoRechazadoFA;

                        rsCuantasActividades = consultas.ObtenerDataTable(txtSQL, "text");

                        if (rsCuantasActividades.Rows.Count > 0)
                        {
                            if (Int32.Parse(rsCuantasActividades.Rows[0]["Cuantos"].ToString()) > 0) { bolUnicaActividad = false; }
                        }
                    }

                    #endregion

                    if (bolUnicaActividad)
                    {
                        if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoActividad)
                        {
                            txtSQL = " SELECT id_Actividad, NomActividad " +
                                     " FROM proyectoactividadPOInterventor " +
                                     " WHERE codproyecto = " + CodProyecto + " AND Id_actividad = " + ddl_actividad_cargo.SelectedValue + " ORDER BY Item ";
                        }
                        else if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoNomina)
                        {
                            txtSQL = " SELECT Id_Nomina AS Id_Actividad, Cargo AS NomActividad " +
                                     " FROM InterventorNomina" +
                                     " WHERE CodProyecto = " + CodProyecto +
                                     " AND Id_Nomina = " + ddl_actividad_cargo.SelectedValue;
                        }

                        RS = consultas.ObtenerDataTable(txtSQL, "text");

                        if (RS.Rows.Count > 0) { NombreActividad = RS.Rows[0]["NomActividad"].ToString(); }

                        NombreActividad = "Mes " + ddl_meses.SelectedValue + " - " + NombreActividad;

                        NumSolicitudRechazada = 0;
                        if (ddl_NumSolicitudRechazada.SelectedValue != "0") { NumSolicitudRechazada = Int32.Parse(ddl_NumSolicitudRechazada.SelectedValue); }

                        //Se calcula el valor del consecutivo de solicitudes por proyecto
                        Consecutivo = 1;

                        txtSQL = " SELECT Max(Consecutivo) AS Consecutivo FROM PagoActividad " +
                                 " WHERE CodProyecto = " + CodProyecto;
                        RS = consultas.ObtenerDataTable(txtSQL, "text");

                        if (RS.Rows.Count > 0)
                        {
                            if (!String.IsNullOrEmpty(RS.Rows[0]["Consecutivo"].ToString())) { Consecutivo = Int32.Parse(RS.Rows[0]["Consecutivo"].ToString()) + 1; }
                            else { Consecutivo = 1; }
                        }

                        #region Finalmente la inserción!.

                        txtSQL = " INSERT INTO PagoActividad (NomPagoActividad, Tipo, Mes, CodPagoConcepto, CodPagoBeneficiario, CodPagoForma, Observaciones, CantidadDinero, Consecutivo, CodActividad, CodProyecto, CodPagoActividadRechazada, Estado, FechaIngreso, FechaInterventor, FechaCoordinador, FechaRtaFA ,TipoPago, ObservaInterventor, RutaArchivoZIP, ObservacionesFA) " +
                                 " VALUES ('" + NombreActividad + "', " + ddl_Tipo.SelectedValue + ", " +
                                 " " + ddl_meses.SelectedValue + ", " + ddl_Concepto.SelectedValue + ", " + ddl_CodPagoBeneficiario.SelectedValue + ", " +
                                 " " + ddl_CodPagoForma.SelectedValue + ", '" + Observaciones.Text.Trim() + "', " + CantidadDinero.Text.Trim() + "," +
                                 " " + Consecutivo + ", " + ddl_actividad_cargo.SelectedValue + "," + CodProyecto + "," + NumSolicitudRechazada +
                                 ",0,GETDATE(),NULL,NULL,NULL," + TipoPago + ",'','','')";

                        try
                        {
                            cmd = new SqlCommand(txtSQL, con);

                            if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                            con.Close();
                            con.Dispose();
                            cmd.Dispose();

                            #region Vaciar campos y redirigir al usuario.

                            lbl_newOrEdit.Text = "NUEVO";
                            pnl_PagosActividad.Visible = true;
                            btn_accion.Text = "Adicionar";
                            ddl_Tipo.SelectedValue = "0";
                            lbl_tipo_seleccionado.Text = "";
                            hdf_tipo.Value = "";
                            ddl_NumSolicitudRechazada.SelectedValue = "0";
                            hdf_numsolicitud.Value = "";
                            ddl_meses.SelectedValue = "0";
                            lbl_mes_seleccionado.Text = "";
                            ddl_actividad_cargo.SelectedValue = "0";
                            lbl_loaded_actividad_cargo.Text = "";
                            ddl_Concepto.SelectedValue = "0";
                            ddl_CodPagoBeneficiario.SelectedValue = "0";
                            lblNombreBeneficiario.Text = "";
                            ddl_CodPagoForma.SelectedValue = "0";
                            lbl_FormaDePago.Text = "";
                            Observaciones.Text = "";
                            CantidadDinero.Text = "";
                            pnl_Datos.Visible = false;
                            btn_accion.Visible = true;

                            #endregion
                        }
                        catch { }

                        #endregion
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Las solicitudes de pagos deben ser sólo una por mes !!!');", true);
                        return;
                    }
                }
                else
                {
                    //Se debe mostrar mensaje de advertencia diciendole al emprendedor que no puede solicitar pagos sin hacer avance
                    if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoActividad)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No puede ingresar una solicitud de pago sin haber ingresado un avance sobre la misma actividad y mes en el Plan Operativo');", true);
                        return;
                    }
                    else if (Int32.Parse(TipoPago) == Constantes.CONST_TipoPagoNomina)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No puede ingresar una solicitud de pago sin haber ingresado un avance sobre el mismo cargo y mes en la pestaña de Nómina');", true);
                        return;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Eliminar...
        /// </summary>
        private void Eliminar(String CodPagoActividad)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();

            try
            {
                //Se borran los archivos asociados a esa solicitud de pago
                txtSQL = " DELETE FROM PagoActividadArchivo WHERE CodPagoActividad = " + CodPagoActividad;

                try
                {
                    cmd = new SqlCommand(txtSQL, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
                catch { }

                //Se borra la solicitud de pago
                txtSQL = " DELETE FROM PagoActividad WHERE Id_PagoActividad = " + CodPagoActividad;

                try
                {
                    cmd = new SqlCommand(txtSQL, con);

                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Regresar a la ventana "anterior".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Session["CodProyecto"] = CodProyecto;
            Session["TipoPago"] = TipoPago;
            Redirect(null, "PagosActividad.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
            pnl_PagosActividad.Visible = true;
            pnl_Datos.Visible = false;
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Crear o actualizar pagos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_accion_Click(object sender, EventArgs e)
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            String validado = "";

            validado = Validar();

            if (validado == "")
            {
                if (btn_accion.Text == "Adicionar")
                {
                    Adicionar();
                }
                else
                {
                    if (btn_accion.Text == "Actualizar")
                    {
                        #region Actualizar.

                        if (CodPagoActividad != "")
                        {
                            txtSQL = " UPDATE PagoActividad SET CodPagoConcepto = " + ddl_Concepto.SelectedValue + ", " +
                                     " CodPagoBeneficiario = " + ddl_CodPagoBeneficiario.SelectedValue + ", " +
                                     " CodPagoForma =" + ddl_CodPagoForma.SelectedValue + ", " +
                                     " Observaciones ='" + Observaciones.Text.Trim() + "', " +
                                     " CantidadDinero = " + CantidadDinero.Text.Trim() + " " +
                                     " WHERE Id_PagoActividad = " + CodPagoActividad; //+ ddl_actividad_cargo.SelectedValue;

                            try
                            {
                                cmd = new SqlCommand(txtSQL, con);

                                if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                                con.Close();
                                con.Dispose();
                                cmd.Dispose();

                                #region Vaciar campos y redirigir al usuario.

                                lbl_newOrEdit.Text = "NUEVO";
                                pnl_PagosActividad.Visible = true;
                                btn_accion.Text = "Adicionar";
                                ddl_Tipo.SelectedValue = "0";
                                lbl_tipo_seleccionado.Text = "";
                                hdf_tipo.Value = "";
                                ddl_NumSolicitudRechazada.SelectedValue = "0";
                                hdf_numsolicitud.Value = "";
                                ddl_meses.SelectedValue = "0";
                                lbl_mes_seleccionado.Text = "";
                                ddl_actividad_cargo.SelectedValue = "0";
                                lbl_loaded_actividad_cargo.Text = "";
                                ddl_Concepto.SelectedValue = "0";
                                ddl_CodPagoBeneficiario.SelectedValue = "0";
                                lblNombreBeneficiario.Text = "";
                                ddl_CodPagoForma.SelectedValue = "0";
                                lbl_FormaDePago.Text = "";
                                Observaciones.Text = "";
                                CantidadDinero.Text = "";
                                pnl_Datos.Visible = false;
                                btn_accion.Visible = true;

                                #endregion
                            }
                            catch { }
                        }

                        #endregion
                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + validado + "');", true);
                btn_accion.Enabled = true;
                btnEnviar.Enabled = true;
                return;
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 30/07/2014.
        /// Adicionar documento "en la página CatalogoDocumentoPagos.aspx".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgBtn_addDocumentoPago_Click(object sender, ImageClickEventArgs e)
        {
            Session["CodPagoActividad"] = Session["CodPagoActividad"].ToString();
            Redirect(null, "CatalogoDocumentoPagos.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
        }
    }
}