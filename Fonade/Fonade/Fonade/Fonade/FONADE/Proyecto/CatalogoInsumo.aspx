<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogoInsumo.aspx.cs" Inherits="Fonade.FONADE.Convocatoria.CatalogoInsumo"  MasterPageFile="~/Emergente.Master" %>
<%@ Register Src="../../Controles/Alert.ascx" TagName="Alert" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="BodyContent"  runat="server" ContentPlaceHolderID="bodyContentPlace">

    <asp:LinqDataSource ID="lds_cargartxt" runat="server" 
        ContextTypeName="Datos.FonadeDBDataContext" AutoPage="true" 
        onselecting="lds_cargartxt_Selecting" >
    </asp:LinqDataSource>

    <asp:UpdatePanel ID="UP_lista" runat="server" Visible="true" Width="100%" UpdateMode="Conditional">
    <ContentTemplate>
        
        <uc2:Alert ID="Alert1" runat="server" />

            <table width="600px">
              <tr>
                <td class="style13"></td>
                <td class="style15"></td>
              </tr>
              <tr>
                <td class="style16">Nombre:</td>
                <td class="style15">
                    <asp:TextBox ID="txt_nombreinsumo" runat="server" Width="350px"></asp:TextBox>
                  </td>
              </tr>
              <tr>
                <td class="style16">Tipo de Insumo:</td>
                <td class="style15">
                    <asp:DropDownList ID="ddl_insumotipos" runat="server" 
                        Height="24px" Width="201px">
                    </asp:DropDownList>
                  </td>
              </tr>
              <tr>
                <td class="style16">IVA:</td>
                <td class="style15">
                    <asp:TextBox ID="txt_ivainsumo" runat="server" Width="70px"></asp:TextBox>
                  </td>
              </tr>
              <tr>
                <td class="style16">Unidad:</td>
                <td class="style15">
                    <asp:TextBox ID="txt_unidadinsumo" runat="server" Width="130px"></asp:TextBox>
                  </td>
              </tr>
              <tr>
                <td class="style16">Presentación:</td>
                <td class="style15">
                    <asp:TextBox ID="txt_presentacioninsumo" runat="server" Width="350px"></asp:TextBox>
                  </td>
              </tr>
              <tr>
                <td class="style16">% Compras Crédito:</td>
                <td class="style15">
                    <asp:TextBox ID="txt_creditoinsumo" runat="server" Width="70px"></asp:TextBox>
                  </td>
              </tr>
            </table>
            <table width="600px">
               <tr>
                <td>&nbsp;</td>
              </tr>
              <tr>
                <td class="style17" style="text-align: center; background-color:#00468F; ">
                    <strong style="font-size: 12pt">PROYECCIÓN DE VENTAS</strong></td>
              </tr>
              <tr>
                <td>
                
                <asp:GridView ID="gvProyeccionVentas" runat="server" CssClass="Grilla"
            AllowPaging="false" AutoGenerateColumns="false" DataSourceID="lds_cargartxt"
            EmptyDataText="No hay información disponible." Width="100%" 
                    onload="gvProyeccionVentas_Load" BorderStyle="None" 
                        onunload="gvProyeccionVentas_Unload">
                        <Columns>
                            <asp:TemplateField HeaderText="Periodos:">
                                <ItemTemplate>
                                    <asp:Label ID="lb_periodo" runat="server" Text='<%# Eval("Precio") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                
                </td>
              </tr>
              <tr>
                <td align="center">
                    <asp:Button ID="btn_crear" runat="server" onclick="btn_crear_Click" 
                        Text="Crear" Visible="False" />
                    <asp:Button ID="btn_Actualizar" runat="server" onclick="btn_Actualizar_Click" 
                        Text="Actualizar" Visible="False" />
                    &nbsp;
                    <asp:Button ID="btn_iralista" runat="server" Text="Cerrar" 
                        onclick="btn_iralista_Click" />
                  </td>
              </tr>
            </table>

    </ContentTemplate>
    </asp:UpdatePanel>

 </asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .style13
        {
            width: 193px;
            height: 23px;
        }
        .style15
        {
            height: 23px;
        }
        .style16
        {
            width: 193px;
            text-align: right;
            font-weight: bold;
            height: 23px;
        }
        .style17
        {
            height: 28px;
            font-size: 13pt;
            color: #FFFFFF;
        }
        </style>
</asp:Content>
