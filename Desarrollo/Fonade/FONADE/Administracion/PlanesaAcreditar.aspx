<%@ Page Language="C#" MasterPageFile="~/Master.master"  EnableEventValidation="false" AutoEventWireup="true" CodeBehind="PlanesaAcreditar.aspx.cs" Inherits="Fonade.FONADE.Administracion.PlanesaAcreditar" %>

 <asp:Content  ID="head1"   ContentPlaceHolderID="head" runat="server">
     <style type="text/css">
         table {
             width:100%;
         }
         td {
             vertical-align:top;
         }
     </style>
    </asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="bodyContentPlace">
    <h1>
        <label>PLANES DE NEGOCIO A ACREDITAR</label>
    </h1>
    <br />
    <br />
    <asp:GridView ID="gvplanes" runat="server" CssClass="Grilla" AutoGenerateColumns="False" Width="100%" AllowSorting="True" OnRowCreated="gvplanes_RowCreated" DataSourceID="LinqDataSource1" OnRowCommand="gvplanes_RowCommand">
        <Columns>
            <asp:TemplateField HeaderText="Nombre" SortExpression="NOMPROYECTO">
                <ItemTemplate>
                    <asp:Button ID="btnframeset" runat="server" Text='<%# Eval("ID_PROYECTO") + " - " + Eval("NOMPROYECTO") %>' CommandArgument='<%# Eval("ID_PROYECTO") %>' CommandName="frameset" CssClass="boton_Link_Grid" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Fecha de Asignación" DataField="FECHAASIGNACION" SortExpression="FECHAASIGNACION" />
            <asp:BoundField HeaderText="Días transcurridos asignación" DataField="DIAS" SortExpression="DIAS" />
            <asp:BoundField HeaderText="Estado" DataField="ESTADO" SortExpression="ESTADO" />
            <asp:TemplateField HeaderText="Acreditar">
                <ItemTemplate>
                    <asp:Button ID="btnacreditar" runat="server" Text="Acreditar" CommandArgument='<%# Eval("ID_PROYECTO") + ";" + Eval("CODCONVOCATORIA") %>' CommandName="proyectoacreditar" CssClass="boton_Link_Grid" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:LinqDataSource ID="LinqDataSource1" runat="server" ContextTypeName="Datos.FonadeDBDataContext" AutoPage="false" OnSelecting="LinqDataSource1_Selecting">
    </asp:LinqDataSource>
</asp:Content>