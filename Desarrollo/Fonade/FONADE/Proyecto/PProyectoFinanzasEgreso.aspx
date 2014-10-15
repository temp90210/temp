<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PProyectoFinanzasEgreso.aspx.cs"
    Inherits="Fonade.FONADE.Proyecto.PProyectoFinanzasEgreso" EnableEventValidation="true" %>

<%@ Register Src="../../Controles/Post_It.ascx" TagName="Post_It" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
    <asp:Panel ID="pnlEgresos" Visible="true" runat="server">
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
                    <table width='95%' border='0' align="center" cellspacing='0' cellpadding='0'>
                        <tr>
                            <td width='18' align='left'>
                                <div class="help_container">
                                    <div onclick="textoAyuda({titulo: 'Egresos', texto: 'Egresos'});">
                                        <img src="../../Images/imgAyuda.gif" border="0" alt="help_Objetivos">
                                    </div>
                                </div>
                            </td>
                            <td width='350'>
                                <b>Egresos:&nbsp;&nbsp;&nbsp;&nbsp;</b>
                            </td>
                            <td align='right'>
                                <div id="div_Post_It_2" runat="server" visible="false">
                                    <uc1:Post_It ID="Post_It2" runat="server" _txtCampo="Egresos" _txtTab="1" />
                                </div>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table border='0' cellspacing='1' cellpadding='4'>
                        <tr>
                            <td width='50%'>
                                Índice de Actualización monetaria
                            </td>
                            <td>
                                <!--Validar formato Moneda-->
                                <asp:TextBox ID="txtActualizacionMonetaria" runat="server" MaxLengt="5" Width="30px"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" CssClass="failureNotification"
                                    ControlToValidate="txtActualizacionMonetaria" ErrorMessage="0 a 9 con punto (.) decimal"
                                    ValidationExpression="[0-9]*\,?[0-9]*" ValidationGroup="validaGuardar">0 a 9 con punto (.) decimal</asp:RegularExpressionValidator>
                            </td>
                            <td>
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass='Boton' OnClick="btnGuardar_Click"
                                    ValidationGroup="validaGuardar" Visible="false" />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table width='95%' align="Center" border='0' cellpadding='0' cellspacing='0'>
                        <tr>
                            <td align='left' valign='top' width='98%'>
                                <table width='100%' border='0' cellspacing='1' cellpadding='4'>
                                    <tr>
                                        <td align='left'>
                                            <asp:Panel ID="pnlAdicionar" runat="server" Visible="false">
                                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/icoAdicionarUsuario.gif" />
                                                <asp:Button ID="btnAdicionarInversion" runat="server" Text="Adicionar Inversión"
                                                    CssClass='TituloDestacados' BorderStyle="None" OnClick="btnAdicionarInversion_Click" />
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr bgcolor="#00468f" align="center">
                                        <td>
                                            <span style="color: #ffffff;">Inversiones Fijas y Diferidas</span>
                                        </td>
                                    </tr>
                                </table>
                                <asp:GridView ID="gw_InversionesFijas" runat="server" Width="100%" AutoGenerateColumns="False"
                                    CssClass="Grilla" OnRowCommand="gw_InversionesFijas_RowCommand" DataKeyNames="Id_Inversion"
                                    GridLines="None" CellSpacing="1" CellPadding="4" HeaderStyle-HorizontalAlign="Left"
                                    OnRowDataBound="gw_InversionesFijas_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-Width="3%">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="btn_Borrar" CommandName="Borrar" CommandArgument='<%# Eval("Id_Inversion") %>'
                                                    runat="server" ImageUrl="/Images/icoBorrar.gif" Visible="true" OnClientClick="return Confirmacion('Esta seguro que desea borrar el concepto seleccionado?')" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="concepto" HeaderText="Concepto" HeaderStyle-Width="47%"
                                            ItemStyle-HorizontalAlign="Left" />
                                        <asp:BoundField DataField="valor" HeaderText="Valor" HeaderStyle-Width="12%" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="mes" HeaderText="Mes" HeaderStyle-Width="8%" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="tipoFuente" HeaderText="Tipo de Fuente" HeaderStyle-Width="30%"
                                            ItemStyle-HorizontalAlign="Center" />
                                    </Columns>
                                </asp:GridView>
                                <br />
                                <table border="0" cellpadding="4" cellspacing="1" width="100%">
                                    <tr bgcolor="#00468f" align="center">
                                        <td>
                                            <span style="color: #ffffff;">Costos de Puesta en Marcha</span>
                                        </td>
                                    </tr>
                                </table>
                                <asp:GridView ID="gw_CostosPuestaMarcha" runat="server" AutoGenerateColumns="true"
                                    CssClass="Grilla" Width="100%" GridLines="None" CellSpacing="1" CellPadding="4"
                                    HeaderStyle-HorizontalAlign="Left" />
                                <br />
                                <table border="0" cellpadding="4" cellspacing="1" width="100%">
                                    <tr bgcolor="#00468f" align="center">
                                        <td>
                                            <span style="color: #ffffff;">Costos Anualizados Administrativos</span>
                                        </td>
                                    </tr>
                                </table>
                                <asp:GridView ID="gw_CostosAnualizados" runat="server" AutoGenerateColumns="True"
                                    CssClass="Grilla" Width="100%" GridLines="None" CellSpacing="1" CellPadding="4"
                                    HeaderStyle-HorizontalAlign="Left" />
                                <br />
                                <table border="0" cellpadding="4" cellspacing="1" width="100%">
                                    <tr bgcolor="#00468f" align="center">
                                        <td>
                                            <span style="color: #ffffff;">Gastos de Personal</span>
                                        </td>
                                    </tr>
                                </table>
                                <asp:GridView ID="gw_GastosPersonales" runat="server" AutoGenerateColumns="True"
                                    CssClass="Grilla" Width="100%" GridLines="None" CellSpacing="1" CellPadding="4" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
    </asp:Panel>
    <!--  Nuevo Panel -->
    <asp:Panel ID="pnlCrearInversion" Visible="false" runat="server">
        <table width='600px' border='0' cellspacing='0' cellpadding='2'>
            <tr>
                <td align='center'>
                    NUEVA INVERSIÓN
                </td>
            </tr>
        </table>
        <table width='600px' border='1' cellpadding='0' cellspacing='0' bordercolor='#4E77AF'>
            <tr>
                <td align='center' valign='top' width='98%'>
                    <table width='98%' border='0' cellspacing='0' cellpadding='3'>
                        <tr valign="top">
                            <td align="Right" width="110">
                                <b>Concepto:</b>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtConcepto" runat="server" MaxLength="255" Width="300px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtConcepto"
                                    ErrorMessage="Ingrese el campo Concepto" ValidationGroup="GrupoCrearInversion">Ingrese el campo Concepto</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="Right">
                                <b>Valor:</b>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtValor" runat="server" MaxLength="20" Width="90px" Text="0"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtValor"
                                    ErrorMessage="Ingrese el campo Valor" ValidationExpression="[0-9]*\,?[0-9]*"
                                    ValidationGroup="GrupoCrearInversion">Ingrese el campo Valor</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="Right">
                                <b>Meses:</b>
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtSemana" runat="server" MaxLength="5" Width="30px" Text="0"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtSemana"
                                    ErrorMessage="Ingrese el campo Semana" ValidationExpression="[0-9]*" ValidationGroup="GrupoCrearInversion">Ingrese el campo Valor</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="Right">
                                <b>Tipo de Fuente:</b>
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ddlTipoFuente" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="right" class="TitDestacado" colspan="2">
                                <asp:Button ID="btnCrearInversion" runat="server" Text="Crear" OnClick="btnCrearInversion_Click"
                                    ValidationGroup="GrupoCrearInversion" />
                                <asp:Button ID="btnCancelarNuevaInversion" runat="server" Text="Cancelar" OnClick="btnCancelarNuevaInversion_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <br />
    </asp:Panel>
    </form>
</body>
</html>
