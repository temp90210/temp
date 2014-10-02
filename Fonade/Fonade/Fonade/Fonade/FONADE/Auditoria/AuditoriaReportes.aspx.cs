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
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.IO;
using System.Text;
using System.Globalization;

namespace Fonade.FONADE.Auditoria
{
    public partial class AuditoriaReportes : Negocio.Base_Page
    {
        StreamWriter w;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_Titulo.Text = void_establecerTitulo("REPORTES DE AUDITORÍA");
                txt_fechainicio.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txt_fichaFin.Text = DateTime.Now.ToString("dd/MM/yyyy");
                llenarLista();
                lbx_tablas.Items[0].Selected = true;
            }
        }

        protected void llenarLista()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT name AS tbname FROM sysobjects WHERE id IN(SELECT parent_obj FROM sysobjects WHERE xtype='tr') order by name", con);
                con.Open();
                SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                { lbx_tablas.Items.Add(r[0].ToString()); }
            }
        }

        protected void btn_generareporte_Click(object sender, EventArgs e)
        {
            //generarExcel(); 
            CrearExcel();
        }

        public int CrearExcel()
        {
            try
            {
                string fechaInicial, fechaFinal;
                string tablaSelect = lbx_tablas.SelectedItem.ToString();
                fechaInicial = txt_fechainicio.Text + " 00:00:00";
                fechaFinal = txt_fichaFin.Text + " 23:59:59";
                DateTime fInicialSql = DateTime.ParseExact(txt_fechainicio.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime fFinalSql = DateTime.ParseExact(txt_fichaFin.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                fFinalSql = fFinalSql.AddDays(1);

                var fechaInicialSql = fInicialSql.Date.ToString("yyyy-MM-dd HH:mm:ss");
                var fechaFinalSql = fFinalSql.Date.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand cmd = new SqlCommand("MD_ReporteAuditoria", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tabla", tablaSelect);
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicialSql);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinalSql);
                SqlDataAdapter Adaptador = new SqlDataAdapter();
                Adaptador.SelectCommand = cmd;
                DataSet DS = new DataSet();

                try
                {
                    con.Open();
                }
                catch (Exception EX1)
                {
                    //Alert1.Ver("error de base de datos: " + EX1.Message, true);
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error en base de datos: " + EX1.Message + "');", true);
                    return 0;
                }

                Adaptador.Fill(DS);
                con.Close();

                if (DS.Tables[0].Rows.Count != 0)
                {
                    FileStream fs = new FileStream(Server.MapPath("/FONADE/Auditoria/reporteAuditoria.xls"), FileMode.Create, FileAccess.ReadWrite);
                    w = new StreamWriter(fs);
                    EscribeCabecera(tablaSelect, fechaInicial, fechaFinal);

                    foreach (DataRow Row in DS.Tables[0].Rows)
                    {

                        EscribeLinea(Row.ItemArray[6].ToString(), Row.ItemArray[5].ToString()
                            , Row.ItemArray[7].ToString(), Row.ItemArray[0].ToString()
                            , Row.ItemArray[1].ToString(), Row.ItemArray[2].ToString()
                            , Row.ItemArray[3].ToString()
                            , Convert.ToDateTime(Row.ItemArray[4]).ToString("dd 'de' MMMM 'de' yyyy hh:mm tt"));
                    }
                    EscribePiePagina();
                    w.Close();
                }
                else
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No hay datos de Auditoría para la tabla  " + tablaSelect + "  en el rango de fechas seleccionado!')", true);
                    //Alert1.Ver("No hay datos de Auditoría para la tabla  " + tablaSelect + "  en el rango de fechas seleccionado!", true);

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No hay datos de auditoría para la tabla " + tablaSelect + " en el rango de fechas seleccionado.');", true);
                    return 0;
                }

            }
            catch (Exception exc)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "Error al generar Excel: " + exc.Message + "')", true);
                //Alert1.Ver("error al generar excel: " + exc.Message, true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al cargar el excel: " + exc.Message + "');", true);
                return 0;
            }

            return 0;
        }

        public void EscribeCabecera(string NombreTabla, string desde, string hasta)
        {
            try
            {
                StringBuilder html = new StringBuilder();
                html.Append("<html>");
                html.Append("<head>");
                html.Append("<META HTTP-EQUIV='Content-Type' content='text/html; charset=utf-8'/>");
                html.Append("<style>");
                html.Append("<!--table");
                html.Append(".xl1510515");
                html.Append("	{padding-top:1px;");
                html.Append("	padding-right:1px;");
                html.Append("	padding-left:1px;");
                html.Append("	mso-ignore:padding;");
                html.Append("	color:black;");
                html.Append("	font-size:11.0pt;");
                html.Append("	font-weight:400;");
                html.Append("	font-style:normal;");
                html.Append("	text-decoration:none;");
                html.Append("	font-family:Calibri, sans-serif;");
                html.Append("	mso-font-charset:0;");
                html.Append("	mso-number-format:General;");
                html.Append("	text-align:general;");
                html.Append("	vertical-align:bottom;");
                html.Append("	mso-background-source:auto;");
                html.Append("	mso-pattern:auto;");
                html.Append("	white-space:nowrap;}");
                html.Append(".xl6310515");
                html.Append("	{padding-top:1px;");
                html.Append("	padding-right:1px;");
                html.Append("	padding-left:1px;");
                html.Append("	mso-ignore:padding;");
                html.Append("	color:#3A3838;");
                html.Append("	font-size:11.0pt;");
                html.Append("	font-weight:700;");
                html.Append("	font-style:normal;");
                html.Append("	text-decoration:none;");
                html.Append("	font-family:Calibri, sans-serif;");
                html.Append("	mso-font-charset:0;");
                html.Append("	mso-number-format:General;");
                html.Append("	text-align:center;");
                html.Append("	vertical-align:bottom;");
                html.Append("	mso-background-source:auto;");
                html.Append("	mso-pattern:auto;");
                html.Append("	white-space:nowrap;}");
                html.Append(".xl6410515");
                html.Append("	{padding-top:1px;");
                html.Append("	padding-right:1px;");
                html.Append("	padding-left:1px;");
                html.Append("	mso-ignore:padding;");
                html.Append("	color:#3A3838;");
                html.Append("	font-size:11.0pt;");
                html.Append("	font-weight:400;");
                html.Append("	font-style:normal;");
                html.Append("	text-decoration:none;");
                html.Append("	font-family:'Arial Narrow', sans-serif;");
                html.Append("	mso-font-charset:0;");
                html.Append("	mso-number-format:General;");
                html.Append("	text-align:center;");
                html.Append("	vertical-align:bottom;");
                html.Append("	mso-background-source:auto;");
                html.Append("	mso-pattern:auto;");
                html.Append("	white-space:nowrap;}");
                html.Append(".xl6510515");
                html.Append("	{padding-top:1px;");
                html.Append("	padding-right:1px;");
                html.Append("	padding-left:1px;");
                html.Append("	mso-ignore:padding;");
                html.Append("	color:#3A3838;");
                html.Append("	font-size:15.0pt;");
                html.Append("	font-weight:700;");
                html.Append("	font-style:normal;");
                html.Append("	text-decoration:none;");
                html.Append("	font-family:'Arial Narrow', sans-serif;");
                html.Append("	mso-font-charset:0;");
                html.Append("	mso-number-format:General;");
                html.Append("	text-align:general;");
                html.Append("	vertical-align:middle;");
                html.Append("	mso-background-source:auto;");
                html.Append("	mso-pattern:auto;");
                html.Append("	white-space:nowrap;}");
                html.Append("-->");
                html.Append("</style>");
                html.Append("</head>");
                html.Append("<body>");
                html.Append("<div align=center >");
                html.Append("<table border=0 cellpadding=0 cellspacing=0 width=1225 style='border-collapse:");
                html.Append(" collapse;table-layout:fixed;width:919pt'>");
                html.Append(" <col width=100 style='mso-width-source:userset;mso-width-alt:3657;width:75pt'>");
                html.Append(" <col width=236 style='mso-width-source:userset;mso-width-alt:8630;width:177pt'>");
                html.Append(" <col width=292 style='mso-width-source:userset;mso-width-alt:10678;width:219pt'>");
                html.Append(" <col width=115 style='mso-width-source:userset;mso-width-alt:4205;width:86pt'>");
                html.Append(" <col width=136 style='mso-width-source:userset;mso-width-alt:4973;width:102pt'>");
                html.Append(" <col width=80 style='width:60pt'>");
                html.Append(" <col width=73 style='mso-width-source:userset;mso-width-alt:2669;width:55pt'>");
                html.Append(" <col width=193 style='mso-width-source:userset;mso-width-alt:7058;width:145pt'>");
                html.Append(" <tr height=20 style='height:15.0pt'>");
                html.Append("  <td colspan=8 rowspan=2 height=40 class=xl6510515 width=1225");
                html.Append("  style='height:30.0pt;width:919pt'>REPORTE DE AUDITORÍA:");
                html.Append("  " + NombreTabla + ".<span style='mso-spacerun:yes'>  ");
                html.Append("  </span>desde: " + desde + "<span style='mso-spacerun:yes'> ");
                html.Append("  </span>hasta: " + hasta + "</td>");
                html.Append(" </tr>");
                html.Append(" <tr height=20 style='height:15.0pt'>");
                html.Append(" </tr>");
                html.Append(" <tr height=20 style='height:15.0pt'>");
                html.Append("  <td height=20 class=xl6310515 style='height:15.0pt'>Ip del equipo</td>");
                html.Append("  <td class=xl6310515>Id del Usuario que ejecutó el evento</td>");
                html.Append("  <td class=xl6310515>Nombre del Usuario que ejecutó el evento</td>");
                html.Append("  <td class=xl6310515>Tipo de Evento</td>");
                html.Append("  <td class=xl6310515>Campo Afectado</td>");
                html.Append("  <td class=xl6310515>Valor Inicial</td>");
                html.Append("  <td class=xl6310515>Valor Final</td>");
                html.Append("  <td class=xl6310515>Fecha de ejecución de Evento</td>");
                html.Append(" </tr>");
                w.Write(html.ToString());
            }
            catch (Exception exc)
            {
                //Alert1.Ver("error al generar excel (encabezado exccel): " + exc.Message, true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al generar excel (encabezado excel): " + exc.Message + "');", true);
                return;
            }
        }

        public void EscribeLinea(string ip, string idUsuario, string nomUsuario, string caso, string tabla, string datoant, string datonuevo, string fecha)
        {
            try
            {
                StringBuilder html = new StringBuilder();
                html.Append(" <tr height=22 style='height:16.5pt'>");
                html.Append("  <td height=22 class=xl6410515 style='height:16.5pt'>" + ip + "</td>");
                html.Append("  <td class=xl6410515>" + idUsuario + "</td>");
                html.Append("  <td class=xl6410515>" + nomUsuario + "</td>");
                html.Append("  <td class=xl6410515>" + caso + "</td>");
                html.Append("  <td class=xl6410515>" + tabla + "</td>");
                html.Append("  <td class=xl6410515>" + datoant + "</td>");
                html.Append("  <td class=xl6410515>" + datonuevo + "</td>");
                html.Append("  <td class=xl6410515>" + fecha + "</td>");
                html.Append(" </tr>");
                w.Write(html.ToString());
            }
            catch (Exception exc)
            {
                //Alert1.Ver("error al generar excel (escribir fila a fila método): " + exc.Message, true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al generar excel (escribir fila a fila método): " + exc.Message + "');", true);
                return;
            }

        }

        public void EscribePiePagina()
        {
            try
            {
                StringBuilder html = new StringBuilder();
                html.Append(" <![if supportMisalignedColumns]>");
                html.Append(" <tr height=0 style='display:none'>");
                html.Append("  <td width=100 style='width:75pt'></td>");
                html.Append("  <td width=236 style='width:177pt'></td>");
                html.Append("  <td width=292 style='width:219pt'></td>");
                html.Append("  <td width=115 style='width:86pt'></td>");
                html.Append("  <td width=136 style='width:102pt'></td>");
                html.Append("  <td width=80 style='width:60pt'></td>");
                html.Append("  <td width=73 style='width:55pt'></td>");
                html.Append("  <td width=193 style='width:145pt'></td>");
                html.Append(" </tr>");
                html.Append(" <![endif]>");
                html.Append("</table>");
                html.Append("</div>");
                html.Append("</body>");
                html.Append("</html>");
                w.Write(html.ToString());
                btn_descargar.Visible = true;
            }
            catch (Exception exc)
            {
                //Alert1.Ver("error al generar excel (final excel): " + exc.Message, true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('Error al generar excel (final excel): " + exc.Message + "');", true);
                return;
            }

        }

        protected void btn_descargar_Click(object sender, EventArgs e)
        {
            btn_descargar.Visible = false;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "document.location.href  ='/FONADE/Auditoria/reporteAuditoria.xls';", true);
        }

        //protected void generarExcel()
        //{
        //    try
        //    {
        //        string fechaInicial, fechaFinal;
        //        string tablaSelect = lbx_tablas.SelectedItem.ToString();
        //        fechaInicial = Convert.ToDateTime(txt_fechainicio.Text).ToString("yyyy-MM-dd") + " 00:00:00";
        //        fechaFinal = Convert.ToDateTime(txt_fichaFin.Text).ToString("yyyy-MM-dd") + " 23:59:59";

        //        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
        //        SqlCommand cmd = new SqlCommand("MD_ReporteAuditoria", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@tabla", tablaSelect);
        //        cmd.Parameters.AddWithValue("@fechaInicio", fechaInicial);
        //        cmd.Parameters.AddWithValue("@fechaFin", fechaFinal);
        //        SqlDataAdapter Adaptador = new SqlDataAdapter();
        //        Adaptador.SelectCommand = cmd;
        //        DataSet DS = new DataSet();

        //        con.Open();
        //        Adaptador.Fill(DS);
        //        con.Close();

        //        if (DS.Tables[0].Rows.Count != 0)
        //        {
        //            Microsoft.Office.Interop.Excel.Application Mi_Excel = default(Microsoft.Office.Interop.Excel.Application);
        //            Microsoft.Office.Interop.Excel.Workbook LibroExcel = default(Microsoft.Office.Interop.Excel.Workbook);
        //            Microsoft.Office.Interop.Excel.Worksheet HojaExcel = default(Microsoft.Office.Interop.Excel.Worksheet);
        //            Mi_Excel = new Microsoft.Office.Interop.Excel.Application();

        //            LibroExcel = Mi_Excel.Workbooks.Add();
        //            HojaExcel = (Microsoft.Office.Interop.Excel.Worksheet)LibroExcel.Worksheets[1];
        //            HojaExcel.Visible = Microsoft.Office.Interop.Excel.XlSheetVisibility.xlSheetVisible;

        //            string fech1 = Convert.ToDateTime(txt_fechainicio.Text).ToString("dd 'de' MMMM 'de' yyyy") + " 00:00:00";
        //            string fech2 = Convert.ToDateTime(txt_fichaFin.Text).ToString("dd 'de' MMMM 'de' yyyy") + " 23:59:59";

        //            HojaExcel.Range["A1:H2"].Merge();
        //            HojaExcel.Range["A1:H2"].Value = "REPORTE DE AUDITORÍA: " + tablaSelect + ".   desde: " + fech1 + "  hasta: " + fech2;
        //            HojaExcel.Range["A1:H2"].VerticalAlignment = XlVAlign.xlVAlignCenter;
        //            HojaExcel.Range["A1:H2"].Font.Bold = true;
        //            HojaExcel.Range["A1:H2"].Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);
        //            HojaExcel.Range["A1:H2"].Font.Size = 15;
        //            HojaExcel.Range["A1:H2"].Font.Name = "Arial Narrow";

        //            Microsoft.Office.Interop.Excel.Range objCelda = HojaExcel.Range["A3", Type.Missing];
        //            objCelda.Value = "Ip del equipo";
        //            objCelda.Font.Bold = true;
        //            objCelda.Font.Name = "Calibri";
        //            objCelda.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);

        //            objCelda = HojaExcel.Range["B3", Type.Missing];
        //            objCelda.Value = "Id del Usuario que ejecuto el evento";
        //            objCelda.Font.Bold = true;
        //            objCelda.Font.Name = "Calibri";
        //            objCelda.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);

        //            objCelda = HojaExcel.Range["C3", Type.Missing];
        //            objCelda.Value = "Nombre del Usuario que ejecuto el evento";
        //            objCelda.Font.Bold = true;
        //            objCelda.Font.Name = "Calibri";
        //            objCelda.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);

        //            objCelda = HojaExcel.Range["D3", Type.Missing];
        //            objCelda.Value = "Tipo de Evento";
        //            objCelda.Font.Bold = true;
        //            objCelda.Font.Name = "Calibri";
        //            objCelda.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);

        //            objCelda = HojaExcel.Range["E3", Type.Missing];
        //            objCelda.Value = "Campo Afectado";
        //            objCelda.Font.Bold = true;
        //            objCelda.Font.Name = "Calibri";
        //            objCelda.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);

        //            objCelda = HojaExcel.Range["F3", Type.Missing];
        //            objCelda.Value = "Valor Inicial";
        //            objCelda.Font.Bold = true;
        //            objCelda.Font.Name = "Calibri";
        //            objCelda.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);

        //            objCelda = HojaExcel.Range["G3", Type.Missing];
        //            objCelda.Value = "Valor Final";
        //            objCelda.Font.Bold = true;
        //            objCelda.Font.Name = "Calibri";
        //            objCelda.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);

        //            objCelda = HojaExcel.Range["H3", Type.Missing];
        //            objCelda.Value = "Fecha de ejecución de Evento";
        //            objCelda.Font.Bold = true;
        //            objCelda.Font.Name = "Calibri";
        //            objCelda.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);

        //            //Mi_Excel.Visible = true;

        //            int i = 4;
        //            foreach (DataRow Row in DS.Tables[0].Rows)
        //            {
        //                try
        //                {
        //                    HojaExcel.Cells[i, "A"] = Row.ItemArray[6];
        //                    HojaExcel.Cells[i, "B"] = Row.ItemArray[5];
        //                    HojaExcel.Cells[i, "C"] = Row.ItemArray[7];
        //                    HojaExcel.Cells[i, "D"] = Row.ItemArray[0];
        //                    HojaExcel.Cells[i, "E"] = Row.ItemArray[1];
        //                    HojaExcel.Cells[i, "F"] = Row.ItemArray[2];
        //                    HojaExcel.Cells[i, "G"] = Row.ItemArray[3];
        //                    HojaExcel.Cells[i, "H"] = Convert.ToDateTime(Row.ItemArray[4]).ToString("dd 'de' MMMM 'de' yyyy hh:mm tt");
        //                    i++;
        //                }
        //                catch (Exception ex)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "Error al generar Excel: " + ex.Message + "')", true);
        //                }

        //            }
        //            Microsoft.Office.Interop.Excel.Range Rangodatos = HojaExcel.Range["A4:H" + (i - 1).ToString()];
        //            Rangodatos.Font.Color = System.Drawing.Color.FromArgb(58, 56, 56);
        //            Rangodatos.Font.Name = "Arial Narrow";

        //            Microsoft.Office.Interop.Excel.Range Rangofiltros = HojaExcel.Range["C3:D3"];
        //            Rangofiltros.AutoFilter(1);

        //            Microsoft.Office.Interop.Excel.Range Rango = HojaExcel.Range["A3:H" + (i - 1).ToString()];
        //            Rango.Select();
        //            Rango.Columns.AutoFit();
        //            Rango.Columns.HorizontalAlignment = XlHAlign.xlHAlignCenter;
        //            try
        //            {
        //                string rutaArchivo = "~/FONADE/Auditoria/Reporte Auditoria " + tablaSelect + ".xls"; //Mi_Excel.GetSaveAsFilename("Reporte Auditoría " + tablaSelect , "Archivos de Excel (*.xls), *.xls", 1, "Guardar como", Missing.Value).ToString();

        //                Mi_Excel.ActiveWorkbook.Close(true, rutaArchivo, Type.Missing);
        //            }
        //            catch (System.Runtime.InteropServices.COMException ex)
        //            {
        //                Response.Write(ex.Message);
        //                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "Error al generar Excel: " + ex.Message + "')", true);
        //            }

        //        }
        //        else
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "alert('No hay datos de Auditoría para la tabla  " + tablaSelect + "  en el rango de fechas seleccionado!')", true);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        Response.Write(exc.Message);
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Mensaje", "Error al generar Excel: " + exc.Message + "')", true);
        //    }


        //}
    }
}