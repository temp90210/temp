<%@ Page Language="C#" MasterPageFile="~/Master.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeBehind="ProyectoProrroga.aspx.cs" Inherits="Fonade.FONADE.Administracion.ProyectoProrroga" %>

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
        <label>
            PRORROGA DE PROYECTOS</label>
    </h1>
    <br />
    <br />
    <asp:Panel ID="pnlAgregar" runat="server" Visible="false" Enabled="false">
        <table>
            <tr>
                <td>
                    Proyecto
                </td>
                <td>
                    <asp:TextBox ID="txtproyecto" runat="server" Width="200px" ValidationGroup="adicionar"
                        Enabled="false"></asp:TextBox>
                    <asp:LinkButton ID="lnkbuscar" runat="server" Text="Buscar" OnClick="lnkbuscar_Click"></asp:LinkButton>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="RequiredFieldValidator"
                        ControlToValidate="txtproyecto" ValidationGroup="adicionar" Text="campo requerido"
                        ForeColor="#ff0000"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    Meses Prorroga
                </td>
                <td>
                    <asp:TextBox ID="txtmeses" runat="server" Width="200px" ValidationGroup="adicionar"></asp:TextBox>
                    <br />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="RequiredFieldValidator"
                        ControlToValidate="txtmeses" ValidationGroup="adicionar" Text="campo requerido"
                        ForeColor="#ff0000"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:Button ID="btnadicionar" runat="server" Text="Adicionar" ValidationGroup="adicionar"
                        OnClick="btnadicionar_Click" />
                </td>
                <td>
                    <asp:Button ID="btnvolver" runat="server" Text="Volver" OnClick="btnvolver_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlproyectos" runat="server">
        <table>
            <tr>
                <td>
                    <asp:ImageButton ID="imgagregar" runat="server" ImageUrl="~/Images/icoAdicionarUsuario.gif"
                        OnClick="imgagregar_Click" />
                    &nbsp;&nbsp;
                    <asp:LinkButton ID="lnkagregar" runat="server" Text="Adicionar Proyecto prorroga"
                        OnClick="lnkagregar_Click"></asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <br />
                    <asp:GridView ID="gv_proyectosProrroga" runat="server" CssClass="Grilla" Width="100%"
                        AutoGenerateColumns="false" AllowPaging="True" PageSize="50" OnPageIndexChanging="gv_proyectosProrroga_PageIndexChanging">
                        <Columns>
                            <asp:BoundField HeaderText="Id" DataField="Id_proyecto" />
                            <asp:BoundField HeaderText="Proyecto" DataField="nomproyecto" />
                            <asp:BoundField HeaderText="Prorroga" DataField="Prorroga" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
