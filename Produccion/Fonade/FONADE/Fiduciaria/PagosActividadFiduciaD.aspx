<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PagosActividadFiduciaD.aspx.cs" MasterPageFile="~/Master.Master" Inherits="Fonade.FONADE.Fiduciaria.PagosActividadFiduciaD" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/Site.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="bodyContentPlace" runat="server">
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
    <table style="width: 100%; height: 79px;">
        <tr>
            <td class="auto-style2">&nbsp;</td>
            <td class="auto-style3">
                <asp:Label ID="lblNoActa" runat="server" Text="No Acta"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="textBoxSolicitud" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">&nbsp;</td>
            <td class="auto-style3">&nbsp;</td>
            <td>
                <asp:Button ID="btnEnviar" runat="server" Text="Buscar" OnClick="btnEnviar_Click" />

            </td>
        </tr>
    </table>
    <br />

    <asp:GridView ID="gvActas" runat="server" Width="98%" BorderWidth="0" CellSpacing="1" CellPadding="4" AllowPaging="True" PageSize="5" AutoGenerateColumns="False" OnPageIndexChanging="gvActas_PageIndexChanging" CssClass="Grilla" HeaderStyle-HorizontalAlign="Left" OnRowCommand="gvActas_RowCommand">
        <Columns>
            <%--<asp:BoundField HeaderText="Número Solicitud" DataField="CodActaFonade" Visible="false" />--%>
            <asp:TemplateField HeaderText="Nombre">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkmostrar" runat="server" CausesValidation="False" CommandArgument='<%# Eval("CodActaFonade") %>'
                        CommandName="mostrar" Text='<%#Eval("CodActaFonade")  + " - " +"Ver"%>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Fecha Envio" DataField="Fecha" HtmlEncode="false" />
            <asp:BoundField HeaderText="Número Solicitudes" DataField="NEW" HtmlEncode="false" />
            <asp:BoundField HeaderText="Firma" DataField="DatosFirma" HtmlEncode="false" />
            <asp:BoundField HeaderText="Estado" DataField="ESTADO" HtmlEncode="false" />
        </Columns>

    </asp:GridView>

    <%--<asp:BoundField HeaderText="Número Solicitud" DataField="CodActaFonade" Visible="false" />--%>
</asp:Content>



