<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerActaAsignacionRecursos.aspx.cs" Inherits="Fonade.FONADE.Priorizacion_de_Proyectos.VerActaAsignacionRecursos"  MasterPageFile="~/Master.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="BodyContent"  runat="server" ContentPlaceHolderID="bodyContentPlace">

    <asp:LinqDataSource ID="lds_ActaAsignacion" runat="server" 
        ContextTypeName="Datos.FonadeDBDataContext" AutoPage="true" 
        onselecting="lds_ActaAsignacion_Selecting" >
    </asp:LinqDataSource>

    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"> </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="PanelInfo" runat="server" Visible="true" Width="98%" UpdateMode="Conditional">
    <ContentTemplate>

        <table width="100%">
          <tr>
            <td colspan="4">&nbsp;</td>
          </tr>
          <tr>
            <td class="style27">&nbsp;</td>
            <td colspan="2" ><h1><asp:Label runat="server" ID="lbl_Titulo" style="font-weight: 700"></asp:Label></h1></td>
            <td>&nbsp;</td>
          </tr>
          <tr>
            <td class="style27">&nbsp;</td>
            <td class="style25" valign="baseline">Número de Acta:</td>
            <td class="style28" valign="baseline">
                <asp:Label ID="l_numActa" runat="server"></asp:Label>
              </td>
            <td>
                &nbsp;</td>
          </tr>
          <tr>
            <td class="style27">&nbsp;</td>
            <td class="style25" valign="baseline">Nombre del Acta:</td>
            <td class="style28" valign="baseline">
                <asp:Label ID="l_nombreActa" runat="server"></asp:Label>
              </td>
            <td>
                &nbsp;</td>
          </tr>
          <tr>
            <td class="style27">&nbsp;</td>
            <td class="style25" valign="baseline">Fecha del Acta:</td>
            <td class="style28" valign="baseline">
                <asp:Label ID="l_fecha" runat="server"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
          </tr>
          <tr>
            <td class="style27">&nbsp;</td>
            <td class="style25" valign="baseline">Convocatoria:</td>
            <td class="style28" valign="baseline">
                <asp:Label ID="l_convocatoria" runat="server"></asp:Label>
              </td>
            <td>&nbsp;</td>
          </tr>
          <tr>
            <td class="style27"></td>
            <td class="style25" valign="bottom">Observaciones:</td>
            <td class="style28"></td>
            <td class="style19"></td>
          </tr>
          <tr>
            <td class="style27">&nbsp;</td>
            <td colspan="2">
                <asp:TextBox ID="txt_observaciones" runat="server" Height="140px" TextMode="MultiLine" 
                    Width="440px" BackColor="White" ReadOnly="True"></asp:TextBox>
              </td>
            <td>
                &nbsp;</td>
          </tr>
          <tr>
            <td colspan="4">&nbsp;</td>
          </tr>
        </table>
        <table width="100%">
          <tr>
            <td class="style30">&nbsp;</td>
            <td class="style31">
            <asp:GridView ID="gv_ActaAsignacion"  CssClass="Grilla" runat="server" 
            AllowPaging="false" AutoGenerateColumns="false" DataSourceID="lds_ActaAsignacion"
            EmptyDataText="No hay información disponible." Width="100%" 
                    onload="gv_ActaAsignacion_Load">
                <Columns>
                    <asp:TemplateField HeaderText="Id">
                        <ItemTemplate>
                            <asp:Label ID="lId" runat="server" Text='<%# Eval("Id_Proyecto") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Plan de Negocio">
                        <ItemTemplate>
                            <asp:Label ID="LNombre" runat="server" Text='<%# Eval("NomProyecto") %>'></asp:Label>
                        </ItemTemplate> 
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Recursos (SMMLV)" >
                        <ItemTemplate>
                            <asp:HiddenField ID="hValor" runat="server" Value='<%# Eval("valorrecomendado") %>' />
                            <asp:Label ID="lSalarios" runat="server" Text='<%# Eval("salarios") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Legalizado" >
                        <ItemTemplate>
                            <asp:Label ID="lAsignados" runat="server" Text='<%# Eval("Asignado") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            </td>
            <td>&nbsp;</td>
          </tr>
          <tr>
            <td class="style30">&nbsp;</td>
            <td class="style31" align="right"><strong>Total (SMMLV):</strong>&nbsp;<asp:Label 
                    ID="l_totalsalarios" runat="server" Text=""></asp:Label>
              </td>
            <td>&nbsp;</td>
          </tr>
          <tr>
            <td class="style30">&nbsp;</td>
            <td class="style31" align="right"><strong>Total:</strong>&nbsp;<asp:Label 
                    ID="l_Total" runat="server" Text=""></asp:Label></td>
            <td>&nbsp;</td>
          </tr>
          <tr>
            <td class="style30">&nbsp;</td>
            <td class="style31" align="center">
                <asp:Button ID="btn_Imprimir" runat="server" Text="Imprimir" 
                    onclick="btn_Imprimir_Click" />
              </td>
            <td>&nbsp;</td>
          </tr>
        </table>

    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .style19
        {
            width: 131px;
        }
        .style25
        {
            width: 152px;
            font-weight: bold;
        }
        .style27
        {
            width: 37px;
        }
        .style28
        {
            width: 721px;
        }
        .style30
        {
            width: 22px;
        }
        .style31
        {
            width: 693px;
        }
    </style>
</asp:Content>
