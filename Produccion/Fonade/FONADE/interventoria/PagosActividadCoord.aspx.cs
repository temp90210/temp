using Datos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Data.SqlClient;
using System.Configuration;
using Fonade.Clases;
//using CAPICOM;

namespace Fonade.FONADE.interventoria
{
    public partial class PagosActividadCoord : Negocio.Base_Page
    {
        #region Variables globales.

        SignedXml signedXml = new SignedXml();

        /// <summary>
        /// Contiene el valor de la firma digital...
        /// </summary>
        String Firma;

        /// <summary>
        /// Contiene el Xml generado con la información a almacenar.
        /// </summary>
        String Datos;

        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "FONDO EMPRENDER - Aprobacion de Solicitudes de Pago";

            if (!IsPostBack)
            {
                CargarInformacionTablaDinamica();
            }

            #region Código anterior que cargaba la grilla = NO BORRAR!
            //var datat = new DataTable();

            //datat.Columns.Add("Id_PagoActividad");
            //datat.Columns.Add("Fecha");
            //datat.Columns.Add("RazonSocial");
            //datat.Columns.Add("Intervemtor");
            //datat.Columns.Add("Valor");
            //datat.Columns.Add("ObservaInterventor");

            //CodContatoFiduciaria = Session["CodContatoFiduciaria"] != null && !string.IsNullOrEmpty(Session["CodContatoFiduciaria"].ToString()) ? Session["CodContatoFiduciaria"].ToString() : "0";

            //if (!IsPostBack)
            //{
            //    var result = from f in consultas.Db.MD_Fiduciaria() select f;

            //    if (result != null)
            //    {

            //        if (!string.IsNullOrEmpty(CodContatoFiduciaria))
            //        {
            //            result = result.Where(f => f.codcontactofiduciaria == Convert.ToInt32(CodContatoFiduciaria));
            //        }

            //        result = result.OrderBy(f => f.Fecha);
            //        result = result.OrderBy(f => f.Id_PagoActividad);

            //        foreach (var res in result)
            //        {
            //            var result2 = (from i in consultas.Db.MD_InterventorFida(Constantes.CONST_RolInterventorLider, usuario.IdContacto, res.Id_Empresa)
            //                           select new
            //                           {
            //                               Intervemtor = i.Intervemtor
            //                           }).FirstOrDefault();

            //            if (result2 != null)
            //            {
            //                DataRow filadt = datat.NewRow();

            //                filadt["Id_PagoActividad"] = "" + res.Id_PagoActividad;
            //                filadt["Fecha"] = "" + res.Fecha;
            //                filadt["RazonSocial"] = "" + res.razonsocial;
            //                filadt["Intervemtor"] = "" + result2.Intervemtor;
            //                filadt["Valor"] = "" + res.Valor;
            //                filadt["ObservaInterventor"] = "" + res.ObservaInterventor;

            //                datat.Rows.Add(filadt);
            //            }
            //        }

            //        gvsolicitudes.DataSource = datat;
            //        gvsolicitudes.DataBind();
            //    }
            //    else
            //    {
            //        resTotal.Visible = false;
            //        resTotal.Enabled = false;
            //        lblerror.Text = "No hay Solicitudes de pago registradas";
            //    }
            //}
            #endregion
        }

        #region Métodos generales.

        /// <summary>
        /// Establecer el primer valor en mayúscula, retornando un string con la primera en maýsucula.
        /// </summary>
        /// <param name="s">String a procesar</param>
        /// <returns>String procesado.</returns>
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

        #endregion

        /// <summary>
        /// Mauricio Arias Olave.
        /// 13/05/2014.
        /// Cargar la información de la grilla.
        /// Este método internamente evalúa si hay valores en la variable de sesión.
        /// </summary>
        private void CargarInformacionTablaDinamica()
        {
            //Inicializar variables.
            String txtSQL = "";
            String txtSQL1 = "";
            DataTable tabla_sql = new DataTable();

            try
            {
                txtSQL1 = " SELECT DISTINCT PagoActividad.Id_PagoActividad AS tablaId_PagoActividad, CodActaFonade " +
                          " FROM PagoActividad " +
                          " INNER JOIN Empresa ON PagoActividad.CodProyecto = Empresa.codproyecto " +
                          " INNER JOIN PagoBeneficiario ON PagoActividad.CodPagoBeneficiario = PagoBeneficiario.Id_PagoBeneficiario " +
                          " INNER JOIN PagosActaSolicitudPagos ON (PagoActividad.id_PagoActividad = PagosActaSolicitudPagos.codPagoActividad) " +
                          " INNER JOIN PagosActaSolicitudes ON (PagosActaSolicitudPagos.codPagosActaSolicitudes = PagosActaSolicitudes.id_acta AND PagosActaSolicitudes.tipo = 'Fiduciaria') " +
                          " WHERE PagoActividad.Estado = " + Constantes.CONST_EstadoCoordinador;

                txtSQL = " SELECT * FROM (SELECT Empresa.Id_Empresa, PagoActividad.Id_PagoActividad, Empresa.razonsocial, PagoActividad.FechaInterventor AS Fecha, " +
                         " PagoActividad.CantidadDinero AS Valor, PagoBeneficiario.NumIdentificacion, Empresa.codproyecto,codactafonade, PagoActividad.ObservaInterventor, " +
                         " (SELECT TOP 1 CodContactoFiduciaria FROM convocatoria, Convocatoriaproyecto, convenio WHERE id_convocatoria = codconvocatoria AND id_convenio = codconvenio AND id_convocatoria IN " +
                         " (SELECT MAX(CodConvocatoria) AS CodConvocatoria FROM Convocatoriaproyecto WHERE viable = 1 AND Codproyecto = Empresa.codproyecto)  " +
                         " AND Codproyecto = Empresa.codproyecto ) AS codcontactofiduciaria " +
                         " FROM PagoActividad " +
                         " INNER JOIN Empresa ON PagoActividad.CodProyecto = Empresa.codproyecto " +
                         " INNER JOIN PagoBeneficiario ON PagoActividad.CodPagoBeneficiario = PagoBeneficiario.Id_PagoBeneficiario " +
                         " LEFT JOIN (" + txtSQL1 + ") AS tabla ON (tablaId_PagoActividad = id_PagoActividad) " +
                         " WHERE PagoActividad.Estado = " + Constantes.CONST_EstadoCoordinador +
                         " ) AS tabla ";

                if (!String.IsNullOrEmpty(Session["CodContatoFiduciaria"].ToString()))
                {
                    txtSQL = txtSQL + " where codcontactofiduciaria = " + Session["CodContatoFiduciaria"].ToString();
                }
                //txtSQL = txtSQL + " ORDER BY fecha, Id_PagoActividad	 ";
                // Para mostrar la información en el orden establecido en FONADE clásico.
                txtSQL = txtSQL + " ORDER BY fecha ASC	 ";

                //Asignar resultados de la consulta anterior a variable DataTable.
                tabla_sql = consultas.ObtenerDataTable(txtSQL, "text");

                #region Recorrer listado para eliminar valores que NO corresponden a la grilla (ver notas internas).

                ///Según FONADE clásico, si la consulta que sigue a continuación NO contiene datos, NO debe mostrar
                ///los resultados de la consulta principal, en resumen, sólo se mostrará la información que debe mostrar
                ///en el GridView.

                for (int i = 0; i < tabla_sql.Rows.Count; i++)
                {
                    String mini_txtSQL = "";
                    DataTable DataTable_TMP = new DataTable();
                    mini_txtSQL = " SELECT DISTINCT Contacto.Nombres + ' ' + Contacto.Apellidos AS Intervemtor " +
                                  " FROM Contacto  INNER JOIN EmpresaInterventor " +
                                  " ON Contacto.Id_Contacto = EmpresaInterventor.CodContacto " +
                                  " INNER JOIN Interventor ON EmpresaInterventor.CodContacto = Interventor.CodContacto " +
                                  " WHERE (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                                  " AND (EmpresaInterventor.Inactivo = 0) " +
                                  " AND (Interventor.CodCoordinador = " + usuario.IdContacto + ")" +
                                  " AND (EmpresaInterventor.CodEmpresa = " + tabla_sql.Rows[i]["Id_Empresa"].ToString() + ")";

                    //Asignar valores a DataTable_TMP;
                    DataTable_TMP = consultas.ObtenerDataTable(mini_txtSQL, "text");

                    //Si la consulta NO trajo datos, debe eliminar la información correspondiente en el DataTable de la 
                    //consulta principal.
                    if (DataTable_TMP.Rows.Count == 0)
                    { tabla_sql.Rows[i].Delete(); }
                }

                #endregion

                //Bindear datos.
                Session["dtEmpresas"] = tabla_sql;
                gvsolicitudes.DataSource = tabla_sql;
                gvsolicitudes.DataBind();
            }
            catch { string err = ""; }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 13/05/2014.
        /// RowCommand para la grilla "gvsolicitudes".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvsolicitudes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "mostrar_coordinadorPago")
            {
                LinkButton lnkBtn = e.CommandSource as LinkButton;
                Session["Id_PagoActividad"] = lnkBtn.Text;
                //response.redirect...
                Redirect(null, "CoodinadorPago.aspx", "_blank",
                    "menubar=0,scrollbars=1,width=380,height=220,top=100");
            }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 13/05/2014.
        /// RowDataBound que ejecuta determinadas consultas para establecer valores sobre los controles
        /// internos del GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvsolicitudes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lnk_Id = e.Row.FindControl("lnk_btn_Id_PagoActividad") as LinkButton;
                var lblFecha = e.Row.FindControl("lbl_fecha") as Label;
                var hdf = e.Row.FindControl("hdf_codactafonade") as HiddenField;
                var rbLst = e.Row.FindControl("rb_lst_aprobado") as RadioButtonList;
                var lbl_Display = e.Row.FindControl("lbl_displayText") as Label;
                var lbl_Inter = e.Row.FindControl("lbl_Intervemtor") as Label;
                var lbl_valor_formateado = e.Row.FindControl("lbl_valor") as Label;
                var hdf_idEmpresa = e.Row.FindControl("hdf_empresa") as HiddenField;

                //Comprobar que TODOS los controles fueron instanciados correctamente.
                if (lnk_Id != null && lblFecha != null && hdf != null && rbLst != null && lbl_Display != null
                    && lbl_Inter != null && lbl_valor_formateado != null && hdf_idEmpresa != null)
                {
                    #region Depurar fecha según FONADE clásico.
                    try
                    {
                        //Inicializar variable DateTime.
                        DateTime fecha = Convert.ToDateTime(lblFecha.Text);

                        //Obtener el nombre del mes (las primeras tres letras).
                        string sMes = fecha.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

                        //Formatear la fecha según manejo de FONADE clásico. "Ej: Nov 19 de 2013".
                        lblFecha.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year;
                    }
                    catch { }
                    #endregion

                    #region Establecer las acciones en el RadioButtonList según el valor almacenado en "codactafonade".
                    if (hdf.Value.Trim() == "" || hdf.Value == null)
                    {
                        //Establecer selección del ítem "Pendiente" del RadioButtonList según FONADE clásico.
                        rbLst.SelectedValue = "opcion_Pendiente";
                    }
                    else
                    {
                        //Mostrar el siguente texto.
                        lbl_Display.Visible = true;
                        lbl_Display.Text = "Este Registro No puede ser enviado A pagos, está incluído en el Acta " + hdf.Value;
                        //Bloquear otros ítems del RadioButtonList y chequear el ítem "Pendiente".
                        rbLst.Items[0].Enabled = false;
                        rbLst.Items[1].Enabled = false;
                        rbLst.SelectedValue = "opcion_Pendiente";
                    }
                    #endregion

                    #region Formatear el valor del campo "lbl_valor_formateado" para que vea igual que en FONADE clásico.

                    try
                    {
                        Double valor = Convert.ToDouble(lbl_valor_formateado.Text);
                        lbl_valor_formateado.Text = valor.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"));
                    }
                    catch { }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Paginación de la grilla "gvsolicitudes".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvsolicitudes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvsolicitudes.PageIndex = e.NewPageIndex;
            CargarInformacionTablaDinamica();
        }

        /// <summary>
        /// Generar XML y "Enviar Datos" que es, generar el archivo plan, el registro en la base de datos
        /// y firmar el documento generado con la certificación digital.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnenviardatos_Click(object sender, EventArgs e)
        {
            //crearXML();
            btnenviardatos.Enabled = false;
            GenerarXml();
        }

        private void crearXML()
        {
            try
            {
                CspParameters cspParams = new CspParameters();
                cspParams.KeyContainerName = "XML_DSIG_RSA_KEY";
                RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load("test.xml");
                byte[] hashSignature = rsaKey.SignHash(Convert.FromBase64String("e7jQRU4xmLaQmWVO9pVovhWSeGU="), CryptoConfig.MapNameToOID("SHA1"));
                SignXml(xmlDoc, rsaKey);

                // Add the key to the SignedXml document.
                signedXml.SigningKey = rsaKey;
                Console.WriteLine("XML file signed.");
                xmlDoc.Save("test.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void SignXml(XmlDocument xmlDoc, RSA Key)
        {
            try
            {

                RSAKeyValue rsakey = new RSAKeyValue();
                // Check arguments.
                if (xmlDoc == null)
                    throw new ArgumentException("xmlDoc");
                if (Key == null)
                    throw new ArgumentException("Key");

                // Create a SignedXml object.

                SignedXml signedXml = new SignedXml(xmlDoc);
                // Add the key to the SignedXml document.

                // Create a reference to be signed.
                Reference reference = new Reference();
                reference.Uri = "";

                // Add an enveloped transformation to the reference.
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);

                // Add the reference to the SignedXml object.
                signedXml.AddReference(reference);


                // Compute the signature.
                signedXml.ComputeSignature();

                // Get the XML representation of the signature and save
                // it to an XmlElement object.
                XmlElement xmlDigitalSignature = signedXml.GetXml();

                // Append the element to the XML document.
                xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Mauricio Arias Olave.
        /// 04/08/2014.
        /// Generar Xml "es un archvo plano que se crea como registro en base de datos 
        /// (y al parecer un archivo físico también)"
        /// Contiene TODO el código de las páginas "PagosActivdadCoord.aspx" y "FirmaCoordinador.asp".
        /// </summary>
        private void GenerarXml()
        {
            //Inicializar variables.
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
            String opcion = "No";
            RadioButtonList rbList = new RadioButtonList();
            LinkButton lnk = new LinkButton();
            HiddenField hdf = new HiddenField();
            HiddenField hdf_proyecto = new HiddenField();
            Label lbl = new Label();
            HiddenField hdf_beneficiario = new HiddenField();
            TextBox txtObservaciones = new TextBox();
            String PerfilFonade = usuario.Nombres + " " + usuario.Apellidos;
            String Firma = ""; //Obtiene el valor de la firma digital.
            String txtSQL = "";
            //Boolean DatosFirmados = false;
            String DatosFirmados = "";
            Boolean ValidaCRL = false;

            #region Variables de la pantalla "FirmaCoordinador.aspx". COMENTADO, redireccionará a "FirmaCoordinador.aspx".
            //SqlCommand cmd = new SqlCommand();
            //String CodActa = "";
            //DataTable RS = new DataTable();
            //DataTable rscount = new DataTable();
            //String txtmensajeNoInsert = "";
            //Boolean bolVerifica = false;
            //String txtRechazo = "";
            //int numSolicitudes = 0;
            //String txtDatosFirma = "";
            //String CodRechazoFirmaDigital = "";
            //Int32 numAprobadas = 0; 
            #endregion

            try
            {
                #region Generar XML en variable String llamada "Datos".

                //Inicio del cuerpo del Xml.
                Datos = Datos + @"<?xml version=""1.0"" encoding=""windows-1252""?>";
                Datos = Datos + "<Xml_PAGOS>";

                // Se arma el contenido de los datos del form con ASP.
                // con C# se arman los datos en este orden:
                // 1. CodActividad, para cada solicitud.
                // 2. nomEmpresa, para cada solicitud.
                // 3. CodProyecto, para cada solicitud.
                // 3. Valor, para cada solicitud.
                // 4. CodBeneficiario, para cada solicitud.
                // 5. Opcion, para cada solicitud.
                // 6. Observaciones, para cada solicitud.
                // 7. Fecha de Envio, para el form.
                // 8. Numero de solicitudes, para el form.

                for (int k = 0; k < gvsolicitudes.Rows.Count; k++)
                {
                    //Instanciar valores.
                    opcion = "";
                    GridViewRow row = gvsolicitudes.Rows[k];
                    rbList = (RadioButtonList)row.FindControl("rb_lst_aprobado");
                    lnk = (LinkButton)row.FindControl("lnk_btn_Id_PagoActividad");
                    hdf = (HiddenField)row.FindControl("hdf_RazonSocial");
                    hdf_proyecto = (HiddenField)row.FindControl("hdf_codProyecto");
                    lbl = (Label)row.FindControl("lbl_valor");
                    hdf_beneficiario = (HiddenField)row.FindControl("hdf_CodBeneficiario");
                    txtObservaciones = (TextBox)row.FindControl("txt_observ");

                    if (rbList.SelectedItem.Text == "Si" || rbList.SelectedItem.Text == "No")
                    {
                        opcion = rbList.SelectedItem.Text;  //Establecer Si / No para ponerlo en el Xml.

                        if (k == 0) { k = 1; }

                        //Cuerpo del Xml.
                        Datos = Datos + "		 <Xml_Solicitud" + k.ToString() + "> ";
                        Datos = Datos + "		    <xml_CodSolicitud> " + lnk.Text + "   </xml_CodSolicitud> ";
                        Datos = Datos + "		    <xml_NomEmpresa> " + hdf.Value + " </xml_NomEmpresa>";
                        Datos = Datos + "		    <xml_CodProyecto>" + hdf_proyecto.Value + " </xml_CodProyecto>";
                        Datos = Datos + "		    <xml_Valor>" + lbl.Text + "</xml_Valor>" + " ";
                        Datos = Datos + "		    <xml_CodBeneficiario> " + hdf_beneficiario.Value + "</xml_CodBeneficiario> ";
                        Datos = Datos + "		    <xml_opcion>" + opcion + "</xml_opcion> ";
                        Datos = Datos + "		    <xml_Observaciones> " + txtObservaciones.Text + " </xml_Observaciones> ";
                        Datos = Datos + "		 </Xml_Solicitud" + k.ToString() + "> ";
                    }
                }

                //Finalizar cuerpo del xml.
                Datos = Datos + "	<xml_FechaSolicitudes>" + DateTime.Now.ToString("dd/MM/yyyy") + "</xml_FechaSolicitudes>";
                Datos = Datos + "	<xml_NumeroSolicitudes>" + gvsolicitudes.Rows.Count.ToString() + "</xml_NumeroSolicitudes>";
                Datos = Datos + "	<xml_UsuarioFonade>" + PerfilFonade + "</xml_UsuarioFonade>";
                Datos = Datos + "</Xml_PAGOS>";

                #endregion

                #region El siguiente paso es firmar el Xml con el certificado digital y enviar los datos a "FirmaCoordinador.asp".

                DatosFirmados = SignData(Datos, Firma);

                if (DatosFirmados == "")
                { ValidaCRL = true; }
                else
                {
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + DatosFirmados + "')", true);
                    return;
                }

                if (ValidaCRL)
                {
                    Session["Datos"] = Datos;
                    Session["Firma"] = Firma;
                    Session["numSolicitudes"] = gvsolicitudes.Rows.Count.ToString();
                    //Ya tiene en sesión la variable Session["CodContatoFiduciaria"]

                    #region Enviar la información a "FirmaCoordinador.aspx...". COMENTADO.

                    #region NO BORRAR!.
                    //if (Datos != "")
                    //{
                    //    //Recibe los datos del formulario que firmo el coordinador de interventoria
                    //    numSolicitudes = gvsolicitudes.Rows.Count;

                    //    #region Se crea el acta.

                    //    txtSQL = " INSERT INTO PagosActaSolicitudes (Fecha, NumSolicitudes, Datos, Firma, CodContacto,Tipo,CodContactoFiduciaria) " +
                    //             " VALUES(GETDATE()," + gvsolicitudes.Rows.Count.ToString() + ",'" + Datos + "','" + Firma + "'," + usuario.IdContacto + ",'Fonade'," + Session["CodContatoFiduciaria"].ToString() + ")";

                    //    try
                    //    {
                    //        //NEW RESULTS:
                    //        cmd = new SqlCommand(txtSQL, con);

                    //        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    //        cmd.CommandType = CommandType.Text;
                    //        cmd.ExecuteNonQuery();
                    //        con.Close();
                    //        con.Dispose();
                    //        cmd.Dispose();
                    //    }
                    //    catch { }

                    //    #endregion

                    //    #region Se trae el id del acta recien ingresada.

                    //    txtSQL = " SELECT MAX(Id_Acta) AS Id_Acta FROM PagosActaSolicitudes WHERE CodContacto = " + usuario.IdContacto + " AND Tipo = 'Fonade'";
                    //    RS = consultas.ObtenerDataTable(txtSQL, "text");

                    //    if (RS.Rows.Count > 0) { CodActa = RS.Rows[0]["Id_Acta"].ToString(); }

                    //    #endregion

                    //    for (int k = 0; k < gvsolicitudes.Rows.Count; k++)
                    //    {
                    //        #region Recorrer la grilla para hacer otro tipo de inserciones...

                    //        //Instanciar valores.
                    //        opcion = "";
                    //        GridViewRow row = gvsolicitudes.Rows[k];
                    //        rbList = (RadioButtonList)row.FindControl("rb_lst_aprobado");
                    //        lnk = (LinkButton)row.FindControl("lnk_btn_Id_PagoActividad");
                    //        hdf = (HiddenField)row.FindControl("hdf_RazonSocial");
                    //        hdf_proyecto = (HiddenField)row.FindControl("hdf_codProyecto");
                    //        lbl = (Label)row.FindControl("lbl_valor");
                    //        hdf_beneficiario = (HiddenField)row.FindControl("hdf_CodBeneficiario");
                    //        txtObservaciones = (TextBox)row.FindControl("txt_observ");

                    //        if (rbList.SelectedItem.Text != "Pendiente")
                    //        {
                    //            opcion = "0";

                    //            if (rbList.SelectedItem.Text == "Si")
                    //                opcion = "1";
                    //            else
                    //                opcion = "2";

                    //            //Se limpia la cadena, se elimina el salto de linea, el retorno de carro y el tabulador.
                    //            txtObservaciones.Text = txtObservaciones.Text.TrimEnd(new char[] { '\r', '\n', '\t' });

                    //            //Valido si el Pago Existe en algun acta.
                    //            txtSQL = " select count(*) as cuantos,codpagosActaSolicitudes " +
                    //                     " from PagosActaSolicitudPagos where codpagoactividad = " + lnk.Text +
                    //                     " group by codpagosActaSolicitudes ";

                    //            rscount = consultas.ObtenerDataTable(txtSQL, "text");

                    //            if (rscount.Rows.Count > 0)
                    //            {
                    //                txtmensajeNoInsert = txtmensajeNoInsert + " Pago No : " + lnk.Text + " - Acta No: " + rscount.Rows[0]["codpagosActaSolicitudes"].ToString() + "</br>";
                    //            }
                    //            else
                    //            {
                    //                #region Inserción.

                    //                txtSQL = " INSERT INTO PagosActaSolicitudPagos (CodPagosActaSolicitudes,CodPagoActividad,Aprobado,Observaciones) " +
                    //                         " VALUES(" + CodActa + "," + lnk.Text + "," + opcion + ",'" + txtObservaciones.Text + "')";

                    //                try
                    //                {
                    //                    //NEW RESULTS:
                    //                    cmd = new SqlCommand(txtSQL, con);

                    //                    if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    //                    cmd.CommandType = CommandType.Text;
                    //                    cmd.ExecuteNonQuery();
                    //                    con.Close();
                    //                    con.Dispose();
                    //                    cmd.Dispose();
                    //                }
                    //                catch { }

                    //                #endregion

                    //                if (rbList.SelectedItem.Text == "Pendiente")
                    //                {
                    //                    opcion = Constantes.CONST_EstadoCoordinador.ToString();
                    //                }
                    //                else
                    //                {
                    //                    if (rbList.SelectedItem.Text == "Si")
                    //                    { opcion = Constantes.CONST_EstadoFiduciaria.ToString(); }
                    //                    else
                    //                    { opcion = Constantes.CONST_EstadoRechazadoFA.ToString(); }

                    //                    if (rbList.SelectedItem.Text != "Pendiente")
                    //                    {
                    //                        #region Actualización.

                    //                        txtSQL = " UPDATE PagoActividad SET Estado = " + opcion + ", FechaCoordinador = GETDATE()" +
                    //                                 " WHERE Id_PagoActividad = " + lnk.Text;

                    //                        try
                    //                        {
                    //                            //NEW RESULTS:
                    //                            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                    //                            cmd = new SqlCommand(txtSQL, con);

                    //                            if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    //                            cmd.CommandType = CommandType.Text;
                    //                            cmd.ExecuteNonQuery();
                    //                            con.Close();
                    //                            con.Dispose();
                    //                            cmd.Dispose();
                    //                        }
                    //                        catch { }

                    //                        #endregion
                    //                    }
                    //                }
                    //            }
                    //        }

                    //        #endregion
                    //    }

                    //    if (txtmensajeNoInsert != "")
                    //    { System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Los siguientes pagos no fueron procesados porque se encontraban registrados en las actas relacionadas <br/>" + txtmensajeNoInsert + "')", true); }

                    //    //PART TWO...
                    //    bolVerifica = false;
                    //    txtRechazo = "";

                    //    //Variable de session para capturar el error que pueda detectar CAPICOM.
                    //    Session["ErrorCapicom"] = "";
                    //    Session["MensajeErrorCapicom"] = "";

                    //    #region Verificar que los datos que fueron firmados en el cliente son los mismos datos que se han recibido en el servidor y no han sido adulterados.

                    //    if (VerifySign(Datos, Firma))
                    //    {
                    //        //Verificar que el certificado digital empleado para el proceso de firma no se encuentra en la Lista de Certificados Revocados (CRL) y, por lo tanto, es un certificado valido.
                    //        if (ValidateRoot(Datos, Firma))
                    //        {
                    //            //Verificar que el certificado digital empleado para el proceso de firma se encuentra dentro de su período de vigencia
                    //            if (Validatetime(Datos, Firma))
                    //            {
                    //                if (ValidaFirmantes(Datos, Firma)) { bolVerifica = true; }
                    //            }
                    //            else
                    //            {
                    //                Session["ErrorCapicom"] = "Time";
                    //                Session["MensajeErrorCapicom"] = "El Certificado utilizado no esta vigente, por lo tanto no es válido.";
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Session["ErrorCapicom"] = "Root";
                    //            Session["MensajeErrorCapicom"] = "El Certificado utilizado no fue emitido por una entidad certificadora, por lo tanto no es válido.";
                    //        }
                    //    }

                    //    #endregion

                    //    //Extraer la información de identificación del firmante, se actualiza el acta con estos datos
                    //    txtDatosFirma = "";

                    //    //Inicializar el objeto Capicom que gestiona las firmas digitales
                    //    SignedData Verifier = new SignedData();
                    //    Verifier.Content = Datos;

                    //    //Verificar la firma digital
                    //    Verifier.Verify(Firma, true, 0);

                    //    foreach (Certificate Certificate in Verifier.Certificates)
                    //    { txtDatosFirma = txtDatosFirma + Certificate.SubjectName; }

                    //    #region Actualización.

                    //    txtSQL = "UPDATE PagosActaSolicitudes SET DatosFirma = '" + txtDatosFirma + "' WHERE Id_Acta = " + CodActa;

                    //    try
                    //    {
                    //        //NEW RESULTS:
                    //        cmd = new SqlCommand(txtSQL, con);

                    //        if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    //        cmd.CommandType = CommandType.Text;
                    //        cmd.ExecuteNonQuery();
                    //        con.Close();
                    //        con.Dispose();
                    //        cmd.Dispose();
                    //    }
                    //    catch { }

                    //    #endregion

                    //    #region Si todas las verificaciones OK, se hacen los cambios de estado de los pagos (se van a fiduciaria o se devuelven a Interventor).

                    //    if (bolVerifica)
                    //    {
                    //        for (int m = 0; m < gvsolicitudes.Rows.Count; m++)
                    //        {
                    //            //Instanciar valores.
                    //            opcion = "";
                    //            GridViewRow row = gvsolicitudes.Rows[m];
                    //            rbList = (RadioButtonList)row.FindControl("rb_lst_aprobado");
                    //            lnk = (LinkButton)row.FindControl("lnk_btn_Id_PagoActividad");
                    //            hdf = (HiddenField)row.FindControl("hdf_RazonSocial");
                    //            hdf_proyecto = (HiddenField)row.FindControl("hdf_codProyecto");
                    //            lbl = (Label)row.FindControl("lbl_valor");
                    //            hdf_beneficiario = (HiddenField)row.FindControl("hdf_CodBeneficiario");
                    //            txtObservaciones = (TextBox)row.FindControl("txt_observ");

                    //            if (rbList.SelectedItem.Text == "Pendiente")
                    //            { opcion = Constantes.CONST_EstadoCoordinador.ToString(); }
                    //            else if (rbList.SelectedItem.Text == "Si")
                    //            {
                    //                opcion = Constantes.CONST_EstadoFiduciaria.ToString();
                    //                numAprobadas = numAprobadas + 1;
                    //            }
                    //            else if (rbList.SelectedItem.Text == "No")
                    //            {
                    //                opcion = Constantes.CONST_EstadoInterventor.ToString();
                    //            }

                    //            #region Mensaje 1.

                    //            //Oct 19 2005, Alejandro Garzon R.
                    //            //Inicio modificacion
                    //            //Solicitud rechazada, se genera tarea avisandole al interventor y al emprendedor
                    //            //Interventor
                    //            txtSQL = "SELECT EmpresaInterventor.CodContacto, Empresa.CodProyecto " +
                    //                             " FROM EmpresaInterventor " +
                    //                             " INNER JOIN Empresa ON EmpresaInterventor.CodEmpresa = Empresa.id_empresa " +
                    //                             " INNER JOIN PagoActividad ON Empresa.codproyecto = PagoActividad.CodProyecto " +
                    //                             " WHERE (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ") " +
                    //                             " AND (PagoActividad.Id_PagoActividad = " + lnk.Text + ")";

                    //            var rsInterventores = consultas.ObtenerDataTable(txtSQL, "text");

                    //            foreach (DataRow row_rsInterventores in rsInterventores.Rows)
                    //            {
                    //                AgendarTarea agenda = new AgendarTarea
                    //                    (Int32.Parse(row_rsInterventores["CodContacto"].ToString()),
                    //                    "Solicitud de Pago No. " + lnk.Text + " Rechazada por Coordinador de Interventoria",
                    //                    "Se ha rechazado la Solicitud de pago No " + lnk.Text + ". <BR><BR>Observaciones Coordinador: " + txtObservaciones.Text,
                    //                    row_rsInterventores["CodProyecto"].ToString(),
                    //                    2,
                    //                    "0",
                    //                    true,
                    //                    1,
                    //                    true,
                    //                    false,
                    //                    usuario.IdContacto,
                    //                    null,
                    //                    null,
                    //                    "Firma Coordinador");

                    //                agenda.Agendar();
                    //            }

                    //            rsInterventores = null;

                    //            #endregion

                    //            #region Mensaje 2.

                    //            //Emprendedores
                    //            txtSQL = " SELECT ProyectoContacto.CodContacto, ProyectoContacto.CodProyecto " +
                    //                     " FROM PagoActividad " +
                    //                     " INNER JOIN ProyectoContacto ON PagoActividad.CodProyecto = ProyectoContacto.CodProyecto " +
                    //                     " WHERE (PagoActividad.Id_PagoActividad = " + lnk.Text + ") " +
                    //                     " AND (ProyectoContacto.Inactivo = 0) " +
                    //                     " AND (dbo.ProyectoContacto.CodRol = " + Constantes.CONST_RolEmprendedor + ")";

                    //            var rsEmprendedores = consultas.ObtenerDataTable(txtSQL, "text");

                    //            foreach (DataRow row_rsEmprendedores in rsEmprendedores.Rows)
                    //            {
                    //                AgendarTarea agenda = new AgendarTarea
                    //                    (Int32.Parse(row_rsEmprendedores["CodContacto"].ToString()),
                    //                    "Solicitud de Pago No. " + lnk.Text + " Rechazada por Coordinador de Interventoria",
                    //                    "Se ha rechazado la Solicitud de pago No " + lnk.Text + ". </br></br>Observaciones Coordinador: " + txtObservaciones.Text,
                    //                    row_rsEmprendedores["CodProyecto"].ToString(),
                    //                    2,
                    //                    "0",
                    //                    true,
                    //                    1,
                    //                    true,
                    //                    false,
                    //                    usuario.IdContacto,
                    //                    null,
                    //                    null,
                    //                    "Firma Coordinador");

                    //                agenda.Agendar();
                    //            }

                    //            rsEmprendedores = null;
                    //            //Fin modificacion, Oct 19 2005

                    //            #endregion
                    //        }

                    //        System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('La Datos se han procesado con éxito.')", true);

                    //        //Se genera actividad para la fiduciaria, se genera una tarea avisando por email.
                    //        if (numAprobadas > 0)
                    //        {
                    //            if (Session["CodContatoFiduciaria"].ToString() != "")
                    //            {
                    //                AgendarTarea agenda = new AgendarTarea
                    //                    (Int32.Parse(Session["CodContatoFiduciaria"].ToString()),
                    //                    "Descargar Archivos de Solicitudes de Pago",
                    //                    "Se han generado nuevas solicitudes de pago, debe descargarlas para procesarlas en el sistema de la Fiduciaria.",
                    //                    "",
                    //                    //row_rsInterventores["CodProyecto"].ToString(),
                    //                    2,
                    //                    "0",
                    //                    true,
                    //                    1,
                    //                    true,
                    //                    false,
                    //                    usuario.IdContacto,
                    //                    null,
                    //                    null,
                    //                    "Firma Coordinador");

                    //                agenda.Agendar();
                    //            }
                    //        }
                    //        else
                    //        {

                    //        }
                    //    }
                    //    else
                    //    {
                    //        #region Se genero un error en una de las validaciones, se captura el error,

                    //        if (Session["ErrorCapicom"].ToString() != "")
                    //        {
                    //            CodRechazoFirmaDigital = TraerCodRechazoFirmaDigital(Session["ErrorCapicom"].ToString()).ToString();

                    //            txtSQL = " UPDATE PagosActaSolicitudes SET CodRechazoFirmaDigital = " + CodRechazoFirmaDigital + " WHERE Id_Acta = " + CodActa;

                    //            try
                    //            {
                    //                //NEW RESULTS:
                    //                cmd = new SqlCommand(txtSQL, con);

                    //                if (con != null) { if (con.State != ConnectionState.Open) { con.Open(); } }
                    //                cmd.CommandType = CommandType.Text;
                    //                cmd.ExecuteNonQuery();
                    //                con.Close();
                    //                con.Dispose();
                    //                cmd.Dispose();
                    //            }
                    //            catch { }

                    //            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('" + Session["MensajeErrorCapicom"].ToString() + "')", true);
                    //            return;
                    //        }

                    //        #endregion
                    //    }

                    //    #endregion
                    //} 
                    #endregion

                    #endregion

                    //Mostrar la página "FirmaCoordinador.aspx" con la información procesada.
                    Redirect(null, "FirmaCoordinador.aspx", "_self", "menubar=0,scrollbars=1,width=710,height=400,top=100");
                }

                #endregion
            }
            catch { }
        }
    }
}