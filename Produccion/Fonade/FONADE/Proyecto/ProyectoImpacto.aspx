<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProyectoImpacto.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.ProyectoImpacto" %>

<%@ Register Src="../../Controles/Post_It.ascx" TagName="Post_It" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <style type="text/css">
        .style10
        {
            height: 10px;
        }
        .style11
        {
            height: 188px;
        }
        .style12
        {
            height: 53px;
        }
        .MsoNormal
        {
            margin: 0cm 0cm 0pt 0cm !important;
            padding: 5px 15px 0px 15px;
        }
        .MsoNormalTable
        {
            margin: 6px 0px 4px 8px !important;
        }
    </style>
</head>
<body bgcolor="#ffffff">
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <asp:UpdatePanel ID="PanelImpacto" runat="server" Width="100%" UpdateMode="Conditional">
        <ContentTemplate>
            <table>
                <tbody>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            ULTIMA ACTUALIZACIÓN:&nbsp;
                        </td>
                        <td>
                            <asp:Label ID="lbl_nombre_user_ult_act" Text="" runat="server" ForeColor="#CC0000" />&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="lbl_fecha_formateada" Text="" runat="server" ForeColor="#CC0000" />
                        </td>
                        <td style="width: 100px;">
                        </td>
                        <td>
                            <asp:CheckBox ID="chk_realizado" Text="MARCAR COMO REALIZADO:&nbsp;&nbsp;&nbsp;&nbsp;"
                                runat="server" TextAlign="Left" />
                            &nbsp;<asp:Button ID="btn_guardar_ultima_actualizacion" Text="Guardar" runat="server"
                                ToolTip="Guardar" OnClick="btn_guardar_ultima_actualizacion_Click" Visible="false" />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br />
            <table width="100%" border="0" bgcolor="White">
                <tr>
                    <td>
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 50%">
                                    <div class="help_container">
                                        <div onclick="textoAyuda({titulo: 'Impacto Económico, Regional, Social, Ambiental', texto: 'Impacto'});">
                                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                                        </div>
                                        <div>
                                            Impacto Económico, Regional, Social, Ambiental:
                                        </div>
                                    </div>
                                </td>
                                <td id="td_postIt" runat="server" visible="false">
                                    <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="Impacto" _txtTab="1" />
                                </td>
                            </tr>
                        </table>
                </tr>
                <tr id="tr_data_impacto" runat="server">
                    <td class="style11">
                        <asp:Panel ID="PanelEditar" runat="server" Enabled="true" Width="100%" Visible="true">
                            <asp:TextBox ID="txt_ImpactoEconomico" runat="server" Enabled="true" Width="98%"
                                TextMode="MultiLine" Height="184px" />
                            <br />
                            <ajaxToolkit:HtmlEditorExtender ID="txt_Impacto_HtmlEditorExtender" runat="server"
                                EnableSanitization="false" TargetControlID="txt_ImpactoEconomico" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" BackColor="White"
                                ControlToValidate="txt_ImpactoEconomico" ErrorMessage=" * Este campo está vacío"
                                Display="Dynamic" Style="font-size: small; color: #FF0000;" />
                        </asp:Panel>
                    </td>
                </tr>
                <tr id="tr_div_data" runat="server" visible="false">
                    <td>
                        <div id="div_data" runat="server" visible="false" style="border-color: Black; border-width: 2px;
                            border-style: solid; overflow: scroll; width: 98%; height: 184px; overflow-x: hidden;">
                        </div>
                    </td>
                </tr>
                <tr id="tr_numEmpleados_Evaluacion" runat="server" visible="false">
                    <td>
                        <strong>Número de empleos detectados en evaluación:</strong> &nbsp;
                        <asp:TextBox ID="empleosgenerados" runat="server" Width="68px" Height="22px" Text="20" />
                    </td>
                </tr>
                <tr>
                    <td class="style10">
                        <asp:Panel ID="PanelGuardar" runat="server" Enabled="true" Width="95%" Visible="false"
                            Height="55px" HorizontalAlign="Center">
                            <table width="100%" style="height: 38px">
                                <tr>
                                    <td class="style12">
                                        <asp:Button ID="btn_guardar" runat="server" OnClick="btn_guardar_Click" Text="Guardar"
                                            CausesValidation="true" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
