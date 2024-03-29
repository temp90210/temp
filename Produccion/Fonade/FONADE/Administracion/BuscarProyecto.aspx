﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuscarProyecto.aspx.cs" Inherits="Fonade.FONADE.Administracion.BuscarProyecto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../../Styles/jquery-ui-1.10.3.min.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <style type="text/css">
         table {
             width:100%;
         }
         td {
             vertical-align:top;
         }
     </style>
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
        <h1>
            <label>PRORROGA DE PROYECTOS</label>
        </h1>
        <br />
        <br />
        <table>
            <tr>
                <td>Proyecto</td>
                <td>
                    <asp:TextBox ID="txtProyecto" runat="server" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>No proyecto</td>
                <td>
                    <asp:TextBox ID="txtnoproyecto" runat="server" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align:center;">
                    <asp:Button ID="btnbuscar" runat="server" Text="Buscar" OnClick="btnbuscar_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <br />
                    <asp:GridView ID="gvproyectos" runat="server" CssClass="Grilla" Width="100%" AutoGenerateColumns="false" OnRowCommand="gvproyectos_RowCommand">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnidproecto" runat="server" Text='<%# Eval("Nomproyecto") %>' CommandArgument='<%# Eval("Id_Proyecto") + ";" + Eval("Nomproyecto") %>'></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
