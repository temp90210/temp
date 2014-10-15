<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InsumoProducto.aspx.cs" Inherits="Fonade.FONADE.evaluacion.InsumoProducto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
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
    <div style="width:100%; text-align:center;">
        
        <table class="Grilla" style="margin:0px auto; text-align:left;">
            <tr>
                <td colspan="2" style="background-color:#00468f">
                    <asp:Label ID="L_Titulo" runat="server" Font-Bold="True" ForeColor="White" Text="Encripción de llamadas" Width="100%" Height="100%"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="Text-align: center">
                    <asp:Label ID="L_NomProducto" runat="server" Text="Nombre Producto:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="NomProducto" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="Text-align: center">
                    <asp:Label ID="L_NomTipoInsumo" runat="server" Text="Tipo De Insumo:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="NomTipoInsumo" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="Text-align: center">
                    <asp:Label ID="L_nomInsumo" runat="server" Text="Nombre Insumo:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="nomInsumo" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
        
    </div>
    </form>
</body>
</html>
