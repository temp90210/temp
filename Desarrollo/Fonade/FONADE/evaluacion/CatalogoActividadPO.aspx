<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoActividadPO.aspx.cs"
    Inherits="Fonade.FONADE.evaluacion.CatalogoActividadPO" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>FONDO EMPRENDER - Actividad</title>
    <style type="text/css">
        .auto-style1
        {
            width: 80%;
            margin: 0px auto;
            text-align: center;
        }
        .panelmeses
        {
            margin: 0px auto;
            text-align: center;
        }
        .button-aling{
            text-align: right;
            width: auto;
            height: auto;
        }
        .auto-style2
        {
            height: 23px;
        }
    </style>
    <link href="../../Styles/Site.css" rel="stylesheet" />
    <script type="text/javascript">
        function ValidNum(e) {
            var tecla = document.all ? tecla = e.keyCode : tecla = e.which;
            return (tecla > 47 && tecla < 58);
        }
    </script>
</head>
<body style="overflow-x: hidden; overflow-y: hidden; width: 100%;">
    <form id="form1" runat="server">
    <div style="width: 100%">
        <table width="98%" border="0">
            <tr>
                <td class="style50">
                    <h1 style="text-align: center;">
                        <asp:Label ID="lbl_titulo_PO" runat="server" Text="MODIFICAR ACTIVIDAD" Width="50%" Style="text-align: center;" />
                    </h1>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td style="text-align: center">
                    <asp:Label ID="L_Item" runat="server" Text="Item:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TB_item" runat="server" ValidationGroup="accionar" Width="50px"
                        MaxLength="3"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TB_item"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="accionar">Campo Requerido</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:Label ID="L_Actividad" runat="server" Text="Actividad:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TB_Actividad" runat="server" ValidationGroup="accionar" Width="250px"
                        MaxLength="150"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TB_Actividad"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="accionar">Campo Requerido</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:Label ID="L_Metas" runat="server" Text="Metas:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TB_metas" runat="server" TextMode="MultiLine" ValidationGroup="accionar"
                        Columns="60" Rows="7"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TB_metas"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="accionar">Campo Requerido</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:Label ID="lbl_inv_aprobar" Text="Aprobar:" runat="server" Visible="false" />
                </td>
                <td>
                    <asp:DropDownList ID="dd_inv_aprobar" runat="server" Visible="false">
                        <asp:ListItem Text="Si" Value="1" />
                        <asp:ListItem Text="No" Value="0" />
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:Label ID="lbl_inv_obvservaciones" Text="Observaciones:" runat="server" Visible="false" />
                </td>
                <td>
                    <asp:TextBox ID="txt_inv_observaciones" runat="server" TextMode="MultiLine" Columns="25"
                        Rows="5" Visible="false" />
                </td>
            </tr>
            <table width="98%" border="0">
                <tr>
                    <td class="style50">
                        <h1>
                            <asp:Label runat="server" ID="lbl_Titulo" Style="text-align: center;"></asp:Label>
                        </h1>
                    </td>
                    <td align="right">
                    </td>
                </tr>
            </table>
            
            <tr>
                <td class="style50">
                    <h1 style="text-align: center;">
                        <asp:Label ID="Label1" runat="server" Text="REQUERIMIENTOS DE RECURSOS POR MES" Width="100%" Style="text-align: center;"></asp:Label>
                    </h1>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Panel ID="P_Meses" runat="server" Width="100%">
                        <asp:Table ID="T_Meses" runat="server" CssClass="panelmeses">
                        </asp:Table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <div class="button-aling"><br>
            <asp:Button ID="B_Acion" runat="server" ValidationGroup="accionar" OnClick="B_Acion_Click"
                        Text="Crear" />
            <asp:Button ID="B_Cancelar" runat="server" Text="Cancelar" OnClick="B_Cancelar_Click" />            
        </div>
    </div>
    </form>
</body>
</html>
