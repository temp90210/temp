<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditoriaReportes.aspx.cs"
    Inherits="Fonade.FONADE.Auditoria.AuditoriaReportes" MasterPageFile="~/Master.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="panelReportes" runat="server" Visible="true" Width="100%" UpdateMode="Conditional">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td>
                        <h1>
                            <asp:Label runat="server" ID="lbl_Titulo" Style="font-weight: 700" /></h1>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td align="center" class="style17">
                        * seleccione un rango de fechas y una de las tablas en la lista para generar el
                        reporte de Auditoría.
                    </td>
                    <td class="style18">
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td class="style16">
                        &nbsp;
                    </td>
                    <td class="style11" align="right" valign="baseline">
                        <strong>Fecha inicial: </strong>
                    </td>
                    <td class="style15" valign="baseline">
                        <asp:TextBox runat="server" ID="txt_fechainicio" Text="" Enabled="False" BackColor="White"></asp:TextBox>&nbsp;
                        <asp:Image runat="server" ID="btnDateInicio" AlternateText="cal2" ImageUrl="/images/icoModificar.gif"
                            ImageAlign="AbsBottom" />
                        <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txt_fechainicio" Format="dd/MM/yyyy"
                            runat="server" PopupButtonID="btnDateInicio" />
                    </td>
                    <td class="style11" align="right" valign="baseline">
                        <strong>Fecha final: </strong>
                    </td>
                    <td class="style15" valign="baseline">
                        <asp:TextBox runat="server" ID="txt_fichaFin" Text="" Enabled="False" BackColor="White"></asp:TextBox>&nbsp;
                        <asp:Image runat="server" ID="btnDatefin" AlternateText="cal3" ImageUrl="/images/icoModificar.gif"
                            ImageAlign="AbsBottom" />
                        <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="txt_fichaFin" Format="dd/MM/yyyy"
                            runat="server" PopupButtonID="btnDatefin" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td class="style16">
                        &nbsp;
                    </td>
                    <td class="style21" align="center">
                        <asp:ListBox ID="lbx_tablas" runat="server" Height="200px" Width="260px" BackColor="White"
                            Font-Bold="False" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="style22">
                    </td>
                    <td class="style23" align="center">
                        <asp:Button ID="btn_generareporte" runat="server" Text="Generar Reporte" OnClick="btn_generareporte_Click" />
                    </td>
                    <td class="style24">
                    </td>
                </tr>
                <tr>
                    <td class="style22">
                    </td>
                    <td class="style23" align="center">
                        &nbsp;<asp:Button ID="btn_descargar" runat="server" Text="Descargar Reporte" CssClass="boton_Link_Grid"
                            OnClick="btn_descargar_Click" Visible="False" />
                    </td>
                    <td class="style24">
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .style11
        {
            width: 90px;
        }
        .style15
        {
            width: 150px;
        }
        .style16
        {
            width: 30px;
        }
        .style17
        {
            width: 580px;
            height: 30px;
        }
        .style18
        {
            height: 30px;
        }
        .style21
        {
            width: 492px;
        }
        .style22
        {
            width: 30px;
            height: 53px;
        }
        .style23
        {
            width: 492px;
            height: 53px;
        }
        .style24
        {
            height: 53px;
        }
    </style>
</asp:Content>
