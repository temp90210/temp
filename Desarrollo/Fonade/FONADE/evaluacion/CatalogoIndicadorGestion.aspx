<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoIndicadorGestion.aspx.cs"
    Inherits="Fonade.FONADE.evaluacion.CatalogoIndicadorGestion" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1
        {
            width: 70%;
            text-align: center;
            margin: 0px auto;
        }
        .auto-style2
        {
            height: 21px;
        }
    </style>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        <table class="auto-style1">
            <tr>
                <td colspan="2" style="text-align: center;" class="auto-style2">
                    <asp:Label ID="L_NUEVOINDICADOR" runat="server" Text="NUEVO INDICADOR" Font-Bold="true"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td style="text-align: right; width: 50%">
                    <asp:Label ID="L_Aspecto" runat="server" Text="Aspecto:" Font-Bold="True"></asp:Label>
                </td>
                <td style="text-align: center; width: 50%">
                    <asp:TextBox ID="TB_Aspecto" runat="server" TextMode="MultiLine" Height="80px" Width="50%"
                        ValidationGroup="Actualizargrilla"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RFV_Aspecto" runat="server" ControlToValidate="TB_Aspecto"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" Text="Campo requerido"
                        ValidationGroup="Actualizargrilla"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; width: 50%">
                    <asp:Label ID="L_FechaSeguimiento" runat="server" Text="Fecha de Seguimiento:" Font-Bold="True"></asp:Label>
                </td>
                <td style="text-align: center; width: 50%">
                    <asp:TextBox ID="TB_fechaSeguimiento" runat="server" Width="50%" ValidationGroup="Actualizargrilla"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RFV_Fecha" runat="server" ControlToValidate="TB_fechaSeguimiento"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" Text="Campo requerido"
                        ValidationGroup="Actualizargrilla"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; width: 50%">
                    <asp:Label ID="L_TipoIndicador" runat="server" Text="Tipo Indicador:" Font-Bold="True"></asp:Label>
                </td>
                <td style="text-align: center; width: 50%">
                    <asp:DropDownList ID="DD_TipoIndicador" runat="server" OnSelectedIndexChanged="DD_TipoIndicador_SelectedIndexChanged">
                        <asp:ListItem>Indicadores de Gestión</asp:ListItem>
                        <asp:ListItem>Indicadores Cualitativos y de Cumplimiento</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td rowspan="2" style="text-align: right; width: 50%">
                    <asp:Label ID="L_Indicador" runat="server" Text="Indicador:" Font-Bold="True"></asp:Label>
                </td>
                <td style="text-align: center; width: 50%">
                    <asp:TextBox ID="TB_Numerador" runat="server" Width="50%" ValidationGroup="Actualizargrilla"></asp:TextBox>
                    <asp:TextBoxWatermarkExtender ID="TBWE_Numerador" runat="server" TargetControlID="TB_Numerador"
                        WatermarkText="Numerador" Enabled="true">
                    </asp:TextBoxWatermarkExtender>
                    <br />
                    <asp:RequiredFieldValidator ID="RFV_Numerador" runat="server" ControlToValidate="TB_Numerador"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" Text="Campo requerido"
                        ValidationGroup="Actualizargrilla"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: center; width: 50%">
                    <asp:TextBox ID="TB_Denominador" runat="server" Width="50%"></asp:TextBox>
                    <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="TB_Denominador"
                        WatermarkText="Denominador" Enabled="true">
                    </asp:TextBoxWatermarkExtender>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; width: 50%">
                    <asp:Label ID="L_Descripcionindicador" runat="server" Text="Descripción del indicador:"
                        Font-Bold="True"></asp:Label>
                </td>
                <td style="text-align: center; width: 50%">
                    <asp:TextBox ID="TB_Descripcion" runat="server" Height="80px" TextMode="MultiLine"
                        Width="50%" ValidationGroup="Actualizargrilla"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RFV_Descripcion" runat="server" ControlToValidate="TB_Descripcion"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" Text="Campo requerido"
                        ValidationGroup="Actualizargrilla"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; width: 50%">
                    <asp:Label ID="L_RangoAceptable" runat="server" Text="Rango Aceptable:" Font-Bold="True"></asp:Label>
                </td>
                <td style="text-align: center; width: 50%">
                    <asp:TextBox ID="TB_rango" runat="server" Width="20%" ValidationGroup="Actualizargrilla"></asp:TextBox>
                    <asp:Label ID="L_RangoAceptablePorcentaje" runat="server" Text="%" Font-Bold="True"></asp:Label>
                    <br />
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="RegularExpressionValidator"
                        ForeColor="Red" ControlToValidate="TB_rango" ValidationGroup="Actualizargrilla"
                        ValidationExpression="^100$|^\d{0,2}(\.\d{1,7})?$" Text="% invalido"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="RFV_Rango" runat="server" ControlToValidate="TB_rango"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" Text="Campo requerido"
                        ValidationGroup="Actualizargrilla"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="B_Crear" runat="server" OnClick="B_Crear_Click" Text="Crear" ValidationGroup="Actualizargrilla" />
                </td>
                <td>
                    <asp:Button ID="B_Cancelar" runat="server" Text="Cancelar" PostBackUrl="~/FONADE/evaluacion/EvaluacionProductos.aspx" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
