﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoProduccionTMP.aspx.cs"
    Inherits="Fonade.FONADE.evaluacion.CatalogoProduccionTMP" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Producción</title>
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
<body>
    <form id="form1" runat="server">
    <div>
        <table class="auto-style1">
            <tr>
                <td colspan="2" class="style50" style="text-align: center;">
                    <asp:Label ID="lbl_enunciado" runat="server" Width="40%" />
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:Label ID="L_Item" runat="server" Text="Nombre:" Visible="false"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TB_Item" runat="server" ValidationGroup="accionar" Width="250px"
                        MaxLength="150" Visible="false"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TB_Item"
                        ErrorMessage="RequiredFieldValidator" ForeColor="Red" ValidationGroup="accionar">El nombre es requerido</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
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
                <td>
                    <asp:Label ID="lbl_inv_obvservaciones" Text="Observaciones:" runat="server" Visible="false" />
                </td>
                <td>
                    <asp:TextBox ID="txt_inv_observaciones" runat="server" TextMode="MultiLine" Columns="25"
                        Rows="5" Visible="false" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="2" class="style50" style="text-align: center;">
                    <asp:Label ID="Label1" runat="server" Text="REQUERIMIENTOS DE RECURSOS POR MES" Width="100%"></asp:Label>
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
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">
                    <asp:Button ID="B_Acion" runat="server" ValidationGroup="accionar" OnClick="B_Acion_Click" />
                </td>
                <td>
                    <asp:Button ID="B_Cancelar" runat="server" Text="Cancelar" OnClick="B_Cancelar_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
