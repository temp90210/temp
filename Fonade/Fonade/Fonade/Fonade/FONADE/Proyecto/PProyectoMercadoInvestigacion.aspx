<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PProyectoMercadoInvestigacion.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.PProyectoMercadoInvestigacion" %>

<%@ Register Src="../../Controles/Post_It.ascx" TagName="Post_It" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="overflow-x: hidden;">
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
        .editorHTMLDisable p
        {
            margin: 0cm 0cm 0pt 0cm !important;
            padding: 5px 15px 0px 15px;
        }
        .parentContainer
        {
            width: 100%;
            height: 650px;
            overflow-x: hidden;
            overflow-y: visible;
        }
        .childContainer
        {
            width: 100%;
            height: auto;
        }
        html, body, div, iframe
        {
            /*height: 13% !important;*/
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <div class="parentContainer">
        <div class="childContainer">
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
                                    <asp:ImageButton ID="ImageButton1" ImageUrl="../../Images/icoClip.gif" runat="server"
                                        ToolTip="Nuevo Documento" OnClick="ImageButton1_Click" />
                                </td>
                                <td style="width: 138;">
                                    <asp:ImageButton ID="ImageButton2" ImageUrl="../../Images/icoClip2.gif" runat="server"
                                        ToolTip="Ver Documentos" OnClick="ImageButton2_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div class="childContainer">
            <table style="width: 100%">
                <tr>
                    <td style="width: 50%">
                        <div class="help_container">
                            <div onclick="textoAyuda({titulo: 'Definición de Objetivos', texto: 'Objetivos'});">
                                <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos" />
                            </div>
                            <div>
                                Definición de Objetivos:
                            </div>
                        </div>
                    </td>
                    <td>
                        <div id="div_post_it_1" runat="server" visible="false">
                            <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="Objetivos" _txtTab="1" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="childContainer">
            <asp:TextBox ID="txt_objetivos" class="editorHTML" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>
            <div id="panel_objetivos" class="editorHTMLDisable" runat="server">
            </div>
            <ajaxToolkit:HtmlEditorExtender ID="txt_objetivos_HtmlEditorExtender" runat="server"
                TargetControlID="txt_objetivos" Enabled="True">
            </ajaxToolkit:HtmlEditorExtender>
        </div>
        <div class="childContainer">
            <table style="width: 100%">
                <tr>
                    <td style="width: 50%">
                        <div class="help_container">
                            <div onclick="textoAyuda({titulo: 'Justificación y Antecedentes del Proyecto', texto: 'Justificacion'});">
                                <img src="../../Images/imgAyuda.gif" border="0" alt="help_Justificacion">
                            </div>
                            <div>
                                Justificación y Antecedentes del Proyecto:
                            </div>
                        </div>
                    </td>
                    <td>
                        <div id="div_post_it_2" runat="server" visible="false">
                            <uc1:Post_It ID="Post_It2" runat="server" _txtCampo="Justificacion" _txtTab="1" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="childContainer">
            <div id="panel_justificacion" class="editorHTMLDisable" runat="server">
            </div>
            <asp:TextBox ID="txt_justificacion" runat="server" Width="100%" TextMode="MultiLine"
                CssClass="editorHTML"></asp:TextBox>
            <ajaxToolkit:HtmlEditorExtender ID="txt_justificacion_HtmlEditorExtender" runat="server"
                TargetControlID="txt_justificacion" Enabled="True">
            </ajaxToolkit:HtmlEditorExtender>
        </div>
        <div class="childContainer">
            <table style="width: 100%">
                <tr>
                    <td style="width: 50%">
                        <div class="help_container">
                            <div onclick="textoAyuda({titulo: 'Análisis del Sector', texto: 'AnalisisSector'});">
                                <img src="../../Images/imgAyuda.gif" border="0" alt="help_AnalisisSector">
                            </div>
                            <div>
                                Análisis del Sector:
                            </div>
                        </div>
                    </td>
                    <td>
                        <div id="div_post_it_3" runat="server" visible="false">
                            <uc1:Post_It ID="Post_It3" runat="server" _txtCampo="AnalisisSector" _txtTab="1" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="childContainer">
            <div id="panel_analisiss" class="editorHTMLDisable" runat="server">
            </div>
            <asp:TextBox ID="txt_analisisS" runat="server" Width="100%" TextMode="MultiLine"
                CssClass="editorHTML"></asp:TextBox>
            <ajaxToolkit:HtmlEditorExtender ID="txt_analisisS_HtmlEditorExtender" runat="server"
                TargetControlID="txt_analisisS" Enabled="True">
            </ajaxToolkit:HtmlEditorExtender>
        </div>
        <div class="childContainer">
            <table style="width: 100%">
                <tr>
                    <td style="width: 50%">
                        <div class="help_container">
                            <div onclick="textoAyuda({titulo: 'Análisis del Mercado', texto: 'AnalisisMercado'});">
                                <img src="../../Images/imgAyuda.gif" border="0" alt="help_AnalisisMercado">
                            </div>
                            <div>
                                Análisis del Mercado:
                            </div>
                        </div>
                    </td>
                    <td>
                        <div id="div_post_it_4" runat="server" visible="false">
                            <uc1:Post_It ID="Post_It4" runat="server" _txtCampo="AnalisisMercado" _txtTab="1" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="childContainer">
            <div id="panel_analisism" class="editorHTMLDisable" runat="server">
            </div>
            <asp:TextBox ID="txt_analisisM" runat="server" Width="100%" TextMode="MultiLine"
                CssClass="editorHTML"></asp:TextBox>
            <ajaxToolkit:HtmlEditorExtender ID="txt_analisisM_HtmlEditorExtender" runat="server"
                TargetControlID="txt_analisisM" Enabled="True">
            </ajaxToolkit:HtmlEditorExtender>
        </div>
        <div class="childContainer">
            <table style="width: 100%">
                <tr>
                    <td style="width: 50%">
                        <div class="help_container">
                            <div onclick="textoAyuda({titulo: 'Análisis de la Competencia', texto: 'AnalisisCompetencia'});">
                                <img src="../../Images/imgAyuda.gif" border="0" alt="help_AnalisisCompetencia" />
                            </div>
                            <div>
                                Análisis de la Competencia:
                            </div>
                        </div>
                    </td>
                    <td>
                        <div id="div_post_it_5" runat="server" visible="false">
                            <uc1:Post_It ID="Post_It5" runat="server" _txtCampo="AnalisisCompetencia" _txtTab="1" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="childContainer">
            <div id="panel_analisisc" class="editorHTMLDisable" runat="server">
            </div>
            <asp:TextBox ID="txt_analisisC" runat="server" Width="100%" TextMode="MultiLine"
                CssClass="editorHTML"></asp:TextBox>
            <ajaxToolkit:HtmlEditorExtender ID="txt_analisisC_HtmlEditorExtender" runat="server"
                TargetControlID="txt_analisisC" Enabled="True">
            </ajaxToolkit:HtmlEditorExtender>
        </div>
        <div class="childContainer">
            <asp:Button ID="btn_limpiarCampos" runat="server" Text="Limpiar Campos" OnClick="btn_limpiarCampos_Click"
                Visible="false" />
            <asp:Button ID="btm_guardarCambios" runat="server" Text="Guardar" OnClick="btm_guardarCambios_Click"
                Visible="false" />
        </div>
    </div>
    </form>
</body>
</html>
