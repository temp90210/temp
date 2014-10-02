<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="PagosActividadFiducia.aspx.cs" Inherits="Fonade.FONADE.Fiduciaria.PagosActividadFiducia" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/Site.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">

    <%--<asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
        <asp:Label ID="lblTitulo" runat="server" Text="Label"></asp:Label>
    </asp:Panel>--%>
    <table width="98%" border="0">
        <tr>
            <td class="style50">
                <h1>
                    <asp:Label runat="server" ID="lbl_Titulo" Style="font-weight: 700"></asp:Label>
            </td>
            <td align="right">
            </td>
        </tr>
    </table>
    <asp:GridView ID="gvSolicitudes" ShowHeaderWhenEmpty="true" OnPageIndexChanging="gvSolicitudes_PageIndexChanging" OnRowCommand="gvSolicitudes_RowCommand" runat="server" Width="98%" BorderWidth="0" CellSpacing="1" CellPadding="4" AllowPaging="True" PageSize="5" AutoGenerateColumns="False" CssClass="Grilla" HeaderStyle-HorizontalAlign="Left" >
        <Columns>
            <asp:TemplateField HeaderText="Número de solicitud">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkmostrar"  runat="server" CausesValidation="False" CommandArgument='<%# Eval("Id_Acta") %>'
                        CommandName="mostrar" Text='<%#Eval("Id_Acta")  + " " +" Ver"%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Fecha envio" DataField="Fecha" HtmlEncode="false" />
            <asp:BoundField HeaderText="Número de solicitudes" DataField="NEW" HtmlEncode="false" />
            <asp:BoundField HeaderText="Firma" DataField="DatosFirma" HtmlEncode="false"  />
        </Columns>

    </asp:GridView>
</asp:Content>
