<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoNominaInter.aspx.cs"
    Inherits="Fonade.FONADE.interventoria.CatalogoNominaInter" %>

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
        <asp:Panel ID="pnl_NuevoDoc" runat="server" Visible="false">
            <table width="95%" border="1" cellpadding="0" cellspacing="0" bordercolor="#4E77AF">
                <tbody>
                    <tr>
                        <td align="center" valign="top" width="98%">
                            <table width="98%" border="0" cellspacing="0" cellpadding="0">
                                <tbody>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <table width="98%" border="0" cellspacing="0" cellpadding="3">
                                <tbody>
                                    <tr valign="top">
                                        <td colspan="2">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="right">
                                            <b>Nombre:</b>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="NomDocumento" runat="server" MaxLength="80" size="50" />
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="right">
                                            <b>Subir Archivo:</b>
                                        </td>
                                        <td>
                                            <asp:FileUpload ID="Archivo" runat="server" size="50" />
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="right">
                                            <b>Tipo de Documento:</b>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="dd_TipoInterventor" runat="server" />
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="right">
                                            <b>Comentario:</b>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="Comentario" runat="server" TextMode="MultiLine" Columns="50" Rows="5" />
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td colspan="2" align="right">
                                            <asp:Button ID="btnCrear" Text="Crear" runat="server" OnClick="btnCrear_Click" />
                                            <asp:Button ID="Button1" Text="Cerrar" runat="server" OnClientClick="javascript:history.back(-1)" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
        </asp:Panel>
        <br />
    </div>
    </form>
</body>
</html>
