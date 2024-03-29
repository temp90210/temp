﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EvaluacionNomina.aspx.cs"
    Inherits="Fonade.FONADE.evaluacion.EvaluacionNomina" EnableEventValidation="true" %>

<%@ Register Src="~/Controles/Post_It.ascx" TagPrefix="uc1" TagName="Post_It" %>
<%--<%@ Register src="../../Controles/CtrlCheckedProyecto.ascx" tagname="CtrlCheckedProyecto" tagprefix="uc2" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function CalcularTotal(nombreCampo, anio) {
            var totalRegistro = 0;

            for (var i = 1; i <= 12; i++) {
                var valor = document.getElementById(nombreCampo + i).value;
                if (isNaN(parseFloat(valor))) {
                    valor = 0
                    document.getElementById(nombreCampo + i).value = "";
                } else {
                    valor = parseFloat(valor)
                }
                totalRegistro = totalRegistro + valor;
            }
            document.getElementById(nombreCampo + "Total").innerHTML = "<b>" + formatCurrency(totalRegistro) + "</b>";
            //Total Columna
            var valorAporte = document.getElementById('txtAporte' + anio).value;
            var valorFondo = document.getElementById('txtFondo' + anio).value;

            valorAporte = isNaN(parseFloat(valorAporte)) ? 0 : parseFloat(valorAporte);
            valorFondo = isNaN(parseFloat(valorFondo)) ? 0 : parseFloat(valorFondo);

            document.getElementById('TotalMes' + anio).innerHTML = "<b>" + formatCurrency(parseFloat(valorAporte) + parseFloat(valorFondo)) + "</b>";

        }

        function formatCurrency(strValue) {
            strValue = strValue.toString().replace(/\$|\,/g, '');
            dblValue = parseFloat(strValue);

            blnSign = (dblValue == (dblValue = Math.abs(dblValue)));
            dblValue = Math.floor(dblValue * 100 + 0.50000000001);
            intCents = dblValue % 100;
            strCents = intCents.toString();
            dblValue = Math.floor(dblValue / 100).toString();
            if (intCents < 10)
                strCents = "0" + strCents;
            for (var i = 0; i < Math.floor((dblValue.length - (1 + i)) / 3); i++)
                dblValue = dblValue.substring(0, dblValue.length - (4 * i + 3)) + ',' +
            dblValue.substring(dblValue.length - (4 * i + 3));
            return (((blnSign) ? '' : '-') + '$' + dblValue + '.' + strCents);
        }

    </script>
    <style type="text/css">
        .auto-style1
        {
            color: #FFFFFF;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel ID="pnlPrincipal" Visible="true" runat="server">
        <table>
            <tbody>
                <tr>
                    <td>
                        ULTIMA ACTUALIZACIÓN:&nbsp;
                    </td>
                    <td>
                        <asp:Label ID="lbl_nombre_user_ult_act" Text="" runat="server" ForeColor="#CC0000" />&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="lbl_fecha_formateada" Text="" runat="server" ForeColor="#CC0000" />
                    </td>
                    <td style="width: 20px;">
                    </td>
                    <td>
                        <asp:CheckBox ID="chk_realizado" Text="MARCAR COMO REALIZADO:&nbsp;&nbsp;&nbsp;&nbsp;"
                            runat="server" TextAlign="Left" />
                        &nbsp;<asp:Button ID="btn_guardar_ultima_actualizacion" Text="Guardar" runat="server"
                            ToolTip="Guardar" OnClick="btn_guardar_ultima_actualizacion_Click" />
                    </td>
                </tr>
            </tbody>
        </table>
        <table width='100%' border='0' align="center" cellspacing='0' cellpadding='0'>
            <%--<tr>
                <td align='left' colspan="2">
                    <uc2:ctrlcheckedproyecto id="CtrlCheckedProyecto1" runat="server" />
                </td>
            </tr>--%>
            <tr>
                <td align='left'>
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Nómina', texto: 'Nomina'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos" />
                        </div>
                        <div>
                            &nbsp; <strong>Nómina:</strong>
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_Post_It1" runat="server">
                        <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="Nomina" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <br />
        <table width='100%' align="Center" border='0' cellpadding='0' cellspacing='0'>
            <tr>
                <td align='left' valign='top' width='98%'>
                    <asp:Panel ID="PanelPersonalCalificado" runat="server" Visible="false">
                        <table width='100%' border='0' cellspacing='1' cellpadding='4'>
                        </table>
                        <table cellpadding="0px" cellspacing="0px">
                            <tr>
                                <td valign="Top">
                                    <div style="width: 300px; overflow: auto; border-right: silver 1px solid">
                                        <table class="Grilla" cellpadding="0" cellspacing="0" width="300px" border="0">
                                            <tr>
                                                <th style="width: 260px; text-align: center">
                                                    &nbsp;<span class="auto-style1"> Personal Calificado</span>
                                                </th>
                                            </tr>
                                        </table>
                                        <asp:GridView ID="gw_Anexos" runat="server" Width="300px" AutoGenerateColumns="false"
                                            CssClass="Grilla" RowStyle-Height="35px" GridLines="None" CellSpacing="1" CellPadding="4">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Puesto de Trabajo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEditar" runat="server" Text='<%# Eval("Cargo") %>' Visible="true" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </td>
                                <td valign="Top">
                                    <div style="width: 560px; overflow: auto">
                                        <table class="Grilla" cellpadding="4" cellspacing="1" width="3380px" border="0">
                                            <tr>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 1
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 2
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 3
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 4
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 5
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 6
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 7
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 8
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 9
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 10
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 11
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 12
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Costo Total
                                                </th>
                                            </tr>
                                        </table>
                                        <asp:GridView ID="gw_AnexosActividad" runat="server" Width="3380px" AutoGenerateColumns="false"
                                            CssClass="Grilla" RowStyle-Height="35px" GridLines="None" CellSpacing="1" CellPadding="4">
                                            <RowStyle HorizontalAlign="Right" />
                                            <Columns>
                                                <asp:BoundField DataField="Sueldo1" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones1" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo2" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones2" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo3" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones3" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo4" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones4" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo5" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones5" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo6" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones6" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo7" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones7" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo8" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones8" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo9" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones9" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo10" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones10" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo11" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones11" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo12" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones12" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="SueldoTotal" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="PrestacionesTotal" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td align='left' valign='top' width='98%'>
                    <asp:Panel ID="PanelManodeObraDirecta" runat="server" Visible="false">
                        <table width='100%' border='0' cellspacing='1' cellpadding='4'>
                        </table>
                        <table cellpadding="0px" cellspacing="0px">
                            <tr>
                                <td valign="Top">
                                    <div style="width: 300px; overflow: auto; border-right: silver 1px solid">
                                        <table class="Grilla" cellpadding="0" cellspacing="0" width="300px" border="0">
                                            <tr>
                                                <th style="width: 260px; text-align: center">
                                                    &nbsp; <span class="auto-style1">Mano de Obra Directa</span>
                                                </th>
                                            </tr>
                                        </table>
                                        <asp:GridView ID="gw_Anexos2" runat="server" Width="300px" AutoGenerateColumns="false"
                                            CssClass="Grilla" RowStyle-Height="35px" GridLines="None" CellSpacing="1" CellPadding="4">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Puesto de Trabajo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEditar1" runat="server" Text='<%# Eval("nomInsumo") %>' Visible="true" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </td>
                                <td valign="Top">
                                    <div style="width: 560px; overflow: auto">
                                        <table class="Grilla" cellpadding="4" cellspacing="1" width="3380px" border="0">
                                            <tr>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 1
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 2
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 3
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 4
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 5
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 6
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 7
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 8
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 9
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 10
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 11
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Mes 12
                                                </th>
                                                <th style="width: 260px; text-align: center">
                                                    Costo Total
                                                </th>
                                            </tr>
                                        </table>
                                        <asp:GridView ID="gw_AnexosActividad2" runat="server" Width="3380px" AutoGenerateColumns="false"
                                            CssClass="Grilla" RowStyle-Height="35px" GridLines="None" CellSpacing="1" CellPadding="4">
                                            <Columns>
                                                <asp:BoundField DataField="Sueldo1" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones1" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo2" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones2" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo3" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones3" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo4" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones4" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo5" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones5" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo6" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones6" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo7" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones7" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo8" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones8" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo9" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones9" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo10" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones10" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo11" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones11" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Sueldo12" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="Prestaciones12" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="SueldoTotal" HeaderText="Sueldo" ItemStyle-Width="130px" />
                                                <asp:BoundField DataField="PrestacionesTotal" HeaderText="Prestaciones" ItemStyle-Width="130px" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    </form>
</body>
</html>
