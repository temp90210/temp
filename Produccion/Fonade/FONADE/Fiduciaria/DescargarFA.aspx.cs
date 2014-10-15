using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.Fiduciaria
{
    public partial class DescargarFA : Negocio.Base_Page
    {
        public string codigoActa;
        public string varTemporal;

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Titulo.Text = void_establecerTitulo("Detalle Solicitudes de Pago");
            codigoActa = Session["CodActaFonade"] != null ? codigoActa = Session["CodActaFonade"].ToString() : "0";
            varTemporal = Session["Temporal"] != null ? varTemporal = Session["Temporal"].ToString() : "";

            if (varTemporal != "")
            {
                TraerArchivosA(Session["Tabla_Detalle"] as DataTable);
                TraerArchivosB(Session["Tabla_Detalle"] as DataTable);
            }

            if (!IsPostBack)
            { CargarDetalleSolicitud(); }
        }

        /// <summary>
        /// Ejecuta la consulta SQL para cargar la información a la tabla y auna variable de sesión.
        /// </summary>
        private void CargarDetalleSolicitud()
        {
            //Inicializar variables.
            DataTable tabla = new DataTable();
            String txtSQL;

            txtSQL = " SELECT PagoActividad.Id_PagoActividad, PagoActividad.FechaCoordinador, Empresa.razonsocial, PagoActividad.CantidadDinero " +
                        " ,archivopagosFA,ArchivotercerosFA,RutaArchivozip " +
                        " FROM PagosActaSolicitudPagos " +
                        " INNER JOIN PagosActaSolicitudes ON PagosActaSolicitudes.id_acta =PagosActaSolicitudPagos.CodPagosActaSolicitudes " +
                        " INNER JOIN PagoActividad ON PagosActaSolicitudPagos.CodPagoActividad = PagoActividad.Id_PagoActividad " +
                        " INNER JOIN Empresa ON PagoActividad.CodProyecto = Empresa.codproyecto " +
                        " WHERE PagosActaSolicitudPagos.CodPagosActaSolicitudes = " + codigoActa + " AND PagosActaSolicitudPagos.Aprobado = 1 ";

            tabla = consultas.ObtenerDataTable(txtSQL, "text");

            Session["Tabla_Detalle"] = tabla;
            Session["Temporal"] = "temporal";
            detalleSolicitud.DataSource = tabla;
            detalleSolicitud.DataBind();
            TraerArchivosB(tabla);
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// Navegación de la grilla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void detalle_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var dt = Session["Tabla_Detalle"] as DataTable;

            if (dt != null)
            {
                detalleSolicitud.PageIndex = e.NewPageIndex;
                detalleSolicitud.DataSource = dt;
                detalleSolicitud.DataBind();
            }
        }

        /// <summary>
        /// Método que trae los soportes de los pagos
        /// FUNCIONAL!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TraerArchivosA(DataTable parametro)
        {
            //Limpiar el panel.
            Panel1.Controls.Clear();

            //Instanciar Label.
            Label myLabel = new Label();

            //Crear Label y agregarlo al panel.
            myLabel.ID = "TituloA";
            myLabel.Text = "Haga clic en los siguientes enlaces para descargar los archivos. <br />";
            Panel1.Controls.Add(myLabel);

            //Instanciar LinkButton.
            LinkButton lnk_VerAvance1 = new LinkButton();

            for (int i = 0; i < 2; i++)
            {
                lnk_VerAvance1 = new LinkButton();
                lnk_VerAvance1.ID = i.ToString();
                if (i == 0)
                {
                    lnk_VerAvance1.Text = "<b>Archivos de terceros </b><br />";
                    lnk_VerAvance1.CommandArgument = ConfigurationManager.AppSettings["RutaDocumentosPagos"] + parametro.Rows[0]["archivopagosFA"].ToString(); //+campo["RutaArchivozip"]; 
                }
                else if (i == 1)
                {
                    lnk_VerAvance1.Text = "<b>Archivos de pagos </b><br />";
                    lnk_VerAvance1.CommandArgument = ConfigurationManager.AppSettings["RutaDocumentosPagos"] + parametro.Rows[0]["ArchivotercerosFA"].ToString(); //+campo["RutaArchivozip"]; 
                }
                lnk_VerAvance1.CommandName = "Descargar";
                lnk_VerAvance1.Command += new CommandEventHandler(DynamicCommand_VerAvance);
                Panel1.Controls.Add(lnk_VerAvance1);
            }
        }

        /// <summary>
        /// Método que trae los archivos de pago
        /// FUNCIONAL!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TraerArchivosB(DataTable parametro)
        {
            //Limpiar controles.
            Panel2.Controls.Clear();
            //Contador de ID para controles dinámicos.
            int cont = 0;

            foreach (DataRow campo in parametro.Rows)
            {
                if (campo["archivopagosFA"].ToString() != "")
                {
                    //Instancias de los controles a usar.
                    HyperLink lnk_VerAvance = new HyperLink();
                    Label myLabel = new Label();

                    //Label.
                    myLabel.ID = "TituloB" + cont.ToString();
                    myLabel.Text = "<br />";
                    //LinkButton.
                    lnk_VerAvance.ID = "lnk_VerAvance_3" + cont.ToString();
                    lnk_VerAvance.Text = "<b>Archivos Pago No. " + campo["Id_PagoActividad"] + "</b>";
                    lnk_VerAvance.NavigateUrl = ConfigurationManager.AppSettings["RutaDocumentosPagos"] + campo["RutaArchivoZip"];
                    //Adicionar los controles al panel e incrementar la variable.
                    Panel2.Controls.Add(lnk_VerAvance);
                    Panel2.Controls.Add(myLabel);
                    cont++;
                }
            }
        }

        /// <summary>
        /// Método asignado a los controles dinámicos para ver los avances.
        /// FUNCIONAL!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicCommand_VerAvance(Object sender, CommandEventArgs e)
        {
            try
            {
                //Descargar el archivo / ver avance.
                Response.Redirect(e.CommandArgument.ToString());
            }
            catch { }
        }

        protected void detalleSolicitud_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("mostrar"))
            {
                Session["Id_PagoActividad"] = e.CommandArgument.ToString();

                Redirect(null, "CoordinadorPago.aspx", "_blank",
                         "menubar=0,scrollbars=1,width=710,height=400,top=100");
            }
        }

        /// <summary>
        /// RowDataBound para formatear los datos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void detalleSolicitud_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lbl_fec = e.Row.FindControl("") as Label;
                var lbl_can = e.Row.FindControl("") as Label;
                double cant_din = 0;

                if (lbl_fec != null && lbl_can != null)
                {
                    if (!String.IsNullOrEmpty(lbl_can.Text.Trim()))
                    {
                        cant_din = Double.Parse(lbl_can.Text.Trim());
                        lbl_can.Text = cant_din.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                    }

                    try
                    {
                        if (!String.IsNullOrEmpty(lbl_fec.Text.Trim()))
                        {
                            //Establecer fecha obtenida del campo de texto en variable.
                            DateTime fecha = Convert.ToDateTime(lbl_fec.Text.Trim());

                            //Obtener la hora en minúscula.
                            string hora = fecha.ToString("hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToLowerInvariant();

                            //Mostrar fecha total.
                            lbl_fec.Text = fecha.ToString("dd/MM/yyyy") + " " + hora + ".";
                        }
                    }
                    catch { }
                }
            }
        }

        //private void TraerArchivosDos(DataTable parametro)
        //{
        //    int cont = 0;

        //    foreach (DataRow campo in parametro.Rows)
        //    {
        //        if (campo["archivopagosFA"].ToString() != "")
        //        {
        //            LinkButton lnk_VerAvance = new LinkButton();


        //            Label myLabel = new Label();

        //            //LinkButton.
        //            myLabel.ID = "Label1_" + cont.ToString();
        //            myLabel.Text = "<br />";
        //            lnk_VerAvance.ID = "lnk_VerAvance_" + cont.ToString();
        //            lnk_VerAvance.Text = "<b>Archivos Pago No. " + campo["Id_PagoActividad"] + "</b>";
        //            //lnk_VerAvance.Style.Add("text-decoration", "none");
        //            //lnk_VerAvance.CommandName = "VerAvance";
        //            //lnk_VerAvance.CommandArgument = "crear" + ";" + Cod_Proyecto + ";" + Cod_Nomina + ";" + mes_data + ";" + nomCargo;
        //            //lnk_VerAvance.Command += new CommandEventHandler(DynamicCommand_VerAvance);
        //            Panel2.Controls.Add(lnk_VerAvance);
        //            Panel2.Controls.Add(myLabel);
        //            cont++;
        //        }
        //    }
        //}

    }
}