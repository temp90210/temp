<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListarPostIt.aspx.cs" Inherits="Fonade.Controles.ListarPostIt1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="../Styles/siteProyecto.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/jquery-ui-1.10.3.min.js" rel="stylesheet" type="text/css" />
    
    <script src="../Scripts/jquery-ui-1.10.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/common.js" type="text/javascript"></script>

    <style type="text/css">
        .auto-style1 {
            width: 730px;
            height: 470px;
        }
        .para {
            height:200px;
            width:300px;
            overflow:scroll;
        }
        .auto-style2 {
            width: 153px;
        }
        .Grilla {}
    </style>
    <script type="text/javascript">
        function closeWindow() {
            window.parent.opener.focus();
            window.close();
        }
    </script>
</head>
<body class="auto-style1">
    <form id="form1" runat="server" class="auto-style1">
    <div class="auto-style1">
        <asp:Panel ID="P_PostIt" runat="server" CssClass="auto-style1">
            <table class="auto-style1">
                <thead>
                    <tr style="width:100%;">
                        <th style="background-color:#00468f; text-align:left; padding-left:50px">
                            <asp:Label ID="L_PostIt" runat="server" ForeColor="White" Text="LISTA POST IT" Width="260px"></asp:Label>
                        </th>
                    </tr>
                </thead>
                <tr>
                    <td  style="width:100%; text-align:right;">
                        <asp:Label ID="L_Nombreusuario" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td  style="width:100%;"><br /></td>
                </tr>
                 <tr>
                    <td  style="width:100%; vertical-align:top;">
                        <asp:GridView ID="GV_POST" runat="server" CssClass="Grilla" AutoGenerateColumns="False" Width="100%" AllowPaging="True">

                            <Columns>
                                <asp:BoundField DataField="Fecha" HeaderText="Fecha" />
                                <asp:BoundField DataField="Tarea" HeaderText="Tarea" />
                                <asp:BoundField DataField="Agendado" HeaderText="Agendado a" />
                                <asp:BoundField DataField="Agendo" HeaderText="Agendó" />
                            </Columns>

                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td  style="width:100%;"><br /></td>
                </tr>
                <tr>
                    <td  style="width:100%; text-align:center;">
                        <asp:Button ID="B_Cerrar" runat="server" Text="Cerrar" OnClientClick="closeWindow();" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
