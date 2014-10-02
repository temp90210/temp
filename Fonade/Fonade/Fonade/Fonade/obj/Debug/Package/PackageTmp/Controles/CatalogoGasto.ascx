<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CatalogoGasto.ascx.cs"
    Inherits="Fonade.Controles.CatalogoGasto" %>
NUEVO GASTO

<table width='900px' border='0' cellspacing='0' cellpadding='3'>
    <tr valign="top">
        <td colspan="2">
            &nbsp;
        </td>
    </tr>
    <tr valign="top">
        <td width='167'  align="Right">
            <b>Descripción:</b>
        </td>
        <td >
                <asp:TextBox ID="txtDescripcion" MaxLength="255" Width="300px" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ErrorMessage="La descripción es requerida" 
                    ControlToValidate="txtDescripcion" Display="Dynamic" 
                    ValidationGroup="GrupoGastos"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr valign="top">
        <td align="Right">
            <b>Valor:</b>
        </td>
        <td align="left" colspan="3" >
            <asp:TextBox ID="txtValor" MaxLength="20" Width="100px" runat="server" Text="0"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                    ErrorMessage="El valor debe ser numérico" ControlToValidate="txtValor" 
                Display="Dynamic" ValidationGroup="GrupoGastos"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                ControlToValidate="txtValor" Display="Dynamic" 
                ErrorMessage="El valor debe ser numérico" ValidationExpression="[0-9]*" 
                ValidationGroup="GrupoGastos"></asp:RegularExpressionValidator>
        </td>
    </tr>
    <tr valign="top">
        <td colspan="4" align="center">
            <asp:HiddenField ID="hddCodProyecto" runat="server" Value="" />
            <asp:HiddenField ID="hddAccion" runat="server" Value="" />
            <asp:HiddenField ID="hddIdGasto" runat="server" Value="" />
            <asp:HiddenField ID="hddTipo" runat="server" Value="" />
            <asp:Button ID="btnGasto" runat="server" Text="Crear" OnClick="btnGasto_Click" 
                ValidationGroup="GrupoGastos" />
            <asp:Button ID="btnCancelarGasto" runat="server" Text="Cerrar" OnClick="btnCancelarGasto_Click" />
        </td>
    </tr>
</table>
