<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoVentasInter.aspx.cs"
    Inherits="Fonade.FONADE.interventoria.CatalogoVentasInter" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style type="text/css">
        table
        {
            width: 100%;
        }
        .celdaest
        {
            text-align: center;
        }
    </style>
    <link href="../../Styles/siteProyecto.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table class="style10">
            <tr>
                <td>
                    <h1>
                        <asp:Label ID="lbltitulo" runat="server" Style="font-weight: 700">Documentos</asp:Label>
                    </h1>
                </td>
            </tr>
        </table>
        <br />
        <asp:Panel ID="pnlPrincipal" runat="server" Style="margin-left: 10px;">
            <asp:GridView ID="GrvDocumentos" runat="server" Width="100%" AutoGenerateColumns="False"
                CssClass="Grilla" AllowPaging="True" AllowSorting="True" PagerStyle-CssClass="Paginador"
                OnSorting="GrvDocumentosSorting" OnPageIndexChanging="GrvDocumentosPageIndexChanging"
                ShowHeader="true" ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:TemplateField HeaderText="Tipo" SortExpression="NomTipo">
                        <ItemTemplate>
                            <asp:ImageButton ID="PDF" runat="server" ImageUrl="../../Images/IcoDocPDF.gif" Style="cursor: pointer;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Nombre" DataField="NomDocumento" />
                    <asp:BoundField HeaderText="Fecha" DataField="Fecha" HtmlEncode="false" DataFormatString="{0:MMM-dd-yyyy hh:mm tt}" />
                </Columns>
                <PagerStyle CssClass="Paginador" />
            </asp:GridView>
            <br />
            <asp:Button ID="btnRegresar" runat="server" OnClick="btnRegresar_Click" Text="Regresar" />
            <br />
        </asp:Panel>
        <br />
    </div>
    </form>
</body>
</html>
