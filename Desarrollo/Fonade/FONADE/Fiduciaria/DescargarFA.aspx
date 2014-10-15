<%@ Page Title="FONDO EMPRENDER" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true"
    CodeBehind="DescargarFA.aspx.cs" Inherits="Fonade.FONADE.Fiduciaria.DescargarFA" %>

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
    <asp:GridView ID="detalleSolicitud" runat="server" OnRowCommand="detalleSolicitud_RowCommand"
        Width="98%" BorderWidth="0" CellSpacing="1" CellPadding="4" AllowPaging="True"
        PageSize="5" AutoGenerateColumns="False" CssClass="Grilla" HeaderStyle-HorizontalAlign="Left"
        OnPageIndexChanging="detalle_PageIndexChanging" 
        onrowdatabound="detalleSolicitud_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Número solicitud">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkmostrar" CommandArgument='<%# Eval("Id_PagoActividad") %>'
                        CommandName="mostrar" CausesValidation="False" Text='<%#Eval("Id_PagoActividad") %>'
                        runat="server" Font-Bold="true" ForeColor="Black" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Fecha envío">
                <ItemTemplate>
                    <asp:Label ID="lbl_FechEnvi" Text='<%# Eval("FechaCoordinador") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Empresa" DataField="razonsocial" HtmlEncode="false" />
            <asp:TemplateField HeaderText="Valor">
                <ItemTemplate>
                    <asp:Label ID="lbl_CantDin" Text='<%# Eval("CantidadDinero") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
    </asp:Panel>
    <br />
    <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Center">
    </asp:Panel>
</asp:Content>
