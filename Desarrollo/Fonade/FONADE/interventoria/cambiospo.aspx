<%@ Page Language="C#" MasterPageFile="~/Master.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeBehind="cambiospo.aspx.cs" Inherits="Fonade.FONADE.interventoria.cambiospo" %>

<asp:Content ID="head1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        table
        {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <asp:ScriptManager ID="scrptMng_PO" runat="server" />
    <asp:UpdatePanel ID="updt_cambiosPO" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlprincipal" runat="server">
                <h1>
                    <label>
                        CAMBIOS A PLANES OPERATIVOS</label>
                </h1>
                <br />
                <br />
                <table>
                    <tr>
                        <td colspan="2">
                            Filtrar:&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlfiltro" runat="server" Width="100px"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlfiltro_SelectedIndexChanged">
                                <asp:ListItem Value="">Seleccione...</asp:ListItem>
                                <asp:ListItem Value="Borrar">Borrar</asp:ListItem>
                                <asp:ListItem Value="Adicionar">Adicionar</asp:ListItem>
                                <asp:ListItem Value="Modificar">Modificar</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chectodos" runat="server" Text="Todos" AutoPostBack="true" OnCheckedChanged="chectodos_CheckedChanged" />
                        </td>
                        <td style="text-align: right;">
                            <asp:LinkButton ID="lnkelemeir" runat="server" Text="Ir para los elementos seleccionados"
                                OnClientClick="alert('Acciones aplicadas satisfactoriamente')" />
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <br />
                <table class="Grilla">
                    <thead>
                        <tr>
                            <th style="text-align: center;">
                                ACTIVIDADES DEL PLAN OPERATIVO
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:GridView ID="gvactividadesplanoperativo" runat="server" CssClass="Grilla" Width="100%"
                                    AutoGenerateColumns="False" EmptyDataText="No existen actividades en plan operativo"
                                    DataKeyNames="Tarea" OnRowCommand="gvactividadesplanoperativo_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chckplanopera" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Id Proyecto" DataField="CodProyecto" />
                                        <asp:BoundField HeaderText="Actividad" DataField="NomActividad" />
                                        <asp:BoundField HeaderText="Tipo de Solicitud" DataField="Tarea" />
                                        <asp:BoundField HeaderText="Empresa" DataField="RazonSocial" />
                                        <asp:TemplateField HeaderText="Interventor">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Nombres") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Nombres") + " " + Eval("Apellidos") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Button ID="btnplanoperativo" runat="server" Text="Ir" CommandArgument='<%# "Editar" + ";" + Eval("CodProyecto") + ";" + Eval("Id_Actividad") %>'
                                                    CommandName="ActividadPOEdditar" CssClass="boton_Link_Grid" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <br />
                <br />
                <table class="Grilla">
                    <thead>
                        <tr>
                            <th style="text-align: center;">
                                CARGOS DE NOMINA
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:GridView ID="gvcargosnomina" runat="server" CssClass="Grilla" Width="100%" AutoGenerateColumns="False"
                                    EmptyDataText="No existen cargos de nomina" DataKeyNames="Tarea" OnRowCommand="gvcargosnomina_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chcknomina" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Id Proyecto" DataField="CodProyecto" />
                                        <asp:BoundField HeaderText="Cargo" DataField="Cargo" />
                                        <asp:BoundField HeaderText="Tipo de Solicitud" DataField="Tarea" />
                                        <asp:BoundField HeaderText="Empresa" DataField="RazonSocial" />
                                        <asp:TemplateField HeaderText="Interventor">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Nombres") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Nombres") + " " + Eval("Apellidos") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Button ID="btnplanoperativo" runat="server" Text="Ir" CommandArgument='<%# "Editar" + ";" + Eval("CodProyecto") + ";" + Eval("Id_Nomina") %>'
                                                    CommandName="CargosNomina" CssClass="boton_Link_Grid" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <br />
                <br />
                <table class="Grilla">
                    <thead>
                        <tr>
                            <th style="text-align: center;">
                                PRODUCTOS EN PRODUCCIÓN
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:GridView ID="gvproductosproduccion" runat="server" CssClass="Grilla" Width="100%"
                                    AutoGenerateColumns="False" EmptyDataText="No hay productos en producción" DataKeyNames="Tarea"
                                    OnRowCommand="gvproductosproduccion_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chckproduccion" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Id Proyecto" DataField="CodProyecto" />
                                        <asp:BoundField HeaderText="Producto" DataField="NomProducto" />
                                        <asp:BoundField HeaderText="Tipo de Solicitud" DataField="Tarea" />
                                        <asp:BoundField HeaderText="Empresa" DataField="RazonSocial" />
                                        <asp:TemplateField HeaderText="Interventor">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Nombres") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Nombres") + " " + Eval("Apellidos") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Button ID="btnplanoperativo" runat="server" Text="Ir" CommandArgument='<%# "Editar" + ";" + Eval("CodProyecto") + ";" + Eval("Id_Produccion") %>'
                                                    CommandName="productosProduccion" CssClass="boton_Link_Grid" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <br />
                <br />
                <table class="Grilla">
                    <thead>
                        <tr>
                            <th style="text-align: center;">
                                PRODUCTOS EN VENTAS
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <asp:GridView ID="gvproductosventas" runat="server" CssClass="Grilla" Width="100%"
                                    AutoGenerateColumns="False" EmptyDataText="No hay productos en ventas" DataKeyNames="Tarea"
                                    OnRowCommand="gvproductosventas_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chckventas" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Id Proyecto" DataField="CodProyecto" />
                                        <asp:BoundField HeaderText="Producto" DataField="NomProducto" />
                                        <asp:BoundField HeaderText="Tipo de Solicitud" DataField="Tarea" />
                                        <asp:BoundField HeaderText="Empresa" DataField="RazonSocial" />
                                        <asp:TemplateField HeaderText="Interventor">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Nombres") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Nombres") + " " + Eval("Apellidos") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Button ID="btnplanoperativo" runat="server" Text="Ir" CommandArgument='<%# "Editar" + ";" + Eval("CodProyecto") + ";" + Eval("Id_Ventas") %>'
                                                    CommandName="productosVentas" CssClass="boton_Link_Grid" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
