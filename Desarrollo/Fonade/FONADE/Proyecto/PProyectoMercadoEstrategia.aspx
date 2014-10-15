<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PProyectoMercadoEstrategia.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.PProyectoMercadoEstrategia" %>

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
            padding: 5px 15px 0px 15px !important;
        }
        .MsoNormalTable
        {
            margin: 6px 0px 4px 8px !important;
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
    <%--<div>
        <%= obtenerUltimaActualizacion(txtTab, codProyecto) %>
    </div>--%>
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
    <div>
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Concepto del Producto o Servicio', texto: 'ConceptoProducto'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_ConceptoProducto">
                        </div>
                        <div>
                            Concepto del Producto o Servicio:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_1" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="ConceptoProducto" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_conceptoP" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="txt_conceptoP" class="editorHTML" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>
        <ajaxToolkit:HtmlEditorExtender ID="txt_conceptoP_HtmlEditorExtender" runat="server"
            TargetControlID="txt_conceptoP" Enabled="True">
        </ajaxToolkit:HtmlEditorExtender>
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Estrategías de Distribución', texto: 'EstrategiasDistribucion'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasDistribucion">
                        </div>
                        <div>
                            Estrategías de Distribución:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_2" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It2" runat="server" _txtCampo="EstrategiasDistribucion" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_estrategiaDist" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="txt_estrategiaDist" runat="server" Width="100%" TextMode="MultiLine"
            CssClass="editorHTML"></asp:TextBox>
        <ajaxToolkit:HtmlEditorExtender ID="txt_estrategiaDist_HtmlEditorExtender" runat="server"
            TargetControlID="txt_estrategiaDist">
        </ajaxToolkit:HtmlEditorExtender>
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Estrategías de Precio', texto: 'EstrategiasPrecio'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasPrecio">
                        </div>
                        <div>
                            Estrategías de Precio:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_3" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It3" runat="server" _txtCampo="EstrategiasPrecio" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_estrategiaPrecio" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="txt_estrategiaPrecio" runat="server" Width="100%" TextMode="MultiLine"
            CssClass="editorHTML"></asp:TextBox>
        <ajaxToolkit:HtmlEditorExtender ID="txt_estrategiaPrecio_HtmlEditorExtender" runat="server"
            TargetControlID="txt_estrategiaPrecio">
        </ajaxToolkit:HtmlEditorExtender>
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Estrategías de Promoción', texto: 'EstrategiasPromocion'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasPromocion">
                        </div>
                        <div>
                            Estrategías de Promoción:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_4" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It4" runat="server" _txtCampo="EstrategiasPromocion" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_estrategiaPromo" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="txt_estrategiaPromo" runat="server" Width="100%" TextMode="MultiLine"
            CssClass="editorHTML"></asp:TextBox>
        <ajaxToolkit:HtmlEditorExtender ID="txt_estrategiaPromo_HtmlEditorExtender" runat="server"
            TargetControlID="txt_estrategiaPromo">
        </ajaxToolkit:HtmlEditorExtender>
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Estrategías de Comunicación', texto: 'EstrategiasComunicacion'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasComunicacion">
                        </div>
                        <div>
                            Estrategías de Comunicación:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_5" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It5" runat="server" _txtCampo="EstrategiasComunicacion" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_estrategiaCom" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="txt_estrategiaCom" runat="server" Width="100%" TextMode="MultiLine"
            CssClass="editorHTML"></asp:TextBox>
        <ajaxToolkit:HtmlEditorExtender ID="txt_estrategiaCom_HtmlEditorExtender" runat="server"
            TargetControlID="txt_estrategiaCom">
        </ajaxToolkit:HtmlEditorExtender>
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Estrategías de Servicio', texto: 'EstrategiasServicio'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasServicio">
                        </div>
                        <div>
                            Estrategías de Servicio:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_6" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It6" runat="server" _txtCampo="EstrategiasServicio" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_estrategiaServ" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="txt_estrategiaServ" runat="server" Width="100%" TextMode="MultiLine"
            CssClass="editorHTML"></asp:TextBox>
        <ajaxToolkit:HtmlEditorExtender ID="txt_estrategiaServ_HtmlEditorExtender" runat="server"
            TargetControlID="txt_estrategiaServ">
        </ajaxToolkit:HtmlEditorExtender>
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Presupuesto de la Mezcla de Mercadeo', texto: 'PresupuestoMercado'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_PresupuestoMercado">
                        </div>
                        <div>
                            Presupuesto de la Mezcla de Mercadeo:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_7" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It7" runat="server" _txtCampo="PresupuestoMercado" _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_presupuestoM" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="txt_presupuestoM" runat="server" Width="100%" TextMode="MultiLine"
            CssClass="editorHTML"></asp:TextBox>
        <ajaxToolkit:HtmlEditorExtender ID="txt_presupuestoM_HtmlEditorExtender" runat="server"
            TargetControlID="txt_presupuestoM">
        </ajaxToolkit:HtmlEditorExtender>
        <table style="width: 100%">
            <tr>
                <td style="width: 50%">
                    <div class="help_container">
                        <div onclick="textoAyuda({titulo: 'Estrategias de Aprovisionamiento', texto: 'EstrategiasAprovisionamiento'});">
                            <img src="../../Images/imgAyuda.gif" border="0" alt="help_EstrategiasAprovisionamiento">
                        </div>
                        <div>
                            Estrategias de Aprovisionamiento:
                        </div>
                    </div>
                </td>
                <td>
                    <div id="div_post_it_8" runat="server" visible="false">
                        <uc1:Post_It ID="Post_It8" runat="server" _txtCampo="EstrategiasAprovisionamiento"
                            _txtTab="1" />
                    </div>
                </td>
            </tr>
        </table>
        <div id="panel_estrategiaApr" class="editorHTMLDisable" runat="server">
        </div>
        <asp:TextBox ID="txt_estrategiaApr" runat="server" Width="100%" TextMode="MultiLine"
            CssClass="editorHTML"></asp:TextBox>
        <ajaxToolkit:HtmlEditorExtender ID="txt_estrategiaApr_HtmlEditorExtender" runat="server"
            TargetControlID="txt_estrategiaApr">
        </ajaxToolkit:HtmlEditorExtender>
        <asp:Button ID="btn_limpiarCampos" runat="server" Text="Limpiar Campos" OnClick="btn_limpiarCampos_Click" />
        <asp:Button ID="btm_guardarCambios" runat="server" Text="Guardar" OnClick="btm_guardarCambios_Click" />
    </div>
    </form>
</body>
</html>
