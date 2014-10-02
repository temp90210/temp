<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PProyectoFinanzasIngreso.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.PProyectoFinanzasIngreso" EnableEventValidation="true" %>

<%@ Register Src="../../Controles/CargarArchivos.ascx" TagName="CargarArchivos" TagPrefix="uc1" %>
<%@ Register Src="../../Controles/Post_It.ascx" TagName="Post_It" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../../Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/ScriptsGenerales.js" type="text/javascript"></script>
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel ID="pnlPrincipal" Visible="true" runat="server">
        <table>
            <tbody>
                <tr>
                    <td width="19">
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
        <br />
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td width="19">
                    &nbsp;
                </td>
                <td>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 50%">
                                <span class="help_container"><span onclick="textoAyuda({titulo: 'Fuentes de Financiación', texto: 'FuentesFinanciacion'});">
                                    <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                                </span></span><b>Fuentes de Financiaci&oacute;n:&nbsp;&nbsp;&nbsp;&nbsp;</b>
                            </td>
                            <td>
                                <div id="div_post_it_1" runat="server" visible="false">
                                    <uc1:Post_It ID="Post_It2" runat="server" _txtCampo="FuentesFinanciacion" _txtTab="1" />
                                </div>
                            </td>
                        </tr>
                    </table>
                    <br />
                    Recursos solicitados al fondo emprender en (smlv)
                    <asp:TextBox ID="txtRecursosSolicitados" runat="server" MaxLengt="3" Width="30px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" CssClass="failureNotification"
                        ControlToValidate="txtRecursosSolicitados" ErrorMessage="*" ValidationExpression="[0-9]*"
                        ValidationGroup="validaGuardar" Display="Dynamic">0 a 9 con punto (.) decimal</asp:RegularExpressionValidator>
                    <asp:Label ID="lblErrorRecursos" runat="server" Text="" CssClass="failureNotification"></asp:Label>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass='Boton' OnClick="btnGuardar_Click"
                        ValidationGroup="validaGuardar" Visible="False" />
                    <br />
                    <table width="100%">
                        <tr>
                            <td align='left' valign='top' width='98%'>
                                <table width='100%' border='0' cellspacing='1' cellpadding='4'>
                                    <tr>
                                        <td align='left'>
                                            <asp:Panel ID="pnlBotonAdicionarAporte" runat="server" Visible="false">
                                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/icoAdicionarUsuario.gif"
                                                    Visible="False" />
                                                <asp:Button ID="btnAdicionarAporte" runat="server" Text="Adicionar Aporte" CssClass='boton_Link'
                                                    BorderStyle="None" OnClick="btnAdicionarAporte_Click" Visible="false" />
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr style="background-color: #00468F;">
                                        <td align="left" style="text-align: center; color: White;">
                                            Aporte de los Emprendedores
                                        </td>
                                    </tr>
                                </table>
                                <asp:GridView ID="gw_AporteEmprendedores" runat="server" Width="100%" AutoGenerateColumns="False"
                                    CssClass="Grilla" OnRowCommand="gw_AporteEmprendedores_RowCommand" DataKeyNames="Id_Aporte"
                                    OnRowDataBound="gw_AporteEmprendedores_RowDataBound">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <Columns>
                                        <asp:TemplateField ItemStyle-Width="3%" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="btn_BorrarEmprendedor" CommandName="Borrar" CommandArgument='<%# Eval("Id_Aporte") %>'
                                                    runat="server" ImageUrl="/Images/icoBorrar.gif" Visible="true" OnClientClick="return Confirmacion('Esta seguro que desea borrar el aporte seleccionado?')" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="nombre" HeaderText="Nombre" ItemStyle-Width="47%" ItemStyle-HorizontalAlign="Left" />
                                        <asp:BoundField DataField="valor" HeaderText="Valor" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Right" />
                                        <asp:BoundField DataField="detalle" HeaderText="Detalle" ItemStyle-Width="35%" ItemStyle-HorizontalAlign="Right" />
                                    </Columns>
                                </asp:GridView>
                                <br />
                                <table width='100%' border='0' cellspacing='1' cellpadding='4'>
                                    <tr>
                                        <td align='left'>
                                            <asp:Panel ID="pnlBotonAdicionarRecurso" runat="server" Visible="false">
                                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/icoAdicionarUsuario.gif" />
                                                <asp:Button ID="btnAdicionarRecurso" runat="server" Text="Adicionar Recurso" CssClass='boton_Link'
                                                    BorderStyle="None" OnClick="btnAdicionarRecurso_Click" />
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr style="background-color: #00468F;">
                                        <td align="left" style="text-align: center; color: White;">
                                            Recursos de Capital
                                        </td>
                                    </tr>
                                </table>
                                <asp:GridView ID="gw_RecursosCapital" runat="server" Width="100%" AutoGenerateColumns="False"
                                    CssClass="Grilla" OnRowCommand="gw_RecursosCapital_RowCommand" DataKeyNames="Id_Recurso"
                                    OnRowDataBound="gw_RecursosCapital_RowDataBound">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <Columns>
                                        <asp:TemplateField ItemStyle-Width="3%">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="btn_BorrarRecursosCapital" CommandName="Borrar" CommandArgument='<%# Eval("Id_Recurso") %>'
                                                    runat="server" ImageUrl="/Images/icoBorrar.gif" Visible="true" OnClientClick="return Confirmacion('Esta seguro que desea borrar el Recurso de Capital seleccionado?')" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="cuantia" HeaderText="Cuantía" ItemStyle-Width="10%" />
                                        <asp:BoundField DataField="plazo" HeaderText="Plazo" ItemStyle-Width="18%" />
                                        <asp:BoundField DataField="formaPago" HeaderText="Forma de Pago" ItemStyle-Width="18%" />
                                        <asp:BoundField DataField="intereses" HeaderText="Interes (Nominal/Anual)" ItemStyle-Width="10%" />
                                        <asp:BoundField DataField="destinacion" HeaderText="Destinación" ItemStyle-Width="31%" />
                                    </Columns>
                                </asp:GridView>
                                <br />
                                <table border="0" cellpadding="4" cellspacing="1" width="100%" style="background-color: #00468F;">
                                    <tr>
                                        <td align="left" style="text-align: center; color: White;">
                                            Proyeccion de Ingresos por Ventas
                                        </td>
                                    </tr>
                                </table>
                                <asp:GridView ID="gw_IngresosVentas" runat="server" CssClass="Grilla" Width="100%"
                                    OnSelectedIndexChanged="gw_IngresosVentas_SelectedIndexChanged" OnRowDataBound="gw_IngresosVentas_RowDataBound"
                                    CellSpacing="1" GridLines="None">
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table width='95%' border='0' align="center" cellspacing='0' cellpadding='0'>
                        <tr>
                            <td width='18' align='left'>
                                <span class="help_container"><span onclick="textoAyuda({titulo: 'Modelo Financiero', texto: 'FormatosFinancieros'});">
                                    <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos" />
                                </span></span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
                            </td>
                            <td width='350'>
                                <b>Modelo Financiero&nbsp;&nbsp;&nbsp;&nbsp;</b>
                            </td>
                            <td align="left">
                                <div id="div_post_it_2" runat="server" visible="false">
                                    <uc1:Post_It ID="Post_It1" runat="server" _txtCampo="FormatosFinancieros" _txtTab="1" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <table width='95%' border='0' align="center" cellspacing='0' cellpadding='0'>
            <tr>
                <td>
                    <asp:Button ID="btnLinkBajarModeloFinanciero" runat="server" Text="Bajar Modelo Financiero"
                        OnClick="btnLinkBajarModeloFinanciero_Click" CssClass="boton_Link" Visible="false" />
                </td>
                <td>
                    <asp:Button ID="btnLinkSubirModeloFinanciero" runat="server" Text="Subir Modelo Financiero"
                        OnClick="btnLinkSubirModeloFinanciero_Click" CssClass="boton_Link" Visible="false" />
                </td>
                <td>
                    <%--<asp:Button ID="btnLinkVerModeloFinanciero" runat="server" Text="Ver Modelo Financiero"
                        OnClick="btnLinkVerModeloFinanciero_Click" CssClass="boton_Link" Enabled="false" />--%>
                    <asp:HyperLink ID="HyperLink1" Text="Ver Modelo Financiero" runat="server" Width="264px"
                        Target="_blank" NavigateUrl="~/FONADE/Plantillas/modelofinanciero.xls" Style="text-decoration: none;" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnLinkGuiaLlenarModeloFinanciero" runat="server" Text="Guía para llenar Modelo Financiero"
                        OnClick="btnLinkGuiaLlenarModeloFinanciero_Click" CssClass="boton_Link" Visible="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!--  Nuevo Panel -->
    <asp:Panel ID="pnlAporte" Visible="false" runat="server">
        <table width='600' border='0' cellspacing='0' cellpadding='2'>
            <tr>
                <td align='center' valign='baseline'>
                    NUEVO APORTE
                </td>
            </tr>
        </table>
        <table width='1000' border='0' cellspacing='0' cellpadding='3'>
            <tr valign="top">
                <td align="Right" width='167'>
                    <b>Nombre:</b>
                </td>
                <td>
                    <asp:TextBox ID="txtNombreAporte" runat="server" MaxLength="100" Width="300px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNombreAporte"
                        ErrorMessage="El nombre es requerido" ValidationGroup="ValidacionNuevoAporte">El nombre es requerido</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr valign="top">
                <td align="Right">
                    <b>Valor:</b>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtValorAporte" runat="server" MaxLength="20" Width="100px" Text="0"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtValorAporte"
                        ErrorMessage="El valor debe ser numérico" ValidationExpression="[0-9]*" ValidationGroup="ValidacionNuevoAporte">El valor debe ser numérico</asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr valign="top">
                <td align="Right">
                    <b>Tipo de Aporte:</b>
                </td>
                <td align="left">
                    <asp:DropDownList ID="ddlTipoAporte" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr valign="top">
                <td align="Right">
                    <b>Detalle:</b>
                </td>
                <td>
                    <asp:TextBox ID="txtDetalleAporte" runat="server" MaxLength="20" Width="410px" TextMode="MultiLine"
                        Height="100px"></asp:TextBox>
                </td>
            </tr>
            <tr valign="top">
                <td colspan="2" align="center">
                    <asp:Button ID="btnCrearAporte" runat="server" Text="Crear" OnClick="btnCrearAporte_Click"
                        ValidationGroup="ValidacionNuevoAporte" />
                    <asp:Button ID="btnCancelarAporte" runat="server" Text="Cerrar" OnClick="btnCancelarAporte_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- Nuevo Panel -->
    <asp:Panel ID="pnlRecurso" runat="server" Visible="false">
        <table width='600' border='0' cellspacing='0' cellpadding='2'>
            <tr>
                <td align='center' valign='baseline'>
                    NUEVO RECURSO
                </td>
            </tr>
        </table>
        <table width='1000' border='0' cellspacing='0' cellpadding='3'>
            <tr valign="top">
                <td width='167' align="Right">
                    <b>Tipo:</b>
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlTipoRecurso">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr valign="top">
                <td align="Right">
                    <b>Cuantía:</b>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtCuantiaRecurso" runat="server" MaxLength="20" Width="100" Text="0" />
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtCuantiaRecurso"
                        ErrorMessage="* El valor debe ser numérico" ValidationGroup="ValidaNuevoRecurso"
                        ValidationExpression="\d+" />
                </td>
            </tr>
            <tr valign="top">
                <td align="Right">
                    <b>Plazo:</b>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtPlazoRecurso" runat="server" MaxLength="30" Width="220"></asp:TextBox>
                </td>
            </tr>
            <tr valign="top">
                <td align="Right">
                    <b>Forma de Pago:</b>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtFormaPagoRecurso" runat="server" MaxLength="50" Width="240px"
                        Height="22px"></asp:TextBox>
                </td>
            </tr>
            <tr valign="top">
                <td align="Right">
                    <b>Interes (Nominal/Anual):</b>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtInteresRecurso" runat="server" MaxLength="5" Width="40" Text="0" />
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtInteresRecurso"
                        ErrorMessage="* El valor debe ser numérico" ValidationGroup="ValidaNuevoRecurso"
                        ValidationExpression="\d+" />
                </td>
            </tr>
            <tr valign="top">
                <td align="Right">
                    <b>Destinación:</b>
                </td>
                <td>
                    <asp:TextBox ID="txtDestinacionRecurso" runat="server" Width="410" Height="100" TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
            <tr valign="top">
                <td colspan="2" align="center">
                    <asp:Button ID="btnCrearRecurso" runat="server" Text="Crear" OnClick="btnCrearRecurso_Click"
                        ValidationGroup="ValidaNuevoRecurso" />
                    <asp:Button ID="btnCancelarRecurso" runat="server" Text="Cerrar" OnClick="btnCancelarRecurso_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlCargueDocumento" runat="server" Visible="false">
        <uc1:CargarArchivos ID="CargarArchivos1" runat="server" />
    </asp:Panel>
    <br />
    <br />
    <br />
    <br />
    </form>
</body>
</html>
