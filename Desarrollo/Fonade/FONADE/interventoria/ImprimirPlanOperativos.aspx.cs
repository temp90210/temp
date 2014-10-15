using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Datos;

namespace Fonade.FONADE.interventoria
{
    public partial class ImprimirPlanOperativos : Negocio.Base_Page
    {
        #region Variables globales.

        /// <summary>
        /// Código del Proyecto.
        /// </summary>
        int CodProyecto;

        /// <summary>
        /// Contiene las consultas de SQL.
        /// </summary>
        String txtSQL;

        #endregion

        /// <summary>
        /// Page_Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    #region Obtener el código del proyecto.

                    CodProyecto = (int)
                                (!string.IsNullOrEmpty(Session["CodProyecto"].ToString())
                                     ? Convert.ToInt64(Session["CodProyecto"])
                                     : 0);

                    #endregion

                    CargarInformeCompleto();
                }
            }
            catch { Response.Redirect("FramePlanOperativoInterventoria.aspx"); }

            #region Establecer fecha.

            //Establecer fecha.
            DateTime fecha = DateTime.Now;
            string sMes = fecha.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
            L_Fecha.Text = UppercaseFirst(sMes) + " " + fecha.Day + " de " + fecha.Year;

            #endregion

            #region Comentarios NO BORRAR!.
            //datosGenerales();

            //CrearTabla();

            //llennarTabla(); 
            #endregion
        }

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

        /// <summary>
        /// Mauricio Arias Olave.
        /// 10/07/2014.
        /// Cargar el informe completo.
        /// </summary>
        private void CargarInformeCompleto()
        {
            #region Inicializar variables.

            DataTable RS = new DataTable();
            String CodEmpresa = "";
            String Empresa = "";
            String Telefono = "";
            String Direccion = "";
            String Ciudad = "";
            String CodContacto = "";
            String nomInterventor = "";
            String apeInterventor = "";
            String NumeroContrato = "";
            String NombreCoordinador = "";
            DataTable rsActividades = new DataTable();
            String CodActividad = "";
            String NomActividad = "";
            Double TotalFE = 0;
            Double TotalEmp = 0;
            int filas = 0;
            int z = 0;
            Double TotalTipo1 = 0;
            Double TotalTipo2 = 0;
            DataTable rsTipo1 = new DataTable();
            DataTable rsTipo2 = new DataTable();
            String Tipo1 = "";
            String Tipo2 = "";
            DataTable rsActividad = new DataTable();
            #region Obtener el valor de la prórroga para sumarla a la constante de meses generar la tabla.
            int CONST_Meses = 12 + ObtenerProrroga(CodProyecto.ToString());
            #endregion
            DataTable rsCargo = new DataTable();
            int i = 0;
            Double TotalTipo1A = 0;
            Double TotalTipo2A = 0;
            DataTable rsCargoss = new DataTable();
            DataTable rsCargos = new DataTable();
            DataTable rsProducto = new DataTable();
            DataTable rsProductos = new DataTable();
            String txtProducto = "";
            #endregion

            try
            {
                #region Trae datos de la empresa.

                txtSQL = " SELECT Id_Empresa, RazonSocial, Telefono, DomicilioEmpresa, NomCiudad FROM Empresa, Ciudad WHERE CodCiudad = id_ciudad and CodProyecto = " + CodProyecto;

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0)
                {
                    CodEmpresa = RS.Rows[0]["Id_Empresa"].ToString();
                    Empresa = RS.Rows[0]["RazonSocial"].ToString();
                    Telefono = RS.Rows[0]["Telefono"].ToString();
                    Direccion = RS.Rows[0]["DomicilioEmpresa"].ToString();
                    Ciudad = RS.Rows[0]["NomCiudad"].ToString();
                }

                #endregion

                #region Se trae los datos del interventor.

                txtSQL = " SELECT Contacto.Id_Contacto, Contacto.Nombres, Contacto.Apellidos " +
                         " FROM EmpresaInterventor INNER JOIN Contacto " +
                         " ON EmpresaInterventor.CodContacto = Contacto.Id_Contacto " +
                         " WHERE (EmpresaInterventor.CodEmpresa = " + CodEmpresa + ") " +
                         " AND (EmpresaInterventor.Rol = " + Constantes.CONST_RolInterventorLider + ")";

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0)
                {
                    CodContacto = RS.Rows[0]["Id_Contacto"].ToString();
                    nomInterventor = RS.Rows[0]["Nombres"].ToString();
                    apeInterventor = RS.Rows[0]["Apellidos"].ToString();
                }

                #endregion

                #region Información del interventor.

                lbl_cuerpo.Text = "<table width='100%' align=center border='1' cellpadding='0' cellspacing='0' bordercolor='#CCCCCC'>" +
                        " <tr>" +
                        " 	 <td valign='top' width='100%'><br/>" +
                        " 	 	<TABLE width='100%' align=center border='0' cellpadding='0' cellspacing='0'>" +
                        " 	 		<TR>" +
                        " 	 			<TD align='center' COLSPAN='2'>" +
                        " 	 				<B></B>" +
                        " 	 			</TD>" +
                        " 	 		</TR>" +
                        " 	 		<TR>" +
                        " 	 			<TD align='center' COLSPAN='2'>" +
                        " 	 				<B>Interventor: </B>" + nomInterventor + " " + apeInterventor +
                        " 	 			</TD>" +
                        " 	 		</TR>";

                #endregion

                #region Se trae el Coordinador de Interventoría.

                txtSQL = " SELECT nombres, apellidos FROM contacto " +
                         " WHERE id_contacto In (SELECT CodCoordinador FROM interventor " +
                         " WHERE codcontacto = " + CodContacto + ")";

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0) { NombreCoordinador = RS.Rows[0]["Nombres"].ToString() + " " + RS.Rows[0]["Apellidos"].ToString(); }

                #endregion

                #region Se trae el número de contrato asociado a la empresa.

                txtSQL = "SELECT ContratoEmpresa.NumeroContrato FROM ContratoEmpresa " +
                         " INNER JOIN Empresa ON ContratoEmpresa.CodEmpresa = Empresa.id_empresa" +
                         " WHERE (Empresa.codproyecto = " + CodProyecto + ")";

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                if (RS.Rows.Count > 0) { NumeroContrato = RS.Rows[0]["NumeroContrato"].ToString(); }

                #endregion

                #region Dibujar Coordinador, Número del contrato y Empresa.

                lbl_cuerpo.Text = lbl_cuerpo.Text + "<TR>" +
                        "		<TD align='center' COLSPAN='2'>" +
                        "			<b>Coordinador: </b>" + NombreCoordinador +
                        "		</TD>" +
                        "	</TR>" +
                        "	<TR>" +
                        "		<TD align='center' bgcolor='#CCCCCC'>" +
                        "			<b>Número Contrato: </b>" + NumeroContrato +
                        "		</TD>" +
                        "		<TD align='center' bgcolor='#CCCCCC'>" +
                        "		</TD>" +
                        "	</TR>" +
                        "	<TR>" +
                        "		<TD ALIGN='center'>" +
                        "			<b>Empresa: </b>" + Empresa +
                        "		</TD>" +
                        "		<TD ALIGN='center'><b>Socios:</b>";

                #endregion

                #region Trae los socios de la empresa.

                txtSQL = " SELECT Nombres + ' ' + Apellidos AS Nombre, Identificacion FROM Contacto " +
                         " WHERE (Id_Contacto IN " +
                         " (SELECT EmpresaContacto.codcontacto " +
                         " FROM EmpresaContacto INNER JOIN " +
                         " Empresa ON EmpresaContacto.codempresa = Empresa.id_empresa " +
                         " WHERE (Empresa.codproyecto = " + CodProyecto + "))) ";

                RS = consultas.ObtenerDataTable(txtSQL, "text");

                foreach (DataRow row in RS.Rows)
                { lbl_cuerpo.Text = lbl_cuerpo.Text + "<br><b>" + row["Nombre"].ToString() + "</b> Identificación: <b>" + row["Identificacion"].ToString() + "</b>"; }

                #endregion

                #region Dibujar Teléfono, Dirección y la Ciudad "INICIO DE PLAN OPERATIVO".

                lbl_cuerpo.Text = lbl_cuerpo.Text +
                        "</TD>" +
                        "</TR>" +
                        " <TR bgcolor='#CCCCCC'>" +
                        " 		<TD align='center' Class='TitDestacado'>" +
                        " 			<b>Teléfono: </b>" + Telefono +
                        " 		</TD>" +
                        " 		<TD align='center' Class='TitDestacado'>" +
                        " 			<b>Dirección: </b>" + Direccion + " - " + Ciudad +
                        " 		</TD>" +
                        " 	</TR>" +
                        "<TR>" +
                        "	<TD align='center' colspan=2>" +
                        "		<TABLE width='100%' align=center border='1' cellpadding='0' cellspacing='0'>" +
                        "  <tr> " +
                        "    <td><p>&nbsp;</p></td>" +
                        "  </tr>	" +
                        "  <tr> " +
                        "    <td><p>&nbsp;</p></td>" +
                        "  </tr>			" +
                        "			<tr>" +
                        "				<td bgcolor='#CCCCCC' align=center><B>PLAN OPERATIVO</B></td>" +
                        "			</tr>" +
                        "			<tr>" +
                        "				<td align=center>" +
                        "					<TABLE width='100%' align=center border='1' cellpadding='0' cellspacing='0'>";
                #endregion

                #region Se traen las actividades del plan operativo.

                if (CodProyecto != 0)
                {
                    txtSQL = " select id_Actividad,Item, Nomactividad, Metas " +
                                     " from proyectoactividadPOInterventor where codproyecto =" + CodProyecto + " order by Item ";

                    rsActividades = consultas.ObtenerDataTable(txtSQL, "text");

                    foreach (DataRow row_actividades in rsActividades.Rows)
                    {
                        #region Ciclo inicial.

                        #region Nombre de la actividad.

                        lbl_cuerpo.Text = lbl_cuerpo.Text + "<TR>" +
                                                            "	<TD COLSPAN='12'>Actividad: " + row_actividades["Nomactividad"].ToString() + "</TD>" +
                                                            "</TR>";
                        #endregion

                        txtSQL = " SELECT * FROM ProyectoActividadPOMesInterventor " +
                                 " RIGHT OUTER JOIN ProyectoActividadPOInterventor " +
                                 " ON ProyectoActividadPOMesInterventor.CodActividad = ProyectoActividadPOInterventor.Id_Actividad " +
                                 " WHERE (ProyectoActividadPOInterventor.CodProyecto = " + CodProyecto + ") " +
                                 " AND (ProyectoActividadPOMesInterventor.CodActividad = " + row_actividades["id_Actividad"].ToString() + ") " +
                                 " ORDER BY ProyectoActividadPOInterventor.Item, ProyectoActividadPOMesInterventor.Mes, " +
                                 " ProyectoActividadPOMesInterventor.CodTipoFinanciacion ";

                        rsActividad = consultas.ObtenerDataTable(txtSQL, "text");
                        CodActividad = "";
                        i = 0;

                        foreach (DataRow r_rsActividad in rsActividad.Rows)
                        {
                            #region Ciclo...

                            NomActividad = r_rsActividad["NomActividad"].ToString();

                            TotalFE = 0;
                            TotalEmp = 0;

                            if (CodActividad != r_rsActividad["id_actividad"].ToString())
                            {
                                CodActividad = r_rsActividad["id_actividad"].ToString();
                                lbl_cuerpo.Text = lbl_cuerpo.Text + " <tr align='left' valign='top'>";
                                i = i + 1;
                            }

                            //Para pintar los datos en dos filas...

                            if (CONST_Meses % 6 > 0)
                            { filas = (CONST_Meses / 6) + 1; }
                            else
                            { filas = (CONST_Meses / 6); }

                            #region For (2).

                            for (int m = 1; m <= filas; m++)
                            {
                                z = m * 6 - 5;
                                if (!String.IsNullOrEmpty(r_rsActividad["Mes"].ToString()))
                                {
                                    #region Se pintan los titulos de los meses.

                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "					<tr>";

                                    for (int j = z; j <= 6 * m; j++)
                                    {
                                        if (j <= CONST_Meses)
                                        {
                                            if (j > 12) { }//aaa
                                            lbl_cuerpo.Text = lbl_cuerpo.Text + "					<td colspan=2 align=center>Mes " + j + "</td>";
                                        }
                                    }

                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "					</tr>";

                                    #endregion

                                    #region Se pintan los titulos de los tipos de aportes.

                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "					<tr>";

                                    for (int j = z; j <= 6 * m; j++)
                                    {
                                        if (j <= CONST_Meses)
                                        {
                                            lbl_cuerpo.Text = lbl_cuerpo.Text + "					<td align=center>Fondo</td><td align=center>Emprendedor</td>";
                                        }
                                    }

                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "					</tr>";

                                    #endregion

                                    for (int j = z; j <= 6 * m; j++)
                                    {
                                        if (j <= CONST_Meses)
                                        {
                                            #region Mes y CodTipoFinanciacion (ciclo).

                                            for (int k = 1; k <= Constantes.CONST_Fuentes; k++) //Fuentes de Financiación.
                                            {
                                                if (rsActividades.Rows.Count > 0)
                                                {
                                                    if (j == Int32.Parse(r_rsActividad["Mes"].ToString()) && k == Int32.Parse(r_rsActividad["CodTipoFinanciacion"].ToString()))
                                                    {
                                                        #region Colocar el valor.

                                                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + Double.Parse(r_rsActividad["Valor"].ToString()).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";

                                                        switch (k)
                                                        {
                                                            case 1:
                                                                TotalFE = TotalFE + Double.Parse(r_rsActividad["Valor"].ToString());
                                                                break;
                                                            case 2:
                                                                TotalEmp = TotalEmp + Double.Parse(r_rsActividad["Valor"].ToString());
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                        //rsActividad.movenext 

                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>&nbsp;</td>";
                                                    }
                                                }
                                                else
                                                {
                                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>&nbsp;</td>";
                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                    //Costo Total.
                                    //lbl_cuerpo.Text = lbl_cuerpo.Text = "                 <td align='right'>"&formatCurrency(TotalFE,2)&"</td>";
                                    //lbl_cuerpo.Text = lbl_cuerpo.Text = "                <td align='right'>"&formatCurrency(TotalEmp,2)&"</td>";
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                                }
                                else
                                {
                                    #region CONST_Meses * Constantes.CONST_Fuentes
                                    for (int j = z; j < CONST_Meses * Constantes.CONST_Fuentes; j++)//abc
                                    {
                                        //lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>&nbsp;</td>";
                                        //lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>&nbsp;</td>";
                                    }
                                    #endregion

                                    //Costo Total
                                    //lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>"&formatCurrency(TotalFE,2)&"</td>"&VbCrLf
                                    //lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>"&formatCurrency(TotalEmp,2)&"</td>"&VbCrLf
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                                    //rsActividad.movenext
                                }

                                //Avances reportados.
                                lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";

                                TotalTipo1 = 0;
                                TotalTipo2 = 0;

                                for (int j = z; j <= 6 * m; j++)
                                {
                                    if (j <= CONST_Meses)
                                    {
                                        #region Tipo 1.

                                        txtSQL = " SELECT * " +
                                                 " FROM AvanceActividadPOMes " +
                                                 " WHERE codactividad = " + CodActividad +
                                                 " AND Mes = " + j + " AND codtipofinanciacion = 1 ";

                                        rsTipo1 = consultas.ObtenerDataTable(txtSQL, "text");

                                        if (rsTipo1.Rows.Count > 0)
                                        {
                                            Tipo1 = Double.Parse(rsTipo1.Rows[0]["valor"].ToString()).ToString();
                                            TotalTipo1 = TotalTipo1 + Double.Parse(rsTipo1.Rows[0]["valor"].ToString());
                                        }
                                        else
                                        {
                                            Tipo1 = "";
                                        }

                                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <td align='right'>";
                                        if (Tipo1 != "")
                                        { lbl_cuerpo.Text = lbl_cuerpo.Text + Double.Parse(rsTipo1.Rows[0]["valor"].ToString()).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")); }
                                        else { lbl_cuerpo.Text = lbl_cuerpo.Text + Tipo1; }
                                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </td>";

                                        #endregion

                                        #region Tipo 2.

                                        txtSQL = " SELECT * " +
                                                 " FROM AvanceActividadPOMes " +
                                                 " WHERE codactividad=" + CodActividad +
                                                 " AND Mes=" + j + " AND codtipofinanciacion = 2 ";

                                        rsTipo2 = consultas.ObtenerDataTable(txtSQL, "text");

                                        if (rsTipo2.Rows.Count > 0)
                                        {
                                            Tipo2 = Double.Parse(rsTipo2.Rows[0]["valor"].ToString()).ToString();
                                            TotalTipo2 = TotalTipo2 + Double.Parse(rsTipo2.Rows[0]["valor"].ToString());
                                        }
                                        else
                                        {
                                            Tipo2 = "";
                                        }

                                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <td align='right'><font color='#CC0000'>";
                                        if (Tipo2 != "")
                                        { lbl_cuerpo.Text = lbl_cuerpo.Text + Double.Parse(rsTipo2.Rows[0]["valor"].ToString()).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")); }
                                        else { lbl_cuerpo.Text = lbl_cuerpo.Text + Tipo2; }
                                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </font></td>";

                                        #endregion
                                    }
                                }

                                //Costo Total de Avances reportados
                                //lbl_cuerpo.Text = lbl_cuerpo.Text + "              <td align='right'><font color='#CC0000'>"&VbCrLf
                                //lbl_cuerpo.Text = lbl_cuerpo.Text + formatCurrency(TotalTipo1,2)
                                //lbl_cuerpo.Text = lbl_cuerpo.Text + "              </font></td>"&VbCrLf
                                //lbl_cuerpo.Text = lbl_cuerpo.Text + "              <td align='right'><font color='#CC0000'>"&VbCrLf
                                //lbl_cuerpo.Text = lbl_cuerpo.Text + formatCurrency(TotalTipo2,2)
                                //lbl_cuerpo.Text = lbl_cuerpo.Text + "              </font> </td>"&VbCrLf
                                lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            }

                            #endregion

                            #region Líneas...

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "							<TR>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "								<TD COLSPAN='2'>Costo Total:</TD>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "							</TR>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "							<TR>";

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "								<TD align='Center'>Emprendedor</TD>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "								<TD align='Center'>Fondo</TD>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "							</TR>";

                            //Costo Total
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalEmp.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalFE.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                            //Costo Total de Avances reportados
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <td align='right'>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + TotalTipo1.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </td>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <td align='right'>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + TotalTipo2.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </td>";
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                            #endregion

                            #endregion
                        }

                        //rsActividades.MoveNext 

                        #endregion
                    }
                }

                #endregion

                #region Nómina.

                lbl_cuerpo.Text = lbl_cuerpo.Text + "</TABLE>" +
                                                    "		</td>" +
                                                    "	</tr>" +
                                                    "	<tr>" +
                                                    "		<td colspan=12 bgcolor='#CCCCCC' class='TitDestacado' align=center><B>NÓMINA</B></td>" +
                                                    "	</tr>" +
                                                    "	<tr>" +
                                                    "		<td class='TitDestacado' align=center>" +
                                                    "			<TABLE width='100%' align=center border='1' cellpadding='0' cellspacing='0'>";

                #region Personal Calificado - Cargos.

                txtSQL = " select Id_Nomina, Cargo from InterventorNomina where Tipo='Cargo' and codproyecto=" + CodProyecto + " order by id_nomina ";
                rsCargo = consultas.ObtenerDataTable(txtSQL, "text");

                i = 0;

                if (rsCargo.Rows.Count > 0)
                {
                    lbl_cuerpo.Text = lbl_cuerpo.Text + "     		<tr class='Titulo'> " +
                                                        "       			<td colspan=12><b>Personal Calificado</b></td>" +
                                                        "     		</tr>";
                    #region Cargo.

                    foreach (DataRow r_rsCargo in rsCargo.Rows)
                    {
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr align='left' valign='top'>" +
                                                            "                <td align='left' colspan=12>" + r_rsCargo["Cargo"].ToString() + "</td>" +
                                                            "              </tr>";
                        i = i + 1;

                        TotalTipo1 = 0;
                        TotalTipo2 = 0;

                        TotalTipo1A = 0;
                        TotalTipo2A = 0;

                        //para pintar los datos en dos filas.
                        for (int m = 1; m <= filas; m++)
                        {
                            #region Interno.

                            z = m * 6 - 5;

                            #region Pinta los titulos de los meses.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    if (k > 12) { }
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='center' colspan=2>Mes " + k + "</td>";
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Pinta los titulos de sueldo/prestaciones.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='center'>Sueldo</td><td align='center'>Prestaciones</td>";
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Pinta los valores de nómina.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";

                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    #region Trae valores de tipo sueldo.

                                    txtSQL = " SELECT * FROM InterventorNomina a, InterventorNominaMes b " +
                                             " WHERE a.tipo = 'Cargo' AND id_nomina = codcargo AND b.Tipo = 1 " +
                                             " AND codproyecto = " + CodProyecto +
                                             " AND mes = " + k +
                                             " ORDER BY id_nomina, mes, b.tipo";

                                    rsCargoss = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo1 = "&nbsp;";

                                    if (rsCargoss.Rows.Count > 0)
                                    {
                                        Tipo1 = rsCargoss.Rows[0]["Valor"].ToString();
                                        TotalTipo1 = TotalTipo1 + Double.Parse(rsCargoss.Rows[0]["Valor"].ToString());
                                    }

                                    #endregion

                                    #region Trae valores de tipo prestaciones.

                                    txtSQL = " SELECT * FROM InterventorNomina a, InterventorNominaMes b " +
                                             " WHERE a.tipo = 'Cargo' AND id_nomina=codcargo AND b.Tipo = 2" +
                                             " AND codproyecto = " + CodProyecto +
                                             " AND mes = " + k +
                                             " ORDER BY id_nomina, mes, b.tipo ";
                                    rsCargoss = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo2 = "&nbsp;";

                                    if (rsCargoss.Rows.Count > 0)
                                    {
                                        Tipo2 = rsCargoss.Rows[0]["Valor"].ToString();
                                        TotalTipo2 = TotalTipo2 + Double.Parse(rsCargoss.Rows[0]["Valor"].ToString());
                                    }

                                    #endregion

                                    if (Tipo1 != "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td>" + Double.Parse(Tipo1).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td>" + Tipo1 + "</td>"; }

                                    if (Tipo2 != "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td>" + Double.Parse(Tipo2).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td>" + Tipo2 + "</td>"; }
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Pinta los valores reportados por el emprendedor.
                            //for (int k = z; i < m * 6; i++)//mal
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    #region Tipo 1.

                                    txtSQL = " select * " +
                                             " from AvanceCargoPOMes " +
                                             " where CodCargo = " + r_rsCargo["Id_Nomina"].ToString() +
                                             " and Mes = " + k + " and codtipofinanciacion = 1 ";

                                    rsTipo1 = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (rsTipo1.Rows.Count > 0)
                                    {
                                        Tipo1 = rsTipo1.Rows[0]["valor"].ToString();
                                        TotalTipo1A = TotalTipo1A + Double.Parse(rsTipo1.Rows[0]["valor"].ToString());
                                    }
                                    else
                                    { Tipo1 = ""; }

                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "              <td align='right'>";
                                    if (Tipo1 != "")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + Double.Parse(Tipo1).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")); }
                                    else { lbl_cuerpo.Text = lbl_cuerpo.Text + Tipo1; }
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "              </td>";

                                    #endregion

                                    #region Tipo 2.

                                    txtSQL = " select * " +
                                             " from AvanceCargoPOMes " +
                                             " where CodCargo = " + r_rsCargo["Id_Nomina"].ToString() +
                                             " and Mes=" + k + " and codtipofinanciacion = 2 ";

                                    rsTipo2 = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (rsTipo2.Rows.Count > 0)
                                    {
                                        Tipo2 = rsTipo2.Rows[0]["valor"].ToString();
                                        TotalTipo2A = TotalTipo2A + Double.Parse(rsTipo2.Rows[0]["valor"].ToString());
                                    }
                                    else
                                    { Tipo2 = ""; }

                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "              <td align='right'>";
                                    if (Tipo2 != "")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + Double.Parse(Tipo2).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")); }
                                    else { lbl_cuerpo.Text = lbl_cuerpo.Text + Tipo2; }
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "              </td>";

                                    #endregion
                                }
                            }
                            #endregion

                            #endregion
                        }

                        #region Líneas finales.

                        //Costo Total
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td colspan=2>Costo Total</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalTipo1.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalTipo2.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                        //Costo Total avances reportados
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalTipo1A.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalTipo2A.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                        //rsCargo.movenext

                        #endregion
                    }

                    #endregion
                }

                #endregion

                #region Personal Calificado - Insumos.

                txtSQL = " select * from InterventorNomina where Tipo = 'Insumo' and codproyecto = " + CodProyecto + " order by id_nomina";
                rsCargo = consultas.ObtenerDataTable(txtSQL, "text");
                i = 0;

                if (rsCargo.Rows.Count > 0)
                {
                    lbl_cuerpo.Text = lbl_cuerpo.Text + "     		<tr class='Titulo'> " +
                                                        "       			<td colspan=12><b>Mano de Obra Directa</b></td>" +
                                                        "     		</tr>";

                    #region Insumo (ciclo).

                    foreach (DataRow r_rsCargo in rsCargo.Rows)
                    {
                        #region Ciclo inicial de nómina.

                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr align='left' valign='top'>" +
                                                                            "                <td colspan=12 align='left'>" + r_rsCargo["Cargo"].ToString() + "</td>" +
                                                                            "              </tr>";

                        TotalTipo1 = 0;
                        TotalTipo2 = 0;

                        TotalTipo1A = 0;
                        TotalTipo2A = 0;

                        for (int m = 1; m <= filas; m++)
                        {
                            #region Insumo.

                            z = m * 6 - 5;

                            #region Pinta los titulos de los meses.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";

                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    if (k > 12) { }
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='center' colspan=2>Mes " + k + "</td>";
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Pinta los titulos de sueldo/prestaciones.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";

                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='center'>Sueldo</td><td align='center'>Prestaciones</td>";
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Tipos 1 y 2.

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    #region Tipo 1.

                                    txtSQL = " select *  from InterventorNomina a,InterventorNominaMes b " +
                                             " where a.tipo='Insumo' and id_nomina=codcargo and codcargo = " + r_rsCargo["Id_Nomina"].ToString() +
                                             " and codproyecto = " + CodProyecto + " and mes = " + k + " and b.Tipo = 1 order by id_nomina, mes, b.tipo ";

                                    rsCargos = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo1 = "&nbsp;";

                                    if (rsCargos.Rows.Count > 0)
                                    {
                                        Tipo1 = rsCargos.Rows[0]["Valor"].ToString();
                                        TotalTipo1 = TotalTipo1 + Double.Parse(rsCargos.Rows[0]["Valor"].ToString());
                                    }

                                    #endregion

                                    #region Tipo 2.

                                    txtSQL = " select *  from InterventorNomina a,InterventorNominaMes b " +
                                             " where a.tipo='Insumo' and id_nomina=codcargo and codcargo = " + r_rsCargo["Id_Nomina"].ToString() +
                                             " and codproyecto = " + CodProyecto + " and mes = " + k + " and b.Tipo = 2 order by id_nomina, mes, b.tipo ";

                                    rsCargos = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo2 = "&nbsp;";

                                    if (rsCargos.Rows.Count > 0)
                                    {
                                        Tipo2 = rsCargos.Rows[0]["Valor"].ToString();
                                        TotalTipo2 = TotalTipo2 + Double.Parse(rsCargos.Rows[0]["Valor"].ToString());
                                    }

                                    #endregion

                                    if (Tipo1 != "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo1).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo1 + "</td>"; }

                                    if (Tipo2 != "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo2).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo2 + "</td>"; }
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                            #endregion

                            #region Avances de tipo 1 y 2.

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    #region Tipo 1.

                                    txtSQL = " select * " +
                                             " from AvanceCargoPOMes " +
                                             " where CodCargo = " + r_rsCargo["Id_Nomina"].ToString() +
                                             " and Mes = " + k + " and codtipofinanciacion = 1 ";
                                    rsCargos = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo1 = "&nbsp;";

                                    if (rsCargos.Rows.Count > 0)
                                    {
                                        Tipo1 = rsCargos.Rows[0]["Valor"].ToString();
                                        TotalTipo1A = TotalTipo1A + Double.Parse(rsCargos.Rows[0]["Valor"].ToString());
                                    }

                                    #endregion

                                    #region Tipo 2.

                                    txtSQL = " select * " +
                                             " from AvanceCargoPOMes " +
                                             " where CodCargo = " + r_rsCargo["Id_Nomina"].ToString() +
                                             " and Mes = " + k + " and codtipofinanciacion = 2 ";
                                    rsCargos = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo2 = "&nbsp;";

                                    if (rsCargos.Rows.Count > 0)
                                    {
                                        Tipo2 = rsCargos.Rows[0]["Valor"].ToString();
                                        TotalTipo2A = TotalTipo2A + Double.Parse(rsCargos.Rows[0]["Valor"].ToString());
                                    }

                                    #endregion

                                    if (Tipo1 != "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo1).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo1 + "</td>"; }

                                    if (Tipo2 != "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo2).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo2 + "</td>"; }
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                            #endregion

                            #endregion
                        }

                        //Costo Total
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td colspan=2>Costo Total</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalTipo1.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalTipo2.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                        //Costo Total avances reportados
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalTipo1A.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalTipo2A.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                        i = i + 1;
                        //rsCargo.movenext 

                        #endregion
                    }

                    #endregion
                }

                #endregion

                #endregion

                #region Producción.

                lbl_cuerpo.Text = lbl_cuerpo.Text + "</table>" +
                                        "	</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                        "	<td colspan=12 bgcolor='#CCCCCC' class='TitDestacado' align=center><B>PRODUCCIÓN</B></td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td class='TitDestacado' align=center>" +
                                                "<TABLE width='100%' align=center border='1' cellpadding='0' cellspacing='0'>";

                if (CodProyecto != 0)
                {
                    txtSQL = " select * from InterventorProduccion " +
                             " where codproyecto = " + CodProyecto + " order by id_produccion ";
                    rsProducto = consultas.ObtenerDataTable(txtSQL, "text");

                    lbl_cuerpo.Text = lbl_cuerpo.Text + "     		<tr class='Titulo'> " +
                                                       "       			<td colspan=12><b>Producto o Servicio</b></td>" +
                                                       "     		</tr>";
                    i = 0;
                    txtProducto = "";

                    foreach (DataRow r_rsProducto in rsProducto.Rows)
                    {
                        #region Ciclo de producto.

                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr align='left' valign='top'>" +
                                                                           "                <td align='left' colspan=12>" + r_rsProducto["NomProducto"].ToString() + "</td>" +
                                                                           "              </tr>";

                        TotalTipo1 = 0;
                        TotalTipo2 = 0;

                        TotalTipo1A = 0;
                        TotalTipo2A = 0;

                        for (int m = 1; m <= filas; m++)
                        {
                            #region Producción.

                            z = m * 6 - 5;

                            #region Pinta los titulos de los meses.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";

                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    if (k > 12) { }
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='center' colspan=2>Mes " + k + "</td>";
                                }
                            }
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Pinta los titulos de sueldo/prestaciones.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='center'>Cantidad</td><td align='center'>Costo</td>";
                                }
                            }
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr align='left' valign='top'>";

                            #region Datos registrados y aprobados.
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    #region Tipo 1.

                                    txtSQL = " select * from InterventorProduccion,InterventorProduccionMes " +
                                             " where id_produccion=codproducto and codproyecto = " + CodProyecto +
                                             " and mes = " + k + " and tipo = 1 order by id_produccion, mes, tipo";
                                    rsProductos = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo1 = "&nbsp;";
                                    if (rsProductos.Rows.Count > 0)
                                    {
                                        Tipo1 = rsProductos.Rows[0]["valor"].ToString();
                                        TotalTipo1 = TotalTipo1 + Double.Parse(rsProductos.Rows[0]["valor"].ToString());
                                    }

                                    #endregion

                                    #region Tipo 2.

                                    txtSQL = " select * from InterventorProduccion,InterventorProduccionMes " +
                                             " where id_produccion=codproducto and codproyecto = " + CodProyecto +
                                             " and mes = " + k + " and tipo = 2 order by id_produccion, mes, tipo";
                                    rsProductos = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo2 = "&nbsp;";
                                    if (rsProductos.Rows.Count > 0)
                                    {
                                        Tipo2 = rsProductos.Rows[0]["valor"].ToString();
                                        TotalTipo2 = TotalTipo2 + Double.Parse(rsProductos.Rows[0]["valor"].ToString());
                                    }

                                    #endregion

                                    if (Tipo1 == "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo1 + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo1).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }

                                    if (Tipo2 == "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo2 + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo2).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Datos avances reportados.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr align='left' valign='top'>";

                            //for (int k = z; i < m * 6; i++)//mal
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    #region Tipo 1.
                                    txtSQL = " select * " +
                                             " from AvanceProduccionPOMes " +
                                             " where CodProducto=" + r_rsProducto["Id_produccion"].ToString() +
                                             " and mes = " + k + " and codtipofinanciacion = 1 ";
                                    rsTipo1 = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (rsTipo1.Rows.Count > 0)
                                    {
                                        Tipo1 = rsTipo1.Rows[0]["valor"].ToString();
                                        TotalTipo1A = TotalTipo1A + Double.Parse(rsTipo1.Rows[0]["valor"].ToString());
                                    }
                                    else
                                    {
                                        Tipo1 = "&nbsp;";
                                    }
                                    #endregion

                                    #region Tipo 2.
                                    txtSQL = " select * " +
                                             " from AvanceProduccionPOMes " +
                                             " where CodProducto=" + r_rsProducto["Id_produccion"].ToString() +
                                             " and mes = " + k + " and codtipofinanciacion = 2 ";
                                    rsTipo2 = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (rsTipo2.Rows.Count > 0)
                                    {
                                        Tipo2 = rsTipo2.Rows[0]["valor"].ToString();
                                        TotalTipo2A = TotalTipo2A + Double.Parse(rsTipo2.Rows[0]["valor"].ToString());
                                    }
                                    else
                                    {
                                        Tipo2 = "&nbsp;";
                                    }
                                    #endregion

                                    if (Tipo1 == "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo1 + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo1).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }

                                    if (Tipo2 == "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo2 + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo2).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #endregion
                        }

                        #region Líneas finales.

                        //Costo Total
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td colspan=2>Costo Total</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalTipo1.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalTipo2.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                        //Costo Total avances reportados
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalTipo1A.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalTipo2A.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                        i = i + 1;
                        //rsProducto.movenext

                        #endregion

                        #endregion
                    }

                    rsProducto = null;
                }

                #endregion

                #region Ventas.

                lbl_cuerpo.Text = lbl_cuerpo.Text + "</table>" +
                                        "	</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                        "	<td colspan=12 bgcolor='#CCCCCC' class='TitDestacado' align=center><B>VENTAS</B></td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td class='TitDestacado' align=center>" +
                                                "<TABLE width='100%' align=center border='1' cellpadding='0' cellspacing='0'>";

                lbl_cuerpo.Text = lbl_cuerpo.Text + "     		<tr class='Titulo'> " +
                                                   "       			<td colspan=12><b>Producto o Servicio</b></td>" +
                                                   "     		</tr>";

                if (CodProyecto != 0)
                {
                    txtSQL = " select * from InterventorVentas " +
                             " where codproyecto = " + CodProyecto + " order by id_ventas ";
                    rsProducto = consultas.ObtenerDataTable(txtSQL, "text");
                    i = 0;
                    txtProducto = "";

                    foreach (DataRow r_rsProducto in rsProducto.Rows)
                    {
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr align='left' valign='top'>" +
                                                            "                <td align='left' colspan=12>" + r_rsProducto["NomProducto"].ToString() + "</td>" +
                                                            "              </tr>";

                        TotalTipo1 = 0;
                        TotalTipo2 = 0;

                        TotalTipo1A = 0;
                        TotalTipo2A = 0;

                        for (int m = 1; m <= filas; m++)
                        {
                            #region Ventas.

                            z = m * 6 - 5;

                            #region Pinta los titulos de los meses.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";

                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    if (k > 12) { }
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='center' colspan=2>Mes " + k + "</td>";
                                }
                            }
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Pinta los titulos de Ventas/ingresos.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='center'>Ventas</td><td align='center'>Ingreso</td>";
                                }
                            }
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #region Datos registrados y aprobados.

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr align='left' valign='top'>";

                            //Datos registrados y aprobados.
                            //for (int k = z; i < m * 6; i++)//mal
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    #region Tipo 1.

                                    txtSQL = " select valor " +
                                             " from InterventorVentas,InterventorVentasMes " +
                                             " where id_ventas=codproducto and codProducto = " + r_rsProducto["Id_Ventas"].ToString() + " and codproyecto = " + CodProyecto +
                                             " and mes = " + k + " and tipo = 1 order by id_ventas, mes, tipo ";
                                    rsProductos = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo1 = "&nbsp;";
                                    if (rsProductos.Rows.Count > 0)
                                    {
                                        Tipo1 = rsProductos.Rows[0]["valor"].ToString();
                                        TotalTipo1 = TotalTipo1 + Double.Parse(rsProductos.Rows[0]["valor"].ToString());
                                    }

                                    #endregion

                                    #region Tipo 2.

                                    txtSQL = " select valor " +
                                             " from InterventorVentas,InterventorVentasMes " +
                                             " where id_ventas=codproducto and codProducto = " + r_rsProducto["Id_Ventas"].ToString() + " and codproyecto = " + CodProyecto +
                                             " and mes = " + k + " and tipo = 2 order by id_ventas, mes, tipo ";
                                    rsProductos = consultas.ObtenerDataTable(txtSQL, "text");

                                    Tipo2 = "&nbsp;";
                                    if (rsProductos.Rows.Count > 0)
                                    {
                                        Tipo2 = rsProductos.Rows[0]["valor"].ToString();
                                        TotalTipo2 = TotalTipo2 + Double.Parse(rsProductos.Rows[0]["valor"].ToString());
                                    }

                                    #endregion

                                    if (Tipo1 == "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo1 + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo1).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }

                                    if (Tipo2 == "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo2 + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo2).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                            #endregion

                            #region Datos avances reportados.
                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr align='left' valign='top'>";

                            //for (int k = z; i < m * 6; i++)//mal
                            for (int k = z; k <= m * 6; k++)
                            {
                                if (k <= CONST_Meses)
                                {
                                    #region Tipo 1.
                                    txtSQL = " select * " +
                                             " from AvanceVentasPOMes " +
                                             " where CodProducto = " + r_rsProducto["Id_ventas"].ToString() +
                                             " and mes = " + k + " and codtipofinanciacion = 1 ";
                                    rsTipo1 = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (rsTipo1.Rows.Count > 0)
                                    {
                                        Tipo1 = rsTipo1.Rows[0]["valor"].ToString();
                                        TotalTipo1A = TotalTipo1A + Double.Parse(rsTipo1.Rows[0]["valor"].ToString());
                                    }
                                    else
                                    {
                                        Tipo1 = "&nbsp;";
                                    }
                                    #endregion

                                    #region Tipo 2.
                                    txtSQL = " select * " +
                                             " from AvanceVentasPOMes " +
                                             " where CodProducto = " + r_rsProducto["Id_ventas"].ToString() +
                                             " and mes = " + k + " and codtipofinanciacion = 2 ";
                                    rsTipo2 = consultas.ObtenerDataTable(txtSQL, "text");

                                    if (rsTipo2.Rows.Count > 0)
                                    {
                                        Tipo2 = rsTipo2.Rows[0]["valor"].ToString();
                                        TotalTipo2A = TotalTipo2A + Double.Parse(rsTipo2.Rows[0]["valor"].ToString());
                                    }
                                    else
                                    {
                                        Tipo2 = "&nbsp;";
                                    }
                                    #endregion

                                    if (Tipo1 == "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo1 + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo1).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }

                                    if (Tipo2 == "&nbsp;")
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Tipo2 + "</td>"; }
                                    else
                                    { lbl_cuerpo.Text = lbl_cuerpo.Text + "<td align='right'>" + Double.Parse(Tipo2).ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>"; }
                                }
                            }

                            lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                            #endregion

                            #endregion
                        }

                        #region Líneas finales.

                        //Costo Total
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td colspan=2>Costo Total</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalTipo1.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalTipo2.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";
                        //Costo Total avances reportados
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              <tr>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                 <td align='right'>" + TotalTipo1A.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "                <td align='right'>" + TotalTipo2A.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO")) + "</td>";
                        lbl_cuerpo.Text = lbl_cuerpo.Text + "              </tr>";

                        i = i + 1;
                        //rsProducto.movenext

                        #endregion
                    }

                    rsProducto = null;
                }

                #endregion
            }
            catch (Exception ex) { lbl_cuerpo.Text = lbl_cuerpo.Text + "<strong style='color:#CC0000;'>" + ex.Message + "</strong>"; }
        }
    }
}