using Fonade.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fonade.FONADE.interventoria
{
    public partial class SubirRespuestaPagos : Negocio.Base_Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnsu8bir_Click(object sender, EventArgs e)
        {
            ClientScriptManager cm = this.ClientScript;

            if (string.IsNullOrEmpty(fuarchivo.PostedFile.FileName))
            {
                cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert ('Debe seleccionar un archivo!!!');</script>");
            }
            else
            {
                string nombreArchivo;
                string extencionArchivo;

                nombreArchivo = System.IO.Path.GetFileName(fuarchivo.PostedFile.FileName);
                extencionArchivo = System.IO.Path.GetExtension(fuarchivo.PostedFile.FileName);

                if (extencionArchivo.ToLower().Equals("xls"))
                {
                    cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El archivo seleccionado no es valido');</script>");
                }
                else
                {
                    string saveLocation = Server.MapPath("\\FonadeDocumentos\\RespuestaPagos") + "\\" + nombreArchivo;

                    if (!(System.IO.File.Exists(saveLocation)))
                    {
                        fuarchivo.PostedFile.SaveAs(saveLocation);
                    }
                    else
                    {
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('Ya se encuantra almacenado un archivo con este nombre');</script>");
                        return;
                    }

                    DataTable dt = Excel.recogerExcel(saveLocation, "Pagos");

                    if (dt.Rows.Count <= 0)
                    {
                        if (!(System.IO.File.Exists(saveLocation)))
                        {
                            System.IO.File.Delete(saveLocation);
                        }
                        cm.RegisterClientScriptBlock(this.GetType(), "", "<script type='text/javascript'>alert('El archivo no contiene la hoja de nombre Pagos por favor verifique.');</script>");
                        return;
                    }
                    else
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string txtSQLHistorico = string.Empty;

                            int strCampo0, strCampo5, strCampo11, strCampo12, strCampo13, strCampo14, strCampo15, strCampo16, strCampo17, strCampo18, strCampo19, strCampo20, strCampo21, strCampo22;

                            try
                            {

                                #region insercion
                                bool val = Int32.TryParse(dr[0].ToString(), out strCampo0);
                                if (!val) strCampo0 = 0;

                                txtSQLHistorico = "insert into PagoActividadHistorico (";
		                        txtSQLHistorico = txtSQLHistorico + " CodPagoActividad, Estado, ObservacionesFA, FechaRtaFA";
		                        txtSQLHistorico = txtSQLHistorico + " , valorretefuente, valorreteiva, valorreteica, otrosdescuentos ";
                                txtSQLHistorico = txtSQLHistorico + " , codigopago, valorpagado,fechapago, fecharechazo";
                                txtSQLHistorico = txtSQLHistorico + " , ObservacionCambio, fechaRegistro";
                                txtSQLHistorico = txtSQLHistorico + ")";
                                txtSQLHistorico = txtSQLHistorico + "( select ";
                                txtSQLHistorico = txtSQLHistorico + " Id_PagoActividad, Estado, ObservacionesFA, FechaRtaFA";
		                        txtSQLHistorico = txtSQLHistorico + " , valorretefuente, valorreteiva, valorreteica, otrosdescuentos ";
                                txtSQLHistorico = txtSQLHistorico + " , codigopago, valorpagado,fechapago, fecharechazo";
                                txtSQLHistorico = txtSQLHistorico + " , ObservacionCambio, getdate()";
                                txtSQLHistorico = txtSQLHistorico + ")";

                                ejecutaReader(txtSQLHistorico, 1);

                                #endregion
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
        }
    }
}