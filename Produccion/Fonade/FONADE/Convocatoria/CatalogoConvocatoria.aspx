<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoConvocatoria.aspx.cs" Inherits="Fonade.FONADE.Convocatoria.CatalogoConvocatoria1" MasterPageFile="~/Master.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="BodyContent"  runat="server" ContentPlaceHolderID="bodyContentPlace">

    <asp:LinqDataSource ID="lds_listadoConvoct" runat="server" 
    ContextTypeName="Datos.FonadeDBDataContext" AutoPage="false" 
    onselecting="lds_listadoConvoct_Selecting" >
    </asp:LinqDataSource>

    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"> </asp:ToolkitScriptManager>

        <table width="100%">
          <tr>
            <td>
                <h1><asp:Label runat="server" ID="lbl_Titulo" style="font-weight: 700"></asp:Label></h1>
            </td>
          </tr>
          <tr>
            <td class="style10">
                
                <asp:ImageButton ID="ibtn_crearConvoct" runat="server" ImageAlign="AbsBottom" 
                    ImageUrl="~/Images/add.png" />
                <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click">Crear Convocatoria</asp:LinkButton>
                
            </td>
          </tr>
          <tr>
            <td>
                <asp:GridView ID="GridViewConvoct"  CssClass="Grilla" runat="server"  AllowSorting="True"
                    AutoGenerateColumns="false" DataSourceID="lds_listadoConvoct" AllowPaging="true"
                    EmptyDataText="No hay información disponible." Width="100%" 
                    onrowcommand="GridViewConvoct_RowCommand" >
        
                    <Columns>
                        <asp:TemplateField HeaderText="Nombre"  SortExpression="nomConvocatoria">
                            <ItemTemplate>
                                <asp:Button ID="hl_numacta" Text='<%# Eval("nomConvocatoria") %>' CommandArgument='<%# Eval("id_convocatoria") %>' CommandName="VerConvocatoria" CssClass="boton_Link_Grid" runat="server"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fecha Inicio" SortExpression="FechaInicio">
                            <ItemTemplate>
                                <asp:Label ID="hl_finicio" runat="server" Text='<%# Eval("FInicio") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fecha Fin" SortExpression="FechaFin">
                            <ItemTemplate>
                                <asp:Label ID="hl_ffin" runat="server" Text='<%# Eval("FFin") %>'></asp:Label>
                            </ItemTemplate> 
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Publicado" SortExpression="Publicado" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>                    
                                <asp:ImageButton ID="btnCheck" CommandArgument='<%# Eval("id_convocatoria") %>'  Visible='<%# !((int)Eval("Publicado") == 0) %>' CommandName="VerProyectosConvatoria" runat="server" ImageUrl="~/Images/check.png" />
                            </ItemTemplate>
                        </asp:TemplateField> 
                        <asp:TemplateField HeaderText="Aspectos Evaluados" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Button ID="btnconvocatoria" runat="server" Text="Ver" CommandArgument='<%# Eval("id_convocatoria") %>' CommandName="VerEvalConvatoria" CssClass="boton_Link_Grid" />
                            </ItemTemplate> 
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
          </tr>
        </table>

</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .style10
        {
            height: 30px;
        }
    </style>
</asp:Content>

