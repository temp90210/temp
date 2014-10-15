<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoProducto.aspx.cs" Inherits="Fonade.FONADE.evaluacion.CatalogoProducto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Styles/Site.css" rel="stylesheet" />
    <style type="text/css">
        .style10
        {
            width: 100%;
        }
        .style11
        {
            width: 299px;
        }
        .titulos
        {
            text-align: center;
        }
        .celda
        {
            text-align:center;
         }
        .auto-style1 {
            width: 322px;
        }
        .auto-style2 {
            text-align: center;
            width: 322px;
        }
    </style>
    <script type="text/javascript">
        function abrirBuscar() {
            open("ayudaArancel.aspx", "ayuda Arancel", "800px; 500px");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table class="style10">
        <tr>
            <td>
                <asp:Label ID="L_NombreProductoServicio" runat="server" Text="Nombre del Producto o Servicio:"></asp:Label>
            </td>
            <td colspan="3">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"  ValidationGroup="crear" ControlToValidate="TB_NombreProductoServicio" ErrorMessage="RequiredFieldValidator" ForeColor="Red">*</asp:RequiredFieldValidator>
                <asp:TextBox ID="TB_NombreProductoServicio" runat="server" Width="300px"  ValidationGroup="crear"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="L_PosicionArancelaria" runat="server" 
                    Text="Posición Arancelaria:"></asp:Label>
            </td>
            <td class="auto-style1">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"  ValidationGroup="crear" ControlToValidate="TB_PosicionArancelariacodigo" ErrorMessage="RequiredFieldValidator" ForeColor="Red">*</asp:RequiredFieldValidator>
                <asp:TextBox ID="TB_PosicionArancelariacodigo" runat="server" Width="99px"  ValidationGroup="crear" Enabled="False"></asp:TextBox>&nbsp;&nbsp;
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"  ValidationGroup="crear" ControlToValidate="TB_PosicionArancelariadescripcion" ErrorMessage="RequiredFieldValidator" ForeColor="Red">*</asp:RequiredFieldValidator>
                <asp:TextBox ID="TB_PosicionArancelariadescripcion" runat="server" Width="185px"  ValidationGroup="crear" Enabled="False"></asp:TextBox>
                <br />
                <asp:LinkButton ID="LB_Buscar" runat="server" OnClick="LB_Buscar_Click" OnClientClick="abrirBuscar()">Buscar</asp:LinkButton>
            </td>
            <td colspan="2" rowspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="L_PrecioLanzamiento" runat="server" 
                    Text="Precio de Lanzamiento:"></asp:Label>
            </td>
            <td class="auto-style1">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"  ValidationGroup="crear" ControlToValidate="TB_PrecioLanzamiento" ErrorMessage="RequiredFieldValidator" ForeColor="Red">*</asp:RequiredFieldValidator>
                <asp:TextBox ID="TB_PrecioLanzamiento" runat="server"  ValidationGroup="crear"
                    Width="185px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="L_IVA" runat="server" 
                    Text="%IVA:"></asp:Label>
            </td>
            <td class="auto-style1">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"  ValidationGroup="crear" ControlToValidate="TB_IVA" ErrorMessage="RequiredFieldValidator" ForeColor="Red">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="RegularExpressionValidator" ForeColor="Red" ControlToValidate="TB_IVA" ValidationGroup="crear" ValidationExpression="^100$|^\d{0,2}(\.\d{1,7})?$" Text="% invalido"></asp:RegularExpressionValidator>
                <asp:TextBox ID="TB_IVA" runat="server" Width="99px"  ValidationGroup="crear"></asp:TextBox></td>
            <td>
                <asp:Label ID="L_Retencionfuente" runat="server" 
                    Text="%Retencion en la fuente:"></asp:Label>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"  ValidationGroup="crear" ControlToValidate="TB_Retencionfuente" ErrorMessage="RequiredFieldValidator" ForeColor="Red">*</asp:RequiredFieldValidator>
               <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="RegularExpressionValidator" ForeColor="Red" ControlToValidate="TB_Retencionfuente" ValidationGroup="crear" ValidationExpression="^100$|^\d{0,2}(\.\d{1,7})?$" Text="% invalido"></asp:RegularExpressionValidator>
                <asp:TextBox ID="TB_Retencionfuente" runat="server" Width="99px"  ValidationGroup="crear"></asp:TextBox></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="L_VentasCredito" runat="server" 
                    Text="%Ventas a Crédito:"></asp:Label>
            </td>
            <td class="auto-style1">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"  ValidationGroup="crear" ControlToValidate="TB_VentasCredito" ErrorMessage="RequiredFieldValidator" ForeColor="Red">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ErrorMessage="RegularExpressionValidator" ForeColor="Red" ControlToValidate="TB_VentasCredito" ValidationGroup="crear" ValidationExpression="^100$|^\d{0,2}(\.\d{1,7})?$" Text="% invalido"></asp:RegularExpressionValidator>
                <asp:TextBox ID="TB_VentasCredito" runat="server" Width="99px"  ValidationGroup="crear"></asp:TextBox></td>
            <td colspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">
            <br />
            <br />
                <asp:Label ID="L_titulo" runat="server" Text="PROYECCION DE VENTAS" Width="100%" Font-Bold="true" BackColor="Blue" ForeColor="White" BorderStyle="NotSet" CssClass="titulos"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="celda" style="font-weight: bold">
                <asp:Label ID="L_periodo" runat="server" 
                    Text="PERIODOS" Font-Bold="true"></asp:Label>
            </td>
            <td class="auto-style2" style="font-weight: bold">
                <asp:Label ID="L_anio1" runat="server" 
                    Text="Año 1" Font-Bold="true"></asp:Label>
            </td>
            <td class="celda" style="font-weight: bold">
                <asp:Label ID="L_anio2" runat="server" 
                    Text="Año 2" Font-Bold="true"></asp:Label>
            </td>
            <td class="celda" style="font-weight: bold">
                <asp:Label ID="L_anio3" runat="server" 
                    Text="Año 3" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label1" runat="server" Text="Cant. Mes 1"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes1" runat="server" OnTextChanged="CantAnio1Mes1_TextChanged"  AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes1" runat="server" OnTextChanged="CantAnio2Mes1_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes1" runat="server" OnTextChanged="CantAnio3Mes1_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label2" runat="server" Text="Cant. Mes 2"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes2" runat="server" OnTextChanged="CantAnio1Mes2_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes2" runat="server" OnTextChanged="CantAnio2Mes2_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes2" runat="server" OnTextChanged="CantAnio3Mes2_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label3" runat="server" Text="Cant. Mes 3"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes3" runat="server" OnTextChanged="CantAnio1Mes3_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes3" runat="server" OnTextChanged="CantAnio2Mes3_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes3" runat="server" OnTextChanged="CantAnio3Mes3_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label4" runat="server" Text="Cant. Mes 4"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes4" runat="server" OnTextChanged="CantAnio1Mes4_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes4" runat="server" OnTextChanged="CantAnio2Mes4_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes4" runat="server" OnTextChanged="CantAnio3Mes4_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label5" runat="server" Text="Cant. Mes 5"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes5" runat="server" OnTextChanged="CantAnio1Mes5_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes5" runat="server" OnTextChanged="CantAnio2Mes5_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes5" runat="server" OnTextChanged="CantAnio3Mes5_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label6" runat="server" Text="Cant. Mes 6"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes6" runat="server" OnTextChanged="CantAnio1Mes6_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes6" runat="server" OnTextChanged="CantAnio2Mes6_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes6" runat="server" OnTextChanged="CantAnio3Mes6_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label7" runat="server" Text="Cant. Mes 7"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes7" runat="server" OnTextChanged="CantAnio1Mes7_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes7" runat="server" OnTextChanged="CantAnio2Mes7_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes7" runat="server" OnTextChanged="CantAnio3Mes7_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label8" runat="server" Text="Cant. Mes 8"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes8" runat="server" OnTextChanged="CantAnio1Mes8_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes8" runat="server" OnTextChanged="CantAnio2Mes8_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes8" runat="server" OnTextChanged="CantAnio3Mes8_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label9" runat="server" Text="Cant. Mes 9"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes9" runat="server" OnTextChanged="CantAnio1Mes9_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes9" runat="server" OnTextChanged="CantAnio2Mes9_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes9" runat="server" OnTextChanged="CantAnio3Mes9_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label10" runat="server" Text="Cant. Mes 10"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes10" runat="server" OnTextChanged="CantAnio1Mes10_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes10" runat="server" OnTextChanged="CantAnio2Mes10_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes10" runat="server" OnTextChanged="CantAnio3Mes10_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label11" runat="server" Text="Cant. Mes 11"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes11" runat="server" OnTextChanged="CantAnio1Mes11_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes11" runat="server" OnTextChanged="CantAnio2Mes11_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes11" runat="server" OnTextChanged="CantAnio3Mes11_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="Label12" runat="server" Text="Cant. Mes 12"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="CantAnio1Mes12" runat="server" OnTextChanged="CantAnio1Mes12_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio2Mes12" runat="server" OnTextChanged="CantAnio2Mes12_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="CantAnio3Mes12" runat="server" OnTextChanged="CantAnio3Mes12_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="L_periodo0" runat="server"
                    Text="Precio" Font-Bold="True"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:TextBox ID="L_PrecioAnio1" runat="server" OnTextChanged="L_PrecioAnio1_TextChanged"  AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="L_PrecioAnio2" runat="server" OnTextChanged="L_PrecioAnio2_TextChanged"  AutoPostBack="true"></asp:TextBox>
            </td>
            <td class="celda">
                <asp:TextBox ID="L_PrecioAnio3" runat="server" OnTextChanged="L_PrecioAnio3_TextChanged"  AutoPostBack="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="celda">
                <asp:Label ID="L_periodo1" runat="server"
                    Text="Ventas Esperadas" Font-Bold="True"></asp:Label>
            </td>
            <td class="auto-style2">
                <asp:Label ID="L_totalVentas1" runat="server" Font-Bold="True" Text="0.0"></asp:Label>
            </td>
            <td class="celda">
                <asp:Label ID="L_totalVentas2" runat="server" Font-Bold="True" Text="0.0"></asp:Label>
            </td>
            <td class="celda">
                <asp:Label ID="L_totalVentas3" runat="server" Font-Bold="True" Text="0.0"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center" colspan="2" valign="middle">
                <asp:Button ID="B_Crear" runat="server" OnClick="Button1_Click" Height="30px" ValidationGroup="crear" Text="" />
            </td>
            <td colspan="2">
                <asp:Button ID="B_Cancelar" runat="server" PostBackUrl="~/FONADE/Proyecto/PProyectoMercadoProyecciones.aspx" Text="Cancelar" Height="30px" />
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2" valign="middle">
                &nbsp;</td>
            <td colspan="2">
                &nbsp;</td>
        </tr>
    </table>

    </div>
    </form>
</body>
</html>
