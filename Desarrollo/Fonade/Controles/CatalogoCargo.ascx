<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogoCargo.ascx.cs"
    Inherits="Fonade.Controles.CatalogoCargo" %>
NUEVO CARGO
<table width='1000px' border='0' cellspacing='0' cellpadding='3'>
    <tr valign="top">
        <td colspan="2">
            &nbsp;
        </td>
    </tr>
    <tr valign="top">
        <td width='167' align="Right">
            <b>Cargo:</b>
        </td>
        <td>
            <asp:TextBox ID="txtCargo" runat="server" MaxLength="100" Width="300px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                ControlToValidate="txtCargo" Display="Dynamic" 
                ErrorMessage="El cargo del usuario es requerido" 
                ValidationGroup="ValidadorCargo"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr valign="top">
        <td align="Right">
            <b>Dedicación:</b>
        </td>
        <td align="left" colspan="3" class="TitDestacado">
            <asp:DropDownList ID="ddlDedicacion" runat="server">
                <asp:ListItem Value="Completa" Text="Completa"></asp:ListItem>
                <asp:ListItem Value="Parcial" Text="Parcial"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr valign="top">
        <td align="Right">
            <b>Tipo de Contratación:</b>
        </td>
        <td align="left" colspan="3">
            <asp:DropDownList ID="ddlTipoContratacion" runat="server">
                <asp:ListItem Value="Fija" Text="Fija"></asp:ListItem>
                <asp:ListItem Value="Temporal" Text="Temporal"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr valign="top">
        <td align="Right">
            <b>Valor Mensual:</b>
        </td>
        <td align="left" colspan="3">
            <asp:TextBox ID="txtValorMensual" runat="server" MaxLength="20" Width="100px" Text="0"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                ControlToValidate="txtValorMensual" Display="Dynamic" 
                ErrorMessage="El valor debe ser numérico" ValidationGroup="ValidadorCargo"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                ControlToValidate="txtValorMensual" Display="Dynamic" 
                ErrorMessage="El valor debe ser numérico" ValidationExpression="[0-9]*" 
                ValidationGroup="ValidadorCargo"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr valign="top">
        <td align="Right">
            <b>Valor Anual:</b>
        </td>
        <td align="left" colspan="3" class="TitDestacado">
            <asp:TextBox ID="txtValorAnual" runat="server" MaxLength="20" Width="100px" Text="0"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                ControlToValidate="txtValorAnual" Display="Dynamic" 
                ErrorMessage="El valor debe ser numérico" ValidationGroup="ValidadorCargo"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                ControlToValidate="txtValorAnual" Display="Dynamic" 
                ErrorMessage="El valor debe ser numérico" ValidationExpression="[0-9]*" 
                ValidationGroup="ValidadorCargo"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr valign="top">
        <td align="Right">
            <b>Otros Gastos:</b>
        </td>
        <td align="left" colspan="3">
            <asp:TextBox ID="txtOtrosGastos" runat="server" MaxLength="20" Width="100px" Text="0"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                ControlToValidate="txtOtrosGastos" Display="Dynamic" 
                ErrorMessage="El valor debe ser numérico" ValidationGroup="ValidadorCargo"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                ControlToValidate="txtOtrosGastos" Display="Dynamic" 
                ErrorMessage="El valor debe ser numérico" ValidationExpression="[0-9]*" 
                ValidationGroup="ValidadorCargo"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr valign="top">
        <td align="Right">
            <b>Observación:</b>
        </td>
        <td colspan="3">
            <asp:TextBox ID="txtObservacion" runat="server" MaxLength="20" Width="410px" TextMode="MultiLine"
                Height="100px"></asp:TextBox>
        </td>
    </tr>
    <tr valign="top">
        <td colspan="4" align="center">
            <asp:HiddenField ID="hddCodProyecto" runat="server" Value="" />
            <asp:HiddenField ID="hddAccion" runat="server" Value="" />
            <asp:HiddenField ID="hddIdCargo" runat="server" Value="" />
            <asp:Button ID="btnCargo" runat="server" Text="Crear" OnClick="btnCargo_Click" 
                ValidationGroup="ValidadorCargo" />
            <asp:Button ID="btnCancelarCargo" runat="server" Text="Cerrar" OnClick="btnCancelarCargo_Click" />
        </td>
    </tr>
</table>
