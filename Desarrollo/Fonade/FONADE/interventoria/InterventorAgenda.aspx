<%@ Page Language="C#" MasterPageFile="~/Master.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeBehind="InterventorAgenda.aspx.cs" Inherits="Fonade.FONADE.interventoria.InterventorAgenda" %>

<asp:Content ID="head1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        table
        {
            width: 100%;
        }
        td
        {
            vertical-align: top;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <h1>
        <asp:Label ID="L_titulo" runat="server" Text="AGENDAR VISITA" />
    </h1>
    <table>
        <tr>
            <td>
                <asp:GridView ID="gv_agenda" runat="server" CssClass="Grilla" Width="100%" EmptyDataText="Usted no tiene Visitas Programadas."
                    AutoGenerateColumns="False" OnRowCommand="gv_agenda_RowCommand" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Image ID="imgEstado" runat="server" ImageUrl='<%# "~/Images/ico" + Eval("Estado").ToString().Trim() + "s.gif" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Nit" DataField="Nit" />
                        <asp:TemplateField HeaderText="Nombre">
                            <ItemTemplate>
                                <asp:Button ID="btnNombre" runat="server" Text='<%# Eval("razonsocial") %>' CommandArgument='<%# Eval("Id_Visita") %>'
                                    CssClass="boton_Link_Grid" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fecha o Período">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("FechaInicio") + " a " + Eval("FechaFin") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnNuevaVisita" runat="server" Text="Agendar nueva visita" Visible="false"
                    Enabled="false" OnClick="btnNuevaVisita_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
