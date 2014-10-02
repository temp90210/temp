<%@ Page Language="C#" Title="Fondo Emprender - Menu" MasterPageFile="~/Master.master"
    AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Fonade.FONADE.MiPerfil.Home" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <span style="margin: 5px;">Hasta</span>
    <asp:DropDownList AutoPostBack="true" OnSelectedIndexChanged="FiltrarFecha_OnSelectedIndexChanged"
        runat="server" ID="FiltrarFecha">
        <asp:ListItem Value="1">Hoy</asp:ListItem>
        <asp:ListItem Value="7">Dentro de 1 semana</asp:ListItem>
        <asp:ListItem Value="15">Dentro de 2 semanas</asp:ListItem>
        <asp:ListItem Value="30">Dentro de 1 mes</asp:ListItem>
        <asp:ListItem Value="180">Dentro de 6 meses</asp:ListItem>
        <asp:ListItem Value="*" Selected="True">Siempre</asp:ListItem>
    </asp:DropDownList>
    <div style="padding: 20px 0px;">
        <asp:GridView ID="gw_Tareas" runat="server" Width="98%" AutoGenerateColumns="False"
            DataKeyNames="" CssClass="Grilla" AllowPaging="true" AllowSorting="True" OnRowDataBound="gw_tareas_RowDataBound"
            OnRowCreated="gw_Tareas_RowCreated" OnDataBound="gw_tareas_DataBound" OnPageIndexChanging="gw_Tareas_PageIndexChanged"
            OnSorting="gw_Tareas_Sorting" RowStyle-VerticalAlign="Top" OnRowCommand="gw_Tareas_RowCommand">
            <Columns>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Left" SortExpression="NivelUrgencia"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:ImageButton ID="btn_Inactivar" CommandArgument='<%# Bind("Id_Proyecto") %>'
                            runat="server" ImageUrl="../../Images/Tareas/Urgencia1.gif" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FechaHora" HeaderText="Fecha" SortExpression="Fecha" HeaderStyle-HorizontalAlign="Center"
                    ItemStyle-Width="19%" />
                <asp:TemplateField SortExpression="NomTareaUsuario" HeaderText="Plan De Negocio">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnk_proyecto" Text='<%# Eval("PlanNegocio") %>' runat="server"
                            CausesValidation="false" CommandName="mostrar_proyecto" Style="text-decoration: none;"
                            Font-Bold="true" CommandArgument='<%# Eval("Id_Proyecto") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="usuarioAgendo" HeaderText="Agendó" SortExpression="usuarioAgendo"
                    HeaderStyle-HorizontalAlign="Center" />
                <asp:TemplateField SortExpression="NomTareaUsuario" HeaderText="Tema" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="hl_Tema" runat="server" Text='<%# Eval("NomTareaUsuario") %>'
                            Style="text-decoration: none;" CausesValidation="false" CommandArgument='<%# Bind("Id_tareaRepeticion") %>'
                            CommandName="mostrar_tarea" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <p style="text-align: right;">
            <asp:Label ID="Lbl_Resultados" CssClass="Indicador" runat="server" /></p>
    </div>
</asp:Content>
