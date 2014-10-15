<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImprimirPlanOperativos.aspx.cs"
    Inherits="Fonade.FONADE.interventoria.ImprimirPlanOperativos" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Resultado de impresión</title>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" />
    <script type="text/javascript">

        function imprimir() {

            var divToPrint = document.getElementById('contentPrincipal');
            var newWin = window.open('', 'Print-Window', 'width=1000,height=500');
            newWin.document.open();
            newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
            newWin.document.close();
            setTimeout(function () { newWin.close(); }, 1000);
        }

    </script>
</head>
<body onload="imprimir()">
    <div id="contentPrincipal">
        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tbody>
                <tr>
                    <td width="50%" align="center" valign="baseline" bgcolor="#000000" style="color: White;">
                        <b>PLAN OPERATIVO</b>
                    </td>
                    <td width="10%" align="right">
                        &nbsp;
                    </td>
                    <td width="40%" align="left">
                        <asp:Label ID="L_Fecha" runat="server" />
                    </td>
                </tr>
                <tr bgcolor="#000000">
                    <td colspan="3">
                        &nbsp;
                    </td>
                </tr>
                <tr bgcolor="#CCCCCC">
                    <td colspan="3">
                        &nbsp;
                    </td>
                </tr>
            </tbody>
        </table>
        <asp:Label ID="lbl_cuerpo" runat="server" />
    </div>
</body>
</html>
