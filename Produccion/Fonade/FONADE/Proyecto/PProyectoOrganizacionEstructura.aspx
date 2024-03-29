﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PProyectoOrganizacionEstructura.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.PProyectoOrganizacionEstructura" %>

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
        .MsoNormal
        {
            margin: 0cm 0cm 0pt 0cm !important;
            padding: 5px 15px 0px 15px;
        }
        .MsoNormalTable
        {
            margin: 6px 0px 4px 8px !important;
        }
        .editorHTMLDisable span
        {
            margin: 0cm 0cm 0pt 0cm !important;
            padding: 5px 15px 0px 15px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <div>
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
        <table id="tabla_docs" runat="server" visible="false" width="780" border="0" cellspacing="0"
            cellpadding="0">
            <tr>
                <td align="right">
                    <table width="52" border="0" cellspacing="0" cellpadding="0">
                        <tr align="center">
                            <td style="width: 50;">
                                <asp:ImageButton ID="ImageButton1" ImageUrl="../../Images/icoClip.gif" runat="server" ToolTip="Nuevo Documento"
                                    OnClick="ImageButton1_Click" />
                            </td>
                            <td style="width: 138;">
                                <asp:ImageButton ID="ImageButton2" ImageUrl="../../Images/icoClip2.gif" runat="server" ToolTip="Ver Documentos"
                                    OnClick="ImageButton2_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Estructura Organizacional', texto: 'EstructuraOrganizacional'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                        </div>
                        <div>
                            Estructura Organizacional:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_Post_It_2" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It2" runat="server" _txtCampo="EstructuraOrganizacional" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <asp:TextBox ID="txt_estructura" class="editorHTML" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>
        <div id="panel_estructura" class="editorHTMLDisable" runat="server">
        </div>
        <ajaxToolkit:HtmlEditorExtender ID="txt_estructura_HtmlEditorExtender" runat="server"
            TargetControlID="txt_estructura" Enabled="True" EnableSanitization="false">
        </ajaxToolkit:HtmlEditorExtender>
        <br />
        <br />
        <br />
        <asp:Button ID="btm_guardarCambios" runat="server" Text="Guardar" OnClick="btm_guardarCambios_Click"
            Visible="false" />
    </div>
    </form>
</body>
</html>
