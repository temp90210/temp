<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/Emergente.Master"
    AutoEventWireup="true" CodeBehind="CatalogoIndicador.aspx.cs" Inherits="Fonade.FONADE.evaluacion.CatalogoIndicador" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <table width="565" border="0" align="center" cellspacing="0" cellpadding="0">
        <tr>
            <td width="100%" align="center" valign="top">
                <table width='95%' border='1' cellpadding='0' cellspacing='0' bordercolor='#4E77AF'>
                    <tr>
                        <td align='center' valign='top' width='98%'>
                            <table width='98%' border='0' cellspacing='0' cellpadding='0'>
                                <tr>
                                    <td>
                                        <h2>
                                            <asp:Label runat="server" ID="lbltitle" />
                                        </h2>
                                    </td>
                                    <td align="right">
                                        <asp:Label runat="server" ID="lbltitle0" Style="font-weight: 700" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                            Font-Bold="True" Font-Size="Small" ForeColor="Red" HeaderText="Llene los Siguientes Campos"
                                            ValidationGroup="crear" />
                                    </td>
                                </tr>
                            </table>
                            <table width='98%' border='0' cellspacing='0' cellpadding='3'>
                                <tr valign="top">
                                    <td align="right">
                                        <b>Detalle:</b>
                                    </td>
                                    <td align="right">
                                        &nbsp;
                                    </td>
                                    <td colspan="3">
                                        <asp:TextBox name='Detalle' Rows='6' cols='50' runat="server" ID="txt_detalle" TextMode="MultiLine"
                                            Width="408px" MaxLength="255" />
                                    </td>
                                    <td align="left">
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic"
                                            ErrorMessage="Detalle" ForeColor="Red" ValidationGroup="crear" ControlToValidate="txt_detalle">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr valign="top">
                                    <td align="right">
                                        <b>Valor:</b>
                                    </td>
                                    <td align="right">
                                        &nbsp;
                                    </td>
                                    <td width='167' align="left" colspan="3" class="TitDestacado">
                                        <asp:TextBox runat="server" class='soloLectura' ID="txtvalor" size='15' MaxLength='20'
                                            runat="server" />
                                        <%--<ajaxToolkit:FilteredTextBoxExtender runat="server" TargetControlID="txtvalor" FilterMode="ValidChars"
                                            ValidChars="1234567890" Enabled="True" ID="FilteredTextBoxExtender1" FilterType="Numbers" />--%>
                                    </td>
                                </tr>
                                <tr valign="top">
                                    <td align="right">
                                        <b>Tipo :</b>
                                    </td>
                                    <td align="right">
                                        &nbsp;
                                    </td>
                                    <td width='167' align='left' colspan='3' class='TitDestacado'>
                                        <asp:DropDownList ID="dpl_tipo" runat="server">
                                            <asp:ListItem Value="$">Dinero</asp:ListItem>
                                            <asp:ListItem Value="%">Porcentaje</asp:ListItem>
                                            <asp:ListItem Value="#">Numérico</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td width='167' align='left' class='TitDestacado'>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic"
                                            ErrorMessage="Tipo Aporte" ForeColor="Red" ValidationGroup="crear" ControlToValidate="dpl_tipo">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr valign="top">
                                    <td colspan='4' align='center' class='TitDestacado'>
                                        &nbsp;
                                    </td>
                                    <td align='center' class='TitDestacado'>
                                        <asp:Button runat="server" ID="btn_crearaporte" OnClick="btn_crearaporte_Click" ValidationGroup="crear"
                                            Height="28px" />
                                        &nbsp;&nbsp;
                                        <asp:Button runat="server" ID="BtnCerrar" Text="Cerrar" PostBackUrl="~/FONADE/evaluacion/EvaluacionIndicadoresFinancieros.aspx"
                                            OnClick="BtnCerrar_Click" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table width="95%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td bgcolor="#3D5A87">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
